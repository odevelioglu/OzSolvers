using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprGenrator
{
    public class ListConstant : BaseFunction
    {
        public override string Name => "list";
        public int[] Value;

        public ListConstant(int[] value)
        {
            ParamValues = new BaseFunction[0];
            this.Value = value;
        }

        public override bool IsValid()
        {
            return true;
        }

        public override object Eval()
        {
            ExecutionCounter.Evaluated?.Invoke(this);

            return this.Value;
        }

        public override string ToString()
        {
            return "A";
        }

        public override BaseFunction Copy(BaseFunction parent)
        {
            return this;
            //return new ListConstant((int[])this.Value.Clone())
            //{
            //    Parent = parent
            //};
        }
    }
}
