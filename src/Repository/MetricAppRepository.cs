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
    public class MetricAppRepository(AppDbContext context) : IMetricAppRepository
    {
        #region READ
        public async Task<ResponseApi<dynamic>> GetSummaryAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var now      = DateTime.UtcNow;
                var today    = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
                var week     = today.AddDays(-7);
                var month    = today.AddDays(-30);

                int lastDay = DateTime.DaysInMonth(today.Year, today.Month);

                DateTime start = new(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime end = new(endDate.Year, endDate.Month, lastDay, 23, 59, 59, DateTimeKind.Utc);

                long actionsToday = await context.MetricApps.Find(x => !x.Deleted && x.CreatedAt.Date == today.Date).CountDocumentsAsync();
                long actionsMonth = await context.MetricApps.Find(x => !x.Deleted && x.CreatedAt.Date >= start.Date && x.CreatedAt.Date <= end.Date).CountDocumentsAsync();
                long actions = await context.MetricApps.Find(x => !x.Deleted && x.CreatedAt.Date >= startDate.Date.AddDays(-1) && x.CreatedAt.Date <= endDate.Date).CountDocumentsAsync();
                
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

                var distinctResult = await context.MetricApps
                    .Aggregate<BsonDocument>(distinctPipeline).ToListAsync();

                long uniqueUsers = distinctResult.Count > 0
                    ? distinctResult[0].GetValue("total", 0).ToInt64()
                    : 0;

                dynamic summary = new
                {
                    actionsToday,
                    actionsMonth,
                    actions,
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
                    // new("$limit", limit),
                    new("$lookup", new BsonDocument
                    {
                        { "from", "customer_recipients" },
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
                            { new BsonDocument("$arrayElemAt", new BsonArray { "$_user.name", 0 }), "Usuário sem nome" }) },
                        { "userEmail", new BsonDocument("$ifNull", new BsonArray
                            { new BsonDocument("$arrayElemAt", new BsonArray { "$_user.email", 0 }), "" }) },
                    }),
                    new("$project", new BsonDocument
                    {
                        { "_id", 0 }, { "_user", 0 }
                    })
                };

                var results = await context.MetricApps.Aggregate<BsonDocument>(pipeline).ToListAsync();
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
                                { "action",     "$action" },
                                { "function",   "$function" },
                                { "screen",   "$screen" },
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
                        { "function",   "$_id.function" },
                        { "screen",     "$_id.screen" },
                    }),
                    new("$project", new BsonDocument { { "_id", 0 } })
                };

                var results = await context.MetricApps.Aggregate<BsonDocument>(pipeline).ToListAsync();
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

                var results = await context.MetricApps.Aggregate<BsonDocument>(pipeline).ToListAsync();
                var list = results.Select(d => (dynamic)BsonSerializer.Deserialize<dynamic>(d)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar timeline de atividade.");
            }
        }        
        #endregion
        #region CREATE
        public async Task<ResponseApi<MetricApp?>> CreateAsync(MetricApp metricApp)
        {
            try
            {
                DateTime today = DateTime.UtcNow;

                MetricApp existedMetricApp = await context.MetricApps.Find(x => 
                    !x.Deleted &&
                    x.Action == metricApp.Action &&
                    x.Function == metricApp.Function &&
                    x.CreatedAt.Date == today.Date &&
                    x.CreatedAt.Date.Hour == today.Date.Hour &&
                    x.CreatedAt.Date.Minute == today.Date.Minute && 
                    x.CreatedAt.Date.Second == today.Date.Second
                ).FirstOrDefaultAsync();

                if(existedMetricApp is null)
                {
                    await context.MetricApps.InsertOneAsync(metricApp);
                }

                return new(metricApp, 201, "Metrica criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao criar Metrica");   
            }
        }
        #endregion
    }
}