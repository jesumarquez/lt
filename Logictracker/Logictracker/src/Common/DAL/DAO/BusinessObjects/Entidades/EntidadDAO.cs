using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.BusinessObjects.Dispositivos;
using Logictracker.DAL.Filtros;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.BusinessObjects.Vehiculos;
using NHibernate;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Entidades
{
    public class EntidadDAO : GenericDAO<EntidadPadre>
    {
//        public EntidadDAO(ISession session) : base(session) { }

        public IEnumerable<EntidadPadre> FindAllAssigned()
        {
            return Query.Where(coche => !coche.Baja && coche.Dispositivo != null)
                .Cacheable()
                .ToList();
        }

        public List<EntidadPadre> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> dispositivos, IEnumerable<int> tiposEntidad)
        {
            var q = Query.FilterEmpresa(Session, empresas);

            if (!QueryExtensions.IncludesAll(lineas))
                q = q.FilterLinea(Session, empresas, lineas);
            if (!QueryExtensions.IncludesAll(dispositivos))
                q = q.FilterDispositivo(Session, empresas, lineas, dispositivos);
            if (!QueryExtensions.IncludesAll(tiposEntidad))
                q = q.FilterTipoEntidad(Session, empresas, lineas, tiposEntidad);

            return q.Where(m => !m.Baja).Cacheable().ToList();
        }

        public override void Delete(EntidadPadre obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }

        public bool IsCodeUnique(int empresa, int linea, int idEntidad, string code)
        {
            var q = Query.FilterEmpresa(Session, new[] { empresa }, null);

            if (linea != -1)
                q = q.FilterLinea(Session, new[] { empresa }, new[] { linea }, null);

            return  q.FirstOrDefault(g => g.Id != idEntidad 
                                          && g.Codigo == code 
                                          && !g.Baja) == null;
        }

        public List<EntidadPadre> GetByDescripcion(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> dispositivos, IEnumerable<int> tiposEntidad, string descripcion)
        {
            return GetList(empresas, lineas, dispositivos, tiposEntidad)
                          .Where(e => e.Descripcion.ToLower().Contains(descripcion.ToLower()))
                          .ToList();
        }

        public IList GetEntidadesByFiltros(IEnumerable<int> entidadesId, List<Filtro> filtros)
        {
            var parametros = new Dictionary<string, object>();
            var index = 0;

            var hql = @"from EntidadPadre e where e.Id in (:entidades) and (1=1 ";

            foreach (var filtro in filtros)
            {
                if (filtro.IdUnion.Equals(1))
                    hql += @"and ";
                else
                    hql += @"or ";

                hql += @"e.Id in (select dv.Entidad.Id from Detalle d, DetalleValor dv where d.Id = dv.Detalle.Id and d.Id = " + filtro.IdDetalle + " and ";

                var detalleDao = new DetalleDAO();
                var detalle = detalleDao.FindById(filtro.IdDetalle);

                switch (detalle.Tipo)
                {
                    case 1: hql += "dv.ValorStr "; break;
                    case 2: hql += "dv.ValorNum "; break;
                    case 3: hql += "dv.ValorDt "; break;
                }

                switch (filtro.IdOperador)
                {
                    case 1: hql += "= "; break;
                    case 2: hql += "> "; break;
                    case 3: hql += "< "; break;
                    case 4: hql += "like "; break;
                }

                hql += ":param" + index + ")";

                object obj = null;

                switch (detalle.Tipo)
                {
                    case 1:
                        if (filtro.IdOperador != 4) obj = filtro.ValorStr;
                        else obj = "%" + filtro.ValorStr + "%";
                        break;
                    case 2:
                        if (filtro.IdOperador != 4) obj = filtro.ValorNum;
                        else obj = "%" + filtro.ValorNum + "%";
                        break;
                    case 3:
                        if (filtro.IdOperador != 4) obj = filtro.ValorDt;
                        else obj = "%" + filtro.ValorDt + "%";
                        break;
                }

                parametros.Add("param" + index++, obj);

                if (detalle.Representacion == 3 && filtro.ValorStr.Split('-').Count() > 1) // DROPDOWNLIST
                {
                    var valores = filtro.ValorStr.Split('-');

                    for (var i = 0; i < valores.Count(); i++)
                    {
                        hql += @"or e.Id in (select dv.Entidad.Id from Detalle d, DetalleValor dv where d.Id = dv.Detalle.Id and d.Id = "
                            + filtro.IdDetalle + " and dv.ValorStr like :param" + index + ")";

                        parametros.Add("param" + index++, "%" + valores[i] + "%");
                    }
                }
            }

            hql += ")";
            var q = Session.CreateQuery(hql);

            q = q.SetParameterList("entidades", entidadesId);
            foreach (var parametro in parametros) 
                q = q.SetParameter(parametro.Key, parametro.Value);

            return q.List();
        }

        public EntidadPadre FindByDispositivo(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> dispositivos)
        {
            var q = Query.FilterEmpresa(Session, empresas);

            if (!QueryExtensions.IncludesAll(lineas))
                q = q.FilterLinea(Session, empresas, lineas);
            if (!QueryExtensions.IncludesAll(dispositivos))
                q = q.FilterDispositivo(Session, empresas, lineas, dispositivos);

            q = q.Where(e => !e.Baja);

            return q.FirstOrDefault();
        }

        public void SaveAndUpdateSubEntidades(EntidadPadre entidad)
        {
            var todos = new[] {-1};
            var subEntidadDao = new SubEntidadDAO();
            var dispositivoDao = new DispositivoDAO();

            var subentidades = subEntidadDao.GetList(todos, todos, todos, new[] {entidad.Id}, todos, todos);
            foreach (var subentidad in subentidades)
            {
                subentidad.Empresa = entidad.Empresa;
                subentidad.Linea = entidad.Linea;
                subEntidadDao.SaveOrUpdate(subentidad);
            }

            var dispositivo = entidad.Dispositivo;
            dispositivo.Empresa = entidad.Empresa;
            dispositivoDao.SaveOrUpdate(dispositivo);

            SaveOrUpdate(entidad);
        }

        public void SaveAndUpdateEmpresa(EntidadPadre entidad, Coche coche)
        {
            entidad.Empresa = coche.Empresa;
            entidad.Linea = coche.Linea;
            entidad.ReferenciaGeografica = coche.Linea.ReferenciaGeografica;

            var tipoEntidadDao = new TipoEntidadDAO();
            var tipoEntidad = tipoEntidadDao.FindByCode(new[] { entidad.Empresa != null ? entidad.Empresa.Id : -1 },
                                                        new[] { entidad.Linea != null ? entidad.Linea.Id : -1 },
                                                        "VEH");
            entidad.TipoEntidad = tipoEntidad;
           
            SaveAndUpdateSubEntidades(entidad);
        }
    }
}