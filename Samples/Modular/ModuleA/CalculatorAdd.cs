using Interface;

namespace ModuleA
{
    class CalculatorAdd : ICalculatorAdd
    {
        public int Add(int lhs, int rhs)
        {
            return lhs + rhs;
        }
    }
}
