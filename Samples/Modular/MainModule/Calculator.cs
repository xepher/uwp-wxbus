using Interface;

namespace MainModule
{
    class Calculator : ICalculator
    {
        private ICalculatorAdd _add;
        private ICalculatorSub _sub;
        private ICalculatorMul _mul;
        private ICalculatorDiv _div;

        public Calculator(ICalculatorAdd add, ICalculatorSub sub, ICalculatorMul mul, ICalculatorDiv div)
        {
            _add = add;
            _sub = sub;
            _mul = mul;
            _div = div;
        }

        public int Add(int lhs, int rhs)
        {
            return _add.Add(lhs, rhs);
        }

        public int Sub(int lhs, int rhs)
        {
            return _sub.Sub(lhs, rhs);
        }

        public int Mul(int lhs, int rhs)
        {
            return _mul.Mul(lhs, rhs);
        }

        public int Div(int lhs, int rhs)
        {
            return _div.Div(lhs, rhs);
        }
    }
}
