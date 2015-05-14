using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Process.Import.EntityParser
{
    public class VehiculoV1: EntityParserBase
    {
        protected override string EntityName
        {
            get { return "Vehiculo"; }
        }
        public VehiculoV1()
        {
        }

        public VehiculoV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var vehiculo = GetVehiculo(empresa, linea, data);
            if (data.Operation == (int)Operation.Delete) return vehiculo;

            if (vehiculo.Id == 0)
            {
                // valores default
                vehiculo.Poliza = string.Empty;
            }
            vehiculo.Interno = data.AsString(Properties.Vehiculo.Interno, 32);

            var idlinea = data.AsInt32(Properties.Vehiculo.Base);
            if(idlinea.HasValue) linea = idlinea.Value;
            vehiculo.Empresa = empresa > 0 ? DaoFactory.EmpresaDAO.FindById(empresa) : null;
            vehiculo.Linea = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;

            var tipo = data.AsString(Properties.Vehiculo.TipoVehiculo, 10);
            if(tipo != null)
            {
                vehiculo.TipoCoche = DaoFactory.TipoCocheDAO.FindByCodigo(empresa, linea, tipo) ?? vehiculo.TipoCoche;
            }
            if (vehiculo.TipoCoche == null) { ThrowProperty("TipoVehiculo"); }

            vehiculo.Patente = data.AsString(Properties.Vehiculo.Patente, 50) ?? vehiculo.Patente;
            if (vehiculo.Patente == null) { ThrowProperty("Patente"); }

            var cc = data.AsString(Properties.Vehiculo.CentroCostos, 10);
            if(cc != null)
            {
                vehiculo.CentroDeCostos = DaoFactory.CentroDeCostosDAO.FindByCodigo(empresa, linea, cc) ?? vehiculo.CentroDeCostos;
                if (vehiculo.CentroDeCostos == null) { ThrowProperty("CentroCostos"); }
            }
            
            var trans = data.AsString(Properties.Vehiculo.Transportista, 10);
            if(trans != null)
            {
                vehiculo.Transportista = DaoFactory.TransportistaDAO.FindByCodigo(empresa, linea, trans) ?? vehiculo.Transportista;
                if (vehiculo.Transportista == null) { ThrowProperty("Transportista"); }
            }

            vehiculo.Referencia = data.AsString(Properties.Vehiculo.Referencia, 8000) ?? vehiculo.Referencia;

            var resp = data.AsString(Properties.Vehiculo.Responsable, 10);
            if(resp != null)
            {
                vehiculo.Chofer = DaoFactory.EmpleadoDAO.FindByLegajo(empresa, linea, resp) ?? vehiculo.Chofer;
                if (vehiculo.Chofer == null) { ThrowProperty("Responsable"); }
            }
            var dispo = data.AsString(Properties.Vehiculo.Dispositivo, 10);
            if(dispo != null)
            {
                vehiculo.Dispositivo = DaoFactory.DispositivoDAO.GetByCode(dispo) ?? vehiculo.Dispositivo;
                if (vehiculo.Dispositivo == null) { ThrowProperty("Dispositivo"); }
            }

            var estado = data.AsString(Properties.Vehiculo.Estado, 1);
            if(!string.IsNullOrEmpty(estado))
            {
                switch(estado.ToLower())
                {
                    case "o": vehiculo.Estado = Coche.Estados.Activo; break;
                    case "i": vehiculo.Estado = Coche.Estados.Inactivo; break;
                    case "m": vehiculo.Estado = Coche.Estados.EnMantenimiento; break;
                }
            }
            var marca = data.AsString(Properties.Vehiculo.Marca, 64);
            if(marca != null)
            {
                vehiculo.Marca = DaoFactory.MarcaDAO.GetByDescripcion(empresa, linea, marca) ?? vehiculo.Marca;
            }
            if (vehiculo.Marca == null) { ThrowProperty("Marca"); }

            var modelo = data.AsString(Properties.Vehiculo.Modelo, 32);
            if(modelo != null)
            {
                vehiculo.Modelo = DaoFactory.ModeloDAO.FindByCodigo(empresa, linea, modelo) ?? vehiculo.Modelo;
                vehiculo.ModeloDescripcion = vehiculo.Modelo.Descripcion;
            }
            if (vehiculo.Modelo == null) { ThrowProperty("Modelo"); }

            vehiculo.AnioPatente = data.AsInt16(Properties.Vehiculo.Ano) ?? vehiculo.AnioPatente;
            vehiculo.NroChasis = data.AsString(Properties.Vehiculo.Chasis, 64) ?? vehiculo.NroChasis;
            vehiculo.NroMotor = data.AsString(Properties.Vehiculo.Motor, 64) ?? vehiculo.NroMotor;
            vehiculo.Poliza = data.AsString(Properties.Vehiculo.Poliza, 16) ?? vehiculo.Poliza;
            vehiculo.FechaVto = data.AsDateTime(Properties.Vehiculo.VencimientoPoliza) ?? vehiculo.FechaVto;

            return vehiculo;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Coche;
            if (ValidateSaveOrUpdate(item) && ValidateDevice(item)) DaoFactory.CocheDAO.SaveOrUpdate(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Coche;
            if (ValidateDelete(item))
            {
                item.Dispositivo = null;
                DaoFactory.CocheDAO.Delete(item);
            }
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Coche;
            if (ValidateSave(item) && ValidateDevice(item)) DaoFactory.CocheDAO.SaveOrUpdate(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Coche;
            if (ValidateUpdate(item) && ValidateDevice(item)) DaoFactory.CocheDAO.SaveOrUpdate(item);
        }

        #endregion

        public bool ValidateDevice(Coche item)
        {
            if (item.Dispositivo == null) return true;
            var vehiculo = DaoFactory.CocheDAO.FindMobileByDevice(item.Dispositivo.Id);
            if (vehiculo != null && vehiculo.Id != item.Id)
            {
                ThrowMessage("El dispositivo {0} ya está asignado a otro vehiculo", item.Dispositivo.Codigo);
            }
            return true;
        }

        protected virtual Coche GetVehiculo(int empresa, int linea, IData data)
        {
            var codigo = data[Properties.Vehiculo.Interno];
            if(codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.CocheDAO.FindByInterno(new[]{empresa}, new[]{linea}, codigo);
            return sameCode ?? new Coche();
        }
    }

    
}
