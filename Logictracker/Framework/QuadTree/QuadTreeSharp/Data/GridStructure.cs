#region Usings

using System;
using System.Runtime.InteropServices;

#endregion

namespace Logictracker.QuadTree.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GridStructure
    {
		/// <summary>
		/// Descripcion/Identificacion del respositorio
		/// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string Signature;

		/// <summary>
		/// Cantidad de bits de resolicion por celda (actualmente fijo 4)
		/// </summary>
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public char CellBits;

		/// <summary>
		/// Código de activación = 0x55AA55AA
		/// </summary>
        [MarshalAs(UnmanagedType.U8, SizeConst = 8)]
        public ulong ActiveKey;

		/// <summary>
		/// Inicio de datos de las zonas en sectores, nunca menor a 1
		/// </summary>
        [MarshalAs(UnmanagedType.U8, SizeConst = 8)]
        public ulong DataStart;

		/// <summary>
		/// Cantidad de Sectores por Fila, debe ser mayor o igual a (Lon_GridCount / 32) teniendo en cuenta el remanente
		/// </summary>
        [MarshalAs(UnmanagedType.U8, SizeConst = 8)]
        public ulong FileSectorCount;

		/// <summary>
		/// OffSet de Latitud en 1/10,000,000 de Grados
		/// </summary>
        [MarshalAs(UnmanagedType.I8, SizeConst = 8)]
        public long Lat_OffSet;
 
		/// <summary>
		/// OffSet de Longitud en 1/10,000,000 de Grados
		/// </summary>
        [MarshalAs(UnmanagedType.I8, SizeConst = 8)]
        public long Lon_OffSet; /*  */ 

		/// <summary>
		/// Tamaño de la Grilla de Latitud en 1/10,000,000 de Grados
		/// </summary>
        [MarshalAs(UnmanagedType.U8, SizeConst = 8)]
        public ulong Lat_Grid;

		/// <summary>
		/// Tamaño de la Grilla de Longitud en 1/10,000,000 de Grados
		/// </summary>
        [MarshalAs(UnmanagedType.U8, SizeConst = 8)]
        public ulong Lon_Grid;

		/// <summary>
		/// Cantidad de Grillas en Latitud
		/// </summary>
        [MarshalAs(UnmanagedType.U8, SizeConst = 8)]
        public ulong Lat_GridCount;

		/// <summary>
		/// Cantidad de Grillas en Longitud
		/// </summary>
        [MarshalAs(UnmanagedType.U8, SizeConst = 8)]
        public ulong Lon_GridCount;

		//compatibilidad con headers gr2
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
		public string SignatureOld;
		[MarshalAs(UnmanagedType.U1, SizeConst = 1)]
		public char Status;
		[MarshalAs(UnmanagedType.I4, SizeConst = 4)]
		public int Revision;
		[MarshalAs(UnmanagedType.I2, SizeConst = 2)]
		public short base_lat;
		[MarshalAs(UnmanagedType.I2, SizeConst = 2)]
		public short base_lon;
	}

	public static class GridStructureX
	{
		public static int GetCountSectors(this GridStructure me)
		{
			return (int) (Math.Ceiling(me.Lat_GridCount/32.0)*Math.Ceiling(me.Lon_GridCount/32.0));
		}
	}
}