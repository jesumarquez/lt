using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Telemetria
{
    public partial class Mediciones : OnLineSecuredPage
    {
        protected override InfoLabel LblInfo { get { return null; } }
        protected override string GetRefference() { return "REP_MEDICIONES"; }
    }
}
