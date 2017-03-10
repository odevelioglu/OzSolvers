using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using System.Collections;

namespace MySolver
{
    public class Searcher
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Searcher));
        private BinaryFunction[][] funcs;
        private List<Constant> constants;
        private List<Variable> variables = new List<Variable>();
        public Dictionary<int[], double> n_result = new Dictionary<int[], double>();
        public static List<Node>[] cache;
        public int Deep { get; set; }
        public bool StopAtFirstResult { get; set; }

        public int NumberOfVerifiedSolutions { get; set; }

        public delegate void ResultFoundHandler(Node n);

        public event ResultFoundHandler ResultFound;
        
        public Searcher(Func<int[], double> toSolve, List<VariableInfo> variableInfos, int deep, List<BinaryFunction> functions, List<Constant> constants)
        {
            // funcs[0] functions which take 0 parameters
            // funcs[1] functions which take 1 parameters
            var maxNumberOfVariables = functions.Max(f => f.NumberOfVariables);
            this.funcs = new BinaryFunction[maxNumberOfVariables+1][];

            for (int i = 0; i <= maxNumberOfVariables; i++)
            {
                this.funcs[i] = functions.Where(f => f.NumberOfVariables == i).ToArray();
            }
            
            this.constants = constants;

            for (var i = 0; i < variableInfos.Count; i++)
            {
                var info = variableInfos[i];
                variables.Add(new Variable(info.Name, i));
            }
            
            foreach (var perm in MathHelper.Permut(variableInfos.Select(p=>p.Values)))
            {
                n_result[perm] = toSolve(perm);
            }
            
            this.Deep = deep;
            cache = new List<Node>[deep+1];
        }

        public List<Node> FinalResults = new List<Node>();

        private void LogNodes(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                Log.Info(node);
            }
        }

        public void Search()
        {
            cache[1] = GetTerminals();

            for (var i = 2; i <= Deep; i++) // deep
            {
                cache[i] = Expand(GetAllNodes(i-1).ToList());

                if (!this.StopAtFirstResult)
                {
                    foreach (var res in cache[i])
                    {
                        if (VerifyAll(res)) ResultFound(res);
                    }
                    
                }

                //Log.Info($"Deep {i} results Replced:");
                //LogNodes(cache[i]);
                //Log.Info("");
            }
        }

        private static List<Node> terminalsCache;
        private List<Node> GetTerminals()
        {
            if (terminalsCache != null) return terminalsCache;

            terminalsCache = new List<Node>();

            terminalsCache.AddRange(this.variables.Select(n => new Node(n, null, null)));
            terminalsCache.AddRange(this.constants.Select(c => new Node(c, null, null)));

            return terminalsCache;
        }

        private bool VerifyAll(Node node)
        {
            NumberOfVerifiedSolutions++;
            
            foreach (var kvp in n_result)
            {
                if (!node.Eval(kvp.Key).Eq(kvp.Value))
                {
                    return false;
                }
            }
            
            return true;
        }

        private IEnumerable<Node> GetAllNodes(int deep)
        {
            for (int i = 1; i <= deep; i++)
            {
                foreach (var node in cache[i])
                {
                    yield return node;
                }
            }
        }

        public List<Node> Expand(List<Node> nodes)
        {
            var results = new List<Node>();
            
            foreach (var func in funcs[1])
            {
                foreach (var node in nodes)
                {
                    var tmp = new Node(func, node, null);
                    if (IsValid1(tmp))
                    {
                        results.Add(tmp);

                        if (this.StopAtFirstResult)
                        {
                            if (VerifyAll(tmp))
                            {
                                this.FinalResults.Add(tmp);
                                return new List<Node>();
                            }
                        }
                    }
                }
            }

            foreach (var func in funcs[2])
            {
                foreach (var node in nodes)
                {
                    foreach (var node2 in nodes)
                    {
                        var tmp = new Node(func, node, node2);
                        if (IsValid2(tmp))
                        {
                            results.Add(tmp);

                            if (this.StopAtFirstResult)
                            {
                                if (VerifyAll(tmp))
                                {
                                    this.FinalResults.Add(tmp);
                                    return new List<Node>();
                                }
                            }
                        }
                    }
                }
            }

            //Log.Info($"Results Unsorted {counter}:");
            //LogNodes(results);
            //Log.Info("");

            results.AddRange(GetTerminals());
            if(this.counter++ < 3)
                return results.DistinctNodes(this.n_result.Keys.ToList());
            else
                return results;
        }

        private int counter = 0;

        //public IEnumerable<Node> GenNodes(int deep)
        //{
        //    if (deep <= 0) yield break;

        //    foreach (var n in variables)
        //    {
        //        var tmp = new Node(n, null, null, this.n_result.Keys);
        //        //if (IsValid(tmp))
        //            yield return tmp;
        //    }

        //    foreach (var c in constants)
        //    {
        //        var tmp = new Node(c, null, null, this.n_result.Keys);
        //        //if (IsValid(tmp))
        //            yield return tmp;
        //    }

        //    foreach (var func in funcs)
        //    {
        //        foreach (var node in GenNodes(deep - 1))
        //        {
        //            foreach (var node2 in GenNodes(deep - 1))
        //            {
        //                var tmp = new Node(func, node, node2, this.n_result.Keys);
        //                if (IsValid(tmp))
        //                {
        //                    yield return tmp;
        //                }
        //            }
        //        }
        //    }
        //}

        private static Node c1 = new Node(new Constant(1), null, null);
        private static Node c0 = new Node(new Constant(0), null, null);

        public static bool IsValid1(Node theNode)
        {
            var n = theNode.Left;
            var func = theNode.Expression as BinaryFunction;
            if (func == null) return true;

            switch (func.name)
            {
                case "log":
                    // log(0), log(1)
                    if (n.SemEq(c1) || n.SemEq(c0)) return false;
                    
                    break;
                case "neg":
                    // c, v
                    var tmp = n?.Expression as BinaryFunction;
                    if (tmp != null) return false;

                    break;
            }

            return true;
        }

        public static bool IsValid2(Node theNode)
        {
            //Log.Info(theNode);

            var func = theNode.Expression as BinaryFunction;
            if (func == null) return true; // Variables and constants are all good

            var left = theNode.Left;
            var right = theNode.Right;
            
            // func(c, c)
            var leftCons = left.Expression as Constant;
            var rightCons = right.Expression as Constant;
            if (leftCons != null && rightCons != null)
                return false;
            
            var func1 = left.Expression as BinaryFunction;
            var func2 = right.Expression as BinaryFunction;
            
            switch (func.name)
            {
                case "div":
                    // E / 1
                    if (right.SemEq(c1)) return false;
                    
                    // E / E
                    if (left.SemEq(right)) return false;

                    // 0 / E
                    if (left.SemEq(c0)) return false;

                    if(func1?.name == "mul")
                    { 
                        // (n * E) / E
                        if(left.Right.SemEq(right)) return false;

                        // (E * n) / E
                        if (left.Left.SemEq(right)) return false;
                    }

                    // E / E ^ n
                    if (func2?.name == "pow" && right.Left.SemEq(left)) return false;

                    // E ^ n / E
                    if (func1?.name == "pow" && left.Left.SemEq(right)) return false;

                    // (E / n) / E
                    if (func1?.name == "div" && left.Left.SemEq(right)) return false;

                    // E / (E / n) 
                    if (func2?.name == "div" && right.Left.SemEq(left)) return false;
                    
                    break;
                case "mul":
                    // (N / E) * E
                    if (func1?.name == "div" && left.Right.SemEq(right)) return false;

                    // E * (N / E)
                    if (func2?.name == "div" && right.Right.SemEq(left)) return false;

                    // E * 1, E * 0
                    if (right.SemEq(c1) || right.SemEq(c0)) return false;

                    // 1 * E, 0 * E
                    if (left.SemEq(c1) || left.SemEq(c0)) return false;

                    // E * E
                    //if (n1.SemEq(n2)) return false; //?

                    break;
                case "add":
                    // 0 + E
                    if (left.SemEq(c0)) return false;

                    // E + 0
                    if (right.SemEq(c0)) return false;

                    // (n - E) + E
                    if (func1?.name == "sub" && left.Right.SemEq(right)) return false;

                    // E + (n - E)
                    if (func2?.name == "sub" && right.Right.SemEq(left)) return false;

                    // E + E
                    //if (n1.SemEq(n2)) return false; //?

                    break;
                case "sub":
                    // E - 0
                    if (right.SemEq(c0)) return false;

                    // E - E
                    if (left.SemEq(right)) return false;
                    
                    if (func1?.name == "add")
                    {
                        // (n + E) - E
                        if (left.Right.SemEq(right)) return false;

                        // (E + n) - E
                        if (left.Left.SemEq(right)) return false;
                    }
                    
                    if (func2?.name == "add")
                    {
                        // E - (n + E)
                        if (right.Right.SemEq(left)) return false;

                        // E - (E + n)
                        if (right.Left.SemEq(left)) return false;
                    }

                    break;
                case "pow":
                    // 1 ^ E, 0 ^ E
                    if (left.SemEq(c1) || left.SemEq(c0)) return false;

                    // E ^ 1, E ^ 0
                    if (right.SemEq(c1) || right.SemEq(c0)) return false;

                    break;
            }

            return true;
        }


    }
    
    public class VariableInfo
    {
        public string Name { get; set; }
        public int[] Values { get; set; }

        public VariableInfo(string name, int[] values)
        {
            this.Name = name;
            this.Values = values;
        }
    }

    
    
}
