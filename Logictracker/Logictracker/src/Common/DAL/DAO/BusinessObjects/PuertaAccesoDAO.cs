using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Utils;
using NHibernate;
using Logictracker.DAL.DAO.BusinessObjects.ReferenciasGeograficas;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class PuertaAccesoDAO : GenericDAO<PuertaAcceso>
    {
//        public PuertaAccesoDAO(ISession session) : base(session){}

        public PuertaAcceso FindByCodigo(int empresa, int linea, short codigo)
        {
            return Query.FilterEmpresa(Session, new[]{empresa})
                .FilterLinea(Session, new[] { empresa }, new[] { linea })
                .SingleOrDefault(p => p.Codigo == codigo && !p.Baja);
        }

		public PuertaAcceso FindByVehiculo(int vehiculo)
		{
			return Query.Where(p => p.Vehiculo.Id == vehiculo && !p.Baja).SafeFirstOrDefault();
		}

        public PuertaAcceso FindByReferenciaGeografica(int referencia)
        {
            return Query.Where(p => p.ReferenciaGeografica.Id == referencia && !p.Baja).SafeFirstOrDefault();
        }

        public List<PuertaAcceso> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .Where(p => !p.Baja)
                .ToList();
        }

        public override void Delete(PuertaAcceso obj)
        {
            if (obj == null) return;

            obj.Baja = true;
            obj.Vehiculo = null;
            SaveOrUpdate(obj);
        }

        public void GenerarByGeocercas(IEnumerable<ReferenciaGeografica> referencias)
        {
            foreach (var referencia in referencias)
            {
                var puerta = FindByReferenciaGeografica(referencia.Id);
                if (puerta == null)
                {
                    puerta = new PuertaAcceso();
                    puerta.Empresa = referencia.Empresa;
                    puerta.Linea = referencia.Linea;
                    puerta.ReferenciaGeografica = referencia;
                    puerta.Codigo = GetNextCode(referencia.Empresa.Id);
                    puerta.Descripcion = referencia.Descripcion;

                    SaveOrUpdate(puerta);
                }
            }
        }

        public void GenerarByVehiculos(IEnumerable<Coche> vehiculos)
        {
            foreach (var vehiculo in vehiculos)
            {
                var puerta = FindByVehiculo(vehiculo.Id);
                if (puerta == null)
                {
                    puerta = new PuertaAcceso();
                    puerta.Empresa = vehiculo.Empresa;
                    puerta.Linea = vehiculo.Linea;
                    puerta.Vehiculo = vehiculo;
                    puerta.Codigo = GetNextCode(vehiculo.Empresa.Id);
                    puerta.Descripcion = vehiculo.Interno;

                    SaveOrUpdate(puerta);
                }
            }
        }

        private short GetNextCode(int empresaId)
        {
            short lastCode = 0;

            try { lastCode = Query.Where(p => p.Empresa.Id == empresaId && !p.Baja).Max(p => p.Codigo); }
            catch { }

            var nextCode = lastCode++;

            return nextCode;
        }
    }
}
