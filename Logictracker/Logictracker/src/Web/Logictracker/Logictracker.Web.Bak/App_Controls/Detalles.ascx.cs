using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.BaseClasses.BaseControls;
using Logictracker.Web.CustomWebControls.Input;

namespace Logictracker.App_Controls
{
    public partial class Detalles : BaseUserControl
    {
        public int IdTipoEntidad { get; set; }
        public EntidadPadre Entidad { get; set; }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadDetalles(IdTipoEntidad);
            if (!IsPostBack)
                BindDetalles(IdTipoEntidad, Entidad);
        }

        public Dictionary<int, DetalleValor> GetValores()
        {
            var dic = new Dictionary<int, DetalleValor>();

            foreach (var control in tpDetalles.Controls)
            {
                var detalleId = GetId(control);
                if (detalleId <= 0) continue;

                var detalle = DAOFactory.DetalleDAO.FindById(detalleId);
                if (detalle == null) continue;
                
                var detalleValor = new DetalleValor { Detalle = detalle };

                switch (detalle.Representacion)
                {
                    case 1:
                        switch (detalle.Tipo)
                        {
                            case 1:
                                detalleValor.ValorStr = ((TextBox) control).Text;
                                break;
                            case 2:
                                double num;
                                if (double.TryParse(((TextBox)control).Text, out num))
                                    detalleValor.ValorNum = num;
                                else
                                    ThrowError("INVALID_VALUE", detalle.Nombre);
                                break;
                            case 3:
                                detalleValor.ValorDt = ((DateTimePicker) control).SelectedDate.Value;
                                break;
                        }
                        break;
                    case 2:
                        detalleValor.ValorNum = Convert.ToDouble(((DropDownList) control).SelectedValue);
                        break;
                    case 3:
                        var list = ((ListBox) control);
                        var str = "";

                        foreach (var i in list.GetSelectedIndices())
                        {
                            if (str != "") str += "-";
                            str += list.Items[i].Value;
                        }

                        detalleValor.ValorStr = str;
                        break;
                }

                dic.Add(detalleId, detalleValor);
            }

            return dic;
        }

        public void LoadDetalles(int idTipoEntidad)
        {
            tpDetalles.Controls.Clear();

            if (idTipoEntidad > 0)
            {
                var detalles = DAOFactory.DetalleDAO.GetList(new[] {-1}, new[] {-1}, new[] {idTipoEntidad}, new[] {-1});

                foreach (var detalle in detalles)
                {
                    var label = new Label {Text = detalle.Nombre};
                    tpDetalles.Controls.Add(label);

                    LoadDetalle(detalle);
                }
            }
        }

        public void BindDetalles(int idTipoEntidad, EntidadPadre editObject)
        {
            Entidad = editObject;
            IdTipoEntidad = idTipoEntidad;
            tpDetalles.Controls.Clear();

            if (idTipoEntidad <= 0) return;

            var detalles = DAOFactory.DetalleDAO.GetList(new[] {-1}, new[] {-1}, new[] {idTipoEntidad}, new[] {-1});

            foreach (var detalle in detalles)
            {
                var label = new Label {Text = detalle.Nombre};
                tpDetalles.Controls.Add(label);

                switch (detalle.Representacion)
                {
                    case 1:
                        switch (detalle.Tipo)
                        {
                            case 1:
                                var txt = new TextBox {ID = detalle.Id.ToString()};
                                var valor = editObject != null ? editObject.GetDetalle(detalle.Id) : null;

                                if (editObject != null && editObject.Id > 0 && valor != null)
                                    txt.Text = valor.ValorStr;

                                tpDetalles.Controls.Add(txt);
                                break;
                            case 2:
                                var num = new TextBox {ID = detalle.Id.ToString()};
                                var val = editObject != null ? editObject.GetDetalle(detalle.Id) : null;

                                if (editObject != null && editObject.Id > 0 && val != null)
                                    num.Text = val.ValorNum.ToString();

                                tpDetalles.Controls.Add(num);
                                break;
                            case 3:
                                var dt = new DateTimePicker {ID = detalle.Id.ToString()};
                                var value = editObject != null ? editObject.GetDetalle(detalle.Id) : null;

                                if (editObject != null && editObject.Id > 0 && value != null)
                                    dt.SelectedDate = value.ValorDt;

                                tpDetalles.Controls.Add(dt);
                                break;
                        }
                        break;
                    case 2:
                        var lst = new DropDownList {ID = detalle.Id.ToString()};
                        var opciones = detalle.Opciones.Split('|');
                        if (opciones.Length > 0)
                        {
                            foreach (var opcion in opciones)
                            {
                                var valores = opcion.Split('.');
                                if (valores.Length > 1)
                                    lst.Items.Add(new ListItem(valores[valores.Length - 1],
                                                               valores[valores.Length - 2]));
                            }
                        }

                        var det = editObject != null ? editObject.GetDetalle(detalle.Id) : null;

                        lst.SelectedValue = det != null ? Convert.ToInt32(det.ValorNum).ToString() : null;

                        tpDetalles.Controls.Add(lst);

                        var dic = LastPostBack;
                        if (dic.ContainsKey(lst.ID))
                            dic.Remove(lst.ID);
                        dic.Add(lst.ID, lst.SelectedValue);
                        LastPostBack = dic;
                        break;
                    case 3:
                        var mlt = new ListBox
                                      {
                                          ID = detalle.Id.ToString(),
                                          SelectionMode = ListSelectionMode.Multiple
                                      };

                        var options = detalle.Options;

                        if (detalle.HasParent)
                        {
                            var parent = tpDetalles.FindControl(detalle.DetallePadre.Id.ToString()) as ListControl;
                            if (parent != null)
                            {
                                AddParent(detalle.Id.ToString(), parent);
                                options = options.Where(o => o.Parent == parent.SelectedValue).ToList();
                            }
                        }

                        var selectedDet = editObject != null ? editObject.GetDetalle(detalle.Id) : null;

                        var selected =
                            (selectedDet != null ? selectedDet.ValorStr.Split('-') : new string[0]).ToList();

                        foreach (var option in options)
                        {
                            var li = new ListItem(option.Text, option.Index)
                                         {
                                             Selected = selected.Contains(option.Index)
                                         };
                            mlt.Items.Add(li);
                        }

                        tpDetalles.Controls.Add(mlt);
                        break;
                }
            }
        }

