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
    public class CustomerRepository(AppDbContext context) : ICustomerRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Customer> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),  
                new("$addFields", new BsonDocument
                {
                    {"id", new BsonDocument("$toString", "$_id")},
                }),
                
                MongoUtil.Lookup("customer_recipients", ["$id"], ["$contractorId"], "_recipient", [["deleted", false]], 1),
                MongoUtil.Lookup("customer_recipients", ["$id"], ["$contractorId"], "_recipient", [["deleted", false]], 1),
                new("$addFields", new BsonDocument
                {
                    {"planId", MongoUtil.First("_recipient.planId")},
                }),
                new("$addFields", new BsonDocument
                {
                    {"recipientName", MongoUtil.First("_recipient.name")},
                    {"recipientCode", MongoUtil.First("_recipient.code")},
                    {"recipientCPF", MongoUtil.First("_recipient.cpf")},
                }),
                
                new("$project", new BsonDocument
                {
                    {"_id", 0},                     
                    {"_recipient", 0},                     
                }),
                new("$sort", pagination.PipelineSort),
                // new("$skip", pagination.Skip),
                // new("$limit", pagination.Limit),

            };

            List<BsonDocument> results = await context.Customers.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Clientes");
        }
    }
    public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Customer> pagination)
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
                    {"corporateName", 1},
                    {"type", 1},
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Customers.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Clientes");
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
                
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "addresses" },

                    { "let", new BsonDocument("profId", "$id") },

                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match", new BsonDocument
                            {
                                { "$expr", new BsonDocument("$and", new BsonArray
                                    {
                                        new BsonDocument("$eq", new BsonArray
                                        {
                                            "$parentId",
                                            "$$profId"
                                        }),

                                        new BsonDocument("$eq", new BsonArray
                                        {
                                            "$parent",
                                            "customer-contract"
                                        })
                                    })
                                }
                            })
                        }
                    },

                    { "as", "_address" }
                }),

                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$_address" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "addresses" },

                    { "let", new BsonDocument("profId", "$id") },

                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match", new BsonDocument
                            {
                                { "$expr", new BsonDocument("$and", new BsonArray
                                    {
                                        new BsonDocument("$eq", new BsonArray
                                        {
                                            "$parentId",
                                            "$$profId"
                                        }),

                                        new BsonDocument("$eq", new BsonArray
                                        {
                                            "$parent",
                                            "customer-contractor-responsible"
                                        })
                                    })
                                }
                            })
                        }
                    },

                    { "as", "_address_responsible" }
                }),

                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$_address_responsible" },
                    { "preserveNullAndEmptyArrays", true }
                }),

                new("$addFields", new BsonDocument
                {
                    {"address", new BsonDocument
                        {
                            {"id", new BsonDocument("$ifNull", new BsonArray { new BsonDocument("$toString", "$_address._id"), "" })},
                            {"street", new BsonDocument("$ifNull", new BsonArray { "$_address.street", "" })},
                            {"number", new BsonDocument("$ifNull", new BsonArray {"$_address.number" , "" })},
                            {"complement", new BsonDocument("$ifNull", new BsonArray {"$_address.complement" , "" })},
                            {"neighborhood", new BsonDocument("$ifNull", new BsonArray {"$_address.neighborhood" , "" })},
                            {"city", new BsonDocument("$ifNull", new BsonArray {"$_address.city" , "" })},
                            {"state", new BsonDocument("$ifNull", new BsonArray {"$_address.state" , "" })},
                            {"zipCode", new BsonDocument("$ifNull", new BsonArray {"$_address.zipCode" , "" })},
                            {"parent", new BsonDocument("$ifNull", new BsonArray {"$_address.parent" , "" })},
                            {"parentId", new BsonDocument("$ifNull", new BsonArray {"$_address.parentId" , "" })},
                        }
                    },
                    {
                        "responsible", new BsonDocument
                        {
                            {"address", new BsonDocument {
                                {"id", new BsonDocument("$ifNull", new BsonArray { new BsonDocument("$toString", "$_address_responsible._id"), "" })},
                                {"street", new BsonDocument("$ifNull", new BsonArray { "$_address_responsible.street", "" })},
                                {"number", new BsonDocument("$ifNull", new BsonArray {"$_address_responsible.number" , "" })},
                                {"complement", new BsonDocument("$ifNull", new BsonArray {"$_address_responsible.complement" , "" })},
                                {"neighborhood", new BsonDocument("$ifNull", new BsonArray {"$_address_responsible.neighborhood" , "" })},
                                {"city", new BsonDocument("$ifNull", new BsonArray {"$_address_responsible.city" , "" })},
                                {"state", new BsonDocument("$ifNull", new BsonArray {"$_address_responsible.state" , "" })},
                                {"zipCode", new BsonDocument("$ifNull", new BsonArray {"$_address_responsible.zipCode" , "" })},
                                {"parent", new BsonDocument("$ifNull", new BsonArray {"$_address_responsible.parent" , "" })},
                                {"parentId", new BsonDocument("$ifNull", new BsonArray {"$_address_responsible.parentId" , "" })},
                            }}
                        }
                    }
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                    {"_address", 0}, 
                    {"_address_responsible", 0}, 
                }),
            ];

            BsonDocument? response = await context.Customers.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Clientes não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Clientes");
        }
    }
    
    public async Task<ResponseApi<Customer?>> GetByIdAsync(string id)
    {
        try
        {
            Customer? customer = await context.Customers.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(customer);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Clientes");
        }
    }
    
    public async Task<ResponseApi<Customer?>> GetByEmailAsync(string email)
    {
        try
        {
            Customer? customer = await context.Customers.Find(x => x.Email == email && !x.Deleted).FirstOrDefaultAsync();
            return new(customer);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Clientes");
        }
    }
    public async Task<ResponseApi<Customer?>> GetByCodeAccessAsync(string codeAccess)
    {
        try
        {
            Customer? customer = await context.Customers.Find(x => x.CodeAccess == codeAccess && !x.Deleted).FirstOrDefaultAsync();
            return new(customer);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar usuário");
        }
    }
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Customer> pagination)
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

        List<BsonDocument> results = await context.Customers.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Customer?>> CreateAsync(Customer customer)
    {
        try
        {
            await context.Customers.InsertOneAsync(customer);

            return new(customer, 201, "Clientes criado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Clientes");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Customer?>> UpdateAsync(Customer customer)
    {
        try
        {
            await context.Customers.ReplaceOneAsync(x => x.Id == customer.Id, customer);

            return new(customer, 201, "Clientes atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Clientes");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Customer>> DeleteAsync(string id)
    {
        try
        {
            Customer? customer = await context.Customers.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(customer is null) return new(null, 404, "Clientes não encontrado");
            customer.Deleted = true;
            customer.DeletedAt = DateTime.UtcNow;

            await context.Customers.ReplaceOneAsync(x => x.Id == id, customer);

            return new(customer, 204, "Clientes excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Clientes");
        }
    }
    #endregion
}
}