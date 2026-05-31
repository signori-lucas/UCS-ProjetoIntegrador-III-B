using Microsoft.Data.SqlClient;
using UCS_ProjetoIntegrador_III_B.Data;
using UCS_ProjetoIntegrador_III_B.Entities;
using UCS_ProjetoIntegrador_III_B.Repositories.Interfaces;

namespace UCS_ProjetoIntegrador_III_B.Repositories
{
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly DatabaseService _db;
        public EmpresaRepository(DatabaseService db) { _db = db; }

        public async Task<List<Empresa>> GetAllAsync()
        {
            var sql = "SELECT Id, Nome, Endereco, Telefone FROM Empresas";
            var empresas = await _db.QueryAsync(sql, reader => new Empresa
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Endereco = reader.IsDBNull(2) ? null : reader.GetString(2),
                Telefone = reader.IsDBNull(3) ? null : reader.GetString(3)
            });

            foreach (var emp in empresas)
            {
                var estSql = "SELECT Id, Titulo, Descricao, EmpresaId, OrientadorId FROM Estagios WHERE EmpresaId = @id";
                var ests = await _db.QueryAsync(estSql, r => new Estagio
                {
                    Id = r.GetInt32(0),
                    Titulo = r.GetString(1),
                    Descricao = r.IsDBNull(2) ? null : r.GetString(2),
                    EmpresaId = r.GetInt32(3),
                    OrientadorId = r.GetInt32(4)
                }, new SqlParameter("@id", emp.Id));

                emp.Estagios = ests;
            }

            return empresas;
        }

        public async Task<Empresa?> GetByIdAsync(int id)
        {
            var sql = "SELECT Id, Nome, Endereco, Telefone FROM Empresas WHERE Id = @id";
            var list = await _db.QueryAsync(sql, reader => new Empresa
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Endereco = reader.IsDBNull(2) ? null : reader.GetString(2),
                Telefone = reader.IsDBNull(3) ? null : reader.GetString(3)
            }, new SqlParameter("@id", id));

            var emp = list.FirstOrDefault();
            if (emp != null)
            {
                var estSql = "SELECT Id, Titulo, Descricao, EmpresaId, OrientadorId FROM Estagios WHERE EmpresaId = @id";
                var ests = await _db.QueryAsync(estSql, r => new Estagio
                {
                    Id = r.GetInt32(0),
                    Titulo = r.GetString(1),
                    Descricao = r.IsDBNull(2) ? null : r.GetString(2),
                    EmpresaId = r.GetInt32(3),
                    OrientadorId = r.GetInt32(4)
                }, new SqlParameter("@id", emp.Id));

                emp.Estagios = ests;
            }

            return emp;
        }

        public async Task CreateAsync(Empresa empresa)
        {
            var sql = "INSERT INTO Empresas (Nome, Endereco, Telefone) VALUES (@nome, @endereco, @telefone); SELECT SCOPE_IDENTITY();";
            var idObj = await _db.ExecuteScalarAsync(sql,
                new SqlParameter("@nome", empresa.Nome),
                new SqlParameter("@endereco", (object?)empresa.Endereco ?? DBNull.Value),
                new SqlParameter("@telefone", (object?)empresa.Telefone ?? DBNull.Value));

            if (idObj != null && int.TryParse(idObj.ToString(), out var id)) empresa.Id = id;
        }

        public async Task UpdateAsync(Empresa empresa)
        {
            var sql = "UPDATE Empresas SET Nome = @nome, Endereco = @endereco, Telefone = @telefone WHERE Id = @id";
            await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@nome", empresa.Nome),
                new SqlParameter("@endereco", (object?)empresa.Endereco ?? DBNull.Value),
                new SqlParameter("@telefone", (object?)empresa.Telefone ?? DBNull.Value),
                new SqlParameter("@id", empresa.Id));
        }

        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM Empresas WHERE Id = @id";
            await _db.ExecuteNonQueryAsync(sql, new SqlParameter("@id", id));
        }
    }
}