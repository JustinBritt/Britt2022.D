namespace Britt2022.D.Interfaces.Calculations
{
    using System;
    using System.Collections.Immutable;

    using Hl7.Fhir.Model;

    using NGenerics.DataStructures.Trees;

    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;

    public interface InCalculation
    {
        ImmutableList<Tuple<Organization, INullableValue<int>, INullableValue<int>>> Calculate(
            INullableValueFactory nullableValueFactory,
            RedBlackTree<Organization, RedBlackTree<INullableValue<int>, Duration>> A,
            Duration v);
    }
}