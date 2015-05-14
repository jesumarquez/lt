#region Usings

using System.Collections;
using NHibernate;
using Urbetrack.Types.BusinessObjects;

#endregion

namespace Urbetrack.DAL.DAO.BusinessObjects
{
    public class TallerDAO : GenericDAO<Taller>
    {
        public TallerDAO(ISession sess) : base(sess) { }

        /// <summary>
        /// Gets talleres by empresa and linea.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <returns></returns>
        public IList FindByEmpresaYLinea(int empresa, int linea)
        {
            return
                sess.CreateQuery(
                    @"select t
                      from Taller t
                      where ((:empresa > 0 and (t.Empresa is null or (t.Empresa is not null and t.Empresa.Id = :empresa)))
                            or :empresa < 0)
                            and ((:linea > 0 and (t.Linea is null or (t.Linea is not null and t.Linea.Id = :linea)))
                            or :linea < 0)
                            and t.Baja = 0")
                    .SetParameter("empresa", empresa)
                    .SetParameter("linea", linea)
                    .List();
        }

        /// <summary>
        /// Deletes the givenn taller.
        /// </summary>
        /// <param name="obj"></param>
        public void Delete(Taller obj)
        {
            obj.Baja = true;

            SaveOrUpdate(obj);
        }

        /// <summary>
        /// Saves or updates the givenn taller.
        /// </summary>
        /// <param name="obj"></param>
        public override void SaveOrUpdate(Taller obj)
        {
            obj.Domicilio.Empresa = obj.Empresa;
            obj.Domicilio.Linea = obj.Linea;
            
            sess.Evict(obj.Domicilio);

            base.SaveOrUpdate(obj);
        }
    }
}
