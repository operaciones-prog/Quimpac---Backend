#!/bin/bash

# ========================================
# Script de inicialización de BD QUIMPAC Local
# ========================================

echo "🚀 Iniciando configuración de base de datos QUIMPAC Local..."

# Esperar a que SQL Server esté listo
echo "⏳ Esperando a que SQL Server esté disponible..."
sleep 30

# Crear la base de datos
echo "🗃️ Creando base de datos DBQUIMPAC_LOCAL..."
/opt/mssql-tools18/bin/sqlcmd -S quimpac-sqlserver -U sa -P "QuimpacLocal2025!" -C -Q "
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DBQUIMPAC_LOCAL')
BEGIN
    CREATE DATABASE DBQUIMPAC_LOCAL
    PRINT 'Base de datos DBQUIMPAC_LOCAL creada exitosamente'
END
ELSE
    PRINT 'Base de datos DBQUIMPAC_LOCAL ya existe'
"

# Ejecutar script de datos de prueba (incluye creación de tablas)
echo "📊 Creando tablas e insertando datos de prueba..."
/opt/mssql-tools18/bin/sqlcmd -S quimpac-sqlserver -U sa -P "QuimpacLocal2025!" -C -d DBQUIMPAC_LOCAL -i /scripts/SeedData.sql

echo "✅ Base de datos QUIMPAC Local configurada correctamente!"
echo ""
echo "🔑 CREDENCIALES DE PRUEBA:"
echo "   👤 admin / admin123 (Administrador)"
echo "   👤 user / user123 (Usuario)"
echo "   👤 client / client123 (Cliente)"
echo ""
echo "🌐 URLs disponibles:"
echo "   📊 Backend API: http://localhost:5002"
echo "   📊 Swagger: http://localhost:5002/swagger"  
echo "   🖥️ Frontend: http://localhost:4200"
echo ""