namespace Britt2022.D.InterfacesAbstractFactories
{
    public interface IAbstractFactory
    {
        ICalculationsAbstractFactory CreateCalculationsAbstractFactory();

        IComparersAbstractFactory CreateComparersAbstractFactory();

        IDependenciesAbstractFactory CreateDependenciesAbstractFactory();

        IExperimentsAbstractFactory CreateExperimentsAbstractFactory();
    }
}