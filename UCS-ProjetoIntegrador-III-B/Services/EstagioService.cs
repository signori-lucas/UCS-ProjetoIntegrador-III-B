using Microsoft.Data.SqlClient;
using UCS_ProjetoIntegrador_III_B.Data;
using UCS_ProjetoIntegrador_III_B.Entities;
using UCS_ProjetoIntegrador_III_B.Repositories.Interfaces;
using UCS_ProjetoIntegrador_III_B.Services.Interfaces;

namespace UCS_ProjetoIntegrador_III_B.Services
{
    public class EstagioService : IEstagioService
    {
        private readonly IEstagioRepository _repo;
        public EstagioService(IEstagioRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Estagio>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Estagio?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task CreateAsync(Estagio estagio)
        {
            await _repo.CreateAsync(estagio);
        }

        public async Task UpdateAsync(Estagio estagio)
        {
            await _repo.UpdateAsync(estagio);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
