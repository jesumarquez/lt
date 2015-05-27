using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Vehiculos
{
    public class OdometroDAO : GenericDAO<Odometro>
    {
        #region Constructor

//        public OdometroDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        public IList FindByEmpresaLineaYUsuario(Empresa empresa, Linea linea,Usuario user)
        {
            var lin = linea != null ? linea.Id : -1;
            var emp = empresa != null ? empresa.Id : linea != null ? linea.Empresa.Id : -1;

            return FindByEmpresaLineaYUsuario(emp, lin,user);
        }

        public IList FindByEmpresaYLineaYTipo(Empresa empresa, Linea linea, List<int> tiposMovil, Usuario user)
        {
            var odometros = FindByEmpresaLineaYUsuario(empresa, linea, user);

            if (tiposMovil.Contains(-1) || tiposMovil.Contains(0)) return odometros;

            var results = new List<Odometro>();

            foreach (var odometro in 
                from Odometro odometro in odometros
                from tipoCoche in odometro.TiposDeVehiculo.Cast<TipoCoche>().Where(tipoCoche => tiposMovil.Contains(tipoCoche.Id) && !results.Contains(odometro)) select odometro)
                results.Add(odometro);

            return results;
        }

        protected override void DeleteWithoutTransaction(Odometro odometro)
        {
            if (odometro != null)
            {
                odometro.ClearVehicleTypes();

                RemoveObsoleteOdometers(odometro);

                base.Delete(odometro);
            }
        }

        public void SaveOrUpdate(Odometro obj, IEnumerable<TipoCoche> tiposVehiculo, Usuario user)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    SaveOrUpdateWithoutTransaction(obj, tiposVehiculo, user);
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof(OdometroDAO).FullName, ex, "Exception in SaveOrUpdate(Odometro, IEnumerable<TipoCoche>, Usuario) -> transaction.Commit();");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof(OdometroDAO).FullName, ex, "Exception in SaveOrUpdate(Odometro, IEnumerable<TipoCoche>, Usuario)");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(typeof(OdometroDAO).FullName, ex2, "Exception in SaveOrUpdate(Odometro, IEnumerable<TipoCoche>, Usuario) -> transaction.Rollback();");
                    }
                    throw ex;
                }
            }           
        }

        protected void SaveOrUpdateWithoutTransaction(Odometro obj, IEnumerable<TipoCoche> tiposVehiculo, Usuario user)
        {
            var cocheDao = new CocheDAO();

            if (obj.Id > 0) RemoveObsoleteOdometers(obj);

            SaveOrUpdate(obj);

            GenerateNewOdometers(obj, obj.TiposDeVehiculo.OfType<TipoCoche>(), cocheDao, user);
        }

        public void SaveOrUpdate(Odometro obj, IEnumerable<int> vehiculos, Usuario user)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {

                    SaveOrUpdateWithoutTransaction(obj, vehiculos, user);
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof (OdometroDAO).FullName, ex,
                            "Exception in SaveOrUpdate(Odometro, IEnumerable<int>, Usuario) -> transaction.Commit();");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof (OdometroDAO).FullName, ex, "Exception in SaveOrUpdate(Odometro, IEnumerable<int>, Usuario)");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(typeof (OdometroDAO).FullName, ex2,
                            "Exception in SaveOrUpdate(Odometro, IEnumerable<int>, Usuario) -> transaction.Rollback();");
                    }
                    throw ex;
                }
            }
        }

        public void SaveOrUpdateWithoutTransaction(Odometro obj, IEnumerable<int> vehiculos, Usuario user)
        {
            var cocheDao = new CocheDAO();

            SaveOrUpdate(obj);

            foreach (var id in vehiculos)
            {
                if (id == 0) break;
                var vehiculo = cocheDao.FindById(id);
                var odometro = new MovOdometroVehiculo {Vehiculo = vehiculo, Odometro = obj};

                if (!vehiculo.Odometros.Contains(odometro))
                {
                    vehiculo.AddOdometro(odometro);
                    cocheDao.SaveOrUpdate(vehiculo);
                }
            }

        }

        #endregion

        #region Private Methods

        private IList FindByEmpresaLineaYUsuario(int emp, int lin,Usuario user)
        {
            var list = (from Odometro o in Session.Query<Odometro>().Cacheable().ToList()
                        where (emp <= 0 || o.Empresa == null || o.Empresa.Id == emp) && (lin <= 0 || o.Linea == null || o.Linea.Id == lin)
                        orderby o.Descripcion ascending select o).ToList();
                            
            return (from Odometro o in list
                    where user == null
                            || (o.Empresa == null && o.Linea == null)
                            || (o.Empresa != null && o.Linea == null && (user.Empresas.IsEmpty()|| user.Empresas.Contains(o.Empresa)))
                            || (o.Linea != null && (user.Lineas.IsEmpty()|| user.Lineas.Contains(o.Linea)))
                    select o).ToList();
        }

        private void RemoveObsoleteOdometers(Odometro odometro)
        {
            var movOdometroDao = new MovOdometroVehiculoDAO();
            var odometros = movOdometroDao.GetByOdometro(odometro.Id);
            var cocheDao = new CocheDAO();
            foreach (var ov in odometros)
            {
                var vehiculo = ov.Vehiculo;
                vehiculo.RemoveOdometro(ov);
                cocheDao.SaveOrUpdate(vehiculo);
            }
        }

        private static void GenerateNewOdometers(Odometro obj, IEnumerable<TipoCoche> tiposVehiculo, CocheDAO cocheDAO, Usuario user)
        {
            var empresas = new List<int> {obj.Empresa != null ? obj.Empresa.Id : 0};
            var lineas = new List<int> {obj.Linea != null ? obj.Linea.Id : 0};

            var typesToRemove = tiposVehiculo.Select(t=>t.Id).ToList();

            foreach (var vehiculo in cocheDAO.FindList(empresas, lineas, typesToRemove, user))
            {
                var odometro = new MovOdometroVehiculo {Vehiculo = vehiculo, Odometro = obj};

                if (!vehiculo.Odometros.Contains(odometro))
                {
                    vehiculo.AddOdometro(odometro);
                    cocheDAO.SaveOrUpdate(vehiculo);
                }
            }
        }

        #endregion
    }
}