using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Organizacion
{
    [Serializable]
    public class UsuarioVo
    {
        public const int IndexNombreUsuario = 0;
        public const int IndexTipo = 1;
        public const int IndexClient = 2;
        public const int IndexPerfiles = 3;
        public const int IndexEmpleado = 4;

        public int Id { get; set; }

        [GridMapping(Index = IndexNombreUsuario, ResourceName = "Labels", VariableName = "NAME", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string NombreUsuario { get; set; }

        [GridMapping(Index = IndexTipo, ResourceName = "Labels", VariableName = "TYPE", IncludeInSearch = false)]
        public string Tipo { get; set; }

        [GridMapping(Index = IndexClient, ResourceName = "Labels", VariableName = "GRUPO_RECURSOS", IncludeInSearch = true)]
        public string Client { get; set; }

        [GridMapping(Index = IndexPerfiles, ResourceName = "Labels", VariableName = "PERFILES", IncludeInSearch = true, Width = "50%")]
        public string Perfiles { get; set; }

        [GridMapping(Index = IndexEmpleado, ResourceName = "Entities", VariableName = "PARENTI09", IncludeInSearch = true)]
        public string Empleado { get; set; }

        public bool Inhabilitado { get; set; }

        public UsuarioVo(Usuario usuario)
        {
            Id = usuario.Id;
            NombreUsuario = usuario.NombreUsuario;
            Tipo = GetTipoDescription(usuario.Tipo);
            Client = usuario.Client;
            var perfiles = string.Empty;
            foreach (Perfil perfil in usuario.Perfiles)
            {
                if (!perfiles.Equals(string.Empty)) perfiles = perfiles + ", ";

                perfiles = perfiles + perfil.Descripcion;
            }
            Perfiles = perfiles;
            Empleado = usuario.Empleado != null && usuario.Empleado.Entidad != null ? usuario.Empleado.Entidad.Descripcion : string.Empty;
            Inhabilitado = usuario.Inhabilitado;
        }

        /// <summary>
        /// Gets the user T associated description.
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        private static string GetTipoDescription(short tipo)
        {
            var description = string.Empty;
            switch (tipo)
            {
                case Usuario.NivelAcceso.NoAccess: description = "USERTYPE_NOACCESS"; break;
                case Usuario.NivelAcceso.Public: description = "USERTYPE_PUBLIC"; break;
                case Usuario.NivelAcceso.User: description = "USERTYPE_USER"; break;
                case Usuario.NivelAcceso.Installer: description = "USERTYPE_INSTALLER"; break;
                case Usuario.NivelAcceso.Developer: description = "USERTYPE_DEVELOPER"; break;
                case Usuario.NivelAcceso.AdminUser: description = "USERTYPE_ADMIN_USER"; break;
                case Usuario.NivelAcceso.SysAdmin: description = "USERTYPE_SYS_ADMIN"; break;
                case Usuario.NivelAcceso.SuperAdmin: description = "USERTYPE_SUPER_ADMIN"; break;
            }
            return CultureManager.GetUser(description);
        }
    }
}
