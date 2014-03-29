using Interface;

namespace ModuleB
{
    class CalculatorSub : ICalculatorSub
    {
        public int Sub(int lhs, int rhs)
        {
            return lhs - rhs;
        }
    }
}
