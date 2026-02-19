using System.Globalization;
using api_slim.src.Configuration;
using api_slim.src.Interfaces;
using api_slim.src.Models;
using api_slim.src.Models.Base;
using api_slim.src.Shared.DTOs.Dashboard;
using MongoDB.Driver;

namespace api_slim.src.Repository
{
    public class DashboardRepository(AppDbContext context) : IDashboardRepository
    {
        // ─────────────────────────────────────────────────────────────────
        //  EXISTENTE — não alterado
        // ─────────────────────────────────────────────────────────────────
        public async Task<ResponseApi<dynamic>> GetFirstCardAsync()
        {
            try
            {
                DateTime now = DateTime.UtcNow;

                DateTime startOfMonth        = new(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime endOfMonth          = startOfMonth.AddMonths(1).AddTicks(-1);
                DateTime startOfYear         = new(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime endOfYear           = startOfYear.AddYears(1).AddTicks(-1);
                DateTime previousStartDate   = startOfMonth.AddMonths(-1);
                DateTime previousEndDate     = endOfMonth.AddMonths(-1);
                DateTime previousStartOfYear = startOfYear.AddYears(-1);
                DateTime previousEndOfYear   = endOfYear.AddYears(-1);

                List<AccountsReceivable> arPrevMes  = await context.AccountsReceivables.Find(x => !x.Deleted && x.BillingDate >= previousStartDate   && x.BillingDate <= previousEndDate).ToListAsync();
                List<AccountsReceivable> arMes      = await context.AccountsReceivables.Find(x => !x.Deleted && x.BillingDate >= startOfMonth          && x.BillingDate <= endOfMonth).ToListAsync();
                List<AccountsReceivable> arPrevAno  = await context.AccountsReceivables.Find(x => !x.Deleted && x.BillingDate >= previousStartOfYear   && x.BillingDate <= previousEndOfYear).ToListAsync();
                List<AccountsReceivable> arAno      = await context.AccountsReceivables.Find(x => !x.Deleted && x.BillingDate >= startOfYear            && x.BillingDate <= endOfYear).ToListAsync();

                long customer  = await context.Customers.Find(x => !x.Deleted).CountDocumentsAsync();
                long recipient = await context.CustomerRecipients.Find(x => !x.Deleted).CountDocumentsAsync();

                decimal prevMes  = arPrevMes.Sum(x => x.Value);
                decimal mes      = arMes.Sum(x => x.Value);
                decimal prevAno  = arPrevAno.Sum(x => x.Value);
                decimal ano      = arAno.Sum(x => x.Value);

                decimal pctMes = prevMes  > 0 ? Math.Round(((mes - prevMes)   / prevMes)  * 100, 2) : (mes  > 0 ? 100 : 0);
                decimal pctAno = prevAno  > 0 ? Math.Round(((ano - prevAno)   / prevAno)  * 100, 2) : (ano  > 0 ? 100 : 0);

                dynamic obj = new
                {
                    accountReceivableMonth = mes,
                    percentageChangeMonth  = pctMes,
                    accountReceivableYear  = ano,
                    percentageChangeYear   = pctAno,
                    customer,
                    recipient
                };

                return new(obj);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        // ─────────────────────────────────────────────────────────────────
        //  NOVO — summary completo para o dashboard
        // ─────────────────────────────────────────────────────────────────
        public async Task<ResponseApi<DashboardSummaryDto>> GetSummaryAsync()
        {
            try
            {
                DateTime now          = DateTime.UtcNow;
                DateTime startOfMonth = new(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime endOfMonth   = startOfMonth.AddMonths(1).AddTicks(-1);

                // ── 1. Últimas 5 consultas (passadas, mais recentes primeiro) ──────
                var ultimasRaw = await context.InPersons
                    .Find(x => !x.Deleted && x.Date.HasValue && x.Date!.Value <= now)
                    .SortByDescending(x => x.Date)
                    .Limit(5)
                    .ToListAsync();

                // ── 2. Próximas 5 consultas (futuras, mais próximas primeiro) ──────
                var proximasRaw = await context.InPersons
                    .Find(x => !x.Deleted && x.Date.HasValue && x.Date!.Value > now)
                    .SortBy(x => x.Date)
                    .Limit(5)
                    .ToListAsync();

                // ── 3. Contas a pagar no mês ──────────────────────────────────────
                var apMes = await context.AccountsPayables
                    .Find(x => !x.Deleted && x.DueDate >= startOfMonth && x.DueDate <= endOfMonth)
                    .ToListAsync();
                decimal contasPagarMes = apMes.Sum(x => x.Value);

                // ── 4. Contas a receber em aberto (sem baixa) ─────────────────────
                var arAberto = await context.AccountsReceivables
                    .Find(x => !x.Deleted && x.LowDate == null)
                    .ToListAsync();
                decimal contasReceberAberto = arAberto.Sum(x => x.Value);

                // ── 5. Ticket médio e beneficiários ativos ────────────────────────
                var beneficiariosAtivos = await context.CustomerRecipients
                    .Find(x => !x.Deleted && x.Active)
                    .ToListAsync();

                int     totalAtivos   = beneficiariosAtivos.Count;
                decimal ticketMedio   = totalAtivos > 0
                    ? Math.Round(beneficiariosAtivos.Where(r => r.Total > 0).Select(r => r.Total).DefaultIfEmpty(0).Average(), 2)
                    : 0;

                // ── 6. Consultas no mês (total) ───────────────────────────────────
                long consultasMes = await context.InPersons
                    .Find(x => !x.Deleted && x.Date >= startOfMonth && x.Date <= endOfMonth)
                    .CountDocumentsAsync();

                // ── 7. Série financeira — últimos 6 meses ─────────────────────────
                var financeiroMensal = new List<MesFinanceiroDto>();
                var ptBr = new CultureInfo("pt-BR");

                for (int i = 5; i >= 0; i--)
                {
                    DateTime mStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-i);
                    DateTime mEnd   = mStart.AddMonths(1).AddTicks(-1);

                    decimal receita = (await context.AccountsReceivables
                        .Find(x => !x.Deleted && x.BillingDate >= mStart && x.BillingDate <= mEnd)
                        .ToListAsync()).Sum(x => x.Value);

                    decimal despesa = (await context.AccountsPayables
                        .Find(x => !x.Deleted && x.DueDate >= mStart && x.DueDate <= mEnd)
                        .ToListAsync()).Sum(x => x.Value);

                    financeiroMensal.Add(new MesFinanceiroDto
                    {
                        Mes     = mStart.ToString("MMM", ptBr),
                        Receita = receita,
                        Despesa = despesa,
                    });
                }

                // ── 8. Distribuição de status (mês atual) ─────────────────────────
                var consultasMesLista = await context.InPersons
                    .Find(x => !x.Deleted && x.Date >= startOfMonth && x.Date <= endOfMonth)
                    .ToListAsync();

                var porStatus = consultasMesLista
                    .GroupBy(x => string.IsNullOrEmpty(x.Status) ? "Indefinido" : x.Status)
                    .Select(g => new StatusConsultaDto { Status = g.Key, Total = g.Count() })
                    .OrderByDescending(x => x.Total)
                    .ToList();

                // ── 9. Enriquecer consultas com nomes (beneficiário, profissional, módulo) ──
                var allInPersons = ultimasRaw.Concat(proximasRaw).ToList();

                var recipientIds    = allInPersons.Select(x => x.RecipientId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
                var professionalIds = allInPersons.Select(x => x.ProfessionalId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
                var moduleIds       = allInPersons.Select(x => x.ServiceModuleId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();

                var recipientNames = recipientIds.Count > 0
                    ? (await context.CustomerRecipients.Find(x => recipientIds.Contains(x.Id)).ToListAsync())
                       .ToDictionary(x => x.Id, x => x.Name)
                    : new Dictionary<string, string>();

                var professionalNames = professionalIds.Count > 0
                    ? (await context.Professionals.Find(x => professionalIds.Contains(x.Id)).ToListAsync())
                       .ToDictionary(x => x.Id, x => x.Name)
                    : new Dictionary<string, string>();

                var moduleNames = moduleIds.Count > 0
                    ? (await context.ServiceModules.Find(x => moduleIds.Contains(x.Id)).ToListAsync())
                       .ToDictionary(x => x.Id, x => x.Name)
                    : new Dictionary<string, string>();

                ConsultaItemDto Map(InPerson ip) => new()
                {
                    Id           = ip.Id,
                    Beneficiario = recipientNames.GetValueOrDefault(ip.RecipientId, "—"),
                    Profissional = professionalNames.GetValueOrDefault(ip.ProfessionalId, "—"),
                    Modulo       = moduleNames.GetValueOrDefault(ip.ServiceModuleId, "—"),
                    Data         = ip.Date.HasValue ? ip.Date.Value.ToString("dd/MM/yyyy") : "—",
                    Hora         = ip.Hour,
                    Status       = ip.Status,
                };

                var summary = new DashboardSummaryDto
                {
                    UltimasConsultas       = ultimasRaw.Select(Map).ToList(),
                    ProximasConsultas      = proximasRaw.Select(Map).ToList(),
                    ContasPagarMes         = contasPagarMes,
                    ContasReceberAberto    = contasReceberAberto,
                    TicketMedio            = ticketMedio,
                    ConsultasMes           = (int)consultasMes,
                    BeneficiariosAtivos    = totalAtivos,
                    FinanceiroMensal       = financeiroMensal,
                    ConsultasPorStatus     = porStatus,
                };

                return new(summary);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
    }
}
