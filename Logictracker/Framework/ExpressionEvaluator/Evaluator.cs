#region Usings

using System;
using System.CodeDom.Compiler;
using System.Runtime.Remoting.Contexts;
using System.Text;
using Microsoft.CSharp;

#endregion

namespace Urbetrack.Common.ExpressionEvaluator
{
    #region Public Classes

    /// <summary>
    /// .Net dynamic expressions evaluator.
    /// </summary>
    [Serializable]
    public static class Evaluator
    {
        #region Private Porpierties

        /// <summary>
        /// Name for the dynamic generated class.
        /// </summary>
        private const String DynamicClassName = "_Evaluator";

        /// <summary>
        /// Namespace for the dynamic generated class.
        /// </summary>
        private const String DynamicClassNamespace = "Urbetrack.Common.ExpressionEvaluator";


        #endregion

        #region Public Methods

        /// <summary>
        /// Evaluates the givenn expression.
        /// </summary>
        /// <returns></returns>
        public static Object Evaluate(EvaluatorItem item)
        {
            var loSetup = new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory };
            var evaluationDomain = AppDomain.CreateDomain("ExpressionEvaluationContextDomainfda", null, loSetup);
            try
            {
                evaluationDomain.SetData("item", item);

                evaluationDomain.DoCallBack(EvaluateExpression);

                var result = evaluationDomain.GetData("result");
                
                return result;
            }
            finally
            {
                AppDomain.Unload(evaluationDomain);    
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Evaluates the givenn expression within its own evaluation app domain.
        /// </summary>
        private static void EvaluateExpression()
        {
            var compilerResults = ConstructEvaluator();

            var compiled = ProcessCompilerResults(compilerResults);

            DoEvaluate(compiled);
        }

        /// <summary>
        /// Gets the expresion result.
        /// </summary>
        private static void DoEvaluate(Object compiled)
        {
            var item = (EvaluatorItem)AppDomain.CurrentDomain.GetData("item");

            var mi = compiled.GetType().GetMethod(item.Name);

            var results = mi.Invoke(compiled, null);

            AppDomain.CurrentDomain.SetData("result", results);
        }

        /// <summary>
        /// Generates a dynamic class for evaluating .Net expressions.
        /// </summary>
        private static CompilerResults ConstructEvaluator()
        {
            var item = (EvaluatorItem)AppDomain.CurrentDomain.GetData("item");

            var compilerParameters = GetCompilerParameters();

            var code = GetDynamicClassCode(item);

            var compiler = new CSharpCodeProvider();

            return compiler.CompileAssemblyFromSource(compilerParameters, code);
        }

        /// <summary>
        /// Evaluates the compiler results to save the dynamically generated class or to inform of any compilation errors.
        /// </summary>
        /// <param name="compilerResults"></param>
        private static Object ProcessCompilerResults(CompilerResults compilerResults)
        {
            if (compilerResults.Errors.HasErrors) InformCompilationErrors(compilerResults);

            var assembly = compilerResults.CompiledAssembly;

            return assembly.CreateInstance(String.Format("{0}.{1}", DynamicClassNamespace, DynamicClassName));
        }

        /// <summary>
        /// Informs all compilation error detected.
        /// </summary>
        /// <param name="compilerResults"></param>
        private static void InformCompilationErrors(CompilerResults compilerResults)
        {
            var error = new StringBuilder();

            error.Append("Error Compiling Expression:\n");

            foreach (CompilerError err in compilerResults.Errors) error.AppendFormat("{0}\n", err.ErrorText);

            throw new Exception(String.Concat("Error Compiling Dynamic Class: ", error));
        }

        /// <summary>
        /// Generates the code for the dynamic class to be compiled.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static String GetDynamicClassCode(EvaluatorItem item)
        {
            var code = new StringBuilder();

            AppendClassHeader(code);

            AppendMethod(code, item);

            code.Append("} }");

            return code.ToString();
        }

        /// <summary>
        /// Adds a method to the dynamic class beeing generated.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="item"></param>
        private static void AppendMethod(StringBuilder code, EvaluatorItem item)
        {
            code.AppendFormat("    public {0} {1}() ", item.ReturnType.Name, item.Name);
            code.Append("{ ");
            code.AppendFormat("      return ({0}); ", item.Expression);
            code.Append("}\n");
        }

        /// <summary>
        /// Adds the header of the dynamic class.
        /// </summary>
        /// <param name="code"></param>
        private static void AppendClassHeader(StringBuilder code)
        {
            code.Append("using System; \n");
            code.Append("using System.Data; \n");
            code.Append("using System.Data.SqlClient; \n");
            code.Append("using System.Data.OleDb; \n");
            code.Append("using System.Xml; \n");

            code.Append(String.Format("namespace {0} {{ \n", DynamicClassNamespace));
            code.Append(String.Format("  public class {0} {{ \n", DynamicClassName));
        }

        /// <summary>
        /// Sets up the compiler configuration.
        /// </summary>
        /// <returns></returns>
        private static CompilerParameters GetCompilerParameters()
        {
            var compilerParameters = new CompilerParameters { GenerateExecutable = false, GenerateInMemory = true };

            compilerParameters.ReferencedAssemblies.Add("system.dll");
            compilerParameters.ReferencedAssemblies.Add("system.data.dll");
            compilerParameters.ReferencedAssemblies.Add("system.xml.dll");

            return compilerParameters;
        }

        #endregion
    }

    /// <summary>
    /// .Net dynamic expression.
    /// </summary>
    [Serializable]
    public class EvaluatorItem
    {
        #region Constructors

        /// <summary>
        /// Instanciates a EvaluatorItem using the givenn values within the givenn context.
        /// </summary>
        /// <param name="returnType"></param>
        /// <param name="expression"></param>
        /// <param name="name"></param>
        public EvaluatorItem(Type returnType, String expression, String name)
        {
            ReturnType = returnType;
            Expression = expression;
            Name = name;
        }
        public EvaluatorItem()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The return type of the expression.
        /// </summary>
        public Type ReturnType { get; private set; }

        /// <summary>
        /// The refference name for the expression.
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// The body of the expression.
        /// </summary>
        public String Expression { get; private set; }

        #endregion
    }

    #endregion
}
