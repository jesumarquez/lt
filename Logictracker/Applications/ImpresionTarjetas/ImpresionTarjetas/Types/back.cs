namespace Tarjetas
{
    public partial class back
    {
        public static class Legajo
        {
            public const int Top = 1760;
            public const int Left = 120;
        }
        public static class Documento
        {
            public const int Top = 2160;
            public const int Left = 120;
        }
        public static class Upcode
        {
            public const int Top = 3460;
            public const int Left = 120;
            public const int OffsetLabelTop = -220;
            public const int OffsetLabelLeft = 120;
        }
        public static class Nombre
        {
            public const int Top = 4260;
            public const int Left = 1270;
        }
        public static class Apellido
        {
            public const int Top = 3840;
            public const int Left = 1270;
        }
        public static class Foto
        {
            public const int Top = 1540;
            public const int Left = 1410;
        }

        public string ShowName { get { return nombre + (active.HasValue && active.Value ? "*" : ""); } }

        partial void OnCreated()
        {
            legajo_left = Legajo.Left;
            legajo_top = Legajo.Top;
            documento_left = Documento.Left;
            documento_top = Documento.Top;
            upcode_left = Upcode.Left;
            upcode_top = Upcode.Top;
            nombre_left = Nombre.Left;
            nombre_top = Nombre.Top;
            apellido_left = Apellido.Left;
            apellido_top = Apellido.Top;
            foto_left = Foto.Left;
            foto_top = Foto.Top;
        }
    }
}
