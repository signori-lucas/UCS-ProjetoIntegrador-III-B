using Microsoft.Data.SqlClient;
using UCS_ProjetoIntegrador_III_B.Data;
using UCS_ProjetoIntegrador_III_B.Entities;
using UCS_ProjetoIntegrador_III_B.Repositories.Interfaces;
using UCS_ProjetoIntegrador_III_B.Services.Interfaces;

namespace UCS_ProjetoIntegrador_III_B.Services
{
    public class AlunoService : IAlunoService
    {
        private readonly IAlunoRepository _repo;
        public AlunoService(IAlunoRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Aluno>> GetAllAsync()
        {
            var alunos = await _repo.GetAllAsync();
            return alunos;
        }

        public async Task<Aluno?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task CreateAsync(Aluno aluno)
        {
            // Validação: CPF único
            if (!string.IsNullOrWhiteSpace(aluno.CPF))
            {
                var exists = await _repo.ExistsByCpfAsync(aluno.CPF);
                if (exists)
                {
                    throw new InvalidOperationException("Já existe um aluno cadastrado com o mesmo CPF.");
                }
            }

            await _repo.CreateAsync(aluno);
        }

        public async Task UpdateAsync(Aluno aluno)
        {
            // Se CPF informado, garantir unicidade entre outros registros
            if (!string.IsNullOrWhiteSpace(aluno.CPF))
            {
                // obter registro existente para comparar CPF
                var existing = await _repo.GetByIdAsync(aluno.Id);
                if (existing == null)
                {
                    throw new InvalidOperationException("Aluno não encontrado para atualização.");
                }

                // se o CPF mudou, verificar existência em outros registros
                if (!string.Equals(existing.CPF, aluno.CPF, StringComparison.OrdinalIgnoreCase))
                {
                    var exists = await _repo.ExistsByCpfAsync(aluno.CPF);
                    if (exists)
                    {
                        throw new InvalidOperationException("Já existe outro aluno cadastrado com o mesmo CPF.");
                    }
                }
            }

            await _repo.UpdateAsync(aluno);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
