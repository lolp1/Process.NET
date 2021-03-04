using Process.NET.Marshaling;

namespace Process.NET.Applied
{
    /// <summary>
    ///     Interface <see cref="IApplied" />
    /// </summary>
    /// <seealso cref="T:Process.NET.Marshaling.IDisposableState" />
    public interface IApplied : IDisposableState
    {
        /// <summary>
        ///     Disables this instance.
        /// </summary>
        void Disable();

        /// <summary>
        ///     Enables this instance.
        /// </summary>
        void Enable();

        /// <summary>
        ///     Gets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        string Identifier { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is enabled; otherwise, <c>false</c> .
        /// </value>
        bool IsEnabled { get; }
    }
}