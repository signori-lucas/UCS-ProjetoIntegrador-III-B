using Microsoft.AspNetCore.Mvc;
using UCS_ProjetoIntegrador_III_B.Entities;
using UCS_ProjetoIntegrador_III_B.Services;
using UCS_ProjetoIntegrador_III_B.Services.Interfaces;

namespace UCS_ProjetoIntegrador_III_B.Controllers
{
    public class AlunosController : Controller
    {
        private readonly IAlunoService _service;
        public AlunosController(IAlunoService service)
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
                return View(new List<Aluno>());
            }
            catch (Exception ex)
            {
                TempData["DbError"] = "Ocorreu um erro ao carregar a lista de alunos.";
                return View(new List<Aluno>());
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
                TempData["DbError"] = "Ocorreu um erro ao obter detalhes do aluno.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Aluno aluno)
        {
            if (!ModelState.IsValid) return View(aluno);
            try
            {
                await _service.CreateAsync(aluno);
                TempData["DbMessage"] = "Aluno criado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Data.DatabaseException dbEx)
            {
                ModelState.AddModelError(string.Empty, dbEx.Message);
                return View(aluno);
            }
            catch (InvalidOperationException invEx)
            {
                ModelState.AddModelError("CPF", invEx.Message);
                return View(aluno);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Erro ao criar o aluno.");
                return View(aluno);
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
                TempData["DbError"] = "Ocorreu um erro ao carregar o aluno para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Aluno aluno)
        {
            if (id != aluno.Id) return BadRequest();
            if (!ModelState.IsValid) return View(aluno);
            try
            {
                await _service.UpdateAsync(aluno);
                TempData["DbMessage"] = "Aluno atualizado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Data.DatabaseException dbEx)
            {
                ModelState.AddModelError(string.Empty, dbEx.Message);
                return View(aluno);
            }
            catch (InvalidOperationException invEx)
            {
                ModelState.AddModelError("CPF", invEx.Message);
                ModelState.AddModelError(string.Empty, invEx.Message);
                return View(aluno);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Erro ao atualizar o aluno.");
                return View(aluno);
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
                TempData["DbError"] = "Ocorreu um erro ao carregar o aluno para exclusão.";
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
                TempData["DbMessage"] = "Aluno excluído com sucesso.";
            }
            catch (Data.DatabaseException dbEx)
            {
                TempData["DbError"] = dbEx.Message;
            }
            catch (Exception)
            {
                TempData["DbError"] = "Erro ao excluir o aluno.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
