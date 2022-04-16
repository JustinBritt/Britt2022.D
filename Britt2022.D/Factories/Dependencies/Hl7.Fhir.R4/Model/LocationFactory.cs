namespace Britt2022.D.Factories.Dependencies.Hl7.Fhir.R4.Model
{
    using global::Hl7.Fhir.Model;

    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;

    internal sealed class LocationFactory : ILocationFactory
    {
        public LocationFactory()
        {
        }

        public Location Create()
        {
            Location location;

            try
            {
                location = new Location();
            }
            finally
            {
            }

            return location;
        }

        public Location Create(
            string id)
        {
            Location location;

            try
            {
                location = new Location()
                {
                    Id = id
                };
            }
            finally
            {
            }

            return location;
        }
    }
}