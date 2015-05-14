using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Urbetrack.DAL.DAO.BaseClasses;
using Urbetrack.Types.BusinessObjects.Dispositivos;

namespace Urbetrack.DAL.DAO.BusinessObjects.Dispositivos
{
	public class TemperatureSensorEntropyDAO : GenericDAO<TemperatureSensorEntropy>
	{
		public TemperatureSensorEntropyDAO(ISession session) : base(session) { }

		public TemperatureSensorEntropy Get(String serial_)
		{
			var serial = serial_.TrimStart('0');
			return Session.Query<TemperatureSensorEntropy>().Where(e => (e.Serial == serial)).Cacheable().SingleOrDefault();
		}

        public List<TemperatureSensorEntropy> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Session.Query<TemperatureSensorEntropy>()
                          //.FilterEmpresa(Session, empresas)
                          //.FilterLinea(Session, empresas, lineas)
                          .ToList();
        }

		public void SetTemperatura(int deviceid, String serial, float temperatura, DateTime dt)
		{
			var ent = Get(serial) ?? new TemperatureSensorEntropy
  			{
				Serial = serial,
				ButtonPressed = false,
			};
			ent.Temperature = temperatura;
			ent.Connected = true;
			ent.DateTime = dt;
			SaveOrUpdate(ent);
		}

		public void SetDesconexion(String serial, DateTime dt)
		{
			var ent = Get(serial) ?? new TemperatureSensorEntropy
			{
				Serial = serial,
				ButtonPressed = false,
			};
			ent.DateTime = dt;
			ent.Connected = false;
			SaveOrUpdate(ent);
		}

		public void SetBotonPresionado(String serial, DateTime dt)
		{
			var ent = Get(serial) ?? new TemperatureSensorEntropy
			{
				Serial = serial,
				Connected = true,
			};
			ent.DateTime = dt;
			ent.ButtonPressed = true;
			SaveOrUpdate(ent);
		}

		public void SetBotonLiberado(String serial, DateTime dt)
		{
			var ent = Get(serial) ?? new TemperatureSensorEntropy
			{
				Serial = serial,
				Connected = true,
			};
			ent.DateTime = dt;
			ent.ButtonPressed = true;
			SaveOrUpdate(ent);
		}
	}
}
