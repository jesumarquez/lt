using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class VerificadorEmpleadoDAO : ReportDAO
    {
        public VerificadorEmpleadoDAO(DAOFactory daoFactory) : base(daoFactory) {}

        public List<Empleado.VerificadorEmpleado> GetVerificadorEmpleados(IEnumerable<int> empresas, IEnumerable<int> lineas , IEnumerable<int> centrosDeCosto, IEnumerable<int> departamentos, IEnumerable<int> categoriasDeAcceso, string legajo, IEnumerable<int> tiposZonaAcceso, IEnumerable<int> zonasAcceso, IEnumerable<int> puertasAcceso)
        {
            var empleados = DAOFactory.EmpleadoDAO.GetList(empresas,
                                                           lineas,
                                                           new[] {-1},
                                                           new[] {-1},
                                                           centrosDeCosto,
                                                           departamentos,
                                                           categoriasDeAcceso,
                                                           legajo)
                                                  .Select(emp => DAOFactory.EmpleadoDAO.GetLastLog(emp));
            
            if (!QueryExtensions.IncludesAll(tiposZonaAcceso))
                empleados = empleados.Where(emp => emp.ZonaAcceso != null && tiposZonaAcceso.Contains(emp.ZonaAcceso.TipoZonaAcceso.Id));
            if (!QueryExtensions.IncludesAll(zonasAcceso))
                empleados = empleados.Where(emp => emp.ZonaAcceso != null && zonasAcceso.Contains(emp.ZonaAcceso.Id));
            if (!QueryExtensions.IncludesAll(puertasAcceso))
                empleados = empleados.Where(emp => emp.PuertaAcceso != null && puertasAcceso.Contains(emp.PuertaAcceso.Id));

            return empleados.ToList();
        }
    }
}
