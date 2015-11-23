#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Web.UI;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Configuration;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

#endregion

namespace Logictracker.Admin
{
    /// <summary>
    /// Page for managing backend application services.
    /// </summary>
    public partial class AdminServices : SecuredGridReportPage<ServiceVo>
    {
        #region Protected Properties

        /// <summary>
        /// The grid to display the results.
        /// </summary>
        public override C1GridView Grid { get { return gridResults; } }

        /// <summary>
        /// The update panel associated to the grid.
        /// </summary>
        protected override UpdatePanel UpdatePanelGrid { get { return updGrid; } }

        /// <summary>
        /// The name of the report.
        /// </summary>
        protected override string VariableName { get { return String.Empty; } }

        /// <summary>
        /// The search button.
        /// </summary>
        protected override ResourceButton BtnSearch { get { return btnActualizar; } }

        /// <summary>
        /// Not found label message.
        /// </summary>
        protected override InfoLabel NotFound { get { return infoLabel1; } }

        /// <summary>
        /// C1 web tool bar.
        /// </summary>
        protected override ToolBar ToolBar { get { return null; } }

        /// <summary>
        /// Info messages label.
        /// </summary>
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        /// <summary>
        /// Auxiliar print grid.
        /// </summary>
        protected override C1GridView GridPrint { get { return new C1GridView(); } }

        /// <summary>
        /// Update panel for the print grid.
        /// </summary>
        protected override UpdatePanel UpdatePanelPrint { get { return new UpdatePanel(); } }

        /// <summary>
        /// Print filters.
        /// </summary>
        protected override Repeater PrintFilters { get { return new Repeater(); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initial services binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            Bind();
        }

        /// <summary>
        /// Custom data binding for each grid row.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="e"></param>
        /// <param name="dataItem"></param>
        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ServiceVo dataItem) { SetBackgroundColor(e); }

        /// <summary>
        /// Sets the back color of the current service.
        /// </summary>
        /// <param name="e"></param>
        private static void SetBackgroundColor(C1GridViewRowEventArgs e)
        {
            var service = e.Row.DataItem as ServiceVo;

            if (service == null) return;

            e.Row.BackColor = service.Status.Equals(CultureManager.GetLabel("SERVICE_STATUS_RUNNING")) ? Color.LightGreen : Color.LightCoral;
        }

        protected override void SelectedIndexChanged()
        {

            var item = GridUtils.GetCell(gridResults.SelectedRow, ServiceVo.IndexName).Text;
            ServiceReact(item);
        }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override String GetRefference() { return "SERVICES_ADMIN"; }

        /// <summary>
        /// Gets the report objects.
        /// </summary>
        /// <returns></returns>
        protected override List<ServiceVo> GetResults()
        {
            return (from service in Config.Services.ServicesToMonitor
                    let winService = new ServiceController(service)
                    select new ServiceVo { Name = service, Description = winService.DisplayName, Status = GetServiceStatus(winService) }).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a description of the current service status.
        /// </summary>
        /// <param name="winService"></param>
        /// <returns></returns>
        private static String GetServiceStatus(ServiceController winService)
        {
            switch (winService.Status)
            {
                case ServiceControllerStatus.ContinuePending: return CultureManager.GetLabel("SERVICE_STATUS_CONTINUE_PENDING");
                case ServiceControllerStatus.Paused: return CultureManager.GetLabel("SERVICE_PAUSED");
                case ServiceControllerStatus.PausePending: return CultureManager.GetLabel("SERVICE_STATUS_PAUSE_PENDING");
                case ServiceControllerStatus.Running: return CultureManager.GetLabel("SERVICE_STATUS_RUNNING");
                case ServiceControllerStatus.StartPending: return CultureManager.GetLabel("SERVICE_STATUS_START_PENDING");
                case ServiceControllerStatus.Stopped: return CultureManager.GetLabel("SERVICE_STATUS_STOPPED");
                case ServiceControllerStatus.StopPending: return CultureManager.GetLabel("SERVICE_STATUS_STOP_PENDING");
                default: return String.Empty;
            }
        }

        /// <summary>
        /// Change service status.
        /// </summary>
        /// <param name="item"></param>
        private void ServiceReact(string item)
        {
            using (var securityHelper = new WindowsSecurityHelper())
            {
                if (!securityHelper.ImpersonateValidUser()) ThrowError("IMPERSONATION_FAILED");

                var service = new ServiceController(item);

                switch (service.Status)
                {
                    case ServiceControllerStatus.Running:
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped);
                        break;
                    case ServiceControllerStatus.StopPending:
                        //Procesos
                        string query = string.Format("SELECT ProcessId FROM Win32_Service WHERE Name='{0}'", service.ServiceName);
                        var searcher = new ManagementObjectSearcher(query);
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            var id = ((int)obj["ProcessId"]);
                            var process = System.Diagnostics.Process.GetProcessById(id);
                            process.Kill();
                        }
                        break;
                    case ServiceControllerStatus.Stopped:
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running);
                        break;
                }

                Bind();

                securityHelper.UndoImpersonation();
            }
        }

        #endregion
    }
}
