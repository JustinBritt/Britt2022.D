namespace Britt2022.D.Interfaces.Calculations
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;

    using Hl7.Fhir.Model;

    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;

    public interface IΦCalculation
    {
        ImmutableList<Tuple<Organization, PositiveInt, PositiveInt, FhirDecimal>> Calculate(
            INullableValueFactory nullableValueFactory,
            Bundle surgeons,
            ImmutableList<KeyValuePair<PositiveInt, FhirDateTime>> planningHorizon,
            ImmutableList<INullableValue<int>> lengthOfStayDays,
            ImmutableSortedSet<INullableValue<int>> scenarios,
            ImmutableList<KeyValuePair<Organization, PositiveInt>> surgeonLengthOfStayMaximums,
            ImmutableList<Tuple<Organization, PositiveInt, PositiveInt>> surgeonScenarioMaximumNumberPatients,
            ImmutableList<Tuple<Organization, PositiveInt, PositiveInt, FhirDecimal>> surgeonDayScenarioLengthOfStayProbabilities);
    }
}