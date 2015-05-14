#region Usings

using System;

#endregion

namespace Urbetrack.Common.ExpressionEvaluator.Tester
{
    /// <summary>
    /// Console application for testing the Urbetrack.Common.ExpressionEvaluator.Evaluator
    /// </summary>
    class Test
    {
        /// <summary>
        /// Displays the results of several comparissons.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var time = new TimeSpan(22, 0, 0);

            EvaluatorItem[] items = {
                                        new EvaluatorItem(typeof(int), "(30 + 4) * 2", "GetNumber"),
                                        new EvaluatorItem(typeof(string), "\"Hello \" + \"There\"", "GetString"),
                                        new EvaluatorItem(typeof(bool), "40 == 40", "GetBool"),
                                        new EvaluatorItem(typeof(object), "new DataSet()", "GetDataSet"),
                                        new EvaluatorItem(typeof(bool), "[TotalHours] == 22", "TestMike", time), 
                                    };
      
            var eval = new Evaluator(items);

            Console.WriteLine("Test 0: {0}", eval.EvaluateInt("GetNumber"));
            Console.WriteLine("Test 1: {0}", eval.EvaluateString("GetString"));
            Console.WriteLine("Test 2: {0}", eval.EvaluateBool("GetBool"));
            Console.WriteLine("Test 3: {0}", eval.Evaluate("GetDataSet"));
            Console.WriteLine("Test 4: {0}", eval.Evaluate("TestMike"));

            Console.ReadKey();
        }
    }
}