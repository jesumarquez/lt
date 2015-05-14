#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion

namespace Logictracker.ExpressionEvaluator
{
    /// <summary>
    /// Reads the Intermediate Language from a byte array and the module the method belongs to.
    /// Parses out the Field, Method, Type, and Literal String tokens.
    /// </summary>
    public class IlReader
    {
        private static OpCode[] m_multiByteOpCodes;
        private static OpCode[] m_singleByteOpCodes;

        //token offset dictionaries
        private readonly IDictionary<int, RuntimeFieldHandle> m_fields = new Dictionary<int, RuntimeFieldHandle>();
        private readonly IDictionary<int, MethodBase> m_methods = new Dictionary<int, MethodBase>();
        private readonly IDictionary<int, RuntimeTypeHandle> m_types = new Dictionary<int, RuntimeTypeHandle>();
        private readonly IDictionary<int, string> m_literalStrings = new Dictionary<int, string>();

        static IlReader()
        {
            LoadOpCodes();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code">byte code</param>
        /// <param name="module">module of method</param>
        public IlReader(byte[] code, Module module)
        {
            ReadCode(code, module);
        }

        #region Code From Other Articles Refactored

        //Original Author
        //'Parsing the IL of a Method Body' - by Sorin Serban    
        //http://www.codeproject.com/csharp/sdilreader.asp
        //I refactored here a little

        private static void LoadOpCodes()
        {
            m_singleByteOpCodes = new OpCode[0x100];
            m_multiByteOpCodes = new OpCode[0x100];
            var opCodeFields = typeof(OpCodes).GetFields();
            foreach (var opCodeField in opCodeFields)
            {
            	if (opCodeField.FieldType != typeof (OpCode)) continue;
            	var opCode = (OpCode)opCodeField.GetValue(null);
            	var opCodeValue = (ushort)opCode.Value;
            	if (opCodeValue < 0x100)
            	{
            		m_singleByteOpCodes[opCodeValue] = opCode;
            	}
            	else
            	{
            		m_multiByteOpCodes[opCodeValue & 0xff] = opCode;
            	}
            }
        }

        private void ReadCode(byte[] code, Module module)
        {
            int byteIndex = 0;
            while (byteIndex < code.Length)
            {
                OpCode opCode;

                ushort opValue = code[byteIndex++];

                if (opValue != 0xfe)
                {
                    opCode = m_singleByteOpCodes[opValue];
                }
                else
                {
                    opValue = code[byteIndex++];
                    opCode = m_multiByteOpCodes[opValue];
                }

                int metadataToken;
                int startingIndex = byteIndex;

                switch (opCode.OperandType)
                {
                    case OperandType.InlineField:
                        metadataToken = ReadInt32(code, ref byteIndex);
                        m_fields.Add(startingIndex, module.ResolveField(metadataToken).FieldHandle);
                        break;
                    case OperandType.InlineMethod:
                        metadataToken = ReadInt32(code, ref byteIndex);
                        m_methods.Add(startingIndex, module.ResolveMethod(metadataToken));
                        break;
                    case OperandType.InlineType:
                        metadataToken = ReadInt32(code, ref byteIndex);
                        m_types.Add(startingIndex, module.ResolveType(metadataToken).TypeHandle);
                        break;
                    case OperandType.InlineString:
                        metadataToken = ReadInt32(code, ref byteIndex);
                        m_literalStrings.Add(startingIndex, module.ResolveString(metadataToken));
                        break;
                    case OperandType.InlineBrTarget:
                        byteIndex = byteIndex + 4;
                        break;
                    case OperandType.InlineSig:
                        byteIndex = byteIndex + 4;
                        break;
                    case OperandType.InlineTok:
                        metadataToken = ReadInt32(code, ref byteIndex);

                        var memberInfo = module.ResolveMember(metadataToken);

                        switch (memberInfo.MemberType)
                        {
                        	case MemberTypes.NestedType:
                        	case MemberTypes.TypeInfo:
                        		{
                        			var type = memberInfo as Type;
                        			m_types.Add(startingIndex, type.TypeHandle);
                        		}
                        		break;
                        	case MemberTypes.Constructor:
                        	case MemberTypes.Method:
                        		{
                        			var m = memberInfo as MethodBase;
                        			m_methods.Add(startingIndex, m);
                        		}
                        		break;
                        	case MemberTypes.Field:
                        		{
                        			var f = memberInfo as FieldInfo;
                        			m_fields.Add(startingIndex, f.FieldHandle);
                        		}
                        		break;
                        }

                        break;
                    case OperandType.InlineI:
                        byteIndex = byteIndex + 4;
                        break;
                    case OperandType.InlineI8:
                        byteIndex = byteIndex + 8;
                        break;
                    case OperandType.InlineR:
                        byteIndex = byteIndex + 8;
                        break;
                    case OperandType.InlineSwitch:
                        int count = ReadInt32(code, ref byteIndex);
                        byteIndex = byteIndex + (count * 4);
                        break;
                    case OperandType.InlineVar:
                        byteIndex = byteIndex + 2;
                        break;
                    case OperandType.ShortInlineBrTarget:
                        byteIndex = byteIndex + 1;
                        break;
                    case OperandType.ShortInlineI:
                        byteIndex = byteIndex + 1;
                        break;
                    case OperandType.ShortInlineR:
                        byteIndex = byteIndex + 4;
                        break;
                    case OperandType.ShortInlineVar:
                        byteIndex = byteIndex + 1;
                        break;
                }

            }
        }

        private static int ReadInt32(byte[] code, ref int byteIndex)
        {
            return (((code[byteIndex++] | (code[byteIndex++] << 8)) | (code[byteIndex++] << 0x10)) | (code[byteIndex++] << 0x18));
        }

        #endregion

        public IDictionary<int, RuntimeFieldHandle> Fields
        {
            get { return m_fields; }
        }

        public IDictionary<int, MethodBase> Methods
        {
            get { return m_methods; }
        }

        public IDictionary<int, RuntimeTypeHandle> Types
        {
            get { return m_types; }
        }

        public IDictionary<int, string> LiteralStrings
        {
            get { return m_literalStrings; }
        }
    }
}
