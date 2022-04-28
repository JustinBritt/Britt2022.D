namespace Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model
{
    using System;

    using global::Hl7.Fhir.Model;

    public interface IFhirDateTimeFactory
    {
        FhirDateTime Create();

        FhirDateTime Create(
            DateTime dateTime);

        FhirDateTime Create(
            DateTimeOffset dateTimeOffset);
    }
}