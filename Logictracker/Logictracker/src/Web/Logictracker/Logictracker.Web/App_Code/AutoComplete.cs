using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using Logictracker.Web.BaseClasses.BasePages;
using System.Web.Script.Services;
using AjaxControlToolkit;

[WebService(Namespace = "http://plataforma.logictracker.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class AutoComplete : BaseWebService
{
    public AutoComplete()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string[] GetVehiculos(string prefixText, int count, string contextKey)
    {
        prefixText = prefixText.ToLower();
        var parents = ParseContextKey(contextKey);
        var empresas = GetParentElement(parents, 0);
        var lineas = GetParentElement(parents, 1);
        var tiposVehiculo = GetParentElement(parents, 2);
        var transportistas = GetParentElement(parents, 3);
        var departamentos = GetParentElement(parents, 4);
        var centrosDeCosto = GetParentElement(parents, 5);
        var subCentrosDeCosto = GetParentElement(parents, 6);

        return DAOFactory.CocheDAO.GetList(empresas, lineas, tiposVehiculo, transportistas, departamentos, centrosDeCosto, subCentrosDeCosto, true)
                                  .Where(c => c.CompleteDescripcion().ToLower().Contains(prefixText))
                                  .OrderBy(c => c.CompleteDescripcion())
                                  .Take(count)
                                  .Select(c => AutoCompleteExtender.CreateAutoCompleteItem(c.CompleteDescripcion(), c.Id.ToString("#0")))
                                  .ToArray();
    }
    [WebMethod]
    public string[] GetEmpleados(string prefixText, int count, string contextKey)
    {
        prefixText = prefixText.ToLower();
        var parents = ParseContextKey(contextKey);
        var empresas = GetParentElement(parents, 0);
        var lineas = GetParentElement(parents, 1);
        var tiposEmpleado = GetParentElement(parents, 2);
        var transportistas = GetParentElement(parents, 3);
        var centrosCosto = GetParentElement(parents, 4);
        var departamentos = GetParentElement(parents, 5);

        return DAOFactory.EmpleadoDAO.GetList(empresas, lineas, tiposEmpleado, transportistas, centrosCosto, departamentos)
            .Where(e => e.Entidad.Descripcion.ToLower().Contains(prefixText))
            .OrderBy(e=>e.Entidad.Descripcion)
            .Take(count)
            .Select(e=>AutoCompleteExtender.CreateAutoCompleteItem(e.Entidad.Descripcion, e.Id.ToString("#0")))
            .ToArray();
    }
    [WebMethod]
    public string[] GetPuntosEntrega(string prefixText, int count, string contextKey)
    {
        prefixText = prefixText.ToLower();
        var parents = ParseContextKey(contextKey);
        var empresas = GetParentElement(parents, 0);
        var lineas = GetParentElement(parents, 1);
        var clientes = GetParentElement(parents, 2);
    
        return DAOFactory.PuntoEntregaDAO.GetList(empresas, lineas, clientes, prefixText)
            .OrderBy(p => p.Descripcion)
            .Take(count)
            .Select(p => AutoCompleteExtender.CreateAutoCompleteItem(p.Descripcion, p.Id.ToString("#0")))
            .ToArray();
    }
    [WebMethod]
    public string[] GetRutas(string prefixText, int count, string contextKey)
    {
        prefixText = prefixText.ToLower();
        var parents = ParseContextKey(contextKey);
        var empresas = GetParentElement(parents, 0);
        var lineas = GetParentElement(parents, 1);
        var vehiculos = GetParentElement(parents, 2);
        var empleados = GetParentElement(parents, 3);

        return DAOFactory.ViajeDistribucionDAO.GetList(empresas, lineas, vehiculos, empleados)
            .Where(v => v.Codigo.ToLower().Contains(prefixText))
            .OrderBy(v => v.Codigo)
            .Take(count)
            .Select(v => AutoCompleteExtender.CreateAutoCompleteItem(v.Codigo, v.Id.ToString("#0")))
            .ToArray();
    }

    private static int[][] ParseContextKey(string contextKey)
    {
        return string.IsNullOrEmpty(contextKey) 
                    ? new int[0][] 
                    : contextKey.Split('|').Select(p => p.Split(',').Select(i => Convert.ToInt32(i)).ToArray()).ToArray();
    }

    private static IEnumerable<int> GetParentElement(IList<int[]> parents, int index)
    {
        return parents.Count > index ? parents[index] : new[] { -1 };
    }
}
