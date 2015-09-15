using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.BusinessObjects.Vehiculos;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Documentos;
using NHibernate;
using NHibernate.Criterion;
using System.Data;

namespace Logictracker.DAL.DAO.BusinessObjects.Documentos
{
    public class DocumentoDAO : GenericDAO<Documento>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public DocumentoDAO(ISession session) : base(session) { }

        #endregion

        public List<Documento> FindForVehiculo(int tipo, int vehiculo)
        {
            return Query.Where(d =>
                    d.Estado != Documento.Estados.Eliminado && 
                    d.TipoDocumento.Id == tipo 
                    && d.Vehiculo.Id == vehiculo)
                    .ToList();
        }
        public IEnumerable<Documento> FindByVehiculo(int vehiculo)
        {
            return Query.Where(d => d.Estado != Documento.Estados.Eliminado 
                                 && d.Vehiculo.Id == vehiculo);
        }
        public List<Documento> FindForEmpleado(int tipo, int empleado)
        {
            return Query.Where(d =>
                    d.Estado != Documento.Estados.Eliminado &&
                    d.TipoDocumento.Id == tipo
                    && d.Empleado.Id == empleado)
                    .ToList();
        }
        public List<Documento> FindVencidosForEmpleado(int empleado, DateTime hasta)
        {
            return Query.Where(d => d.Estado != Documento.Estados.Eliminado 
                                 && d.Empleado.Id == empleado
                                 && d.Vencimiento <= hasta)
                        .ToList();
        }

        public Documento FindByCodigo(int tipoDocumento, string codigo)
        {
            return Query.FirstOrDefault(d => d.Estado != Documento.Estados.Eliminado &&
                                             d.TipoDocumento.Id == tipoDocumento
                                             && d.Codigo == codigo);
        }

        public IList FindByTipo(int tipoDocumento)
        {
            return Session.CreateQuery("from Documento d where d.TipoDocumento.Id = :tipo and d.Estado != -1 order by d.Codigo")
                .SetParameter("tipo", tipoDocumento)
                .List();
        }

        public IList<Documento> FindByTipo(int[] tiposDocumento, List<int> empresas, List<int> lineas)
        {
            var q = Query.FilterEmpresa(Session, empresas)
                         .FilterLinea(Session, empresas, lineas)
                         .FilterVehiculo(Session, empresas, lineas, new[]{-1},new[]{-1},new[]{-1},new[]{-1},new[]{-1})
                         .FilterEmpleado(Session, empresas, lineas, new[]{-1},new[]{-1},new[]{-1});
            
            if (!QueryExtensions.IncludesAll(tiposDocumento))
            {
                var tipos = tiposDocumento.ToList();
                q = q.Where(x => tipos.Contains(x.TipoDocumento.Id));
            }

            return q.Where(x => x.Estado != Documento.Estados.Eliminado)
                .OrderBy(x => x.Codigo)
                .ToList();
        }

        public IList FindByTipoYCodigo(int tipoDocumento, string codigo)
        {
            return Session.CreateQuery("from Documento d where d.TipoDocumento.Id = :tipo and d.Codigo = :codigo and d.Estado != -1")
                .SetParameter("tipo", tipoDocumento)
                .SetParameter("codigo", codigo)
                .List();
        }

        public IList FindByTipoAndUsuario(Usuario user, int tipoDocumento, int empresa, int linea)
        {
            return FindByTipoAndUsuario(user, tipoDocumento, empresa, linea, -1);
        }
        public IList FindByTipoAndUsuario(Usuario user, int tipoDocumento, int empresa, int linea, int transportista)
        {
            return Query.FilterEmpresa(Session, new[] {empresa}, user)
                        .FilterLinea(Session, new[] {empresa}, new[] {linea}, user)
                        .FilterVehiculo(Session, new[] {empresa}, new[] {linea}, new[] {transportista}, new[] {-1}, new[] {-1}, new[] {-1}, new[] {-1})
                        .FilterEmpleado(Session, new[] {empresa}, new[] {linea}, new[] {transportista}, new[] {-1}, new[] {-1})
                        .Where(x => x.TipoDocumento.Id == tipoDocumento)
                        .Where(x => x.Estado != Documento.Estados.Eliminado)
                        .OrderBy(x => x.Codigo)
                        .ToList();
        }

