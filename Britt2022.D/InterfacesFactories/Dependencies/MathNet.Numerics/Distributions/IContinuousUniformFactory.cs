namespace Britt2022.D.InterfacesFactories.Dependencies.MathNet.Numerics.Distributions
{
    using global::MathNet.Numerics.Distributions;

    public interface IContinuousUniformFactory
    {
        IContinuousDistribution Create(
            double lower,
            double upper);
    }
}