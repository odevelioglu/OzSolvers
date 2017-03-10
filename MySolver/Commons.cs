using System;

namespace MySolver
{
    public class Commons
    {
        public static BinaryFunction mul = new BinaryFunction("mul", vars => vars[0] * vars[1], 2);
        public static BinaryFunction add = new BinaryFunction("add", vars => vars[0] + vars[1], 2);
        public static BinaryFunction div = new BinaryFunction("div", vars => vars[0] / vars[1], 2);
        public static BinaryFunction sub = new BinaryFunction("sub", vars => vars[0] - vars[1], 2);
        public static BinaryFunction pow = new BinaryFunction("pow", vars => Math.Pow(vars[0], vars[1]), 2);
        public static BinaryFunction log = new BinaryFunction("log", vars => Math.Log(vars[0]), 1);
        public static BinaryFunction neg = new BinaryFunction("neg", vars => -vars[0], 1);
        public static BinaryFunction fac = new BinaryFunction("fac", vars => MathHelper.Fact((int)vars[0]), 1);

        public static Constant c1 = new Constant(1);
        public static Constant c2 = new Constant(2);
    }
}