        public List<Documento> FindBy(int tipoDocumento, int transportista, int coche, int empleado, int equipo)
        {
            var q = Query.Where(x=>x.Estado != Documento.Estados.Eliminado);
            if (tipoDocumento > 0)
            {
                q = q.Where(x => x.TipoDocumento.Id == tipoDocumento);
            }
            if (transportista > 0)
            {
                q = q.Where(x => x.Transportista.Id == transportista);
            }
            if (coche > 0)
            {
                q = q.Where(x => x.Vehiculo.Id == coche);
            }
            if (empleado > 0)
            {
                q = q.Where(x => x.Empleado.Id == empleado);
            }
            if (equipo > 0)
            {
                q = q.Where(x => x.Equipo.Id == equipo);
            }
            return q.OrderBy(x => x.Fecha).ToList();
        }

        public IList FindByVencimiento()
        {
            return Session.CreateQuery("from Documento d where d.Estado not in (-1,9) and d.Vencimiento is not null and (d.EnviadoAviso1 = 0 or d.EnviadoAviso2 = 0 or d.EnviadoAviso3 = 0) order by d.Vencimiento").List();
        }

        public IList FindAllOrderByParte()
        {
            return Session.CreateCriteria(typeof(Documento))
                .AddOrder(Order.Asc("Codigo"))
                .List();
        }
        
        public Documento FindLastForVehicle(int coche, DateTime date)
        {
            const string script = @"from Documento d 
                    where d.Fecha < :date
                    and d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.TipoDato like 'coche'  and v.Valor = :coche)
                    and d.Estado != -1
                    order by d.Fecha desc";

            var l =  Session.CreateQuery(script)
                .SetParameter("date", date)
                .SetParameter("coche", coche.ToString("#0"))
                .SetMaxResults(1).List<Documento>();

