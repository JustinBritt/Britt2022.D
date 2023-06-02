namespace Britt2022.D.InterfacesAbstractFactories
{
    using Britt2022.D.InterfacesFactories.Comparers;

    public interface IComparersAbstractFactory
    {
        IFhirDateTimeComparerFactory CreateFhirDateTimeComparerFactory();

        ILocationComparerFactory CreateLocationComparerFactory();

        INullableValueintComparerFactory CreateNullableValueintComparerFactory();

        IOrganizationComparerFactory CreateOrganizationComparerFactory();
    }
}