using Microsoft.Data.SqlClient;
using UCS_ProjetoIntegrador_III_B.Data;
using UCS_ProjetoIntegrador_III_B.Entities;
using UCS_ProjetoIntegrador_III_B.Repositories.Interfaces;

namespace UCS_ProjetoIntegrador_III_B.Repositories
{
    public class EstagioRepository : IEstagioRepository
    {
        private readonly DatabaseService _db;
        public EstagioRepository(DatabaseService db) { _db = db; }

        public async Task<List<Estagio>> GetAllAsync()
        {
            var sql = "SELECT Id, Titulo, Descricao, EmpresaId, OrientadorId FROM Estagios";
            var estagios = await _db.QueryAsync(sql, r => new Estagio
            {
                Id = r.GetInt32(0),
                Titulo = r.GetString(1),
                Descricao = r.IsDBNull(2) ? null : r.GetString(2),
                EmpresaId = r.GetInt32(3),
                OrientadorId = r.GetInt32(4)
            });

            foreach (var e in estagios)
            {
                var emp = (await _db.QueryAsync("SELECT Id, Nome, Endereco, Telefone FROM Empresas WHERE Id = @id", r => new Empresa
                {
                    Id = r.GetInt32(0),
                    Nome = r.GetString(1),
                    Endereco = r.IsDBNull(2) ? null : r.GetString(2),
                    Telefone = r.IsDBNull(3) ? null : r.GetString(3)
                }, new SqlParameter("@id", e.EmpresaId))).FirstOrDefault();
                e.Empresa = emp;

                var ori = (await _db.QueryAsync("SELECT Id, Nome, Email FROM Orientadores WHERE Id = @id", r => new Orientador
                {
                    Id = r.GetInt32(0),
                    Nome = r.GetString(1),
                    Email = r.IsDBNull(2) ? null : r.GetString(2)
                }, new SqlParameter("@id", e.OrientadorId))).FirstOrDefault();
                e.Orientador = ori;

                var alunos = await _db.QueryAsync("SELECT a.Id, a.Nome, a.Matricula FROM Alunos a JOIN EstagioAluno ea ON a.Id = ea.AlunoId WHERE ea.EstagioId = @id", r => new Aluno
                {
                    Id = r.GetInt32(0),
                    Nome = r.GetString(1),
                    Matricula = r.IsDBNull(2) ? null : r.GetString(2)
                }, new SqlParameter("@id", e.Id));

                e.Alunos = alunos;
            }

            return estagios;
        }

        public async Task<Estagio?> GetByIdAsync(int id)
        {
            var sql = "SELECT Id, Titulo, Descricao, EmpresaId, OrientadorId FROM Estagios WHERE Id = @id";
            var list = await _db.QueryAsync(sql, r => new Estagio
            {
                Id = r.GetInt32(0),
                Titulo = r.GetString(1),
                Descricao = r.IsDBNull(2) ? null : r.GetString(2),
                EmpresaId = r.GetInt32(3),
                OrientadorId = r.GetInt32(4)
            }, new SqlParameter("@id", id));

            var est = list.FirstOrDefault();
            if (est == null) return null;

            est.Empresa = (await _db.QueryAsync("SELECT Id, Nome, Endereco, Telefone FROM Empresas WHERE Id = @id", r => new Empresa
            {
                Id = r.GetInt32(0),
                Nome = r.GetString(1),
                Endereco = r.IsDBNull(2) ? null : r.GetString(2),
                Telefone = r.IsDBNull(3) ? null : r.GetString(3)
            }, new SqlParameter("@id", est.EmpresaId))).FirstOrDefault();

            est.Orientador = (await _db.QueryAsync("SELECT Id, Nome, Email FROM Orientadores WHERE Id = @id", r => new Orientador
            {
                Id = r.GetInt32(0),
                Nome = r.GetString(1),
                Email = r.IsDBNull(2) ? null : r.GetString(2)
            }, new SqlParameter("@id", est.OrientadorId))).FirstOrDefault();

            est.Alunos = await _db.QueryAsync("SELECT a.Id, a.Nome, a.Matricula FROM Alunos a JOIN EstagioAluno ea ON a.Id = ea.AlunoId WHERE ea.EstagioId = @id", r => new Aluno
            {
                Id = r.GetInt32(0),
                Nome = r.GetString(1),
                Matricula = r.IsDBNull(2) ? null : r.GetString(2)
            }, new SqlParameter("@id", est.Id));

            return est;
        }

        public async Task CreateAsync(Estagio estagio)
        {
            var sql = "INSERT INTO Estagios (Titulo, Descricao, EmpresaId, OrientadorId) VALUES (@titulo, @descricao, @empresaId, @orientadorId); SELECT SCOPE_IDENTITY();";
            var idObj = await _db.ExecuteScalarAsync(sql,
                new SqlParameter("@titulo", estagio.Titulo),
                new SqlParameter("@descricao", (object?)estagio.Descricao ?? DBNull.Value),
                new SqlParameter("@empresaId", estagio.EmpresaId),
                new SqlParameter("@orientadorId", estagio.OrientadorId));

            if (idObj != null && int.TryParse(idObj.ToString(), out var id)) estagio.Id = id;

            if (estagio.Alunos != null && estagio.Alunos.Any())
            {
                foreach (var a in estagio.Alunos)
                {
                    await _db.ExecuteNonQueryAsync("INSERT INTO EstagioAluno (EstagioId, AlunoId) VALUES (@eId, @aId)",
                        new SqlParameter("@eId", estagio.Id),
                        new SqlParameter("@aId", a.Id));
                }
            }
        }

        public async Task UpdateAsync(Estagio estagio)
        {
            var sql = "UPDATE Estagios SET Titulo = @titulo, Descricao = @descricao, EmpresaId = @empresaId, OrientadorId = @orientadorId WHERE Id = @id";
            await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@titulo", estagio.Titulo),
                new SqlParameter("@descricao", (object?)estagio.Descricao ?? DBNull.Value),
                new SqlParameter("@empresaId", estagio.EmpresaId),
                new SqlParameter("@orientadorId", estagio.OrientadorId),
                new SqlParameter("@id", estagio.Id));

            await _db.ExecuteNonQueryAsync("DELETE FROM EstagioAluno WHERE EstagioId = @id", new SqlParameter("@id", estagio.Id));
            if (estagio.Alunos != null && estagio.Alunos.Any())
            {
                foreach (var a in estagio.Alunos)
                {
                    await _db.ExecuteNonQueryAsync("INSERT INTO EstagioAluno (EstagioId, AlunoId) VALUES (@eId, @aId)",
                        new SqlParameter("@eId", estagio.Id),
                        new SqlParameter("@aId", a.Id));
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _db.ExecuteNonQueryAsync("DELETE FROM EstagioAluno WHERE EstagioId = @id", new SqlParameter("@id", id));
            await _db.ExecuteNonQueryAsync("DELETE FROM Estagios WHERE Id = @id", new SqlParameter("@id", id));
        }
    }
}