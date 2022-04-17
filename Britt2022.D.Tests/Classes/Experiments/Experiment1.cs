namespace Britt2022.D.Tests.Classes.Experiments
{
    using System.Collections.Immutable;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Hl7.Fhir.Model;

    using Britt2022.D.InterfacesAbstractFactories;
    using Britt2022.D.Interfaces.Experiments;

    [TestClass]
    public sealed class Experiment1
    {
        [DataTestMethod]
        [DataRow(8)]
        public void NumberClusters(
            int numberClusters)
        {
            // Arrange
            IAbstractFactory abstractFactory = Britt2022.D.AbstractFactories.AbstractFactory.Create();

            ICalculationsAbstractFactory calculationsAbstractFactory = abstractFactory.CreateCalculationsAbstractFactory();

            IDependenciesAbstractFactory dependenciesAbstractFactory = abstractFactory.CreateDependenciesAbstractFactory();

            IExperimentsAbstractFactory experimentsAbstractFactory = abstractFactory.CreateExperimentsAbstractFactory();

            IExperiment1 experiment1 = experimentsAbstractFactory.CreateExperiment1Factory().Create(
                calculationsAbstractFactory,
                dependenciesAbstractFactory);

            // Act
            ImmutableList<PositiveInt> clusters = experiment1.Clusters;

            // Assert
            Assert.AreEqual(
                expected: numberClusters,
                actual: clusters.Count);
        }
    }
}