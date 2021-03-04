using System;
using System.Linq;

namespace Process.NET.Applied
{
    /// <summary>
    ///     Class ComplexAppliedManager.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="T:Process.NET.Applied.AppliedManager`1" />
    /// <seealso cref="T:Process.NET.Applied.IComplexAppliedManager`1" />
    public abstract class ComplexAppliedManager<T> : AppliedManager<T>, IComplexAppliedManager<T>
        where T : IComplexApplied
    {
        /// <summary>
        ///     Disables <see langword="this" /> instance.
        /// </summary>
        /// <param name="detour">The item.</param>
        /// <param name="dueToRules">
        ///     if set to <c>&lt;see langword="true"</c> [due to rules].
        /// </param>
        public abstract void Disable(T detour, bool dueToRules);

        /// <summary>
        ///     Disables <see langword="this" /> instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dueToRules">
        ///     if set to <c>&lt;see langword="true"</c> [due to rules].
        /// </param>
        public abstract void Disable(string name, bool dueToRules);

        /// <summary>
        ///     Disables all.
        /// </summary>
        /// <param name="dueToRules">
        ///     if set to <c>true</c> [due to rules].
        /// </param>
        public void DisableAll(bool dueToRules)
        {
            foreach (var value in InternalItems.Values.Where(applied => applied.IsEnabled))
            {
                Disable(value, dueToRules);
            }
        }

        /// <summary>
        ///     Enables <see langword="this" /> instance.
        /// </summary>
        /// <param name="item">The <paramref name="item" /> .</param>
        /// <param name="dueToRules">
        ///     if set to <c>&lt;see langword="true"</c> [due to rules].
        /// </param>
        public void Enable(T item, bool dueToRules)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.IsEnabled)
            {
                return;
            }

            item.Enable(dueToRules);
        }

        /// <summary>
        ///     Enables <see langword="this" /> instance.
        /// </summary>
        /// <param name="name">The <paramref name="name" /> .</param>
        /// <param name="dueToRules">
        ///     if set to <c>&lt;see langword="true"</c> [due to rules].
        /// </param>
        public void Enable(string name, bool dueToRules)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (InternalItems.TryGetValue(name, out T value))
            {
               Enable(value,dueToRules);
            }
        }

        /// <summary>
        ///     Enables all.
        /// </summary>
        /// <param name="dueToRules">
        ///     if set to <c>true</c> [due to rules].
        /// </param>
        public void EnableAll(bool dueToRules)
        {
            foreach (var value in InternalItems.Values.Where(applied => !applied.IsEnabled))
            {
                Enable(value, dueToRules);
            }
        }
    }
}