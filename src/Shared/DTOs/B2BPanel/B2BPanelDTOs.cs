using api_slim.src.Models;

namespace api_slim.src.Shared.DTOs
{
    // ─── Mass Movement ───────────────────────────────────────────────────────────
    public class CreateB2BMassMovementDTO : Request
    {
        public string CustomerId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ProgramId { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public B2BBeneficiaryData? Beneficiary { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }

    public class UpdateB2BMassMovementDTO : Request
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ProgramId { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public B2BBeneficiaryData? Beneficiary { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }

    // ─── Invoice ─────────────────────────────────────────────────────────────────
    public class CreateB2BInvoiceDTO : Request
    {
        public string CustomerId { get; set; } = string.Empty;
        public int ReferenceMonth { get; set; }
        public int ReferenceYear { get; set; }
         public string Status { get; set; }
        public DateTime CycleStart { get; set; }
        public DateTime CycleEnd { get; set; }
        public decimal TotalAmount { get; set; }
        public int BeneficiaryCount { get; set; }
        public DateTime? DueDate { get; set; }
        // ClosingDate é calculado automaticamente: último dia do mês de referência
        public List<B2BInvoiceItem> Items { get; set; } = [];

        /// <summary>Retorna o último dia do mês de referência.</summary>
        public DateTime GetClosingDate()
        {
            int lastDay = DateTime.DaysInMonth(ReferenceYear, ReferenceMonth);
            return new DateTime(ReferenceYear, ReferenceMonth, lastDay);
        }
    }

    public class UpdateB2BInvoiceDTO : Request
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int BeneficiaryCount { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? PaidAt { get; set; }
        public List<B2BInvoiceItem> Items { get; set; } = [];
    }

    // ─── Attachment ──────────────────────────────────────────────────────────────
    public class CreateB2BAttachmentDTO : Request
    {
        public string CustomerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public bool Required { get; set; } = false;
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdateB2BAttachmentDTO : Request
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public bool Required { get; set; } = false;
        public string Notes { get; set; } = string.Empty;
    }
}
