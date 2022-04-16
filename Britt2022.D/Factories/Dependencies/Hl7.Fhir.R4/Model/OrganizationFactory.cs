namespace Britt2022.D.Factories.Dependencies.Hl7.Fhir.R4.Model
{
    using global::Hl7.Fhir.Model;

    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;

    internal sealed class OrganizationFactory : IOrganizationFactory
    {
        public OrganizationFactory()
        {
        }

        public Organization Create()
        {
            Organization organization;

            try
            {
                organization = new Organization();
            }
            finally
            {
            }

            return organization;
        }

        public Organization Create(
            string id)
        {
            Organization organization;

            try
            {
                organization = new Organization()
                {
                    Id = id
                };
            }
            finally
            {
            }

            return organization;
        }
    }
}