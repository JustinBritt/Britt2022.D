namespace Britt2022.D.Classes.Calculations
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;

    using log4net;

    using Hl7.Fhir.Model;

    using NGenerics.DataStructures.Trees;

    using Britt2022.D.Interfaces.Calculations;
    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;
    
    internal sealed class ACalculation : IACalculation
    {
        private ILog Log => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ACalculation()
        {
        }

        public ImmutableList<Tuple<Organization, INullableValue<int>, Duration>> Calculate(
            IDurationFactory durationFactory,
            ImmutableSortedSet<INullableValue<int>> clusters,
            Bundle surgeons,
            ImmutableSortedSet<INullableValue<int>> scenarios,
            RedBlackTree<Organization, RedBlackTree<INullableValue<int>, RedBlackTree<INullableValue<int>, INullableValue<decimal>>>> D,
            RedBlackTree<Organization, RedBlackTree<INullableValue<int>, INullableValue<decimal>>> f,
            RedBlackTree<Organization, RedBlackTree<INullableValue<int>, INullableValue<decimal>>> O)
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
                            D[i.Item1][k][i.Item2].Value.Value
                            *
                            O[i.Item1][k].Value.Value
                            *
                            f[i.Item1][k].Value.Value)
                        .Sum())))
                .ToImmutableList();
        }
    }
}