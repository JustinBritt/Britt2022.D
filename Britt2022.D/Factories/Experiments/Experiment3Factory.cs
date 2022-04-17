namespace Britt2022.D.Factories.Experiments
{
    using Britt2022.D.Classes.Experiments;
    using Britt2022.D.Interfaces.Experiments;
    using Britt2022.D.InterfacesAbstractFactories;
    using Britt2022.D.InterfacesFactories.Experiments;

    internal sealed class Experiment3Factory : IExperiment3Factory
    {
        public Experiment3Factory()
        {
        }

        public IExperiment3 Create(
            ICalculationsAbstractFactory calculationsAbstractFactory,
            IDependenciesAbstractFactory dependenciesAbstractFactory)
        {
            IExperiment3 experiment;

            try
            {
                experiment = new Experiment3(
                    calculationsAbstractFactory,
                    dependenciesAbstractFactory);
            }
            finally
            {
            }

            return experiment;
        }
    }
}