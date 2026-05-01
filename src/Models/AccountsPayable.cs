using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models
{
    public class AccountsPayable : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;

        [BsonElement("referenceCode")]
        public string ReferenceCode { get; set; } = string.Empty;

        [BsonElement("supplierId")]
        public string SupplierId { get; set; } = string.Empty;

        [BsonElement("costCenter")]
        public string CostCenter { get; set; } = string.Empty;

        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;

        [BsonElement("typeOfPeriodicity")]
        public string TypeOfPeriodicity { get; set; } = string.Empty;

        [BsonElement("quantityOfPeriodicity")]
        public int QuantityOfPeriodicity { get; set; }

        [BsonElement("subTotal")]
        public decimal SubTotal { get; set; }

        [BsonElement("total")]
        public decimal Total { get; set; }

        [BsonElement("discount")]
        public decimal Discount { get; set; }

        [BsonElement("paymentMethod")]
        public string PaymentMethod { get; set; } = string.Empty;

        [BsonElement("value")]
        public decimal Value { get; set; } = 0;

        [BsonElement("lowValue")]
        public decimal LowValue { get; set; } = 0;

        [BsonElement("fines")]
        public decimal Fines { get; set; } = 0;

        [BsonElement("fees")]
        public decimal Fees { get; set; } = 0;

        [BsonElement("lowDate")]
        public DateTime? LowDate { get; set; }

        [BsonElement("dueDate")]
        public DateTime? DueDate { get; set; }

        [BsonElement("billingDate")]
        public DateTime? BillingDate { get; set; }

        [BsonElement("billingPeriod")]
        public string BillingPeriod { get; set; } = string.Empty;

        [BsonElement("billing")]
        public string Billing { get; set; } = string.Empty;
    }
}