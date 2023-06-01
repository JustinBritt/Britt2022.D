namespace Britt2022.D.Interfaces.Calculations
{
    using System;
    using System.Collections.Immutable;

    using Hl7.Fhir.Model;

    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;

    public interface InCalculation
    {
        ImmutableList<Tuple<Organization, INullableValue<int>, PositiveInt>> Calculate(
            INullableValueFactory nullableValueFactory,
            ImmutableList<Tuple<Organization, INullableValue<int>, Duration>> A,
            Duration v);
    }
}