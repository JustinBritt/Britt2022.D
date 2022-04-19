namespace Britt2022.D.Classes.Calculations
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;

    using log4net;

    using Hl7.Fhir.Model;

    using Britt2022.D.Interfaces.Calculations;
    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;

    internal sealed class ACalculation : IACalculation
    {
        private ILog Log => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ACalculation()
        {
        }

        public ImmutableList<Tuple<Organization, PositiveInt, Duration>> Calculate(
            IDurationFactory durationFactory,
            ImmutableList<PositiveInt> clusters,
            Bundle surgeons,
            ImmutableList<PositiveInt> scenarios,
            ImmutableList<Tuple<Organization, PositiveInt, PositiveInt, FhirDecimal>> D,
            ImmutableList<Tuple<Organization, PositiveInt, FhirDecimal>> f,
            ImmutableList<Tuple<Organization, PositiveInt, FhirDecimal>> O)
        {
            return surgeons.Entry
                .Where(x => x.Resource is Organization)
                .Select(x => (Organization)x.Resource)
                .SelectMany(b => scenarios, (a, b) => Tuple.Create(a, b))
                .Select(i => Tuple.Create(
                    i.Item1, // i.Item1: Surgeon
                    i.Item2, // i.Item2: Scenario
                    durationFactory.CreateHour(
                        value: clusters
                        .Select(
                            k =>  // k: Cluster
                            D.Where(j => j.Item1 == i.Item1 && j.Item2 == k && j.Item3 == i.Item2)
                            .Select(j => j.Item4.Value.Value)
                            .SingleOrDefault()
                            *
                            O.Where(j => j.Item1 == i.Item1 && j.Item2 == k)
                            .Select(j => j.Item3.Value.Value)
                            .SingleOrDefault()
                            *
                            f.Where(j => j.Item1 == i.Item1 && j.Item2 == k)
                            .Select(j => j.Item3.Value.Value)
                            .SingleOrDefault())
                        .Sum())))
                .ToImmutableList();
        }
    }
}