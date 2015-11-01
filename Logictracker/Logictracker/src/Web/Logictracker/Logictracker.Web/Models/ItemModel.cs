using System.Collections.Generic;
using Logictracker.Culture;

namespace Logictracker.Web.Models
{

    public class ItemModel
    {
        public int Key { get; set; }
        public string Value { get; set; }

        public static IEnumerable<ItemModel> All
        {
            get { return new[] {new ItemModel {Key = -1, Value = CultureManager.GetControl("DDL_ALL_ITEMS")}}; }
        }

        public static IEnumerable<ItemModel> None
        {
            get { return new[] {new ItemModel {Key = -2, Value = CultureManager.GetControl("DDL_NONE")}}; }
        }
    }
}