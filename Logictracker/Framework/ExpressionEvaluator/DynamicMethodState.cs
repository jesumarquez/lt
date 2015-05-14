#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Logictracker.ExpressionEvaluator
{
    /// <summary>
    /// A serialized version of a method.  Holds everything to create a DynamicMethod.
    /// </summary>
    [Serializable]
    public class DynamicMethodState
    {
        /// <summary>
        /// The IL bytes from compilation
        /// </summary>
        public byte[] CodeBytes;

        /// <summary>
        /// Whether or not local variables are initialized
        /// </summary>
        public bool InitLocals;

        /// <summary>
        /// The maximum size of the stack required for this method
        /// </summary>
        public int MaxStackSize;

        /// <summary>
        /// Local variables defined for the method
        /// </summary>
        public IDictionary<int,LocalVariable> LocalVariables;

        /// <summary>
        /// Definition of tokens that need to be resolved
        /// </summary>
        public TokenOffset TokenOffset;

    }

    /// <summary>
    /// Defines a local variable for a method
    /// </summary>
    [Serializable]
    public class LocalVariable
    {
        /// <summary>
        /// Whether or not the variable is pinned in memory, used for working with unmanaged memory
        /// </summary>
        public bool IsPinned;

        /// <summary>
        /// Type of the variable
        /// </summary>
        public RuntimeTypeHandle LocalType;

        public LocalVariable(bool isPinned, RuntimeTypeHandle localType)
        {
            IsPinned = isPinned;
            LocalType = localType;
        }


    }

    /// <summary>
    /// A definition of tokens to resolve for given offsets in IL code bytes
    /// </summary>
    [Serializable]
    public class TokenOffset
    {
        /// <summary>
        /// Fields to be resolved
        /// </summary>
        public IDictionary<int, RuntimeFieldHandle> Fields;

        /// <summary>
        /// Method to be resolved
        /// </summary>
        public IDictionary<int, MethodBase> Methods;

        /// <summary>
        /// Types to be resolved
        /// </summary>
        public IDictionary<int, RuntimeTypeHandle> Types;

        /// <summary>
        /// Literal strings to resolve
        /// </summary>
        public IDictionary<int, string> LiteralStrings;
    }
}
