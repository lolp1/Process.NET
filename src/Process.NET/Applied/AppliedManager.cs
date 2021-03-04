using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Process.NET.Applied
{
    /// <summary>
    ///     Class AppliedManager.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="T:Process.NET.Applied.IAppliedManager`2" />
    public abstract class AppliedManager<T> : IDisposable, IAppliedManager<string, T> where T : IApplied
    {
        /// <summary>
        ///     Gets the <see cref="T" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     T.
        /// </returns>
        public T this[string key] => InternalItems[key];

        /// <summary>
        ///     Adds the specified <paramref name="applicable" /> .
        /// </summary>
        /// <param name="applicable">The applicable.</param>
        public void Add(T applicable)
        {
            if (applicable == null)
            {
                throw new ArgumentNullException(nameof(applicable));
            }

            if (string.IsNullOrEmpty(applicable.Identifier))
            {
                return;
            }

            if (!InternalItems.ContainsKey(applicable.Identifier))
            {
                InternalItems.TryAdd(applicable.Identifier, applicable);
            }
        }

        /// <summary>
        ///     Adds the specified applicable range.
        /// </summary>
        /// <param name="applicableRange">The applicable range.</param>
        public void Add(IEnumerable<T> applicableRange)
        {
            if (applicableRange == null)
            {
                throw new ArgumentNullException(nameof(applicableRange));
            }

            foreach (var applicable in applicableRange)
            {
                Add(applicable);
            }
        }

        /// <summary>
        ///     Disables <see cref="Process.NET.Applied.AppliedManager`1" />
        ///     instance.
        /// </summary>
        /// <param name="detour">The item.</param>
        /// <exception cref="NotImplementedException" />
        public abstract void Disable(T detour);

        /// <summary>
        ///     Disables <see cref="Process.NET.Applied.AppliedManager`1" />
        ///     instance.
        /// </summary>
        /// <param name="name">The <paramref name="name" /> .</param>
        /// <exception cref="NotImplementedException" />
        public abstract void Disable(string name);

        /// <summary>
        ///     Disables all.
        /// </summary>
        public void DisableAll()
        {
            foreach (var item in InternalItems.Values.Where(item => item.IsEnabled))
            {
                Disable(item);
            }
        }

        /// <summary>
        ///     Enables this instance.
        /// </summary>
        /// <param name="item">The item.</param>
        public abstract void Enable(T item);

        /// <summary>
        ///     Enables this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        public abstract void Enable(string name);

        /// <summary>
        ///     Enables all.
        /// </summary>
        public void EnableAll()
        {
            foreach (var item in InternalItems.Values.Where(item => !item.IsEnabled))
            {
                Enable(item);
            }
        }

        /// <summary>
        ///     Removes the specified <paramref name="name" /> .
        /// </summary>
        /// <param name="name">The name.</param>
        public void Remove(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (InternalItems.ContainsKey(name))
            {
                var removed = false;
                var value = default(T);
                try
                {
                    removed = InternalItems.TryRemove(name, out value);
                }
                finally
                {
                    if (removed)
                    {
                        value?.Dispose();
                    }
                }
            }
        }

        /// <summary>
        ///     Removes the specified <paramref name="item" /> .
        /// </summary>
        /// <param name="item">The item.</param>
        public void Remove(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Remove(item.Identifier);
        }

        /// <summary>
        ///     Removes all.
        /// </summary>
        public void RemoveAll()
        {
            foreach (var item in InternalItems.Values.ToArray())
            {
                Remove(item);
            }
        }

        /// <summary>
        ///     Gets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        public IReadOnlyDictionary<string, T> Items => InternalItems;

        /// <summary>
        ///     The <see langword="internal" /> items
        /// </summary>
        abstract protected ConcurrentDictionary<string, T> InternalItems { get; }

        private void ReleaseUnmanagedResources()
        {
            DisableAll();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                RemoveAll();
            }
            else
            {
                ReleaseUnmanagedResources();
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
        ~AppliedManager()
        {
            Dispose(false);
        }
    }
}