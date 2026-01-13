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
    public class TradingTableRepository(AppDbContext context) : ITradingTableRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<TradingTable> pagination)
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
                    {"name", 1},
                    {"createdAt", 1},
                    {"items", 1}
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.TradingTables.Aggregate<BsonDocument>(pipeline).ToListAsync();
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
                new("$addFields", new BsonDocument {
                    {"id", new BsonDocument("$toString", "$_id")},
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                }),
            ];

            BsonDocument? response = await context.TradingTables.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Tabela de Negociação não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    
    public async Task<ResponseApi<TradingTable?>> GetByIdAsync(string id)
    {
        try
        {
            TradingTable? address = await context.TradingTables.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(address);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    
    public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<TradingTable> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"name", 1},
                    {"createdAt", 1}
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.TradingTables.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<dynamic?>> GetByaccreditedNetworkIdAsync(string accreditedNetworkId)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", new BsonDocument {
                    {"deleted", false},
                    {"accreditedNetworkId", accreditedNetworkId}
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"name", 1},
                    {"accreditedNetworkId", 1},
                    {"createdAt", 1},
                    {"items", 1}
                }),
                new("$sort", new BsonDocument {
                    { "createdAt", -1}
                }),
            };

            BsonDocument? response = await context.TradingTables.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Tabela de Negociação não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<TradingTable> pagination)
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

        List<BsonDocument> results = await context.TradingTables.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<TradingTable?>> CreateAsync(TradingTable address)
    {
        try
        {
            await context.TradingTables.InsertOneAsync(address);

            return new(address, 201, "Tabela de Negociação criado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<TradingTable?>> UpdateAsync(TradingTable address)
    {
        try
        {
            await context.TradingTables.ReplaceOneAsync(x => x.Id == address.Id, address);

            return new(address, 201, "Tabela de Negociação atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<TradingTable>> DeleteAsync(string id)
    {
        try
        {
            TradingTable? address = await context.TradingTables.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(address is null) return new(null, 404, "Tabela de Negociação não encontrado");
            address.Deleted = true;
            address.DeletedAt = DateTime.UtcNow;

            await context.TradingTables.ReplaceOneAsync(x => x.Id == id, address);

            return new(address, 204, "Tabela de Negociação excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
}
}