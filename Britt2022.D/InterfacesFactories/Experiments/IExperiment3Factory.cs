namespace Britt2022.D.InterfacesFactories.Experiments
{
    using Britt2022.D.Interfaces.Experiments;
    using Britt2022.D.InterfacesAbstractFactories;

    public interface IExperiment3Factory
    {
        IExperiment3 Create(
            ICalculationsAbstractFactory calculationsAbstractFactory,
            IDependenciesAbstractFactory dependenciesAbstractFactory);
    }
}