using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprGenrator
{
    public class IntConstant : BaseFunction
    {
        public override string Name => "int";
        public int Value;

        public override bool IsValid()
        {
            return true;
        }

        public IntConstant(int value)
        {
            ParamValues = new BaseFunction[0];
            this.Value = value;
        }

        public override object Eval()
        {
            ExecutionCounter.Evaluated?.Invoke(this);

            return Value;
        }

        public override string ToString()
        {
            return $"{this.Value}";
        }

        public override BaseFunction Copy(BaseFunction parent)
        {
            return new IntConstant(this.Value)
                       {
                           Parent = parent
                       };
        }
    }
}
