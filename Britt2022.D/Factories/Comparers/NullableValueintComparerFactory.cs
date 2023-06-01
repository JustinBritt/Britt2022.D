namespace Britt2022.D.Factories.Comparers
{
    using Britt2022.D.Classes.Comparers;
    using Britt2022.D.Interfaces.Comparers;
    using Britt2022.D.InterfacesFactories.Comparers;

    internal sealed class NullableValueintComparerFactory : INullableValueintComparerFactory
    {
        public NullableValueintComparerFactory()
        {
        }

        public INullableValueintComparer Create()
        {
            INullableValueintComparer instance = null;

            try
            {
                instance = new NullableValueintComparer();
            }
            finally
            {
            }

            return instance;
        }
    }
}