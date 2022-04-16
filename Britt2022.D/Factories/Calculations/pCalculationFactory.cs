namespace Britt2022.D.Factories.Calculations
{
    using Britt2022.D.Classes.Calculations;
    using Britt2022.D.Interfaces.Calculations;
    using Britt2022.D.InterfacesFactories.Calculations;

    internal sealed class pCalculationFactory : IpCalculationFactory
    {
        public pCalculationFactory()
        {
        }

        public IpCalculation Create()
        {
            IpCalculation calculation;

            try
            {
                calculation = new pCalculation();
            }
            finally
            {
            }

            return calculation;
        }
    }
}