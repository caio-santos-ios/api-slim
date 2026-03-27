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
    public class AccountsPayableRepository(AppDbContext context) : IAccountsPayableRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<AccountsPayable> pagination)
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
                        { "from", "suppliers" },

                        { "let", new BsonDocument("profId", new BsonDocument("$toObjectId", "$supplierId")) },

                        { "pipeline", new BsonArray
                            {
                                new BsonDocument("$match", new BsonDocument
                                {
                                    { "$expr", new BsonDocument("$and", new BsonArray
                                        {
                                            new BsonDocument("$eq", new BsonArray
                                            {
                                                "$_id",
                                                "$$profId"
                                            }),
                                        })
                                    }
                                })
                            }
                        },

                        { "as", "_supplier" }
                    }),
                    
                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "generic_tables" }, 
                        { "let", new BsonDocument("category", "$category") },
                        { "pipeline", new BsonArray
                            {
                                new BsonDocument("$match", new BsonDocument
                                {
                                    { "$expr", new BsonDocument("$and", new BsonArray
                                        {
                                            new BsonDocument("$eq", new BsonArray { "$code", "$$category" }),
                                            new BsonDocument("$eq", new BsonArray { "$table", "categoria-despesas" })
                                        })
                                    }
                                })
                            }
                        },
                        { "as", "_category" } 
                    }),

                    new BsonDocument("$unwind", new BsonDocument
                    {
                        { "path", "$_category" },
                        { "preserveNullAndEmptyArrays", true }
                    }),
                    
                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "generic_tables" }, 
                        { "let", new BsonDocument("costCenter", "$costCenter") },
                        { "pipeline", new BsonArray
                            {
                                new BsonDocument("$match", new BsonDocument
                                {
                                    { "$expr", new BsonDocument("$and", new BsonArray
                                        {
                                            new BsonDocument("$eq", new BsonArray { "$code", "$$costCenter" }),
                                            new BsonDocument("$eq", new BsonArray { "$table", "centro-custo" })
                                        })
                                    }
                                })
                            }
                        },
                        { "as", "_costCenter" } 
                    }),

                    new BsonDocument("$unwind", new BsonDocument
                    {
                        { "path", "$_costCenter" },
                        { "preserveNullAndEmptyArrays", true }
                    }),
                    
                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "generic_tables" }, 
                        { "let", new BsonDocument("paymentMethod", "$paymentMethod") },
                        { "pipeline", new BsonArray
                            {
                                new BsonDocument("$match", new BsonDocument
                                {
                                    { "$expr", new BsonDocument("$and", new BsonArray
                                        {
                                            new BsonDocument("$eq", new BsonArray { "$code", "$$paymentMethod" }),
                                            new BsonDocument("$eq", new BsonArray { "$table", "forma-pagamento" })
                                        })
                                    }
                                })
                            }
                        },
                        { "as", "_payment_method" } 
                    }),

                    new BsonDocument("$unwind", new BsonDocument
                    {
                        { "path", "$_payment_method" },
                        { "preserveNullAndEmptyArrays", true }
                    }),

                    new BsonDocument("$addFields", new BsonDocument
                    {
                        {"total", new BsonDocument("$sum", new BsonArray 
                            { 
                                new BsonDocument("$toDouble", "$fees"), 
                                new BsonDocument("$toDouble", "$fines"),
                                new BsonDocument("$toDouble", "$value"),
                            }) 
                        },
                       
                    }),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"value", 1},
                        {"lowValue", 1},
                        {"code", 1},
                        {"balance", new BsonDocument("$subtract", new BsonArray 
                            { 
                                new BsonDocument("$toDouble", "$total"), 
                                new BsonDocument("$toDouble", "$lowValue") 
                            }) 
                        },
                        {"total", 1},
                        {"fees", 1},
                        {"fines", 1},                        
                        {"dueDate", 1},                        
                        {"supplierName", new BsonDocument("$first", "$_supplier.corporateName")},
                        {"categoryDescription", new BsonDocument("$ifNull", new BsonArray { "$_category.description", "" })},
                        {"costCenterDescription", new BsonDocument("$ifNull", new BsonArray { "$_costCenter.description", "" })},
                        {"paymentMethodDescription", new BsonDocument("$ifNull", new BsonArray { "$_payment_method.description", "" })},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.AccountsPayables.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Contas a Pagar"); ;
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

                BsonDocument? response = await context.AccountsPayables.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Contas a Pagar não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); ;
            }
        }
        public async Task<ResponseApi<AccountsPayable?>> GetByIdAsync(string id)
        {
            try
            {
                AccountsPayable? accountsPayable = await context.AccountsPayables.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(accountsPayable);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Contas a Pagar"); ;
            }
        }
        public async Task<ResponseApi<long>> GetNextCodeAsync()
        {
            try
            {
                long count = await context.AccountsPayables.Find(x => true).CountDocumentsAsync();
                return new(count);
            }
            catch
            {
                return new(0, 500, "Falha ao buscar novo código");
            }
        }
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<AccountsPayable> pagination)
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
                    {"password", 0},
                    {"role", 0},
                    {"blocked", 0},
                    {"codeAccess", 0},
                    {"validatedAccess", 0}
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.AccountsPayables.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        #region CREATE
        public async Task<ResponseApi<AccountsPayable?>> CreateAsync(AccountsPayable accountsPayable)
        {
            try
            {
                await context.AccountsPayables.InsertOneAsync(accountsPayable);

                return new(accountsPayable, 201, "Contas a Pagar criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao criar Contas a Pagar");   
            }
        }
        #endregion
        #region UPDATE
        public async Task<ResponseApi<AccountsPayable?>> UpdateAsync(AccountsPayable accountsPayable)
        {
            try
            {
                await context.AccountsPayables.ReplaceOneAsync(x => x.Id == accountsPayable.Id, accountsPayable);

                return new(accountsPayable, 201, "Contas a Pagar atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao atualizar Contas a Pagar");
            }
        }
        public async Task<ResponseApi<AccountsPayable?>> UpdateLowAsync(AccountsPayable accountsPayable)
        {
            try
            {
                await context.AccountsPayables.ReplaceOneAsync(x => x.Id == accountsPayable.Id, accountsPayable);

                return new(accountsPayable, 201, "Baixa feita com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao dar baixa na Conta a Receber");
            }
        }
        #endregion
        #region DELETE
        public async Task<ResponseApi<AccountsPayable>> DeleteAsync(string userId)
        {
            try
            {
                AccountsPayable? accountsPayable = await context.AccountsPayables.Find(x => x.Id == userId && !x.Deleted).FirstOrDefaultAsync();
                if(accountsPayable is null) return new(null, 404, "Contas a Pagar não encontrado");
                accountsPayable.Deleted = true;
                accountsPayable.DeletedAt = DateTime.UtcNow;

                await context.AccountsPayables.ReplaceOneAsync(x => x.Id == userId, accountsPayable);

                return new(accountsPayable, 204, "Contas a Pagar excluído com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao excluír Contas a Pagar");
            }
        }
        #endregion
    }
}

