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
    public class NotificationRepository(AppDbContext context) : INotificationRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<NotificationJob> pagination)
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
                        {"phone", 1},
                        {"message", 1},
                        {"sendDate", 1},
                        {"sent", 1},
                        {"type", 1},
                        {"title", 1}
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.NotificationJobs.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Notificações");
            }
        }
        public async Task<ResponseApi<NotificationJob?>> GetByIdAsync(string id)
        {
            try
            {
                NotificationJob? notification = await context.NotificationJobs.Find(x => x.Id == id).FirstOrDefaultAsync();
                return new(notification);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Notificação");
            }
        }
        #endregion
        #region UPDATE
        public async Task<ResponseApi<NotificationJob?>> UpdateAsync(NotificationJob notification)
        {
            try
            {
                await context.NotificationJobs.ReplaceOneAsync(x => x.Id == notification.Id, notification);

                return new(notification, 201, "Notificação atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao atualizar Notificação");
            }
        }
        #endregion
    }
}