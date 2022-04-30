namespace Britt2022.D.Tests.Classes.Experiments
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Hl7.Fhir.Model;

    using Britt2022.D.InterfacesAbstractFactories;
    using Britt2022.D.Interfaces.Experiments;
    
    [TestClass]
    public sealed class Experiment1
    {
        private IExperiment1 CreateExperiment1()
        {
            IAbstractFactory abstractFactory = Britt2022.D.AbstractFactories.AbstractFactory.Create();

            ICalculationsAbstractFactory calculationsAbstractFactory = abstractFactory.CreateCalculationsAbstractFactory();

            IDependenciesAbstractFactory dependenciesAbstractFactory = abstractFactory.CreateDependenciesAbstractFactory();

            IExperimentsAbstractFactory experimentsAbstractFactory = abstractFactory.CreateExperimentsAbstractFactory();

            return experimentsAbstractFactory.CreateExperiment1Factory().Create(
                calculationsAbstractFactory,
                dependenciesAbstractFactory);
        }

        [DataTestMethod]
        [DataRow(8)]
        public void NumberClusters(
            int numberClusters)
        {
            // Arrange
            IExperiment1 experiment1 = this.CreateExperiment1();

            // Act
            ImmutableList<PositiveInt> clusters = experiment1.Clusters;

            // Assert
            Assert.AreEqual(
                expected: numberClusters,
                actual: clusters.Count);
        }

        [DataTestMethod]
        [DataRow(9)]
        public void NumberOperatingRooms(
            int numberOperatingRooms)
        {
            // Arrange
            IExperiment1 experiment1 = this.CreateExperiment1();

            // Act
            Bundle operatingRooms = experiment1.OperatingRooms;

            // Assert
            Assert.AreEqual(
                expected: numberOperatingRooms,
                actual: operatingRooms.Entry.Count);
        }

        [DataTestMethod]
        [DataRow(30)]
        public void NumberSurgeons(
            int numberSurgeons)
        {
            // Arrange
            IExperiment1 experiment1 = this.CreateExperiment1();

            // Act
            Bundle surgeons = experiment1.Surgeons;

            // Assert
            Assert.AreEqual(
                expected: numberSurgeons,
                actual: surgeons.Entry.Count);
        }

        [DataTestMethod]
        [DataRow(28)]
        public void PlanningHorizonLength(
            int planningHorizonLength)
        {
            // Arrange
            IExperiment1 experiment1 = this.CreateExperiment1();

            // Act
            ImmutableList<KeyValuePair<PositiveInt, FhirDateTime>> planningHorizon = experiment1.PlanningHorizon;

            // Assert
            Assert.AreEqual(
                expected: planningHorizonLength,
                actual: planningHorizon.Count);
        }
    }
}