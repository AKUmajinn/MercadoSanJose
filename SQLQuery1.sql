-- 1. Crear la Base de Datos
CREATE DATABASE MercadoSanJose;
GO

USE MercadoSanJose;
GO

-- 1. Tabla Persona
CREATE TABLE Persona (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DNI VARCHAR(20) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Telefono VARCHAR(20),
    EsGerencia BIT DEFAULT 0
);

-- 2. Tabla Puesto
CREATE TABLE Puesto (
    Id INT PRIMARY KEY IDENTITY(1,1),
    NumeroPuesto INT NOT NULL,
    Sector VARCHAR(50),
    Estado INT DEFAULT 0, -- 0:Disponible, 1:Vendido, 2:Alquilado
    PropietarioId INT NOT NULL,
    InquilinoId INT NULL,
    FOREIGN KEY (PropietarioId) REFERENCES Persona(Id),
    FOREIGN KEY (InquilinoId) REFERENCES Persona(Id)
);

-- 3. Tabla ConceptoDeuda
CREATE TABLE ConceptoDeuda (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(100) NOT NULL,
    MontoBase DECIMAL(18,2) NOT NULL
);

-- 4. Tabla Deuda
CREATE TABLE Deuda (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PuestoId INT NOT NULL,
    ConceptoDeudaId INT NOT NULL,
    ResponsableId INT NOT NULL,
    FechaEmision DATETIME DEFAULT GETDATE(),
    MontoTotal DECIMAL(18,2) NOT NULL,
    Estado INT DEFAULT 0, -- 0:Pendiente, 1:Pagada
    FOREIGN KEY (PuestoId) REFERENCES Puesto(Id),
    FOREIGN KEY (ConceptoDeudaId) REFERENCES ConceptoDeuda(Id),
    FOREIGN KEY (ResponsableId) REFERENCES Persona(Id)
);

-- 5. Tabla Pago
CREATE TABLE Pago (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DeudaId INT NOT NULL,
    FechaPago DATETIME DEFAULT GETDATE(),
    MontoPagado DECIMAL(18,2) NOT NULL,
    NumeroRecibo VARCHAR(50) NOT NULL,
    FOREIGN KEY (DeudaId) REFERENCES Deuda(Id)
);

INSERT INTO Persona (DNI, Nombre, Telefono, EsGerencia) 
VALUES ('00000000', 'GERENCIA GENERAL', '999999999', 1);