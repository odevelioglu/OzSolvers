namespace MySolver.Inferring
{
    // Definitely read this for pattern finding
    //http://mymathforum.com/number-theory/17452-number-sequence-software.html

    //https://www.algebra.com/algebra/homework/Sequences-and-series/Sequences-and-series.faq.question.155130.html
    public class DifferenceTableInferer
    {
        public Node Infer(int[] sequence)
        {
            var node = InternalInfer(sequence);
            var simplified = Simplifier.Simplify(node);
            if (Verify(simplified, sequence))
                return simplified;
            return null;
        }

        private bool Verify(Node node, int[] list)
        {
            for (var i = 0; i < list.Length; i++)
            {
                if (!node.Eval(i).Equals(list[i]))
                    return false;
            }

            return true;
        }

        public Node InternalInfer(int[] sequence)
        {
            var main = Expr.FromConstant(sequence[0]);

            for (int i = 2; i < sequence.Length + 1; i++)
            {
                var fn = sequence[i - 1];

                var diff = fn - (int)main.Eval(i);

                main.Add(Get(i).Mul(diff));
            }

            return main.Node;
        }

        private Expr Get(int cnt)
        {
            var res = Expr.FromVariableN().Sub(1);

            for (int i = 2; i < cnt; i++)
            {
                res.Mul(Expr.FromVariableN().Sub(i));
            }

            return res.Div(Expr.FromConstant(cnt - 1).Fac());
        }
    }


    public class Expr
    {
        static Node N = new Node(new Variable("n", 0), null, null);
        public static Expr FromNode(Node n)
        {
            return new Expr(n);
        }

        public static Expr FromConstant(int n)
        {
            return new Expr(new Node(new Constant(n), null, null));
        }

        public static Expr FromVariableN()
        {
            return FromNode(N);
        }

        public Node Node;

        public double Eval(int n)
        {
            return this.Node.Eval(n);
        }

        public Expr(Node n)
        {
            this.Node = n;
        }

        public Expr Add(Expr exp)
        {
            return this.Add(exp.Node);
        }

        private Expr Add(Node n)
        {
            this.Node = new Node(Commons.add, this.Node, n);
            return this;
        }

        public Expr Mul(Expr expr)
        {
            return this.Mul(expr.Node);
        }

        public Expr Mul(int n)
        {
            return this.Mul(FromConstant(n));
        }

        private Expr Mul(Node n)
        {
            this.Node = new Node(Commons.mul, this.Node, n);
            return this;
        }

        public Expr Sub(Node n)
        {
            this.Node = new Node(Commons.sub, this.Node, n);
            return this;
        }

        public Expr Sub(int i)
        {
            return this.Sub(new Node(new Constant(i), null, null));
        }

        private Expr Div(Node n)
        {
            this.Node = new Node(Commons.div, this.Node, n);
            return this;
        }

        public Expr Div(Expr exp)
        {
            return Div(exp.Node);
        }

        public Expr Fac()
        {
            this.Node = new Node(Commons.fac, this.Node, null);
            return this;
        }
    }
}
