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
    public class AppointmentTelemedicineRepository(AppDbContext context) : IAppointmentTelemedicineRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<AppointmentTelemedicine> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),

                    MongoUtil.Lookup("customer_recipients", ["$beneficiaryCPF"], ["$cpf"], "_recipient", [["deleted", false]], 1),
                    
                    new("$addFields", new BsonDocument {
                        {"beneficiaryName", MongoUtil.First("_recipient.name")}
                    }),

                    new("$match", pagination.PipelineFilter),
                    
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"id", MongoUtil.ToString("$_id")},
                        {"createdAt", 1},
                        {"date", 1},
                        {"hour", 1},
                        {"status", 1},
                        {"specialtyName", 1},
                        {"professionalName", 1},
                        {"beneficiaryName", 1},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.AppointmentTelemedicines.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
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
                    }),
                ];

                BsonDocument? response = await context.AppointmentTelemedicines.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Agendamento não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<AppointmentTelemedicine?>> GetByIdAsync(string id)
        {
            try
            {
                AppointmentTelemedicine? appointmentTelemedicine = await context.AppointmentTelemedicines.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(appointmentTelemedicine);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<AppointmentTelemedicine?>> GetByAppointmentUuidAsync(string uuid)
        {
            try
            {
                AppointmentTelemedicine? appointmentTelemedicine = await context.AppointmentTelemedicines.Find(x => x.AppointmentUuid == uuid && !x.Deleted).FirstOrDefaultAsync();
                return new(appointmentTelemedicine);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<AppointmentTelemedicine> pagination)
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

            List<BsonDocument> results = await context.AppointmentTelemedicines.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<AppointmentTelemedicine?>> CreateAsync(AppointmentTelemedicine appointmentTelemedicine)
        {
            try
            {
                await context.AppointmentTelemedicines.InsertOneAsync(appointmentTelemedicine);

                return new(appointmentTelemedicine, 201, "Agendamento criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<AppointmentTelemedicine?>> UpdateAsync(AppointmentTelemedicine appointmentTelemedicine)
        {
            try
            {
                await context.AppointmentTelemedicines.ReplaceOneAsync(x => x.Id == appointmentTelemedicine.Id, appointmentTelemedicine);

                return new(appointmentTelemedicine, 201, "Agendamento atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<AppointmentTelemedicine>> DeleteAsync(string id)
        {
            try
            {
                AppointmentTelemedicine? appointmentTelemedicine = await context.AppointmentTelemedicines.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if (appointmentTelemedicine is null) return new(null, 404, "Agendamento não encontrado");
                appointmentTelemedicine.Deleted = true;
                appointmentTelemedicine.DeletedAt = DateTime.UtcNow;

                await context.AppointmentTelemedicines.ReplaceOneAsync(x => x.Id == id, appointmentTelemedicine);

                return new(appointmentTelemedicine, 204, "Agendamento excluído com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}