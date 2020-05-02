using System;
using System.Threading.Tasks;
using Process.NET.Assembly.Assemblers;
using Process.NET.Memory;

namespace Process.NET.Assembly
{
    public interface IAssemblyFactory : IDisposable
    {
        IAssembler Assembler { get; set; }
        IProcess Process { get; }
        AssemblyTransaction BeginTransaction(bool autoExecute = true);
        AssemblyTransaction BeginTransaction(IntPtr address, bool autoExecute = true);

        IntPtr Execute(IntPtr address);
        IntPtr Execute(IntPtr address, dynamic parameter);
        IntPtr Execute(IntPtr address, Native.Types.CallingConventions callingConvention, params dynamic[] parameters);
        T Execute<T>(IntPtr address);
        T Execute<T>(IntPtr address, dynamic parameter);
        T Execute<T>(IntPtr address, Native.Types.CallingConventions callingConvention, params dynamic[] parameters);
        Task<IntPtr> ExecuteAsync(IntPtr address);
        Task<IntPtr> ExecuteAsync(IntPtr address, dynamic parameter);

        Task<IntPtr> ExecuteAsync(IntPtr address, Native.Types.CallingConventions callingConvention,
            params dynamic[] parameters);

        Task<T> ExecuteAsync<T>(IntPtr address);
        Task<T> ExecuteAsync<T>(IntPtr address, dynamic parameter);

        Task<T> ExecuteAsync<T>(IntPtr address, Native.Types.CallingConventions callingConvention,
            params dynamic[] parameters);

        IAllocatedMemory Inject(string[] asm);
        IAllocatedMemory Inject(string asm);
        void Inject(string[] asm, IntPtr address);
        void Inject(string asm, IntPtr address);
        IntPtr InjectAndExecute(string[] asm);
        IntPtr InjectAndExecute(string asm);
        IntPtr InjectAndExecute(string[] asm, IntPtr address);
        IntPtr InjectAndExecute(string asm, IntPtr address);
        T InjectAndExecute<T>(string[] asm);
        T InjectAndExecute<T>(string asm);
        T InjectAndExecute<T>(string[] asm, IntPtr address);
        T InjectAndExecute<T>(string asm, IntPtr address);
        Task<IntPtr> InjectAndExecuteAsync(string[] asm);
        Task<IntPtr> InjectAndExecuteAsync(string asm);
        Task<IntPtr> InjectAndExecuteAsync(string[] asm, IntPtr address);
        Task<IntPtr> InjectAndExecuteAsync(string asm, IntPtr address);
        Task<T> InjectAndExecuteAsync<T>(string[] asm);
        Task<T> InjectAndExecuteAsync<T>(string asm);
        Task<T> InjectAndExecuteAsync<T>(string[] asm, IntPtr address);
        Task<T> InjectAndExecuteAsync<T>(string asm, IntPtr address);
    }
}