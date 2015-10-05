using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions.Math
{
    public class MathFunctionLib : FunctionLib
    {
        public MathFunctionLib()
        {
            RegisterFunction(new RoundFunction());
            RegisterFunction(new AbsFunction());
            RegisterFunction(new FloorFunction());
            RegisterFunction(new CeilingFunction());
            RegisterFunction(new MaxFunction());
            RegisterFunction(new MinFunction());
        }

        public override string GroupName
        {
            get { return "_math"; }
        }
    }

}