namespace Britt2022.D.Classes.Calculations
{
    using System;
    using System.Collections.Immutable;

    using log4net;

    using Hl7.Fhir.Model;

    using MathNet.Numerics.Distributions;

    using Britt2022.D.Interfaces.Calculations;
    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;
    using Britt2022.D.InterfacesFactories.Dependencies.MathNet.Numerics.Distributions;

    internal sealed class DCalculation : IDCalculation
    {
        private ILog Log => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DCalculation()
        {
        }

        public ImmutableList<Tuple<Organization, INullableValue<int>, INullableValue<int>, INullableValue<decimal>>> CalculateLogNormal(
            INullableValueFactory nullableValueFactory,
            ILogNormalFactory logNormalFactory,
            PositiveInt cluster,
            ImmutableList<PositiveInt> scenarios,
            Organization surgeon,
            double µ,
            double σ)
        {
            ImmutableList<Tuple<Organization, INullableValue<int>, INullableValue<int>, INullableValue<decimal>>>.Builder builder = ImmutableList.CreateBuilder<Tuple<Organization, INullableValue<int>, INullableValue<int>, INullableValue<decimal>>>();

            // https://stackoverflow.com/questions/48014712/get-lognormal-random-number-given-log10-mean-and-log10-standard-deviation/48016650#48016650
            if (σ != 0)
            {
                double normalσ = Math.Sqrt(
                Math.Log(
                    1.0d
                    +
                    Math.Pow((σ / µ), 2)));

                double normalµ = Math.Log(µ) - 0.5d * Math.Pow(normalσ, 2);

                IContinuousDistribution logNormal = logNormalFactory.Create(
                        normalµ,
                        normalσ);

                foreach (PositiveInt scenario in scenarios)
                {
                    builder.Add(
                        Tuple.Create(
                            surgeon,
                            (INullableValue<int>)cluster,
                            (INullableValue<int>)scenario,
                            nullableValueFactory.Create<decimal>(
                                (decimal)logNormal.Sample())));
                }
            }
            else
            {
                foreach (PositiveInt scenario in scenarios)
                {
                    builder.Add(
                        Tuple.Create(
                            surgeon,
                            (INullableValue<int>)cluster,
                            (INullableValue<int>)scenario,
                            nullableValueFactory.Create<decimal>(
                                0m)));
                }
            }

            return builder.ToImmutableList();
        }
    }
}