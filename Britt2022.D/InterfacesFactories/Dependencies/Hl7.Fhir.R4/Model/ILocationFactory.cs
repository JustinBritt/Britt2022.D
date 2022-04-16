namespace Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model
{
    using global::Hl7.Fhir.Model;

    public interface ILocationFactory
    {
        Location Create();

        Location Create(
            string id);
    }
}