using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using System.Collections;

namespace MySolver
{
    public class Searcher2
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

        public Searcher2(Func<int[], double> toSolve, List<VariableInfo> variableInfos, int deep, List<BinaryFunction> functions, List<Constant> constants)
        {
            // funcs[0] functions which take 0 parameters
            // funcs[1] functions which take 1 parameters
            var maxNumberOfVariables = functions.Max(f => f.NumberOfVariables);
            this.funcs = new BinaryFunction[maxNumberOfVariables + 1][];

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

            foreach (var perm in MathHelper.Permut(variableInfos.Select(p => p.Values)))
            {
                n_result[perm] = toSolve(perm);
            }

            this.Deep = deep;
            cache = new List<Node>[deep + 1];
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
                cache[i] = Expand(GetAllNodes(i - 1).ToList());

                if (!this.StopAtFirstResult)
                {
                    foreach (var res in cache[i])
                    {
                        if (VerifyAll(res))
                        {
                            this.FinalResults.Add(res);
                            ResultFound(res);
                        }
                    }

                }

                //Log.Info($"Deep {i} results Replced:");
                //LogNodes(cache[i]);
                //Log.Info("");
            }

            var les4 = GetAllNodes(3).ToList().DistinctNodes(this.n_result.Keys.ToList());
            
            var resExpand = Expand2(les4, les4);
            foreach (var res in resExpand)
            {
                if (VerifyAll(res))
                {
                    this.FinalResults.Add(res);
                    ResultFound(res);
                }
            }

            //this.FinalResults = this.FinalResults.DistinctNodes(this.n_result.Keys.ToList());

            //var resExpand2 = Expand2(resExpand, GetAllNodes(3).ToList());
            //Log.Info("Exopanded2:");
            //LogNodes(resExpand2);
        }

        public List<Node> Expand2(List<Node> basenodes, List<Node> nodes)
        {
            var results = new List<Node>();
            
            foreach (var node in basenodes)
            {
                foreach (var node2 in nodes)
                {
                    foreach (var func in funcs[2])
                    {
                        var tmp = new Node(func, node, node2);
                        if (Searcher.IsValid2(tmp))
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

            return results;
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
                    if (Searcher.IsValid1(tmp))
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
                        if (Searcher.IsValid2(tmp))
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

            Log.Info($"Results Unsorted {counter}:");
            LogNodes(results);
            Log.Info("");

            results.AddRange(GetTerminals());
            
            if (this.counter++ < 2)
            { 
                var distinct = results.DistinctNodes(this.n_result.Keys.ToList());
                
                return distinct;
            }
            else
                return results;
        }

        private int counter = 0;


        private static Constant c10 = new Constant(10);
        private static Constant c100 = new Constant(100);
        public bool IsPeasantValid(Node node)
        {
            if (node.Count(Commons.mul) > 6) return false;
            if (node.Count(Commons.add) > 3) return false;
            if (node.Count(Commons.sub) > 3) return false;

            foreach (var variable in this.variables)
            {
                if (node.Count(variable) > 3) return false;
            }

            if (node.Count(c10) > 1) return false;
            if (node.Count(c100) > 1) return false;

            return true;
        }
    }




}
