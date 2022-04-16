namespace Britt2022.D.Factories.Dependencies.MathNet.Numerics.Distributions
{
    using global::MathNet.Numerics.Distributions;

    using Britt2022.D.InterfacesFactories.Dependencies.MathNet.Numerics.Distributions;

    internal sealed class LogNormalFactory : ILogNormalFactory
    {
        public LogNormalFactory()
        {
        }

        public IContinuousDistribution Create(
            double µ,
            double σ)
        {
            IContinuousDistribution continuousDistribution;

            try
            {
                continuousDistribution = new LogNormal(
                    mu: µ,
                    sigma: σ);
            }
            finally
            {
            }

            return continuousDistribution;
        }
    }
}