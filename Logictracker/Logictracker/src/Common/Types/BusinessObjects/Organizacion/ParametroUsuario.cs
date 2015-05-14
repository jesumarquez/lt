namespace Logictracker.Types.BusinessObjects.Organizacion
{
    public class ParametroUsuario
    {
        public virtual int Id { get; set; }
        public virtual Usuario Usuario { get; set;}
        public virtual string Nombre { get; set; }
        public virtual string Valor { get; set; }
    }
}
