#region Usings

using System;
using System.Data;

#endregion

namespace Quartz.Impl.AdoJobStore
{
    public class AdoUtil
    {
        private readonly IDbProvider dbProvider;

        public AdoUtil(IDbProvider dbProvider)
        {
            this.dbProvider = dbProvider;
        }

        public void AddCommandParameter(IDbCommand cmd, int parameterIndex, string paramName, object paramValue)
        {
            AddCommandParameter(cmd, parameterIndex, paramName, paramValue, null);
        }

        public void AddCommandParameter(IDbCommand cmd, int parameterIndex, string paramName, object paramValue,
                                           Enum dataType)
        {
            var param = cmd.CreateParameter();
            if (dataType != null)
            {
                SetDataTypeToCommandParameter(param, dataType);
            }
            param.ParameterName = dbProvider.Metadata.GetParameterName(paramName);
            if (paramValue != null)
            {
                param.Value = paramValue;
            }
            else
            {
                param.Value = DBNull.Value;
            }
            cmd.Parameters.Add(param);

            if (dbProvider.Metadata.ParameterNamePrefix != "@")
            {
                // we need to replace
                cmd.CommandText =
                    cmd.CommandText.Replace("@" + paramName, dbProvider.Metadata.ParameterNamePrefix + paramName);
            }
        }

        private void SetDataTypeToCommandParameter(IDbDataParameter param, object parameterType)
        {
            dbProvider.Metadata.ParameterDbTypeProperty.GetSetMethod().Invoke(param, new[] { parameterType });
        }

        public IDbCommand PrepareCommand(ConnectionAndTransactionHolder cth, string commandText)
        {
            var cmd = dbProvider.CreateCommand();
            cmd.CommandText = commandText;
            cmd.Connection = cth.Connection;
            cmd.Transaction = cth.Transaction;
            return cmd;
        }
    }
}
