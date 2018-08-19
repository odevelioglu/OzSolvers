using System;
using System.Collections.Generic;
using System.Linq;

namespace ExprGenrator
{
    // case embedded 1:
    // ...
    // if() 
    // {
    //    swap();
    
    // case Next 1:
    // ...
    // swap();
    // swap();

    // case Next 2:
    // ...
    // if() {...}
    // swap();
    public class SwapFunctionExpression : IExpression
    {
        public int Max { get; set; } = int.MaxValue;
        public int MaxConsecutive { get; set; }
        public SwapFunctionExpression(int max = int.MaxValue)
        {
            this.Max = max;
        }
        public List<Type> ParameterTypes => new List<Type> { typeof(List<int>), typeof(int), typeof(int) };
        public Type ReturnType => typeof(void);

        public static BaseFunction[][] ParamCache; // has always 3 elems
        public IEnumerable<BaseFunction> GenerateDepthFirst(Context context, BaseFunction parentFunc)
        {
            if (parentFunc.Info.SwapCount >= this.Max) yield break;
            
            var swapFunc = new SwapFunction (parentFunc); 
            swapFunc.Info.Set(parentFunc.Info);
            swapFunc.Info.SwapCount++;

            swapFunc.ParamValues[0] = context.List;

            if (ParamCache == null)
            {
                var list = new List<BaseFunction[]>();
                var ind = 0;
                foreach (var intVar0 in context.Ints)
                {
                    ind++;
                    foreach (var intVar1 in context.Ints.Skip(ind))
                    {
                        list.Add(new[]{ intVar0 , intVar1});
                    }
                }

                ParamCache = list.ToArray();
            }
            
            foreach (var pars in ParamCache)
            {
                swapFunc.ParamValues[1] = pars[0];
                swapFunc.ParamValues[2] = pars[1];

                if (!swapFunc.IsValid()) // TODO:sort specific
                {
                    continue;
                }

                foreach (var nextFunc in context.VoidExpressions.SelectMany(p => p.GenerateDepthFirst(context, swapFunc)))
                {
                    swapFunc.Next = nextFunc;
                    swapFunc.Info.Set(nextFunc.Info);
                    yield return swapFunc;
                    swapFunc.Next = null;
                    swapFunc.Info.Set(parentFunc.Info);
                    swapFunc.Info.SwapCount++;
                }

                yield return swapFunc; // next = null
            }
        }

        public bool AcceptParams(BaseFunction swapFunc, BaseFunction generatorFunc, BaseFunction[] parameters)
        {
            //Check if this swap improves new fitness value
            var par0 = ((IntConstant)parameters[0]).Value;
            var par1 = ((IntConstant)parameters[1]).Value;

            if (generatorFunc is IfFunction iffparent)
            {
                var paremas = iffparent.GetParams();
                if (paremas != null && (paremas[0] != par0 || paremas[1] != par1))
                {
                    return false;
                }
            }
            else if (generatorFunc is SwapFunction swapparent)
            {
                var paremas = new int[] { ((IntConstant)swapparent.ParamValues[1]).Value, ((IntConstant)swapparent.ParamValues[2]).Value };
                if (paremas.Contains(par0) && paremas.Contains(par1))
                {
                    return false;
                }
            }
            //else
            //{
                return false;
            //}
        }

        public static HashSet<string> stateCache = new HashSet<string>();

