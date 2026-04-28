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
    public class SupplierRepository(AppDbContext context) : ISupplierRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Supplier> pagination)
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

                List<BsonDocument> results = await context.Suppliers.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Fornecedores");
            }
        }

        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                BsonDocument[] pipeline = [
                    new("$match", new BsonDocument
                    {
                        {"_id", new ObjectId(id)},
                        {"deleted", false}
                    }),

                    MongoUtil.LookupV2("addresses", ["$_id"], ["$parentId"], "_address", [["deleted", false], ["parent", "supplier"]], 1),

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

                BsonDocument? response = await context.Suppliers.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Fornecedor não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Fornecedor");
            }
        }

        public async Task<ResponseApi<Supplier?>> GetByIdAsync(string id)
        {
            try
            {
                Supplier? billing = await context.Suppliers.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(billing);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Fornecedor");
            }
        }

        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Supplier> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"tradeName", 1},
                        {"corporateName", 1},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Suppliers.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Fornecedores");
            }
        }
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Supplier> pagination)
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

            List<BsonDocument> results = await context.Suppliers.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<Supplier?>> CreateAsync(Supplier billing)
        {
            try
            {
                await context.Suppliers.InsertOneAsync(billing);

                return new(billing, 201, "Fornecedor criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao criar Fornecedor");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<Supplier?>> UpdateAsync(Supplier billing)
        {
            try
            {
                await context.Suppliers.ReplaceOneAsync(x => x.Id == billing.Id, billing);

                return new(billing, 201, "Fornecedor atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao atualizar Fornecedor");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<Supplier>> DeleteAsync(string id)
        {
            try
            {
                Supplier? billing = await context.Suppliers.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if (billing is null) return new(null, 404, "Fornecedor não encontrado");
                billing.Deleted = true;
                billing.DeletedAt = DateTime.UtcNow;

                await context.Suppliers.ReplaceOneAsync(x => x.Id == id, billing);

                return new(billing, 204, "Fornecedor excluído com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao excluír Fornecedor");
            }
        }
        #endregion
    }
}