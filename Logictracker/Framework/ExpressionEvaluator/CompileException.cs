#region Usings

using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

#endregion

namespace Logictracker.ExpressionEvaluator
{
    /// <summary>
    /// A custom exception that will be thrown when there is a compile exception when using CodeDom
    /// </summary>
    [Serializable]
    public class CompileException : ApplicationException
    {
        private readonly CompilerErrorCollection _mErrors;

        protected CompileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _mErrors = (CompilerErrorCollection)info.GetValue("Errors", typeof(CompilerErrorCollection));
        }

        public CompileException(string message) : base(message)
        {
        }

        public CompileException(CompilerErrorCollection errors)
        {
            _mErrors = errors;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            
            info.AddValue("Errors", _mErrors);
        }

        public CompilerErrorCollection CompileErrors
        {
            get { return _mErrors; }
        }
    }
}
