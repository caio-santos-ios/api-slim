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
    public class AccreditedNetworkRepository(AppDbContext context) : IAccreditedNetworkRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<AccreditedNetwork> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),
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

                List<BsonDocument> results = await context.AccreditedNetworks.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Items");
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
                        { "from", "addresses" },

                        { "let", new BsonDocument("id", new BsonDocument("$toString", "$_id")) },

                        { "pipeline", new BsonArray
                            {
                                new BsonDocument("$match", new BsonDocument
                                {
                                    { "$expr", new BsonDocument("$and", new BsonArray
                                        {
                                            new BsonDocument("$eq", new BsonArray
                                            {
                                                "$parentId",
                                                "$$id"
                                            }),

                                            new BsonDocument("$eq", new BsonArray
                                            {
                                                "$parent",
                                                "accredited-network"
                                            })
                                        })
                                    }
                                })
                            }
                        },

                        { "as", "_address" }
                    }),
                    
                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "addresses" },

                        { "let", new BsonDocument("id", new BsonDocument("$toString", "$_id")) },

                        { "pipeline", new BsonArray
                            {
                                new BsonDocument("$match", new BsonDocument
                                {
                                    { "$expr", new BsonDocument("$and", new BsonArray
                                        {
                                            new BsonDocument("$eq", new BsonArray
                                            {
                                                "$parentId",
                                                "$$id"
                                            }),

                                            new BsonDocument("$eq", new BsonArray
                                            {
                                                "$parent",
                                                "accredited-network-responsible"
                                            })
                                        })
                                    }
                                })
                            }
                        },

                        { "as", "_address_responsible" }
                    }),
                    
                    new("$addFields", new BsonDocument {
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"address", new BsonDocument
                            {
                                {"id", new BsonDocument("$toString", new BsonDocument("$first", "$_address._id"))},
                                {"street", new BsonDocument("$first", "$_address.street")},
                                {"number", new BsonDocument("$first", "$_address.number")},
                                {"complement", new BsonDocument("$first", "$_address.complement")},
                                {"neighborhood", new BsonDocument("$first", "$_address.neighborhood")},
                                {"city", new BsonDocument("$first", "$_address.city")},
                                {"state", new BsonDocument("$first", "$_address.state")},
                                {"zipCode", new BsonDocument("$first", "$_address.zipCode")},
                                {"parent", new BsonDocument("$first", "$_address.parent")},
                                {"parentId", new BsonDocument("$first", "$_address.parentId")},
                            }
                        },
                        {
                            "responsible.address", new BsonDocument {
                                {"id", new BsonDocument("$toString", new BsonDocument("$first", "$_address_responsible._id"))},
                                {"street", new BsonDocument("$first", "$_address_responsible.street")},
                                {"number", new BsonDocument("$first", "$_address_responsible.number")},
                                {"complement", new BsonDocument("$first", "$_address_responsible.complement")},
                                {"neighborhood", new BsonDocument("$first", "$_address_responsible.neighborhood")},
                                {"city", new BsonDocument("$first", "$_address_responsible.city")},
                                {"state", new BsonDocument("$first", "$_address_responsible.state")},
                                {"zipCode", new BsonDocument("$first", "$_address_responsible.zipCode")},
                                {"parent", new BsonDocument("$first", "$_address_responsible.parent")},
                                {"parentId", new BsonDocument("$first", "$_address_responsible.parentId")},
                            }
                        }
                    }),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                    }),
                ];

                BsonDocument? response = await context.AccreditedNetworks.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Item não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Item");
            }
        }
        
        public async Task<ResponseApi<AccreditedNetwork?>> GetByIdAsync(string id)
        {
            try
            {
                AccreditedNetwork? billing = await context.AccreditedNetworks.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(billing);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Item");
            }
        }

        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<AccreditedNetwork> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),

                    new("$addFields", new BsonDocument {
                        {"id", new BsonDocument("$toString", "$_id")},
                    }),

                    MongoUtil.Lookup("accredited_networks", ["$accreditedNetworkId"], ["$id"], "_accredited_network", [["deleted", false]], 1),
                    MongoUtil.Lookup("addresses", ["$id"], ["$parentId"], "_address", [["deleted", false]], 1),

                    new("$addFields", new BsonDocument {
                        {"tradingTableId", MongoUtil.First("_accredited_network.tradingTable")},
                    }),

                    MongoUtil.Lookup("trading_tables", ["$tradingTableId"], ["$_id"], "_trading_table", [["deleted", false]], 1),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0}, 
                        {"id", 1},
                        {"corporateName", 1},
                        {"tradingTableItems", MongoUtil.First("_trading_table.items")},
                        {"address", MongoUtil.First("_address")},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.AccreditedNetworks.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Items");
            }
        }
        
        public async Task<ResponseApi<long?>> GetNextCodeAsync()
        {
            try
            {
                long code = await context.AccreditedNetworks.Find(x => true).CountDocumentsAsync() + 1;
                return new(code);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Próximo código");
            }
        }
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<AccreditedNetwork> pagination)
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

            List<BsonDocument> results = await context.AccreditedNetworks.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<AccreditedNetwork?>> CreateAsync(AccreditedNetwork billing)
        {
            try
            {
                await context.AccreditedNetworks.InsertOneAsync(billing);

                return new(billing, 201, "Item criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao criar Item");  
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<AccreditedNetwork?>> UpdateAsync(AccreditedNetwork billing)
        {
            try
            {
                await context.AccreditedNetworks.ReplaceOneAsync(x => x.Id == billing.Id, billing);

                return new(billing, 201, "Item atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao atualizar Item");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<AccreditedNetwork>> DeleteAsync(string id)
        {
            try
            {
                AccreditedNetwork? billing = await context.AccreditedNetworks.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if(billing is null) return new(null, 404, "Item não encontrado");
                billing.Deleted = true;
                billing.DeletedAt = DateTime.UtcNow;

                await context.AccreditedNetworks.ReplaceOneAsync(x => x.Id == id, billing);

                return new(billing, 204, "Item excluído com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao excluír Item");
            }
        }
        #endregion
    }
}