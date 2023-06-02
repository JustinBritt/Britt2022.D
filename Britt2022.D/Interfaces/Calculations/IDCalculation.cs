namespace Britt2022.D.Interfaces.Calculations
{
    using System;
    using System.Collections.Immutable;

    using Hl7.Fhir.Model;

    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;
    using Britt2022.D.InterfacesFactories.Dependencies.MathNet.Numerics.Distributions;

    public interface IDCalculation
    {
        ImmutableList<Tuple<Organization, INullableValue<int>, INullableValue<int>, INullableValue<decimal>>> CalculateLogNormal(
            INullableValueFactory nullableValueFactory,
            ILogNormalFactory logNormalFactory,
            PositiveInt cluster,
            ImmutableList<PositiveInt> scenarios,
            Organization surgeon,
            double µ,
            double σ);
    }
}