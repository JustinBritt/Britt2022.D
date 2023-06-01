namespace Britt2022.D.AbstractFactories
{
    using System;
    using System.IO;

    using log4net;

    using Britt2022.D.InterfacesAbstractFactories;

    public sealed class AbstractFactory : IAbstractFactory
    {
        public AbstractFactory()
        {
            using (FileStream fileStream = File.OpenRead($"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.config"))
            {
                log4net.Config.XmlConfigurator.Configure(
                    LogManager.GetRepository(System.Reflection.Assembly.GetExecutingAssembly()),
                    fileStream);
            }
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

        public IComparersAbstractFactory CreateComparersAbstractFactory()
        {
            IComparersAbstractFactory abstractFactory = null;

            try
            {
                abstractFactory = new ComparersAbstractFactory();
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