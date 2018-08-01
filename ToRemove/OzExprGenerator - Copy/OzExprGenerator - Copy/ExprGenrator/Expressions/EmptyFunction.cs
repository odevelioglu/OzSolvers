namespace ExprGenrator
{
    public class EmptyFunction : BaseFunction
    {
        public override string Name => "nil";
        public override bool IsValid()
        {
            return true;
        }

        public EmptyFunction()
        {
            ParamValues = new BaseFunction[0];
        }

        public override object Eval()
        {
            ExecutionCounter.Evaluated?.Invoke(this);
            return null;
        }

        public override string ToString()
        {
            return "nil";
        }

        public override BaseFunction Copy(BaseFunction parent)
        {
            return new EmptyFunction { Parent = parent };
        }
    }
}
