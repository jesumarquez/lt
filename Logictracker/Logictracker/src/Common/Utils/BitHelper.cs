#region Usings

using System;
using System.Collections;
using System.Linq;
using System.Text;

#endregion

namespace Logictracker.Utils
{
    /// <summary>
    /// Helps perform certain operations on primative types
    /// that deal with bits
    /// </summary>
    public static class BitHelper
    {
        #region Private Properties

        /// <summary>
        /// The max number of bits in byte
        /// </summary>
        private const short BIT_SIZE_BYTE = 8;

        /// <summary>
        /// The max number of bits in short 
        /// </summary>
        private const short BIT_SIZE_SHORT = 16;

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether [is bit set] [the specified p input].
        /// </summary>
        /// <param name="pInput">The p input.</param>
        /// <param name="pPosition">The p position.</param>
        /// <returns>
        /// 	<c>true</c> if [is bit set] [the specified p input]; otherwise, <c>false</c>.
        /// </returns>
        //public static bool IsBitSet (byte pInput, int pPosition) { return GetBits(pInput, pPosition,1, false) == 1; }
        public static bool IsBitSet(byte pInput, int pPosition)
        {
            return (pInput & (1 << pPosition)) != 0;
        }

        public static bool AreBitsSet (byte input, int mask) { return (input & mask) == mask; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a number in the specified range of bits
        /// </summary>
        /// <param name="pInput"></param>
        /// <param name="pLength"></param>
        /// <param name="pShift"></param>
        /// <param name="pStartIndex"></param>
        /// <returns></returns>
        public static byte GetBits(byte pInput, int pStartIndex, int pLength, bool pShift)
        {
            if (pInput < 2 && pInput > 0) return pInput; //Should be either a 0 or 1
            
            var lSize = SizeOf(pInput);

            if (pStartIndex < 1 || pStartIndex > BIT_SIZE_SHORT) throw new ArgumentException("Start bit is out of range.", "pStartIndex");
            
            if (pLength < 0 || pLength + pStartIndex > BIT_SIZE_BYTE + 1) throw new ArgumentException("End bit is out of range.", "pLength");


            byte lRetval = 0;
            var lPosition = 1;

            for (var i = pStartIndex; (i < pLength + pStartIndex) && (lPosition <= lSize); i++)
            {
                var lTemp = 1 << i - 1;

                if ((pInput & lTemp) == lTemp) lRetval |= (byte)(1 << (lPosition - 1));

                lPosition++;
            }

            if (pShift && lPosition < lSize) lRetval <<= lSize - lPosition;

            return lRetval;
        }

        /// <summary>
        /// Gets the size of the input value in bits
        /// </summary>
        /// <param name="pInput">The input value</param>
        /// <returns></returns>
        private static short SizeOf(byte pInput)
        {
            if (pInput == 0) return 0;

            if (pInput == 1) return 1;

            if (pInput < 0) return BIT_SIZE_BYTE;

            for (short i = BIT_SIZE_BYTE - 1; i > 1; i--)
            {
                var lTemp = 1 << i - 1;

                if ((pInput & lTemp) != lTemp) continue;

                return i;
            }

            return BIT_SIZE_BYTE;
        }

        #endregion
    }

    /// <summary>
    ///     Converts base data types to an array of bytes, and an array of bytes to base
    ///     data types.
    ///     All info taken from the meta data of System.BitConverter. This implementation
    ///     allows for Endianness consideration.
    ///</summary>
    public static class BitConverter_BigEndian
    {
        #region Public Methods
		
		///
        /// <summary>
        ///     Returns the specified Boolean value as an array of bytes.
        ///
        /// Parameters:
        ///   value:
        ///     A Boolean value.
        ///
        /// Returns:
        ///     An array of bytes with length 1.
        ///</summary>
        public static byte[] GetBytes(bool value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }

        ///
        /// <summary>
        ///     Returns the specified Unicode character value as an array of bytes.
        ///
        /// Parameters:
        ///   value:
        ///     A character to convert.
        ///
        /// Returns:
        ///     An array of bytes with length 2.
        ///</summary>
        public static byte[] GetBytes(char value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }

        ///
        /// <summary>
        ///     Returns the specified double-precision floating point value as an array of
        ///     bytes.
        ///
        /// Parameters:
        ///   value:
        ///     The number to convert.
        ///
        /// Returns:
        ///     An array of bytes with length 8.
        ///</summary>
        public static byte[] GetBytes(double value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }

