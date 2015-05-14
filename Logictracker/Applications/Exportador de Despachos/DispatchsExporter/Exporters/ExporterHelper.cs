
using Logictracker.DAL.Factories;

namespace DispatchsExporter.Exporters
{
    public class ExporterHelper
    {
        //En un futuro hacer que este Exporter genere el archivo en vez de llamar a un Store Procedure que lo haga.
        //private const string _VOLUME_UNIT = "LT";
        //private const string _GASOIL_CODE = "01";

        private readonly DAOFactory daof = new DAOFactory();
        private readonly DataTransferDAO dao = new DataTransferDAO();

        public void ExportToJDEdwards(int centerId)
        {
            var l = daof.LineaDAO.FindById(centerId);

            if(l == null) return;

            var code = l.DescripcionCorta;

            dao.GenerateJDEdwardsFileForCenter(code);
        }

        public void ExportDispatchsToLogictracker(int centerId) { dao.ExportDespachosToLogictracker(centerId); }

        public void ExportRemitosToLogictracker(int centerId) { dao.ImportRemitosFromCenter(centerId); }

        public void ExportNivelesToLogictracker(int centerId) { dao.ImportNivelesFromCenter(centerId); }
    }


    //public void ExportList(List<GridDespacho> list,int centerId)
    //{
    //    var code = daof.LineaDAO.FindById(centerId).DescripcionCorta;

    //    foreach(CentroDeCostos c in daof.CentroDeCostosDAO.FindAll())
    //    {
    //        var centerSequenceNumber = D.getCenterSequenceNumber(code);

    //    }

    //var FileName = String.Format("C:\\Archivos\\{0}{1:ddMMyyyy}.txt",code,DateTime.Now);
    //var initLine = String.Format("{0};{1:dd/MM/yyyy};{2:hh:mm:ss};{3}",
    //                            code, DateTime.Now, DateTime.Now,centerSequenceNumber);
    //    using (var file = File.AppendText(FileName))
    //    {
    //        file.WriteLine(initLine);
    //        foreach (var d in list)
    //        {
    //            dao.UpdateDispatch(d.ID, d.MobileID);
    //            file.WriteLine(FormatEntry(d));
    //        }
    //    }

    //    dao.ConfirmExport(code,centerSequenceNumber);


        //private static string FormatEntry(GridDespacho d)
        //{
        //    return String.Format("{0};{1};{2};{3};{4};{5};{6};{7:dd/MM/yyyy-hh:mm:ss}",
        //                    d.CodigoCentroDeCostos,d.DescriCentroDeCostos,_GASOIL_CODE,
        //                    d.Patente.Trim().Replace("-","").Replace(" ",""),d.Interno,_VOLUME_UNIT,d.Volumen,d.Fecha);
        //}
}
