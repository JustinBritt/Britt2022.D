namespace Britt2022.D.InterfacesAbstractFactories
{
    public interface IAbstractFactory
    {
        ICalculationsAbstractFactory CreateCalculationsAbstractFactory();

        IDependenciesAbstractFactory CreateDependenciesAbstractFactory();

        IExperimentsAbstractFactory CreateExperimentsAbstractFactory();
    }
}