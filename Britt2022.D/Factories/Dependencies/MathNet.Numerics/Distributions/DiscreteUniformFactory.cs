namespace Britt2022.D.Factories.Dependencies.MathNet.Numerics.Distributions
{
    using global::MathNet.Numerics.Distributions;

    using Britt2022.D.InterfacesFactories.Dependencies.MathNet.Numerics.Distributions;

    internal sealed class DiscreteUniformFactory : IDiscreteUniformFactory
    {
        public DiscreteUniformFactory()
        {
        }

        public IDiscreteDistribution Create(
            int lower,
            int upper)
        {
            IDiscreteDistribution discreteDistribution;

            try
            {
                discreteDistribution = new DiscreteUniform(
                    lower: lower,
                    upper: upper);
            }
            finally
            {
            }

            return discreteDistribution;
        }
    }
}