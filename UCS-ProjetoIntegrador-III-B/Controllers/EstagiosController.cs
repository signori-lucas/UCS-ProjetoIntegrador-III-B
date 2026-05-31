using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UCS_ProjetoIntegrador_III_B.Entities;
using UCS_ProjetoIntegrador_III_B.Services;
using UCS_ProjetoIntegrador_III_B.Services.Interfaces;

namespace UCS_ProjetoIntegrador_III_B.Controllers
{
    public class EstagiosController : Controller
    {
        private readonly IEstagioService _service;
        private readonly IEmpresaService _empresaService;
        private readonly IOrientadorService _orientadorService;
        private readonly IAlunoService _alunoService;

        public EstagiosController(IEstagioService service, IEmpresaService empresaService, IOrientadorService orientadorService, IAlunoService alunoService)
        {
            _service = service;
            _empresaService = empresaService;
            _orientadorService = orientadorService;
            _alunoService = alunoService;
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
                return View(new List<Estagio>());
            }
            catch (Exception)
            {
                TempData["DbError"] = "Ocorreu um erro ao carregar a lista de estágios.";
                return View(new List<Estagio>());
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
                TempData["DbError"] = "Ocorreu um erro ao obter detalhes do estágio.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Empresas = new SelectList(await _empresaService.GetAllAsync(), "Id", "Nome");
                ViewBag.Orientadores = new SelectList(await _orientadorService.GetAllAsync(), "Id", "Nome");
                ViewBag.Alunos = await _alunoService.GetAllAsync();
                return View();
            }
            catch (Data.DatabaseException dbEx)
            {
                TempData["DbError"] = dbEx.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["DbError"] = "Ocorreu um erro ao carregar dados para criar o estágio.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Estagio estagio, int[]? selectedAlunos)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Empresas = new SelectList(await _empresaService.GetAllAsync(), "Id", "Nome");
                ViewBag.Orientadores = new SelectList(await _orientadorService.GetAllAsync(), "Id", "Nome");
                ViewBag.Alunos = await _alunoService.GetAllAsync();
                return View(estagio);
            }

            if (selectedAlunos != null)
            {
                foreach (var aId in selectedAlunos)
                {
                    var aluno = await _alunoService.GetByIdAsync(aId);
                    if (aluno != null) estagio.Alunos.Add(aluno);
                }
            }

            await _service.CreateAsync(estagio);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ent = await _service.GetByIdAsync(id);
            if (ent == null) return NotFound();

            ViewBag.Empresas = new SelectList(await _empresaService.GetAllAsync(), "Id", "Nome", ent.EmpresaId);
            ViewBag.Orientadores = new SelectList(await _orientadorService.GetAllAsync(), "Id", "Nome", ent.OrientadorId);
            ViewBag.Alunos = await _alunoService.GetAllAsync();

            return View(ent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Estagio estagio, int[]? selectedAlunos)
        {
            if (id != estagio.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Empresas = new SelectList(await _empresaService.GetAllAsync(), "Id", "Nome", estagio.EmpresaId);
                ViewBag.Orientadores = new SelectList(await _orientadorService.GetAllAsync(), "Id", "Nome", estagio.OrientadorId);
                ViewBag.Alunos = await _alunoService.GetAllAsync();
                return View(estagio);
            }

            // update alunos association
            estagio.Alunos.Clear();
            if (selectedAlunos != null)
            {
                foreach (var aId in selectedAlunos)
                {
                    var aluno = await _alunoService.GetByIdAsync(aId);
                    if (aluno != null) estagio.Alunos.Add(aluno);
                }
            }

            await _service.UpdateAsync(estagio);
            return RedirectToAction(nameof(Index));
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
                TempData["DbError"] = "Ocorreu um erro ao carregar o estágio para exclusão.";
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
                TempData["DbMessage"] = "Estágio excluído com sucesso.";
            }
            catch (Data.DatabaseException dbEx)
            {
                TempData["DbError"] = dbEx.Message;
            }
            catch (Exception)
            {
                TempData["DbError"] = "Erro ao excluir o estágio.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