        ///
        /// <summary>
        ///     Returns the specified single-precision floating point value as an array of
        ///     bytes.
        ///
        /// Parameters:
        ///   value:
        ///     The number to convert.
        ///
        /// Returns:
        ///     An array of bytes with length 4.
        ///</summary>
        public static byte[] GetBytes(float value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }

        ///
        /// <summary>
        ///     Returns the specified 32-bit signed integer value as an array of bytes.
        ///
        /// Parameters:
        ///   value:
        ///     The number to convert.
        ///
        /// Returns:
        ///     An array of bytes with length 4.
        ///</summary>
        public static byte[] GetBytes(int value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }

        ///
        /// <summary>
        ///     Returns the specified 64-bit signed integer value as an array of bytes.
        ///
        /// Parameters:
        ///   value:
        ///     The number to convert.
        ///
        /// Returns:
        ///     An array of bytes with length 8.
        ///</summary>
        public static byte[] GetBytes(long value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }

        ///
        /// <summary>
        ///     Returns the specified 16-bit signed integer value as an array of bytes.
        ///
        /// Parameters:
        ///   value:
        ///     The number to convert.
        ///
        /// Returns:
        ///     An array of bytes with length 2.
        ///</summary>
        public static byte[] GetBytes(short value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }

        ///
        /// <summary>
        ///     Returns the specified 32-bit unsigned integer value as an array of bytes.
        ///
        /// Parameters:
        ///   value:
        ///     The number to convert.
        ///
        /// Returns:
        ///     An array of bytes with length 4.
        ///</summary>
        //[CLSCompliant(false)]
        public static byte[] GetBytes(uint value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }

        ///
        /// <summary>
        ///     Returns the specified 64-bit unsigned integer value as an array of bytes.
        ///
        /// Parameters:
        ///   value:
        ///     The number to convert.
        ///
        /// Returns:
        ///     An array of bytes with length 8.
        ///</summary>
        //[CLSCompliant(false)]
        public static byte[] GetBytes(ulong value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }

        ///
        /// <summary>
        ///     Returns the specified 16-bit unsigned integer value as an array of bytes.
        ///
        /// Parameters:
        ///   value:
        ///     The number to convert.
        ///
        /// Returns:
        ///     An array of bytes with length 2.
        ///</summary>
        public static byte[] GetBytes(ushort value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }

        ///
        /// <summary>
        ///     Converts the specified 64-bit signed integer to a double-precision floating
        ///     point number.
        ///
        /// Parameters:
        ///   value:
        ///     The number to convert.
        ///
        /// Returns:
        ///     A double-precision floating point number whose value is equivalent to value.
        ///</summary>
        public static double Int64BitsToDouble(long value) { throw new NotImplementedException(); }
        
		///
        /// <summary>
        ///     Returns a Boolean value converted from one byte at a specified position in
        ///     a byte array.
        ///
        /// Parameters:
        ///   value:
        ///     An array of bytes.
        ///
        ///   startIndex:
        ///     The starting position within value.
        ///
        /// Returns:
        ///     true if the byte at startIndex in value is nonzero; otherwise, false.
        ///
        /// Exceptions:
        ///   System.ArgumentNullException:
        ///     value is null.
        ///
        ///   System.ArgumentOutOfRangeException:
        ///     startIndex is less than zero or greater than the length of value minus 1.
        ///</summary>
        public static bool ToBoolean(byte[] value, int startIndex) { throw new NotImplementedException(); }
        
		///
        /// <summary>
        ///     Returns a Unicode character converted from two bytes at a specified position
        ///     in a byte array.
        ///
        /// Parameters:
        ///   value:
        ///     An array.
        ///
        ///   startIndex:
        ///     The starting position within value.
        ///
        /// Returns:
        ///     A character formed by two bytes beginning at startIndex.
        ///
        /// Exceptions:
        ///   System.ArgumentException:
        ///     startIndex equals the length of value minus 1.
        ///
        ///   System.ArgumentNullException:
        ///     value is null.
        ///
        ///   System.ArgumentOutOfRangeException:
        ///     startIndex is less than zero or greater than the length of value minus 1.
        ///</summary>
        public static char ToChar(byte[] value, int startIndex) { throw new NotImplementedException(); }
        
		///
        /// <summary>
        ///     Returns a double-precision floating point number converted from eight bytes
        ///     at a specified position in a byte array.
        ///
        /// Parameters:
        ///   value:
        ///     An array of bytes.
        ///
        ///   startIndex:
        ///     The starting position within value.
        ///
        /// Returns:
        ///     A double precision floating point number formed by eight bytes beginning
        ///     at startIndex.
        ///
        /// Exceptions:
        ///   System.ArgumentException:
        ///     startIndex is greater than or equal to the length of value minus 7, and is
        ///     less than or equal to the length of value minus 1.
        ///
        ///   System.ArgumentNullException:
        ///     value is null.
        ///
        ///   System.ArgumentOutOfRangeException:
        ///     startIndex is less than zero or greater than the length of value minus 1.
        ///</summary>
        public static double ToDouble(byte[] value, int startIndex) { throw new NotImplementedException(); }
        
