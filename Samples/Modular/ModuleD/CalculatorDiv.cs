using Interface;

namespace ModuleD
{
    class CalculatorDiv : ICalculatorDiv
    {
        public int Div(int lhs, int rhs)
        {
            return lhs / rhs;
        }
    }
}
