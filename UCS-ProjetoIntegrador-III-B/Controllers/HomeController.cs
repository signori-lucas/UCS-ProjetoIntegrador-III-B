using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UCS_ProjetoIntegrador_III_B.Models;
using UCS_ProjetoIntegrador_III_B.Data;

namespace UCS_ProjetoIntegrador_III_B.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseService _db;
        private readonly IWebHostEnvironment _env;

        public HomeController(DatabaseService db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            bool canConnect = false;
            bool hasPendingMigrations = false;

            try
            {
                canConnect = await _db.CanConnectAsync();
                hasPendingMigrations = !await _db.HasTablesAsync();
            }
            catch
            {
                // If any exception occurs, treat as DB not available / needs creation
                canConnect = false;
                hasPendingMigrations = true;
            }

            ViewBag.ShowCreateDatabaseButton = !canConnect || hasPendingMigrations;
            ViewBag.ShowDeleteDatabaseButton = canConnect && !hasPendingMigrations;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDatabase()
        {
            try
            {
                // Ensure database/tables are created via SQL script
                await _db.EnsureCreatedAsync();
                TempData["DbMessage"] = "Banco de dados criado/atualizado com sucesso via script.";
            }
            catch (Exception ex)
            {
                TempData["DbError"] = "Erro ao criar/atualizar o banco de dados: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDatabase()
        {
            try
            {
                await _db.DropDatabaseAsync();
                TempData["DbMessage"] = "Banco de dados excluído com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["DbError"] = "Erro ao excluir o banco de dados: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Privacy view removed; no action needed.

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
