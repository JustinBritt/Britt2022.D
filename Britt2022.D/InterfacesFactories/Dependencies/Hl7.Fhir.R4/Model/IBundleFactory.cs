namespace Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model
{
    using System.Collections.Generic;

    using global::Hl7.Fhir.Model;

    public interface IBundleFactory
    {
        Bundle Create();

        Bundle Create(
            List<Bundle.EntryComponent> entry);
    }
}