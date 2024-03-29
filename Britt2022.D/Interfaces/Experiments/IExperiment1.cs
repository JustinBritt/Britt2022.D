﻿namespace Britt2022.D.Interfaces.Experiments
{
    using System;
    using System.Collections.Immutable;

    using Hl7.Fhir.Model;

    using NGenerics.DataStructures.Trees;

    public interface IExperiment1
    {
        /// <summary>
        /// Gets the clusters.
        /// Index: e
        /// </summary>
        ImmutableSortedSet<INullableValue<int>> Clusters { get; }

        /// <summary>
        /// Gets the surgeons.
        /// Index: i
        /// </summary>
        Bundle Surgeons { get; }

        /// <summary>
        /// Gets the operating rooms.
        /// Index: j
        /// </summary>
        Bundle OperatingRooms { get; }

        /// <summary>
        /// Gets the planning horizon.
        /// Index: k
        /// </summary>
        RedBlackTree<INullableValue<int>, FhirDateTime> PlanningHorizon { get; }

        /// <summary>
        /// Gets the length of stay days.
        /// Index: l
        /// </summary>
        ImmutableSortedSet<INullableValue<int>> LengthOfStayDays { get; }

        /// <summary>
        /// Gets the General / Mixed (GEN / MIX) surgical specialty.
        /// Index: r
        /// </summary>
        Organization SurgicalSpecialty1GEN { get; }

        /// <summary>
        /// Gets the Gynecology (GYN) surgical specialty.
        /// Index: r
        /// </summary>
        Organization SurgicalSpecialty2GYN { get; }

        /// <summary>
        /// Gets the Plastic (PLA) surgical specialty.
        /// Index: r
        /// </summary>
        Organization SurgicalSpecialty3PLA { get; }

        /// <summary>
        /// Gets the Ear-Nose-Throat (ENT) surgical specialty.
        /// Index: r
        /// </summary>
        Organization SurgicalSpecialty4ENT { get; }

        /// <summary>
        /// Gets the Orthopedic (ORT) surgical specialty.
        /// Index: r
        /// </summary>
        Organization SurgicalSpecialty5ORT { get; }

        /// <summary>
        /// Gets the Urology (URO) surgical specialty.
        /// Index: r
        /// </summary>
        Organization SurgicalSpecialty6URO { get; }

        /// <summary>
        /// Gets the scenarios.
        /// Index: ω
        /// </summary>
        ImmutableSortedSet<INullableValue<int>> Scenarios { get; }

        /// <summary>
        /// Gets the weighted average surgical durations.
        /// Parameter: A(i, ω)
        /// </summary>
        RedBlackTree<Organization, RedBlackTree<INullableValue<int>, Duration>> WeightedAverageSurgicalDurations { get; }

        /// <summary>
        /// Gets SurgicalSpecialtyStrategicTargetNumberTimeBlocks.
        /// Parameter: B(r)
        /// </summary>
        RedBlackTree<Organization, INullableValue<int>> SurgicalSpecialtyStrategicTargetNumberTimeBlocks { get; }

        /// <summary>
        /// Gets the surgical durations.
        /// Parameter: D(i, e, ω)
        /// </summary>
        RedBlackTree<Organization, RedBlackTree<INullableValue<int>, RedBlackTree<INullableValue<int>, INullableValue<decimal>>>> SurgicalDurations { get; }

        /// <summary>
        /// Gets the surgical frequencies.
        /// Parameter: f(i, e)
        /// </summary>
        RedBlackTree<Organization, RedBlackTree<INullableValue<int>, INullableValue<decimal>>> SurgicalFrequencies { get; }

        /// <summary>
        /// Gets SurgeonMaximumNumberTimeBlocks.
        /// Parameter: H(i)
        /// </summary>
        RedBlackTree<Organization, INullableValue<int>> SurgeonMaximumNumberTimeBlocks { get; }

        /// <summary>
        /// Gets SurgeonLengthOfStayMaximums.
        /// Parameter: h(i)
        /// </summary>
        RedBlackTree<Organization, INullableValue<int>> SurgeonLengthOfStayMaximums { get; }

        /// <summary>
        /// Gets SurgeonMinimumNumberTimeBlocks.
        /// Parameter: L(i)
        /// </summary>
        RedBlackTree<Organization, INullableValue<int>> SurgeonMinimumNumberTimeBlocks { get; }

        /// <summary>
        /// Gets SurgeonStrategicTargets.
        /// Parameter: N(i)
        /// </summary>
        RedBlackTree<Organization, INullableValue<int>> SurgeonStrategicTargets { get; }

        /// <summary>
        /// Gets SurgeonScenarioMaximumNumberPatients.
        /// Parameter: n(i, ω)
        /// </summary>
        RedBlackTree<Organization, RedBlackTree<INullableValue<int>, INullableValue<int>>> SurgeonScenarioMaximumNumberPatients { get; }

        /// <summary>
        /// Gets the surgical overheads.
        /// Parameter: O(i, e)
        /// </summary>
        RedBlackTree<Organization, RedBlackTree<INullableValue<int>, INullableValue<decimal>>> SurgicalOverheads { get; }

        /// <summary>
        /// Gets SurgeonDayScenarioLengthOfStayProbabilities.
        /// Parameter: p(i, l, ω)
        /// </summary>
        RedBlackTree<Organization, RedBlackTree<INullableValue<int>, RedBlackTree<INullableValue<int>, INullableValue<decimal>>>> SurgeonDayScenarioLengthOfStayProbabilities { get; }

        /// <summary>
        /// Gets the surgical specialties.
        /// Parameter: S(r)
        /// </summary>
        RedBlackTree<Organization, ImmutableSortedSet<Organization>> SurgicalSpecialties { get; }

        /// <summary>
        /// Gets the time block length.
        /// Parameter: v
        /// </summary>
        Duration TimeBlockLength { get; }

        /// <summary>
        /// Gets the goal weights.
        /// Parameter: (w1, w2, w3, w4)
        /// </summary>
        RedBlackTree<INullableValue<int>, Tuple<INullableValue<decimal>, INullableValue<decimal>, INullableValue<decimal>, INullableValue<decimal>>> GoalWeights { get; }

        /// <summary>
        /// Gets SurgeonOperatingRoomAvailabilities.
        /// Parameter: Π(i, j)
        /// </summary>
        RedBlackTree<Organization, RedBlackTree<Location, INullableValue<bool>>> SurgeonOperatingRoomAvailabilities { get; }

        /// <summary>
        /// Gets the scenario probabilities.
        /// Parameter: Ρ(ω)
        /// </summary>
        RedBlackTree<INullableValue<int>, INullableValue<decimal>> ScenarioProbabilities { get; }

        /// <summary>
        /// Gets SurgeonDayScenarioCumulativeNumberPatients.
        /// Parameter: Φ(i, l, ω)
        /// </summary>
        RedBlackTree<Organization, RedBlackTree<INullableValue<int>, RedBlackTree<INullableValue<int>, INullableValue<decimal>>>> SurgeonDayScenarioCumulativeNumberPatients { get; }

        /// <summary>
        /// Gets SurgeonDayAvailabilities.
        /// Parameter: Ω(i, k)
        /// </summary>
        RedBlackTree<Organization, RedBlackTree<FhirDateTime, INullableValue<bool>>> SurgeonDayAvailabilities { get; }
    }
}