        public IEnumerable<BaseFunction> GenerateStateFull(Context context, BaseFunction generatorFunc, bool isNext)
        {
            if (generatorFunc.Info.SwapCount >= this.Max) yield break;            

            var swapFunc = isNext ? new SwapFunction(generatorFunc.Parent ?? generatorFunc) { Before = generatorFunc } : new SwapFunction(generatorFunc);

            swapFunc.Info.Set(generatorFunc.Info);
            swapFunc.Info.SwapCount++;
            swapFunc.Info.IfDepth = swapFunc.Parent.Info.IfDepth;
            swapFunc.Info.Scope = swapFunc.Parent.Info.Scope;
            swapFunc.Info.ConsecutiveSwapCount = 1;
            if (swapFunc.Before is SwapFunction)
            {
                if (swapFunc.Before.Info.ConsecutiveSwapCount >= MaxConsecutive)
                    yield break;

                swapFunc.Info.ConsecutiveSwapCount = swapFunc.Before.Info.ConsecutiveSwapCount + 1;                
            }

            swapFunc.ParamValues[0] = context.List;

            if (ParamCache == null)
            {
                var list = new List<BaseFunction[]>();
                var ind = 0;
                foreach (var intVar0 in context.Ints)
                {
                    ind++;
                    foreach (var intVar1 in context.Ints.Skip(ind))
                    {
                        list.Add(new[] { intVar0, intVar1 });
                    }
                }

                ParamCache = list.ToArray();
            }

            foreach (var pars in ParamCache)
            {
                swapFunc.ParamValues[1] = pars[0];
                swapFunc.ParamValues[2] = pars[1];

                //Check if this swap improves new fitness value
                var par0 = ((IntConstant)pars[0]).Value;
                var par1 = ((IntConstant)pars[1]).Value;
                
                if(generatorFunc is IfFunction iffparent)
                { 
                    var paremas = iffparent.GetParams();
                    if (paremas!=null &&(paremas[0] != par0 || paremas[1] != par1))
                    {
                        swapFunc.Next = null;
                        continue;
                    }
                }
                else if (generatorFunc is IfElseFunction ifElseparent)
                {
                    var paremas = ifElseparent.GetParams();
                    if (paremas != null && (paremas[0] != par0 || paremas[1] != par1))
                    {
                        swapFunc.Next = null;
                        continue;
                    }
                }
                else if (generatorFunc is SwapFunction swapparent)
                {
                    var paremas = new int[] { ((IntConstant)swapparent.ParamValues[1]).Value, ((IntConstant)swapparent.ParamValues[2]).Value } ;
                    if (paremas.Contains(par0) && paremas.Contains(par1))
                    {
                        swapFunc.Next = null;
                        continue;
                    }
                }
                else
                {
                    swapFunc.Next = null;
                    continue;
                }

                //bool canImprove;
                //if (((ListConstant)context.List).Value.Length == 4 && swapFunc.Info.IfCount == 3 && swapFunc.Info.SwapCount == 4 && swapFunc.Info.IfDepth == 1 && swapFunc.Info.IfExecCount == 72) // discrapency 
                //{
                //    canImprove = true;// generatorFunc.Info.States.Scope(swapFunc.Parent.Info.Scope).Count(p => p[par0] > p[par1]) >= swapFunc.Parent.Info.Scope.Count - 3;
                //}
                //else
                //    canImprove = generatorFunc.Info.States.Scope(swapFunc.Parent.Info.Scope).All(p => p[par0] > p[par1]);

                //if (!canImprove)
                //{
                //    swapFunc.Next = null;
                //    continue;
                //}

                var states = generatorFunc.Info.States.Copy();
                states.Scope(swapFunc.Parent.Info.Scope).ForEach(p => p.Swap(par0, par1));

                //var hash = states.GetHash(swapFunc.Info);
                //if (stateCache.Contains(hash))
                //{
                //    swapFunc.Next = null;
                //    continue;
                //}
                //stateCache.Add(hash);

                //if (!this.CheckState(swapFunc.Parent.Info.Scope, states, generatorFunc.Info.StateHistory))
                //{
                //    swapFunc.Next = null;
                //    continue;
                //}

                swapFunc.Info.States = states;   // always get the states from the generator
                //swapFunc.Info.StateHistory = generatorFunc.Info.StateHistory.Copy();
                //swapFunc.Info.HistorizeState(states);

                yield return swapFunc; // next = null

                foreach (var nextFunc in context.VoidExpressions.SelectMany(p => p.GenerateStateFull(context, swapFunc, true)))
                {
                    swapFunc.Next = nextFunc;
                    swapFunc.Info.Set(nextFunc.Info);
                    swapFunc.Info.States = nextFunc.Info.States;
                    //swapFunc.Info.StateHistory = nextFunc.Info.StateHistory;
                    yield return swapFunc;
                    swapFunc.Next = null;
                    swapFunc.Info.Set(generatorFunc.Info);
                    swapFunc.Info.SwapCount++;

                    swapFunc.Info.States = states;
                    //swapFunc.Info.StateHistory = generatorFunc.Info.StateHistory.Copy();
                    //swapFunc.Info.HistorizeState(states);
                }
            }
        }
        public bool CheckState(List<int> scope, List<int[]> states, List<int[]>[] history)
        {
            foreach (var ind in scope)
            {
                if (history[ind].Any(p=> p.SequenceEqual(states[ind])))
                    return false;
            }

            return true;
        }

        //public void Prune(List<int> scope, List<int[]> states, int swap0, int swap1)
        //{
        //    var toRemove = new List<int[]>();
        //    //var toRemoveFromScope = new List<int>();
        //    foreach (var ind in scope)
        //    {
        //        var swaped = states[ind].CopyAndSwap(swap0, swap1);

        //        var i = Contains(states, scope, swaped);
        //        if (i > -1)
        //        {
        //            toRemove.Add(states[i]);
        //            //toRemoveFromScope.Add(ind);
        //        }
        //        else
        //        {
        //            states[ind].Swap(swap0, swap1);
        //        }
        //    }

        //    toRemove.ForEach(p => states.Remove(p));
        //   // toRemoveFromScope.ForEach(p => scope.Remove(p));
        //}

        //private int Contains(List<int[]> states, List<int> scope, int[] state)
        //{
        //    for (var i = 0; i < states.Count; i++)
        //    {
        //        if (!scope.Contains(i) && states[i].SequenceEqual(state)) return i;
        //    }
            
        //    return -1;
        //}
        public bool CanHaveNext => true;
    }

    public class SwapFunction : BaseFunction
    {
        public override string Name => "swap";
        public SwapFunction(BaseFunction parent)
        {
            ParamValues = new BaseFunction[3];
            this.Parent = parent;
        }

        public override bool IsValid()
        {
            var parent = this.Parent as SwapFunction;
            var consecutiveCount = 0;
            while (parent != null)
            {
                consecutiveCount++;
                if (consecutiveCount > 3) return false;

                if (this.Equivalent(parent)) return false;

                parent = parent.Parent as SwapFunction;
            }

            return true;
        }

        public override object Eval()
        {
            ExecutionCounter.Evaluated?.Invoke(this);

            var list = ((ListConstant)ParamValues[0]).Value;
            var index0 = ((IntConstant)ParamValues[1]).Value;
            var index1 = ((IntConstant)ParamValues[2]).Value;

            var tmp = list[index0];
            list[index0] = list[index1];
            list[index1] = tmp;

            StateTester.AfterEval?.Invoke(this, list);

            Next?.Eval();
            
            return null;
        }

        public override string ToString()
        {
            return $"swap(A, {ParamValues[1]}, {ParamValues[2]})" + (Next == null ? "" : " next:" + Next);
        }

        public override BaseFunction Copy(BaseFunction parent)
        {
            var copy = new SwapFunction(parent);
            copy.ParamValues = new[] { ParamValues[0].Copy(copy), ParamValues[1].Copy(copy), ParamValues[2].Copy(copy) };
            copy.Next = Next?.Copy(copy);
            copy.Info.Set(this.Info);
            return copy;
        }
    }
}
