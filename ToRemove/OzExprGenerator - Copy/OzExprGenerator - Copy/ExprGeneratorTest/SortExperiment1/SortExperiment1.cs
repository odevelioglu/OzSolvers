using System;
using System.Linq;
using ExprGenrator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExprGeneratorTest.SortExperiment1
{
    [TestClass]
    public class SortExperiment1
    {
        // len	comple  lg(n!)	    n*n-1/2
        // 3	2.667	2.584962501	3
        // 4	4.917	4.584962501	6
        // 5	7.717	6.906890596	10
        // 6	11.05	9.491853096	15
        // 7	14.907	12.29920802	21
        // 8	19.282	15.29920802	28
        // 9	24.171	18.46913302	36
        // 10	29.571	21.79106111	45

        [TestMethod]
        public void TestMethod1()
        {
            for (int n = 3; n < 8; n++)
            {
                var theList = Enumerable.Range(1, n).OrderByDescending(p => p).ToArray();
                var context = Context.CreateDefault(theList);
                var builder = new FunctionCreator(context);

                var sort = builder.List(n - 2);

                if (!SortTester.Test(context, sort, (int[])theList.Clone()))
                {

                }

                Console.WriteLine(sort.FullInfo(theList, context));
            }
        }
    }


    public class FunctionCreator
    {
        private Context context;
        public FunctionCreator(Context context)
        {
            this.context = context;
        }
        public BlockFunction Block(BaseFunction parent, BaseFunction first, BaseFunction second)
        {
            var block = new BlockFunction { Parent = parent };
            block.ParamValues[0] = first;
            block.ParamValues[1] = second;
            return block;
        }

        public IfFunction Iff(BaseFunction parent, int i, int j)
        {
            var if1 = new IfFunction { Parent = parent };
            if1.ParamValues[0] = Gt(if1, i, j);
            if1.ParamValues[1] = Swap(if1, i, j);

            return if1;
        }

        public GreaterThanFunction Gt(BaseFunction parent, int i, int j)
        {
            var gt = new GreaterThanFunction { Parent = parent };
            gt.ParamValues[0] = Gv(gt, i);
            gt.ParamValues[1] = Gv(gt, j);

            return gt;
        }

        public GetValueFunction Gv(BaseFunction parent, int i)
        {
            return new GetValueFunction
            {
                Parent = parent,
                ParamValues = { [0] = context.List, [1] = new IntConstant(i) }
            };
        }

        public SwapFunction Swap(BaseFunction parent, int i, int j)
        {
            return new SwapFunction
            {
                Parent = parent,
                ParamValues = new[] { context.List, new IntConstant(i), new IntConstant(j) }
            };
        }

        public IfFunction IffRecursive(BaseFunction parent, int i)
        {
            var if1 = new IfFunction { Parent = parent };
            if1.ParamValues[0] = Gt(if1, i, i + 1);

            var block = new BlockFunction { Parent = if1 };
            if1.ParamValues[1] = block;

            block.ParamValues[0] = Swap(block, i, i + 1);
            if (i >= 1)
                block.ParamValues[1] = this.IffRecursive(block, i - 1);
            else
            {
                block.ParamValues[1] = new EmptyFunction();
            }

            return if1;
        }

        public SortFunction List(int max)
        {
            var sort = new SortFunction();
            sort.ParamValues[0] = context.List;
            var block = new BlockFunction { Parent = sort };
            sort.ParamValues[1] = block;
            block.ParamValues[0] = Iff(block, 0, 1);

            this.SetBlockRecursive(block, 1, max);

            return sort;
        }

        public BlockFunction SetBlockRecursive(BlockFunction upperBlock, int i, int max)
        {
            var innerBlock = new BlockFunction { Parent = upperBlock };

            upperBlock.ParamValues[1] = innerBlock;

            innerBlock.ParamValues[0] = this.IffRecursive(innerBlock, i);

            if (i + 1 <= max)
                innerBlock.ParamValues[1] = this.SetBlockRecursive(innerBlock, i + 1, max);
            else
            {
                innerBlock.ParamValues[1] = new EmptyFunction();
            }

            return innerBlock;
        }
    }
}
