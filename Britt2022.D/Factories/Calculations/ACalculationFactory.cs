namespace Britt2022.D.Factories.Calculations
{
    using Britt2022.D.Classes.Calculations;
    using Britt2022.D.Interfaces.Calculations;
    using Britt2022.D.InterfacesFactories.Calculations;

    internal sealed class ACalculationFactory : IACalculationFactory
    {
        public ACalculationFactory()
        {
        }

        public IACalculation Create()
        {
            IACalculation calculation;

            try
            {
                calculation = new ACalculation();
            }
            finally
            {
            }

            return calculation;
        }
    }
}