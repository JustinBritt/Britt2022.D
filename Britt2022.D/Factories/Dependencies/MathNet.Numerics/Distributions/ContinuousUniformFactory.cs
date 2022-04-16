namespace Britt2022.D.Factories.Dependencies.MathNet.Numerics.Distributions
{
    using global::MathNet.Numerics.Distributions;

    using Britt2022.D.InterfacesFactories.Dependencies.MathNet.Numerics.Distributions;

    internal sealed class ContinuousUniformFactory : IContinuousUniformFactory
    {
        public ContinuousUniformFactory()
        {
        }

        public IContinuousDistribution Create(
            double lower,
            double upper)
        {
            IContinuousDistribution continuousDistribution;

            try
            {
                continuousDistribution = new ContinuousUniform(
                    lower: lower,
                    upper: upper);
            }
            finally
            {
            }

            return continuousDistribution;
        }
    }
}