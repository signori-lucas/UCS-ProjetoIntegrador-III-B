using Microsoft.AspNetCore.Mvc;
using UCS_ProjetoIntegrador_III_B.Entities;
using UCS_ProjetoIntegrador_III_B.Services;
using UCS_ProjetoIntegrador_III_B.Services.Interfaces;

namespace UCS_ProjetoIntegrador_III_B.Controllers
{
    public class OrientadoresController : Controller
    {
        private readonly IOrientadorService _service;
        public OrientadoresController(IOrientadorService service)
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
                return View(new List<Orientador>());
            }
            catch (Exception)
            {
                TempData["DbError"] = "Ocorreu um erro ao carregar a lista de orientadores.";
                return View(new List<Orientador>());
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
                TempData["DbError"] = "Ocorreu um erro ao obter detalhes do orientador.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Orientador orientador)
        {
            if (!ModelState.IsValid) return View(orientador);
            try
            {
                await _service.CreateAsync(orientador);
                TempData["DbMessage"] = "Orientador criado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Data.DatabaseException dbEx)
            {
                ModelState.AddModelError(string.Empty, dbEx.Message);
                return View(orientador);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Erro ao criar o orientador.");
                return View(orientador);
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
                TempData["DbError"] = "Ocorreu um erro ao carregar o orientador para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Orientador orientador)
        {
            if (id != orientador.Id) return BadRequest();
            if (!ModelState.IsValid) return View(orientador);
            try
            {
                await _service.UpdateAsync(orientador);
                TempData["DbMessage"] = "Orientador atualizado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Data.DatabaseException dbEx)
            {
                ModelState.AddModelError(string.Empty, dbEx.Message);
                return View(orientador);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Erro ao atualizar o orientador.");
                return View(orientador);
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
                TempData["DbError"] = "Ocorreu um erro ao carregar o orientador para exclusão.";
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
                TempData["DbMessage"] = "Orientador excluído com sucesso.";
            }
            catch (Data.DatabaseException dbEx)
            {
                TempData["DbError"] = dbEx.Message;
            }
            catch (Exception)
            {
                TempData["DbError"] = "Erro ao excluir o orientador.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
