﻿namespace Britt2022.D.InterfacesFactories.Comparers
{
    using Britt2022.D.Interfaces.Comparers;

    public interface IFhirDateTimeComparerFactory
    {
        IFhirDateTimeComparer Create();
    }
}