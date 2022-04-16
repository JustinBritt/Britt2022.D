namespace Britt2022.D.Factories.Dependencies.Hl7.Fhir.R4.Model
{
    using System;

    using global::Hl7.Fhir.Model;

    using Britt2022.D.InterfacesFactories.Dependencies.Hl7.Fhir.R4.Model;

    internal sealed class NullableValueFactory : INullableValueFactory
    {
        public NullableValueFactory()
        {
        }

        public INullableValue<T> Create<T>(
            T? value)
            where T : struct
        {
            INullableValue<T> instance = null;

            try
            {
                PrimitiveType primitiveType = value switch
                {
                    bool b => new FhirBoolean(
                        b),

                    decimal d => new FhirDecimal(
                        d),

                    int i => new PositiveInt(
                        i),

                    { } => throw new ArgumentNullException(nameof(value)),

                    _ => null
                };

                instance = (INullableValue<T>)primitiveType;
            }
            finally
            {
            }

            return instance;
        }
    }
}