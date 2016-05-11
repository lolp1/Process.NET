using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Process.NET.Applied
{
    [SuppressMessage("ReSharper", "LoopCanBePartlyConvertedToQuery")]
    public class AppliedManager<T> : IAppliedManager<T> where T : IApplied
    {
        protected readonly Dictionary<string, T> InternalItems = new Dictionary<string, T>();

        public IReadOnlyDictionary<string, T> Items => InternalItems;

        public void Disable(T item)
        {
            throw new NotImplementedException();
        }

        public void Disable(string name)
        {
            throw new NotImplementedException();
        }

        public T this[string key] => InternalItems[key];

        public void EnableAll()
        {
            foreach (var item in InternalItems)
                if (!item.Value.IsEnabled)
                    item.Value.Disable();
        }

        public void DisableAll()
        {
            foreach (var item in InternalItems)
                if (item.Value.IsEnabled)
                    item.Value.Disable();
        }

        public void Remove(string name)
        {
            if (!InternalItems.ContainsKey(name))
                return;

            try
            {
                InternalItems[name].Dispose();
            }

            finally
            {
                InternalItems.Remove(name);
            }
        }

        public void Remove(T item)
        {
            Remove(item.Identifier);
        }

        public void RemoveAll()
        {
            foreach (var item in InternalItems)
                item.Value.Dispose();
            InternalItems.Clear();
        }

        public void Add(T applicable)
        {
            InternalItems.Add(applicable.Identifier, applicable);
        }

        public void Add(IEnumerable<T> applicableRange)
        {
            foreach (var applicable in applicableRange)
                Add(applicable);
        }
    }
}