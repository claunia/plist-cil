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
        public enum Types
        {
            BOOL, INTEGER, FLOATING_POINT, UNDEFINED_NUMBER,
            STRING, DATA, DATE
        };
        
        private record struct TypeIdentifier(Types ValueType, Type ProcessingType);
        private static T NullPreprocessor<T>(T value) => value;

        private static readonly Dictionary<TypeIdentifier, Delegate> _preprocessors = new()
        {
            { new TypeIdentifier(Types.BOOL, typeof(bool)), NullPreprocessor<bool> },
            { new TypeIdentifier(Types.BOOL, typeof(string)), NullPreprocessor<string> },
            { new TypeIdentifier(Types.INTEGER, typeof(string)), NullPreprocessor<string> },
            { new TypeIdentifier(Types.FLOATING_POINT, typeof(string)), NullPreprocessor<string> },
            { new TypeIdentifier(Types.UNDEFINED_NUMBER, typeof(string)), NullPreprocessor<string> },
            { new TypeIdentifier(Types.STRING, typeof(string)), NullPreprocessor<string> },
            { new TypeIdentifier(Types.DATA, typeof(string)), NullPreprocessor<string> },
            { new TypeIdentifier(Types.DATA, typeof(byte[])), NullPreprocessor<byte[]> },
            { new TypeIdentifier(Types.DATE, typeof(string)), NullPreprocessor<string> },
            { new TypeIdentifier(Types.DATE, typeof(double)), NullPreprocessor<double> },
        };

        public static void Register<T>(Func<T, T> preprocessor, Types type) => 
            _preprocessors[new(type, typeof(T))] = preprocessor;

        public static void Unregister<T>(Types type) =>
            _preprocessors[new(type, typeof(T))] = NullPreprocessor<T>;

        public static T Preprocess<T>(T value, Types type) => 
            TryGetPreprocessor(type, out Func<T, T> preprocess)
                ? preprocess(value)
                : throw new ArgumentException($"Failed to find a preprocessor for value '{value}'.");

        private static bool TryGetPreprocessor<T>(Types type, out Func<T, T> preprocess)
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