		///
        /// <summary>
        ///     Returns a 16-bit signed integer converted from two bytes at a specified position
        ///     in a byte array.
        ///
        /// Parameters:
        ///   value:
        ///     An array of bytes.
        ///
        ///   startIndex:
        ///     The starting position within value.
        ///
        /// Returns:
        ///     A 16-bit signed integer formed by two bytes beginning at startIndex.
        ///
        /// Exceptions:
        ///   System.ArgumentException:
        ///     startIndex equals the length of value minus 1.
        ///
        ///   System.ArgumentNullException:
        ///     value is null.
        ///
        ///   System.ArgumentOutOfRangeException:
        ///     startIndex is less than zero or greater than the length of value minus 1.
        ///</summary>
        public static short ToInt16(byte[] value, int startIndex)
        {
            return BitConverter.ToInt16(value.Reverse().ToArray(), value.Length - sizeof(Int16) - startIndex);
        }

		///
		/// <summary>
		///     Returns a 32-bit signed integer converted from four bytes at a specified
		///     position in a byte array.
		///
		/// Parameters:
		///   value:
		///     An array of bytes.
		///
		///   startIndex:
		///     The starting position within value.
		///
		/// Returns:
		///     A 32-bit signed integer formed by four bytes beginning at startIndex.
		///
		/// Exceptions:
		///   System.ArgumentException:
		///     startIndex is greater than or equal to the length of value minus 3, and is
		///     less than or equal to the length of value minus 1.
		///
		///   System.ArgumentNullException:
		///     value is null.
		///
		///   System.ArgumentOutOfRangeException:
		///     startIndex is less than zero or greater than the length of value minus 1.
		///</summary>
		public static int ToInt32(byte[] value, int startIndex)
		{
			return BitConverter.ToInt32(value.Reverse().ToArray(), value.Length - sizeof(Int32) - startIndex);
		}

		///
        /// <summary>
        ///     Returns a 64-bit signed integer converted from eight bytes at a specified
        ///     position in a byte array.
        ///
        /// Parameters:
        ///   value:
        ///     An array of bytes.
        ///
        ///   startIndex:
        ///     The starting position within value.
        ///
        /// Returns:
        ///     A 64-bit signed integer formed by eight bytes beginning at startIndex.
        ///
        /// Exceptions:
        ///   System.ArgumentException:
        ///     startIndex is greater than or equal to the length of value minus 7, and is
        ///     less than or equal to the length of value minus 1.
        ///
        ///   System.ArgumentNullException:
        ///     value is null.
        ///
        ///   System.ArgumentOutOfRangeException:
        ///     startIndex is less than zero or greater than the length of value minus 1.
        ///</summary>
        public static long ToInt64(byte[] value, int startIndex)
        {
            return BitConverter.ToInt64(value.Reverse().ToArray(), value.Length - sizeof(Int64) - startIndex);
        }

        ///
        /// <summary>
        ///     Returns a single-precision floating point number converted from four bytes
        ///     at a specified position in a byte array.
        ///
        /// Parameters:
        ///   value:
        ///     An array of bytes.
        ///
        ///   startIndex:
        ///     The starting position within value.
        ///
        /// Returns:
        ///     A single-precision floating point number formed by four bytes beginning at
        ///     startIndex.
        ///
        /// Exceptions:
        ///   System.ArgumentException:
        ///     startIndex is greater than or equal to the length of value minus 3, and is
        ///     less than or equal to the length of value minus 1.
        ///
        ///   System.ArgumentNullException:
        ///     value is null.
        ///
        ///   System.ArgumentOutOfRangeException:
        ///     startIndex is less than zero or greater than the length of value minus 1.
        ///</summary>
        public static float ToSingle(byte[] value, int startIndex)
        {
            return BitConverter.ToSingle(value.Reverse().ToArray(), value.Length - sizeof(Single) - startIndex);
        }

        ///
        /// <summary>
        ///     Converts the numeric value of each element of a specified array of bytes
        ///     to its equivalent hexadecimal string representation.
        ///
        /// Parameters:
        ///   value:
        ///     An array of bytes.
        ///
        /// Returns:
        ///     A System.String of hexadecimal pairs separated by hyphens, where each pair
        ///     represents the corresponding element in value; for example, "7F-2C-4A".
        ///
        /// Exceptions:
        ///   System.ArgumentNullException:
        ///     value is null.
        ///</summary>
        public static string ToString(byte[] value)
        {
            return BitConverter.ToString(value.Reverse().ToArray());
        }