            return l.Count > 0 ? l[0] : null;
        }

        public IList FindByTransportistaYCodigo(int transportista, string codigo)
        {
            const string script = @"from Documento d 
                    where d.Codigo = :codigo
                    and d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.TipoDato like 'Aseguradora'  and v.Valor = :ase)
                    and d.Estado != -1
                    order by d.Fecha";

            var q = Session.CreateQuery(script)
                .SetParameter("codigo", codigo)
                .SetParameter("ase", transportista.ToString("#0"));

            return q.List();
        }

        public IList FindParteReport(int aseguradora, int locacion, int linea, int movil, int equipo, DateTime inicio, DateTime fin, int estado, int usuario)
        {
            var empresaDAO = new EmpresaDAO();
            var lineaDAO = new LineaDAO();
            var cocheDAO = new CocheDAO();

            var lin = linea > 0 ? lineaDAO.FindById(linea) : null;
            var emp = lin != null ? lin.Empresa : locacion > 0 ? empresaDAO.FindById(locacion) : null;

            IEnumerable<string> coches;

            if (movil > 0) coches = new List<string>(new[] { movil.ToString("#0") });
            else coches = cocheDAO.GetList(new[] {emp != null ? emp.Id : -1}, new[] {lin != null ? lin.Id : -1})
                    .Select(c => c.Id.ToString("#0"))
                    .ToList();

            var script =
                @"from Documento d 
                    where d.Fecha >= :ini
                    and d.Fecha <= :fin    
                    and d.Estado != -1
                    and d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.TipoDato like 'Aseguradora'  and v.Valor = :ase)       
                ";

            if (equipo > 0)
                script +=
                    @" and d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.Nombre like 'Equipo'  and v.Valor = :equ) ";

            if (estado >= 0)
                script +=
                    @" and ((:est = '0' and d.Id not in 
                        (select v.Documento.Id from DocumentoValor v 
                            where v.Parametro.Nombre like 'Estado Control' and v.Valor <> :est)
                        ) or (:est <> '0' and d.Id in (select v.Documento.Id from DocumentoValor v 
                            where v.Parametro.Nombre like 'Estado Control' and v.Valor = :est))) ";

            script += " order by d.Fecha";

            var q = Session.CreateQuery(script)
                .SetParameter("ini", inicio)
                .SetParameter("fin", fin)
                .SetParameter("ase", aseguradora.ToString("#0"));

            if (equipo > 0) q.SetParameter("equ", equipo.ToString("#0"));

            if (estado >= 0) q.SetParameter("est", estado.ToString("#0"));

            return q.List().Cast<Documento>().Where(documento => documento.Parametros.OfType<DocumentoValor>().Any(v => v.Parametro.TipoDato.ToLower().Equals("coche") && coches.Contains(v.Valor))).ToList();
        }
        public IList FindParteReport(int aseguradora, DateTime inicio, DateTime fin, ICollection coches)
        {
            var q = Session.CreateQuery(
                @"from Documento d 
                    where d.Fecha >= :ini
                    and d.Fecha <= :fin    
                    and d.Estado != -1
                    and d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.TipoDato like 'Aseguradora'  and v.Valor = :ase)       

                    and d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.TipoDato like 'Coche' and v.Valor in (:coches))
                ")
                .SetParameter("ini", inicio)
                .SetParameter("fin", fin)
                .SetParameter("ase", aseguradora.ToString("#0"))
                .SetParameterList("coches", coches);

            return q.List();
        }
        public IList FindParteReport(int aseguradora, DateTime inicio, DateTime fin, ICollection coches, int estado, ICollection equipos, ICollection tipos)
        {
            var hql =
                @"select d from Documento d 
                    where d.Fecha >= :ini
                    and d.Fecha <= :fin    
                    and d.Estado != -1
                    and ((:est = '0' and d.Id not in 
                        (select v.Documento.Id from DocumentoValor v 
                            where v.Parametro.Nombre like 'Estado Control' and v.Valor <> :est)
                        ) or (:est <> '0' and d.Id in (select v.Documento.Id from DocumentoValor v 
                            where v.Parametro.Nombre like 'Estado Control' and v.Valor = :est)))

                    and d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.TipoDato like 'Aseguradora'  and v.Valor = :ase)

                    and d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.TipoDato like 'Coche' and v.Valor in (:coches))
                ";

            if (equipos != null || tipos != null)
            {
                hql += "  and (";

                if (equipos != null)
                {
                    hql +=
                        @" (d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.TipoDato like 'Equipo' and v.Valor in (:equipos))
                        and d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.Nombre like 'Tipo Servicio' and v.Valor == '0')
                    ) ";
                    if (tipos != null) hql += " or ";
                }
                if (tipos != null)
                    hql += @" d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.Nombre like 'Tipo Servicio' and v.Valor != '0' and v.Valor in (:tipos)) ";

                hql += ")";
            }

            var q = Session.CreateQuery(hql)
                .SetParameter("ini", inicio)
                .SetParameter("fin", fin)
                .SetParameter("ase", aseguradora.ToString("#0"))
                .SetParameter("est", estado.ToString("#0"))
                .SetParameterList("coches", coches);

            if (equipos != null)
                q.SetParameterList("equipos", equipos);

            if (tipos != null)
                q.SetParameterList("tipos", tipos);

            return q.List();
        }
        public IList FindParteReport(int aseguradora, DateTime inicio, DateTime fin, ICollection coches, int estado, ICollection equipos)
        {
            return FindParteReport(aseguradora, inicio, fin, coches, estado, equipos, null);

//            var hql =
//                @"select d from Documento d 
//                    where d.Fecha >= :ini
//                    and d.Fecha <= :fin    
//
//                    and ((:est = '0' and d.Id not in 
//                        (select v.Documento.Id from DocumentoValor v 
//                            where v.Parametro.Nombre like 'Estado Control' and v.Valor <> :est)
//                        ) or (:est <> '0' and d.Id in (select v.Documento.Id from DocumentoValor v 
//                            where v.Parametro.Nombre like 'Estado Control' and v.Valor = :est)))
//
//                    and d.Id in 
//                        (select v.Documento.Id from DocumentoValor v 
//                        where v.Parametro.TipoDato like 'Aseguradora'  and v.Valor = :ase)
//
//                    and d.Id in 
//                        (select v.Documento.Id from DocumentoValor v 
//                        where v.Parametro.TipoDato like 'Coche' and v.Valor in (:coches))
//                ";
//            if (equipos != null)
//                hql +=
//                    @" and d.Id in 
//                        (select v.Documento.Id from DocumentoValor v 
//                        where v.Parametro.TipoDato like 'Equipo' and v.Valor in (:equipos)) ";
//            var q = Session.CreateQuery(hql)
//                .SetParameter("ini", inicio)
//                .SetParameter("fin", fin)
//                .SetParameter("ase", aseguradora.ToString())
//                .SetParameter("est", estado.ToString())
//                .SetParameterList("coches", coches);

//            if (equipos != null)
//                q.SetParameterList("equipos", equipos);

//            return q.List();
        }
        public IList FindParteReport(int aseguradora, DateTime inicio, DateTime fin, ICollection coches, int estado)
        {
            return FindParteReport(aseguradora, inicio, fin, coches, estado, null);
        }
        
        public IList FindPartesControladosOVerificados(int aseguradora, DateTime inicio, DateTime fin)
        {
            var q = Session.CreateQuery(
                @"select distinct d from Documento d 
                    where d.Fecha >= :ini
                    and d.Fecha < :fin    

                    and d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.Nombre like 'Estado Control' and v.Valor in ('1', '2'))

                    and d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.TipoDato like 'Aseguradora'  and v.Valor = :ase)
                    and d.Estado != -1
                ")
                .SetParameter("ini", inicio)
                .SetParameter("fin", fin)
                .SetParameter("ase", aseguradora.ToString("#0"));

            return q.List();
        }
        
        public IList FindPartesControlados(int aseguradora, DateTime inicio, DateTime fin)
        {
            var q = Session.CreateQuery(
                @"select distinct d from Documento d 
                    where d.Fecha >= :ini
                    and d.Fecha < :fin    

                    and d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.Nombre like 'Estado Control' and v.Valor = '1')

                    and d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.TipoDato like 'Aseguradora'  and v.Valor = :ase)
                    and d.Estado != -1
                ")
                .SetParameter("ini", inicio)
                .SetParameter("fin", fin)
                .SetParameter("ase", aseguradora.ToString("#0"));

            return q.List();
        }
        
        public IList FindPartes(int aseguradora, DateTime inicio, DateTime fin)
        {
            var q = Session.CreateQuery(
                @"select distinct d from Documento d 
                    where d.Fecha >= :ini
                    and d.Fecha < :fin    

                    and d.Id in 
                        (select v.Documento.Id from DocumentoValor v 
                        where v.Parametro.TipoDato like 'Aseguradora'  and v.Valor = :ase)
                    and d.Estado != -1
                ")
                .SetParameter("ini", inicio)
                .SetParameter("fin", fin)
                .SetParameter("ase", aseguradora.ToString("#0"));

            return q.List();
        }
        
        public IList FindList(int transportista, int linea, int vehiculo, DateTime desde, DateTime hasta, int estado, int equipo,Usuario usuario)
        {
            var script =
                @"select d from Documento d 
                    where d.Fecha >= :ini
                    and d.Fecha <= :fin
                    and d.Estado != -1
                    and d.Id in 
                        (select v0.Documento.Id from DocumentoValor v0 
                        where v0.Parametro.TipoDato like 'Aseguradora'  and v0.Valor = :ase)
                ";

            if(linea > 0)
                script += @" and d.Linea.Id = :linea ";

            if (estado > -1)
                script +=
                    @" and ((:est = '0' and d.Id not in 
                        (select v.Documento.Id from DocumentoValor v 
                            where v.Parametro.Nombre like 'Estado Control' and v.Valor <> :est)
                        ) or (:est <> '0' and d.Id in (select v.Documento.Id from DocumentoValor v 
                            where v.Parametro.Nombre like 'Estado Control' and v.Valor = :est))) ";

            if(linea > 0 && vehiculo > 0)
                script +=
                    @" and d.Id in 
                        (select v1.Documento.Id from DocumentoValor v1 
                        where v1.Parametro.TipoDato like 'Coche' and v1.Valor = :vehiculo) ";

            if (equipo > 0)
                script +=
                    @" and d.Id in 
                        (select v3.Documento.Id from DocumentoValor v3 
                        where v3.Parametro.Nombre like 'Equipo'  and v3.Valor = :equ) ";

            var q = Session.CreateQuery(script);
            q.SetParameter("ini", desde);
            q.SetParameter("fin", hasta);
            q.SetParameter("ase", transportista.ToString("#0"));

            if(linea > 0) q.SetParameter("linea", linea);
            if (estado > -1) q.SetParameter("est", estado.ToString("#0"));
            if (linea > 0 && vehiculo > 0) q.SetParameter("vehiculo", vehiculo.ToString("#0"));
            if(equipo > 0) q.SetParameter("equ", equipo.ToString("#0"));

            if(usuario == null)
            {
                return q.List();
            }

            var transportistaDao = new TransportistaDAO();
            var lineaDao = new LineaDAO();
            var userTransp = transportistaDao.GetList(new [] { -1 }, new [] { linea });
            var userLines = lineaDao.GetList(new[] { -1 });

            return (from Documento d in q.List() 
                    where (transportista != -1 || userTransp.Contains(d.Transportista)) && (linea != -1 || userLines.Contains(d.Linea))
                    select d).ToList();
        }

        public List<Documento> GetListForConsumos(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposVehiculo, IEnumerable<int> vehiculos, DateTime desde, DateTime hasta)
        {
            var q = Query;

            if (!QueryExtensions.IncludesAll(empresas))
                q = q.FilterEmpresa(Session, empresas);
            if (!QueryExtensions.IncludesAll(lineas))
                q = q.FilterLinea(Session, empresas, lineas);
            if (!QueryExtensions.IncludesAll(tiposVehiculo) || !QueryExtensions.IncludesAll(vehiculos))
                q = q.FilterVehiculo(Session, empresas, lineas, new[] { -1 }, new[] { -1 }, new[] { -1 }, tiposVehiculo, vehiculos);

            q = q.Where(c => c.TipoDocumento.ControlaConsumo);
            q = q.Where(c => (c.Fecha >= desde && c.Fecha < hasta) || (c.FechaCierre >= desde && c.FechaCierre < hasta));
            
            var documentos = q.ToList();
            var retorno = new List<Documento>();

            foreach (var documento in documentos)
            {
                var valores = documento.Parametros.Cast<DocumentoValor>();
                valores = valores.Where(v => v.Parametro.TipoDato.Equals("centrocostos") && v.Parametro.Nombre.Equals("Destino")).ToList();

                if (valores.Any())
                    retorno.Add(documento);
            }

            return retorno;
        }

        public override void Delete(Documento obj)
        {
            obj.Estado = -1;
            SaveOrUpdate(obj);
        }

        public DataRow GetDocumentExpirationSummary(int[] tipos, List<int> empresas, List<int> lineas, DateTime hasta)
        {
            var documents = FindByTipo(tipos, empresas, lineas);
            
            var row = new DataTable().NewRow();
            row["1er Aviso"] = documents.Count(d => !d.EnviadoAviso3 && !d.EnviadoAviso2 && d.EnviadoAviso1);
            row["2do Aviso"] = documents.Count(d => !d.EnviadoAviso3 && d.EnviadoAviso2);
            row["Vencidos"] = documents.Count(d => d.EnviadoAviso3);
            row["A vencer"] = documents.Count(d => !d.EnviadoAviso3 && d.Vencimiento.HasValue && d.Vencimiento.Value < hasta);

            return row;
        }
    }
}