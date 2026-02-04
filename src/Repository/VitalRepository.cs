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
    public class VitalRepository(AppDbContext context) : IVitalRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Vital> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),

                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "generic_tables" }, 
                    { "let", new BsonDocument("department", "$department") },
                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match", new BsonDocument
                            {
                                { "$expr", new BsonDocument("$and", new BsonArray
                                    {
                                        new BsonDocument("$eq", new BsonArray { "$code", "$$department" }),
                                        new BsonDocument("$eq", new BsonArray { "$table", "departamento-contato-representante" })
                                    })
                                }
                            })
                        }
                    },
                    { "as", "_department" } 
                }),

                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$_department" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "generic_tables" }, 
                    { "let", new BsonDocument("position", "$position") },
                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match", new BsonDocument
                            {
                                { "$expr", new BsonDocument("$and", new BsonArray
                                    {
                                        new BsonDocument("$eq", new BsonArray { "$code", "$$position" }),
                                        new BsonDocument("$eq", new BsonArray { "$table", "funcao-contato-representante" })
                                    })
                                }
                            })
                        }
                    },
                    { "as", "_position" } 
                }),

                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$_position" },
                    { "preserveNullAndEmptyArrays", true }
                }),

                new("$addFields", new BsonDocument
                {
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"departmentDescription", "$_department.description"},
                    {"positionDescription", "$_position.description"}
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                    {"_department", 0}, 
                    {"_position", 0} 
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Vitals.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Items");
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

            BsonDocument? response = await context.Vitals.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Item não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item");
        }
    }
    
    public async Task<ResponseApi<Vital?>> GetByIdAsync(string id)
    {
        try
        {
            Vital? vital = await context.Vitals.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(vital);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item");
        }
    }
    
    public async Task<ResponseApi<Vital?>> GetByBeneficiaryIdAsync(string beneficiaryId)
    {
        try
        {
            Vital? vital = await context.Vitals.Find(x => x.BeneficiaryId == beneficiaryId && x.CreatedAt.Date == DateTime.UtcNow.Date && !x.Deleted).FirstOrDefaultAsync();
            return new(vital);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item");
        }
    }
    
    public async Task<ResponseApi<List<Vital>>> GetByBeneficiaryIdWeekAsync(string beneficiaryId)
    {
        try
        {
            DateTime hoje = DateTime.Today;
            int diasParaSegunda = (int)hoje.DayOfWeek - (int)DayOfWeek.Monday;
            if (diasParaSegunda < 0) diasParaSegunda += 7;

            DateTime inicioSemana = hoje.AddDays(-diasParaSegunda);
            DateTime fimSemana = inicioSemana.AddDays(6);

            List<Vital> vitals = await context.Vitals.Find(x => x.BeneficiaryId == beneficiaryId && x.CreatedAt >= inicioSemana && x.CreatedAt <= fimSemana && !x.Deleted).ToListAsync();
            return new(vitals);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item");
        }
    }
    public async Task<ResponseApi<List<Vital>>> GetByBeneficiaryIAllAsync(string beneficiaryId)
    {
        try
        {
            List<Vital> vitals = await context.Vitals.Find(x => x.BeneficiaryId == beneficiaryId && !x.Deleted).ToListAsync();
            return new(vitals);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item");
        }
    }
    
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Vital> pagination)
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

        List<BsonDocument> results = await context.Vitals.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Vital?>> CreateAsync(Vital vital)
    {
        try
        {
            await context.Vitals.InsertOneAsync(vital);

            return new(vital, 201, "Item criado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Item");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Vital?>> UpdateAsync(Vital vital)
    {
        try
        {
            await context.Vitals.ReplaceOneAsync(x => x.Id == vital.Id, vital);

            return new(vital, 201, "Item atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Item");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Vital>> DeleteAsync(string id)
    {
        try
        {
            Vital? vital = await context.Vitals.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(vital is null) return new(null, 404, "Item não encontrado");
            vital.Deleted = true;
            vital.DeletedAt = DateTime.UtcNow;

            await context.Vitals.ReplaceOneAsync(x => x.Id == id, vital);

            return new(vital, 204, "Item excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Item");
        }
    }
    #endregion
}
}