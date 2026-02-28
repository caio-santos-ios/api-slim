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
    public class SellerRepresentativeRepository(AppDbContext context) : ISellerRepresentativeRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<SellerRepresentative> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),

                new("$addFields", new BsonDocument {
                    {"id", new BsonDocument("$toString", "$_id")}
                }),

                MongoUtil.Lookup("addresses", ["$parentId"], ["$id"], "_address", [["deleted", false], ["parent", "seller-representative"]], 1),
                MongoUtil.Lookup("addresses", ["$parentId"], ["$id"], "_addressResponsible", [["deleted", false], ["parent", "seller-representative-responsible"]], 1),
                MongoUtil.Lookup("sellers", ["$parentId"], ["$id"], "_seller", [["deleted", false]], 1),
                
                new("$addFields", new BsonDocument {
                    {"myAddress", MongoUtil.First("_address")},
                    {"_address_responsible", MongoUtil.First("_addressResponsible")},
                    {"_seller", MongoUtil.First("_seller")},
                }),

                MongoUtil.Lookup("addresses", ["$_seller._id"], ["$_id"], "_sellerAddress", [["deleted", false]], 1),
                
                new("$addFields", new BsonDocument {
                    {"_address_seller", MongoUtil.First("_sellerAddress")},
                }),

                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"cnpj", 1},
                    {"tradeName", 1},
                    {"corporateName", 1},
                    {"phone", 1},
                    {"whatsapp", 1},
                    {"email", 1},
                    {"effectiveDate", 1},
                    {"bank", 1},
                    {"createdAt", 1},
                    {"_address", 1},
                    {"responsible", new BsonDocument 
                        {
                            {
                                "address", new BsonDocument
                                {
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
                                }
                            },
                            {"dateOfBirth", 1},
                            {"gender", 1},
                            {"name", 1},
                            {"cpf", 1},
                            {"rg", 1},
                            {"phone", 1},
                            {"whatsapp", 1},
                            {"email", 1},
                            {"notes", 1},
                        }
                    },
                    {"address", new BsonDocument
                        {
                            {"id", new BsonDocument("$ifNull", new BsonArray { new BsonDocument("$toString", "$myAddress._id"), "" })},
                            {"street", new BsonDocument("$ifNull", new BsonArray { "$myAddress.street", "" })},
                            {"number", new BsonDocument("$ifNull", new BsonArray {"$myAddress.number" , "" })},
                            {"complement", new BsonDocument("$ifNull", new BsonArray {"$myAddress.complement" , "" })},
                            {"neighborhood", new BsonDocument("$ifNull", new BsonArray {"$myAddress.neighborhood" , "" })},
                            {"city", new BsonDocument("$ifNull", new BsonArray {"$myAddress.city" , "" })},
                            {"state", new BsonDocument("$ifNull", new BsonArray {"$myAddress.state" , "" })},
                            {"zipCode", new BsonDocument("$ifNull", new BsonArray {"$myAddress.zipCode" , "" })},
                            {"parent", new BsonDocument("$ifNull", new BsonArray {"$myAddress.parent" , "" })},
                            {"parentId", new BsonDocument("$ifNull", new BsonArray {"$myAddress.parentId" , "" })},
                        }
                    },
                    {
                        "seller", new BsonDocument
                        {
                            {"id", new BsonDocument("$ifNull", new BsonArray { new BsonDocument("$toString", "$_seller._id"), "" })},
                            {"name", new BsonDocument("$ifNull", new BsonArray { "$_seller.name", "" })},
                            {"email", new BsonDocument("$ifNull", new BsonArray { "$_seller.email", "" })},
                            {"phone", new BsonDocument("$ifNull", new BsonArray { "$_seller.phone", "" })},
                            {"cpf", new BsonDocument("$ifNull", new BsonArray { "$_seller.cpf", "" })},
                            {"notes", new BsonDocument("$ifNull", new BsonArray { "$_seller.notes", "" })},
                            {"type", new BsonDocument("$ifNull", new BsonArray { "$_seller.type", "" })},
                            {"address", new BsonDocument
                                {
                                    {"id", new BsonDocument("$toString", "$_address_seller._id")},
                                    {"street", new BsonDocument("$ifNull", new BsonArray { "$_address_seller.street", "" })},
                                    {"number", new BsonDocument("$ifNull", new BsonArray {"$_address_seller.number" , "" })},
                                    {"complement", new BsonDocument("$ifNull", new BsonArray {"$_address_seller.complement" , "" })},
                                    {"neighborhood", new BsonDocument("$ifNull", new BsonArray {"$_address_seller.neighborhood" , "" })},
                                    {"city", new BsonDocument("$ifNull", new BsonArray {"$_address_seller.city" , "" })},
                                    {"state", new BsonDocument("$ifNull", new BsonArray {"$_address_seller.state" , "" })},
                                    {"zipCode", new BsonDocument("$ifNull", new BsonArray {"$_address_seller.zipCode" , "" })},
                                    {"parent", new BsonDocument("$ifNull", new BsonArray {"$_address_seller.parent" , "" })},
                                    {"parentId", new BsonDocument("$ifNull", new BsonArray {"$_address_seller.parentId" , "" })},
                                }
                            },
                        }
                    }
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.SellerRepresentatives.Aggregate<BsonDocument>(pipeline).ToListAsync();
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

            BsonDocument? response = await context.SellerRepresentatives.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Item não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item");
        }
    }
    
    public async Task<ResponseApi<SellerRepresentative?>> GetByIdAsync(string id)
    {
        try
        {
            SellerRepresentative? billing = await context.SellerRepresentatives.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(billing);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item");
        }
    }
    
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<SellerRepresentative> pagination)
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

        List<BsonDocument> results = await context.SellerRepresentatives.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<SellerRepresentative?>> CreateAsync(SellerRepresentative billing)
    {
        try
        {
            await context.SellerRepresentatives.InsertOneAsync(billing);

            return new(billing, 201, "Item criado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Item");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<SellerRepresentative?>> UpdateAsync(SellerRepresentative billing)
    {
        try
        {
            await context.SellerRepresentatives.ReplaceOneAsync(x => x.Id == billing.Id, billing);

            return new(billing, 201, "Item atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Item");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<SellerRepresentative>> DeleteAsync(string id)
    {
        try
        {
            SellerRepresentative? billing = await context.SellerRepresentatives.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(billing is null) return new(null, 404, "Item não encontrado");
            billing.Deleted = true;
            billing.DeletedAt = DateTime.UtcNow;

            await context.SellerRepresentatives.ReplaceOneAsync(x => x.Id == id, billing);

            return new(billing, 204, "Item excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Item");
        }
    }
    #endregion
}
}