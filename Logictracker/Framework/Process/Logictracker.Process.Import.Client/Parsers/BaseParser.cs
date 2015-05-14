using System;
using System.Configuration;
using Logictracker.Process.Import.Client.Mapping;
using Logictracker.Process.Import.Client.Types;
using Operation = Logictracker.Process.Import.Client.Mapping.Operation;

namespace Logictracker.Process.Import.Client.Parsers
{
    public class BaseParser
    {
        protected int Entity { get; set; }
        protected Type EntityType { get; set; }

        public BaseParser(Entities entities, Entity entity)
        {
            Configuration = entity;
            Entity = (int) entities;
            var name = Enum.GetName(typeof (Entities), Entity);
            EntityType = typeof (Properties).GetNestedType(name);
        }

        protected int GetOperation(Record record)
        {
            var op = Configuration.Operation != null && Configuration.Operation.TypeSpecified
                                         ? Configuration.Operation.Type
                                         : OperationType.AddOrModify;
            if (op == OperationType.Custom)
            {
                if (Configuration.Operation.OperationProperty == null) throw new ConfigurationErrorsException("Debe definir el elemento OperationProperty");
                var byColumnName = !string.IsNullOrEmpty(Configuration.Operation.OperationProperty.ColumnName);
                var byColumnIndex = Configuration.Operation.OperationProperty.ColumnIndexSpecified;
                if (!byColumnIndex && !byColumnName) throw new ConfigurationErrorsException("Debe definir uno de los elementos OperationProperty.ColumnIndex o OperationProperty.ColumnName");

                var value = (byColumnIndex
                    ? record[Configuration.Operation.OperationProperty.ColumnIndex]
                    : record[Configuration.Operation.OperationProperty.ColumnName]).ToString();

                foreach (var ov in Configuration.Operation.OperationValue)
                {
                    var val = ov.Value ?? string.Empty;
                    var same = ov.CaseSensitive
                                   ? val == value
                                   : val.ToLower() == value.ToLower();
                    if (ov.Default) op = ov.Operation;
                    if (!same) continue;
                    op = ov.Operation;
                    break;
                }
            }
            return (int)Operation.GetEquivalent(op);
        }

        #region IParser Members

        public IData Parse(Record record)
        {
            var data = new Data { Entity = Entity, Operation = GetOperation(record) };
            foreach (var prop in Configuration.Property)
            {
                
                var property = Properties.GetAs(prop.Name, EntityType);

                if (prop.ColumnName != null && !record.Contains(prop.ColumnName)) throw new ApplicationException("No se encontro la columna: " + prop.ColumnName);
                var byNameValue = prop.ColumnName != null ? record[prop.ColumnName] : null;

                var value = prop.ColumnIndexSpecified ? record[prop.ColumnIndex]
                    : byNameValue != null && !string.IsNullOrEmpty(byNameValue.ToString()) ? byNameValue
                    : prop.Default;

                if(property == 0)
                {
                    value = string.Format("{0}=\"{1}\"", prop.Name, value);
                }

                data.Add(property, (value ?? string.Empty).ToString());
            }
            return data;
        }

        public Entity Configuration { get; set; }

        #endregion
    }
}
