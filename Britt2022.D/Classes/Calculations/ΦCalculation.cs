﻿namespace Britt2022.D.Classes.Calculations
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Hl7.Fhir.Model;

    using NGenerics.DataStructures.Trees;

    using Britt2022.D.Interfaces.Calculations;
    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;

    internal sealed class ΦCalculation : IΦCalculation
    {
        public ΦCalculation()
        {
        }

        private int CalculateτLowerBound()
        {
            return 0;
        }

        private int CalculateτUpperBound(
            Organization iIndexElement,
            RedBlackTree<INullableValue<int>, FhirDateTime> planningHorizon,
            RedBlackTree<Organization, INullableValue<int>> surgeonLengthOfStayMaximums)
        {
            return (int)Math.Floor(
                    Convert.ToDecimal(surgeonLengthOfStayMaximums.Where(w => w.Key == iIndexElement).Select(w => w.Value.Value.Value).SingleOrDefault())
                    /
                    Convert.ToDecimal(planningHorizon.Select(w => w.Key.Value.Value).Max()));
        }

        private ImmutableList<Tuple<Organization, int>> CalculateτIndex(
            Bundle surgeons,
            RedBlackTree<INullableValue<int>, FhirDateTime> planningHorizon,
            RedBlackTree<Organization, INullableValue<int>> surgeonLengthOfStayMaximums)
        {
            ImmutableList<Tuple<Organization, int>>.Builder builder = ImmutableList.CreateBuilder<Tuple<Organization, int>>();

            foreach (Organization iIndexElement in surgeons.Entry.Where(x => x.Resource is Organization).Select(x => (Organization)x.Resource))
            {
                int τLowerBound = this.CalculateτLowerBound();

                int τUpperBound = this.CalculateτUpperBound(
                    iIndexElement,
                    planningHorizon,
                    surgeonLengthOfStayMaximums);

                for (int tau = τLowerBound; tau <= τUpperBound; tau = tau + 1)
                {
                    builder.Add(
                        Tuple.Create(
                            iIndexElement,
                            tau));
                }
            }

            return builder.ToImmutableList();
        }

        private decimal? CalculatepSum(
            Organization iIndexElement,
            INullableValue<int> lIndexElement,
            INullableValue<int> ωIndexElement,
            RedBlackTree<INullableValue<int>, FhirDateTime> planningHorizon,
            ImmutableSortedSet<INullableValue<int>> lengthOfStayDays,
            ImmutableList<Tuple<Organization, int>> τ,
            RedBlackTree<Organization, INullableValue<int>> surgeonLengthOfStayMaximums,
            RedBlackTree<Organization, RedBlackTree<INullableValue<int>, RedBlackTree<INullableValue<int>, INullableValue<decimal>>>> surgeonDayScenarioLengthOfStayProbabilities)
        {
            decimal? sum = 0;

            foreach (int tau in τ.Where(x => x.Item1 == iIndexElement).Select(x => x.Item2))
            {
                int lPrimeLowerBound = lIndexElement.Value.Value + planningHorizon.Select(w => w.Key.Value.Value).Max() * tau + 1;

                int lPrimeUpperBound = surgeonLengthOfStayMaximums.Where(w => w.Key == iIndexElement).Select(w => w.Value.Value.Value).SingleOrDefault();

                for (int lPrime = lPrimeLowerBound; lPrime <= lPrimeUpperBound; lPrime = lPrime + 1)
                {
                    sum += surgeonDayScenarioLengthOfStayProbabilities[iIndexElement][lengthOfStayDays.Where(w => w.Value.Value == lPrime).SingleOrDefault()][ωIndexElement].Value.Value;
                }
            }

            return sum;
        }

        public ImmutableList<Tuple<Organization, INullableValue<int>, INullableValue<int>, INullableValue<decimal>>> Calculate(
            INullableValueFactory nullableValueFactory,
            Bundle surgeons,
            RedBlackTree<INullableValue<int>, FhirDateTime> planningHorizon,
            ImmutableSortedSet<INullableValue<int>> lengthOfStayDays,
            ImmutableSortedSet<INullableValue<int>> scenarios,
            RedBlackTree<Organization, INullableValue<int>> surgeonLengthOfStayMaximums,
            RedBlackTree<Organization, RedBlackTree<INullableValue<int>, INullableValue<int>>> surgeonScenarioMaximumNumberPatients,
            RedBlackTree<Organization, RedBlackTree<INullableValue<int>, RedBlackTree<INullableValue<int>, INullableValue<decimal>>>> surgeonDayScenarioLengthOfStayProbabilities)
        {
            ImmutableList<Tuple<Organization, INullableValue<int>, INullableValue<int>, INullableValue<decimal>>>.Builder builder = ImmutableList.CreateBuilder<Tuple<Organization, INullableValue<int>, INullableValue<int>, INullableValue<decimal>>>();

            ImmutableList<Tuple<Organization, int>> τ = this.CalculateτIndex(
                surgeons,
                planningHorizon,
                surgeonLengthOfStayMaximums);

            foreach (INullableValue<int> ωIndexElement in scenarios)
            {
                foreach (Organization iIndexElement in surgeons.Entry.Where(x => x.Resource is Organization).Select(x => (Organization)x.Resource))
                {
                    foreach (INullableValue<int> lIndexElement in lengthOfStayDays)
                    {
                        builder.Add(
                            Tuple.Create(
                                iIndexElement,
                                lIndexElement,
                                ωIndexElement,
                                nullableValueFactory.Create<decimal>(
                                    surgeonScenarioMaximumNumberPatients[iIndexElement][ωIndexElement].Value.Value
                                    *
                                    this.CalculatepSum(
                                        iIndexElement,
                                        lIndexElement,
                                        ωIndexElement,
                                        planningHorizon,
                                        lengthOfStayDays,
                                        τ,
                                        surgeonLengthOfStayMaximums,
                                        surgeonDayScenarioLengthOfStayProbabilities))));
                    }
                }
            }

            return builder.ToImmutableList();
        }
    }
}