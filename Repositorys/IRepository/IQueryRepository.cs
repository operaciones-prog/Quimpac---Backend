using Newtonsoft.Json.Linq;
using PROYEC_QUIMPAC.Models;
using PROYEC_QUIMPAC.Models.ModelFil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Repositorys.IRepository
{
    public interface IQueryRepository
    {
        public List<Usuarios> usuariosXcliente(int id);
        public List<Stock> listaStock();
        public List<Ordenes> listaOrdenes();
        public List<StockTotalFil> listaStockXCliente(StockFil stk);
        //public List<Stock_Cliente> visualizarMov(StockClienteFil stk);
        public List<Resumen_stock_Cliente> visualizarMov2(StockClienteFil stk);
        public List<Ordenes> visualizarOrdenes(OrdenesFil ord);
        public List<Rol_permiso> permisosXRol(int rol);
        //entregas y detalle entrega
        public string newEntrega(JObject json);
        public string editarEntrega(JObject json);
        //solicitud de entrega
        public List<Entrega> SolicEntrega(string org_ven, string cod_cho, string pla_veh, string fecha, string cod_hor);
        //listar entregas con status S
        public List<ListaXestadoFil> listaXestado(ListaXestadoFil2 lxs);
        public Entrega buscarEntrega(string num_solicitud);
        public string buscarHora(string cod_hor);
        public List<Horario> listaHora();
        public string actualizarEstadoEntrega(JObject json);
        public List<ListaStockXCliente> listaStockXCliente2(StockFil stk);
        public string validarStock(string cod_sap, string cod_almacen, string cod_producto,string lote);
        public void SendEmailAsync(List<Usuarios> lstAdm, string subject, string message, string ind_soc);
        public string eliminarDetalleEntrega(JObject json);
        public void SendEmailQC(List<String> lstDestinatarios, string subject, string message, string ind_soc);
        public List<Constantes> obtenerConstante(string codigo);
        public Task<List<Combo>> postCombo(OpcionCentro r);
        public string cargaOrdenes(JObject json);
        public string cargaMovimientos(JObject json);
        public string cargaStock(JObject json);
    } 
}
