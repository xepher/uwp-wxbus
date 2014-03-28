using Interface;

namespace Concrete
{
    public class Calculator : ICalculator
    {
        public int add(int lhs, int rhs)
        {
            return lhs + rhs;
        }

        public int sub(int lhs, int rhs)
        {
            return lhs - rhs;
        }

        public int mul(int lhs, int rhs)
        {
            return lhs * rhs;
        }

        public int div(int lhs, int rhs)
        {
            return lhs / rhs;
        }
    }
}
