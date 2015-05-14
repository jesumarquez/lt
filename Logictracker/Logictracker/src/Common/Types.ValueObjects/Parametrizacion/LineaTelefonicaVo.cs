using System;
using Logictracker.Culture;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class LineaTelefonicaVo
    {   
        public const int IndexNumeroLinea = 0;
        public const int IndexImei = 1;
        public const int IndexDispositivo = 2;
        public const int IndexEmpresa = 3;
        public const int IndexPlan = 4;

        private string _dispositivo;

        public int Id { get; set; }

        [GridMapping(Index = IndexNumeroLinea, ResourceName = "Labels", VariableName = "NUMERO_LINEA", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string NumeroLinea { get; set; }

        [GridMapping(Index = IndexImei, ResourceName = "Labels", VariableName = "SIM", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string Imei { get; set; }

        [GridMapping(Index = IndexDispositivo, ResourceName = "Entities", VariableName = "PARENTI08", AllowGroup = false, IncludeInSearch = true)]
        public string Dispositivo {
            get
            {
                if (_dispositivo == null)
                {
                    var daoF = new DAOFactory();
                    var disp = daoF.DispositivoDAO.GetByLineaTelefonica(Id);
                    _dispositivo = disp != null && disp.Count > 0 ? disp[0].Codigo : string.Empty;
                }
                return _dispositivo;
            } }

        [GridMapping(Index = IndexEmpresa, ResourceName = "Labels", VariableName = "EMPRESA", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = true)]
        public string Empresa { get; set; }

        [GridMapping(Index = IndexPlan, ResourceName = "Entities", VariableName = "PARENTI73", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = true)]
        public string Plan { get; set; }

        public LineaTelefonicaVo(LineaTelefonica lineaTelefonica)
        {
            Id = lineaTelefonica.Id;
            NumeroLinea = lineaTelefonica.NumeroLinea;
            Imei = lineaTelefonica.Imei;
                       
            var vigencia = lineaTelefonica.GetVigencia(DateTime.UtcNow);
            
            if (vigencia != null)
            {
                Plan = vigencia.Plan.CodigoAbono;
                switch (vigencia.Plan.Empresa)
                {
                    case 1: Empresa = "Claro"; break;
                    case 2: Empresa = "Movistar"; break;
                    case 3: Empresa = "Personal"; break;
                    case 4: Empresa = "Orbcom"; break;
                    default: Empresa = ""; break;
                }
            }
            else
            {
                Plan = CultureManager.GetLabel("NINGUNO");
                Empresa = CultureManager.GetLabel("NINGUNO");
            }
        }
    }
}
