using Interface;

namespace ModuleC
{
    class CalculatorMul: ICalculatorMul
    {
        public int Mul(int lhs, int rhs)
        {
            return lhs*rhs;
        }
    }
}
