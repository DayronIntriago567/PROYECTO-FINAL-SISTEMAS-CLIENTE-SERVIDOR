-- =========================================
-- BASE DE DATOS: BikeStore
-- =========================================
CREATE DATABASE BikeStore;
GO

USE BikeStore;
GO

-- =========================================
-- TABLA: Categoria
-- =========================================
CREATE TABLE Categoria (
    IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(200) NULL,
    Activo BIT NOT NULL DEFAULT 1
);
GO

-- =========================================
-- TABLA: Bicicleta
-- =========================================
CREATE TABLE Bicicleta (
    IdBicicleta INT IDENTITY(1,1) PRIMARY KEY,
    IdCategoria INT NOT NULL,
    Marca VARCHAR(50) NOT NULL,
    Modelo VARCHAR(50) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL DEFAULT 0,
    Estado VARCHAR(20) NOT NULL DEFAULT 'Disponible',
    CONSTRAINT FK_Bicicleta_Categoria FOREIGN KEY (IdCategoria)
        REFERENCES Categoria(IdCategoria)
);
GO

-- =========================================
-- TABLA: Cliente
-- =========================================
CREATE TABLE Cliente (
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    Cedula VARCHAR(10) NOT NULL UNIQUE,
    Nombres VARCHAR(80) NOT NULL,
    Apellidos VARCHAR(80) NOT NULL,
    Telefono VARCHAR(15) NULL,
    Correo VARCHAR(100) NULL
);
GO

-- =========================================
-- TABLA: Venta
-- =========================================
CREATE TABLE Venta (
    IdVenta INT IDENTITY(1,1) PRIMARY KEY,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    IdCliente INT NOT NULL,
    Total DECIMAL(10,2) NOT NULL DEFAULT 0,
    CONSTRAINT FK_Venta_Cliente FOREIGN KEY (IdCliente)
        REFERENCES Cliente(IdCliente)
);
GO

-- =========================================
-- TABLA: DetalleVenta
-- =========================================
CREATE TABLE DetalleVenta (
    IdDetalle INT IDENTITY(1,1) PRIMARY KEY,
    IdVenta INT NOT NULL,
    IdBicicleta INT NOT NULL,
    Cantidad INT NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Subtotal AS (Cantidad * Precio) PERSISTED,
    CONSTRAINT FK_Detalle_Venta FOREIGN KEY (IdVenta)
        REFERENCES Venta(IdVenta),
    CONSTRAINT FK_Detalle_Bicicleta FOREIGN KEY (IdBicicleta)
        REFERENCES Bicicleta(IdBicicleta)
);
GO

-- =========================================
-- INSERTS DE PRUEBA
-- =========================================

-- Categorias
INSERT INTO Categoria (Nombre, Descripcion, Activo) VALUES
('Montaña', 'Bicicletas para terreno irregular', 1),
('Ruta', 'Bicicletas para pavimento y velocidad', 1),
('BMX', 'Bicicletas para acrobacias', 1),
('Electricas', 'Bicicletas con motor electrico', 1),
('Infantiles', 'Bicicletas para niños', 1);
GO

-- Bicicletas
INSERT INTO Bicicleta (IdCategoria, Marca, Modelo, Precio, Stock, Estado) VALUES
(1, 'Trek', 'Marlin 7', 850.00, 10, 'Disponible'),
(1, 'GT', 'Aggressor', 620.00, 3, 'Disponible'),
(2, 'Specialized', 'Allez', 1200.00, 5, 'Disponible'),
(3, 'Mongoose', 'Legion L20', 450.00, 0, 'Agotado'),
(4, 'Cannondale', 'Tesoro Neo', 2100.00, 8, 'Disponible'),
(5, 'Huffy', 'Rock It', 180.00, 15, 'Disponible');
GO

-- Clientes
INSERT INTO Cliente (Cedula, Nombres, Apellidos, Telefono, Correo) VALUES
('1712345678', 'Juan', 'Perez Lopez', '0991234567', 'juan.perez@mail.com'),
('1798765432', 'Maria', 'Gonzalez Ruiz', '0987654321', 'maria.gonzalez@mail.com'),
('1755566677', 'Carlos', 'Mendez Solis', '0976655443', 'carlos.mendez@mail.com');
GO

-- Ventas
INSERT INTO Venta (IdCliente, Total) VALUES
(1, 0),
(2, 0);
GO

-- Detalle de ventas
INSERT INTO DetalleVenta (IdVenta, IdBicicleta, Cantidad, Precio) VALUES
(1, 1, 1, 850.00),
(1, 6, 2, 180.00),
(2, 3, 1, 1200.00);
GO

-- Actualizar totales de venta segun el detalle
UPDATE Venta
SET Total = (SELECT SUM(Subtotal) FROM DetalleVenta WHERE DetalleVenta.IdVenta = Venta.IdVenta)
WHERE IdVenta IN (1, 2);
GO