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
    public class CustomerRecipientRepository(AppDbContext context) : ICustomerRecipientRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<CustomerRecipient> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$sort", pagination.PipelineSort),  

                
                new("$addFields", new BsonDocument
                {
                    {"id", new BsonDocument("$toString", "$_id")},
                }),

                MongoUtil.Lookup("addresses", ["$id"], ["$parentId"], "_address", [["deleted", false]], 1),
                MongoUtil.Lookup("customers", ["$contractorId"], ["$_id"], "_customer", [["deleted", false]], 1),
                MongoUtil.Lookup("plans", ["$planId"], ["$_id"], "_plan", [["deleted", false]], 1),
                
                new("$match", pagination.PipelineFilter),
                
                MongoUtil.Lookup("generic_tables", ["$gender"], ["$code"], "_gender", [["deleted", false], ["table", "genero"]], 1),

                new("$addFields", new BsonDocument
                {
                    {"addressId", MongoUtil.First("_address._id")}
                }),

                MongoUtil.Lookup("users", ["$updatedBy"], ["$_id"], "_user", [["deleted", false]], 1),

                new("$addFields", new BsonDocument
                {
                    {"addressId", MongoUtil.ToString("$addressId")},
                    {"type", MongoUtil.First("_customer.type")},
                    {"userName", MongoUtil.First("_user.name")},
                    {"typePlan", MongoUtil.First("_customer.typePlan")},
                    {"customerDocument", MongoUtil.First("_customer.document")},
                    {"genderDescription", MongoUtil.First("_gender.description")},
                    {"planName", MongoUtil.First("_plan.name")},
                    {"address", new BsonDocument
                        {
                            {"id", MongoUtil.ToString("$addressId")},
                            {"street",  MongoUtil.First("_address.street")},
                            {"number", MongoUtil.First("_address.number") },
                            {"complement", MongoUtil.First("_address.complement") },
                            {"neighborhood", MongoUtil.First("_address.neighborhood") },
                            {"city", MongoUtil.First("_address.city") },
                            {"state", MongoUtil.First("_address.state") },
                            {"zipCode", MongoUtil.First("_address.zipCode") },
                            {"parent", MongoUtil.First("_address.parent") },
                            {"parentId", MongoUtil.First("_address.parentId") },
                        }
                    }
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                    {"_address", 0}, 
                    {"_gender", 0}, 
                    {"_plan", 0}, 
                    {"_user", 0}, 
                    {"_customer", 0} 
                }),
                new("$sort", pagination.PipelineSort),
                // new("$skip", pagination.Skip),
                // new("$limit", pagination.Limit)
            };

            List<BsonDocument> results = await context.CustomerRecipients.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Beneficiário");
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
    new("$addFields", new BsonDocument
    {
        {"id", new BsonDocument("$toString", "$_id")},
    }),
    MongoUtil.Lookup("addresses", ["$id"], ["$parentId"], "_address", [["deleted", false]], 1),
    new("$addFields", new BsonDocument
    {
        {"addressId", MongoUtil.First("_address._id")},
    }),
    new("$addFields", new BsonDocument
    {
        {"addressId", MongoUtil.ToString("$addressId")},
        {"address", new BsonDocument
            {
                {"id", MongoUtil.ToString("$addressId")},
                {"street", MongoUtil.First("_address.street")},
                {"number", MongoUtil.First("_address.number")},
                {"complement", MongoUtil.First("_address.complement")},
                {"neighborhood", MongoUtil.First("_address.neighborhood")},
                {"city", MongoUtil.First("_address.city")},
                {"state", MongoUtil.First("_address.state")},
                {"zipCode", MongoUtil.First("_address.zipCode")},
                {"parent", MongoUtil.First("_address.parent")},
                {"parentId", MongoUtil.First("_address.parentId")},
            }
        }
    }),
    new("$project", new BsonDocument
    {
        {"_id", 0},
        {"_address", 0},
    }),
];

            BsonDocument? response = await context.CustomerRecipients.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Beneficiário não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Beneficiário");
        }
    }
    public async Task<ResponseApi<dynamic?>> GetByCPFAggregateAsync(string cpf)
    {
        try
        {
            BsonDocument[] pipeline = [
                new("$match", new BsonDocument{
                    {"cpf", cpf},
                    {"deleted", false}
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"name", 1},
                    {"cpf", 1},
                    {"rapidocId", 1},
                }),
            ];

            BsonDocument? response = await context.CustomerRecipients.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Beneficiário não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Beneficiário");
        }
    }
    public async Task<ResponseApi<CustomerRecipient?>> GetByIdAsync(string id)
    {
        try
        {
            CustomerRecipient? customerRecipient = await context.CustomerRecipients.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(customerRecipient);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Beneficiário");
        }
    }
    public async Task<ResponseApi<CustomerRecipient?>> GetByCodeAccessAsync(string codeAccess)
    {
        try
        {
            CustomerRecipient? customerRecipient = await context.CustomerRecipients.Find(x => x.CodeAccess == codeAccess && !x.ValidatedAccess && x.CodeAccessExpiration > DateTime.UtcNow && !x.Deleted).FirstOrDefaultAsync();
            return new(customerRecipient);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Beneficiário");
        }
    }
    public async Task<ResponseApi<CustomerRecipient?>> GetByRapidocIdAsync(string rapidoc)
    {
        try
        {
            CustomerRecipient? customerRecipient = await context.CustomerRecipients.Find(x => x.RapidocId == rapidoc && !x.Deleted).FirstOrDefaultAsync();
            return new(customerRecipient);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Beneficiário");
        }
    }
    public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<CustomerRecipient> pagination)
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
                    {"name", 1},
                    {"createdAt", 1},
                    {"rapidocId", 1},
                    {"cpf", 1}
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.CustomerRecipients.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Items");
        }
    }
    public async Task<ResponseApi<List<dynamic>>> GetManagerContractorIdAggregationAsync(PaginationUtil<CustomerRecipient> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                
                MongoUtil.Lookup("plans", ["$planId"], ["$_id"], "_plan", [["deleted", false]], 1),
                MongoUtil.Lookup("addresses", ["$id"], ["$parentId"], "_address", [["deleted", false]], 1),

                new("$addFields", new BsonDocument
                {
                    {"addressId", MongoUtil.First("_address._id")}
                }),

                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"planId", 1},
                    {"planName", MongoUtil.First("_plan.name")},
                    {"name", 1},
                    {"createdAt", 1},
                    {"dateOfBirth", 1},
                    {"effectiveDate", 1},
                    {"cpf", 1},
                    {"gender", 1},
                    {"active", 1},
                    {"email", 1},
                    {"phone", 1},
                    {"whatsapp", 1},
                    {"department", 1},
                    {"role", 1},
                    {"bond", 1},
                    {"serviceModuleIds", 1},
                    {"address", new BsonDocument
                        {
                            {"id", MongoUtil.ToString("$addressId")},
                            {"street",  MongoUtil.First("_address.street")},
                            {"number", MongoUtil.First("_address.number") },
                            {"complement", MongoUtil.First("_address.complement") },
                            {"neighborhood", MongoUtil.First("_address.neighborhood") },
                            {"city", MongoUtil.First("_address.city") },
                            {"state", MongoUtil.First("_address.state") },
                            {"zipCode", MongoUtil.First("_address.zipCode") },
                            {"parent", MongoUtil.First("_address.parent") },
                            {"parentId", MongoUtil.First("_address.parentId") },
                        }
                    }
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.CustomerRecipients.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Beneficiários");
        }
    }
    public async Task<ResponseApi<long?>> GetNextCodeAsync()
    {
        try
        {
            long code = await context.CustomerRecipients.Find(x => true).CountDocumentsAsync() + 1;
            return new(code);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Items");
        }
    }
    public async Task<ResponseApi<CustomerRecipient?>> GetByCPFAsync(string cpf, string contractorId)
    {
        try
        {
            CustomerRecipient? customerRecipient = await context.CustomerRecipients.Find(x => x.Cpf == cpf && x.ContractorId == contractorId && !x.Deleted).FirstOrDefaultAsync();
            return new(customerRecipient);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Beneficiário");
        }
    }
    public async Task<ResponseApi<CustomerRecipient?>> GetByPhoneAsync(string phone)
    {
        try
        {
            CustomerRecipient? customerRecipient = await context.CustomerRecipients.Find(x => x.Phone == phone && x.Active && !x.Deleted).FirstOrDefaultAsync();
            return new(customerRecipient);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Beneficiário");
        }
    }
    public async Task<ResponseApi<CustomerRecipient?>> GetByEmailAsync(string email)
    {
        try
        {
            CustomerRecipient? customerRecipient = await context.CustomerRecipients.Find(x => x.Email == email && x.Active && !x.Deleted).FirstOrDefaultAsync();
            return new(customerRecipient);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Beneficiário");
        }
    }
    public async Task<ResponseApi<CustomerRecipient?>> GetByDocumentAsync(string cpf)
    {
        try
        {
            CustomerRecipient? customerRecipient = await context.CustomerRecipients.Find(x => x.Cpf == cpf && !x.Deleted && x.Active).FirstOrDefaultAsync();
            return new(customerRecipient);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Beneficiário");
        }
    }
    public async Task<ResponseApi<CustomerRecipient?>> GetByCPFImportAsync(string cpf, string contractorId)
    {
        try
        {
            CustomerRecipient? customerRecipient = await context.CustomerRecipients.Find(x => x.Cpf == cpf && x.ContractorId == contractorId).FirstOrDefaultAsync();
            return new(customerRecipient);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Beneficiário");
        }
    }
    public async Task<ResponseApi<List<CustomerRecipient>>> GetPeriodAsync(int month, int year, string contractorId)
    {
        try
        {
            List<CustomerRecipient> customerRecipients = await context.CustomerRecipients.Find(x => x.CreatedAt.Date.Month == month && x.CreatedAt.Date.Year == year && x.ContractorId == contractorId).ToListAsync();
            return new(customerRecipients);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Beneficiário");
        }
    }
    public async Task<ResponseApi<List<CustomerRecipient>>> GetContractIdAsync(string contractorId)
    {
        try
        {
            List<CustomerRecipient> customerRecipients = await context.CustomerRecipients.Find(x => x.ContractorId == contractorId).ToListAsync();
            return new(customerRecipients);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Beneficiário");
        }
    }
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<CustomerRecipient> pagination)
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

        List<BsonDocument> results = await context.CustomerRecipients.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<CustomerRecipient?>> CreateAsync(CustomerRecipient customerRecipient)
    {
        try
        {
            await context.CustomerRecipients.InsertOneAsync(customerRecipient);

            return new(customerRecipient, 201, "Beneficiário criado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Beneficiário");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<CustomerRecipient?>> UpdateAsync(CustomerRecipient customerRecipient)
    {
        try
        {
            await context.CustomerRecipients.ReplaceOneAsync(x => x.Id == customerRecipient.Id, customerRecipient);

            return new(customerRecipient, 201, "Beneficiário atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Beneficiário");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<CustomerRecipient>> DeleteAsync(string id)
    {
        try
        {
            CustomerRecipient? customerRecipient = await context.CustomerRecipients.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(customerRecipient is null) return new(null, 404, "Beneficiário não encontrado");
            customerRecipient.Deleted = true;
            customerRecipient.DeletedAt = DateTime.UtcNow;

            await context.CustomerRecipients.ReplaceOneAsync(x => x.Id == id, customerRecipient);

            return new(customerRecipient, 204, "Beneficiário excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Beneficiário");
        }
    }
    #endregion
}
}