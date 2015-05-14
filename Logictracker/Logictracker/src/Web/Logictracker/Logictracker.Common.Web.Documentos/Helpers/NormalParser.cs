#region Usings

using System;
using System.Collections.Generic;
using Logictracker.Types.BusinessObjects.Documentos;

#endregion

namespace Logictracker.Web.Documentos.Helpers
{
    internal class NormalParser: BaseParser
    {
        public NormalParser(TipoDocumentoHelper tipoDocHelper) : base(tipoDocHelper)
        {
        }

        /// <summary>
        /// Parsea el documento
        /// </summary>
        public override void Parse()
        {
            OnLiteral(string.Concat("<table style='padding: 10px; width:80%; border: solid 1px #DDDDDD;height: auto;' class='Grid_Header' align='center'><tr><td><div style='font-size: 20px; font-weight: bold; text-align: center;'>", TipoDocumentoHelper.TipoDocumento.Nombre));
            OnLiteral(string.Concat("<div style='font-size: 12px; text-align: center;'>", TipoDocumentoHelper.TipoDocumento.Descripcion, "</div></div></td></tr></table>"));
            OnLiteral("<br/><table style='padding: 10px; width:80%; border: solid 1px #DDDDDD;background-color: #E7E7E7;' align='center'>");

            OnLiteral("<tr><td>Distrito<td><td>");
            OnBase(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_PARENTI01, string.Empty));
            OnLiteral("</td><td>");
            if (TipoDocumentoHelper.TipoDocumento.AplicarATransportista)
                OnLiteral("Transportista");
            OnLiteral("</td><td>");
            if (TipoDocumentoHelper.TipoDocumento.AplicarATransportista)
                OnTransportista(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_PARENTI07, string.Empty));
            OnLiteral("</td></tr>");

            OnLiteral("<tr><td>Base<td><td>");
            OnPlanta(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_PARENTI02, string.Empty));
            OnLiteral("</td><td>");
            if (TipoDocumentoHelper.TipoDocumento.AplicarAVehiculo)
                OnLiteral("Vehículo");
            OnLiteral("</td><td>");
            if (TipoDocumentoHelper.TipoDocumento.AplicarAVehiculo)
                OnVehiculo(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_PARENTI03, string.Empty));
            OnLiteral("</td></tr>");

            OnLiteral("<tr><td>Código<td><td>");
            OnCodigo(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_CODIGO, string.Empty));
            OnLiteral("</td><td>");
            if (TipoDocumentoHelper.TipoDocumento.AplicarAEmpleado)
                OnLiteral("Empleado");
            OnLiteral("</td><td>");
            if (TipoDocumentoHelper.TipoDocumento.AplicarAEmpleado)
                OnEmpleado(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_PARENTI09, string.Empty));
            OnLiteral("</td></tr>");

            OnLiteral("<tr><td>Descripción<td><td>");
            OnDescripcion(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_DESCRIPCION, string.Empty));
            OnLiteral("</td><td>");
            if (TipoDocumentoHelper.TipoDocumento.AplicarAEquipo)
                OnLiteral("Equipo");
            OnLiteral("</td><td>");
            if (TipoDocumentoHelper.TipoDocumento.AplicarAEquipo)
                OnEquipo(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_PARENTI19, string.Empty));
            OnLiteral("</td></tr>");

            OnLiteral("<tr><td>Fecha<td><td>");
            OnFecha(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_FECHA, string.Empty));
            OnLiteral("</td><td></td><td></td></tr>");

            OnLiteral("<tr><td>Presentación<td><td>");
            OnPresentacion(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_PRESENTACION, string.Empty));
            OnLiteral("</td><td></td><td></td></tr>");

            OnLiteral("<tr><td>Vencimiento<td><td>");
            OnVencimiento(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_VENCIMIENTO, string.Empty));
            OnLiteral("</td><td></td><td></td></tr>");

            OnLiteral("<tr><td>Cierre<td><td>");
            OnCierre(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_CIERRE, string.Empty));
            OnLiteral("</td><td></td><td></td></tr>");

            OnLiteral("<tr><td>Estado<td><td>");
            OnEstado(new DocumentoParametroEventArgs(TipoDocumentoHelper.CONTROL_NAME_ESTADO, string.Empty));
            OnLiteral("</td><td></td><td></td></tr>");

            OnLiteral("</table>");
            OnLiteral("<br />");

            var parametros = TipoDocumentoHelper.GetParametros();

            OnLiteral("<table style='padding: 10px; width:80%;border: solid 1px #DDDDDD;background-color: #EEEEEE;' align='center'>");

            var repetidos = new List<TipoDocumentoParametro>();
            var lastOrder = -1;
            var repeating = false;
            foreach (var parametro in parametros)
            {
                var order = (int)Math.Floor(parametro.Orden);

                if (repeating && lastOrder != order)
                {
                    OnLiteral("<tr><td colspan='2'><br/><table border='1' align='center' cellspacing='0' cellpadding='2' style='text-align: center;'>");

                    int repeatTimes = repetidos[0].Repeticion;
                    for (short i = -1; i < repeatTimes; i++)
                    {
                        OnLiteral("<tr>");
                        foreach (var repetido in repetidos)
                        {
                            OnLiteral("<td>");
                            if (i == -1)
                                OnLiteral(repetido.Nombre);
                            else
                            {
                                var id = TipoDocumentoHelper.GetControlName(repetido, i);
                                OnParametro(new DocumentoParametroEventArgs(id, "width: 100px", repetido));
                            }
                            OnLiteral("</td>");
                        }
                        OnLiteral("</tr>");
                    }
                    OnLiteral("</table><br/></td></tr>");
                    repetidos.Clear();
                }

                if (parametro.Repeticion == 0 || parametro.Repeticion == 1)
                {
                    var id = TipoDocumentoHelper.GetControlName(parametro);
                    OnLiteral(string.Concat("<tr><td>", parametro.Nombre, "</td><td>"));
                    OnParametro(new DocumentoParametroEventArgs(id, string.Empty, parametro));
                    OnLiteral("</td></tr>");
                }
                else repetidos.Add(parametro);

                lastOrder = order;
                repeating = parametro.Repeticion != 1;
            }

            if (repeating)
            {
                OnLiteral("<tr><td colspan='2'><table>");

                int repeatTimes = repetidos[0].Repeticion;
                for (short i = -1; i < repeatTimes; i++)
                {
                    OnLiteral("<tr>");
                    foreach (var repetido in repetidos)
                    {
                        OnLiteral("<td>");
                        if (i == -1)
                            OnLiteral(repetido.Nombre);
                        else
                        {
                            var id = TipoDocumentoHelper.GetControlName(repetido, i);
                            OnParametro(new DocumentoParametroEventArgs(id, string.Empty, repetido));
                        }
                        OnLiteral("</td>");
                    }
                    OnLiteral("</tr>");
                }
                OnLiteral("</table></td></tr>");
                repetidos.Clear();
            }

            OnLiteral("</table>");
        }
    }
}