namespace Britt2022.D.Interfaces.Experiments
{
    using System;
    using System.Collections.Generic;
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
        ImmutableList<INullableValue<int>> LengthOfStayDays { get; }

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
        ImmutableList<Tuple<Organization, INullableValue<int>, Duration>> WeightedAverageSurgicalDurations { get; }

        /// <summary>
        /// Gets SurgicalSpecialtyStrategicTargetNumberTimeBlocks.
        /// Parameter: B(r)
        /// </summary>
        ImmutableList<KeyValuePair<Organization, INullableValue<int>>> SurgicalSpecialtyStrategicTargetNumberTimeBlocks { get; }

        /// <summary>
        /// Gets the surgical durations.
        /// Parameter: D(i, e, ω)
        /// </summary>
        ImmutableList<Tuple<Organization, INullableValue<int>, INullableValue<int>, FhirDecimal>> SurgicalDurations { get; }

        /// <summary>
        /// Gets SurgeonClusterDurationMeans.
        /// Parameter: D(i, e, ω) (Means)
        /// </summary>
        ImmutableList<Tuple<Organization, INullableValue<int>, Duration>> SurgeonClusterDurationMeans { get; }

        /// <summary>
        /// Gets SurgeonClusterDurationStandardDeviations.
        /// Parameter: D(i, e, ω) (Standard deviations)
        /// </summary>
        ImmutableList<Tuple<Organization, INullableValue<int>, Duration>> SurgeonClusterDurationStandardDeviations { get; }

        /// <summary>
        /// Gets the surgical frequencies.
        /// Parameter: f(i, e)
        /// </summary>
        ImmutableList<Tuple<Organization, INullableValue<int>, FhirDecimal>> SurgicalFrequencies { get; }

        /// <summary>
        /// Gets SurgeonMaximumNumberTimeBlocks.
        /// Parameter: H(i)
        /// </summary>
        ImmutableList<KeyValuePair<Organization, INullableValue<int>>> SurgeonMaximumNumberTimeBlocks { get; }

        /// <summary>
        /// Gets SurgeonLengthOfStayMaximums.
        /// Parameter: h(i)
        /// </summary>
        ImmutableList<KeyValuePair<Organization, INullableValue<int>>> SurgeonLengthOfStayMaximums { get; }

        /// <summary>
        /// Gets SurgeonMinimumNumberTimeBlocks.
        /// Parameter: L(i)
        /// </summary>
        ImmutableList<KeyValuePair<Organization, INullableValue<int>>> SurgeonMinimumNumberTimeBlocks { get; }

        /// <summary>
        /// Gets SurgeonStrategicTargets.
        /// Parameter: N(i)
        /// </summary>
        ImmutableList<KeyValuePair<Organization, INullableValue<int>>> SurgeonStrategicTargets { get; }

        /// <summary>
        /// Gets SurgeonScenarioMaximumNumberPatients.
        /// Parameter: n(i, ω)
        /// </summary>
        ImmutableList<Tuple<Organization, INullableValue<int>, INullableValue<int>>> SurgeonScenarioMaximumNumberPatients { get; }

        /// <summary>
        /// Gets the surgical overheads.
        /// Parameter: O(i, e)
        /// </summary>
        ImmutableList<Tuple<Organization, INullableValue<int>, FhirDecimal>> SurgicalOverheads { get; }

        /// <summary>
        /// Gets SurgeonDayScenarioLengthOfStayProbabilities.
        /// Parameter: p(i, l, ω)
        /// </summary>
        ImmutableList<Tuple<Organization, INullableValue<int>, INullableValue<int>, FhirDecimal>> SurgeonDayScenarioLengthOfStayProbabilities { get; }

        /// <summary>
        /// Gets the surgical specialties.
        /// Parameter: S(r)
        /// </summary>
        ImmutableList<Tuple<Organization, ImmutableList<Organization>>> SurgicalSpecialties { get; }

        /// <summary>
        /// Gets the time block length.
        /// Parameter: v
        /// </summary>
        Duration TimeBlockLength { get; }

        /// <summary>
        /// Gets the goal weights.
        /// Parameter: (w1, w2, w3, w4)
        /// </summary>
        ImmutableList<Tuple<FhirDecimal, FhirDecimal, FhirDecimal, FhirDecimal>> GoalWeights { get; }

        /// <summary>
        /// Gets SurgeonOperatingRoomAvailabilities.
        /// Parameter: Π(i, j)
        /// </summary>
        ImmutableList<Tuple<Organization, Location, FhirBoolean>> SurgeonOperatingRoomAvailabilities { get; }

        /// <summary>
        /// Gets the scenario probabilities.
        /// Parameter: Ρ(ω)
        /// </summary>
        ImmutableList<KeyValuePair<INullableValue<int>, FhirDecimal>> ScenarioProbabilities { get; }

        /// <summary>
        /// Gets SurgeonDayScenarioCumulativeNumberPatients.
        /// Parameter: Φ(i, l, ω)
        /// </summary>
        ImmutableList<Tuple<Organization, PositiveInt, PositiveInt, FhirDecimal>> SurgeonDayScenarioCumulativeNumberPatients { get; }

        /// <summary>
        /// Gets SurgeonDayAvailabilities.
        /// Parameter: Ω(i, k)
        /// </summary>
        ImmutableList<Tuple<Organization, FhirDateTime, FhirBoolean>> SurgeonDayAvailabilities { get; }
    }
}