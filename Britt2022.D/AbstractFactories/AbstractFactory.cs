namespace Britt2022.D.AbstractFactories
{
    using Britt2022.D.AbstractFactories;
    using Britt2022.D.InterfacesAbstractFactories;

    public sealed class AbstractFactory : IAbstractFactory
    {
        public AbstractFactory()
        {
        }

        public static IAbstractFactory Create()
        {
            return new AbstractFactory();
        }

        public ICalculationsAbstractFactory CreateCalculationsAbstractFactory()
        {
            ICalculationsAbstractFactory abstractFactory = null;

            try
            {
                abstractFactory = new CalculationsAbstractFactory();
            }
            finally
            {
            }

            return abstractFactory;
        }

        public IDependenciesAbstractFactory CreateDependenciesAbstractFactory()
        {
            IDependenciesAbstractFactory abstractFactory = null;

            try
            {
                abstractFactory = new DependenciesAbstractFactory();
            }
            finally
            {
            }

            return abstractFactory;
        }

        public IExperimentsAbstractFactory CreateExperimentsAbstractFactory()
        {
            IExperimentsAbstractFactory abstractFactory = null;

            try
            {
                abstractFactory = new ExperimentsAbstractFactory();
            }
            finally
            {
            }

            return abstractFactory;
        }
    }
}