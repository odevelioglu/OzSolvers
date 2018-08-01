using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprGenrator
{
    public interface IExpression
    {
        List<Type> ParameterTypes { get; }
        Type ReturnType { get; }
        IEnumerable<BaseFunction> GenerateDepthFirst(Context context, BaseFunction parentFunc);
        IEnumerable<BaseFunction> GenerateDepthFirstDyna(Context context, BaseFunction parentFunc);
    }

    public class BaseFunction
    {
        public virtual string Name { get; }
        public BaseFunction Parent { get; set; }

        public BaseFunction[] ParamValues { get; set; }

        public virtual bool IsValid()
        {
            throw new NotImplementedException();
        }

        public virtual object Eval()
        {
            throw new NotImplementedException();
        }

        public virtual BaseFunction Copy(BaseFunction parent)
        {
            throw new NotImplementedException();
        }

        public string GetKey()
        {
            var iff = this.Count("if");
            var swap = this.Count("swap");
            var block = this.Count("block");

            return $"swap{swap} iff{iff} block{block}";//swap + iff * 100 + block * 10000;
        }
    }

}
