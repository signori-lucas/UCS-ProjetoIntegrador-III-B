using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace UCS_ProjetoIntegrador_III_B.Data
{
    public class DatabaseService
    {
        private readonly string _connectionString;
        private readonly IHostEnvironment _env;

        public DatabaseService(IConfiguration configuration, IHostEnvironment env)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _env = env;
        }

        public async Task DropDatabaseAsync()
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(_connectionString);
                var dbName = builder.InitialCatalog;
                if (string.IsNullOrWhiteSpace(dbName))
                {
                    throw new InvalidOperationException("Connection string does not contain an Initial Catalog/Database to drop.");
                }

                var masterBuilder = new SqlConnectionStringBuilder(_connectionString)
                {
                    InitialCatalog = "master"
                };

                using var conn = new SqlConnection(masterBuilder.ToString());
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@"
IF EXISTS(SELECT name FROM sys.databases WHERE name = '{dbName}')
BEGIN
    ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [{dbName}];
END
";
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Erro ao excluir o banco de dados. Verifique se o servidor está disponível e se a aplicação tem permissão para excluir o banco.", ex);
            }
        }

        public async Task<bool> CanConnectAsync()
        {
            try
            {
                var connected = false;
                var attempts = 0;
                var maxAttempts = 10;
                var delayMs = 500;
                while (!connected && attempts < maxAttempts)
                {
                    try
                    {
                        using var testConn = new SqlConnection(_connectionString);
                        await testConn.OpenAsync();
                        connected = true;
                    }
                    catch
                    {
                        attempts++;
                        await Task.Delay(delayMs);
                    }
                }
                if (!connected)
                {
                    throw new DatabaseException("Erro ao conectar ao banco recém-criado após múltiplas tentativas.");
                }

                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Não foi possível conectar ao banco de dados. Verifique a connection string e se o servidor está acessível.", ex);
            }
        }

        public async Task<bool> HasTablesAsync()
        {
            var sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Empresas'";
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(sql, conn);
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Erro ao verificar existência das tabelas no banco de dados.", ex);
            }
        }

        public async Task EnsureCreatedAsync()
        {
            var tried = new List<string>();
            var scriptPath = Path.Combine(_env.ContentRootPath, "SqlScripts", "create_tables.sql");
            tried.Add(scriptPath);
            if (!File.Exists(scriptPath))
            {
                var alt1 = Path.Combine(AppContext.BaseDirectory, "SqlScripts", "create_tables.sql");
                tried.Add(alt1);
                if (File.Exists(alt1)) scriptPath = alt1;
                else
                {
                    var alt2 = Path.Combine(Directory.GetCurrentDirectory(), "SqlScripts", "create_tables.sql");
                    tried.Add(alt2);
                    if (File.Exists(alt2)) scriptPath = alt2;
                }
            }
            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"SQL script for creating tables not found. Paths tried: {string.Join(';', tried)}", scriptPath);
            }

            var script = await File.ReadAllTextAsync(scriptPath);

            var batches = SplitSqlBatches(script);

            try
            {
                var builder = new SqlConnectionStringBuilder(_connectionString);
                var dbName = builder.InitialCatalog;
                if (string.IsNullOrWhiteSpace(dbName))
                    throw new InvalidOperationException("Connection string does not contain an Initial Catalog/Database to create.");

                var masterBuilder = new SqlConnectionStringBuilder(_connectionString)
                {
                    InitialCatalog = "master"
                };

                using (var masterConn = new SqlConnection(masterBuilder.ToString()))
                {
                    await masterConn.OpenAsync();
                    using var checkCmd = masterConn.CreateCommand();
                    checkCmd.CommandType = CommandType.Text;
                    checkCmd.CommandText = "SELECT COUNT(*) FROM sys.databases WHERE name = @name";
                    checkCmd.Parameters.AddWithValue("@name", dbName);
                    var exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync()) > 0;
                    if (!exists)
                    {
                        using var createCmd = masterConn.CreateCommand();
                        createCmd.CommandType = CommandType.Text;
                        createCmd.CommandText = $"CREATE DATABASE [{dbName}];";
                        await createCmd.ExecuteNonQueryAsync();
                    }
                }

                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                using var tran = conn.BeginTransaction();
                try
                {
                    foreach (var batch in batches)
                    {
                        if (string.IsNullOrWhiteSpace(batch)) continue;
                        using var cmd = new SqlCommand(batch, conn, tran);
                        cmd.CommandType = CommandType.Text;
                        await cmd.ExecuteNonQueryAsync();
                    }
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw new DatabaseException("Erro ao criar as tabelas no banco de dados. Verifique o script SQL e as permissões de acesso.", ex);
                }
            }
            catch (DatabaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex);
            }
        }

        private static IEnumerable<string> SplitSqlBatches(string sql)
        {
            var lines = sql.Replace("\r\n", "\n").Split('\n');
            var batch = new List<string>();
            foreach (var line in lines)
            {
                if (line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))
                {
                    yield return string.Join('\n', batch);
                    batch.Clear();
                }
                else
                {
                    batch.Add(line);
                }
            }
            if (batch.Count > 0) yield return string.Join('\n', batch);
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(sql, conn);
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Erro ao executar comando no banco de dados.", ex);
            }
        }

        public async Task<object?> ExecuteScalarAsync(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(sql, conn);
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                return await cmd.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Erro ao executar consulta no banco de dados.", ex);
            }
        }

        public async Task<List<T>> QueryAsync<T>(string sql, Func<SqlDataReader, T> map, params SqlParameter[] parameters)
        {
            var list = new List<T>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(sql, conn);
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(map(reader));
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Erro ao realizar consulta. Verifique a criação do Banco de Dados", ex);
            }
        }
    }
}
