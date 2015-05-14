#region Usings

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.Documentos.Helpers;
using Logictracker.Web.Documentos.Interfaces;

#endregion

namespace Logictracker.Web.Documentos.Combustible
{
    public class CombustibleSaver: GenericSaver
    {
        public CombustibleSaver(TipoDocumento tipoDoc, IDocumentView view, DAOFactory daof) : base(tipoDoc, view, daof)
        {
        }

        private readonly Dictionary<int, double> diario = new Dictionary<int, double>();
        private double cantidad;
        private double motores;
        private double generador;
        private double propios;
        private double terceros;
        private int dia = 26;
        protected override string GetParameterValue(TipoDocumentoParametro parameter, short repeticion)
        {
            if (dia > 31) dia = 1;
            switch (parameter.Nombre)
            {
                case CamposCombustible.Dia: 
                    return (dia++).ToString(); 
                case CamposCombustible.Actividad:
                    var combo = GetControlFromView(TipoDocumentoHelper.GetControlName(parameter, repeticion)) as DropDownList;
                    return combo.SelectedValue;
                case CamposCombustible.StockDiario:
                case CamposCombustible.TotalCantidad:
                case CamposCombustible.TotalMotores:
                case CamposCombustible.TotalGenerador:
                case CamposCombustible.TotalEgresosPropios:
                case CamposCombustible.TotalEgresosTerceros:
                case CamposCombustible.TotalStockDiario:
                    return "0";
                case CamposCombustible.Cliente:
                    var cbEquipo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI19) as DropDownList;
                    var equipo = DAOFactory.EquipoDAO.FindById(Convert.ToInt32(cbEquipo.SelectedValue));
                    return equipo.Cliente.Id.ToString();
                default:
                    var val = base.GetParameterValue(parameter, repeticion);
                    double d = GetFloat(val);
                    if (d == -999) d= 0;
                    if (parameter.Nombre == CamposCombustible.Cantidad)
                    {
                        cantidad += d;
                        if (!diario.ContainsKey(repeticion)) diario.Add(repeticion, 0);
                        diario[repeticion] += d;
                        return d.ToString();
                    }
                    if (parameter.Nombre ==  CamposCombustible.Motores)
                    {
                        motores += d;
                        if (!diario.ContainsKey(repeticion)) diario.Add(repeticion, 0);
                        diario[repeticion] += d;
                        return d.ToString();
                    }
                    if (parameter.Nombre ==  CamposCombustible.Generador)
                    {
                        generador += d;
                        if (!diario.ContainsKey(repeticion)) diario.Add(repeticion, 0);
                        diario[repeticion] += d;
                        return d.ToString();
                    }
                    if (parameter.Nombre ==  CamposCombustible.EgresosPropios)
                    {
                        propios += d;
                        if (!diario.ContainsKey(repeticion)) diario.Add(repeticion, 0);
                        diario[repeticion] += d;
                        return d.ToString();
                    }
                    if (parameter.Nombre ==  CamposCombustible.EgresosTerceros)
                    {
                        terceros += d;
                        if (!diario.ContainsKey(repeticion)) diario.Add(repeticion, 0);
                        diario[repeticion] += d;
                        return d.ToString();
                    }

                    return val;
            }
        }
        protected override void AfterValidate(Documento doc)
        {
            var txtCodigo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_CODIGO) as TextBox;
            var cbEquipo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI19) as DropDownList;
            var eq = cbEquipo.SelectedItem.Text;
            var mes = string.Empty;
            var anio = string.Empty;
            foreach (TipoDocumentoParametro parametro in TipoDocumento.Parametros)
            {
                switch (parametro.Nombre)
                {
                    case CamposCombustible.Mes:
                        mes = (GetControlFromView(TipoDocumentoHelper.GetControlName(parametro)) as TextBox).Text;
                        break;
                    case CamposCombustible.Anio:
                        anio = (GetControlFromView(TipoDocumentoHelper.GetControlName(parametro)) as TextBox).Text;
                        break;
                }
            }
            txtCodigo.Text = eq + " - " + mes + "/" + anio;
        }
        protected override void BeforeSave(Documento doc)
        {
            foreach (TipoDocumentoParametro parametro in TipoDocumento.Parametros)
            {
                switch (parametro.Nombre)
                {
                    case CamposCombustible.StockDiario:
                        foreach (var pair in diario)
                        {
                            SetValor(doc, parametro.Nombre, pair.Key, pair.Value.ToString());
                            SetValor(doc, CamposCombustible.TotalStockDiario, 1, pair.Value.ToString());
                        }
                        break;
                    case CamposCombustible.TotalCantidad:
                        SetValor(doc, parametro.Nombre, 1, cantidad.ToString());
                        break;
                    case CamposCombustible.TotalMotores:
                        SetValor(doc, parametro.Nombre, 1, motores.ToString());
                        break;
                    case CamposCombustible.TotalGenerador:
                        SetValor(doc, parametro.Nombre, 1, generador.ToString());
                        break;
                    case CamposCombustible.TotalEgresosPropios:
                        SetValor(doc, parametro.Nombre, 1, propios.ToString());
                        break;
                    case CamposCombustible.TotalEgresosTerceros:
                        SetValor(doc, parametro.Nombre, 1, terceros.ToString());
                        break;
                }
            }
        }
        protected override void Validate()
        {
            base.Validate();

            var cbEquipo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI19) as DropDownList;
            if(cbEquipo.SelectedIndex < 0) throw new ApplicationException("No se encontro el valor de Equipo");

            foreach (TipoDocumentoParametro parametro in TipoDocumento.Parametros)
            {
                switch (parametro.Nombre)
                {
                    case CamposCombustible.Tanque:
                        var cb = GetControlFromView(TipoDocumentoHelper.GetControlName(parametro)) as DropDownList;
                        if (cb == null || cb.SelectedIndex < 0) throw new ApplicationException("No se encontro el valor de " + parametro.Nombre);
                        break;
                    case CamposCombustible.Mes:
                    case CamposCombustible.Anio:
                        var txt = GetControlFromView(TipoDocumentoHelper.GetControlName(parametro)) as TextBox;
                        int a;
                        if (txt == null || !int.TryParse(txt.Text, out a)) throw new ApplicationException("Valor invalido para campo " + parametro.Nombre);
                        if (parametro.Nombre == CamposCombustible.Mes && (a < 1 || a > 12)) throw new ApplicationException("Valor invalido para campo " + parametro.Nombre);
                        if (parametro.Nombre == CamposCombustible.Anio && (a < 1900 || a > 3000)) throw new ApplicationException("Valor invalido para campo " + parametro.Nombre);
                        break;
                    case CamposCombustible.StockInicial:
                        var txtd = GetControlFromView(TipoDocumentoHelper.GetControlName(parametro)) as TextBox;
                        double d;
                        if (txtd == null || !double.TryParse(txtd.Text, out d)) throw new ApplicationException("Valor invalido para campo " + parametro.Nombre);
                        break;
                }
            }
        }

    }
}
