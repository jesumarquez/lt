using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Urbetrack.Postal.DataSources;

namespace Urbetrack.Postal
{
    public static class BindingManager
    {
        #region Private Methods

        private static IEnumerable<Motivo> GetMotivos(int cliente)
        {
            var list = new List<Motivo>();

            var sq = new SQLiteDataSet();
            sq.read(String.Concat("SELECT * FROM motivos where cliente is null or cliente = ", cliente, " order by orden"));
            if (sq.dataSet.Tables != null)
            {
                DataTable table = sq.dataSet.Tables[0];
                foreach (DataRow row in table.Rows)
                {
                    var item = new Motivo { Text = (string)row["descripcion"], Value = (int)row["id"], EsEntrega = Convert.ToBoolean(row["es_entrega"]) };
                    list.Add(item);                
                }
            }

            return list;

            /*return new List<Item>
                       { new Item{ Text = "Entregada", Value = 0 },
                         new Item { Text ="Bajo Puerta" , Value = 1 },
                         new Item { Text = "Se Mudo", Value = 2  },
                         new Item { Text = "No Responde", Value = 3}
                       };*/
        }

        private static void Bind(IEnumerable<Motivo> items, ListControl combo)
        {
            combo.DisplayMember = "Text";
            combo.ValueMember = "Value";

            combo.DataSource = items;
        }

        #endregion

        #region Public Methods

        public static void BindMotivos(ListControl combo, int cliente)
        {
            Bind(GetMotivos(cliente), combo);
        }

        #endregion
    }
}
