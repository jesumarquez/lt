using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using DispatchsExporter.Properties;
using DispatchsExporter.Types.ReportObjects;
using DispatchsExporter.Types.SessionObejcts;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects.ControlDeCombustible.Exportador;

namespace DispatchsExporter.BindingManager
{
    public class BindingManager
    {
        #region Constants

        private const int _VOLUMEN = 0;
        private const int _FECHA = 2;
        private const int _INTERNO = 1;
        private const int _PATENTE = 3;
        private const int _ID = 4;
        private const int _DESCRI_CENTRO = 5;
        private const int _CODIGO_CENTRO = 6;
        private const int _ID_MOBILE = 7;
        private const int _EMPLOYEE_DESCRI = 8;

        #endregion

        #region Private Properties

        private readonly DAOFactory daof = new DAOFactory();

        #endregion

        #region Public Methods

        /// <summary>
        /// Binds the Center by the assigned User.
        /// </summary>
        /// <param name="ddlCentro"></param>
        public void BindCenters(ComboBox ddlCentro)
        {
            ddlCentro.DataSource = daof.LineaDAO.FindList(new[]{-1}, Session.user);
            ddlCentro.Refresh();
        }

        /// <summary>
        /// Binds the grid with the according dispatchs for the Perfil.
        /// </summary>
        /// <param name="grdDespachos"></param>
        /// <param name="center"></param>
        public void BindReassignmentGrid(DataGridView grdDespachos, int center)
        {
            grdDespachos.DataSource = MapDespachos(daof.MovimientoDAO.FindNonExportedDispatchsFromLinea(center));
            grdDespachos.Refresh();
        }

        /// <summary>
        /// Binds the grid with the according dispatchs for the Perfil.
        /// </summary>
        /// <param name="grdDespachos"></param>
        /// <param name="center"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        public void BindDespachosGrid(DataGridView grdDespachos, int center,DateTime desde, DateTime hasta)
        {
            grdDespachos.DataSource = MapDespachos(daof.MovimientoDAO.FindExportedDispatchsFromLinea(center,desde,hasta));
            grdDespachos.Refresh();
        }

        public void BindVehicles(ListBox ddlVehicles, int linea)
        {
            ddlVehicles.DataSource = daof.CocheDAO.FindList(new[] {-1}, new[]{linea}, new []{-1}, Session.user);
            ddlVehicles.Refresh();
        }

        #endregion

        #region Private Methods

        private static IList MapDespachos(List<DispatchsViewVO> reader)
        {
            var dispatchs = new List<GridDespacho>();


            foreach (var dispatch in reader)
            {
                dispatchs.Add(new GridDespacho
                                   {
                                       Fecha = dispatch.Fecha,
                                       Volumen = dispatch.Volumen,
                                       Interno = dispatch.Interno,
                                       Patente = dispatch.Patente,
                                       ID = dispatch.IdMovimiento,
                                       DescriCentroDeCostos = dispatch.CentroDeCostos,
                                       CodigoCentroDeCostos = dispatch.CentroDeCostos,
                                       MobileID = dispatch.IdCoche,
                                       Icon = Resources.moncal,
                                       Operador = dispatch.Empleado
                                   });

            }
            
            return dispatchs;
        }

        #endregion


    }
}
