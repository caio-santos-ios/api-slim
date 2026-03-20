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
    // ═══════════════════════════════════════════════════════════════════════════
    // Occupational Micro Checkin
    // ═══════════════════════════════════════════════════════════════════════════
    public class OccupationalMicroCheckinRepository(AppDbContext context) : IOccupationalMicroCheckinRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<OccupationalMicroCheckin> pagination)
        {
            try
            {
                List<BsonDocument> pipeline =
                [
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$addFields", new BsonDocument { { "id", new BsonDocument("$toString", "$_id") } }),
                    new("$project", new BsonDocument { { "_id", 0 } }),
                ];
                List<BsonDocument> results = await context.OccupationalMicroCheckins.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch { return new(null, 500, "Falha ao buscar Micro Checkins"); }
        }

        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                BsonDocument[] pipeline =
                [
                    new("$match", new BsonDocument { { "_id", new ObjectId(id) }, { "deleted", false } }),
                    new("$addFields", new BsonDocument { { "id", new BsonDocument("$toString", "$_id") } }),
                    new("$project", new BsonDocument { { "_id", 0 } }),
                ];
                BsonDocument? response = await context.OccupationalMicroCheckins.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Micro Checkin não encontrado") : new(result);
            }
            catch { return new(null, 500, "Falha ao buscar Micro Checkin"); }
        }

        public async Task<ResponseApi<OccupationalMicroCheckin?>> GetByIdAsync(string id)
        {
            try
            {
                OccupationalMicroCheckin? entity = await context.OccupationalMicroCheckins.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(entity);
            }
            catch { return new(null, 500, "Falha ao buscar Micro Checkin"); }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<OccupationalMicroCheckin> pagination)
        {
            List<BsonDocument> pipeline =
            [
                new("$match", pagination.PipelineFilter),
                new("$count", "total"),
            ];
            BsonDocument? doc = await context.OccupationalMicroCheckins.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return doc is null ? 0 : doc["total"].AsInt32;
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<OccupationalMicroCheckin?>> CreateAsync(OccupationalMicroCheckin entity)
        {
            try
            {
                await context.OccupationalMicroCheckins.InsertOneAsync(entity);
                return new(entity, 201, "Micro Checkin criado com sucesso");
            }
            catch { return new(null, 500, "Falha ao criar Micro Checkin"); }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<OccupationalMicroCheckin?>> UpdateAsync(OccupationalMicroCheckin entity)
        {
            try
            {
                await context.OccupationalMicroCheckins.ReplaceOneAsync(x => x.Id == entity.Id, entity);
                return new(entity, 200, "Micro Checkin atualizado com sucesso");
            }
            catch { return new(null, 500, "Falha ao atualizar Micro Checkin"); }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<OccupationalMicroCheckin>> DeleteAsync(string id)
        {
            try
            {
                OccupationalMicroCheckin? entity = await context.OccupationalMicroCheckins.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if (entity is null) return new(null, 404, "Micro Checkin não encontrado");
                entity.Deleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                await context.OccupationalMicroCheckins.ReplaceOneAsync(x => x.Id == id, entity);
                return new(entity, 204, "Excluído com sucesso");
            }
            catch { return new(null, 500, "Falha ao excluir Micro Checkin"); }
        }
        #endregion
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Occupational Bem Vital
    // ═══════════════════════════════════════════════════════════════════════════
    public class OccupationalBemVitalRepository(AppDbContext context) : IOccupationalBemVitalRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<OccupationalBemVital> pagination)
        {
            try
            {
                List<BsonDocument> pipeline =
                [
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$addFields", new BsonDocument { { "id", new BsonDocument("$toString", "$_id") } }),
                    new("$project", new BsonDocument { { "_id", 0 } }),
                ];
                List<BsonDocument> results = await context.OccupationalBemVitals.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch { return new(null, 500, "Falha ao buscar Bem Vital"); }
        }

        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                BsonDocument[] pipeline =
                [
                    new("$match", new BsonDocument { { "_id", new ObjectId(id) }, { "deleted", false } }),
                    new("$addFields", new BsonDocument { { "id", new BsonDocument("$toString", "$_id") } }),
                    new("$project", new BsonDocument { { "_id", 0 } }),
                ];
                BsonDocument? response = await context.OccupationalBemVitals.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Bem Vital não encontrado") : new(result);
            }
            catch { return new(null, 500, "Falha ao buscar Bem Vital"); }
        }

        public async Task<ResponseApi<OccupationalBemVital?>> GetByIdAsync(string id)
        {
            try
            {
                OccupationalBemVital? entity = await context.OccupationalBemVitals.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(entity);
            }
            catch { return new(null, 500, "Falha ao buscar Bem Vital"); }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<OccupationalBemVital> pagination)
        {
            List<BsonDocument> pipeline =
            [
                new("$match", pagination.PipelineFilter),
                new("$count", "total"),
            ];
            BsonDocument? doc = await context.OccupationalBemVitals.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return doc is null ? 0 : doc["total"].AsInt32;
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<OccupationalBemVital?>> CreateAsync(OccupationalBemVital entity)
        {
            try
            {
                await context.OccupationalBemVitals.InsertOneAsync(entity);
                return new(entity, 201, "Bem Vital criado com sucesso");
            }
            catch { return new(null, 500, "Falha ao criar Bem Vital"); }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<OccupationalBemVital?>> UpdateAsync(OccupationalBemVital entity)
        {
            try
            {
                await context.OccupationalBemVitals.ReplaceOneAsync(x => x.Id == entity.Id, entity);
                return new(entity, 200, "Bem Vital atualizado com sucesso");
            }
            catch { return new(null, 500, "Falha ao atualizar Bem Vital"); }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<OccupationalBemVital>> DeleteAsync(string id)
        {
            try
            {
                OccupationalBemVital? entity = await context.OccupationalBemVitals.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if (entity is null) return new(null, 404, "Bem Vital não encontrado");
                entity.Deleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                await context.OccupationalBemVitals.ReplaceOneAsync(x => x.Id == id, entity);
                return new(entity, 204, "Excluído com sucesso");
            }
            catch { return new(null, 500, "Falha ao excluir Bem Vital"); }
        }
        #endregion
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Occupational PGR
    // ═══════════════════════════════════════════════════════════════════════════
    public class OccupationalPgrRepository(AppDbContext context) : IOccupationalPgrRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<OccupationalPgr> pagination)
        {
            try
            {
                List<BsonDocument> pipeline =
                [
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$addFields", new BsonDocument { { "id", new BsonDocument("$toString", "$_id") } }),
                    new("$project", new BsonDocument { { "_id", 0 } }),
                ];
                List<BsonDocument> results = await context.OccupationalPgrs.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch { return new(null, 500, "Falha ao buscar PGRs"); }
        }

        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                BsonDocument[] pipeline =
                [
                    new("$match", new BsonDocument { { "_id", new ObjectId(id) }, { "deleted", false } }),
                    new("$addFields", new BsonDocument { { "id", new BsonDocument("$toString", "$_id") } }),
                    new("$project", new BsonDocument { { "_id", 0 } }),
                ];
                BsonDocument? response = await context.OccupationalPgrs.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "PGR não encontrado") : new(result);
            }
            catch { return new(null, 500, "Falha ao buscar PGR"); }
        }

        public async Task<ResponseApi<OccupationalPgr?>> GetByIdAsync(string id)
        {
            try
            {
                OccupationalPgr? entity = await context.OccupationalPgrs.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(entity);
            }
            catch { return new(null, 500, "Falha ao buscar PGR"); }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<OccupationalPgr> pagination)
        {
            List<BsonDocument> pipeline =
            [
                new("$match", pagination.PipelineFilter),
                new("$count", "total"),
            ];
            BsonDocument? doc = await context.OccupationalPgrs.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return doc is null ? 0 : doc["total"].AsInt32;
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<OccupationalPgr?>> CreateAsync(OccupationalPgr entity)
        {
            try
            {
                await context.OccupationalPgrs.InsertOneAsync(entity);
                return new(entity, 201, "PGR criado com sucesso");
            }
            catch { return new(null, 500, "Falha ao criar PGR"); }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<OccupationalPgr?>> UpdateAsync(OccupationalPgr entity)
        {
            try
            {
                await context.OccupationalPgrs.ReplaceOneAsync(x => x.Id == entity.Id, entity);
                return new(entity, 200, "PGR atualizado com sucesso");
            }
            catch { return new(null, 500, "Falha ao atualizar PGR"); }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<OccupationalPgr>> DeleteAsync(string id)
        {
            try
            {
                OccupationalPgr? entity = await context.OccupationalPgrs.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if (entity is null) return new(null, 404, "PGR não encontrado");
                entity.Deleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                await context.OccupationalPgrs.ReplaceOneAsync(x => x.Id == id, entity);
                return new(entity, 204, "Excluído com sucesso");
            }
            catch { return new(null, 500, "Falha ao excluir PGR"); }
        }
        #endregion
    }
}
