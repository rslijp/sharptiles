using System;
using org.SharpTiles.Expressions.Functions;

public class MathLib : FunctionLib
{
    public MathLib()
    {
        RegisterFunction(new FacultyFunction());
        RegisterFunction(new FibonacciFunction());
    }


    public override string GroupName
    {
        get { return "math"; }
    }

    public class FibonacciFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
                             {
                                 new FunctionArgument{ Type = typeof (int), Name = "number" }
                             };

        public object Evaluate(params object[] parameters)
        {
            int seed = ((int?)parameters[0]) ?? 0;

            return fib(seed);
        }

        private int fib(int seed)
        {
            if (seed == 0 || seed == 1) return 1;
            return fib(seed - 1) + fib(seed - 2);
        }

        public string Name
        {
            get { return "fibonacci"; }
        }

        public FunctionArgument[] Arguments
        {
            get { return ARGUMENTS; }
        }

        public Type ReturnType
        {
            get { return typeof(int); }
        }
    }

    public class FacultyFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
                             {
                                 new FunctionArgument{ Type = typeof (int), Name = "number" }
                             };

        public object Evaluate(params object[] parameters)
        {
            int seed = ((int?)parameters[0]) ?? 0;

            return fac(seed);
        }

        private int fac(int seed)
        {
            if (seed == 1) return 1;
            return seed * fac(seed - 1);
        }

        public string Name
        {
            get { return "faculty"; }
        }

        public FunctionArgument[] Arguments
        {
            get { return ARGUMENTS; }
        }

        public Type ReturnType
        {
            get { return typeof(int); }
        }
    }


}