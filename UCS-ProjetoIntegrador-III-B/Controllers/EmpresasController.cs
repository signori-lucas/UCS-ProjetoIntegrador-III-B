using Microsoft.AspNetCore.Mvc;
using UCS_ProjetoIntegrador_III_B.Entities;
using UCS_ProjetoIntegrador_III_B.Services;
using UCS_ProjetoIntegrador_III_B.Services.Interfaces;

namespace UCS_ProjetoIntegrador_III_B.Controllers
{
    public class EmpresasController : Controller
    {
        private readonly IEmpresaService _service;
        public EmpresasController(IEmpresaService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _service.GetAllAsync();
                return View(list);
            }
            catch (Data.DatabaseException dbEx)
            {
                TempData["DbError"] = dbEx.Message;
                return View(new List<Empresa>());
            }
            catch (Exception)
            {
                TempData["DbError"] = "Ocorreu um erro ao carregar a lista de empresas.";
                return View(new List<Empresa>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var ent = await _service.GetByIdAsync(id);
                if (ent == null) return NotFound();
                return View(ent);
            }
            catch (Data.DatabaseException dbEx)
            {
                TempData["DbError"] = dbEx.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["DbError"] = "Ocorreu um erro ao obter detalhes da empresa.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Empresa empresa)
        {
            if (!ModelState.IsValid) return View(empresa);
            try
            {
                await _service.CreateAsync(empresa);
                TempData["DbMessage"] = "Empresa criada com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Data.DatabaseException dbEx)
            {
                ModelState.AddModelError(string.Empty, dbEx.Message);
                return View(empresa);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Erro ao criar a empresa.");
                return View(empresa);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var ent = await _service.GetByIdAsync(id);
                if (ent == null) return NotFound();
                return View(ent);
            }
            catch (Data.DatabaseException dbEx)
            {
                TempData["DbError"] = dbEx.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["DbError"] = "Ocorreu um erro ao carregar a empresa para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Empresa empresa)
        {
            if (id != empresa.Id) return BadRequest();
            if (!ModelState.IsValid) return View(empresa);
            try
            {
                await _service.UpdateAsync(empresa);
                TempData["DbMessage"] = "Empresa atualizada com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Data.DatabaseException dbEx)
            {
                ModelState.AddModelError(string.Empty, dbEx.Message);
                return View(empresa);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Erro ao atualizar a empresa.");
                return View(empresa);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ent = await _service.GetByIdAsync(id);
                if (ent == null) return NotFound();
                return View(ent);
            }
            catch (Data.DatabaseException dbEx)
            {
                TempData["DbError"] = dbEx.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["DbError"] = "Ocorreu um erro ao carregar a empresa para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                TempData["DbMessage"] = "Empresa excluída com sucesso.";
            }
            catch (Data.DatabaseException dbEx)
            {
                TempData["DbError"] = dbEx.Message;
            }
            catch (Exception)
            {
                TempData["DbError"] = "Erro ao excluir a empresa.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
