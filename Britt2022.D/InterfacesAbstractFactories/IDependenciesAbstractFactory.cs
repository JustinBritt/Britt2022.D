namespace Britt2022.D.InterfacesAbstractFactories
{
    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;
    using Britt2022.D.InterfacesFactories.Dependencies.MathNet.Numerics.Distributions;

    public interface IDependenciesAbstractFactory
    {
        IBundleFactory CreateBundleFactory();

        IContinuousUniformFactory CreateContinuousUniformFactory();

        IDiscreteUniformFactory CreateDiscreteUniformFactory();

        IDurationFactory CreateDurationFactory();

        IEntryComponentFactory CreateEntryComponentFactory();

        IFhirDateTimeFactory CreateFhirDateTimeFactory();

        ILocationFactory CreateLocationFactory();

        ILogNormalFactory CreateLogNormalFactory();

        INullableValueFactory CreateNullableValueFactory();

        IOrganizationFactory CreateOrganizationFactory();
    }
}