using Microsoft.Data.SqlClient;
using UCS_ProjetoIntegrador_III_B.Data;
using UCS_ProjetoIntegrador_III_B.Entities;
using UCS_ProjetoIntegrador_III_B.Repositories.Interfaces;
using UCS_ProjetoIntegrador_III_B.Services.Interfaces;

namespace UCS_ProjetoIntegrador_III_B.Services
{
    public class OrientadorService : IOrientadorService
    {
        private readonly IOrientadorRepository _repo;
        public OrientadorService(IOrientadorRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Orientador>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Orientador?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task CreateAsync(Orientador orientador)
        {
            await _repo.CreateAsync(orientador);
        }

        public async Task UpdateAsync(Orientador orientador)
        {
            await _repo.UpdateAsync(orientador);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
