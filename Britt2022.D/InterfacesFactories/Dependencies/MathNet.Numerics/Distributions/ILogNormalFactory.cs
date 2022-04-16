namespace Britt2022.D.InterfacesFactories.Dependencies.MathNet.Numerics.Distributions
{
    using global::MathNet.Numerics.Distributions;

    public interface ILogNormalFactory
    {
        IContinuousDistribution Create(
            double µ,
            double σ);
    }
}