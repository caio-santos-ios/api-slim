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
    public class PlanRepository(AppDbContext context) : IPlanRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Plan> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                // new("$skip", pagination.Skip),
                // new("$limit", pagination.Limit),

                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "service_modules" },
                    { "let", new BsonDocument("idsDoContrato", "$serviceModuleIds") }, 
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
                    { "as", "_service_modules" }
                }),

                new BsonDocument("$addFields", new BsonDocument
                {
                    { "id", new BsonDocument("$toString", "$_id") },
                    { "uri", "$image" },
                    { "serviceModuleIds", new BsonDocument("$map", new BsonDocument
                        {
                            { "input", "$_service_modules" },
                            { "as", "m" },
                            // { "in", new BsonDocument("$toString", "$$m._id") }
                            { "in", new BsonDocument
                                {
                                    { "id", new BsonDocument("$toString", "$$m._id") },
                                    { "name", "$$m.name" },
                                    { "planId", "$$m.planId" }
                                }
                            }
                        })
                    }
                }),

                new BsonDocument("$project", new BsonDocument
                {
                    { "_id", 0 },
                    { "_service_modules", 0 }
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Plans.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Plano");
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
                    {"name", 1},
                    {"description", 1}
                }),
            ];

            BsonDocument? response = await context.Plans.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Plano não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Plano");
        }
    }
    
    public async Task<ResponseApi<Plan?>> GetByIdAsync(string id)
    {
        try
        {
            Plan? plan = await context.Plans.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(plan);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Plano");
        }
    }
    
    public async Task<ResponseApi<long?>> GetNextCodeAsync()
    {
        try
        {
            long code = await context.Plans.Find(x => true).CountDocumentsAsync() + 1;
            return new(code);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Items");
        }
    }
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Plan> pagination)
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

        List<BsonDocument> results = await context.Plans.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Plan?>> CreateAsync(Plan plan)
    {
        try
        {
            await context.Plans.InsertOneAsync(plan);

            return new(plan, 201, "Plano criado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Plano");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Plan?>> UpdateAsync(Plan plan)
    {
        try
        {
            await context.Plans.ReplaceOneAsync(x => x.Id == plan.Id, plan);

            return new(plan, 201, "Plano atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Plano");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Plan>> DeleteAsync(string id)
    {
        try
        {
            Plan? plan = await context.Plans.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(plan is null) return new(null, 404, "Plano não encontrado");
            plan.Deleted = true;
            plan.DeletedAt = DateTime.UtcNow;

            await context.Plans.ReplaceOneAsync(x => x.Id == id, plan);

            return new(plan, 204, "Plano excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Plano");
        }
    }
    #endregion
}
}