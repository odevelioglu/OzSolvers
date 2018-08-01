using System;
using System.Collections.Generic;
using System.Text;

namespace ExprGenrator
{
    public static class FunctionVisitor
    {
        public static BaseFunction GetTop(this BaseFunction func)
        {
            var curr = func;

            while (curr.Parent != null)
            {
                curr = curr.Parent;
            }

            return curr;
        }

        public static void Visit(this BaseFunction func, Action<BaseFunction> action)
        {
            if (func == null) return;

            action(func);

            foreach (var param in func.ParamValues)
            {
                param.Visit(action);
            }

            func.Next?.Visit(action);
        }

        public static int Count(this BaseFunction func, string name)
        {
            var count = 0;
            
            var curr = func;

            while (curr != null)
            {
                Visit(curr, f =>
                {
                    if (f.Name == name)
                        count++;
                });

                curr = curr.Parent;
            }

            return count;
        }


        public static StringBuilder builder = new StringBuilder();
        public static string UniqueKey(this BaseFunction func)
        {
            builder.Clear();

            var curr = func;

            while (curr != null)
            {
                Visit(curr, f =>
                {
                    var name = f.Name;
                    if (name == "if" || name == "swap" || name == "block")
                    { 
                        builder.Append(name[0]);
                        if(f.ParamValues != null)
                            builder.Append(",");
                    }
                });

                builder.Append('^');
                curr = curr.Parent;
            }

            return builder.ToString();
        }

        public static Dictionary<string, int> CountAll(this BaseFunction func)
        {
            var ret = new Dictionary<string, int>();

            var curr = func;

            while (curr != null)
            {
                Visit(curr, f =>
                {
                    var name = f.Name;
                    if (ret.ContainsKey(name))
                    {
                        ret[name] = ret[name] + 1;
                    }
                    else
                    {
                        ret[name] = 1;
                    }
                });

                curr = curr.Parent;
            }

            return ret;
        }

        public static int GetIfDepth(this BaseFunction func)
        {
            var max = 0;

            var top = func.GetTop();

            Visit(top, f =>
            {
                var count = 0;
                var curr = f as IfFunction;
                while (curr != null)
                {
                    count++;
                    curr = curr.Parent as IfFunction;
                }

                if (count > max) max = count;
            });

            return max;
        }
        
        public static int[] GetParams(this IfFunction ifFunc)
        {
            var ret = new int[2];

            var thisGt = ifFunc.ParamValues[0] as GreaterThanFunction;

            if (thisGt == null) return null;

            var thisleft = thisGt.ParamValues[0].ParamValues[1] as IntConstant;

            ret[0] = thisleft.Value;
            var thisright = thisGt.ParamValues[1].ParamValues[1] as IntConstant;
            ret[1] = thisright.Value;

            return ret;
        }

        public static bool Equivalent(this SwapFunction one, SwapFunction another)
        {
            if (one.ParamValues[1] == another.ParamValues[1] && one.ParamValues[2] == another.ParamValues[2])
                return true;

            if (one.ParamValues[1] == another.ParamValues[2] && one.ParamValues[2] == another.ParamValues[1])
                return true;

            return false;
        }
    }
}
