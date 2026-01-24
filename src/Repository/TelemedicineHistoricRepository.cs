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
    public class TelemedicineHistoricRepository(AppDbContext context) : ITelemedicineHistoricRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<TelemedicineHistoric> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    // new("$skip", pagination.Skip),
                    // new("$limit", pagination.Limit),
                    MongoUtil.Lookup("users", ["$createdBy"], ["$_id"], "_user", [["deleted", false]], 1),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0}, 
                        {"id", MongoUtil.ToString("$_id")},
                        {"createdAt", 1},
                        {"date", 1},
                        {"recipientName", 1},
                        {"specialistName", 1},
                        {"status", 1},
                        {"time", 1},
                        {"userName", MongoUtil.First("_user.name")},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.TelemedicineHistorics.Aggregate<BsonDocument>(pipeline).ToListAsync();
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

                BsonDocument? response = await context.TelemedicineHistorics.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Atendimento Presencial não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        
        public async Task<ResponseApi<TelemedicineHistoric?>> GetByIdAsync(string id)
        {
            try
            {
                TelemedicineHistoric? inPerson = await context.TelemedicineHistorics.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(inPerson);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<TelemedicineHistoric> pagination)
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

            List<BsonDocument> results = await context.TelemedicineHistorics.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<TelemedicineHistoric?>> CreateAsync(TelemedicineHistoric inPerson)
        {
            try
            {
                await context.TelemedicineHistorics.InsertOneAsync(inPerson);

                return new(inPerson, 201, "Atendimento Presencial criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");  
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<TelemedicineHistoric?>> UpdateAsync(TelemedicineHistoric inPerson)
        {
            try
            {
                await context.TelemedicineHistorics.ReplaceOneAsync(x => x.Id == inPerson.Id, inPerson);

                return new(inPerson, 201, "Atendimento Presencial atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<TelemedicineHistoric>> DeleteAsync(string id)
        {
            try
            {
                TelemedicineHistoric? inPerson = await context.TelemedicineHistorics.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if(inPerson is null) return new(null, 404, "Atendimento Presencial não encontrado");
                inPerson.Deleted = true;
                inPerson.DeletedAt = DateTime.UtcNow;

                await context.TelemedicineHistorics.ReplaceOneAsync(x => x.Id == id, inPerson);

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