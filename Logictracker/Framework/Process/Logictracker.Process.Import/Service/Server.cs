using System;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Process.Import.Client.Transform;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Process.Import.EntityParser;

namespace Logictracker.Process.Import.Service
{
    public class Server
    {
        public void Import(int empresa, int linea, string data, int version)
        {
            var idata = Decode(version, data);
            Import(empresa, linea, idata, version);
        }
        public void Import(int empresa, int linea, IData idata, int version)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    var parser = EntityParserFactory.GetEntityParser(idata);
                    if (parser == null) throw new EntityParserException();

                    var entity = parser.Parse(empresa, linea, idata);
                    if (entity == null) throw new EntityParserException();

                    switch (idata.Operation)
                    {
                        case (int) Operation.Delete:
                            parser.Delete(entity, empresa, linea, idata);
                            break;
                        case (int) Operation.Add:
                            parser.Save(entity, empresa, linea, idata);
                            break;
                        case (int) Operation.Modify:
                            parser.Update(entity, empresa, linea, idata);
                            break;
                        case (int) Operation.AddOrModify:
                            parser.SaveOrUpdate(entity, empresa, linea, idata);
                            break;
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    STrace.Exception(typeof(Server).FullName, ex, String.Format("Exception Server.Import(empresa={0}, linea={1})", empresa, linea));
                    transaction.Rollback();
                }
            }
        }

        private IData Decode(int version, string data)
        {
            var transform = DataTransformFactory.GetDataTransform(version);
            if (transform == null) throw new DataTransformException();

            try
            {
                var idata = transform.Decode(data);
                if (idata == null) throw new FormatException();

                return idata;
            }
            catch
            {
                throw new DataTransformException();
            }
        }
    }
}
