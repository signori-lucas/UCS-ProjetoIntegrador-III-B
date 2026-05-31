using UCS_ProjetoIntegrador_III_B.Entities;

namespace UCS_ProjetoIntegrador_III_B.Repositories.Interfaces
{
    public interface IEstagioRepository
    {
        Task<List<Estagio>> GetAllAsync();
        Task<Estagio?> GetByIdAsync(int id);
        Task CreateAsync(Estagio estagio);
        Task UpdateAsync(Estagio estagio);
        Task DeleteAsync(int id);
    }
}