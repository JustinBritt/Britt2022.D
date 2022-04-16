namespace Britt2022.D.Interfaces.Calculations
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;

    using Hl7.Fhir.Model;

    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;
    using Britt2022.D.InterfacesFactories.Dependencies.MathNet.Numerics.Distributions;

    public interface IpCalculation
    {
        ImmutableList<Tuple<Organization, PositiveInt, PositiveInt, FhirDecimal>> GenerateScenarios(
            INullableValueFactory nullableValueFactory,
            IDiscreteUniformFactory discreteUniformFactory,
            ImmutableList<PositiveInt> lengthOfStayDays,
            ImmutableList<PositiveInt> scenarios,
            Organization surgeon,
            ImmutableList<KeyValuePair<Organization, PositiveInt>> surgeonLengthOfStayMaximums,
            double targetMean);
    }
}