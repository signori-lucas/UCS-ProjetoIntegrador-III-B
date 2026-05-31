using Microsoft.Data.SqlClient;
using UCS_ProjetoIntegrador_III_B.Data;
using UCS_ProjetoIntegrador_III_B.Entities;
using UCS_ProjetoIntegrador_III_B.Repositories.Interfaces;
using UCS_ProjetoIntegrador_III_B.Services.Interfaces;

namespace UCS_ProjetoIntegrador_III_B.Services
{
    public class EmpresaService : IEmpresaService
    {
        private readonly IEmpresaRepository _repo;
        public EmpresaService(IEmpresaRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Empresa>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Empresa?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task CreateAsync(Empresa empresa)
        {
            await _repo.CreateAsync(empresa);
        }

        public async Task UpdateAsync(Empresa empresa)
        {
            await _repo.UpdateAsync(empresa);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
