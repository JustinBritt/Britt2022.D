namespace Britt2022.D.Classes.Experiments
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using log4net;

    using Hl7.Fhir.Model;

    using MathNet.Numerics.Distributions;

    using NGenerics.DataStructures.Trees;

    using Britt2022.D.Extensions.Dependencies.Hl7.Fhir.R4.Model;
    using Britt2022.D.Interfaces.Calculations;
    using Britt2022.D.Interfaces.Experiments;
    using Britt2022.D.InterfacesAbstractFactories;
    using Britt2022.D.InterfacesFactories.Comparers;
    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;
    using Britt2022.D.InterfacesFactories.Dependencies.MathNet.Numerics.Distributions;
    using Britt2022.D.Classes.Comparers;

    public sealed class Experiment1 : IExperiment1
    {
        private ILog Log => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Experiment1(
            ICalculationsAbstractFactory calculationsAbstractFactory,
            IComparersAbstractFactory comparersAbstractFactory,
            IDependenciesAbstractFactory dependenciesAbstractFactory)
        {
            IBundleFactory bundleFactory = dependenciesAbstractFactory.CreateBundleFactory();

            IDurationFactory durationFactory = dependenciesAbstractFactory.CreateDurationFactory();

            IEntryComponentFactory entryComponentFactory = dependenciesAbstractFactory.CreateEntryComponentFactory();

            IFhirDateTimeFactory FhirDateTimeFactory = dependenciesAbstractFactory.CreateFhirDateTimeFactory();

            ILocationFactory locationFactory = dependenciesAbstractFactory.CreateLocationFactory();

            this.NullableValueFactory = dependenciesAbstractFactory.CreateNullableValueFactory();

            IOrganizationFactory organizationFactory = dependenciesAbstractFactory.CreateOrganizationFactory();

            IContinuousUniformFactory continuousUniformFactory = dependenciesAbstractFactory.CreateContinuousUniformFactory();

            IDiscreteUniformFactory discreteUniformFactory = dependenciesAbstractFactory.CreateDiscreteUniformFactory();

            ILogNormalFactory logNormalFactory = dependenciesAbstractFactory.CreateLogNormalFactory();

            // Clusters
            // Index: e

            // 8 Clusters in Van Houdenhoven et al. (2007)
            int numberClusters = 8;

            this.Clusters = this.GenerateClusters(
                comparersAbstractFactory.CreateNullableValueintComparerFactory(),
                this.NullableValueFactory,
                numberClusters);

            // Surgeons
            // Index: i
            int numberSurgeons = 30;

            this.Surgeons = this.GenerateSurgeons(
                bundleFactory,
                entryComponentFactory,
                organizationFactory,
                numberSurgeons);

            // OperatingRooms
            // Index: j
            int numberOperatingRooms = 9;

            this.OperatingRooms = this.GenerateOperatingRooms(
                bundleFactory,
                entryComponentFactory,
                locationFactory,
                numberOperatingRooms);

            // Planning Horizon
            // Index: k
            int planningHorizonLength = 28;

            DateTime startDate = new DateTime(
                2020,
                6,
                1);

            DateTime endDate = startDate.AddDays(
                planningHorizonLength - 1);

            this.PlanningHorizon = this.GeneratePlanningHorizon(
                FhirDateTimeFactory,
                this.NullableValueFactory,
                endDate,
                startDate);

            // Length of stay days
            // Index: l
            int maximumLengthOfStay = 5;

            this.LengthOfStayDays = this.GenerateLengthOfStayDays(
                this.NullableValueFactory,
                planningHorizonLength);

            // Surgical Specialties
            // Index: r
            this.SurgicalSpecialty1GEN = organizationFactory.Create(
                id: "1");

            this.SurgicalSpecialty2GYN = organizationFactory.Create(
                id: "2");

            this.SurgicalSpecialty3PLA = organizationFactory.Create(
                id: "3");

            this.SurgicalSpecialty4ENT = organizationFactory.Create(
                id: "4");

            this.SurgicalSpecialty5ORT = organizationFactory.Create(
                id: "5");

            this.SurgicalSpecialty6URO = organizationFactory.Create(
                id: "6");

            // Scenarios
            // Index: ω
            int numberScenarios = 16;

            this.Scenarios = this.GenerateScenarios(
                comparersAbstractFactory.CreateNullableValueintComparerFactory(),
                this.NullableValueFactory,
                numberScenarios);

            // SurgicalSpecialtyStrategicTargetNumberTimeBlocks
            // Parameter: B(r)
            this.SurgicalSpecialtyStrategicTargetNumberTimeBlocks = this.GenerateSurgicalSpecialtyStrategicTargetNumberTimeBlocks(
                this.NullableValueFactory);

            // SurgeonMaximumNumberTimeBlocks
            // Parameter: H(i)
            this.SurgeonMaximumNumberTimeBlocks = this.GenerateSurgeonMaximumNumberTimeBlocks(
                this.Surgeons);

            // SurgeonLengthOfStayMaximums
            // Parameter: h(i)
            this.SurgeonLengthOfStayMaximums = this.GenerateSurgeonLengthOfStayMaximumsSameForAllSurgeons(
                this.NullableValueFactory,
                maximumLengthOfStay,
                this.Surgeons);

            // SurgeonMinimumNumberTimeBlocks
            // L(i)
            this.SurgeonMinimumNumberTimeBlocks = this.GenerateSurgeonMinimumNumberTimeBlocks(
                this.Surgeons);

            // SurgeonStrategicTargets
            // N(i)
            this.SurgeonStrategicTargets = this.GenerateSurgeonStrategicTargets(
                this.Surgeons);

            // SurgicalOverheads
            // Parameter: O(i, e)
            this.SurgicalOverheads = this.GenerateSurgicalOverheads(
                this.NullableValueFactory,
                continuousUniformFactory,
                this.Clusters,
                this.Surgeons,
                lower: 1.0,
                upper: 1.5);

            // SurgicalSpecialties
            // Parameter: S(r)
            this.SurgicalSpecialties = this.GenerateSurgicalSpecialties();

            // TimeBlockLength
            // Parameter: v
            this.TimeBlockLength = durationFactory.CreateHour(
                8m);

            // GoalWeights
            // Parameter: (w1, w2, w3, w4)
            this.GoalWeights = this.GenerateGoalWeights(
                this.NullableValueFactory);

            // SurgeonOperatingRoomAvailabilities
            // Parameter: Π(i, j)
            this.SurgeonOperatingRoomAvailabilities = this.GenerateSurgeonOperatingRoomAvailabilities();

            // ScenarioProbabilities
            // Parameter: Ρ(ω)
            // Each scenario currently has an equal probability.
            this.ScenarioProbabilities = this.GenerateScenarioProbabilities(
                this.NullableValueFactory,
                this.Scenarios,
                numberScenarios);

            // SurgeonDayAvailabilities
            // Parameter: Ω(i, k)
            this.SurgeonDayAvailabilities = this.GenerateSurgeonDayAvailabilities(
                this.PlanningHorizon.Select(w => KeyValuePair.Create((PositiveInt)w.Key, w.Value)).ToImmutableList(),
                this.Surgeons);

            // SurgicalDurations
            // Parameter: D(i, e, ω)
            this.SurgicalDurations = this.GenerateSurgicalDurationsVanHoudenhoven2007(
                durationFactory,
                this.NullableValueFactory,
                logNormalFactory,
                calculationsAbstractFactory.CreateDCalculationFactory().Create(),
                this.Clusters,
                this.Scenarios,
                this.Surgeons,
                this.SurgicalSpecialties);

            // SurgicalFrequencies
            // Parameter: f(i, e)
            this.SurgicalFrequencies = this.GenerateSurgicalFrequenciesVanHoudenhoven2007(
                this.Clusters,
                this.Surgeons,
                this.SurgicalSpecialties);

            // WeightedAverageSurgicalDurations
            // Parameter: A(i, ω)
            this.WeightedAverageSurgicalDurations = calculationsAbstractFactory.CreateACalculationFactory().Create().Calculate(
                durationFactory,
                this.Clusters,
                this.Surgeons,
                this.Scenarios,
                this.SurgicalDurations,
                this.SurgicalFrequencies,
                this.SurgicalOverheads);

            // SurgeonScenarioMaximumNumberPatients
            // Parameter: n(i, ω)
            this.SurgeonScenarioMaximumNumberPatients = calculationsAbstractFactory.CreatenCalculationFactory().Create().Calculate(
                this.NullableValueFactory,
                this.WeightedAverageSurgicalDurations,
                this.TimeBlockLength);

            // SurgeonDayScenarioLengthOfStayProbabilities
            // Parameter: p(i, l, ω)
            this.SurgeonDayScenarioLengthOfStayProbabilities = this.GenerateSurgeonDayScenarioLengthOfStayProbabilitiesVanOostrum2011(
                this.NullableValueFactory,
                discreteUniformFactory,
                calculationsAbstractFactory.CreatepCalculationFactory().Create(),
                this.Scenarios,
                this.Surgeons,
                this.SurgicalSpecialties);

            // SurgeonDayScenarioCumulativeNumberPatients
            // Parameter: Φ(i, l, ω)
            this.SurgeonDayScenarioCumulativeNumberPatients = calculationsAbstractFactory.CreateΦCalculationFactory().Create().Calculate(
                this.NullableValueFactory,
                this.Surgeons,
                this.PlanningHorizon.Select(w => KeyValuePair.Create((PositiveInt)w.Key, w.Value)).ToImmutableList(),
                this.LengthOfStayDays,
                this.Scenarios,
                this.SurgeonLengthOfStayMaximums,
                this.SurgeonScenarioMaximumNumberPatients,
                this.SurgeonDayScenarioLengthOfStayProbabilities);
        }

        /// <summary>
        /// Gets a factory that can create <see cref="FhirBoolean"/>, <see cref="FhirDecimal"/>, and <see cref="PositiveInt"/> instances.
        /// </summary>
        public INullableValueFactory NullableValueFactory { get; }

        /// <inheritdoc />
        public ImmutableSortedSet<INullableValue<int>> Clusters { get; }

        /// <inheritdoc />
        public Bundle Surgeons { get; }

        /// <inheritdoc />
        public Bundle OperatingRooms { get; }

        /// <inheritdoc />
        public RedBlackTree<INullableValue<int>, FhirDateTime> PlanningHorizon { get; }

        /// <inheritdoc />
        public ImmutableList<INullableValue<int>> LengthOfStayDays { get; }

        /// <inheritdoc />
        public Organization SurgicalSpecialty1GEN { get; }

        /// <inheritdoc />
        public Organization SurgicalSpecialty2GYN { get; }

        /// <inheritdoc />
        public Organization SurgicalSpecialty3PLA { get; }

        /// <inheritdoc />
        public Organization SurgicalSpecialty4ENT { get; }

        /// <inheritdoc />
        public Organization SurgicalSpecialty5ORT { get; }

        /// <inheritdoc />
        public Organization SurgicalSpecialty6URO { get; }

        /// <inheritdoc />
        public ImmutableSortedSet<INullableValue<int>> Scenarios { get; }

        /// <inheritdoc />
        public ImmutableList<Tuple<Organization, INullableValue<int>, Duration>> WeightedAverageSurgicalDurations { get; }

        /// <inheritdoc />
        public ImmutableList<KeyValuePair<Organization, INullableValue<int>>> SurgicalSpecialtyStrategicTargetNumberTimeBlocks { get; }

        /// <inheritdoc />
        public ImmutableList<Tuple<Organization, INullableValue<int>, INullableValue<int>, FhirDecimal>> SurgicalDurations { get; }

        /// <inheritdoc />
        public ImmutableList<Tuple<Organization, INullableValue<int>, Duration>> SurgeonClusterDurationMeans { get; }

        /// <inheritdoc />
        public ImmutableList<Tuple<Organization, INullableValue<int>, Duration>> SurgeonClusterDurationStandardDeviations { get; }

        /// <inheritdoc />
        public ImmutableList<Tuple<Organization, INullableValue<int>, FhirDecimal>> SurgicalFrequencies { get; }

        /// <inheritdoc />
        public ImmutableList<KeyValuePair<Organization, INullableValue<int>>> SurgeonMaximumNumberTimeBlocks { get; }

        /// <inheritdoc />
        public ImmutableList<KeyValuePair<Organization, INullableValue<int>>> SurgeonLengthOfStayMaximums { get; }

        /// <inheritdoc />
        public ImmutableList<KeyValuePair<Organization, INullableValue<int>>> SurgeonMinimumNumberTimeBlocks { get; }

        /// <inheritdoc />
        public ImmutableList<KeyValuePair<Organization, INullableValue<int>>> SurgeonStrategicTargets { get; }

        /// <inheritdoc />
        public ImmutableList<Tuple<Organization, INullableValue<int>, PositiveInt>> SurgeonScenarioMaximumNumberPatients { get; }

        /// <inheritdoc />
        public ImmutableList<Tuple<Organization, INullableValue<int>, FhirDecimal>> SurgicalOverheads { get; }

        /// <inheritdoc />
        public ImmutableList<Tuple<Organization, PositiveInt, PositiveInt, FhirDecimal>> SurgeonDayScenarioLengthOfStayProbabilities { get; }

        /// <inheritdoc />
        public ImmutableList<Tuple<Organization, ImmutableList<Organization>>> SurgicalSpecialties { get; }

        /// <inheritdoc />
        public Duration TimeBlockLength { get; }

        /// <inheritdoc />
        public ImmutableList<Tuple<FhirDecimal, FhirDecimal, FhirDecimal, FhirDecimal>> GoalWeights { get; }

        /// <inheritdoc />
        public ImmutableList<Tuple<Organization, Location, FhirBoolean>> SurgeonOperatingRoomAvailabilities { get; }

        /// <inheritdoc />
        public ImmutableList<KeyValuePair<PositiveInt, FhirDecimal>> ScenarioProbabilities { get; }

        /// <inheritdoc />
        public ImmutableList<Tuple<Organization, PositiveInt, PositiveInt, FhirDecimal>> SurgeonDayScenarioCumulativeNumberPatients { get; }

        /// <inheritdoc />
        public ImmutableList<Tuple<Organization, FhirDateTime, FhirBoolean>> SurgeonDayAvailabilities { get; }

        // Index: e
        private ImmutableSortedSet<INullableValue<int>> GenerateClusters(
            INullableValueintComparerFactory nullableValueintComparerFactory,
            INullableValueFactory nullableValueFactory,
            int numberClusters)
        {
            return Enumerable
                .Range(1, numberClusters)
                .Select(i => nullableValueFactory.Create<int>(i))
                .ToImmutableSortedSet(
                nullableValueintComparerFactory.Create());
        }

        // Index: i
        private Bundle GenerateSurgeons(
            IBundleFactory bundleFactory,
            IEntryComponentFactory entryComponentFactory,
            IOrganizationFactory organizationFactory,
            int numberSurgeons)
        {
            return bundleFactory.Create(
                Enumerable
                .Range(1, numberSurgeons)
                .Select(i =>
                entryComponentFactory.Create(
                    organizationFactory.Create(
                        i.ToString())))
                .ToList());
        }

        // Index: j
        private Bundle GenerateOperatingRooms(
            IBundleFactory bundleFactory,
            IEntryComponentFactory entryComponentFactory,
            ILocationFactory locationFactory,
            int numberOperatingRooms)
        {
            return bundleFactory.Create(
                Enumerable
                .Range(1, numberOperatingRooms)
                .Select(i =>
                entryComponentFactory.Create(
                    locationFactory.Create(
                        i.ToString())))
                .ToList());
        }

        // Index: k
        private RedBlackTree<INullableValue<int>, FhirDateTime> GeneratePlanningHorizon(
            IFhirDateTimeFactory FhirDateTimeFactory,
            INullableValueFactory nullableValueFactory,
            DateTime endDate,
            DateTime startDate)
        {
            RedBlackTree<INullableValue<int>, FhirDateTime> redBlackTree = new(new NullableValueintComparer());

            for (DateTime dt1 = startDate; dt1 <= endDate; dt1 = dt1.AddDays(1))
            {
                redBlackTree.Add(
                    nullableValueFactory.Create<int>(
                            (dt1 - startDate).Days + 1),
                    FhirDateTimeFactory.Create(
                            dt1.Date));
            }

            return redBlackTree;
        }

        // Index: l, where h(i) is the maximum for surgeon i
        private ImmutableList<INullableValue<int>> GenerateLengthOfStayDays(
            INullableValueFactory nullableValueFactory,
            int maximumLengthOfStay)
        {
            return Enumerable
                .Range(0, maximumLengthOfStay + 1)
                .Select(i => nullableValueFactory.Create<int>(i))
                .ToImmutableList();
        }

        // Index: ω
        private ImmutableSortedSet<INullableValue<int>> GenerateScenarios(
            INullableValueintComparerFactory nullableValueintComparerFactory,
            INullableValueFactory nullableValueFactory,
            int numberScenarios)
        {
            return Enumerable
                .Range(1, numberScenarios)
                .Select(i => nullableValueFactory.Create<int>(i))
                .ToImmutableSortedSet(
                nullableValueintComparerFactory.Create());
        }

        // Parameter: B(r)
        private ImmutableList<KeyValuePair<Organization, INullableValue<int>>> GenerateSurgicalSpecialtyStrategicTargetNumberTimeBlocks(
            INullableValueFactory nullableValueFactory)
        {
            ImmutableList<KeyValuePair<Organization, INullableValue<int>>>.Builder builder = ImmutableList.CreateBuilder<KeyValuePair<Organization, INullableValue<int>>>();

            // Surgical Specialty 1
            builder.Add(
                KeyValuePair.Create(
                    this.SurgicalSpecialty1GEN,
                    nullableValueFactory.Create<int>(
                        36)));

            // Surgical Specialty 2
            builder.Add(
                KeyValuePair.Create(
                    this.SurgicalSpecialty2GYN,
                    nullableValueFactory.Create<int>(
                        30)));

            // Surgical Specialty 3
            builder.Add(
                KeyValuePair.Create(
                    this.SurgicalSpecialty3PLA,
                    nullableValueFactory.Create<int>(
                        18)));

            // Surgical Specialty 4
            builder.Add(
                KeyValuePair.Create(
                    this.SurgicalSpecialty4ENT,
                    nullableValueFactory.Create<int>(
                        12)));

            // Surgical Specialty 5
            builder.Add(
                KeyValuePair.Create(
                    this.SurgicalSpecialty5ORT,
                    nullableValueFactory.Create<int>(
                        36)));

            // Surgical Specialty 6
            builder.Add(
                KeyValuePair.Create(
                    this.SurgicalSpecialty6URO,
                    nullableValueFactory.Create<int>(
                        48)));

            return builder.ToImmutableList();
        }

        // Parameter: D(i, e, ω)
        private ImmutableList<Tuple<Organization, INullableValue<int>, INullableValue<int>, FhirDecimal>> GenerateSurgicalDurationsVanHoudenhoven2007(
            IDurationFactory durationFactory,
            INullableValueFactory nullableValueFactory,
            ILogNormalFactory logNormalFactory,
            IDCalculation DCalculation,
            ImmutableSortedSet<INullableValue<int>> clusters,
            ImmutableSortedSet<INullableValue<int>> scenarios,
            Bundle surgeons,
            ImmutableList<Tuple<Organization, ImmutableList<Organization>>> surgicalSpecialties)
        {
            ImmutableList<Tuple<Organization, INullableValue<int>, INullableValue<int>, FhirDecimal>>.Builder builder = ImmutableList.CreateBuilder<Tuple<Organization, INullableValue<int>, INullableValue<int>, FhirDecimal>>();

            VanHoudenhoven2007.InterfacesAbstractFactories.IAbstractFactory abstractFactory = VanHoudenhoven2007.AbstractFactories.AbstractFactory.Create();

            VanHoudenhoven2007.InterfacesAbstractFactories.IContextsAbstractFactory contextsAbstractFactory = abstractFactory.CreateContextsAbstractFactory();
            VanHoudenhoven2007.InterfacesAbstractFactories.IDependenciesAbstractFactory dependenciesAbstractFactory = abstractFactory.CreateDependenciesAbstractFactory();
            VanHoudenhoven2007.InterfacesAbstractFactories.IExportsAbstractFactory exportsAbstractFactory = abstractFactory.CreateExportsAbstractFactory();

            foreach (Tuple<Organization, ImmutableList<Organization>> item in surgicalSpecialties)
            {
                CodeableConcept specialty = item.Item1.Id switch
                {
                    "1" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreateGeneralSurgery(),

                    "2" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreateGynecologicalSurgery(),

                    "3" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreatePlasticSurgery(),

                    "4" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreateEarNoseThroatSurgery(),

                    "5" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreateOrthopedicSurgery(),

                    "6" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreateUrology(),

                    _ => null
                };

                foreach (Organization surgeon in item.Item2)
                {
                    foreach (PositiveInt cluster in clusters)
                    {
                        VanHoudenhoven2007.Interfaces.Contexts.SurgicalDurations.ISurgicalDurationInputContext surgicalDurationAverageInputContext = contextsAbstractFactory.CreateSurgicalDurationInputContextFactory().Create(
                            category: cluster,
                            specialty: specialty,
                            statistic: dependenciesAbstractFactory.CreateValueFactory().CreateAverage());

                        VanHoudenhoven2007.Interfaces.Contexts.SurgicalDurations.ISurgicalDurationInputContext surgicalDurationStdDevInputContext = contextsAbstractFactory.CreateSurgicalDurationInputContextFactory().Create(
                            category: cluster,
                            specialty: specialty,
                            statistic: dependenciesAbstractFactory.CreateValueFactory().CreateStdDev());

                        VanHoudenhoven2007.Interfaces.Exports.SurgicalDurations.ISurgicalDurationExport surgicalDurationAverageExport = exportsAbstractFactory.CreateSurgicalDurationExportFactory().Create();

                        VanHoudenhoven2007.Interfaces.Exports.SurgicalDurations.ISurgicalDurationExport surgicalDurationStdDevExport = exportsAbstractFactory.CreateSurgicalDurationExportFactory().Create();

                        VanHoudenhoven2007.Interfaces.Contexts.SurgicalDurations.ISurgicalDurationOutputContext surgicalDurationAverageOutputContext = surgicalDurationAverageExport.GetSurgicalDuration(
                            abstractFactory: abstractFactory,
                            surgicalDurationInputContext: surgicalDurationAverageInputContext);

                        VanHoudenhoven2007.Interfaces.Contexts.SurgicalDurations.ISurgicalDurationOutputContext surgicalDurationStdDevOutputContext = surgicalDurationStdDevExport.GetSurgicalDuration(
                            abstractFactory: abstractFactory,
                            surgicalDurationInputContext: surgicalDurationStdDevInputContext);

                        builder.AddRange(
                            DCalculation.CalculateLogNormal(
                                nullableValueFactory: nullableValueFactory,
                                logNormalFactory: logNormalFactory,
                                cluster: cluster,
                                scenarios: scenarios.Select(w => (PositiveInt)w).ToImmutableList(),
                                surgeon: surgeon,
                                µ: (double)surgicalDurationAverageOutputContext.Duration.ToHour(
                                    durationFactory).Value.Value,
                                σ: (double)surgicalDurationStdDevOutputContext.Duration.ToHour(
                                    durationFactory).Value.Value));
                    }
                }
            }

            return builder.ToImmutableList();
        }

        // Parameter: f(i, e)
        private ImmutableList<Tuple<Organization, INullableValue<int>, FhirDecimal>> GenerateSurgicalFrequenciesVanHoudenhoven2007(
            ImmutableSortedSet<INullableValue<int>> clusters,
            Bundle surgeons,
            ImmutableList<Tuple<Organization, ImmutableList<Organization>>> surgicalSpecialties)
        {
            ImmutableList<Tuple<Organization, INullableValue<int>, FhirDecimal>>.Builder builder = ImmutableList.CreateBuilder<Tuple<Organization, INullableValue<int>, FhirDecimal>>();

            VanHoudenhoven2007.InterfacesAbstractFactories.IAbstractFactory abstractFactory = VanHoudenhoven2007.AbstractFactories.AbstractFactory.Create();
            VanHoudenhoven2007.InterfacesAbstractFactories.IContextsAbstractFactory contextsAbstractFactory = abstractFactory.CreateContextsAbstractFactory();
            VanHoudenhoven2007.InterfacesAbstractFactories.IDependenciesAbstractFactory dependenciesAbstractFactory = abstractFactory.CreateDependenciesAbstractFactory();
            VanHoudenhoven2007.InterfacesAbstractFactories.IExportsAbstractFactory exportsAbstractFactory = abstractFactory.CreateExportsAbstractFactory();

            foreach (Tuple<Organization, ImmutableList<Organization>> item in surgicalSpecialties)
            {
                CodeableConcept specialty = item.Item1.Id switch
                {
                    "1" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreateGeneralSurgery(),

                    "2" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreateGynecologicalSurgery(),

                    "3" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreatePlasticSurgery(),

                    "4" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreateEarNoseThroatSurgery(),

                    "5" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreateOrthopedicSurgery(),

                    "6" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreateUrology(),

                    _ => null
                };

                foreach (Organization surgeon in item.Item2)
                {
                    foreach (PositiveInt cluster in clusters)
                    {
                        VanHoudenhoven2007.Interfaces.Contexts.SurgicalFrequencies.ISurgicalFrequencyInputContext surgicalFrequencyInputContext = contextsAbstractFactory.CreateSurgicalFrequencyInputContextFactory().Create(
                            category: cluster,
                            specialty: specialty);

                        VanHoudenhoven2007.Interfaces.Exports.SurgicalFrequencies.ISurgicalFrequencyExport surgicalFrequencyExport = exportsAbstractFactory.CreateSurgicalFrequencyExportFactory().Create();

                        VanHoudenhoven2007.Interfaces.Contexts.SurgicalFrequencies.ISurgicalFrequencyOutputContext surgicalFrequencyOutputContext = surgicalFrequencyExport.GetSurgicalFrequency(
                            abstractFactory: abstractFactory,
                            surgicalFrequencyInputContext: surgicalFrequencyInputContext);

                        builder.Add(
                            Tuple.Create(
                                surgeon,
                                (INullableValue<int>)cluster,
                                (FhirDecimal)surgicalFrequencyOutputContext.Frequency));
                    }
                }
            }

            return builder.ToImmutableList();
        }

        // Parameter: H(i)
        private ImmutableList<KeyValuePair<Organization, INullableValue<int>>> GenerateSurgeonMaximumNumberTimeBlocks(
            Bundle surgeons)
        {
            ImmutableList<KeyValuePair<Organization, INullableValue<int>>>.Builder builder = ImmutableList.CreateBuilder<KeyValuePair<Organization, INullableValue<int>>>();

            // Surgeon 1
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        9)));

            // Surgeon 2
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        14)));

            // Surgeon 3
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        12)));

            // Surgeon 4
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        16)));

            // Surgeon 5
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        10)));

            // Surgeon 6
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        8)));

            // Surgeon 7
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        17)));

            // Surgeon 8
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        18)));

            // Surgeon 9
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        10)));

            // Surgeon 10
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        18)));

            // Surgeon 11
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        7)));

            // Surgeon 12
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        9)));

            // Surgeon 13
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        12)));

            // Surgeon 14
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        16)));

            // Surgeon 15
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        16)));

            // Surgeon 16
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        18)));

            // Surgeon 17
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        13)));

            // Surgeon 18
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        12)));

            // Surgeon 19
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        20)));

            // Surgeon 20
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        8)));

            // Surgeon 21
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        13)));

            // Surgeon 22
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        20)));

            // Surgeon 23
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        11)));

            // Surgeon 24
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        7)));

            // Surgeon 25
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        10)));

            // Surgeon 26
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        10)));

            // Surgeon 27
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        17)));

            // Surgeon 28
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        10)));

            // Surgeon 29
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        11)));

            // Surgeon 30
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        6)));

            return builder.ToImmutableList();
        }

        // Parameter: h(i)
        private ImmutableList<KeyValuePair<Organization, INullableValue<int>>> GenerateSurgeonLengthOfStayMaximumsSameForAllSurgeons(
            INullableValueFactory nullableValueFactory,
            int maximumLengthOfStay,
            Bundle surgeons)
        {
            ImmutableList<KeyValuePair<Organization, INullableValue<int>>>.Builder builder = ImmutableList.CreateBuilder<KeyValuePair<Organization, INullableValue<int>>>();

            foreach (Organization surgeon in surgeons.Entry.Where(i => i.Resource is Organization).Select(i => (Organization)i.Resource))
            {
                builder.Add(
                    KeyValuePair.Create(
                        surgeon,
                        nullableValueFactory.Create<int>(
                            maximumLengthOfStay)));
            }

            return builder.ToImmutableList();
        }

        // Parameter: L(i)
        private ImmutableList<KeyValuePair<Organization, INullableValue<int>>> GenerateSurgeonMinimumNumberTimeBlocks(
            Bundle surgeons)
        {
            ImmutableList<KeyValuePair<Organization, INullableValue<int>>>.Builder builder = ImmutableList.CreateBuilder<KeyValuePair<Organization, INullableValue<int>>>();

            // Surgeon 1
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        1)));

            // Surgeon 2
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        5)));

            // Surgeon 3
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        1)));

            // Surgeon 4
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        5)));

            // Surgeon 5
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        2)));

            // Surgeon 6
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        5)));

            // Surgeon 7
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        3)));

            // Surgeon 8
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        2)));

            // Surgeon 9
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        2)));

            // Surgeon 10
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        4)));

            // Surgeon 11
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        3)));

            // Surgeon 12
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        5)));

            // Surgeon 13
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        1)));

            // Surgeon 14
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        1)));

            // Surgeon 15
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        2)));

            // Surgeon 16
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        4)));

            // Surgeon 17
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        3)));

            // Surgeon 18
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        2)));

            // Surgeon 19
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        2)));

            // Surgeon 20
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        2)));

            // Surgeon 21
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        3)));

            // Surgeon 22
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        1)));

            // Surgeon 23
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        5)));

            // Surgeon 24
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        3)));

            // Surgeon 25
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        2)));

            // Surgeon 26
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        5)));

            // Surgeon 27
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        1)));

            // Surgeon 28
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        4)));

            // Surgeon 29
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        3)));

            // Surgeon 30
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        4)));

            return builder.ToImmutableList();
        }

        // Parameter: N(i)
        private ImmutableList<KeyValuePair<Organization, INullableValue<int>>> GenerateSurgeonStrategicTargets(
            Bundle surgeons)
        {
            ImmutableList<KeyValuePair<Organization, INullableValue<int>>>.Builder builder = ImmutableList.CreateBuilder<KeyValuePair<Organization, INullableValue<int>>>();

            // Surgeon 1
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        20)));

            // Surgeon 2
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        21)));

            // Surgeon 3
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        33)));

            // Surgeon 4
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        23)));

            // Surgeon 5
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        24)));

            // Surgeon 6
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        30)));

            // Surgeon 7
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        28)));

            // Surgeon 8
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        26)));

            // Surgeon 9
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        26)));

            // Surgeon 10
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        34)));

            // Surgeon 11
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        25)));

            // Surgeon 12
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        35)));

            // Surgeon 13
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        25)));

            // Surgeon 14
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        21)));

            // Surgeon 15
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        30)));

            // Surgeon 16
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        22)));

            // Surgeon 17
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        27)));

            // Surgeon 18
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        21)));

            // Surgeon 19
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        26)));

            // Surgeon 20
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        31)));

            // Surgeon 21
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        32)));

            // Surgeon 22
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        31)));

            // Surgeon 23
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        32)));

            // Surgeon 24
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        22)));

            // Surgeon 25
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        29)));

            // Surgeon 26
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        29)));

            // Surgeon 27
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        24)));

            // Surgeon 28
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        30)));

            // Surgeon 29
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        30)));

            // Surgeon 30
            builder.Add(
                KeyValuePair.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.NullableValueFactory.Create<int>(
                        35)));

            return builder.ToImmutableList();
        }

        // Parameter: O(i, e)
        private ImmutableList<Tuple<Organization, INullableValue<int>, FhirDecimal>> GenerateSurgicalOverheads(
            INullableValueFactory nullableValueFactory,
            IContinuousUniformFactory continuousUniformFactory,
            ImmutableSortedSet<INullableValue<int>> clusters,
            Bundle surgeons,
            double lower,
            double upper)
        {
            IContinuousDistribution continuousUniform = continuousUniformFactory.Create(
                lower: lower,
                upper: upper);

            ImmutableList<Tuple<Organization, INullableValue<int>, FhirDecimal>>.Builder builder = ImmutableList.CreateBuilder<Tuple<Organization, INullableValue<int>, FhirDecimal>>();

            foreach (Organization surgeon in surgeons.Entry.Where(i => i.Resource is Organization).Select(i => (Organization)i.Resource))
            {
                foreach (PositiveInt cluster in clusters)
                {
                    builder.Add(
                        Tuple.Create(
                            surgeon,
                            (INullableValue<int>)cluster,
                            (FhirDecimal)nullableValueFactory.Create<decimal>(
                                (decimal)continuousUniform.Sample())));
                }
            }

            return builder.ToImmutableList();
        }

        // Parameter: p(i, l, ω)
        private ImmutableList<Tuple<Organization, PositiveInt, PositiveInt, FhirDecimal>> GenerateSurgeonDayScenarioLengthOfStayProbabilitiesVanOostrum2011(
            INullableValueFactory nullableValueFactory,
            IDiscreteUniformFactory discreteUniformFactory,
            IpCalculation pCalculation,
            ImmutableSortedSet<INullableValue<int>> scenarios,
            Bundle surgeons,
            ImmutableList<Tuple<Organization, ImmutableList<Organization>>> surgicalSpecialties)
        {
            ImmutableList<Tuple<Organization, PositiveInt, PositiveInt, FhirDecimal>>.Builder builder = ImmutableList.CreateBuilder<Tuple<Organization, PositiveInt, PositiveInt, FhirDecimal>>();

            VanOostrum2011.InterfacesAbstractFactories.IAbstractFactory abstractFactory = VanOostrum2011.AbstractFactories.AbstractFactory.Create();
            VanOostrum2011.InterfacesAbstractFactories.IContextsAbstractFactory contextsAbstractFactory = abstractFactory.CreateContextsAbstractFactory();
            VanOostrum2011.InterfacesAbstractFactories.IDependenciesAbstractFactory dependenciesAbstractFactory = abstractFactory.CreateDependenciesAbstractFactory();
            VanOostrum2011.InterfacesAbstractFactories.IExportsAbstractFactory exportsAbstractFactory = abstractFactory.CreateExportsAbstractFactory();

            foreach (Tuple<Organization, ImmutableList<Organization>> item in surgicalSpecialties)
            {
                CodeableConcept specialty = item.Item1.Id switch
                {
                    "1" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreateGeneralSurgery(),

                    "2" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreateGynecology(),

                    "3" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreatePlasticSurgery(),

                    "4" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreateEarNoseThroatSurgery(),

                    "5" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreateOrthopedicSurgery(),

                    "6" => dependenciesAbstractFactory.CreateCodeableConceptFactory().CreateUrology(),

                    _ => null
                };

                VanOostrum2011.Interfaces.Contexts.PatientLengthOfStays.IPatientLengthOfStayInputContext patientLengthOfStayInputContext = contextsAbstractFactory.CreatePatientLengthOfStayInputContextFactory().Create(
                    specialty: specialty,
                    statistic: dependenciesAbstractFactory.CreateValueFactory().CreateAverage());

                VanOostrum2011.Interfaces.Exports.PatientLengthOfStays.IPatientLengthOfStayExport patientLengthOfStayExport = exportsAbstractFactory.CreatePatientLengthOfStayExportFactory().Create();

                VanOostrum2011.Interfaces.Contexts.PatientLengthOfStays.IPatientLengthOfStayOutputContext patientLengthOfStayOutputContext = patientLengthOfStayExport.GetPatientLengthOfStay(
                    abstractFactory: abstractFactory,
                    patientLengthOfStayInputContext: patientLengthOfStayInputContext);

                foreach (Organization surgeon in item.Item2)
                {
                    builder.AddRange(
                        pCalculation.GenerateScenarios(
                            nullableValueFactory,
                            discreteUniformFactory,
                            this.LengthOfStayDays.Select(w => (PositiveInt)w).ToImmutableList(),
                            scenarios.Select(w => (PositiveInt)w).ToImmutableList(),
                            surgeon,
                            this.SurgeonLengthOfStayMaximums,
                            (double)patientLengthOfStayOutputContext.Duration.Value.Value));
                }
            }

            return builder.ToImmutableList();
        }

        // S(r)
        private ImmutableList<Tuple<Organization, ImmutableList<Organization>>> GenerateSurgicalSpecialties()
        {
            ImmutableList<Tuple<Organization, ImmutableList<Organization>>>.Builder builder = ImmutableList.CreateBuilder<Tuple<Organization, ImmutableList<Organization>>>();

            builder.AddRange(
                new List<Tuple<Organization, ImmutableList<Organization>>>()
                {
                        Tuple.Create(
                            this.SurgicalSpecialty1GEN,
                            new List<Organization>()
                            {
                                this.GetSurgeonWithId(
                                    "1",
                                    this.Surgeons),
                                this.GetSurgeonWithId(
                                    "2",
                                    this.Surgeons),
                                this.GetSurgeonWithId(
                                    "3",
                                    this.Surgeons),
                                this.GetSurgeonWithId(
                                    "4",
                                    this.Surgeons),
                                this.GetSurgeonWithId(
                                    "5",
                                    this.Surgeons),
                                this.GetSurgeonWithId(
                                    "6",
                                    this.Surgeons)
                        }
                        .ToImmutableList()),

                    Tuple.Create(
                        this.SurgicalSpecialty2GYN,
                        new List<Organization>()
                        {
                            this.GetSurgeonWithId(
                                    "7",
                                    this.Surgeons),
                            this.GetSurgeonWithId(
                                    "8",
                                    this.Surgeons),
                            this.GetSurgeonWithId(
                                    "9",
                                    this.Surgeons),
                            this.GetSurgeonWithId(
                                    "10",
                                    this.Surgeons),
                            this.GetSurgeonWithId(
                                    "11",
                                    this.Surgeons)
                        }
                        .ToImmutableList()),

                    Tuple.Create(
                        this.SurgicalSpecialty3PLA,
                        new List<Organization>()
                        {
                           this.GetSurgeonWithId(
                                    "12",
                                    this.Surgeons),
                           this.GetSurgeonWithId(
                                    "13",
                                    this.Surgeons),
                           this.GetSurgeonWithId(
                                    "14",
                                    this.Surgeons)
                        }
                        .ToImmutableList()),

                    Tuple.Create(
                        this.SurgicalSpecialty4ENT,
                        new List<Organization>()
                        {
                            this.GetSurgeonWithId(
                                    "15",
                                    this.Surgeons),
                            this.GetSurgeonWithId(
                                    "16",
                                    this.Surgeons)
                        }
                        .ToImmutableList()),

                    Tuple.Create(
                        this.SurgicalSpecialty5ORT,
                        new List<Organization>()
                        {
                            this.GetSurgeonWithId(
                                    "17",
                                    this.Surgeons),
                            this.GetSurgeonWithId(
                                    "18",
                                    this.Surgeons),
                            this.GetSurgeonWithId(
                                    "19",
                                    this.Surgeons),
                            this.GetSurgeonWithId(
                                    "20",
                                    this.Surgeons),
                            this.GetSurgeonWithId(
                                    "21",
                                    this.Surgeons),
                            this.GetSurgeonWithId(
                                    "22",
                                    this.Surgeons)
                        }
                        .ToImmutableList()),

                    Tuple.Create(
                        this.SurgicalSpecialty6URO,
                        new List<Organization>()
                        {
                           this.GetSurgeonWithId(
                                    "23",
                                    this.Surgeons),
                           this.GetSurgeonWithId(
                                    "24",
                                    this.Surgeons),
                           this.GetSurgeonWithId(
                                    "25",
                                    this.Surgeons),
                           this.GetSurgeonWithId(
                                    "26",
                                    this.Surgeons),
                           this.GetSurgeonWithId(
                                    "27",
                                    this.Surgeons),
                           this.GetSurgeonWithId(
                                    "28",
                                    this.Surgeons),
                           this.GetSurgeonWithId(
                                    "29",
                                    this.Surgeons),
                           this.GetSurgeonWithId(
                                    "30",
                                    this.Surgeons)
                        }
                        .ToImmutableList())
                }
                .ToImmutableList());
            
            return builder.ToImmutableList();
        }

        // Parameters: (w1, w2, w3, w4)
        private ImmutableList<Tuple<FhirDecimal, FhirDecimal, FhirDecimal, FhirDecimal>> GenerateGoalWeights(
            INullableValueFactory nullableValueFactory)
        {
            ImmutableList<Tuple<FhirDecimal, FhirDecimal, FhirDecimal, FhirDecimal>>.Builder builder = ImmutableList.CreateBuilder<Tuple<FhirDecimal, FhirDecimal, FhirDecimal, FhirDecimal>>();

            builder.Add(
                Tuple.Create(
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1)));

            builder.Add(
                Tuple.Create(
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        20),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1)));

            builder.Add(
                Tuple.Create(
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        20),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1)));

            builder.Add(
                Tuple.Create(
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        20),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1)));

            builder.Add(
                Tuple.Create(
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        20)));

            builder.Add(
                Tuple.Create(
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        40),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        5),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        15),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        10)));

            builder.Add(
                Tuple.Create(
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        5),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        50),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        20),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        100)));

            builder.Add(
                Tuple.Create(
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        100),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        25),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        1),
                    (FhirDecimal)nullableValueFactory.Create<decimal>(
                        15)));

            return builder.ToImmutableList();
        }

        // Parameter: Π(i, j)
        private ImmutableList<Tuple<Organization, Location, FhirBoolean>> GenerateSurgeonOperatingRoomAvailabilities()
        {
            ImmutableList<Tuple<Organization, Location, FhirBoolean>>.Builder builder = ImmutableList.CreateBuilder<Tuple<Organization, Location, FhirBoolean>>();

            // Surgeon 1, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 6, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 7, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 7, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 8, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 8, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 9, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 9, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 10, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 10, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 11, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 11, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 12, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 12, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 13, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 13, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 14, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 14, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 15, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 15, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 16, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 16, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 18, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 18, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 19, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 20, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 21, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 22, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 23, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 24, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 25, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 26, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 27, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 28, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Operating Room 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "1",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Operating Room 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "2",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Operating Room 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "3",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Operating Room 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "4",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 30, Operating Room 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "5",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Operating Room 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "6",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Operating Room 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "7",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 30, Operating Room 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "8",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 30, Operating Room 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        this.Surgeons),
                    this.GetOperatingRoomWithId(
                        "9",
                        this.OperatingRooms),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            return builder.ToImmutableList();
        }

        // Parameter: Ρ(ω)
        private ImmutableList<KeyValuePair<PositiveInt, FhirDecimal>> GenerateScenarioProbabilities(
            INullableValueFactory nullableValueFactory,
            ImmutableSortedSet<INullableValue<int>> scenarios,
            int numberScenarios)
        {
            ImmutableList<KeyValuePair<PositiveInt, FhirDecimal>>.Builder scenarioProbabilitiesBuilder = ImmutableList.CreateBuilder<KeyValuePair<PositiveInt, FhirDecimal>>();

            INullableValue<decimal> probability = nullableValueFactory.Create<decimal>((decimal)1/(decimal)numberScenarios);

            scenarioProbabilitiesBuilder.AddRange(
                new List<KeyValuePair<PositiveInt, FhirDecimal>>()
                {
                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 1).SingleOrDefault(),
                        (FhirDecimal)probability),

                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 2).SingleOrDefault(),
                        (FhirDecimal)probability),

                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 3).SingleOrDefault(),
                        (FhirDecimal)probability),

                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 4).SingleOrDefault(),
                        (FhirDecimal)probability),

                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 5).SingleOrDefault(),
                        (FhirDecimal)probability),

                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 6).SingleOrDefault(),
                        (FhirDecimal)probability),

                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 7).SingleOrDefault(),
                        (FhirDecimal)probability),

                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 8).SingleOrDefault(),
                        (FhirDecimal)probability),

                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 9).SingleOrDefault(),
                        (FhirDecimal)probability),

                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 10).SingleOrDefault(),
                        (FhirDecimal)probability),

                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 11).SingleOrDefault(),
                        (FhirDecimal)probability),

                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 12).SingleOrDefault(),
                        (FhirDecimal)probability),

                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 13).SingleOrDefault(),
                        (FhirDecimal)probability),

                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 14).SingleOrDefault(),
                        (FhirDecimal)probability),

                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 15).SingleOrDefault(),
                        (FhirDecimal)probability),

                    KeyValuePair.Create(
                        (PositiveInt)scenarios.Where(i => i.Value.Value == 16).SingleOrDefault(),
                        (FhirDecimal)probability),
                });

            return scenarioProbabilitiesBuilder.ToImmutableList();
        }

        // Parameter: Ω(i, k)
        private ImmutableList<Tuple<Organization, FhirDateTime, FhirBoolean>> GenerateSurgeonDayAvailabilities(
            ImmutableList<KeyValuePair<PositiveInt, FhirDateTime>> planningHorizon,
            Bundle surgeons)
        {
            ImmutableList<Tuple<Organization, FhirDateTime, FhirBoolean>>.Builder builder = ImmutableList.CreateBuilder<Tuple<Organization, FhirDateTime, FhirBoolean>>();

            // Surgeon 1, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 1, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 1, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 1, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 1, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 1, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 1, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 1, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 1, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 1, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 1, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 1, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 1, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "1",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 2, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 2, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 2, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 2, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 2, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 2, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 2, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 2, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 2, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 2, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 2, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 2, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 2, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "2",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 3, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 3, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 3, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 3, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 3, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 3, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 3, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 3, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 3, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 3, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 3, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 3, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 3, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "3",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 4, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 4, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 4, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 4, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 4, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 4, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 4, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 4, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 4, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 4, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 4, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 4, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 4, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "4",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 5, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 5, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 5, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 5, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 5, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 5, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 5, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 5, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 5, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 5, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 5, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 5, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 5, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "5",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 6, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 6, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 6, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 6, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 6, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 6, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 6, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 6, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 6, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 6, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 6, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 6, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 6, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "6",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 7, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 7, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 7, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 7, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 7, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 7, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 7, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 7, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 7, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 7, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 7, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 7, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 7, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "7",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 8, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 8, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 8, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 8, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 8, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 8, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 8, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 8, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 8, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 8, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 8, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 8, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 8, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "8",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 9, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 9, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 9, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 9, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 9, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 9, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 9, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 9, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 9, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 9, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 9, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 9, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 9, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "9",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 10, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 10, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 10, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 10, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 10, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 10, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 10, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 10, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 10, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 10, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 10, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 10, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 10, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "10",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 11, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 11, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 11, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 11, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 11, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 11, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 11, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 11, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 11, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 11, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 11, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 11, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 11, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "11",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 12, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 12, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 12, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 12, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 12, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 12, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 12, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 12, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 12, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 12, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 12, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 12, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 12, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "12",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 13, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 13, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 13, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 13, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 13, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 13, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 13, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 13, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 13, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 13, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 13, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 13, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 13, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "13",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 14, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 14, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 14, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 14, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 14, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 14, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 14, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 14, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 14, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 14, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 14, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 14, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 14, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "14",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 15, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 15, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 15, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 15, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 15, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 15, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 15, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 15, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 15, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 15, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 15, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 15, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 15, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "15",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 16, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 16, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 16, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 16, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 16, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 16, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 16, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 16, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 16, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 16, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 16, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 16, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 16, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "16",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 17, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 17, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "17",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 18, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 18, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 18, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 18, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 18, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 18, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 18, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 18, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 18, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 18, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 18, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 18, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 18, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "18",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 19, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 19, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 19, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 19, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 19, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 19, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 19, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 19, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 19, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 19, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 19, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 19, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 19, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "19",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 20, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 20, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 20, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 20, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 20, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 20, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 20, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 20, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 20, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 20, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 20, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 20, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 20, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "20",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 21, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 21, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 21, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 21, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 21, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 21, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 21, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 21, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 21, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 21, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 21, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 21, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 21, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "21",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 22, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 22, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 22, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 22, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 22, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 22, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 22, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 22, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 22, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 22, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 22, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 22, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 22, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "22",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 23, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 23, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 23, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 23, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 23, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 23, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 23, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 23, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 23, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 23, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 23, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 23, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 23, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "23",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 24, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 24, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 24, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 24, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 24, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 24, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 24, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 24, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 24, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 24, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 24, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 24, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 24, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "24",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 25, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 25, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 25, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 25, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 25, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 25, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 25, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 25, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 25, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 25, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 25, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 25, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 25, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "25",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 26, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 26, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 26, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 26, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 26, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 26, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 26, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 26, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 26, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 26, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 26, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 26, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 26, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "26",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 27, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 27, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 27, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 27, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 27, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 27, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 27, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 27, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 27, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 27, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 27, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 27, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 27, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "27",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 28, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 28, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 28, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 28, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 28, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 28, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 28, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 28, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 28, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 28, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 28, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 28, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 28, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "28",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 29, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 29, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "29",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 30, Day 1
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        1,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 30, Day 2
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        2,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 3
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        3,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 4
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        4,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 5
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        5,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 6
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        6,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 30, Day 7
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        7,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 30, Day 8
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        8,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 30, Day 9
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        9,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 10
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        10,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 11
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        11,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 12
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        12,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 13
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        13,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 30, Day 14
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        14,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 30, Day 15
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        15,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 30, Day 16
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        16,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 17
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        17,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 18
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        18,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 19
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        19,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 20
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        20,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 30, Day 21
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        21,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 30, Day 22
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        22,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 30, Day 23
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        23,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 24
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        24,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 25
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        25,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 26
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        26,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        false)));

            // Surgeon 30, Day 27
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        27,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            // Surgeon 30, Day 28
            builder.Add(
                Tuple.Create(
                    this.GetSurgeonWithId(
                        "30",
                        surgeons),
                    this.GetDayWithIndex(
                        28,
                        planningHorizon),
                    (FhirBoolean)this.NullableValueFactory.Create<bool>(
                        true)));

            return builder.ToImmutableList();
        }

        private Organization GetSurgeonWithId(
            string id,
            Bundle surgeons)
        {
            Organization surgeon = null;

            foreach (Organization item in surgeons.Entry.Where(i => i.Resource is Organization).Select(i => (Organization)i.Resource))
            {
                if (item.Id.Equals(id))
                {
                    surgeon = item;
                }
            }

            return surgeon;
        }

        private Location GetOperatingRoomWithId(
            string id,
            Bundle operatingRooms)
        {
            Location operatingRoom = null;

            foreach (Location item in operatingRooms.Entry.Where(i => i.Resource is Location).Select(i => (Location)i.Resource))
            {
                if (item.Id.Equals(id))
                {
                    operatingRoom = item;
                }
            }

            return operatingRoom;
        }

        private FhirDateTime GetDayWithIndex(
            int index,
            ImmutableList<KeyValuePair<PositiveInt, FhirDateTime>> planningHorizon)
        {
            FhirDateTime day = null;

            foreach (KeyValuePair<PositiveInt, FhirDateTime> item in planningHorizon)
            {
                if (item.Key.Value.Value.Equals(index))
                {
                    day = item.Value;
                }
            }

            return day;
        }
    }
}