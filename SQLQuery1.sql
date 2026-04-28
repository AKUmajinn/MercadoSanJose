USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'MercadoSanJoseDB')
BEGIN
    ALTER DATABASE MercadoSanJoseDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MercadoSanJoseDB;
END
GO

CREATE DATABASE MercadoSanJoseDB;
GO

USE MercadoSanJoseDB;
GO

CREATE TABLE Persona (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DNI NVARCHAR(20) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Telefono NVARCHAR(20) NOT NULL,
    EsGerencia BIT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE ConceptoDeuda (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    MontoBase DECIMAL(10,2) NOT NULL
);
GO

CREATE TABLE Puesto (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NumeroPuesto NVARCHAR(10) NOT NULL,
    Sector NVARCHAR(50) NOT NULL,
    PropietarioId INT NULL,
    InquilinoId INT NULL,
    Estado INT NOT NULL,
    CONSTRAINT FK_Puesto_Propietario FOREIGN KEY (PropietarioId) REFERENCES Persona(Id),
    CONSTRAINT FK_Puesto_Inquilino FOREIGN KEY (InquilinoId) REFERENCES Persona(Id)
);
GO

CREATE TABLE Deuda (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PuestoId INT NOT NULL,
    ConceptoDeudaId INT NOT NULL,
    ResponsableId INT NOT NULL,
    FechaEmision DATETIME2 NOT NULL,
    MontoTotal DECIMAL(10,2) NOT NULL,
    Estado INT NOT NULL,
    CONSTRAINT FK_Deuda_Puesto FOREIGN KEY (PuestoId) REFERENCES Puesto(Id),
    CONSTRAINT FK_Deuda_ConceptoDeuda FOREIGN KEY (ConceptoDeudaId) REFERENCES ConceptoDeuda(Id),
    CONSTRAINT FK_Deuda_Responsable FOREIGN KEY (ResponsableId) REFERENCES Persona(Id)
);
GO

CREATE TABLE Pago (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DeudaId INT NOT NULL,
    FechaPago DATETIME2 NOT NULL,
    MontoPagado DECIMAL(10,2) NOT NULL,
    NumeroRecibo NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_Pago_Deuda FOREIGN KEY (DeudaId) REFERENCES Deuda(Id)
);
GO

INSERT INTO Persona (DNI, Nombre, Telefono, EsGerencia, Activo)
VALUES 
('00000000', 'Gerencia', '000000000', 1, 1),
('11111111', 'Maria Lopez', '111111111', 0, 1),
('22222222', 'Ana Torres', '222222222', 0, 1);
GO

INSERT INTO ConceptoDeuda (Nombre, MontoBase)
VALUES 
('Limpieza', 50.00),
('Vigilancia', 70.00);
GO

INSERT INTO Puesto (NumeroPuesto, Sector, Estado, PropietarioId, InquilinoId)
VALUES 
('103', 'A', 0, 1, NULL),
('104', 'A', 0, 1, NULL),
('105', 'A', 0, 1, NULL),
('106', 'A', 0, 1, NULL),
('107', 'A', 0, 1, NULL),
('108', 'A', 0, 1, NULL),
('109', 'A', 1, 2, NULL),
('110', 'A', 1, 3, NULL),
('202', 'B', 0, 1, NULL),
('203', 'B', 0, 1, NULL),
('204', 'B', 2, 1, 2),
('205', 'B', 2, 1, 3),
('206', 'B', 1, 2, NULL),
('207', 'B', 1, 3, NULL),
('208', 'B', 2, 2, 3),
('209', 'B', 0, 1, NULL),
('210', 'B', 0, 1, NULL);
GO