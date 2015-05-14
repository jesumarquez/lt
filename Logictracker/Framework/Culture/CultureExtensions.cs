#region Usings

using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Organizacion;

#endregion

namespace Logictracker.Culture
{
    public static class CultureExtensions
    {
        public static string GetDescripcion(this Asegurable asegurable)
        {
            return CultureManager.GetSecurable(asegurable.Referencia);
        }

        public static string GetDescripcion(this Sistema asegurable)
        {
            return CultureManager.GetMenu(asegurable.Descripcion);
        }

        public static string GetDescripcion(this Funcion asegurable)
        {
            return CultureManager.GetSecurable(asegurable.Descripcion);
        }

        public static string GetDescripcionGrupo(this Funcion asegurable)
        {
            return CultureManager.GetSecurable(asegurable.Modulo);
        }
    }
}
