-- ========================================
-- QUIMPAC - Datos de Prueba para Desarrollo Local
-- ========================================
-- IMPORTANTE: Solo para ambiente LOCAL/DESARROLLO
-- NO ejecutar en PRODUCCIÓN

USE DBQUIMPAC_LOCAL;

-- ========================================
-- CREAR TABLAS BÁSICAS
-- ========================================

-- Tabla clientes
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[clientes]') AND type in (N'U'))
CREATE TABLE clientes(
    cli_cod_cli int PRIMARY KEY IDENTITY(1,1),
    cli_raz_soc varchar(100) NOT NULL,
    cli_cod_sap nvarchar(10) NOT NULL,
    cli_pue_pla varchar(100),
    cli_org_ven nvarchar(100),
    cli_cen nvarchar(100),
    cli_rut nvarchar(100) NOT NULL,
    cli_pue_exp varchar(100) NOT NULL,
    cli_est nvarchar(10) NOT NULL,
    cli_fec_cre datetime,
    cli_usu_cre_sap nvarchar(10),
    cli_fec_mod datetime,
    cli_usu_mod_sap nvarchar(10)
);

-- Tabla rol
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rol]') AND type in (N'U'))
CREATE TABLE rol(
    rol_cod int PRIMARY KEY IDENTITY(1,1),
    rol_nom varchar(50) NOT NULL,
    rol_cod_usu_sap nvarchar(10),
    rol_est nvarchar(10) NOT NULL,
    rol_fec_cre datetime,
    rol_usu_cre_sap nvarchar(10),
    rol_fec_mod datetime,
    rol_usu_mod_sap nvarchar(10)
);

-- Tabla permiso
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[permiso]') AND type in (N'U'))
CREATE TABLE permiso(
    per_cod int PRIMARY KEY IDENTITY(1,1),
    per_nom varchar(50) NOT NULL,
    per_uri varchar(100),
    per_uri_icon varchar(50),
    per_est nvarchar(10) NOT NULL,
    per_fec_cre datetime,
    per_usu_cre_sap nvarchar(10),
    per_fec_mod datetime,
    per_usu_mod_sap nvarchar(10)
);

-- Tabla usuarios
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usuarios]') AND type in (N'U'))
CREATE TABLE usuarios(
    usu_cod_usu int PRIMARY KEY IDENTITY(1,1),
    usu_cod_cli nvarchar(10),
    usu_cod_rol int,
    usu_cod_soc nvarchar(10),
    usu_nom_ape varchar(100) NOT NULL,
    usu_usu varchar(50) NOT NULL,
    usu_cla varchar(255) NOT NULL,
    usu_cor varchar(100),
    usu_cod_sap nvarchar(10),
    usu_est nvarchar(10),
    usu_fec_cre datetime,
    usu_usu_cre_sap nvarchar(10),
    usu_fec_mod datetime,
    usu_usu_mod_sap nvarchar(10),
    FOREIGN KEY (usu_cod_rol) REFERENCES rol(rol_cod)
);

-- Tabla rol_permiso
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rol_permiso]') AND type in (N'U'))
CREATE TABLE rol_permiso(
    rol_per_cod int PRIMARY KEY IDENTITY(1,1),
    rol_per_cod_rol int,
    rol_per_cod_per int,
    rol_per_est nvarchar(10) NOT NULL,
    rol_per_fec_cre datetime,
    rol_per_usu_cre_sap nvarchar(10),
    rol_per_fec_mod datetime,
    rol_per_usu_mod_sap nvarchar(10),
    FOREIGN KEY (rol_per_cod_rol) REFERENCES rol(rol_cod),
    FOREIGN KEY (rol_per_cod_per) REFERENCES permiso(per_cod)
);

-- Limpiar datos existentes
DELETE FROM rol_permiso;
DELETE FROM usuarios;
DELETE FROM permiso;
DELETE FROM rol;
DELETE FROM clientes;

-- ========================================
-- 1. ROLES
-- ========================================
INSERT INTO rol (rol_nom, rol_cod_usu_sap, rol_est, rol_fec_cre) VALUES
('Administrador', 'ADMIN', '1', GETDATE()),
('Usuario', 'USER', '1', GETDATE()),
('Cliente', 'CLIENT', '1', GETDATE());

-- ========================================
-- 2. CLIENTES DE PRUEBA
-- ========================================
INSERT INTO clientes (cli_raz_soc, cli_cod_sap, cli_pue_pla, cli_org_ven, cli_cen, cli_rut, cli_pue_exp, cli_est, cli_fec_cre, cli_usu_cre_sap) VALUES
('QUIMPAC S.A.', 'QC001', 'Lima', 'Ventas Lima', 'Centro Lima', 'RUT001', 'Exp Lima', '1', GETDATE(), 'ADMIN'),
('Cliente Demo 1', 'CL001', 'Callao', 'Ventas Norte', 'Centro Norte', 'RUT002', 'Exp Norte', '1', GETDATE(), 'ADMIN'),
('Cliente Demo 2', 'CL002', 'Arequipa', 'Ventas Sur', 'Centro Sur', 'RUT003', 'Exp Sur', '1', GETDATE(), 'ADMIN');

