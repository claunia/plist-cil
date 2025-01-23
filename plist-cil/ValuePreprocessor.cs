using System;
using System.Collections.Generic;

namespace Claunia.PropertyList
{
    /// <summary>
    ///     Allows you to override the default value class initialization for the values found
    ///     in the parsed plists by registering your own preprocessing implementations.
    /// </summary>
    public static class ValuePreprocessor
    {
        /// <summary>
        ///     Indicates the semantic type of content the preprocessor will work on--independent
        ///     from the underlying data type (which will be string in most cases anyway).
        /// </summary>
        public enum Type
        {
            BOOL, INTEGER, FLOATING_POINT, UNDEFINED_NUMBER,
            STRING, DATA, DATE
        };

        /// <summary>
        ///     A Null-Implementation of a preprocessor for registered, but passive, use cases.
        /// </summary>
        private static T NullPreprocessor<T>(T value) => value;
        private record struct TypeIdentifier(Type ValueType, System.Type ProcessingType);

        /// <summary>
        ///     Default preprocessors for all the standard cases.
        /// </summary>
        private static readonly Dictionary<TypeIdentifier, Delegate> _preprocessors = new()
        {
            { new TypeIdentifier(Type.BOOL, typeof(bool)), NullPreprocessor<bool> },
            { new TypeIdentifier(Type.BOOL, typeof(string)), NullPreprocessor<string> },
            { new TypeIdentifier(Type.INTEGER, typeof(string)), NullPreprocessor<string> },
            { new TypeIdentifier(Type.FLOATING_POINT, typeof(string)), NullPreprocessor<string> },
            { new TypeIdentifier(Type.UNDEFINED_NUMBER, typeof(string)), NullPreprocessor<string> },
            { new TypeIdentifier(Type.STRING, typeof(string)), NullPreprocessor<string> },
            { new TypeIdentifier(Type.DATA, typeof(string)), NullPreprocessor<string> },
            { new TypeIdentifier(Type.DATA, typeof(byte[])), NullPreprocessor<byte[]> },
            { new TypeIdentifier(Type.DATE, typeof(string)), NullPreprocessor<string> },
            { new TypeIdentifier(Type.DATE, typeof(double)), NullPreprocessor<double> },
        };

        /// <summary>
        ///     Register a custom preprocessor.
        /// </summary>
        public static void Register<T>(Func<T, T> preprocessor, Type type) => 
            _preprocessors[new(type, typeof(T))] = preprocessor;

        /// <summary>
        ///     Unregister a specific preprocessor--replaces it with a null implementation
        ///     to prevent argument errors.
        /// </summary>
        public static void Unregister<T>(Type type) =>
            _preprocessors[new(type, typeof(T))] = NullPreprocessor<T>;

        /// <summary>
        ///     Preprocess the supplied data using the appropriate registered implementation.
        /// </summary>
        /// <exception cref="ArgumentException">If no appropriate preprocessor--not even a default null implementation--was registered.</exception>
        public static T Preprocess<T>(T value, Type type) => 
            TryGetPreprocessor(type, out Func<T, T> preprocess)
                ? preprocess(value)
                : throw new ArgumentException($"Failed to find a preprocessor for value '{value}'.");

        /// <summary>
        ///     Gets the appropriate registered implementation--or null--and casts it back to the required type.
        /// </summary>
        private static bool TryGetPreprocessor<T>(Type type, out Func<T, T> preprocess)
        {
            if(_preprocessors.TryGetValue(new TypeIdentifier(type, typeof(T)), out Delegate preprocessor))
            {
                preprocess = (Func<T, T>) preprocessor;
                return true;
            }

            preprocess = default;
            return false;
        }
    }

}