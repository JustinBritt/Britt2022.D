namespace Britt2022.D.InterfacesAbstractFactories
{
    using Britt2022.D.InterfacesFactories.Calculations;

    public interface ICalculationsAbstractFactory
    {
        IACalculationFactory CreateACalculationFactory();

        IDCalculationFactory CreateDCalculationFactory();

        InCalculationFactory CreatenCalculationFactory();

        IpCalculationFactory CreatepCalculationFactory();

        IΦCalculationFactory CreateΦCalculationFactory();
    }
}