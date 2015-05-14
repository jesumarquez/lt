#region Usings

using System;
using Logictracker.Types.BusinessObjects.Documentos;

#endregion

namespace Logictracker.Types.ValueObject
{
    [Serializable]
    public class DocumentoVO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Vencimiento { get; set; }
        public int DiasAlVencimiento { get; set; }
        public string TipoDocumento { get; set; }

        public DocumentoVO(Documento doc)
            :this(doc, DateTime.Now)
        {
            
        }
        public DocumentoVO(Documento doc, DateTime fechaActual)
        {
            Id = doc.Id;
            TipoDocumento = doc.TipoDocumento.Descripcion;
            Codigo = doc.Codigo;
            Descripcion = doc.Descripcion;
            Fecha = doc.Fecha;
            if(doc.Vencimiento.HasValue)
            {
                Vencimiento = doc.Vencimiento.Value.ToString("dd/MM/yyyy");
                DiasAlVencimiento = Convert.ToInt32(doc.Vencimiento.Value.Subtract(fechaActual).TotalDays);
            }
            else
            {
                Vencimiento = string.Empty;
                DiasAlVencimiento = 9999;
            }
        }
    }
}