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
    public class GenericTableRepository(AppDbContext context) : IGenericTableRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<GenericTable> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    // new("$skip", pagination.Skip),
                    // new("$limit", pagination.Limit),
                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")},
                    }),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},                 
                        { "table", 1 },          
                        { "id", 1 },
                        { "code", 1 },
                        { "description", 1 },
                        { "active", 1 },
                        { "createdAt", 1 },
                        { "updatedAt", 1 }    
                    }),

                    new BsonDocument("$group", new BsonDocument
                    {
                        { "_id", "$table" },
                        { "items", new BsonDocument("$push", new BsonDocument
                            {
                                { "id", "$id" },
                                { "code", "$code" },
                                { "description", "$description" },
                                { "active", "$active" },
                                { "createdAt", "$createdAt" },
                                { "updatedAt", "$updatedAt" }
                            })
                        }
                    }),

                    new BsonDocument("$project", new BsonDocument
                    {
                        { "_id", 0 },
                        { "table", "$_id" },
                        { "items", 1 }
                    }),

                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.GenericTables.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Tabela Genérica"); ;
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
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"code", 1},
                        {"description", 1}
                    }),
                ];

                BsonDocument? response = await context.GenericTables.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Tabela Genérica não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Tabela Genérica"); ;
            }
        }
        public async Task<ResponseApi<GenericTable?>> GetByIdAsync(string id)
        {
            try
            {
                GenericTable? genericTable = await context.GenericTables.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(genericTable);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Tabela Genérica"); ;
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetByTableAggregateAsync(string table)
        {
            try
            {
                BsonDocument[] pipeline = [
                    new("$match", new BsonDocument{
                        {"table", table},
                        {"active", true},
                        {"deleted", false}
                    }),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"code", 1},
                        {"description", 1},
                        {"active", 1},
                        {"table", 1},
                        {"deleted", 1}
                    }),
                ];

                List<BsonDocument> results = await context.GenericTables.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Tabela Genérica"); ;
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetByTableAllAggregateAsync(string table)
        {
            try
            {
                BsonDocument[] pipeline = [
                    new("$match", new BsonDocument{
                        {"table", table},
                        {"active", true}
                    }),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"code", 1},
                        {"description", 1},
                        {"active", 1},
                        {"table", 1},
                        {"deleted", 1}
                    }),
                ];

                List<BsonDocument> results = await context.GenericTables.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Tabela Genérica"); ;
            }
        }
        public async Task<ResponseApi<GenericTable?>> GetByCodeAsync(string code, string table, string? id = null)
        {
            try
            {
                if(id is not null)
                {
                    GenericTable? genericTableExistent = await context.GenericTables.Find(x => x.Code == code && x.Table == table && x.Id != id && !x.Deleted).FirstOrDefaultAsync();
                    return new(genericTableExistent);
                }
                
                GenericTable? genericTable = await context.GenericTables.Find(x => x.Code == code  && x.Table == table && !x.Deleted).FirstOrDefaultAsync();
                return new(genericTable);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Tabela Genérica"); ;
            }
        }
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<GenericTable> pagination)
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
                        { "table", 1 },          
                        { "id", 1 },
                        { "code", 1 },
                        { "description", 1 },
                        { "active", 1 },
                        { "createdAt", 1 },
                        { "updatedAt", 1 }    
                    }),

                    new BsonDocument("$group", new BsonDocument
                    {
                        { "_id", "$table" },
                        { "items", new BsonDocument("$push", new BsonDocument
                            {
                                { "id", "$id" },
                                { "code", "$code" },
                                { "description", "$description" },
                                { "active", "$active" },
                                { "createdAt", "$createdAt" },
                                { "updatedAt", "$updatedAt" }
                            })
                        }
                    }),

                    new BsonDocument("$project", new BsonDocument
                    {
                        { "_id", 0 },
                        { "table", "$_id" },
                        { "items", 1 }
                    }),

                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.GenericTables.Aggregate<BsonDocument>(pipeline).ToListAsync();
                return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        #region CREATE
        public async Task<ResponseApi<GenericTable?>> CreateAsync(GenericTable genericTable)
        {
            try
            {
                await context.GenericTables.InsertOneAsync(genericTable);

                return new(genericTable, 201, "Tabela Genérica criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao criar Tabela Genérica");   
            }
        }
        #endregion
        #region UPDATE
        public async Task<ResponseApi<GenericTable?>> UpdateAsync(GenericTable genericTable)
        {
            try
            {
                await context.GenericTables.ReplaceOneAsync(x => x.Id == genericTable.Id, genericTable);

                return new(genericTable, 201, "Tabela Genérica atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao atualizar Tabela Genérica");
            }
        }
        #endregion
        #region DELETE
        public async Task<ResponseApi<GenericTable>> DeleteAsync(string userId)
        {
            try
            {
                GenericTable? genericTable = await context.GenericTables.Find(x => x.Id == userId && !x.Deleted).FirstOrDefaultAsync();
                if(genericTable is null) return new(null, 404, "Tabela Genérica não encontrado");
                genericTable.Deleted = true;
                genericTable.DeletedAt = DateTime.UtcNow;

                await context.GenericTables.ReplaceOneAsync(x => x.Id == userId, genericTable);

                return new(genericTable, 204, "Tabela Genérica excluído com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao excluír Tabela Genérica");
            }
        }
        public async Task<ResponseApi<GenericTable>> DeleteByTableAsync(string table)
        {
            try
            {
                List<GenericTable> genericTables = await context.GenericTables.Find(x => x.Table == table && !x.Deleted).ToListAsync();
                foreach (GenericTable genericTable in genericTables)
                {
                    genericTable.Deleted = true;
                    genericTable.DeletedAt = DateTime.UtcNow;

                    await context.GenericTables.ReplaceOneAsync(x => x.Id == genericTable.Id, genericTable);
                }

                return new(null, 204, "Tabela Genérica excluído com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao excluír Tabela Genérica");
            }
        }
        #endregion
    }
}