-- ========================================
-- 3. PERMISOS
-- ========================================
INSERT INTO permiso (per_nom, per_uri, per_uri_icon, per_est, per_fec_cre, per_usu_cre_sap) VALUES
('Dashboard', '/home', 'dashboard', '1', GETDATE(), 'ADMIN'),
('Clientes', '/cliente', 'people', '1', GETDATE(), 'ADMIN'),
('Stock', '/stock', 'inventory', '1', GETDATE(), 'ADMIN'),
('Movimientos', '/movimientos', 'swap_horiz', '1', GETDATE(), 'ADMIN'),
('Órdenes', '/ordenes', 'assignment', '1', GETDATE(), 'ADMIN'),
('Vehículos', '/vehiculo', 'directions_car', '1', GETDATE(), 'ADMIN'),
('Choferes', '/chofer', 'person', '1', GETDATE(), 'ADMIN'),
('Usuarios', '/usuarios', 'group', '1', GETDATE(), 'ADMIN'),
('Entregas', '/entrega', 'local_shipping', '1', GETDATE(), 'ADMIN'),
('Aprobar', '/aprobar', 'check_circle', '1', GETDATE(), 'ADMIN');

-- ========================================
-- 4. USUARIOS DE PRUEBA
-- ========================================
-- NOTA: Las contraseñas serán hasheadas por el servicio BCrypt cuando el usuario haga login
-- Por ahora usamos texto plano, el sistema las convertirá automáticamente
INSERT INTO usuarios (usu_cod_cli, usu_cod_rol, usu_cod_soc, usu_nom_ape, usu_usu, usu_cla, usu_cor, usu_cod_sap, usu_est, usu_fec_cre, usu_usu_cre_sap) VALUES
('QC001', 1, 'SOC001', 'Administrador Sistema', 'admin', 'admin123', 'admin@quimpac.com', 'ADMIN', '1', GETDATE(), 'SYSTEM'),
('CL001', 2, 'SOC002', 'Usuario Demo', 'user', 'user123', 'user@quimpac.com', 'USER', '1', GETDATE(), 'ADMIN'),
('CL002', 3, 'SOC003', 'Cliente Demo', 'client', 'client123', 'client@quimpac.com', 'CLIENT', '1', GETDATE(), 'ADMIN');

-- ========================================
-- 5. ASIGNACIÓN ROL-PERMISOS
-- ========================================
-- Administrador: Todos los permisos
INSERT INTO rol_permiso (rol_per_cod_rol, rol_per_cod_per, rol_per_est, rol_per_fec_cre, rol_per_usu_cre_sap)
SELECT 1, per_cod, '1', GETDATE(), 'ADMIN'
FROM permiso;

-- Usuario: Permisos básicos (sin usuarios ni aprobar)
INSERT INTO rol_permiso (rol_per_cod_rol, rol_per_cod_per, rol_per_est, rol_per_fec_cre, rol_per_usu_cre_sap) VALUES
(2, 1, '1', GETDATE(), 'ADMIN'), -- Dashboard
(2, 2, '1', GETDATE(), 'ADMIN'), -- Clientes
(2, 3, '1', GETDATE(), 'ADMIN'), -- Stock
(2, 4, '1', GETDATE(), 'ADMIN'), -- Movimientos
(2, 5, '1', GETDATE(), 'ADMIN'), -- Órdenes
(2, 6, '1', GETDATE(), 'ADMIN'), -- Vehículos
(2, 7, '1', GETDATE(), 'ADMIN'), -- Choferes
(2, 9, '1', GETDATE(), 'ADMIN'); -- Entregas

-- Cliente: Solo lectura
INSERT INTO rol_permiso (rol_per_cod_rol, rol_per_cod_per, rol_per_est, rol_per_fec_cre, rol_per_usu_cre_sap) VALUES
(3, 1, '1', GETDATE(), 'ADMIN'), -- Dashboard
(3, 3, '1', GETDATE(), 'ADMIN'), -- Stock
(3, 4, '1', GETDATE(), 'ADMIN'), -- Movimientos
(3, 9, '1', GETDATE(), 'ADMIN'); -- Entregas

-- ========================================
-- VERIFICACIÓN
-- ========================================
SELECT 'ROLES CREADOS' as Tabla, COUNT(*) as Cantidad FROM rol;
SELECT 'CLIENTES CREADOS' as Tabla, COUNT(*) as Cantidad FROM clientes;
SELECT 'PERMISOS CREADOS' as Tabla, COUNT(*) as Cantidad FROM permiso;
SELECT 'USUARIOS CREADOS' as Tabla, COUNT(*) as Cantidad FROM usuarios;
SELECT 'ROL-PERMISOS CREADOS' as Tabla, COUNT(*) as Cantidad FROM rol_permiso;

-- ========================================
-- CREDENCIALES DE PRUEBA
-- ========================================
-- Usuario: admin    | Password: admin123  | Rol: Administrador (Todos los permisos)
-- Usuario: user     | Password: user123   | Rol: Usuario (Permisos limitados)
-- Usuario: client   | Password: client123 | Rol: Cliente (Solo lectura)
-- ========================================

PRINT 'Datos de prueba insertados correctamente';
PRINT '===========================================';
PRINT 'CREDENCIALES DE PRUEBA:';
PRINT 'admin / admin123 (Administrador)';
PRINT 'user / user123 (Usuario)';  
PRINT 'client / client123 (Cliente)';
PRINT '===========================================';