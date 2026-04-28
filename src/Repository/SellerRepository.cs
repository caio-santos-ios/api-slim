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
    public class SellerRepository(AppDbContext context) : ISellerRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Seller> pagination)
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

                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "addresses" },

                    { "let", new BsonDocument("profId", "$id") },

                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match", new BsonDocument
                            {
                                { "$expr",
                                    new BsonDocument("$eq",
                                        new BsonArray
                                        {
                                            // new BsonDocument("$toObjectId", "$parentId"),
                                            "$parentId",
                                            "$$profId"
                                        }
                                    )
                                }
                            })
                        }
                    },

                    { "as", "_address" }
                }),

                new BsonDocument("$unwind", "$_address"),

                new("$addFields", new BsonDocument
                {
                    {"address", new BsonDocument
                        {
                            {"id", new BsonDocument("$toString", "$_address._id")},
                            {"street", new BsonDocument("$ifNull", new BsonArray { "$_address.street", "" })},
                            {"number", new BsonDocument("$ifNull", new BsonArray {"$_address.number" , "" })},
                            {"complement", new BsonDocument("$ifNull", new BsonArray {"$_address.complement" , "" })},
                            {"neighborhood", new BsonDocument("$ifNull", new BsonArray {"$_address.neighborhood" , "" })},
                            {"city", new BsonDocument("$ifNull", new BsonArray {"$_address.city" , "" })},
                            {"state", new BsonDocument("$ifNull", new BsonArray {"$_address.state" , "" })},
                            {"zipCode", new BsonDocument("$ifNull", new BsonArray {"$_address.zipCode" , "" })},
                            {"parent", new BsonDocument("$ifNull", new BsonArray {"$_address.parent" , "" })},
                            {"parentId", new BsonDocument("$ifNull", new BsonArray {"$_address.parentId" , "" })},
                        }
                    },
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                    {"_address", 0},
                }),
                new("$sort", pagination.PipelineSort),
            };

                List<BsonDocument> results = await context.Sellers.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Vendedores");
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

                    MongoUtil.LookupV2("addresses", ["$_id"], ["$parentId"], "_address", [["deleted", false], ["parent", "seller"]], 1),

                    new("$addFields", new BsonDocument
                    {
                        {"addressId", MongoUtil.First("_address._id")},
                    }),

                    new("$addFields", new BsonDocument
                    {
                        {"id", MongoUtil.ToString("$_id")},
                        {"address", new BsonDocument
                            {
                                {"id", MongoUtil.ToString("$addressId")},
                                {"street", MongoUtil.First("_address.street")},
                                {"number", MongoUtil.First("_address.number")},
                                {"complement", MongoUtil.First("_address.complement")},
                                {"neighborhood", MongoUtil.First("_address.neighborhood")},
                                {"city", MongoUtil.First("_address.city")},
                                {"state", MongoUtil.First("_address.state")},
                                {"zipCode", MongoUtil.First("_address.zipCode")},
                                {"parent", MongoUtil.First("_address.parent")},
                                {"parentId", MongoUtil.First("_address.parentId")},
                            }
                        },
                    }),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"_address", 0},
                    }),
                ];

                BsonDocument? response = await context.Sellers.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Vendedor não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Vendedor");
            }
        }

        public async Task<ResponseApi<Seller?>> GetByIdAsync(string id)
        {
            try
            {
                Seller? seller = await context.Sellers.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(seller);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Vendedor");
            }
        }

        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Seller> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$addFields", new BsonDocument
                {
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                    {"id", MongoUtil.ToString("$_id")},
                    {"name", 1},
                    {"createdAt", 1},
                    {"deleted", 1},
                }),
                new("$sort", pagination.PipelineSort),
            };

                List<BsonDocument> results = await context.Sellers.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Vendedores");
            }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Seller> pagination)
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

            List<BsonDocument> results = await context.Sellers.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<Seller?>> CreateAsync(Seller seller)
        {
            try
            {
                await context.Sellers.InsertOneAsync(seller);

                return new(seller, 201, "Vendedor criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao criar Vendedor");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<Seller?>> UpdateAsync(Seller seller)
        {
            try
            {
                await context.Sellers.ReplaceOneAsync(x => x.Id == seller.Id, seller);

                return new(seller, 201, "Vendedor atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao atualizar Vendedor");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<Seller>> DeleteAsync(string id)
        {
            try
            {
                Seller? seller = await context.Sellers.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if (seller is null) return new(null, 404, "Vendedor não encontrado");
                seller.Deleted = true;
                seller.DeletedAt = DateTime.UtcNow;

                await context.Sellers.ReplaceOneAsync(x => x.Id == id, seller);

                return new(seller, 204, "Vendedor excluído com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao excluír Vendedor");
            }
        }
        #endregion
    }
}