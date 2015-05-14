namespace Tarjetas
{
    public partial class empleado
    {
        public bool TieneFoto
        {
            get { return foto != null && foto.Length > 1; }
            set { }
        }
    }
}
