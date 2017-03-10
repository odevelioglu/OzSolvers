using System.Collections.Generic;
using System.Linq;
using System.Collections;
using log4net;

namespace MySolver
{
    using System;

    public static class NodeHelpers
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(NodeHelpers));

        public static List<Node> DistinctNodes(this List<Node> nodes, List<int[]> vars)
        {
            var results = new List<Node>();

            nodes.SortNodes(vars);

            //Log.Info("Sorted Nodes");
            //foreach (var result in nodes)
            //{
            //    Log.Info(result + "    Evals: " + vars.Select(p => $"f({p.JoinCsv()})={result.Eval(p)}").JoinCsv());
            //}
            //Log.Info("");

            var cnt = nodes.Count;

            for (var i = 1; i < cnt; i++)
            {
                if (nodes[i].Eq(nodes[i - 1], vars))
                {
                    //Log.Info($"Found equal: {nodes[i - 1]} and {nodes[i]}");
                    continue;
                    // Don't pickup the last. Select the nicest
                }

                results.Add(nodes[i - 1]);
            }

            results.Add(nodes[cnt - 1]);

            //Log.Info("Filtered Nodes");
            //foreach (var result in results)
            //{
            //    Log.Info(result);
            //}
            //Log.Info("");

            return results;
        }

        public static void SortNodes(this List<Node> nodes, List<int[]> vars)
        {
            var comparer = new NodeComparer(vars);
            nodes.Sort(comparer);
        }

        public static string JoinCsv(this IEnumerable objects)
        {
            return string.Join(", ", objects.OfType<object>().Select(o => (o ?? string.Empty).ToString()));
        }

        public static int Count(this Node node, IOzExpression expr)
        {
            int count = 0;
            Visit(node, n =>
            {
                if (n.Expression.SemEq(expr)) count++;
            });

            return count;
        }

        public static void Visit(this Node node, Action<Node> action)
        {
            if (node == null) return;

            action(node);

            Visit(node.Left, action);
            Visit(node.Right, action);
        }
    }

    public class NodeComparer : IComparer<Node>
    {
        private readonly List<int[]> vars;

        public NodeComparer(List<int[]> vars)
        {
            this.vars = vars;
        }

        public int Compare(Node x, Node y)
        {
            var consLeft = x.Expression as Constant;
            var consRight = y.Expression as Constant;

            if (consLeft != null && consRight != null)
            {
                var left = x.Eval(null);
                var right = y.Eval(null);

                if (left.Gt(right)) return 1; //x is greater
                if (right.Gt(left)) return -1;

                return 0;
            }

            foreach (var vari in this.vars)
            {
                var left = x.Eval(vari);
                var right = y.Eval(vari);

                if (left.Gt(right)) return 1; //x is greater
                if (right.Gt(left)) return -1;
            }

            return 0;
        }
    }
}
