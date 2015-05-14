#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Urbetrack.Compresion
{
    public class RLE
    {
        private class Token
        {
            public int Size { get; set; }
            public byte Data { get; set; }
        }

        private static int DLS(int x)
        {
            if (x <= 9) return 3;
            if (x <= 17) return 4;
            if (x <= 33) return 5;
            if (x <= 65) return 6;
            if (x <= 129) return 7;
            if (x <= 257) return 8;
            if (x <= 513) return 9;
            if (x <= 1025) return 10;
            throw new Exception("RLE: bloques de repeticion de mas de 1024 bytes no implementado.");
        }

        public byte[] Compress(byte[] source, int offset, int src_size)
        {
            var tokens = new Queue<Token>();
            // calculo la maxima repeticion
            var last_byte = source[offset];
            var repeats = 0;
            var max_repeats = 0;
            for(var i = offset+1; i < (offset + src_size); ++i)
            {
                var current = source[i];
                if (last_byte == current)
                {
                    repeats++;
                    if (max_repeats < repeats)
                    {
                        max_repeats = repeats;
                    }
                    continue;
                }
                var tok = new Token
                              {
                                  Size = repeats + 1,
                                  Data = last_byte
                              }; 
                tokens.Enqueue(tok);
                repeats = 0;
                last_byte = current;
            }
            if (repeats > 0) // en caso de repeticion, falta encolar este ultimo bloque
            {
                var las_tok = new Token
                                  {
                                      Size = repeats, // Size 0 significa completar hasta el final del boque
                                      Data = last_byte
                                  };
                tokens.Enqueue(las_tok);
            }

            // ahora codifico los datos.    
            var dls = DLS(max_repeats);
            //Marshall.Info("RLE Comprimiendo: blocks=" + tokens.Count + " dls=" + dls + " source-size=" + src_size);
            var result = new BitStream(8192);
            result.Push(tokens.Count,12);
            result.Push(dls, 4);
            result.Push(src_size, 12);
            foreach(var tok in tokens)
            {
                if (tok.Size == 1)
                {
                    //Marshall.User("RLE: Literal 0x{0:X}", tok.Data);
                    result.Push(false); // indicador de literal.
                    result.Push(tok.Data);
                    continue;
                }
                result.Push(true); // indicador de compresion
                //Marshall.User("RLE: Repetir {0} {1} Veces", tok.Data, tok.Size);
                result.Push(tok.Size, dls);
                result.Push(tok.Data);
            }
            result.Pad();  // completa el ultimo byte con Don't Care Bits.
            return result.GetBuffer();
        }

        public void Clear()
        {
        }
    }
}