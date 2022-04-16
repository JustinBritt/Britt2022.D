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
        ImmutableList<PositiveInt> Clusters { get; }
    }
}