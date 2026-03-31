using api_slim.src.Configuration;
using api_slim.src.Interfaces.Metrics;
using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace api_slim.src.Repository
{
    public class MetricsRepository(AppDbContext context) : IMetricsRepository
    {
        // ─────────────────────────────────────────────────────────────────────
        // SUMMARY  –  totais de hoje / semana / mês + usuários únicos no mês
        // ─────────────────────────────────────────────────────────────────────
        public async Task<ResponseApi<dynamic>> GetSummaryAsync()
        {
            try
            {
                var now      = DateTime.UtcNow;
                var today    = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
                var week     = today.AddDays(-7);
                var month    = today.AddDays(-30);

                long actionsToday = await context.Logs
                    .CountDocumentsAsync(Builders<Models.Log>.Filter.And(
                        Builders<Models.Log>.Filter.Eq("deleted", false),
                        Builders<Models.Log>.Filter.Gte("createdAt", today)
                    ));

                long actionsWeek = await context.Logs
                    .CountDocumentsAsync(Builders<Models.Log>.Filter.And(
                        Builders<Models.Log>.Filter.Eq("deleted", false),
                        Builders<Models.Log>.Filter.Gte("createdAt", week)
                    ));

                long actionsMonth = await context.Logs
                    .CountDocumentsAsync(Builders<Models.Log>.Filter.And(
                        Builders<Models.Log>.Filter.Eq("deleted", false),
                        Builders<Models.Log>.Filter.Gte("createdAt", month)
                    ));

                // usuários únicos no mês
                var distinctPipeline = new List<BsonDocument>
                {
                    new("$match", new BsonDocument
                    {
                        { "deleted", false },
                        { "createdAt", new BsonDocument("$gte", month) }
                    }),
                    new("$group", new BsonDocument
                    {
                        { "_id", "$createdBy" }
                    }),
                    new("$count", "total")
                };

                var distinctResult = await context.Logs
                    .Aggregate<BsonDocument>(distinctPipeline).ToListAsync();

                long uniqueUsers = distinctResult.Count > 0
                    ? distinctResult[0].GetValue("total", 0).ToInt64()
                    : 0;

                dynamic summary = new
                {
                    actionsToday,
                    actionsWeek,
                    actionsMonth,
                    uniqueUsersMonth = uniqueUsers
                };

                return new(summary);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar resumo de métricas.");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // TOP USERS  –  usuários mais ativos (últimos 30 dias)
        // ─────────────────────────────────────────────────────────────────────
        public async Task<ResponseApi<List<dynamic>>> GetTopUsersAsync(int limit)
        {
            try
            {
                var since = DateTime.UtcNow.AddDays(-30);

                var pipeline = new List<BsonDocument>
                {
                    new("$match", new BsonDocument
                    {
                        { "deleted", false },
                        { "createdAt", new BsonDocument("$gte", since) }
                    }),
                    new("$group", new BsonDocument
                    {
                        { "_id", "$createdBy" },
                        { "total", new BsonDocument("$sum", 1) }
                    }),
                    new("$sort", new BsonDocument("total", -1)),
                    new("$limit", limit),
                    new("$lookup", new BsonDocument
                    {
                        { "from", "users" },
                        { "let", new BsonDocument("uid", new BsonDocument("$toObjectId", "$_id")) },
                        { "pipeline", new BsonArray
                            {
                                new BsonDocument("$match", new BsonDocument("$expr",
                                    new BsonDocument("$eq", new BsonArray { "$_id", "$$uid" })))
                            }
                        },
                        { "as", "_user" }
                    }),
                    new("$addFields", new BsonDocument
                    {
                        { "userId",   "$_id" },
                        { "userName", new BsonDocument("$ifNull", new BsonArray
                            { new BsonDocument("$arrayElemAt", new BsonArray { "$_user.name", 0 }), "Usuário removido" }) },
                        { "userEmail", new BsonDocument("$ifNull", new BsonArray
                            { new BsonDocument("$arrayElemAt", new BsonArray { "$_user.email", 0 }), "" }) },
                    }),
                    new("$project", new BsonDocument
                    {
                        { "_id", 0 }, { "_user", 0 }
                    })
                };

                var results = await context.Logs.Aggregate<BsonDocument>(pipeline).ToListAsync();
                var list = results.Select(d => (dynamic)BsonSerializer.Deserialize<dynamic>(d)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar usuários mais ativos.");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // TOP FEATURES  –  funcionalidades mais usadas (últimos 30 dias)
        // ─────────────────────────────────────────────────────────────────────
        public async Task<ResponseApi<List<dynamic>>> GetTopFeaturesAsync(int limit)
        {
            try
            {
                var since = DateTime.UtcNow.AddDays(-30);

                var pipeline = new List<BsonDocument>
                {
                    new("$match", new BsonDocument
                    {
                        { "deleted", false },
                        { "createdAt", new BsonDocument("$gte", since) },
                        { "collection", new BsonDocument("$ne", "") }
                    }),
                    new("$group", new BsonDocument
                    {
                        { "_id", new BsonDocument
                            {
                                { "collection", "$collection" },
                                { "action",     "$action" }
                            }
                        },
                        { "total", new BsonDocument("$sum", 1) }
                    }),
                    new("$sort", new BsonDocument("total", -1)),
                    new("$limit", limit),
                    new("$addFields", new BsonDocument
                    {
                        { "feature",    "$_id.collection" },
                        { "action",     "$_id.action" },
                    }),
                    new("$project", new BsonDocument { { "_id", 0 } })
                };

                var results = await context.Logs.Aggregate<BsonDocument>(pipeline).ToListAsync();
                var list = results.Select(d => (dynamic)BsonSerializer.Deserialize<dynamic>(d)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar funcionalidades mais usadas.");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // TIMELINE  –  ações por dia (últimos N dias)
        // ─────────────────────────────────────────────────────────────────────
        public async Task<ResponseApi<List<dynamic>>> GetTimelineAsync(int days)
        {
            try
            {
                var since = DateTime.UtcNow.AddDays(-days);

                var pipeline = new List<BsonDocument>
                {
                    new("$match", new BsonDocument
                    {
                        { "deleted", false },
                        { "createdAt", new BsonDocument("$gte", since) }
                    }),
                    new("$group", new BsonDocument
                    {
                        { "_id", new BsonDocument("$dateToString", new BsonDocument
                            {
                                { "format", "%Y-%m-%d" },
                                { "date",   "$createdAt" }
                            })
                        },
                        { "total", new BsonDocument("$sum", 1) }
                    }),
                    new("$sort", new BsonDocument("_id", 1)),
                    new("$addFields", new BsonDocument { { "date", "$_id" } }),
                    new("$project", new BsonDocument { { "_id", 0 } })
                };

                var results = await context.Logs.Aggregate<BsonDocument>(pipeline).ToListAsync();
                var list = results.Select(d => (dynamic)BsonSerializer.Deserialize<dynamic>(d)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar timeline de atividade.");
            }
        }
    }
}
