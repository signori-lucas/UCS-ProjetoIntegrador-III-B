using UCS_ProjetoIntegrador_III_B.Entities;

namespace UCS_ProjetoIntegrador_III_B.Repositories.Interfaces
{
    public interface IOrientadorRepository
    {
        Task<List<Orientador>> GetAllAsync();
        Task<Orientador?> GetByIdAsync(int id);
        Task CreateAsync(Orientador orientador);
        Task UpdateAsync(Orientador orientador);
        Task DeleteAsync(int id);
    }
}