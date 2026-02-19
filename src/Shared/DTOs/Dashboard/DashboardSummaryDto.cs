namespace api_slim.src.Shared.DTOs.Dashboard
{
    public class ConsultaItemDto
    {
        public string Id            { get; set; } = string.Empty;
        public string Beneficiario  { get; set; } = string.Empty;
        public string Profissional  { get; set; } = string.Empty;
        public string Modulo        { get; set; } = string.Empty;
        public string Data          { get; set; } = string.Empty;   // "dd/MM/yyyy"
        public string Hora          { get; set; } = string.Empty;
        public string Status        { get; set; } = string.Empty;
    }

    public class MesFinanceiroDto
    {
        public string  Mes      { get; set; } = string.Empty;  // "Jan", "Fev"…
        public decimal Receita  { get; set; }
        public decimal Despesa  { get; set; }
    }

    public class StatusConsultaDto
    {
        public string Status { get; set; } = string.Empty;
        public int    Total  { get; set; }
    }

    public class DashboardSummaryDto
    {
        // Listas de consultas
        public List<ConsultaItemDto> UltimasConsultas   { get; set; } = [];
        public List<ConsultaItemDto> ProximasConsultas  { get; set; } = [];

        // KPIs financeiros extras
        public decimal ContasPagarMes       { get; set; }
        public decimal ContasReceberAberto  { get; set; }
        public decimal TicketMedio          { get; set; }

        // KPIs de atendimento
        public int ConsultasMes             { get; set; }
        public int BeneficiariosAtivos      { get; set; }

        // Séries para gráficos
        public List<MesFinanceiroDto>  FinanceiroMensal       { get; set; } = [];
        public List<StatusConsultaDto> ConsultasPorStatus     { get; set; } = [];
    }
}
