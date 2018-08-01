using System;
using System.Collections.Generic;
using System.Linq;


namespace ExprGenrator
{
    public static class ExecutionCounter
    {
        public static Action<BaseFunction> Evaluated;

        public static Dictionary<string, int> CountFunctions(this BaseFunction func, int[] list, Context context)
        {
            var ret = new Dictionary<string, int>();

            Evaluated = f =>
            {
                var name = f.GetType().Name;
                if (ret.ContainsKey(name))
                {
                    ret[name] = ret[name] + 1;
                }
                else
                {
                    ret[name] = 1;
                }
            };

            foreach (var permuted in list.Permute())
            {
                ((ListConstant)context.List).Value = permuted.ToArray();
                func.Eval();
            }

            Evaluated = null;

            return ret;
        }

        public static Dictionary<string, int> CountMinExecutions(this BaseFunction func, int[] list, Context context)
        {
            var mins = new Dictionary<string, int>();
            var ret = new Dictionary<string, int>();

            Evaluated = f =>
            {
                var name = f.GetType().Name;
                if (ret.ContainsKey(name))
                {
                    ret[name] = ret[name] + 1;
                }
                else
                {
                    ret[name] = 1;
                }
            };

            foreach (var permuted in list.Permute())
            {
                ((ListConstant)context.List).Value = permuted.ToArray();
                func.Eval();

                foreach (var kvp in ret)
                {
                    if (!mins.ContainsKey(kvp.Key)) mins[kvp.Key] = int.MaxValue;

                    if (kvp.Value < mins[kvp.Key]) mins[kvp.Key] = kvp.Value;
                }

                ret.Clear();
            }

            Evaluated = null;

            return mins;
        }

        public static Dictionary<string, int> CountMaxExecutions(this BaseFunction func, int[] list, Context context)
        {
            var maxs = new Dictionary<string, int>();
            var ret = new Dictionary<string, int>();

            Evaluated = f =>
            {
                var name = f.GetType().Name;
                if (ret.ContainsKey(name))
                {
                    ret[name] = ret[name] + 1;
                }
                else
                {
                    ret[name] = 1;
                }
            };

            foreach (var permuted in list.Permute())
            {
                ((ListConstant)context.List).Value = permuted.ToArray();
                func.Eval();

                foreach (var kvp in ret)
                {
                    if (!maxs.ContainsKey(kvp.Key)) maxs[kvp.Key] = int.MinValue;

                    if (kvp.Value > maxs[kvp.Key]) maxs[kvp.Key] = kvp.Value;
                }

                ret.Clear();
            }

            Evaluated = null;

            return maxs;
        }
    }
}
