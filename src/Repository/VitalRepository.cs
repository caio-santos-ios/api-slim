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
                new("$sort", pagination.PipelineSort),
                // new("$skip", pagination.Skip),
                // new("$limit", pagination.Limit),

                MongoUtil.Lookup("customer_recipients", ["$beneficiaryId"], ["$_id"], "_recipient", [["deleted", false]], 1),
                new("$addFields", new BsonDocument
                {
                    {"contractorId", MongoUtil.First("_recipient.contractorId")},
                    {"beneficiaryName", MongoUtil.First("_recipient.name")}
                }),

                new("$match", pagination.PipelineFilter),

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
                    {"_position", 0},
                    {"_recipient", 0}
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
    
    public async Task<ResponseApi<List<Vital>>> GetByBeneficiaryIdWeekAsync(string beneficiaryId, string period)
    {
        try
        {
            DateTime hoje = DateTime.UtcNow; 
DateTime dataInicio = DateTime.MinValue;
DateTime dataFim = DateTime.MaxValue; 

switch (period.ToLower())
{
    case "semana":
        int diasParaSegunda = (int)hoje.DayOfWeek - (int)DayOfWeek.Monday;
        if (diasParaSegunda < 0) diasParaSegunda += 7;
        dataInicio = hoje.AddDays(-diasParaSegunda).Date;
        dataFim = dataInicio.AddDays(7);
        break;

    case "mes":
        dataInicio = new DateTime(hoje.Year, hoje.Month, 1);
        dataFim = dataInicio.AddMonths(1);
        break;

    case "ano":
        dataInicio = new DateTime(hoje.Year, 1, 1);
        dataFim = dataInicio.AddYears(1);
        break;

    case "tudo":
        // Já inicializados como Min e Max
        break;

    default:
        if (period.Contains("&"))
        {
            var dates = period.Split("&");
            
            // Tenta converter a data inicial se não estiver vazia
            if (dates.Length > 0 && !string.IsNullOrWhiteSpace(dates[0]))
            {
                if (DateTime.TryParse(dates[0], out DateTime start))
                    dataInicio = start;
            }

            // Tenta converter a data final se existir e não estiver vazia
            if (dates.Length > 1 && !string.IsNullOrWhiteSpace(dates[1]))
            {
                if (DateTime.TryParse(dates[1], out DateTime end))
                    // Somamos 1 dia para incluir as horas do último dia
                    dataFim = end.Date.AddDays(1);
            }
        }
        break;
}

// Filtro robusto: se as datas não forem enviadas, 
// ele usa MinValue e MaxValue, trazendo "tudo".
List<Vital> vitals = await context.Vitals
    .Find(x => x.BeneficiaryId == beneficiaryId && 
               x.CreatedAt >= dataInicio && 
               x.CreatedAt < dataFim && 
               !x.Deleted)
    .SortBy(x => x.CreatedAt)
    .ToListAsync();

            // List<Vital> vitals = await context.Vitals.Find(x => x.BeneficiaryId == beneficiaryId && x.CreatedAt >= inicioSemana && x.CreatedAt <= fimSemana && !x.Deleted).ToListAsync();
            return new(vitals);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item");
        }
    }
    public async Task<ResponseApi<List<Vital>>> GetByBeneficiaryIAllAsync(string beneficiaryId, DateTime? start, DateTime? end)
    {
        try
        {
            if(start is null && end is null) 
            {
                List<Vital> vitals = await context.Vitals.Find(x => x.BeneficiaryId == beneficiaryId && !x.Deleted).ToListAsync();
                return new(vitals);
            };

            
            if(start is not null && end is not null) 
            {
                List<Vital> vitals = await context.Vitals.Find(x => x.BeneficiaryId == beneficiaryId && x.CreatedAt >= start && x.CreatedAt <= end && !x.Deleted).ToListAsync();
                return new(vitals);
            };

            if(start is not null) 
            {
                List<Vital> vitals = await context.Vitals.Find(x => x.BeneficiaryId == beneficiaryId && x.CreatedAt >= start && !x.Deleted).ToListAsync();
                return new(vitals);
            }

            if(end is not null) 
            {
                List<Vital> vitals = await context.Vitals.Find(x => x.BeneficiaryId == beneficiaryId && x.CreatedAt <= end && !x.Deleted).ToListAsync();
                return new(vitals);
            }

            return new([]);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item");
        }
    }
    public async Task<ResponseApi<List<Vital>>> GetBeneficiaryIAllAsync(string beneficiaryId)
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
    
    public async Task<ResponseApi<Vital?>> GetToDateBeneficiaryAsync(string beneficiaryId, DateTime date)
    {
        try
        {
            Vital? vitals = await context.Vitals.Find(x => x.BeneficiaryId == beneficiaryId && x.CreatedAt.Date == date.Date && !x.Deleted).FirstOrDefaultAsync();
            return new(vitals);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item");
        }
    }
    
    // Pauta 11+12: busca vitais dos últimos N dias para calcular IES consecutivo baixo
    public async Task<ResponseApi<List<Vital>>> GetRecentByBeneficiaryAsync(string beneficiaryId, int days)
    {
        try
        {
            DateTime desde = DateTime.UtcNow.Date.AddDays(-(days - 1));
            List<Vital> vitals = await context.Vitals
                .Find(x => x.BeneficiaryId == beneficiaryId && x.CreatedAt >= desde && !x.Deleted)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync();
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
            new("$sort", pagination.PipelineSort),
            
            MongoUtil.Lookup("customer_recipients", ["$beneficiaryId"], ["$_id"], "_recipient", [["deleted", false]], 1),
            new("$addFields", new BsonDocument
            {
                {"contractorId", MongoUtil.First("_recipient.contractorId")},
                {"beneficiaryName", MongoUtil.First("_recipient.name")}
            }),

            new("$match", pagination.PipelineFilter),

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