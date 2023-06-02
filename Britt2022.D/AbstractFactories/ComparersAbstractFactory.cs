namespace Britt2022.D.AbstractFactories
{
    using Britt2022.D.Factories.Comparers;
    using Britt2022.D.InterfacesAbstractFactories;
    using Britt2022.D.InterfacesFactories.Comparers;

    internal sealed class ComparersAbstractFactory : IComparersAbstractFactory
    {
        public ComparersAbstractFactory()
        {
        }

        public IFhirDateTimeComparerFactory CreateFhirDateTimeComparerFactory()
        {
            IFhirDateTimeComparerFactory factory = null;

            try
            {
                factory = new FhirDateTimeComparerFactory();
            }
            finally
            {
            }

            return factory;
        }

        public ILocationComparerFactory CreateLocationComparerFactory()
        {
            ILocationComparerFactory factory = null;

            try
            {
                factory = new LocationComparerFactory();
            }
            finally
            {
            }

            return factory;
        }

        public INullableValueintComparerFactory CreateNullableValueintComparerFactory()
        {
            INullableValueintComparerFactory factory = null;

            try
            {
                factory = new NullableValueintComparerFactory();
            }
            finally
            {
            }

            return factory;
        }

        public IOrganizationComparerFactory CreateOrganizationComparerFactory()
        {
            IOrganizationComparerFactory factory = null;

            try
            {
                factory = new OrganizationComparerFactory();
            }
            finally
            {
            }

            return factory;
        }
    }
}