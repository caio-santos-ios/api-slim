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
    public class ServiceModuleRepository(AppDbContext context) : IServiceModuleRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<ServiceModule> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$addFields", new BsonDocument
                {
                    {"id", new BsonDocument("$toString", "$_id")},
                    { "uri", "$image" }
                }),
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),
                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.ServiceModules.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Módulos de Serviço");
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

            BsonDocument? response = await context.ServiceModules.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Módulo de Serviço não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Módulo de Serviço");
        }
    }
    
    public async Task<ResponseApi<ServiceModule?>> GetByIdAsync(string id)
    {
        try
        {
            ServiceModule? serviceModule = await context.ServiceModules.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(serviceModule);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Módulo de Serviço");
        }
    }
    public async Task<ResponseApi<ServiceModule?>> GetByPlanAsync(string planId)
    {
        try
        {
            ServiceModule? serviceModule = await context.ServiceModules.Find(x => x.PlanId == planId && !x.Deleted).FirstOrDefaultAsync();
            return new(serviceModule);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Módulo de Serviço");
        }
    }
    
    public async Task<ResponseApi<List<ServiceModule>>> GetByPlanIdAsync(string planId)
    {
        try
        {
            List<ServiceModule> serviceModules = await context.ServiceModules.Find(x => x.PlanId == planId && !x.Deleted).ToListAsync();
            return new(serviceModules);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Módulo de Serviço");
        }
    }
    public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<ServiceModule> pagination)
    {
        try
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

            List<BsonDocument> results = await context.ServiceModules.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Items");
        }
    }
    public async Task<ResponseApi<ServiceModule?>> GetByNameAsync(string name)
    {
        try
        {
            ServiceModule? serviceModule = await context.ServiceModules.Find(x => x.Name.ToUpper().Equals(name.ToUpper()) && !x.Deleted).FirstOrDefaultAsync();
            return new(serviceModule);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Módulo de Serviço");
        }
    }
    public async Task<ResponseApi<long?>> GetNextCodeAsync()
    {
        try
        {
            long code = await context.ServiceModules.Find(x => true).CountDocumentsAsync() + 1;
            return new(code);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Items");
        }
    }
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<ServiceModule> pagination)
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

        List<BsonDocument> results = await context.ServiceModules.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<ServiceModule?>> CreateAsync(ServiceModule serviceModule)
    {
        try
        {
            await context.ServiceModules.InsertOneAsync(serviceModule);

            return new(serviceModule, 201, "Módulo de Serviço criado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Módulo de Serviço");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<ServiceModule?>> UpdateAsync(ServiceModule serviceModule)
    {
        try
        {
            await context.ServiceModules.ReplaceOneAsync(x => x.Id == serviceModule.Id, serviceModule);

            return new(serviceModule, 201, "Módulo de Serviço atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Módulo de Serviço");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<ServiceModule>> DeleteAsync(string id)
    {
        try
        {
            ServiceModule? serviceModule = await context.ServiceModules.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(serviceModule is null) return new(null, 404, "Módulo de Serviço não encontrado");
            serviceModule.Deleted = true;
            serviceModule.DeletedAt = DateTime.UtcNow;

            await context.ServiceModules.ReplaceOneAsync(x => x.Id == id, serviceModule);

            return new(serviceModule, 204, "Módulo de Serviço excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Módulo de Serviço");
        }
    }
    #endregion
}
}