        private void LoadDetalle(Detalle detalle)
        {
            switch (detalle.Representacion)
            {
                case 1:
                    switch (detalle.Tipo)
                    {
                        case 1:
                        case 2:
                            var txt = new TextBox { ID = detalle.Id.ToString() };
                            tpDetalles.Controls.Add(txt);
                            break;
                        case 3:
                            var dt = new DateTimePicker { ID = detalle.Id.ToString(), SelectedDate = DateTime.Today };
                            tpDetalles.Controls.Add(dt);
                            break;
                    }
                    break;
                case 2:
                    var lst = new DropDownList { ID = detalle.Id.ToString() };
                    var opciones = detalle.Opciones.Split('|');
                    if (opciones.Length > 0)
                    {
                        foreach (var opcion in opciones)
                        {
                            var valores = opcion.Split('.');
                            if (valores.Length > 1)
                                lst.Items.Add(new ListItem(valores[valores.Length - 1], valores[valores.Length - 2]));
                        }
                    }

                    tpDetalles.Controls.Add(lst);
                    break;
                case 3:
                    var mlt = new ListBox
                    {
                        ID = detalle.Id.ToString(),
                        SelectionMode = ListSelectionMode.Multiple
                    };

                    var options = detalle.Options;

                    if (detalle.HasParent)
                    {
                        var parent = tpDetalles.FindControl(detalle.DetallePadre.Id.ToString()) as ListControl;
                        if (parent != null)
                        {
                            AddParent(detalle.Id.ToString(), parent);
                            options = options.Where(o => o.Parent == parent.SelectedValue).ToList();
                        }
                    }

                    foreach (var option in options)
                    {
                        var li = new ListItem(option.Text, option.Index);
                        mlt.Items.Add(li);
                    }

                    tpDetalles.Controls.Add(mlt);
                    break;
            }
        }

        protected void AddParent(string id, ListControl parent)
        {
            if (!parent.AutoPostBack)
            {
                parent.SelectedIndexChanged += ParentSelectedIndexChanged;
                parent.AutoPostBack = true;
            }
            AddChild(parent.ID, id);
        }

        protected void ParentSelectedIndexChanged(object sender, EventArgs e)
        {
            var dic = LastPostBack;
            var s = sender as ListControl;

            if (s == null) return;

            if (dic.ContainsKey(s.ID))
            {
                if (dic[s.ID] == s.SelectedValue)
                    return;

                dic.Remove(s.ID);
            }

            dic.Add(s.ID, s.SelectedValue);
            LastPostBack = dic;

            if (!DicPadres.ContainsKey(s.ID)) return;

            foreach (var child in DicPadres[s.ID])
            {
                var hijo = DAOFactory.DetalleDAO.FindById(Convert.ToInt32(child));
                tpDetalles.Controls.Remove(FindControl(child));
                LoadDetalle(hijo);
            }
        }
        
        private void AddChild(string parent, string child)
        {
            var dic = DicPadres;
            var list = dic.ContainsKey(parent) ? dic[parent].ToList() : new List<string>();
            if (!list.Contains(child))
                list.Add(child);
            
            if (dic.Keys.Contains(parent))
                dic.Remove(parent);
            dic.Add(parent, list.ToArray());
            DicPadres = dic;
        }

        private Dictionary<string, string[]> DicPadres
        {
            get { return ViewState["DIC_PADRES"] as Dictionary<string, string[]> ?? new Dictionary<string, string[]>() ; }
            set { ViewState["DIC_PADRES"] = value; }
        }

        private Dictionary<string, string> LastPostBack
        {
            get { return ViewState["LAST_POST"] as Dictionary<string, string> ?? new Dictionary<string, string>(); }
            set { ViewState["LAST_POST"] = value; }
        }

        private static int GetId(object ctrl)
        {
            if (ctrl.GetType() == typeof(TextBox))
                return Convert.ToInt32(((TextBox)ctrl).ID);

            if (ctrl.GetType() == typeof(DateTimePicker))
                return Convert.ToInt32(((DateTimePicker)ctrl).ID);

            if (ctrl.GetType() == typeof(DropDownList))
                return Convert.ToInt32(((DropDownList)ctrl).ID);

            if (ctrl.GetType() == typeof(ListBox))
                return Convert.ToInt32(((ListBox)ctrl).ID);

            return 0;
        }
    }
}
