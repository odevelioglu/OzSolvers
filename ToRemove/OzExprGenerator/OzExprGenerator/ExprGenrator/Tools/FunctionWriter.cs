using System;
using System.Text;

namespace ExprGenrator
{
    public static class FunctionWriter
    {
        private const int Tab = 4;
        public static string ToCSharpString(this BaseFunction func, int indent = 0)
        {
            var name = func.Name;
            var ind = new string(' ', indent);

            switch (name)
            {
                case "sort":
                    return ind + "void Sort(int[] A)\n" +
                           ind + "{\n" +
                           func.ParamValues[1]?.ToCSharpString(indent + Tab) +
                           (func.Next == null ? "" : "\n" + func.Next.ToCSharpString(indent + Tab)) + "\n" +
                            ind + "}";
                case "list":
                    return ind + "A";
                case "int":
                    return ind + ((IntConstant)func).Value;
                case "swap":
                        return ind + $"Swap(A, {func.ParamValues[1].ToCSharpString()}, {func.ParamValues[2].ToCSharpString()});" +
                               (func.Next == null ? "" : "\n" + func.Next.ToCSharpString(indent)); 
                case "gv":
                    return ind + $"A[{func.ParamValues[1].ToCSharpString()}]";
                case "gt":
                    return ind + $"{func.ParamValues[0].ToCSharpString()} > {func.ParamValues[1].ToCSharpString()}";
                case "if":
                    return ind + $"if({func.ParamValues[0].ToCSharpString()})\n" +
                           ind + "{\n" +
                           $"{func.ParamValues[1].ToCSharpString(indent + Tab)}\n" +
                           ind + "}" +
                           (func.Next == null ? "" : "\n" + func.Next?.ToCSharpString(indent));
                case "ifElse":
                    return ind + $"if({func.ParamValues[0].ToCSharpString()})\n" +
                           ind + "{\n" +
                           $"{func.ParamValues[1].ToCSharpString(indent + Tab)}\n" +
                           ind + "}" +
                           (func.ParamValues[2] != null ?
                           "\n" + ind + "else\n" +
                           ind + "{\n" +
                           $"{func.ParamValues[2]?.ToCSharpString(indent + Tab)}\n" +
                           ind + "}" : "") +
                           (func.Next == null ? "" : "\n" + func.Next?.ToCSharpString(indent));
                    //case "EmptyFunction":
                    //    return "";
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
            
            var name = func.Name;
            if (name == "int") name += " " + ((IntConstant)func).Value;

            //if (name == "sort")
            //    writer.AppendLine(func.Info.HistoryString());

            if (name == "if" || name == "swap" || name == "sort")
                writer.AppendLine(name + "\t\t\t"+ func.Info);
            else
            {
                writer.AppendLine(name);
            }

            for (int i = 0; i < func.ParamValues.Length; i++)
                func.ParamValues[i].ToTreeString(indent, i == func.ParamValues.Length - 1, writer);

            func.Next?.ToTreeString(indent, true, writer);
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
            if (func.Name == "gt" || func.Name == "list" || func.Name == "int") return;

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

            var name = func.Name;
            if (name == "int") name += " " + ((IntConstant)func).Value;

            if (name == "if" || name == "swap" || name == "sort")
                writer.AppendLine(name + "\t\t\t" + func.Info);
            else
            {
                writer.AppendLine(name);
            }

            for (int i = 0; i < func.ParamValues.Length; i++)
                func.ParamValues[i].ToShortTreeString(indent, i == func.ParamValues.Length - 1, writer);

            func.Next?.ToShortTreeString(indent, true, writer);
        }

        public static string CountInfo(this BaseFunction func, int[] theList, Context context)
        {
            var str = new StringBuilder();

            var round = new Func<int, decimal>(p => Math.Round((decimal)p / context.InputStates.Count, 3));
            var pad = new Func<object, string>(o => o.ToString().PadRight(12));

            var counts = func.CountAll();
            //if ((counts.ContainsKey("if") && func.Info.IfCount != counts["if"]) ||
            //    (counts.ContainsKey("swap") && func.Info.SwapCount != counts["swap"]))
            //{
            //    throw new Exception("Counts wrong");
            //}

            str.AppendLine(pad("Function") + pad("Count") + pad("ExecCount") + pad("AvgExeCount") + pad("MinExeCount") + pad("MaxExeCount"));
            str.AppendLine(pad("--------") + pad("-----") + pad("---------") + pad("-----------") + pad("-----------") + pad("-----------"));
            
            var mins = func.CountMinExecutions(theList, context);
            var maxs = func.CountMaxExecutions(theList, context);

            foreach (var kvp in func.CountFunctions(theList, context))
            {
                str.AppendLine(pad(kvp.Key) + pad(counts[kvp.Key]) + pad(kvp.Value) + pad(round(kvp.Value)) + pad(mins[kvp.Key]) + pad(maxs[kvp.Key]));
            }

            str.AppendLine("IfDepth:" + func.GetIfDepth());
            str.AppendLine("TMP IfExec:" + func.Info.IfExecCount);

            //if (func.CountFunctions(theList, context).ContainsKey("if") && func.Info.IfExecCount != func.CountFunctions(theList, context)["if"])
            //{
            //    throw new Exception("Counts wrong");
            //}

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
