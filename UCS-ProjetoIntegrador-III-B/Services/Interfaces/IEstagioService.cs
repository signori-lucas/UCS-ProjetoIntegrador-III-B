using UCS_ProjetoIntegrador_III_B.Entities;

namespace UCS_ProjetoIntegrador_III_B.Services.Interfaces
{
    public interface IEstagioService
    {
        Task<List<Estagio>> GetAllAsync();
        Task<Estagio?> GetByIdAsync(int id);
        Task CreateAsync(Estagio estagio);
        Task UpdateAsync(Estagio estagio);
        Task DeleteAsync(int id);
    }
}