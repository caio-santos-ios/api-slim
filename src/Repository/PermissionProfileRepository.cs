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
    public class PermissionProfileRepository(AppDbContext context) : IPermissionProfileRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<PermissionProfile> pagination)
        {
            try
            {
                List<BsonDocument> pipeline =
                [
                    new("$match", pagination.PipelineFilter),
                    new("$sort",  pagination.PipelineSort),
                    new("$addFields", new BsonDocument { { "id", new BsonDocument("$toString", "$_id") } }),
                    new("$project", new BsonDocument { { "_id", 0 } }),
                ];
                List<BsonDocument> results = await context.PermissionProfiles.Aggregate<BsonDocument>(pipeline).ToListAsync();
                return new(results.Select(d => BsonSerializer.Deserialize<dynamic>(d)).ToList());
            }
            catch { return new(null, 500, "Falha ao buscar Perfis de Permissão"); }
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
                BsonDocument? doc = await context.PermissionProfiles.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                if (doc is null) return new(null, 404, "Perfil não encontrado");
                return new(BsonSerializer.Deserialize<dynamic>(doc));
            }
            catch { return new(null, 500, "Falha ao buscar Perfil de Permissão"); }
        }

        public async Task<ResponseApi<PermissionProfile?>> GetByIdAsync(string id)
        {
            try
            {
                PermissionProfile? entity = await context.PermissionProfiles.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(entity);
            }
            catch { return new(null, 500, "Falha ao buscar Perfil de Permissão"); }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<PermissionProfile> pagination)
        {
            List<BsonDocument> pipeline =
            [
                new("$match", pagination.PipelineFilter),
                new("$count", "total"),
            ];
            BsonDocument? doc = await context.PermissionProfiles.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return doc is null ? 0 : doc["total"].AsInt32;
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<PermissionProfile?>> CreateAsync(PermissionProfile entity)
        {
            try
            {
                await context.PermissionProfiles.InsertOneAsync(entity);
                return new(entity, 201, "Perfil criado com sucesso");
            }
            catch { return new(null, 500, "Falha ao criar Perfil de Permissão"); }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<PermissionProfile?>> UpdateAsync(PermissionProfile entity)
        {
            try
            {
                await context.PermissionProfiles.ReplaceOneAsync(x => x.Id == entity.Id, entity);
                return new(entity, 200, "Perfil atualizado com sucesso");
            }
            catch { return new(null, 500, "Falha ao atualizar Perfil de Permissão"); }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<PermissionProfile>> DeleteAsync(string id)
        {
            try
            {
                PermissionProfile? entity = await context.PermissionProfiles.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if (entity is null) return new(null, 404, "Perfil não encontrado");
                entity.Deleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                await context.PermissionProfiles.ReplaceOneAsync(x => x.Id == id, entity);
                return new(entity, 204, "Excluído com sucesso");
            }
            catch { return new(null, 500, "Falha ao excluir Perfil de Permissão"); }
        }
        #endregion
    }
}
