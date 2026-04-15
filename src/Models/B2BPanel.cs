using api_slim.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_slim.src.Models
{
    // ─── Movimentação de Massa ───────────────────────────────────────────────────
    public class B2BMassMovement : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("customerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; } = string.Empty;

        [BsonElement("customerName")]
        public string CustomerName { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("status")]
        public string Status { get; set; } = "Pendente";

        [BsonElement("programId")]
        public string ProgramId { get; set; } = string.Empty;

        [BsonElement("programName")]
        public string ProgramName { get; set; } = string.Empty;

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;

        [BsonElement("processedAt")]
        public DateTime? ProcessedAt { get; set; }

        [BsonElement("processedBy")]
        public string ProcessedBy { get; set; } = string.Empty;

        [BsonElement("beneficiary")]
        public B2BBeneficiaryData? Beneficiary { get; set; }

        [BsonElement("fileUrl")]
        public string FileUrl { get; set; } = string.Empty;

        [BsonElement("fileName")]
        public string FileName { get; set; } = string.Empty;
    }

    public class B2BBeneficiaryData
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("cpf")]
        public string Cpf { get; set; } = string.Empty;

        [BsonElement("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [BsonElement("gender")]
        public string Gender { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("whatsapp")]
        public string Whatsapp { get; set; } = string.Empty;

        [BsonElement("department")]
        public string Department { get; set; } = string.Empty;

        [BsonElement("role")]
        public string Role { get; set; } = string.Empty;

        [BsonElement("planId")]
        public string PlanId { get; set; } = string.Empty;

        [BsonElement("programId")]
        public string ProgramId { get; set; } = string.Empty;
    }

    // ─── Painel de Faturas B2B ───────────────────────────────────────────────────
    public class B2BInvoice : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("customerId")]
        public string CustomerId { get; set; } = string.Empty;

        [BsonElement("customerName")]
        public string CustomerName { get; set; } = string.Empty;

        [BsonElement("referenceMonth")]
        public int ReferenceMonth { get; set; }

        [BsonElement("referenceYear")]
        public int ReferenceYear { get; set; }

        [BsonElement("cycleStart")]
        public DateTime CycleStart { get; set; }

        [BsonElement("cycleEnd")]
        public DateTime? CycleEnd { get; set; }

        // ── NOVO: data de corte/fechamento (último dia do mês de referência) ──
        [BsonElement("closingDate")]
        public DateTime? ClosingDate { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Aberta";

        [BsonElement("totalAmount")]
        public decimal TotalAmount { get; set; }

        [BsonElement("beneficiaryCount")]
        public int BeneficiaryCount { get; set; }

        [BsonElement("dueDate")]
        public DateTime? DueDate { get; set; }

        [BsonElement("paidAt")]
        public DateTime? PaidAt { get; set; }

        [BsonElement("items")]
        public List<B2BInvoiceItem> Items { get; set; } = [];
    }

    public class B2BInvoiceItem
    {
        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("recipientName")]
        public string RecipientName { get; set; } = string.Empty;

        [BsonElement("recipientCpf")]
        public string RecipientCpf { get; set; } = string.Empty;

        [BsonElement("planName")]
        public string PlanName { get; set; } = string.Empty;

        [BsonElement("amount")]
        public decimal Amount { get; set; }
    }

    // ─── Anexo B2B ───────────────────────────────────────────────────────────────
    public class B2BAttachment : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("customerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; } = string.Empty;

        [BsonElement("customerName")]
        public string CustomerName { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("fileUrl")]
        public string FileUrl { get; set; } = string.Empty;

        [BsonElement("fileName")]
        public string FileName { get; set; } = string.Empty;

        [BsonElement("fileType")]
        public string FileType { get; set; } = string.Empty;

        [BsonElement("fileSize")]
        public long FileSize { get; set; }

        [BsonElement("required")]
        public bool Required { get; set; } = false;

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;
    }
}
