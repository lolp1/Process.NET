namespace Process.NET.Applied
{
    public interface IComplexApplied : IApplied
    {
        bool DisabledDueToRules { get; set; }
        bool IgnoreRules { get; }
        void Enable(bool disableDueToRules);
        void Disable(bool disableDueToRules);
    }
}