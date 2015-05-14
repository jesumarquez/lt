#region Usings

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Logictracker.ExpressionEvaluator
{
    /// <summary>
    /// Expression compiler using the C# language
    /// </summary>
    public class CSharpExpressionCompiler
    {
        /// <summary>
        /// Compiles the expression into an assembly and returns the method code for it.
        /// It should compile the method into a class that inherits from the functionType.
        /// </summary>
        /// <param name="expression">expression to be compiled</param>
        /// <param name="functionType">Type of the function class to use</param>
        /// <param name="returnType">Return type of the method to create</param>
        /// <returns>DynamicMethodState - A serialized version of the method code</returns>
        public DynamicMethodState CompileExpression(string expression, Type functionType, Type returnType)
        {
            DynamicMethodState methodState;

            //use CodeDom to compile using C#
            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");

            var loParameters = new CompilerParameters();

            //add assemblies
            loParameters.ReferencedAssemblies.Add("System.dll");
            loParameters.ReferencedAssemblies.Add(functionType.Assembly.Location);
            //don't generate assembly on disk and treat warnings as errors
            loParameters.GenerateInMemory = true;
            loParameters.TreatWarningsAsErrors = true;

            //set namespace of dynamic class
            string dynamicNamespace = "ExpressionEval.Functions.Dynamic";

            //set source for inherited class - need to change to use CodeDom objects instead
            string source = @"
{7}
using {5};

namespace {6}
{{
    public class {0} : {1}
    {{
        public {2} {3}()
        {{
            return {4};
        }}
    }}
}}
";

            //set source code replacements
            string className = "Class_" + Guid.NewGuid().ToString("N");
            string methodName = "Method_" + Guid.NewGuid().ToString("N");
            string returnTypeName = returnType.FullName;

            //check for generic type for return
            if (returnType.IsGenericType)
            {
                //check for nullable
                Type genericType = returnType.GetGenericTypeDefinition();
                if (genericType == typeof(Nullable<>))
                {
                    //nullable so add ?
                    Type nullableType = Nullable.GetUnderlyingType(returnType);
                    returnTypeName = nullableType.FullName + "?";
                }
                else
                {
                    //not nullable but is generic so get the list of types
                    Type[] genericArgTypes = returnType.GetGenericArguments();

                    //get type name without the last 2 characters for generic type names
                    returnTypeName = genericType.FullName.Substring(0, genericType.FullName.Length - 2) + "<";

                    //loop through type arguments and build out return type
                    foreach (Type genericArgType in genericArgTypes)
                    {
                        returnTypeName += genericArgType.FullName;
                    }

                    //add ending generic operator
                    returnTypeName += ">";
                }
            }

            //so System namespace is not repeated
            var systemNs = functionType.Namespace != "System" ? "using System;" : string.Empty;
            //format codestring with replacements
            var codeString = string.Format(source, className, functionType.FullName, returnTypeName, methodName, expression, functionType.Namespace, dynamicNamespace, systemNs);

            //compile the code
            var results = codeProvider.CompileAssemblyFromSource(loParameters, codeString);

            if (results.Errors.Count > 0)
            {
                //throw an exception for any errors
                throw new CompileException(results.Errors);
            }
        	//get the type that was compiled
        	var dynamicType = results.CompiledAssembly.GetType(dynamicNamespace + "." + className);

        	//get the MethodInfo for the compiled expression
        	var dynamicMethod = dynamicType.GetMethod(methodName);

        	//get the compiled expression as serializable object
        	methodState = GetMethodState(dynamicMethod);

        	return methodState;
        }

        /// <summary>
        /// Converts a MethodInfo into a serialized version of it.
        /// </summary>
        /// <param name="dynamicMethod">The method for which to create a DynamicMethod for</param>
        /// <returns>DynamicMethodState - serialized version of a method.</returns>
        protected DynamicMethodState GetMethodState(MethodInfo dynamicMethod)
        {
            var methodState = new DynamicMethodState();

            //IL info from method
            var methodIlCode = dynamicMethod.GetMethodBody();

            //get code bytes and other method properties
            methodState.CodeBytes = methodIlCode.GetILAsByteArray();
            methodState.InitLocals = methodIlCode.InitLocals;
            methodState.MaxStackSize = methodIlCode.MaxStackSize;

            //get any local variable information
            IDictionary<int, LocalVariable> locals = new SortedList<int, LocalVariable>();

            foreach (var localInfo in methodIlCode.LocalVariables)
            {
                locals.Add(localInfo.LocalIndex, new LocalVariable(localInfo.IsPinned, localInfo.LocalType.TypeHandle));
            }

            methodState.LocalVariables = locals;

            var tokenOffset = new TokenOffset();

            //get metadata token offsets
            var reader = new IlReader(methodState.CodeBytes, dynamicMethod.Module);

            tokenOffset.Fields = reader.Fields;
            tokenOffset.Methods = reader.Methods;
            tokenOffset.Types = reader.Types;
            tokenOffset.LiteralStrings = reader.LiteralStrings;

            methodState.TokenOffset = tokenOffset;

            return methodState;
        }
    }
}
