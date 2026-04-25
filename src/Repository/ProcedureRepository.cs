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
    public class ProcedureRepository(AppDbContext context) : IProcedureRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Procedure> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$sort", pagination.PipelineSort),
                // new("$skip", pagination.Skip),
                // new("$limit", pagination.Limit),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "service_modules" },
                    { "let", new BsonDocument("registrationId", new BsonDocument("$toObjectId", "$serviceModuleId")) },
                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match", new BsonDocument
                            {
                                { "$expr", new BsonDocument("$eq", new BsonArray { "$_id", "$$registrationId" }) }
                            })
                        }
                    },
                    { "as", "_service_module" }
                }),
                new BsonDocument("$unwind", "$_service_module"),

                new("$match", pagination.PipelineFilter),
                
                new("$addFields", new BsonDocument
                {
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"serviceModule", new BsonDocument("$ifNull", new BsonArray { "$_service_module.name", "" })},
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Procedures.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Procedimentos");
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
                    {"name", 1},
                    {"notes", 1},
                    {"serviceModuleId", 1},
                    {"externalCodes", 1},
                    {"active", 1},
                    {"description", 1}
                }),
            ];

            BsonDocument? response = await context.Procedures.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Procedimento não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Procedimento");
        }
    }
    
    public async Task<ResponseApi<Procedure?>> GetByIdAsync(string id)
    {
        try
        {
            Procedure? procedure = await context.Procedures.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(procedure);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Procedimento");
        }
    }
    
    public async Task<ResponseApi<long>> GetNextCodeAsync()
    {
        try
        {
            long count = await context.Procedures.Find(x => true).CountDocumentsAsync();
            return new(count);
        }
        catch
        {
            return new(0, 500, "Falha ao buscar Procedimento");
        }
    }
    
    public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Procedure> pagination)
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

            List<BsonDocument> results = await context.Procedures.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Items");
        }
    }
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Procedure> pagination)
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

        List<BsonDocument> results = await context.Procedures.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Procedure?>> CreateAsync(Procedure procedure)
    {
        try
        {
            await context.Procedures.InsertOneAsync(procedure);

            return new(procedure, 201, "Procedimento criado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Procedimento");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Procedure?>> UpdateAsync(Procedure procedure)
    {
        try
        {
            await context.Procedures.ReplaceOneAsync(x => x.Id == procedure.Id, procedure);

            return new(procedure, 201, "Procedimento atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Procedimento");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Procedure>> DeleteAsync(string id)
    {
        try
        {
            Procedure? procedure = await context.Procedures.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(procedure is null) return new(null, 404, "Procedimento não encontrado");
            procedure.Deleted = true;
            procedure.DeletedAt = DateTime.UtcNow;

            await context.Procedures.ReplaceOneAsync(x => x.Id == id, procedure);

            return new(procedure, 204, "Procedimento excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Procedimento");
        }
    }
    #endregion
}
}