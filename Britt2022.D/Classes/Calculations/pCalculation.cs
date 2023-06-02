namespace Britt2022.D.Classes.Calculations
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using log4net;

    using Hl7.Fhir.Model;

    using MathNet.Numerics.Distributions;

    using Britt2022.D.Interfaces.Calculations;
    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;
    using Britt2022.D.InterfacesFactories.Dependencies.MathNet.Numerics.Distributions;

    internal sealed class pCalculation : IpCalculation
    {
        private ILog Log => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public pCalculation()
        {
        }

        public ImmutableList<Tuple<Organization, INullableValue<int>, PositiveInt, FhirDecimal>> GenerateScenarios(
            INullableValueFactory nullableValueFactory,
            IDiscreteUniformFactory discreteUniformFactory,
            ImmutableList<PositiveInt> lengthOfStayDays,
            ImmutableList<PositiveInt> scenarios,
            Organization surgeon,
            ImmutableList<KeyValuePair<Organization, INullableValue<int>>> surgeonLengthOfStayMaximums,
            double targetMean)
        {
            ImmutableList<Tuple<Organization, INullableValue<int>, PositiveInt, FhirDecimal>>.Builder builder = ImmutableList.CreateBuilder<Tuple<Organization, INullableValue<int>, PositiveInt, FhirDecimal>>();

            int lengthOfStayMaximum = surgeonLengthOfStayMaximums
                    .Where(i => i.Key == surgeon)
                    .Select(i => i.Value.Value.Value)
                    .SingleOrDefault();

            using (DiscreteHalfNormal.Interfaces.Calculations.IqθμCalculation qθμCalculation = 
                DiscreteHalfNormal.AbstractFactories.AbstractFactory.Create().CreateCalculationsAbstractFactory().CreateqθμCalculationFactory().Create(lengthOfStayMaximum))
            {
                ReadOnlySpan<DiscreteHalfNormal.Structs.CalculationElements.qθμCalculationElement> qθMeans = qθμCalculation.Calculate(
                    0.01,
                    0.01,
                    1,
                    targetMean,
                    lengthOfStayMaximum,
                    0.01,
                    0.01,
                    25,
                    0.01);

                ReadOnlySpan<int> indices = this.GetRandomNumbers(
                    discreteUniformFactory: discreteUniformFactory,
                    maxValue: qθMeans.Length - 1,
                    minValue: 0,
                    numberOfElements: scenarios.Count());

                for (int w = 0; w < scenarios.Count; w = w + 1)
                {
                    int index = indices[w];

                    DiscreteHalfNormal.Structs.CalculationElements.qθμCalculationElement qθMean = qθMeans[index];

                    ReadOnlySpan<DiscreteHalfNormal.Structs.CalculationElements.xpCalculationElement> xp = qθμCalculation.xpCalculation.Calculate(
                        qθMean.q,
                        lengthOfStayMaximum,
                        qθMean.θ);

                    foreach (DiscreteHalfNormal.Structs.CalculationElements.xpCalculationElement item in xp)
                    {
                        builder.Add(
                            Tuple.Create(
                                surgeon,
                                (INullableValue<int>)lengthOfStayDays.Where(i => i.Value.Value == item.x).SingleOrDefault(),
                                scenarios[w],
                                (FhirDecimal)nullableValueFactory.Create<decimal>(
                                    (decimal)item.p)));
                    }

                    foreach (PositiveInt item in lengthOfStayDays.Where(w => w.Value.Value > lengthOfStayMaximum))
                    {
                        builder.Add(
                            Tuple.Create(
                                surgeon,
                                (INullableValue<int>)item,
                                scenarios[w],
                                (FhirDecimal)nullableValueFactory.Create<decimal>(
                                    0)));
                    }
                }
            }

            return builder.ToImmutableList();
        }

        // Based on: https://stackoverflow.com/a/46703316
        private ReadOnlySpan<int> GetRandomNumbers(
            IDiscreteUniformFactory discreteUniformFactory,
            int maxValue,
            int minValue,
            int numberOfElements)
        {
            IDiscreteDistribution discreteDistribution = discreteUniformFactory.Create(
                lower: minValue,
                upper: maxValue);

            HashSet<int> numbers = new HashSet<int>();

            while (numbers.Count < numberOfElements)
            {
                numbers.Add(
                    discreteDistribution.Sample());
            }

            return numbers.ToArray();
        }
    }
}