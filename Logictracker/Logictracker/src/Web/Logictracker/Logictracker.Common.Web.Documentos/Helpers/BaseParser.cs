#region Usings

using System;

#endregion

namespace Logictracker.Web.Documentos.Helpers
{
    public abstract class BaseParser
    {
        protected readonly TipoDocumentoHelper TipoDocumentoHelper;

        protected BaseParser(TipoDocumentoHelper tipoDocHelper)
        {
            TipoDocumentoHelper = tipoDocHelper;
        }

        public event EventHandler<DocumentoLiteralEventArgs> Literal;
        public event EventHandler<DocumentoParametroEventArgs> Codigo;
        public event EventHandler<DocumentoParametroEventArgs> Descripcion;
        public event EventHandler<DocumentoParametroEventArgs> Fecha;
        public event EventHandler<DocumentoParametroEventArgs> Presentacion;
        public event EventHandler<DocumentoParametroEventArgs> Vencimiento;
        public event EventHandler<DocumentoParametroEventArgs> Cierre;
        public event EventHandler<DocumentoParametroEventArgs> Estado;
        public event EventHandler<DocumentoParametroEventArgs> Base;
        public event EventHandler<DocumentoParametroEventArgs> Planta;
        public event EventHandler<DocumentoParametroEventArgs> Parametro;
        public event EventHandler<DocumentoParametroEventArgs> Vehiculo;
        public event EventHandler<DocumentoParametroEventArgs> Empleado;
        public event EventHandler<DocumentoParametroEventArgs> Transportista;
        public event EventHandler<DocumentoParametroEventArgs> Equipo;
       
        /// <summary>
        /// Parsea el template
        /// </summary>
        public abstract void Parse();
        
        protected void OnLiteral(string text)
        {
            if(Literal != null) Literal(this, new DocumentoLiteralEventArgs(text));
        }
        
        protected void OnBase(DocumentoParametroEventArgs e)
        {
            if (Base != null) Base(this, e);
        }
        protected void OnPlanta(DocumentoParametroEventArgs e)
        {
            if(Planta != null) Planta(this, e);
        }
        protected void OnFecha(DocumentoParametroEventArgs e)
        {
            if (Fecha != null) Fecha(this, e);
        }
        protected void OnPresentacion(DocumentoParametroEventArgs e)
        {
            if (Presentacion != null) Presentacion(this, e);
        }
        protected void OnVencimiento(DocumentoParametroEventArgs e)
        {
            if (Vencimiento != null) Vencimiento(this, e);
        }
        protected void OnCierre(DocumentoParametroEventArgs e)
        {
            if (Cierre != null) Cierre(this, e);
        }
        protected void OnCodigo(DocumentoParametroEventArgs e)
        {
            if (Codigo != null) Codigo(this, e);
        }
        protected void OnEstado(DocumentoParametroEventArgs e)
        {
            if (Estado != null) Estado(this, e);
        }
        protected void OnDescripcion(DocumentoParametroEventArgs e)
        {
            if (Descripcion != null) Descripcion(this, e);
        }
        protected void OnParametro(DocumentoParametroEventArgs e)
        {
            if (Parametro != null) Parametro(this, e);
        }
        protected void OnVehiculo(DocumentoParametroEventArgs e)
        {
            if (Vehiculo != null) Vehiculo(this, e);
        }
        protected void OnEmpleado(DocumentoParametroEventArgs e)
        {
            if (Empleado != null) Empleado(this, e);
        }
        protected void OnTransportista(DocumentoParametroEventArgs e)
        {
            if (Transportista != null) Transportista(this, e);
        }
        protected void OnEquipo(DocumentoParametroEventArgs e)
        {
            if (Equipo != null) Equipo(this, e);
        }
    }
}