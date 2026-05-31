using UCS_ProjetoIntegrador_III_B.Entities;

namespace UCS_ProjetoIntegrador_III_B.Repositories.Interfaces
{
    public interface IEmpresaRepository
    {
        Task<List<Empresa>> GetAllAsync();
        Task<Empresa?> GetByIdAsync(int id);
        Task CreateAsync(Empresa empresa);
        Task UpdateAsync(Empresa empresa);
        Task DeleteAsync(int id);
    }
}