namespace Britt2022.D.Classes.Comparers
{
    using System;

    using log4net;

    using Hl7.Fhir.Model;

    using Britt2022.D.Interfaces.Comparers;

    internal sealed class LocationComparer : ILocationComparer
    {
        private ILog Log => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LocationComparer()
        {
        }

        public int Compare(
            Location x,
            Location y)
        {
            return String.CompareOrdinal(
                x.Id,
                y.Id);
        }
    }
}