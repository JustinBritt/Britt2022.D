namespace Britt2022.D.Interfaces.Calculations
{
    using System;
    using System.Collections.Immutable;

    using Hl7.Fhir.Model;

    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;

    public interface IACalculation
    {
        ImmutableList<Tuple<Organization, INullableValue<int>, Duration>> Calculate(
            IDurationFactory durationFactory,
            ImmutableSortedSet<INullableValue<int>> clusters,
            Bundle surgeons,
            ImmutableSortedSet<INullableValue<int>> scenarios,
            ImmutableList<Tuple<Organization, PositiveInt, PositiveInt, FhirDecimal>> D,
            ImmutableList<Tuple<Organization, PositiveInt, FhirDecimal>> f,
            ImmutableList<Tuple<Organization, PositiveInt, FhirDecimal>> O);
    }
}