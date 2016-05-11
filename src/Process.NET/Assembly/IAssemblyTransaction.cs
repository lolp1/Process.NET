using System;

namespace Process.NET.Assembly
{
    public interface IAssemblyTransaction
    {
        IntPtr Address { get; }
        bool IsAutoExecuted { get; set; }

        void AddLine(string asm, params object[] args);
        byte[] Assemble();
        void Clear();
        void Dispose();
        T GetExitCode<T>();
        void InsertLine(int index, string asm, params object[] args);
    }
}