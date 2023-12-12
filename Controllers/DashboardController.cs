using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTrack.Models;
using System.Globalization;

namespace MoneyTrack.Controllers {
    public class DashboardController : Controller {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context) {
            _context = context;
        }


        public async Task<ActionResult> Index() {

            //Last 31 days
            DateTime StartDate = DateTime.Today.AddDays(-30);
            DateTime EndDate = DateTime.Today;

            //Filtro com uso da data da FK
            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();

            //Total income
            int TotalIncome = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .Sum(j => j.Amount);
            CultureInfo cultureInc = CultureInfo.CreateSpecificCulture("pt-BR");
            cultureInc.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.TotalIncome = String.Format(cultureInc, "{0:C0}", TotalIncome);

            //Total expense
            int TotalExpense = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);
            CultureInfo cultureExp = CultureInfo.CreateSpecificCulture("pt-BR");
            cultureExp.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.TotalExpense = String.Format(cultureExp, "{0:C0}", TotalExpense);

            //Balance
            int Balance = TotalIncome - TotalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("pt-BR");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(culture, "{0:C0}", Balance);

            //Lista para o grafico doughnut
            ViewBag.DoughnutChartData = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
                }).ToList();

            return View();
        }
    }
}
