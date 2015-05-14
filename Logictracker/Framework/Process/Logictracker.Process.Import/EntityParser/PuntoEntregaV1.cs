using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Process.Import.EntityParser
{
    public class PuntoEntregaV1: EntityParserBase
    {
        protected override string EntityName { get { return "Punto de Entrega"; } }
        
        public PuntoEntregaV1() { }

        public PuntoEntregaV1(DAOFactory daoFactory) : base(daoFactory) { }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var lineaU = data[Properties.PuntoEntrega.Linea];
            if (lineaU != null)
            {
                var lineaI = lineaU.AsInt();
                if(lineaI.HasValue) linea = lineaI.Value;
            }
            var cliente = GetCliente(empresa, linea, data);
            var punto = GetPuntoEntrega(empresa, linea, cliente.Id, data);
            if (data.Operation == (int)Operation.Delete) return punto;

            if (punto.Id == 0)
            {
                punto.Cliente = cliente;
                punto.Baja = false;
                punto.Nomenclado = false;
                punto.DireccionNomenclada = string.Empty;
            }

            punto.Codigo = (data[Properties.PuntoEntrega.Codigo] ?? string.Empty).Truncate(32);
            punto.Comentario1 = (data[Properties.PuntoEntrega.Comentario1] ?? string.Empty).Truncate(255);
            punto.Comentario2 = (data[Properties.PuntoEntrega.Comentario2] ?? string.Empty).Truncate(255);
            punto.Comentario3 = (data[Properties.PuntoEntrega.Comentario3] ?? string.Empty).Truncate(255);
            punto.Descripcion = (data[Properties.PuntoEntrega.Descripcion] ?? string.Empty).Truncate(128);
            punto.Telefono = (data[Properties.PuntoEntrega.Telefono] ?? string.Empty).Truncate(32);
            punto.ReferenciaGeografica = DaoFactory.ReferenciaGeograficaDAO.FindByCodigo(
                                                    new[] { empresa },
                                                    new[] { linea },
                                                    new[] { -1 },
                                                    data[Properties.PuntoEntrega.ReferenciaGeografica]);

            punto.Nomenclado = punto.ReferenciaGeografica != null && punto.ReferenciaGeografica.Direccion != null;

            if (punto.Nomenclado)
            {
                punto.DireccionNomenclada = punto.ReferenciaGeografica.Observaciones.Truncate(255);
            }

            return punto;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as PuntoEntrega;
            if (ValidateSaveOrUpdate(item)) DaoFactory.PuntoEntregaDAO.SaveOrUpdate(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as PuntoEntrega;
            if (ValidateDelete(item)) DaoFactory.PuntoEntregaDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as PuntoEntrega;
            if (ValidateSave(item)) DaoFactory.PuntoEntregaDAO.SaveOrUpdate(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as PuntoEntrega;
            if (ValidateUpdate(item)) DaoFactory.PuntoEntregaDAO.SaveOrUpdate(item);
        }

        #endregion

        protected virtual PuntoEntrega GetPuntoEntrega(int empresa, int linea, int cliente, IData data)
        {
            var codigo = data[Properties.PuntoEntrega.Codigo];           
            if (codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.PuntoEntregaDAO.FindByCode(new[] { empresa }, new[] { linea }, new[] { cliente }, codigo);
            return sameCode ?? new PuntoEntrega();
        }
        protected virtual Cliente GetCliente(int empresa, int linea, IData data)
        {
            var codigo = data[Properties.PuntoEntrega.Cliente];
            if (codigo == null) throw new EntityParserException("No se encuentra el campo 'Cliente' para el punto de entrega a importar");

            var client = DaoFactory.ClienteDAO.FindByCode(new[] { empresa }, new[] { linea }, codigo);
            if (client == null) throw new EntityParserException("No se encuentra el Cliente " + codigo + " para el punto de entrega a importar");

            return client;
        }
    }

}
