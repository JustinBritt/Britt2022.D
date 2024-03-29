﻿namespace Britt2022.D.Classes.Experiments
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using log4net;

    using Hl7.Fhir.Model;

    using Britt2022.D.Extensions.Dependencies.Hl7.Fhir.R4.Model;
    using Britt2022.D.Interfaces.Calculations;
    using Britt2022.D.Interfaces.Experiments;
    using Britt2022.D.InterfacesAbstractFactories;
    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;
    using Britt2022.D.InterfacesFactories.Dependencies.MathNet.Numerics.Distributions;

    internal sealed class Experiment3 : IExperiment3
    {
        private ILog Log => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Experiment3(
            ICalculationsAbstractFactory calculationsAbstractFactory,
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
        }

        /// <summary>
        /// Gets a factory can create FhirBoolean, FhirDecimal, and PositiveInt instances.
        /// </summary>
        public INullableValueFactory NullableValueFactory { get; }
    }
}