namespace Britt2022.D.InterfacesAbstractFactories
{
    using Britt2022.D.InterfacesFactories.Experiments;

    public interface IExperimentsAbstractFactory
    {
        IExperiment1Factory CreateExperiment1Factory();

        IExperiment3Factory CreateExperiment3Factory();
    }
}