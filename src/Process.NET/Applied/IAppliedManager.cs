using System.Collections.Generic;

namespace Process.NET.Applied
{
    public interface IAppliedManager<T> where T : IApplied
    {
        T this[string key] { get; }

        IReadOnlyDictionary<string, T> Items { get; }
        void Disable(T item);
        void Disable(string name);
        void DisableAll();
        void EnableAll();
        void Remove(T item);
        void Remove(string name);
        void RemoveAll();
        void Add(T applicable);
        void Add(IEnumerable<T> applicableRange);
    }
}