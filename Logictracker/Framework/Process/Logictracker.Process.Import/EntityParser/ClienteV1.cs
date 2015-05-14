using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Process.Import.EntityParser
{
    public class ClienteV1: EntityParserBase
    {
        protected override string EntityName { get { return "Cliente"; } }
        
        public ClienteV1() { }

        public ClienteV1(DAOFactory daoFactory) : base(daoFactory) { }
        
        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var cliente = GetCliente(empresa, linea, data);
            if (data.Operation == (int)Operation.Delete) return cliente;

            if (cliente.Id == 0)
            {
                cliente.Empresa = DaoFactory.EmpresaDAO.FindById(empresa);
                cliente.Linea = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;
                cliente.Baja = false;
                cliente.Nomenclado = false;
                cliente.DireccionNomenclada = string.Empty;
            }

            cliente.Codigo = (data[Properties.Cliente.Codigo] ?? string.Empty).Truncate(32);
            cliente.Comentario1 = (data[Properties.Cliente.Comentario1] ?? string.Empty).Truncate(255);
            cliente.Comentario2 = (data[Properties.Cliente.Comentario2] ?? string.Empty).Truncate(255);
            cliente.Comentario3 = (data[Properties.Cliente.Comentario3] ?? string.Empty).Truncate(255);
            cliente.Descripcion = (data[Properties.Cliente.Descripcion] ?? string.Empty).Truncate(128);
            cliente.DescripcionCorta = (data[Properties.Cliente.DescripcionCorta] ?? string.Empty).Truncate(17);
            cliente.Linea = GetLinea(empresa, (data[Properties.Cliente.Linea]??string.Empty)) ?? cliente.Linea;
            cliente.Telefono = (data[Properties.Cliente.Telefono] ?? string.Empty).Truncate(32);
            cliente.ReferenciaGeografica = DaoFactory.ReferenciaGeograficaDAO.FindByCodigo(new[] { empresa }, new[] { linea }, new[] { -1 }, data[Properties.Cliente.ReferenciaGeografica]);

            cliente.Nomenclado = cliente.ReferenciaGeografica != null && cliente.ReferenciaGeografica.Direccion != null;

            if (cliente.Nomenclado)
                cliente.DireccionNomenclada = cliente.ReferenciaGeografica.Observaciones.Truncate(255);
            
            return cliente;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Cliente;
            if(ValidateSaveOrUpdate(item)) DaoFactory.ClienteDAO.SaveOrUpdate(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Cliente;
            if(ValidateDelete(item)) DaoFactory.ClienteDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Cliente;
            if(ValidateSave(item)) DaoFactory.ClienteDAO.SaveOrUpdate(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Cliente;
            if(ValidateUpdate(item)) DaoFactory.ClienteDAO.SaveOrUpdate(item);
        }

        #endregion

        protected virtual Cliente GetCliente(int empresa, int linea, IData data)
        {
            var codigo = data[Properties.Cliente.Codigo];
            if(codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.ClienteDAO.FindByCode(new[]{empresa}, new[]{linea}, codigo);
            return sameCode ?? new Cliente();
        }
    }
}
