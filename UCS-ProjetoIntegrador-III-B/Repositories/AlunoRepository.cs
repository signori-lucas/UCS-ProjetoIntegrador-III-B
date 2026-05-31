using Microsoft.Data.SqlClient;
using UCS_ProjetoIntegrador_III_B.Data;
using UCS_ProjetoIntegrador_III_B.Entities;
using UCS_ProjetoIntegrador_III_B.Repositories.Interfaces;

namespace UCS_ProjetoIntegrador_III_B.Repositories
{
    public class AlunoRepository : IAlunoRepository
    {
        private readonly DatabaseService _db;
        public AlunoRepository(DatabaseService db) { _db = db; }

        public async Task<List<Aluno>> GetAllAsync()
        {
            var sql = "SELECT Id, Nome, Matricula, CPF, DataNascimento, Endereco, Sexo FROM Alunos";
            var alunos = await _db.QueryAsync(sql, reader => new Aluno
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Matricula = reader.IsDBNull(2) ? null : reader.GetString(2),
                CPF = reader.IsDBNull(3) ? null! : reader.GetString(3),
                DataNascimento = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                Endereco = reader.IsDBNull(5) ? null : reader.GetString(5),
                Sexo = reader.IsDBNull(6) ? null : ParseSexo(reader.GetString(6))
            });

            foreach (var aluno in alunos)
            {
                var estSql = "SELECT e.Id, e.Titulo, e.Descricao, e.EmpresaId, e.OrientadorId FROM Estagios e JOIN EstagioAluno ea ON e.Id = ea.EstagioId WHERE ea.AlunoId = @id";
                var ests = await _db.QueryAsync(estSql, r => new Estagio
                {
                    Id = r.GetInt32(0),
                    Titulo = r.GetString(1),
                    Descricao = r.IsDBNull(2) ? null : r.GetString(2),
                    EmpresaId = r.GetInt32(3),
                    OrientadorId = r.GetInt32(4)
                }, new SqlParameter("@id", aluno.Id));

                aluno.Estagios = ests;
            }

            return alunos;
        }

        public async Task<Aluno?> GetByIdAsync(int id)
        {
            var sql = "SELECT Id, Nome, Matricula, CPF, DataNascimento, Endereco, Sexo FROM Alunos WHERE Id = @id";
            var list = await _db.QueryAsync(sql, reader => new Aluno
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Matricula = reader.IsDBNull(2) ? null : reader.GetString(2),
                CPF = reader.IsDBNull(3) ? null! : reader.GetString(3),
                DataNascimento = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                Endereco = reader.IsDBNull(5) ? null : reader.GetString(5),
                Sexo = reader.IsDBNull(6) ? null : ParseSexo(reader.GetString(6))
            }, new SqlParameter("@id", id));

            var aluno = list.FirstOrDefault();
            if (aluno != null)
            {
                var estSql = "SELECT e.Id, e.Titulo, e.Descricao, e.EmpresaId, e.OrientadorId FROM Estagios e JOIN EstagioAluno ea ON e.Id = ea.EstagioId WHERE ea.AlunoId = @id";
                var ests = await _db.QueryAsync(estSql, r => new Estagio
                {
                    Id = r.GetInt32(0),
                    Titulo = r.GetString(1),
                    Descricao = r.IsDBNull(2) ? null : r.GetString(2),
                    EmpresaId = r.GetInt32(3),
                    OrientadorId = r.GetInt32(4)
                }, new SqlParameter("@id", aluno.Id));

                aluno.Estagios = ests;
            }

            return aluno;
        }

        public async Task CreateAsync(Aluno aluno)
        {
            var sql = "INSERT INTO Alunos (Nome, Matricula, CPF, DataNascimento, Endereco, Sexo) VALUES (@nome, @matricula, @cpf, @dataNascimento, @endereco, @sexo); SELECT SCOPE_IDENTITY();";
            var idObj = await _db.ExecuteScalarAsync(sql,
                new SqlParameter("@nome", aluno.Nome),
                new SqlParameter("@matricula", (object?)aluno.Matricula ?? DBNull.Value),
                new SqlParameter("@cpf", (object?)aluno.CPF ?? DBNull.Value),
                new SqlParameter("@dataNascimento", (object?)aluno.DataNascimento ?? DBNull.Value),
                new SqlParameter("@endereco", (object?)aluno.Endereco ?? DBNull.Value),
                new SqlParameter("@sexo", (object?)(aluno.Sexo.HasValue ? aluno.Sexo.Value.ToString() : null) ?? DBNull.Value));

            if (idObj != null && int.TryParse(idObj.ToString(), out var id)) aluno.Id = id;
        }

        public async Task UpdateAsync(Aluno aluno)
        {
            var sql = "UPDATE Alunos SET Nome = @nome, Matricula = @matricula, CPF = @cpf, DataNascimento = @dataNascimento, Endereco = @endereco, Sexo = @sexo WHERE Id = @id";
            await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@nome", aluno.Nome),
                new SqlParameter("@matricula", (object?)aluno.Matricula ?? DBNull.Value),
                new SqlParameter("@cpf", (object?)aluno.CPF ?? DBNull.Value),
                new SqlParameter("@dataNascimento", (object?)aluno.DataNascimento ?? DBNull.Value),
                new SqlParameter("@endereco", (object?)aluno.Endereco ?? DBNull.Value),
                new SqlParameter("@sexo", (object?)(aluno.Sexo.HasValue ? aluno.Sexo.Value.ToString() : null) ?? DBNull.Value),
                new SqlParameter("@id", aluno.Id));
        }

        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM Alunos WHERE Id = @id";
            await _db.ExecuteNonQueryAsync(sql, new SqlParameter("@id", id));
        }

        public async Task<bool> ExistsByCpfAsync(string cpf)
        {
            var sql = "SELECT COUNT(*) FROM Alunos WHERE CPF = @cpf";
            var result = await _db.ExecuteScalarAsync(sql, new SqlParameter("@cpf", cpf));
            return Convert.ToInt32(result) > 0;
        }

        private static Sexo? ParseSexo(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (Enum.TryParse<Sexo>(s, true, out var sex)) return sex;
            return null;
        }
    }
}