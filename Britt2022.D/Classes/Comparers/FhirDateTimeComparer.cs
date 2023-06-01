namespace Britt2022.D.Classes.Comparers
{
    using System;

    using log4net;

    using Hl7.Fhir.Model;

    using Britt2022.D.Interfaces.Comparers;

    internal sealed class FhirDateTimeComparer : IFhirDateTimeComparer
    {
        private ILog Log => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public FhirDateTimeComparer()
        {
        }

        public int Compare(
                FhirDateTime x,
                FhirDateTime y)
        {
            return x.ToDateTimeOffset(TimeSpan.Zero).UtcDateTime
                .CompareTo(
                y.ToDateTimeOffset(TimeSpan.Zero).UtcDateTime);
        }
    }
}