using UCS_ProjetoIntegrador_III_B.Entities;

namespace UCS_ProjetoIntegrador_III_B.Services.Interfaces
{
    public interface IAlunoService
    {
        Task<List<Aluno>> GetAllAsync();
        Task<Aluno?> GetByIdAsync(int id);
        Task CreateAsync(Aluno aluno);
        Task UpdateAsync(Aluno aluno);
        Task DeleteAsync(int id);
    }
}