using Microsoft.Data.SqlClient;
using UCS_ProjetoIntegrador_III_B.Data;
using UCS_ProjetoIntegrador_III_B.Entities;
using UCS_ProjetoIntegrador_III_B.Repositories.Interfaces;

namespace UCS_ProjetoIntegrador_III_B.Repositories
{
    public class OrientadorRepository : IOrientadorRepository
    {
        private readonly DatabaseService _db;
        public OrientadorRepository(DatabaseService db) { _db = db; }

        public async Task<List<Orientador>> GetAllAsync()
        {
            var sql = "SELECT Id, Nome, Email FROM Orientadores";
            var list = await _db.QueryAsync(sql, reader => new Orientador
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Email = reader.IsDBNull(2) ? null : reader.GetString(2)
            });

            foreach (var o in list)
            {
                var estSql = "SELECT Id, Titulo, Descricao, EmpresaId, OrientadorId FROM Estagios WHERE OrientadorId = @id";
                var ests = await _db.QueryAsync(estSql, r => new Estagio
                {
                    Id = r.GetInt32(0),
                    Titulo = r.GetString(1),
                    Descricao = r.IsDBNull(2) ? null : r.GetString(2),
                    EmpresaId = r.GetInt32(3),
                    OrientadorId = r.GetInt32(4)
                }, new SqlParameter("@id", o.Id));

                o.Estagios = ests;
            }

            return list;
        }

        public async Task<Orientador?> GetByIdAsync(int id)
        {
            var sql = "SELECT Id, Nome, Email FROM Orientadores WHERE Id = @id";
            var list = await _db.QueryAsync(sql, r => new Orientador
            {
                Id = r.GetInt32(0),
                Nome = r.GetString(1),
                Email = r.IsDBNull(2) ? null : r.GetString(2)
            }, new SqlParameter("@id", id));

            var o = list.FirstOrDefault();
            if (o != null)
            {
                var estSql = "SELECT Id, Titulo, Descricao, EmpresaId, OrientadorId FROM Estagios WHERE OrientadorId = @id";
                var ests = await _db.QueryAsync(estSql, r => new Estagio
                {
                    Id = r.GetInt32(0),
                    Titulo = r.GetString(1),
                    Descricao = r.IsDBNull(2) ? null : r.GetString(2),
                    EmpresaId = r.GetInt32(3),
                    OrientadorId = r.GetInt32(4)
                }, new SqlParameter("@id", o.Id));

                o.Estagios = ests;
            }

            return o;
        }

        public async Task CreateAsync(Orientador orientador)
        {
            var sql = "INSERT INTO Orientadores (Nome, Email) VALUES (@nome, @email); SELECT SCOPE_IDENTITY();";
            var idObj = await _db.ExecuteScalarAsync(sql,
                new SqlParameter("@nome", orientador.Nome),
                new SqlParameter("@email", (object?)orientador.Email ?? DBNull.Value));

            if (idObj != null && int.TryParse(idObj.ToString(), out var id)) orientador.Id = id;
        }

        public async Task UpdateAsync(Orientador orientador)
        {
            var sql = "UPDATE Orientadores SET Nome = @nome, Email = @email WHERE Id = @id";
            await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@nome", orientador.Nome),
                new SqlParameter("@email", (object?)orientador.Email ?? DBNull.Value),
                new SqlParameter("@id", orientador.Id));
        }

        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM Orientadores WHERE Id = @id";
            await _db.ExecuteNonQueryAsync(sql, new SqlParameter("@id", id));
        }
    }
}