using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace Logictracker.Utils.NHibernate
{
    public class BaseInterceptor : EmptyInterceptor
    {
        protected bool IsAValidOperation(SqlString sql)
        {
            var operations = new[] { "INSERT", "UPDATE", "DELETE", "SELECT" };
            return operations.Any(operation => sql.ToString().Contains(operation));
        }
    }
}
