namespace Process.NET.Applied
{
    public interface IComplexAppliedManager<T> : IAppliedManager<T> where T : IApplied
    {
        void Disable(T item, bool dueToRules);
        void Disable(string name, bool dueToRules);
        void Enable(T item, bool dueToRules);
        void Enable(string name, bool dueToRules);
        void DisableAll(bool dueToRules);
        void EnableAll(bool dueToRules);
    }
}