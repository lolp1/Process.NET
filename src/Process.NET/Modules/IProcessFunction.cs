using System;

namespace Process.NET.Modules
{
    public interface IProcessFunction
    {
        IntPtr BaseAddress { get; }
        string Name { get; }
        T GetDelegate<T>();
    }
}