using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Iesi.Collections;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Documentos
{
    [Serializable]
    public class Documento : IAuditable, ISecurable, IHasVehiculo, IHasEmpleado, IHasTransportista
    {
        public static class Estados
        {
            public const short Eliminado = -1;
            public const short Abierto = 0;
            public const short Cerrado = 9;
        }

        private ISet<DocumentoValor> _parametros;
        private Dictionary<string, object> _valores;

        public Documento()
        {
            _parametros = new HashSet<DocumentoValor>();
        }

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion


        #region ISecurable

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        #endregion

        public virtual string Descripcion { get; set; }

        public virtual DateTime Fecha { get; set; }

        public virtual DateTime FechaAlta { get; set; }

        public virtual DateTime? Presentacion { get; set; }

        public virtual DateTime? Vencimiento { get; set; }

        public virtual string Codigo { get; set; }

        public virtual TipoDocumento TipoDocumento { get; set; }

        public virtual DateTime? FechaModificacion { get; set; }

        public virtual DateTime? FechaCierre { get; set; }

        public virtual Usuario UsuarioModificacion { get; set; }

        public virtual Usuario Usuario { get; set; }
        public virtual short Estado { get; set; }

        public virtual Coche Vehiculo { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Transportista Transportista { get; set; }
        public virtual Equipo Equipo { get; set; }

        public virtual bool EnviadoAviso1 { get; set; }
        public virtual bool EnviadoAviso2 { get; set; }
        public virtual bool EnviadoAviso3 { get; set; }

        public virtual ISet<DocumentoValor> Parametros
        {
            get { return _parametros; }
            set { _parametros = value; }
        }
        public virtual IDictionary<string, object> Valores
        {
            get
            {
                if(_valores == null)
                {
                    _valores = new Dictionary<string, object>();

                    var unicos = from DocumentoValor valor in Parametros
                                 where valor.Parametro.Repeticion == 1
                                 select valor;
                    var repetidos = from DocumentoValor valor in Parametros
                                    where valor.Parametro.Repeticion > 1
                                    orderby valor.Parametro.Orden, valor.Repeticion
                                    select valor;

                    foreach (var unico in unicos)
                        _valores.Add(unico.Parametro.Nombre, GetObject(unico));

                    foreach (var repetido in repetidos)
                    {
                        if (!_valores.ContainsKey(repetido.Parametro.Nombre))
                            _valores.Add(repetido.Parametro.Nombre, new List<object>());

                        (_valores[repetido.Parametro.Nombre] as List<object>).Add(GetObject(repetido));
                    }
                }
                return _valores;
            }
        }

        public virtual void ResetDictionary() { _valores = null; }

        private object GetObject(DocumentoValor v)
        {
            switch(v.Parametro.TipoDato)
            {
                case TipoParametroDocumento.Integer:
                    return Convert.ToInt32(v.Valor);
                case TipoParametroDocumento.Float:
                    return Convert.ToDouble(v.Valor, CultureInfo.InvariantCulture);
                case TipoParametroDocumento.String:
                    return v.Valor;
                case TipoParametroDocumento.DateTime:
                    return Convert.ToDateTime(v.Valor, CultureInfo.InvariantCulture);
                case TipoParametroDocumento.Boolean:
                    return v.Valor == "true";
                case TipoParametroDocumento.Coche:
                    return Convert.ToInt32(v.Valor);
                case TipoParametroDocumento.Chofer:
                    return Convert.ToInt32(v.Valor);
                case TipoParametroDocumento.Aseguradora:
                    return Convert.ToInt32(v.Valor);
                default:
                    return v.Valor;
            }
        }
    }
}
