namespace Britt2022.D.AbstractFactories
{
    using Britt2022.D.Factories.Calculations;
    using Britt2022.D.InterfacesAbstractFactories;
    using Britt2022.D.InterfacesFactories.Calculations;

    internal sealed class CalculationsAbstractFactory : ICalculationsAbstractFactory
    {
        public CalculationsAbstractFactory()
        {
        }

        public IACalculationFactory CreateACalculationFactory()
        {
            IACalculationFactory factory = null;

            try
            {
                factory = new ACalculationFactory();
            }
            finally
            {
            }

            return factory;
        }

        public IDCalculationFactory CreateDCalculationFactory()
        {
            IDCalculationFactory factory = null;

            try
            {
                factory = new DCalculationFactory();
            }
            finally
            {
            }

            return factory;
        }

        public InCalculationFactory CreatenCalculationFactory()
        {
            InCalculationFactory factory = null;

            try
            {
                factory = new nCalculationFactory();
            }
            finally
            {
            }

            return factory;
        }

        public IpCalculationFactory CreatepCalculationFactory()
        {
            IpCalculationFactory factory = null;

            try
            {
                factory = new pCalculationFactory();
            }
            finally
            {
            }

            return factory;
        }

        public IΦCalculationFactory CreateΦCalculationFactory()
        {
            IΦCalculationFactory factory = null;

            try
            {
                factory = new ΦCalculationFactory();
            }
            finally
            {
            }

            return factory;
        }
    }
}