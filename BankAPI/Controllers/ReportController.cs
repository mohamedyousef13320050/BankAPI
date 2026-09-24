using BankSystem.BL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
    /// <summary>
    /// Admin financial reports and system metrics analytics.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class ReportController : ControllerBase
    {
        private readonly IReportBL reportBL;

        public ReportController(IReportBL reportBL)
        {
            this.reportBL = reportBL;
        }

        /// <summary>
        /// Get complete Admin financial report (Totals for employees, customers, accounts, deposits, withdrawals, transfers, and balances).
        /// </summary>
        [HttpGet("dashboard")]
        public IActionResult GetDashboardReport()
        {
            var report = reportBL.GetDashboardReport();
            return Ok(report);
        }

        /// <summary>
        /// Get summary stats for dashboard cards.
        /// </summary>
        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            var report = reportBL.GetDashboardReport();
            return Ok(new
            {
                Employees    = report.TotalEmployees,
                Branches     = report.TotalBranches,
                Accounts     = report.TotalAccounts,
                Transactions = report.TotalTransactions,
                TotalBalance = report.TotalBalance,
                TotalDeposits = report.TotalDeposits,
                TotalWithdrawals = report.TotalWithdrawals,
                TotalTransfers = report.TotalTransfers
            });
        }
    }
}
