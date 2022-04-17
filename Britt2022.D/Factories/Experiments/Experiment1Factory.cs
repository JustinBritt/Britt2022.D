namespace Britt2022.D.Factories.Experiments
{
    using Britt2022.D.Classes.Experiments;
    using Britt2022.D.Interfaces.Experiments;
    using Britt2022.D.InterfacesAbstractFactories;
    using Britt2022.D.InterfacesFactories.Experiments;

    internal sealed class Experiment1Factory : IExperiment1Factory
    {
        public Experiment1Factory()
        {
        }

        public IExperiment1 Create(
            ICalculationsAbstractFactory calculationsAbstractFactory,
            IDependenciesAbstractFactory dependenciesAbstractFactory)
        {
            IExperiment1 experiment1;

            try
            {
                experiment1 = new Experiment1(
                    calculationsAbstractFactory,
                    dependenciesAbstractFactory);
            }
            finally
            {
            }

            return experiment1;
        }
    }
}