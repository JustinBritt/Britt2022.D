namespace Britt2022.D.Factories.Calculations
{
    using Britt2022.D.Classes.Calculations;
    using Britt2022.D.Interfaces.Calculations;
    using Britt2022.D.InterfacesFactories.Calculations;

    internal sealed class nCalculationFactory : InCalculationFactory
    {
        public nCalculationFactory()
        {
        }

        public InCalculation Create()
        {
            InCalculation calculation;

            try
            {
                calculation = new nCalculation();
            }
            finally
            {
            }

            return calculation;
        }
    }
}