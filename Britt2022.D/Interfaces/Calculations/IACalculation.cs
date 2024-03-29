﻿namespace Britt2022.D.Interfaces.Calculations
{
    using System;
    using System.Collections.Immutable;

    using Hl7.Fhir.Model;

    using NGenerics.DataStructures.Trees;

    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;

    public interface IACalculation
    {
        ImmutableList<Tuple<Organization, INullableValue<int>, Duration>> Calculate(
            IDurationFactory durationFactory,
            ImmutableSortedSet<INullableValue<int>> clusters,
            Bundle surgeons,
            ImmutableSortedSet<INullableValue<int>> scenarios,
            RedBlackTree<Organization, RedBlackTree<INullableValue<int>, RedBlackTree<INullableValue<int>, INullableValue<decimal>>>> D,
            RedBlackTree<Organization, RedBlackTree<INullableValue<int>, INullableValue<decimal>>> f,
            RedBlackTree<Organization, RedBlackTree<INullableValue<int>, INullableValue<decimal>>> O);
    }
}