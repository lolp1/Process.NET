namespace Process.NET.Applied
{
    /// <summary>
    /// Interface IComplexAppliedManager
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="string" />
    public interface IComplexAppliedManager<T> : IAppliedManager<string, T> where T : IApplied
    {
        /// <summary>
        /// Disables <see langword="this"/> instance.
        /// </summary>
        /// <param name="detour">The detour.</param>
        /// <param name="dueToRules">if set to <c><see langword="true"/></c> [due to rules].</param>
        void Disable(T detour, bool dueToRules);

        /// <summary>
        /// Disables <see langword="this"/> instance.
        /// </summary>
        /// <param name="name">The <paramref name="name"/>.</param>
        /// <param name="dueToRules">if set to <c>true</c> [due to rules].</param>
        void Disable(string name, bool dueToRules);

        /// <summary>
        /// Disables all.
        /// </summary>
        /// <param name="dueToRules">if set to <c>true</c> [due to rules].</param>
        void DisableAll(bool dueToRules);

        /// <summary>
        /// Enables <see langword="this"/> instance.
        /// </summary>
        /// <param name="name">The <paramref name="name"/>.</param>
        /// <param name="dueToRules">if set to <c>true</c> [due to rules].</param>
        void Enable(string name, bool dueToRules);

        /// <summary>
        /// Enables all.
        /// </summary>
        /// <param name="dueToRules">if set to <c>true</c> [due to rules].</param>
        void EnableAll(bool dueToRules);
    }
}