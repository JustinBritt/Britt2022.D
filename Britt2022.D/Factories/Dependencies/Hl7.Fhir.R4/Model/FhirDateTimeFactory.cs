namespace Britt2022.D.Factories.Dependencies.Hl7.Fhir.R4.Model
{
    using System;

    using global::Hl7.Fhir.Model;

    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;

    internal sealed class FhirDateTimeFactory : IFhirDateTimeFactory
    {
        public FhirDateTimeFactory()
        {
        }

        public FhirDateTime Create()
        {
            FhirDateTime FhirDateTime;

            try
            {
                FhirDateTime = new FhirDateTime();
            }
            finally
            {
            }

            return FhirDateTime;
        }

        public FhirDateTime Create(
            DateTimeOffset dateTimeOffset)
        {
            FhirDateTime FhirDateTime;

            try
            {
                FhirDateTime = new FhirDateTime(
                    dateTimeOffset);
            }
            finally
            {
            }

            return FhirDateTime;
        }
    }
}