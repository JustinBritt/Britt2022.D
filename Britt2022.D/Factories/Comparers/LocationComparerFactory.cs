namespace Britt2022.D.Factories.Comparers
{
    using Britt2022.D.Classes.Comparers;
    using Britt2022.D.Interfaces.Comparers;
    using Britt2022.D.InterfacesFactories.Comparers;

    internal sealed class LocationComparerFactory : ILocationComparerFactory
    {
        public LocationComparerFactory()
        {
        }

        public ILocationComparer Create()
        {
            ILocationComparer instance = null;

            try
            {
                instance = new LocationComparer();
            }
            finally
            {
            }

            return instance;
        }
    }
}