        ///
        /// <summary>
        ///     Converts the numeric value of each element of a specified subarray of bytes
        ///     to its equivalent hexadecimal string representation.
        ///
        /// Parameters:
        ///   value:
        ///     An array of bytes.
        ///
        ///   startIndex:
        ///     The starting position within value.
        ///
        /// Returns:
        ///     A System.String of hexadecimal pairs separated by hyphens, where each pair
        ///     represents the corresponding element in a subarray of value; for example,
        ///     "7F-2C-4A".
        ///
        /// Exceptions:
        ///   System.ArgumentNullException:
        ///     value is null.
        ///
        ///   System.ArgumentOutOfRangeException:
        ///     startIndex is less than zero or greater than the length of value minus 1.
        ///</summary>
        public static string ToString(byte[] value, int startIndex)
        {
            return BitConverter.ToString(value.Reverse().ToArray(), startIndex);
        }

        ///
        /// <summary>
        ///     Converts the numeric value of each element of a specified subarray of bytes
        ///     to its equivalent hexadecimal string representation.
        ///
        /// Parameters:
        ///   value:
        ///     An array of bytes.
        ///
        ///   startIndex:
        ///     The starting position within value.
        ///
        ///   length:
        ///     The number of array elements in value to convert.
        ///
        /// Returns:
        ///     A System.String of hexadecimal pairs separated by hyphens, where each pair
        ///     represents the corresponding element in a subarray of value; for example,
        ///     "7F-2C-4A".
        ///
        /// Exceptions:
        ///   System.ArgumentNullException:
        ///     value is null.
        ///
        ///   System.ArgumentOutOfRangeException:
        ///     startIndex or length is less than zero.  -or- startIndex is greater than
        ///     zero and is greater than or equal to the length of value.
        ///
        ///   System.ArgumentException:
        ///     The combination of startIndex and length does not specify a position within
        ///     value; that is, the startIndex parameter is greater than the length of value
        ///     minus the length parameter.
        ///</summary>
        public static string ToString(byte[] value, int startIndex, int length)
        {
            return BitConverter.ToString(value.Reverse().ToArray(), startIndex, length);
        }

        ///
        /// <summary>
        ///     Returns a 16-bit unsigned integer converted from two bytes at a specified
        ///     position in a byte array.
        ///
        /// Parameters:
        ///   value:
        ///     The array of bytes.
        ///
        ///   startIndex:
        ///     The starting position within value.
        ///
        /// Returns:
        ///     A 16-bit unsigned integer formed by two bytes beginning at startIndex.
        ///
        /// Exceptions:
        ///   System.ArgumentException:
        ///     startIndex equals the length of value minus 1.
        ///
        ///   System.ArgumentNullException:
        ///     value is null.
        ///
        ///   System.ArgumentOutOfRangeException:
        ///     startIndex is less than zero or greater than the length of value minus 1.
        ///</summary>
        public static ushort ToUInt16(byte[] value, int startIndex) 
        {
            return BitConverter.ToUInt16(value.Reverse().ToArray(), value.Length - sizeof(UInt16) - startIndex);
        }

		///
		/// <summary>
		///     Returns a 24-bit unsigned integer converted from 3 bytes at a specified
		///     position in a byte array.
		///
		/// Parameters:
		///   value:
		///     An array of bytes.
		///
		///   startIndex:
		///     The starting position within value.
		///
		/// Returns:
		///     A 24-bit unsigned integer formed by 3 bytes beginning at startIndex.
		///
		/// Exceptions:
		///   System.ArgumentException:
		///     startIndex is greater than or equal to the length of value minus 3, and is
		///     less than or equal to the length of value minus 1.
		///
		///   System.ArgumentNullException:
		///     value is null.
		///
		///   System.ArgumentOutOfRangeException:
		///     startIndex is less than zero or greater than the length of value minus 1.
		///</summary>
		public static uint ToUInt24(byte[] value, int startIndex)
		{
			var bytes = new byte[]{0,0,0,0};
			Array.Copy(value, startIndex, bytes, 1, 3);
			bytes = bytes.Reverse().ToArray();
			return BitConverter.ToUInt32(bytes, 0);
		}

