namespace Britt2022.D.AbstractFactories
{
    using Britt2022.D.Factories.Experiments;
    using Britt2022.D.InterfacesAbstractFactories;
    using Britt2022.D.InterfacesFactories.Experiments;

    internal sealed class ExperimentsAbstractFactory : IExperimentsAbstractFactory
    {
        public ExperimentsAbstractFactory()
        {
        }

        public IExperiment1Factory CreateExperiment1Factory()
        {
            IExperiment1Factory factory = null;

            try
            {
                factory = new Experiment1Factory();
            }
            finally
            {
            }

            return factory;
        }

        public IExperiment3Factory CreateExperiment3Factory()
        {
            IExperiment3Factory factory = null;

            try
            {
                factory = new Experiment3Factory();
            }
            finally
            {
            }

            return factory;
        }
    }
}