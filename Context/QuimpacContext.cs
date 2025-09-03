using Microsoft.EntityFrameworkCore;
using PROYEC_QUIMPAC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Context
{
    public class QuimpacContext:DbContext
    {
        public QuimpacContext()
        {
        }

        public QuimpacContext(DbContextOptions<QuimpacContext> options) : base(options)
        {
        }

        public virtual DbSet<Clientes> Clientes { get; set; }
        public virtual DbSet<Permiso> Permiso { get; set; }
        public virtual DbSet<Rol> Rol { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }
        public virtual DbSet<Rol_permiso> Rol_Permiso { get; set; }
        public virtual DbSet<Resumen_stock_Cliente> Resumen_Stock_Cliente { get; set; }
        public virtual DbSet<Resumen_stock_Cliente_aux> Resumen_Stock_Cliente_aux { get; set; }
        public virtual DbSet<Stock> Stock { get; set; }
        public virtual DbSet<Stock_aux> Stock_aux { get; set; }
        public virtual DbSet<Ordenes> Ordenes { get; set; }
        public virtual DbSet<Ordenes_aux> Ordenes_aux { get; set; }
        public virtual DbSet<Placa> Placa { get; set; }
        public virtual DbSet<Chofer> Chofer { get; set; }
        public virtual DbSet<Horario> Horario { get; set; }
        public virtual DbSet<Estatus> Estatus { get; set; }
        public virtual DbSet<Entrega> Entrega { get; set; }
        public virtual DbSet<Detalle_Entrega> Detalle_Entrega { get; set; }
        public virtual DbSet<Correlativo> Correlativo { get; set; }
        public virtual DbSet<Actualizaciones> Actualizaciones { get; set; }
        public virtual DbSet<Sociedad> Sociedad { get; set; }
        public virtual DbSet<Constantes> Constantes { get; set; }
    }
}
