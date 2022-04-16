namespace Britt2022.D.Factories.Calculations
{
    using Britt2022.D.Classes.Calculations;
    using Britt2022.D.Interfaces.Calculations;
    using Britt2022.D.InterfacesFactories.Calculations;

    internal sealed class ΦCalculationFactory : IΦCalculationFactory
    {
        public ΦCalculationFactory()
        {
        }

        public IΦCalculation Create()
        {
            IΦCalculation calculation;

            try
            {
                calculation = new ΦCalculation();
            }
            finally
            {
            }

            return calculation;
        }
    }
}