		/// <summary>
		///     Returns a 32-bit unsigned integer converted from four bytes at a specified
		///     position in a byte array.
		///
		/// Parameters:
		///   value:
		///     An array of bytes.
		///
		///   startIndex:
		///     The starting position within value.
		///
		/// Returns:
		///     A 32-bit unsigned integer formed by four bytes beginning at startIndex.
		///
		/// Exceptions:
		///   System.ArgumentException:
		///     startIndex is greater than or equal to the length of value minus 3, and is
		///     less than or equal to the length of value minus 1.
		///
		///   System.ArgumentNullException:
		///     value is null.
		///
		///   System.ArgumentOutOfRangeException:
		///     startIndex is less than zero or greater than the length of value minus 1.
		///</summary>
		public static uint ToUInt32(byte[] value, int startIndex)
		{
			return BitConverter.ToUInt32(value.Reverse().ToArray(), value.Length - sizeof(UInt32) - startIndex);
		}

        ///
        /// <summary>
        ///     Returns a 64-bit unsigned integer converted from eight bytes at a specified
        ///     position in a byte array.
        ///
        /// Parameters:
        ///   value:
        ///     An array of bytes.
        ///
        ///   startIndex:
        ///     The starting position within value.
        ///
        /// Returns:
        ///     A 64-bit unsigned integer formed by the eight bytes beginning at startIndex.
        ///
        /// Exceptions:
        ///   System.ArgumentException:
        ///     startIndex is greater than or equal to the length of value minus 7, and is
        ///     less than or equal to the length of value minus 1.
        ///
        ///   System.ArgumentNullException:
        ///     value is null.
        ///
        ///   System.ArgumentOutOfRangeException:
        ///     startIndex is less than zero or greater than the length of value minus 1.
        ///</summary>
        public static ulong ToUInt64(byte[] value, int startIndex) 
        {
            return BitConverter.ToUInt64(value.Reverse().ToArray(), value.Length - sizeof(UInt64) - startIndex);
        }

		#endregion
	}

    public static class BitHelperExtension
    {
        public static BitArray To32Bits(this BitArray ba)
        {
            if (ba.Length < 32 && 32 - ba.Length > 0)
                return ba.Prepend(new BitArray(32 - ba.Length));
            return ba;
        }

        public static BitArray ToBinary(this int numeral)
        {
            return new BitArray(new[] { numeral });
        }

        public static int ToNumeral(this BitArray ba)
        {
            if (ba == null)
                throw new ArgumentNullException("binary");
            if (ba.Length > 32)
                throw new ArgumentException("must be at most 32 bits long");

            int value = 0;

            for (int i = 0; i < ba.Count; i++)
            {
                if (ba[i])
                    value += Convert.ToInt32(Math.Pow(2, i));
            }

            return value;
        }

        public static int ReverseToNumeral(this BitArray ba)
        {
            if (ba == null)
                throw new ArgumentNullException("binary");
            if (ba.Length > 32)
                throw new ArgumentException("must be at most 32 bits long");

            int value = 0;

            for (int i = ba.Count - 1; i > -1; i--)
            {
                if (ba[i])
                    value += Convert.ToInt32(Math.Pow(2, i));
            }

            return value;
        }

        public static BitArray Trim(this BitArray source, int size)
        {
            var dest = new BitArray(size);

            for (int i = 0; i < size; ++i)
            {
                dest[i] = source[i];
            }

            return dest;
        }

        public static BitArray Prepend(this BitArray current, BitArray before)
        {
            var bools = new bool[current.Count + before.Count];
            before.CopyTo(bools, 0);
            current.CopyTo(bools, before.Count);
            return new BitArray(bools);
        }

        public static BitArray Append(this BitArray current, Byte after)
        {
            return current.Append(new BitArray(new[] { after }));
        }

        public static BitArray Append(this BitArray current, BitArray after)
        {
            var bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }
        public static byte HighBits(this byte b, int bits)
        {
            // not tested
            var bits2Move = 8 - bits;
            return (byte)(b >> bits2Move);
            //return (byte)(((byte)(b >> bits2Move)) << bits2Move);

        }

        public static byte LowBits(this byte b, int bits)
        {
            var bits2Move = 8 - bits;
            return (byte)(((byte)(b << bits2Move)) >> bits2Move);
        }

        public static BitArray SubBits(this byte b, int startIndex, int bits)
        {
            var array = new BitArray(new[] { b });
            var ret = new BitArray(bits);
            for (int i = startIndex; i < startIndex + bits; i++)
			{
			    ret[i-startIndex] = array[i];
			}
            return ret;
        }

        public static string ToBitString(this BitArray bits)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < bits.Count; i++)
            {
                char c = bits[i] ? '1' : '0';
                sb.Insert(0, c);
            }

            return sb.ToString();
        }
    }
}