using System.Diagnostics.CodeAnalysis;

namespace EnTTSharp.Entities
{
    /// <summary>
    ///   A placeholder struct that indicates the absense of a component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public readonly struct Not<T>
    {
    }
}