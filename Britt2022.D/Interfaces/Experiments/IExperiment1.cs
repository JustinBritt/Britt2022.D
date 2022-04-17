namespace Britt2022.D.Interfaces.Experiments
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Hl7.Fhir.Model;

    public interface IExperiment1
    {
        /// <summary>
        /// Gets the clusters.
        /// Index: e
        /// </summary>
        ImmutableList<PositiveInt> Clusters { get; }

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
        ImmutableList<KeyValuePair<PositiveInt, FhirDateTime>> PlanningHorizon { get; }

        /// <summary>
        /// Gets the length of stay days.
        /// Index: l
        /// </summary>
        ImmutableList<PositiveInt> LengthOfStayDays { get; }

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
        ImmutableList<PositiveInt> Scenarios { get; }

        /// <summary>
        /// Gets the weighted average surgical durations.
        /// Parameter: A(i, ω)
        /// </summary>
        ImmutableList<Tuple<Organization, PositiveInt, Duration>> WeightedAverageSurgicalDurations { get; }

        /// <summary>
        /// Gets SurgicalSpecialtyStrategicTargetNumberTimeBlocks.
        /// Parameter: B(r)
        /// </summary>
        ImmutableList<KeyValuePair<Organization, PositiveInt>> SurgicalSpecialtyStrategicTargetNumberTimeBlocks { get; }

        /// <summary>
        /// Gets the surgical durations.
        /// Parameter: D(i, e, ω)
        /// </summary>
        ImmutableList<Tuple<Organization, PositiveInt, PositiveInt, FhirDecimal>> SurgicalDurations { get; }

        /// <summary>
        /// Gets SurgeonClusterDurationMeans.
        /// Parameter: D(i, e, ω) (Means)
        /// </summary>
        ImmutableList<Tuple<Organization, PositiveInt, Duration>> SurgeonClusterDurationMeans { get; }

        /// <summary>
        /// Gets SurgeonClusterDurationStandardDeviations.
        /// Parameter: D(i, e, ω) (Standard deviations)
        /// </summary>
        ImmutableList<Tuple<Organization, PositiveInt, Duration>> SurgeonClusterDurationStandardDeviations { get; }

        /// <summary>
        /// Gets the surgical frequencies.
        /// Parameter: f(i, e)
        /// </summary>
        ImmutableList<Tuple<Organization, PositiveInt, FhirDecimal>> SurgicalFrequencies { get; }
    }
}