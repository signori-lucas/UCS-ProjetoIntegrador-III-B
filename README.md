# UCS — Projeto Integrador III (B)

Resumo técnico
-------------
Aplicação web ASP.NET Core (Razor Pages / controllers + views) implementando CRUD para Gestão de Estágios por Empresas.
Arquitetura simples em camadas:
- `Controllers` (endpoints)
- `Services` (lógica de negócio)
- `Repositories` (acesso a banco de dados)
- `Entities/` (entidades)

Publish e Execução do Projeto
----------
PUBLISH
- Executar via cmd, power shwell ou algum terminal 
   - `dotnet publish -c Release -r win-x64 --self-contained false -o ./publish`

EXECUTAR O PROJETO
- Acessar a pasta publish recénm publicada

- Executar via cmd, power shwell ou algum terminal 
   - Setar a variável de ambiente
      - `$env:ASPNETCORE_ENVIRONMENT = 'Production'`
   - Executar o comando para rodar a aplicação
      - `dotnet .\UCS-ProjetoIntegrador-III-B.dll`
   - Acessar via browser o endereço retornado no terminal
      - Ex: "Now listening on: http://localhost:5000"
      - `http://localhost:5000`

Requisitos
----------
- .NET SDK 10 (instalar do site da Microsoft)
- SQL Server (instância acessível) — pode ser SQL Server LocalDB, Developer ou remota

Configuração
------------
1. Atualize a connection string em `appsettings.json` (ou na variável de ambiente) usando a chave `ConnectionStrings:DefaultConnection`.
   Exemplo:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=UCS_Projeto;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
}
```

2. Criação do esquema / execução do script

A aplicação possui um serviço `DatabaseService` que carrega e executa automaticamente o script `SqlScripts/create_tables.sql` (batches separados por `GO`).

Portanto, não é necessário executar o script manualmente — desde que a aplicação tenha permissões suficientes para criar/alterar o banco de dados a partir da connection string configurada.


Execução
--------
Abra terminal na pasta do projeto (`UCS-ProjetoIntegrador-III-B`) e execute:

```
dotnet restore
dotnet build
dotnet run
```

A aplicação inicia e fica disponível em `https://localhost:5001` (ou porta mostrada no console).

Banco de dados via código
-------------------------
Há um serviço `DatabaseService` que carrega `SqlScripts/create_tables.sql` e executa os batches (separados por `GO`). Se preferir, pode invocar `EnsureCreatedAsync()` programaticamente para criar o esquema baseado nesse script. Tenha cuidado em ambientes com dados existentes.

Validações importantes
----------------------
- CPF: existe validação no `AlunoService` que impede inserir um aluno com CPF já cadastrado; o script SQL também cria `UQ_Alunos_CPF` (constraint única). Em caso de conflito, a mensagem de validação é exibida no formulário.

Estrutura de pastas relevante
----------------------------
- `Controllers/` — controllers MVC
- `Views/` — views Razor
- `Services/` — regras de negócio
- `Repositories/` — queries e mapeamentos para as entidades
- `Entities/` — classes que representam as tabelas
- `SqlScripts/create_tables.sql` — script de criação do schema
