using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Process.Import.EntityParser
{
    public class TipoVehiculoV1: EntityParserBase
    {
        protected override string EntityName
        {
            get { return "TipoVehiculo"; }
        }
        public TipoVehiculoV1()
        {
        }

        public TipoVehiculoV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var tipoVehiculo = GetTipoVehiculo(empresa, linea, data);
            if (data.Operation == (int)Operation.Delete) return tipoVehiculo;

            if (tipoVehiculo.Id == 0)
            {
                // valores default
                tipoVehiculo.MaximaVelocidadAlcanzable = 200;
            }
            tipoVehiculo.Codigo = data.AsString(Properties.TipoVehiculo.Codigo, 10);
            tipoVehiculo.Descripcion = data.AsString(Properties.TipoVehiculo.Descripcion, 32);

            var idlinea = data.AsInt32(Properties.TipoVehiculo.Base);
            if(idlinea.HasValue) linea = idlinea.Value;
            tipoVehiculo.Empresa = empresa > 0 ? DaoFactory.EmpresaDAO.FindById(empresa) : null;
            tipoVehiculo.Linea = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;

            tipoVehiculo.MaximaVelocidadAlcanzable = data.AsInt32(Properties.TipoVehiculo.VelocidadAlcanzable) ?? tipoVehiculo.MaximaVelocidadAlcanzable;
            tipoVehiculo.KilometrosDiarios = data.AsDouble(Properties.TipoVehiculo.KmDiarios) ?? tipoVehiculo.KilometrosDiarios;
            tipoVehiculo.VelocidadPromedio = data.AsInt32(Properties.TipoVehiculo.VelocidadPromedio) ?? tipoVehiculo.VelocidadPromedio;
            tipoVehiculo.Capacidad = data.AsInt32(Properties.TipoVehiculo.Capacidad) ?? tipoVehiculo.Capacidad;
            tipoVehiculo.DesvioMinimo = data.AsInt32(Properties.TipoVehiculo.DesvioCombustibleMin) ?? tipoVehiculo.DesvioMinimo;
            tipoVehiculo.DesvioMaximo = data.AsInt32(Properties.TipoVehiculo.DesvioCombustibleMax) ?? tipoVehiculo.DesvioMaximo;
            tipoVehiculo.ControlaKilometraje = data.AsBool(Properties.TipoVehiculo.ControlaKilometraje) ?? tipoVehiculo.ControlaKilometraje;
            tipoVehiculo.ControlaTurno = data.AsBool(Properties.TipoVehiculo.ControlaTurno) ?? tipoVehiculo.ControlaTurno;
            tipoVehiculo.AlarmaConsumo = data.AsBool(Properties.TipoVehiculo.ControlaConsumos) ?? tipoVehiculo.AlarmaConsumo;
            tipoVehiculo.SeguimientoPersona = data.AsBool(Properties.TipoVehiculo.SeguimientoPersonas) ?? tipoVehiculo.SeguimientoPersona;
            tipoVehiculo.NoEsVehiculo = data.AsBool(Properties.TipoVehiculo.NoVehiculo) ?? tipoVehiculo.NoEsVehiculo;


            var icono = DaoFactory.IconoDAO.GetList(empresa, linea).FirstOrDefault();
            tipoVehiculo.IconoDefault = icono;
            tipoVehiculo.IconoAtraso = icono;
            tipoVehiculo.IconoAdelanto = icono;
            tipoVehiculo.IconoNormal = icono;
            return tipoVehiculo;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as TipoCoche;
            if (ValidateSaveOrUpdate(item)) DaoFactory.TipoCocheDAO.SaveOrUpdate(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as TipoCoche;
            if (ValidateDelete(item)) DaoFactory.TipoCocheDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as TipoCoche;
            if (ValidateSave(item)) DaoFactory.TipoCocheDAO.SaveOrUpdate(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as TipoCoche;
            if (ValidateUpdate(item)) DaoFactory.TipoCocheDAO.SaveOrUpdate(item);
        }

        #endregion

        protected virtual TipoCoche GetTipoVehiculo(int empresa, int linea, IData data)
        {
            var codigo = data[Properties.TipoVehiculo.Codigo];
            if(codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.TipoCocheDAO.FindByCodigo(empresa, linea, codigo);
            return sameCode ?? new TipoCoche();
        }
    }

    
}
