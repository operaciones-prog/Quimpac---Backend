using Newtonsoft.Json.Linq;
using PROYEC_QUIMPAC.Models;
using PROYEC_QUIMPAC.Models.ModelFil;
using PROYEC_QUIMPAC.Repositorys.IRepository;
using PROYEC_QUIMPAC.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYEC_QUIMPAC.Services
{
    public class QueryService : IQueryService
    {
        private IQueryRepository _queryRepository;
        public QueryService(IQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public string actualizarEstadoEntrega(JObject json)
        {
            return _queryRepository.actualizarEstadoEntrega(json);
        }

        public Entrega buscarEntrega(string num_solicitud)
        {
            return _queryRepository.buscarEntrega(num_solicitud);
        }

        public string buscarHora(string cod_hor)
        {
            return _queryRepository.buscarHora(cod_hor);
        }

        public string editarEntrega(JObject json)
        {
            return _queryRepository.editarEntrega(json);
        }

        public List<Horario> listaHora()
        {
            return _queryRepository.listaHora();
        }

        public List<Ordenes> listaOrdenes()
        {
            return _queryRepository.listaOrdenes();
        }

        public List<Stock> listaStock()
        {
            return _queryRepository.listaStock();
        }

        public List<StockTotalFil> listaStockXCliente(StockFil stk)
        {
            return _queryRepository.listaStockXCliente(stk);
        }

        public List<ListaStockXCliente> listaStockXCliente2(StockFil stk)
        {
            return _queryRepository.listaStockXCliente2(stk);
        }

        public List<ListaXestadoFil> listaXestado(ListaXestadoFil2 lxs)
        {
            return _queryRepository.listaXestado(lxs);
        }

        public string newEntrega(JObject json)
        {
            return _queryRepository.newEntrega(json);
        }

        public List<Rol_permiso> permisosXRol(int rol)
        {
            return _queryRepository.permisosXRol(rol);
        }

        public void SendEmailAsync(List<Usuarios> lstAdm, string subject, string message, string ind_soc)
        {
            _queryRepository.SendEmailAsync(lstAdm, subject, message, ind_soc);
        }

        public List<Entrega> SolicEntrega(string org_ven, string cod_cho, string pla_veh, string fecha, string cod_hor)
        {
            return _queryRepository.SolicEntrega(org_ven,cod_cho,pla_veh,fecha,cod_hor);
        }

        public List<Usuarios> usuariosXcliente(int id)
        {
            return _queryRepository.usuariosXcliente(id);
        }

        public string validarStock(string cod_sap, string cod_almacen, string cod_producto, string lote)
        {
            return _queryRepository.validarStock(cod_sap, cod_almacen, cod_producto,lote);
        }

        /*public List<Stock_Cliente> visualizarMov(StockClienteFil stk)
        {
            return _queryRepository.visualizarMov(stk);
        }*/

        public List<Resumen_stock_Cliente> visualizarMov2(StockClienteFil stk)
        {
            return _queryRepository.visualizarMov2(stk);
        }

        public List<Ordenes> visualizarOrdenes(OrdenesFil ord)
        {
            return _queryRepository.visualizarOrdenes(ord);
        }

        public string eliminarDetalleEntrega(JObject json)
        {
            return _queryRepository.eliminarDetalleEntrega(json);
        }

        public void SendEmailQC(List<String> lstDestinatarios, string subject, string message, string ind_soc)
        {
            _queryRepository.SendEmailQC(lstDestinatarios, subject, message, ind_soc);
        }

        public List<Constantes> obtenerConstante(string codigo)
        {
            return _queryRepository.obtenerConstante(codigo);
        }

        public async Task<List<Combo>> postCombo(OpcionCentro r)
        {
            var w = await _queryRepository.postCombo(r);
            return w;
        }

        public string cargaOrdenes(JObject json)
        {
            return _queryRepository.cargaOrdenes(json);
        }
        public string cargaMovimientos(JObject json)
        {
            return _queryRepository.cargaMovimientos(json);
        }

        public string cargaStock(JObject json)
        {
            return _queryRepository.cargaStock(json);
        }
    }
}
