#region Usings

using System;
using System.Reflection;

#endregion

namespace Logictracker.ExpressionEvaluator
{
    /// <summary>
    /// Delegate returned by an expression delegate factory.
    /// </summary>
    /// <typeparam name="R">The return type of the method built on the expression</typeparam>
    /// <typeparam name="C">The type of the function class</typeparam>
    /// <param name="functionClass">An instance of the function class the method is built against</param>
    /// <returns>R - an instance of the return type for the expression</returns>
    public delegate R ExecuteExpression<R, C>(C functionClass);

    /// <summary>
    /// Delegate returned by an expression evaluator.
    /// </summary>
    /// <typeparam name="R">The return type of the method built on the expression</typeparam>
    /// <typeparam name="C">The type of the function class</typeparam>
    /// <param name="functionClass">An instance of the function class the method is built against</param>
    /// <returns>R - an instance of the return type for the expression</returns>
    internal delegate R EvalExpression<R, C>(C functionClass);

    /// <summary>
    /// Implementation of IExpressionEvaluator
    /// </summary>
    public class ExpressionEvaluator
    {
        public static T Evaluate<T>(string expression)
        {
            var eval = new ExpressionEvaluator();
            return eval.Evaluate<T, Object>(expression, new Object());
        }

        /// <summary>
        /// Evaluates an expression and returns the result value.
        /// </summary>
        /// <typeparam name="R">The return type of the expression</typeparam>
        /// <typeparam name="C">The type of the function class</typeparam>
        /// <param name="expression">Expression to evaluate</param>
        /// <param name="functionClass">An instance of the function class the method is built against</param>
        /// <returns>R - an instance of the return type for the expression</returns>
        internal R Evaluate<R, C>(string expression, C functionClass)
        {
            var result = default(R);

            //get delegate for expression
            var methodDelegate = GetDelegate<R, C>(expression);

            if (methodDelegate != null)
            {
                //get result
                result = methodDelegate(functionClass);
            }
            
            return result;
        }

        /// <summary>
        /// Compiles an expression and returns a delegate to the compiled code.
        /// </summary>
        /// <typeparam name="R">The return type of the expression</typeparam>
        /// <typeparam name="C">The type of the function class</typeparam>
        /// <param name="expression">Expression to evaluate</param>
        /// <returns>EvalExpression&lt;R, C&gt; - a delegate that calls the compiled expression</returns>
        internal EvalExpression<R, C> GetDelegate<R, C>(string expression)
        {
            EvalExpression<R, C> methodDelegate = null;

            //get compiled method
            var methodState = GetMethodState<R, C>(expression);

            if (methodState != null)
            {
                //Get delegate for method state
                methodDelegate = GetDelegate<R, C>(methodState);
            }

            return methodDelegate;
        }

        /// <summary>
        /// Compiles an expression and returns a DynamicMethodState
        /// </summary>
        /// <typeparam name="R">The return type of the expression</typeparam>
        /// <typeparam name="C">The type of the function class</typeparam>
        /// <param name="expression">Expression to evaluate</param>
        /// <returns>DynamicMethodState - serialized version of the compiled expression</returns>
        internal DynamicMethodState GetMethodState<R, C>(string expression)
        {
        	//get delegate factory
            var delegateFactory = new ExpressionDelegateFactory();

            //check the function class type to be sure it can be inherited
            Type functionType = typeof(C);

            if (!functionType.IsClass || functionType.IsSealed)
            {
                throw new ApplicationException("Function Type must be a class that is not sealed.");
            }

            //the implementation of the compiler requires an empty constructor
            ConstructorInfo[] infos = functionType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            bool hasEmptyConstructor = false;
            foreach (ConstructorInfo info in infos)
            {
                if (info.GetParameters().Length == 0 && info.IsStatic == false && info.IsPrivate == false)
                {
                    hasEmptyConstructor = true;
                }
            }

            if (!hasEmptyConstructor)
            {
                throw new ApplicationException("Function Type must be have a parameterless constructor defined.");
            }

            //get the method state
            var methodState = delegateFactory.CreateExpressionMethodState<R, C>(expression);

            return methodState;
        }

        /// <summary>
        /// Compiles a DynamicMethodState and returns a delegate.
        /// </summary>
        /// <typeparam name="R">The return type of the expression</typeparam>
        /// <typeparam name="C">The type of the function class</typeparam>
        /// <param name="methodState">The serialized version of a method on the functionClass</param>
        /// <returns>EvalExpression&lt;R, C&gt; - a delegate that calls the compiled expression</returns>
        internal EvalExpression<R, C> GetDelegate<R, C>(DynamicMethodState methodState)
        {
            ExecuteExpression<R, C> methodDelegate = null;

            //get delegate factory
            var delegateFactory = new ExpressionDelegateFactory();

            if (methodState != null && methodState.CodeBytes != null)
            {
                //get delegate from factory
                methodDelegate = delegateFactory.CreateExpressionDelegate<R, C>(methodState);
            }

            //return an eval delegate based on the delegate from the factory
            return new EvalExpression<R, C>(methodDelegate);
        }
    }
}
