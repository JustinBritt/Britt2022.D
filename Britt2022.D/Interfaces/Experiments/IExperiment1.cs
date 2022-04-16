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
    }
}