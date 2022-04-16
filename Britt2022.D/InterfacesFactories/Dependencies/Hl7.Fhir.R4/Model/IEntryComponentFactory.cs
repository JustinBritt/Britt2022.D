namespace Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model
{
    using global::Hl7.Fhir.Model;

    public interface IEntryComponentFactory
    {
        Bundle.EntryComponent Create();

        Bundle.EntryComponent Create(
            Resource resource);
    }
}