using System;
using System.Text;

namespace ExprGenrator
{
    public static class FunctionWriter
    {
        private const int Tab = 4;
        public static string ToCSharpString(this BaseFunction func, int indent = 0)
        {
            var name = func.GetType().Name;
            var ind = new string(' ', indent);

            switch (name)
            {
                case "SortFunction":
                    return ind + "void Sort(int[] A)\n" +
                           ind + "{\n" +
                           func.ParamValues[1].ToCSharpString(indent + Tab) + "\n" +
                           ind + "}";
                case "BlockFunction":
                    if (func.ParamValues[1] is EmptyFunction)
                        return func.ParamValues[0].ToCSharpString(indent);
                    return func.ParamValues[0].ToCSharpString(indent) + "\n" +
                           func.ParamValues[1].ToCSharpString(indent);
                case "ListConstant":
                    return ind + "A";
                case "IntConstant":
                    return ind + ((IntConstant)func).Value;
                case "SwapFunction":
                    return ind + $"Swap(A, {func.ParamValues[1].ToCSharpString()}, {func.ParamValues[2].ToCSharpString()});";
                case "GetValueFunction":
                    return ind + $"A[{func.ParamValues[1].ToCSharpString()}]";
                case "GreaterThanFunction":
                    return ind + $"{func.ParamValues[0].ToCSharpString()} > {func.ParamValues[1].ToCSharpString()}";
                case "IfFunction":
                    return ind + $"if({func.ParamValues[0].ToCSharpString()})\n" +
                           ind + "{\n" +
                           $"{func.ParamValues[1].ToCSharpString(indent + Tab)}\n" +
                           ind + "}";
                case "EmptyFunction":
                    return "";
            }

            throw new Exception("Unknown type");
        }

        public static string ToTreeString(this BaseFunction func, int indent = 0)
        {
            var builder = new StringBuilder();

            func.ToTreeString("", false, builder);

            return builder.ToString();
        }

        private static void ToTreeString(this BaseFunction func, string indent, bool last, StringBuilder writer)
        {
            if (func == null) return;

            writer.Append(indent);
            if (last)
            {
                writer.Append("\\-");
                indent += "  ";
            }
            else
            {
                writer.Append("|-");
                indent += "| ";
            }

            var name = func.GetType().Name.Replace("Function", "").Replace("Constant", "");
            if (name == "Int") name += " " + ((IntConstant)func).Value;
            writer.AppendLine(name);

            for (int i = 0; i < func.ParamValues.Length; i++)
                func.ParamValues[i].ToTreeString(indent, i == func.ParamValues.Length - 1, writer);
        }

        public static string ToShortTreeString(this BaseFunction func, int indent = 0)
        {
            var builder = new StringBuilder();

            func.ToShortTreeString("", false, builder);

            return builder.ToString();
        }

        private static void ToShortTreeString(this BaseFunction func, string indent, bool last, StringBuilder writer)
        {
            if (func == null) return;

            var name = func.GetType().Name.Replace("Function", "").Replace("Constant", "");
            if (name == "GreaterThan" || name == "List" || name == "Int") return;

            writer.Append(indent);
            if (last)
            {
                writer.Append("\\-");
                indent += "  ";
            }
            else
            {
                writer.Append("|-");
                indent += "| ";
            }
            
            writer.AppendLine(name);

            for (int i = 0; i < func.ParamValues.Length; i++)
                func.ParamValues[i].ToShortTreeString(indent, i == func.ParamValues.Length - 1, writer);
        }

        public static string CountInfo(this BaseFunction func, int[] theList, Context context)
        {
            var str = new StringBuilder();

            var round = new Func<int, decimal>(p => Math.Round((decimal)p / MathTools.Fact(theList.Length), 3));
            var pad = new Func<object, string>(o => o.ToString().PadRight(12));

            var counts = func.CountAll();

            str.AppendLine(pad("Function") + pad("Count") + pad("ExecCount") + pad("AvgExeCount") + pad("MinExeCount") + pad("MaxExeCount"));
            str.AppendLine(pad("--------") + pad("-----") + pad("---------") + pad("-----------") + pad("-----------") + pad("-----------"));

            var mins = func.CountMinExecutions(theList, context);
            var maxs = func.CountMaxExecutions(theList, context);

            foreach (var kvp in func.CountFunctions(theList, context))
            {
                var shortname = kvp.Key.Replace("Function", "").Replace("Constant", "");
                str.AppendLine(pad(shortname) + pad(counts[kvp.Key]) + pad(kvp.Value) + pad(round(kvp.Value)) + pad(mins[kvp.Key]) + pad(maxs[kvp.Key]));
            }

            return str.ToString();
        }

        public static string FullInfo(this BaseFunction func, int[] theList, Context context)
        {
            var str = new StringBuilder();
            str.AppendLine();
            str.AppendLine(func.ToCSharpString());
            str.AppendLine();
            str.AppendLine(func.CountInfo(theList, context));
            str.AppendLine();
            str.AppendLine(func.ToTreeString());
            str.AppendLine();
            str.AppendLine(new string('#', 60));
            str.AppendLine();

            return str.ToString();
        }
    }
}
