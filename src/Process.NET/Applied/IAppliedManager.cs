using System.Collections.Generic;

namespace Process.NET.Applied
{
    /// <summary>
    ///     Interface <see cref="IAppliedManager`2" />
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="T"></typeparam>
    public interface IAppliedManager<TKey, T> where T : IApplied
    {
        /// <summary>
        ///     Gets the <see cref="T" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     T.
        /// </returns>
        T this[string key] { get; }

        /// <summary>
        ///     Adds the specified <paramref name="applicable" /> .
        /// </summary>
        /// <param name="applicable">
        ///     The <paramref name="applicable" /> .
        /// </param>
        void Add(T applicable);

        /// <summary>
        ///     Adds the specified applicable range.
        /// </summary>
        /// <param name="applicableRange">The applicable range.</param>
        void Add(IEnumerable<T> applicableRange);

        /// <summary>
        ///     Disables <see cref="Process.NET.Applied.IAppliedManager`2" />
        ///     instance.
        /// </summary>
        /// <param name="detour">The item.</param>
        void Disable(T detour);

        /// <summary>
        ///     Disables <see cref="Process.NET.Applied.IAppliedManager`2" />
        ///     instance.
        /// </summary>
        /// <param name="name">The name.</param>
        void Disable(TKey name);

        /// <summary>
        ///     Disables all.
        /// </summary>
        void DisableAll();

        /// <summary>
        ///     Enables <see cref="Process.NET.Applied.IAppliedManager`2" />
        ///     instance.
        /// </summary>
        /// <param name="item">The item.</param>
        void Enable(T item);

        /// <summary>
        ///     Enables <see cref="Process.NET.Applied.IAppliedManager`2" />
        ///     instance.
        /// </summary>
        /// <param name="name">The name.</param>
        void Enable(string name);

        /// <summary>
        ///     Enables all.
        /// </summary>
        void EnableAll();

        /// <summary>
        ///     Removes the specified <paramref name="item" /> .
        /// </summary>
        /// <param name="item">The item.</param>
        void Remove(T item);

        /// <summary>
        ///     Removes the specified <paramref name="name" /> .
        /// </summary>
        /// <param name="name">The name.</param>
        void Remove(TKey name);

        /// <summary>
        ///     Removes all.
        /// </summary>
        void RemoveAll();

        /// <summary>
        ///     Gets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        IReadOnlyDictionary<TKey, T> Items { get; }
    }
}