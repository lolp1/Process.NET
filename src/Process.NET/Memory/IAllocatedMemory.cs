using Process.NET.Marshaling;

namespace Process.NET.Memory
{
    public interface IAllocatedMemory : IPointer, IDisposableState
    {
        bool IsAllocated { get; }
        int Size { get; }
        string Identifier { get; }
    }
}