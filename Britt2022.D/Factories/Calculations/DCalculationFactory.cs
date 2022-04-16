namespace Britt2022.D.Factories.Calculations
{
    using Britt2022.D.Classes.Calculations;
    using Britt2022.D.Interfaces.Calculations;
    using Britt2022.D.InterfacesFactories.Calculations;

    internal sealed class DCalculationFactory : IDCalculationFactory
    {
        public DCalculationFactory()
        {
        }

        public IDCalculation Create()
        {
            IDCalculation calculation;

            try
            {
                calculation = new DCalculation();
            }
            finally
            {
            }

            return calculation;
        }
    }
}