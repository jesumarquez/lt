using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObject.Positions;
using System;

namespace HandlerTest.Classes
{
    public class Data: IDisposable
    {
        public readonly DAOFactory DaoFactory;

        public Data()
        {
            DaoFactory = new DAOFactory();
        }

        public List<Empresa>  GetEmpresas()
        {
            return DaoFactory.EmpresaDAO.GetList().OrderBy(e => e.RazonSocial).ToList();
        }

        public List<Linea> GetLineas(Empresa empresa)
        {
            return GetLineas(empresa != null ? empresa.Id : -1);
        }
        public List<Linea> GetLineas(int empresa)
        {
            return DaoFactory.LineaDAO.GetList(new[] { empresa }).OrderBy(e => e.Descripcion).ToList();
        }

        public List<Coche> GetVehiculos(Empresa empresa, Linea linea)
        {
            return GetVehiculos(empresa != null ? empresa.Id : -1, linea != null ? linea.Id : -1);
        }
        public List<Coche> GetVehiculos(int empresa, int linea)
        {
            return DaoFactory.CocheDAO.GetList(new[] { empresa }, new[] { linea })
                .Where(e=>e.Dispositivo != null && e.Estado != Coche.Estados.Inactivo)
                .OrderBy(e => e.Interno).ToList();
        }
        public List<Mensaje> GetMensajes(Empresa empresa, Linea linea)
        {
            return DaoFactory.MensajeDAO.FindByEmpresaYLineaAndUser(empresa, linea, null)
                .OrderBy(e => e.Descripcion).ToList();
        }
        public LogUltimaPosicionVo GetLastPosition(Coche vehiculo)
        {
            return vehiculo == null ? null : DaoFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(vehiculo);
        }

        public ViajeDistribucion GetDistribucion(Coche vehiculo)
        {
            return DaoFactory.ViajeDistribucionDAO.FindEnCurso(vehiculo);
        }

        public void Dispose()
        {
            DaoFactory.Dispose();
            SessionHelper.CloseSession();
        }
    }
}
