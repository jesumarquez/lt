#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion

namespace Logictracker.ExpressionEvaluator
{
    /// <summary>
    /// Resolves tokens for given code bytes and a DynamicILInfo
    /// </summary>
    public class IlTokenResolver
    {
        private readonly IDictionary<int, RuntimeFieldHandle> m_fields;
        private readonly IDictionary<int, MethodBase> m_methods;
        private readonly IDictionary<int, RuntimeTypeHandle> m_types;
        private readonly IDictionary<int, string> m_literalStrings;

        /// <summary>
        /// Constructor that takes all the tokens to resolve
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="methods"></param>
        /// <param name="types"></param>
        /// <param name="literalStrings"></param>
        public IlTokenResolver(IDictionary<int, RuntimeFieldHandle> fields, IDictionary<int, MethodBase> methods, IDictionary<int, RuntimeTypeHandle> types, IDictionary<int, string> literalStrings)
        {
            m_fields = fields;
            m_methods = methods;
            m_types = types;
            m_literalStrings = literalStrings;

        }

        /// <summary>
        /// Resolves the tokens given the code bytes and DynamicILInfo class
        /// </summary>
        /// <param name="code"></param>
        /// <param name="dynamicInfo"></param>
        /// <returns>byte[] - code bytes with resolved tokens</returns>
        public byte[] ResolveCodeTokens(byte[] code, DynamicILInfo dynamicInfo)
        {
            var resolvedCode = new byte[code.Length];

            //copy code bytes
            Array.Copy(code, resolvedCode, code.Length);

            //resolve fields
            foreach (int offset in m_fields.Keys)
            {
                int newMetadataToken = dynamicInfo.GetTokenFor(m_fields[offset]);

                OverwriteInt32(resolvedCode, offset, newMetadataToken);
            }

            //resolve methods
            foreach (int offset in m_methods.Keys)
            {
                int newMetadataToken;

            	var methodBase = m_methods[offset];
                
                //generic types require the declaring type when resolving
                if (methodBase.DeclaringType != null && methodBase.DeclaringType.IsGenericType)
                {
                    newMetadataToken = dynamicInfo.GetTokenFor(methodBase.MethodHandle, methodBase.DeclaringType.TypeHandle);
                }
                else
                {
                    newMetadataToken = dynamicInfo.GetTokenFor(methodBase.MethodHandle);
                }

                OverwriteInt32(resolvedCode, offset, newMetadataToken);
            }

            //resolve types
            foreach (int offset in m_types.Keys)
            {
                int newMetadataToken = dynamicInfo.GetTokenFor(m_types[offset]);

                OverwriteInt32(resolvedCode, offset, newMetadataToken);
            }

            //resolve strings
            foreach (int offset in m_literalStrings.Keys)
            {
                int newMetadataToken = dynamicInfo.GetTokenFor(m_literalStrings[offset]);

                OverwriteInt32(resolvedCode, offset, newMetadataToken);
            }

            return resolvedCode;
        }

        /// <summary>
        /// Method to overwrite an int value with another within code bytes.
        /// </summary>
        /// <param name="code">code bytes</param>
        /// <param name="offset">byte index</param>
        /// <param name="tokenValue">value to write</param>
        private static void OverwriteInt32(byte[] code, int offset, int tokenValue)
        {
            code[offset++] = (byte)tokenValue;
            code[offset++] = (byte)(tokenValue >> 8);
            code[offset++] = (byte)(tokenValue >> 16);
            code[offset] = (byte)(tokenValue >> 24);
        }
    }
}
