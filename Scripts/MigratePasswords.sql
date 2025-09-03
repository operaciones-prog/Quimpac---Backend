-- Script para migración de contraseñas a hash BCrypt
-- IMPORTANTE: Ejecutar solo en ambiente de desarrollo/pruebas primero
-- Este script es para casos donde se necesite migrar todas las contraseñas de una vez

USE DBQUIMPAC_PRD;
GO

-- Crear tabla temporal para backup de contraseñas originales
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='usuarios_password_backup' AND xtype='U')
CREATE TABLE usuarios_password_backup (
    usu_cod INT NOT NULL,
    usu_usu NVARCHAR(50) NOT NULL,
    usu_cla_original NVARCHAR(255) NOT NULL,
    fecha_backup DATETIME2 DEFAULT GETDATE()
);
GO

-- Crear función para identificar si una contraseña ya está hasheada
-- Las contraseñas BCrypt comienzan con $2a$, $2b$, $2x$ o $2y$ y tienen ~60 caracteres
IF OBJECT_ID('dbo.IsPasswordHashed', 'FN') IS NOT NULL
    DROP FUNCTION dbo.IsPasswordHashed;
GO

CREATE FUNCTION dbo.IsPasswordHashed(@password NVARCHAR(255))
RETURNS BIT
AS
BEGIN
    DECLARE @isHashed BIT = 0;
    
    IF @password IS NOT NULL 
       AND LEN(@password) >= 59 
       AND (@password LIKE '$2a$%' 
            OR @password LIKE '$2b$%' 
            OR @password LIKE '$2x$%' 
            OR @password LIKE '$2y$%')
    BEGIN
        SET @isHashed = 1;
    END
    
    RETURN @isHashed;
END;
GO

-- Ver usuarios con contraseñas en texto plano
SELECT 
    usu_cod,
    usu_usu,
    usu_nom,
    CASE 
        WHEN dbo.IsPasswordHashed(usu_cla) = 1 THEN 'HASHEADA'
        ELSE 'TEXTO PLANO'
    END as estado_password,
    LEN(usu_cla) as longitud_password
FROM Usuarios
WHERE usu_est = 1; -- Solo usuarios activos

-- Contar usuarios por estado de contraseña
SELECT 
    CASE 
        WHEN dbo.IsPasswordHashed(usu_cla) = 1 THEN 'HASHEADAS'
        ELSE 'TEXTO PLANO'
    END as estado,
    COUNT(*) as cantidad
FROM Usuarios
WHERE usu_est = 1
GROUP BY CASE 
    WHEN dbo.IsPasswordHashed(usu_cla) = 1 THEN 'HASHEADAS'
    ELSE 'TEXTO PLANO'
END;

-- NOTA IMPORTANTE:
-- La migración real de contraseñas se hará automáticamente por el backend
-- cuando cada usuario haga login por primera vez.
-- Este enfoque es más seguro porque:
-- 1. No requiere conocer las contraseñas en texto plano
-- 2. Se migra gradualmente usuario por usuario
-- 3. Solo se ejecuta cuando el usuario autentica correctamente

-- Para casos especiales donde se necesite forzar la migración:
-- 1. Comunicar a los usuarios que cambien sus contraseñas
-- 2. Usar el endpoint de cambio de contraseña que ya hashea automáticamente
-- 3. O crear un proceso especial con las contraseñas conocidas

PRINT 'Script de análisis completado.';
PRINT 'La migración real se ejecutará automáticamente en el primer login de cada usuario.';
PRINT 'Verificar logs del backend para monitorear el progreso de la migración.';

GO