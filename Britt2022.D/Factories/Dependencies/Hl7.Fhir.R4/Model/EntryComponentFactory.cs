namespace Britt2022.D.Factories.Dependencies.Hl7.Fhir.R4.Model
{
    using global::Hl7.Fhir.Model;

    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;

    internal sealed class EntryComponentFactory : IEntryComponentFactory
    {
        public EntryComponentFactory()
        {
        }

        public Bundle.EntryComponent Create()
        {
            Bundle.EntryComponent entryComponent;

            try
            {
                entryComponent = new Bundle.EntryComponent();
            }
            finally
            {
            }

            return entryComponent;
        }

        public Bundle.EntryComponent Create(
            Resource resource)
        {
            Bundle.EntryComponent entryComponent;

            try
            {
                entryComponent = new Bundle.EntryComponent()
                {
                    Resource = resource
                };
            }
            finally
            {
            }

            return entryComponent;
        }
    }
}