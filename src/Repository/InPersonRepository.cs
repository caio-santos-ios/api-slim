using api_slim.src.Configuration;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace api_slim.src.Repository
{
    public class InPersonRepository(AppDbContext context) : IInPersonRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<InPerson> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),
                    
                    MongoUtil.Lookup("customer_recipients", ["$recipientId"], ["$_id"], "_recipient", [["deleted", false]], 1),
                    MongoUtil.Lookup("accredited_networks", ["$accreditedNetworkId"], ["$_id"], "_accredited_network", [["deleted", false]], 1),
                    MongoUtil.Lookup("service_modules", ["$serviceModuleId"], ["$_id"], "_service_module", [["deleted", false]], 1),
                    MongoUtil.Lookup("professionals", ["$professionalId"], ["$_id"], "_professional", [["deleted", false]], 1),
                    new("$addFields", new BsonDocument {
                        {"accreditedNetworkId",  MongoUtil.First("_accredited_network._id")}
                    }),
                    new("$addFields", new BsonDocument {
                        {"strAccreditedNetworkId",  MongoUtil.ToString("$accreditedNetworkId")}
                    }),
                    MongoUtil.Lookup("professionals", ["$professionalId"], ["$_id"], "_professional", [["deleted", false]], 1),
                    MongoUtil.Lookup("trading_tables", ["$strAccreditedNetworkId"], ["$accreditedNetworkId"], "_tradingTables", [["deleted", false]], 1),
                    new("$lookup", new BsonDocument
                    {
                        { "from", "procedures" },
                        { "let", new BsonDocument("pIds", "$procedureIds") },
                        { "pipeline", new BsonArray
                            {
                                // 1. Filtra os procedimentos comparando strings
                                new BsonDocument("$match", new BsonDocument("$expr", 
                                    new BsonDocument("$in", new BsonArray 
                                    { 
                                        new BsonDocument("$toString", "$_id"), // Converte ObjectId p/ String
                                        "$$pIds"                               
                                    })
                                )),
                                // 2. Opcional: Já deixa o 'id' pronto como string dentro de cada procedimento
                                new BsonDocument("$addFields", new BsonDocument
                                {
                                    { "id", new BsonDocument("$toString", "$_id") }
                                })
                            }
                        },
                        { "as", "proceduresDetails" }
                    }),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0}, 
                        {"id", MongoUtil.ToString("$_id")},
                        {"createdAt", 1},
                        {"date", 1},
                        {"responsiblePayment", 1},
                        {"status", 1},
                        {"value", 1},
                        {"hour", 1},
                        {"accreditedNetworkDescription", MongoUtil.First("_accredited_network.corporateName")},
                        {"strAccreditedNetworkId", 1},
                        {"proceduresItem", MongoUtil.First("_accredited_network.items")},
                        {"serviceModuleDescription", MongoUtil.First("_service_module.name")},
                        {"recipientDescription", MongoUtil.First("_recipient.name")},
                        {"recipientCpf", MongoUtil.First("_recipient.cpf")},
                        {"professionalName", MongoUtil.First("_professional.name")},
                        {"procedures", "$proceduresDetails"},
                        {"tradingTables", MongoUtil.First("_tradingTables.items")}
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.InPersons.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        
        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                BsonDocument[] pipeline = [
                    new("$match", new BsonDocument{
                        {"_id", new ObjectId(id)},
                        {"deleted", false}
                    }),

                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "procedures" },
                        { "let", new BsonDocument("idsDoContrato", "$procedureIds") }, 
                        { "pipeline", new BsonArray
                            {
                                new BsonDocument("$match", new BsonDocument
                                {
                                    { "$expr", new BsonDocument("$and", new BsonArray 
                                        {
                                            new BsonDocument("$gt", new BsonArray { new BsonDocument("$size", new BsonDocument("$ifNull", new BsonArray { "$$idsDoContrato", new BsonArray() })), 0 }),
                                            
                                            new BsonDocument("$in", new BsonArray 
                                            { 
                                                new BsonDocument("$toString", "$_id"), 
                                                "$$idsDoContrato" 
                                            })
                                        })
                                    }
                                })
                            }
                        },
                        { "as", "_procedures" }
                    }),
                    new("$addFields", new BsonDocument {
                        {"id", new BsonDocument("$toString", "$_id")},    
                        {"procedureIds", "$_procedures"}
                    }),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                    }),
                ];

                BsonDocument? response = await context.InPersons.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Atendimento Presencial não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        
        public async Task<ResponseApi<InPerson?>> GetByIdAsync(string id)
        {
            try
            {
                InPerson? inPerson = await context.InPersons.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(inPerson);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<InPerson> pagination)
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$addFields", new BsonDocument
                {
                    {"id", new BsonDocument("$toString", "$_id")},
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.InPersons.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<InPerson?>> CreateAsync(InPerson inPerson)
        {
            try
            {
                await context.InPersons.InsertOneAsync(inPerson);

                return new(inPerson, 201, "Atendimento Presencial criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");  
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<InPerson?>> UpdateAsync(InPerson inPerson)
        {
            try
            {
                await context.InPersons.ReplaceOneAsync(x => x.Id == inPerson.Id, inPerson);

                return new(inPerson, 201, "Atendimento Presencial atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<InPerson>> DeleteAsync(string id)
        {
            try
            {
                InPerson? inPerson = await context.InPersons.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if(inPerson is null) return new(null, 404, "Atendimento Presencial não encontrado");
                inPerson.Deleted = true;
                inPerson.DeletedAt = DateTime.UtcNow;

                await context.InPersons.ReplaceOneAsync(x => x.Id == id, inPerson);

                return new(inPerson, 204, "Atendimento Presencial excluído com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}