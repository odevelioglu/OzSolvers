namespace MySolver
{
    public class Simplifier
    {
        private static Node c1 = new Node(new Constant(1), null, null);
        private static Node c0 = new Node(new Constant(0), null, null);

        public static Node Simplify(Node node)
        {
            var clone = node.Clone();

            while(true)
            {
                var refer = clone.Clone();
                Visit(ref clone);

                if (refer.SemEq(clone))
                    break;
            }
            
            return clone;
        }

        public static void Visit(ref Node node)
        {
            if (node == null) return;

            var func = node.Expression as BinaryFunction;
            if (func == null) return; // Variables and constants are all good
            
            var left = node.Left;
            var right = node.Right;

            // func(c, c)
            //var leftCons = left.Expression as Constant;
            //var rightCons = right.Expression as Constant;
            //if (leftCons != null && rightCons != null)
            //    return false;

            var leftFunc = left?.Expression as BinaryFunction;
            var rightFunc = right?.Expression as BinaryFunction;

            switch (func.name)
            {
                case "div":
                    // E / 1
                    if (right.SemEq(c1)) { node = left; return;}
                    //E / E 
                    if (left.SemEq(right)) { node = c1; return;}
                    // 0 / E
                    if (left.SemEq(c0)) { node = c0; return;}

                    if (leftFunc?.name == "mul")
                    {
                        // (n * E) / E
                        if (left.Right.SemEq(right)) { node = left.Left; return; }

                        // (E * n) / E
                        if (left.Left.SemEq(right)) { node = left.Right; return; }
                    }

                    // E / E ^ n

                    // E ^ n / E

                    // (E / n) / E
                    if (leftFunc?.name == "div" && left.Left.SemEq(right)) { node.Left = c1; node.Right = left.Right; return; }

                    // E / (E / n) 
                    if (rightFunc?.name == "div" && right.Left.SemEq(left)) { node = right.Right; return; }

                    break;

                case "mul":
                    // (N / E) * E
                    if (leftFunc?.name == "div" && left.Right.SemEq(right)) { node = left.Left; return; }

                    // E * (N / E)
                    if (rightFunc?.name == "div" && left.SemEq(right.Right)) { node = right.Left; return; }

                    // E * 1
                    if (right.SemEq(c1)) { node = left; return; }

                    // E * 0
                    if (right.SemEq(c0)) { node = c0; return; }

                    // 1 * E
                    if (left.SemEq(c1)) { node = right; return; }

                    // 0 * E
                    if (left.SemEq(c0)) { node = c0; return; }

                    // E * E

                    break;
                case "add":
                    // 0 + E
                    if (left.SemEq(c0)) { node = right; return; }

                    // E + 0
                    if (right.SemEq(c0)) { node = left; return; }

                    // (n - E) + E
                    if(leftFunc?.name == "sub" && left.Right.SemEq(right)) { node = left.Left; return; }

                    // E + (n - E)
                    if (rightFunc?.name == "sub" && left.SemEq(right.Right)) { node = right.Left; return; }

                    // E + E

                    break;
                case "sub":
                    // E - 0
                    if (right.SemEq(c0)) { node = left; return; }

                    // E - E
                    if (left.SemEq(right)) { node = c0; return; }
                    break;

                case "fac":
                    // 1! 
                    if (left.SemEq(c1)) { node = c1; return;}

                    break;

                case "log":
                    // log(1)
                    if (left.SemEq(c1)) { node = c0; return; }

                    break;

                case "neg":
                    // -(-n)
                    if (leftFunc?.name != "neg") { node = left.Left; return; }

                    break;

                case "pow":
                    // 1 ^ E
                    if (left.SemEq(c1)) { node = c1; return; }

                    // 0 ^ E
                    if (left.SemEq(c0)) { node = c0; return; }

                    // E ^ 1
                    if (right.SemEq(c1)) { node = left; return; }

                    // E ^ 0
                    if (right.SemEq(c0)) { node = c1; return; }

                    break;
            }

            Visit(ref node.Left);
            Visit(ref node.Right);
        }
    }

    public static class NodeHelper
    {
        public static Node Clone(this Node node)
        {
            if (node == null) return null;
                
            return new Node((IOzExpression)node.Expression.Clone(), node.Left.Clone(), node.Right.Clone());
        }
    }
}
