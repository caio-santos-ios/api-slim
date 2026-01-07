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
    public class LogRepository(AppDbContext context) : ILogRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Log> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),
                    
                    MongoUtil.Lookup("users", ["$createdBy"], ["$_id"], "_user", [["deleted", false]], 1),

                    new("$addFields", new BsonDocument
                    {
                        {"id", MongoUtil.ToString("$_id")},
                        {"userCreate", MongoUtil.First("_user.name")}
                    }),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"_user", 0},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Logs.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Logs"); ;
            }
        }
        #endregion
        #region CREATE
        public async Task<ResponseApi<Log?>> CreateAsync(Log Log)
        {
            try
            {
                await context.Logs.InsertOneAsync(Log);

                return new(Log, 201, "Log criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao criar Log");   
            }
        }
        #endregion
    }
}