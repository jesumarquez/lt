using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.ValueObjects.Documentos.Partes;
using Logictracker.Web.CustomWebControls.DropDownLists;
using Logictracker.Web.Documentos.Interfaces;

namespace Logictracker.Web.Documentos.Partes
{
    public class PartePresentStrategy: DefaultPresentStrategy
    {
        private MovilDropDownList ddlMovil;
        private int estado;

        public PartePresentStrategy(TipoDocumento tipoDoc, IDocumentView view, DAOFactory daof) : base(tipoDoc, view, daof)
        {
            view.RegisterScript("regkm", "var km = []; var odo = [];");
            view.RegisterScript("CalculaOdometro",
                                @"function CalculaOdometro()
                {
                    var t= 0; 
                    for(var i = 0; i < odo.length; i++) 
                    {
                        try{
                            var ini = $get(odo[i].ini);
                            var fin = $get(odo[i].fin);
                            var txt = $get(km[i]);
                            
                            var val1 = parseInt(ini.value,10);
                            var val2 = parseInt(fin.value,10);
                            if(isNaN(val1) || isNaN(val2)) continue; 
                            
                            txt.value = val2 - val1;
                        }catch(e){}
                    }
                    CalculaKmTotal();
                }");
        }
        public override void SetValores(Documento documento)
        {
            base.SetValores(documento);

            var horas = documento.Valores[ParteCampos.SalidaAlPozo] as List<object>;
            if (horas == null) return;
            DateTime dt;
            if (!DateTime.TryParse(horas[0].ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return;
            dt = documento.Fecha.Date.Add(dt.Subtract(dt.Date));
            var txtFecha = view.DocumentContainer.FindControl("fecha") as TextBox;
            if (txtFecha != null) txtFecha.Text = dt.ToDisplayDateTime().ToString("dd/MM/yyyy");
        }
        protected override void AddParameter(TipoDocumentoParametro par, string id, string style)
        {
            if(par.Nombre == ParteCampos.EstadoControl)
            {
                AddEstado(id, style);
            }
            else if (par.Nombre == ParteCampos.SalidaAlPozo
                     || par.Nombre == ParteCampos.LlegadaAlPozo
                     || par.Nombre == ParteCampos.SalidaDelPozo
                     || par.Nombre == ParteCampos.LlegadaDelPozo)
            {
                AddTime(id, style);
            }
            else if (par.Nombre == ParteCampos.KilometrajeTotal)
            {
                AddKmTotal(id, style);
            }
            else if (par.Nombre == ParteCampos.Kilometraje)
            {
                var txt = new TextBox {ID = id};
                txt.Style.Value = style;
                txt.Attributes.Add("onchange","CalculaKmTotal();");
                view.DocumentContainer.Controls.Add(txt);
                view.RegisterScript("km_" + id, "km.push('" + txt.ClientID + "');");
            }
            else if(par.Nombre == ParteCampos.OdometroInicial)
            {
                var txt = new TextBox {ID = id};
                txt.Style.Value = style;
                txt.Attributes.Add("onchange", "CalculaOdometro();");
                view.DocumentContainer.Controls.Add(txt);
                view.RegisterScript("odom_" + id, "odo.push({ini: '" + txt.ClientID + "'});");
            }
            else if(par.Nombre == ParteCampos.OdometroFinal)
            {
                var txt = new TextBox { ID = id };
                txt.Style.Value = style;
                txt.Attributes.Add("onchange", "CalculaOdometro();");
                view.DocumentContainer.Controls.Add(txt);
                view.RegisterScript("odom_" + id, "odo[odo.length-1].fin = '" + txt.ClientID + "';");
            }
            else if(par.Nombre == ParteCampos.TipoServicio)
            {
                AddTipoServicio(id, style);
            }
            else
            {
                base.AddParameter(par, id, style);
            }
        }
        protected virtual void AddEstado(string id, string style)
        {
            var lbl = new Label {ID = id};
            lbl.Style.Value = style;
            lbl.Text = "Sin Controlar";
            view.DocumentContainer.Controls.Add(lbl);
        }
        protected virtual void AddTipoServicio(string id, string style)
        {
            var cb = new DropDownList { ID = id };
            cb.Style.Value = style;
            for (var i = 0; i < ParteCampos.ListaTipoServicios.Count; i++)
            {
                cb.Items.Add(new ListItem(ParteCampos.ListaTipoServicios[i], i.ToString()));
            }
            view.DocumentContainer.Controls.Add(cb);
        }
        protected virtual void AddTime(string id, string style)
        {
            var date = new TextBox {ID = id};
            date.Style.Value = style;
            var cal = new MaskedEditExtender
                          {
                              ID = (id + "_mask"),
                              TargetControlID = date.ClientID,
                              Mask = "99:99",
                              MaskType = MaskedEditType.Time,
                              UserTimeFormat = MaskedEditUserTimeFormat.TwentyFourHour,
                              AutoComplete = true,
                              AutoCompleteValue = "0"
                          };

            view.DocumentContainer.Controls.Add(date);
            view.DocumentContainer.Controls.Add(cal);
        }
        private void AddKmTotal(string id, string style)
        {
            var date = new TextBox {ID = id};
            date.Style.Value = style;
            //date.Enabled = false;

            view.DocumentContainer.Controls.Add(date);
            view.RegisterScript("CalculaKmTotal",
                                @"function CalculaKmTotal()
                {
                    var t= 0; 
                    for(var i = 0; i < km.length; i++) 
                    {
                        var txt = $get(km[i]);
                        var val = parseInt(txt.value,10);
                        if(!isNaN(val)) 
                            t += val;
                    }
                    $get('" + date.ClientID+"').value = t;}");
        }
        protected override void SetDateTimeValue(Control ctl, DocumentoValor valor, int repeticion)
        {
            if (valor.Parametro.Nombre == "Hs Salida Al Pozo"
                || valor.Parametro.Nombre == "Hs Llegada Al Pozo"
                || valor.Parametro.Nombre == "Hs Salida Del Pozo"
                || valor.Parametro.Nombre == "Hs Llegada Del Pozo")
            {
                var d = Convert.ToDateTime(valor.Valor, CultureInfo.InvariantCulture);
                var textbox = (ctl as TextBox);
                if(textbox != null) textbox.Text = d.ToDisplayDateTime().ToString("HH:mm");
            }
            else base.SetDateTimeValue(ctl, valor, repeticion);
        }

        protected override void SetInt32Value(Control ctl, DocumentoValor valor, int repeticion)
        {
            if (valor.Parametro.Nombre == ParteCampos.EstadoControl)
            {
                estado = Convert.ToInt32(valor.Valor);
                var label = ctl as Label;
                if(label != null)
                    switch (valor.Valor)
                    {
                        case "1": label.Text = "Controlado"; break;
                        case "2": label.Text = "Verificado"; break;
                        default: label.Text = "Sin Controlar";break;
                    }
            }
            else if(valor.Parametro.Nombre == ParteCampos.TipoServicio)
            {
                var cb = (ctl as DropDownList);
                int i;
                if(cb != null && int.TryParse(valor.Valor, out i)) cb.SelectedValue = valor.Valor;
            }
            else base.SetInt32Value(ctl, valor, repeticion);
        }

        protected override void AddCoche(string id, string style)
        {
            base.AddCoche(id, style);
            ddlMovil = view.DocumentContainer.FindControl(id) as MovilDropDownList;
        } 
        protected override void OnCreated()
        {
            base.OnCreated();
            if(!ddlMovil.ParentControls.EndsWith(",Empresa"))
                ddlMovil.ParentControls += ",Empresa";
        }
        protected override void OnValuesSetted()
        {
            base.OnValuesSetted();
            if (estado == 2) view.Enabled = false;
        }
    }
}