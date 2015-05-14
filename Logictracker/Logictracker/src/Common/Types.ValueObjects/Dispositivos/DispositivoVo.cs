using System;
using System.Linq;
using Logictracker.Culture;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Types.ValueObjects.Dispositivos
{
    [Serializable]
    public class DispositivoVo
    {
        public const int IndexCodigo = 0;
        public const int IndexTipo = 1;
        public const int IndexLineaTelefonica = 2;
        public const int IndexTelefono = 3;
        public const int IndexImei = 4;
        public const int IndexVehiculo = 5;
        public const int IndexFullFirmwareVersion = 6;
        public const int IndexConfiguracion = 7;
        public const int IndexQtree = 8;
        public const int IndexEmpresa = 9;
        public const int IndexLinea = 10;
        public const int IndexPrecinto = 11;

        public int Id { get { return Dispositivo.Id; } }

        private string _codigo;
        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get { return _codigo ?? (_codigo = Dispositivo.Codigo ?? string.Empty); }
        }

        private string _tipo;
        [GridMapping(Index = IndexTipo, ResourceName = "Entities", VariableName = "PARENTI32", IncludeInSearch = true)]
        public string Tipo
        {
            get
            {
                return _tipo ?? (_tipo = Dispositivo.TipoDispositivo != null
                                             ? string.Concat(Dispositivo.TipoDispositivo.Fabricante, " - ",
                                                             Dispositivo.TipoDispositivo.Modelo)
                                             : string.Empty);
            }
        }

        private string _lineaTelefonica;

        [GridMapping(Index = IndexLineaTelefonica, ResourceName = "Entities", VariableName = "PARENTI74",
            AllowGroup = false, IncludeInSearch = true)]
        public string LineaTelefonica
        {
            get {
                return _lineaTelefonica ??
                       (_lineaTelefonica =
                        Dispositivo.LineaTelefonica != null ? Dispositivo.LineaTelefonica.ToString() : "");
            }
        }

        private string _telefono;
        [GridMapping(Index = IndexTelefono, ResourceName = "Labels", VariableName = "MDN", AllowGroup = false, IncludeInSearch = true)]
        public string Telefono { get { return _telefono ?? (_telefono = Dispositivo.Telefono); } }

        private string _imei;
        [GridMapping(Index = IndexImei, ResourceName = "Labels", VariableName = "IMEI", AllowGroup = false, IncludeInSearch = true)]
        public string Imei { get { return _imei ?? ( _imei = Dispositivo.Imei ?? string.Empty); } }

        private string _vehiculo;
        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = false, IncludeInSearch = true)]
        public string Vehiculo { get
        {
            return _vehiculo ?? (_vehiculo = Coche != null ? (Coche.TipoCoche != null ? string.Concat(Coche.Interno, " - ", Coche.TipoCoche.Descripcion) : Coche.Interno) : string.Empty);
        }}

        private string _fullFirmawareVersion;
        [GridMapping(Index = IndexFullFirmwareVersion, ResourceName = "Labels", VariableName = "FIRMWARE", IncludeInSearch = true)]
        public string FullFirmwareVersion 
        { 
            get
            {
                var dao = new DAOFactory();
                return _fullFirmawareVersion ?? (_fullFirmawareVersion = dao.DetalleDispositivoDAO.GetFullFirmwareVersionValue(Dispositivo) ?? string.Empty);
            }
        }

        private string _configuracion;
        [GridMapping(Index = IndexConfiguracion, ResourceName = "Labels", VariableName = "CONFIGURACION", IncludeInSearch = true)]
        public string Configuracion
        {
            get
            {
                if (_configuracion == null)
                {
                    _configuracion = string.Join(", ", Dispositivo.Configuraciones
                        .Cast<ConfiguracionDispositivo>()
                        .Select(c => c.Nombre)
                        .ToArray());
                    if (_configuracion.Length > 64) _configuracion = Configuracion.Substring(0, 64);
                }
                return _configuracion;
            }
        }

        private string _qtree;
        [GridMapping(Index = IndexQtree, ResourceName = "Labels", VariableName = "QTREE")]
        public string Qtree
        {
            get
            {
                var dao = new DAOFactory();
                return _qtree ?? (_qtree = dao.DetalleDispositivoDAO.GetQtreeRevisionNumberValue(Dispositivo.Id));
            }
        }

        private string _empresa;
        [GridMapping(Index = IndexEmpresa, ResourceName = "Entities", VariableName = "PARENTI01")]
        public string Empresa { get { return _empresa ?? (Dispositivo.Empresa != null ? Dispositivo.Empresa.RazonSocial : Dispositivo.Linea != null ? Dispositivo.Linea.Empresa.RazonSocial : CultureManager.GetControl("DDL_ALL_ITEMS")); } }

        private string _linea;
        [GridMapping(Index = IndexLinea, ResourceName = "Entities", VariableName = "PARENTI02")]
        public string Linea { get { return _linea ?? (_linea = Dispositivo.Linea != null ? Dispositivo.Linea.Descripcion : CultureManager.GetControl("DDL_ALL_ITEMS")); } }

        private string _precinto;
        [GridMapping(Index = IndexPrecinto, ResourceName = "Entities", VariableName = "PARENTI78")]
        public string Precinto { get { return _precinto ?? (_precinto = Dispositivo.Precinto != null ? Dispositivo.Precinto.Codigo : string.Empty); } }

        public int Estado { get { return Dispositivo.Estado; } }

        private Dispositivo Dispositivo;
        private Coche Coche;

        public DispositivoVo(Dispositivo disp, Coche vehiculo)
        {
            Dispositivo = disp;
            Coche = vehiculo;
        }
    }
}