namespace Process.NET.Applied
{
    /// <summary>
    ///     Interface <see cref="IComplexApplied" />
    /// </summary>
    /// <seealso cref="T:Process.NET.Applied.IApplied" />
    public interface IComplexApplied : IApplied
    {
        /// <summary>
        ///     Disables <see langword="this" /> instance.
        /// </summary>
        /// <param name="disableDueToRules">
        ///     if set to <c>true</c> [disable due to rules].
        /// </param>
        void Disable(bool disableDueToRules);

        /// <summary>
        ///     Enables <see langword="this" /> instance.
        /// </summary>
        /// <param name="disableDueToRules">
        ///     if set to <c>true</c> [disable due to rules].
        /// </param>
        void Enable(bool disableDueToRules);

        /// <summary>
        ///     Gets or sets a value indicating whether [disabled due to rules].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [disabled due to rules]; otherwise, <c>false</c> .
        /// </value>
        bool DisabledDueToRules { get; set; }

        /// <summary>
        ///     Gets a value indicating whether [ignore rules].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [ignore rules]; otherwise, <c>false</c> .
        /// </value>
        bool IgnoreRules { get; }
    }
}