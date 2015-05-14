#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Dispositivos;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Dispositivos
{
    public class FirmwareDAO : GenericDAO<Firmware>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public FirmwareDAO(ISession session) : base(session) { }

        #endregion

        public new IEnumerable<Firmware> FindAll() { return base.FindAll().ToList(); }

        public override void Delete(Firmware obj)
        {
            var dispositivos = Session.Query<Dispositivo>().Where(device => device.FlashedFirmware != null && device.FlashedFirmware.Id == obj.Id).ToList();

            foreach (var dispositivo in dispositivos) dispositivo.FlashedFirmware = null;

            var deviceTypes = Session.Query<TipoDispositivo>().Where(type => type.Firmware != null && type.Firmware.Id == obj.Id).ToList();

            foreach (var tipoDispositivo in deviceTypes) tipoDispositivo.Firmware = null;
            
            base.Delete(obj);
        }
    }
}
