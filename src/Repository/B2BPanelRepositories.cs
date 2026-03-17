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
    // B2B Mass Movement
    // ═══════════════════════════════════════════════════════════════════════════
    public class B2BMassMovementRepository(AppDbContext context) : IB2BMassMovementRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<B2BMassMovement> pagination)
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
                List<BsonDocument> results = await context.B2BMassMovements.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch { return new(null, 500, "Falha ao buscar Movimentações de Massa"); }
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
                BsonDocument? response = await context.B2BMassMovements.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Movimentação não encontrada") : new(result);
            }
            catch { return new(null, 500, "Falha ao buscar Movimentação"); }
        }

        public async Task<ResponseApi<B2BMassMovement?>> GetByIdAsync(string id)
        {
            try
            {
                B2BMassMovement? entity = await context.B2BMassMovements.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(entity);
            }
            catch { return new(null, 500, "Falha ao buscar Movimentação"); }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<B2BMassMovement> pagination)
        {
            List<BsonDocument> pipeline =
            [
                new("$match", pagination.PipelineFilter),
                new("$count", "total"),
            ];
            BsonDocument? doc = await context.B2BMassMovements.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return doc is null ? 0 : doc["total"].AsInt32;
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<B2BMassMovement?>> CreateAsync(B2BMassMovement entity)
        {
            try
            {
                await context.B2BMassMovements.InsertOneAsync(entity);
                return new(entity, 201, "Movimentação criada com sucesso");
            }
            catch { return new(null, 500, "Falha ao criar Movimentação"); }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<B2BMassMovement?>> UpdateAsync(B2BMassMovement entity)
        {
            try
            {
                await context.B2BMassMovements.ReplaceOneAsync(x => x.Id == entity.Id, entity);
                return new(entity, 200, "Movimentação atualizada com sucesso");
            }
            catch { return new(null, 500, "Falha ao atualizar Movimentação"); }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<B2BMassMovement>> DeleteAsync(string id)
        {
            try
            {
                B2BMassMovement? entity = await context.B2BMassMovements.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if (entity is null) return new(null, 404, "Movimentação não encontrada");
                entity.Deleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                await context.B2BMassMovements.ReplaceOneAsync(x => x.Id == id, entity);
                return new(entity, 204, "Excluído com sucesso");
            }
            catch { return new(null, 500, "Falha ao excluir Movimentação"); }
        }
        #endregion
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // B2B Invoice
    // ═══════════════════════════════════════════════════════════════════════════
    public class B2BInvoiceRepository(AppDbContext context) : IB2BInvoiceRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<B2BInvoice> pagination)
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
                List<BsonDocument> results = await context.B2BInvoices.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch { return new(null, 500, "Falha ao buscar Faturas"); }
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
                BsonDocument? response = await context.B2BInvoices.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Fatura não encontrada") : new(result);
            }
            catch { return new(null, 500, "Falha ao buscar Fatura"); }
        }

        public async Task<ResponseApi<B2BInvoice?>> GetByIdAsync(string id)
        {
            try
            {
                B2BInvoice? entity = await context.B2BInvoices.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(entity);
            }
            catch { return new(null, 500, "Falha ao buscar Fatura"); }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<B2BInvoice> pagination)
        {
            List<BsonDocument> pipeline =
            [
                new("$match", pagination.PipelineFilter),
                new("$count", "total"),
            ];
            BsonDocument? doc = await context.B2BInvoices.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return doc is null ? 0 : doc["total"].AsInt32;
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<B2BInvoice?>> CreateAsync(B2BInvoice entity)
        {
            try
            {
                await context.B2BInvoices.InsertOneAsync(entity);
                return new(entity, 201, "Fatura criada com sucesso");
            }
            catch { return new(null, 500, "Falha ao criar Fatura"); }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<B2BInvoice?>> UpdateAsync(B2BInvoice entity)
        {
            try
            {
                await context.B2BInvoices.ReplaceOneAsync(x => x.Id == entity.Id, entity);
                return new(entity, 200, "Fatura atualizada com sucesso");
            }
            catch { return new(null, 500, "Falha ao atualizar Fatura"); }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<B2BInvoice>> DeleteAsync(string id)
        {
            try
            {
                B2BInvoice? entity = await context.B2BInvoices.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if (entity is null) return new(null, 404, "Fatura não encontrada");
                entity.Deleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                await context.B2BInvoices.ReplaceOneAsync(x => x.Id == id, entity);
                return new(entity, 204, "Excluído com sucesso");
            }
            catch { return new(null, 500, "Falha ao excluir Fatura"); }
        }
        #endregion
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // B2B Attachment
    // ═══════════════════════════════════════════════════════════════════════════
    public class B2BAttachmentRepository(AppDbContext context) : IB2BAttachmentRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<B2BAttachment> pagination)
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
                List<BsonDocument> results = await context.B2BAttachments.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch { return new(null, 500, "Falha ao buscar Anexos"); }
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
                BsonDocument? response = await context.B2BAttachments.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Anexo não encontrado") : new(result);
            }
            catch { return new(null, 500, "Falha ao buscar Anexo"); }
        }

        public async Task<ResponseApi<B2BAttachment?>> GetByIdAsync(string id)
        {
            try
            {
                B2BAttachment? entity = await context.B2BAttachments.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(entity);
            }
            catch { return new(null, 500, "Falha ao buscar Anexo"); }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<B2BAttachment> pagination)
        {
            List<BsonDocument> pipeline =
            [
                new("$match", pagination.PipelineFilter),
                new("$count", "total"),
            ];
            BsonDocument? doc = await context.B2BAttachments.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return doc is null ? 0 : doc["total"].AsInt32;
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<B2BAttachment?>> CreateAsync(B2BAttachment entity)
        {
            try
            {
                await context.B2BAttachments.InsertOneAsync(entity);
                return new(entity, 201, "Anexo criado com sucesso");
            }
            catch { return new(null, 500, "Falha ao criar Anexo"); }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<B2BAttachment?>> UpdateAsync(B2BAttachment entity)
        {
            try
            {
                await context.B2BAttachments.ReplaceOneAsync(x => x.Id == entity.Id, entity);
                return new(entity, 200, "Anexo atualizado com sucesso");
            }
            catch { return new(null, 500, "Falha ao atualizar Anexo"); }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<B2BAttachment>> DeleteAsync(string id)
        {
            try
            {
                B2BAttachment? entity = await context.B2BAttachments.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if (entity is null) return new(null, 404, "Anexo não encontrado");
                entity.Deleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                await context.B2BAttachments.ReplaceOneAsync(x => x.Id == id, entity);
                return new(entity, 204, "Excluído com sucesso");
            }
            catch { return new(null, 500, "Falha ao excluir Anexo"); }
        }
        #endregion
    }
}
