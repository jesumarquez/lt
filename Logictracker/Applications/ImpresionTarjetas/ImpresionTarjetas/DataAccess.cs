using System;
using System.Linq;
using Tarjetas.Properties;

namespace Tarjetas
{
    public class DataAccess
    {
        private readonly TarjetasDataContext dc;
        public TarjetasDataContext DataContext { get { return dc; } }
        public DataAccess()
        {
            dc = new TarjetasDataContext(Settings.Default.tarjetasConnectionString);
        }

        public IQueryable<empleado> GetAllEmpleados()
        {
            return from e in dc.empleados orderby e.legajo select e;
        }
        public IQueryable<empleado> GetAllEmpleados(empresa empresa)
        {
            if (empresa == null)
            {
                return from e in dc.empleados orderby e.legajo select e;
            }
            else
            {
                return from e in dc.empleados where e.empresa == empresa.id orderby e.legajo select e;
            }
        }
        public empleado GetEmpleadoByLegajo(int empresa, string legajo)
        {
            var emp = (from c in GetAllEmpleados() where c.empresa == empresa &&  c.legajo == legajo select c).ToList();
            return emp.Any() ? emp[0] : null;
        }
        public IQueryable<empresa> GetAllEmpresas()
        {
            return from u in dc.empresas orderby u.nombre select u;
        }
        public IQueryable<upcode> GetAllUpcodes()
        {
            return from u in dc.upcodes orderby u.code select u;
        }
        public upcode GetUpcodeByCode(string code)
        {
            var upc = (from u in GetAllUpcodes() where u.code == code select u).ToList();
            return upc.Any() ? upc[0] : null;
        }

        public upcode GetNextUpcode()
        {
            return (from u in GetAllUpcodes() where u.used == false orderby u.code select u).FirstOrDefault();
        }
        public IQueryable<back> GetAllBacks()
        {
            return from e in dc.backs orderby e.nombre select e;
        }
        public back GetBackByNombre(string nombre)
        {
            var upc = (from u in GetAllBacks() where u.nombre == nombre select u).ToList();
            return upc.Count() > 0 ? upc[0] : null;
        }
        public back GetActiveBack()
        {
			return GetAllBacks().Where(b => b.active.HasValue && b.active.Value).FirstOrDefault();
        }
        public void AddUpcode(upcode code)
        {
            dc.upcodes.InsertOnSubmit(code);
        }
        public void AddBack(back b)
        {
            dc.backs.InsertOnSubmit(b);
        }
        public void AddEmpleado(empleado emp)
        {
            if (emp.upcode != null) GetUpcodeByCode(emp.upcode).used = true;
            emp.alta = DateTime.Now;
            dc.empleados.InsertOnSubmit(emp);
        }
        public void AddChofer(chofer cho)
        {
            dc.chofers.InsertOnSubmit(cho);
        }
        public void AddEmpresa(empresa e)
        {
            dc.empresas.InsertOnSubmit(e);
        }
        public void ClearChoferes()
        {
            dc.chofers.DeleteAllOnSubmit(from c in dc.chofers select c);
        }
        public void AddImprimir(imprimir cho)
        {
            dc.imprimirs.InsertOnSubmit(cho);
        }
        public void ClearImprimir()
        {
            dc.imprimirs.DeleteAllOnSubmit(from c in dc.imprimirs select c);
        }
        public void DeleteEmpresa(empresa e)
        {
            dc.empresas.DeleteOnSubmit(e);
        }
        public void SubmitChanges()
        {
            dc.SubmitChanges();
        }
    }
}
