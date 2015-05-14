#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Dispositivos;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Dispositivos
{
    public class TipoParametroDispositivoDAO : GenericDAO<TipoParametroDispositivo>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public TipoParametroDispositivoDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets all the Parameters for the devices and the type selected by an Hibernate query
        /// </summary>
        /// <param name="idTipoDispositivo"></param>
        /// <returns></returns>
        public IEnumerable<TipoParametroDispositivo> FindByTipoDispositivo(Int32 idTipoDispositivo)
        {
            var tipoDispositivoDao = new TipoDispositivoDAO();

            var tipoDispositivo = idTipoDispositivo > 0 ? tipoDispositivoDao.FindById(idTipoDispositivo) : null;

            if (tipoDispositivo == null) return FindAll().OfType<TipoParametroDispositivo>().ToList();

            var ids = GetDeviceTypeIds(tipoDispositivo);

            return Session.Query<TipoParametroDispositivo>().Where(param => param.DispositivoTipo != null && ids.Contains(param.DispositivoTipo.Id)).OrderBy(param => param.Nombre)
                .Cacheable().ToList();
        }

        /// <summary>
        /// When saving a new parameter type, generates a new configuration detail entry for all devices of the associated type.
        /// </summary>
        /// <param name="obj"></param>
        public override void SaveOrUpdate(TipoParametroDispositivo obj)
        {
            var dispositivosDAO = new DispositivoDAO();

            var devices = obj.Id > 0 ? new List<Dispositivo>() : dispositivosDAO.GetByTipo(obj.DispositivoTipo);

            if (obj.Id.Equals(0))
            {
                foreach (var device in devices)
                {
                    var detail = new DetalleDispositivo { Dispositivo = device, Revision = (device.GetMaxRevision() + 1), TipoParametro = obj, Valor = obj.ValorInicial };

                    obj.DispositivoDetalle.Add(detail);

                    device.AddDetalleDispositivo(detail);
                }
            }

            base.SaveOrUpdate(obj);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a list of all device types ids of the specified device hierarchy.
        /// </summary>
        /// <param name="tipoDispositivo"></param>
        /// <returns></returns>
        private static List<Int32> GetDeviceTypeIds(TipoDispositivo tipoDispositivo)
        {
            var ids = new List<Int32> { tipoDispositivo.Id };

            var parent = tipoDispositivo.TipoDispositivoPadre;

            while (parent != null)
            {
                ids.Add(parent.Id);

                parent = parent.TipoDispositivoPadre;
            }

            return ids;
        }

        #endregion
    }
}
