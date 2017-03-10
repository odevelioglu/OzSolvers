using System;
using System.Collections.Generic;

namespace MySolver
{
    public class Node
    {
        public IOzExpression Expression;
        public Node Left;
        public Node Right;

        public Node(IOzExpression e, Node left, Node right) 
        {
            this.Expression = e;
            this.Left = left;
            this.Right = right;
        }

        public double Eval(int[] vars)
        {
            return this.Expression.Eval(vars, this.Left, this.Right);
        }

        public double Eval(int var)
        {
            return this.Expression.Eval(new[] {var}, this.Left, this.Right);
        }

        public override string ToString()
        {
            return this.Expression.ToString(this.Left, this.Right);
        }
        
        public bool Eq(Node node, IEnumerable<int[]> vars)
        {
            foreach (var intse in vars)
            {
                if (!this.Eval(intse).Eq(node.Eval(intse)))
                {
                    return false;
                }
            }

            return true;
        }

        public bool SemEq(Node other)
        {
            if (other == null) return false;
            if (this == other) return true;

            if (!this.Expression.SemEq(other.Expression)) return false;

            if (this.Left != null)
            {
                if (!this.Left.SemEq(other.Left)) return false;
            }
            else
            {
                if (other.Left != null) return false;
            }

            if (this.Right != null)
            { 
                if (!this.Right.SemEq(other.Right)) return false;
            }
            else
            {
                if (other.Right != null) return false;
            }

            return true;
        }
    }

    public interface IOzExpression:ICloneable
    {
        double Eval(int[] vars, Node d1, Node d2);

        string ToString(Node left, Node right);

        bool SemEq(object other);
    }

    public class Constant : IOzExpression
    {
        public int Value;
        
        public Constant(int value)
        {
            this.Value = value;
        }

        public double Eval(int[] vars, Node d1, Node d2)
        {
            return this.Value;
        }

        public string ToString(Node left, Node right)
        {
            return this.Value.ToString();
        }

        public bool SemEq(object other)
        {
            var othercon = other as Constant;
            return this.Value == othercon?.Value;
        }

        public object Clone()
        {
            return new Constant(this.Value);
        }
    }

    public class Variable : IOzExpression
    {
        private int index;
        public string Name;

        public Variable(string name, int index)
        {
            this.index = index;
            this.Name = name;
        }

        public double Eval(int[] vars, Node d1, Node d2)
        {
            return vars[this.index];
        }

        public string ToString(Node left, Node right)
        {
            return this.Name;
        }

        public bool SemEq(object other)
        {
            var otherVar = other as Variable;

            return this.index == otherVar?.index;
        }

        public object Clone()
        {
            return new Variable(this.Name, this.index);
        }
    }

    public class BinaryFunction : IOzExpression
    {
        private Func<double[], double> func;
        public string name;
        public int NumberOfVariables;
        public BinaryFunction(string name, Func<double[], double> func, int numberOfVars)
        {
            this.func = func;
            this.name = name;
            this.NumberOfVariables = numberOfVars;
        }

        public double Eval(int[] vars, Node left, Node right)
        {
            if(this.NumberOfVariables == 2)
                return this.func(new []{ left.Eval(vars), right.Eval(vars) });
            else
            {
                return this.func(new[] { left.Eval(vars)});
            }
        }

        public string ToString(Node left, Node right)
        {
            //return $"{name}({left}, {right})";
            var op = "+";
            if (this.name == "mul") op = "*";
            else if (this.name == "div") op = "/";
            else if (this.name == "pow") op = "^";
            else if (this.name == "sub") op = "-";
            else if (this.name == "log") return $"{this.name}({left})";
            else if (this.name == "fac") return $"({left})!";
            else if (this.name == "neg") return $"-({left})";
            return $"({left} {op} {right})";
        }

            public bool SemEq(object other)
            {
               var otherFunc = other as BinaryFunction;
                return this.name == otherFunc?.name;
            }

        public object Clone()
        {
            return new BinaryFunction(this.name, (Func<double[], double>)this.func.Clone(), this.NumberOfVariables);
        }
    }
}
