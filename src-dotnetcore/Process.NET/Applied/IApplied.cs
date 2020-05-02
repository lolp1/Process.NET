using Process.NET.Marshaling;

namespace Process.NET.Applied
{
    public interface IApplied : IDisposableState
    {
        string Identifier { get; }
        bool IsEnabled { get; }
        void Disable();
        void Enable();
    }
}