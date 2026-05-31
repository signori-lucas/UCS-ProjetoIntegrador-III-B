-- Schema for initial database

CREATE TABLE Empresas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(MAX) NOT NULL,
    Endereco NVARCHAR(MAX) NULL,
    Telefone NVARCHAR(MAX) NULL
);

GO

CREATE TABLE Alunos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(MAX) NOT NULL,
    Matricula NVARCHAR(MAX) NULL,
    CPF NVARCHAR(20) NOT NULL,
    DataNascimento DATE NULL,
    Endereco NVARCHAR(MAX) NULL,
    Sexo NVARCHAR(20) NULL,
    CONSTRAINT UQ_Alunos_CPF UNIQUE (CPF)
);

GO

CREATE TABLE Orientadores (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(MAX) NOT NULL,
    Email NVARCHAR(MAX) NULL
);

GO

CREATE TABLE Estagios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Titulo NVARCHAR(MAX) NOT NULL,
    Descricao NVARCHAR(MAX) NULL,
    EmpresaId INT NOT NULL,
    OrientadorId INT NOT NULL,
    CONSTRAINT FK_Estagios_Empresas FOREIGN KEY (EmpresaId) REFERENCES Empresas(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Estagios_Orientadores FOREIGN KEY (OrientadorId) REFERENCES Orientadores(Id) ON DELETE NO ACTION
);

GO

CREATE TABLE EstagioAluno (
    EstagioId INT NOT NULL,
    AlunoId INT NOT NULL,
    CONSTRAINT PK_EstagioAluno PRIMARY KEY (EstagioId, AlunoId),
    CONSTRAINT FK_EstagioAluno_Estagios FOREIGN KEY (EstagioId) REFERENCES Estagios(Id) ON DELETE CASCADE,
    CONSTRAINT FK_EstagioAluno_Alunos FOREIGN KEY (AlunoId) REFERENCES Alunos(Id) ON DELETE CASCADE
);

GO
