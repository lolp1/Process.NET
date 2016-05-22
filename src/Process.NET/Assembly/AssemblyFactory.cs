using System;
using System.Linq;
using System.Threading.Tasks;
using Process.NET.Assembly.Assemblers;
using Process.NET.Assembly.CallingConventions;
using Process.NET.Marshaling;
using Process.NET.Memory;
using Process.NET.Native.Types;
using Process.NET.Threads;
using Process.NET.Utilities;

namespace Process.NET.Assembly
{
    public class AssemblyFactory : IAssemblyFactory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AssemblyFactory" /> class.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="assembler">The assembler.</param>
        public AssemblyFactory(IProcess process, IAssembler assembler)
        {
            Process = process;
            Assembler = assembler;
        }

        /// <summary>
        ///     Gets or sets the assembler.
        /// </summary>
        /// <value>
        ///     The assembler.
        /// </value>
        public IAssembler Assembler { get; set; }

        /// <summary>
        ///     Gets the process.
        /// </summary>
        /// <value>
        ///     The process.
        /// </value>
        public IProcess Process { get; }

        /// <summary>
        ///     Begins a new transaction to inject and execute assembly code into the process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is injected.</param>
        /// <param name="autoExecute">Indicates whether the assembly code is executed once the object is disposed.</param>
        /// <returns>The return value is a new transaction.</returns>
        public AssemblyTransaction BeginTransaction(IntPtr address, bool autoExecute = true)
        {
            return new AssemblyTransaction(this, address, autoExecute);
        }

        /// <summary>
        ///     Begins a new transaction to inject and execute assembly code into the process.
        /// </summary>
        /// <param name="autoExecute">Indicates whether the assembly code is executed once the object is disposed.</param>
        /// <returns>The return value is a new transaction.</returns>
        public AssemblyTransaction BeginTransaction(bool autoExecute = true)
        {
            return new AssemblyTransaction(this, autoExecute);
        }

        /// <summary>
        ///     Releases all resources used by the <see cref="AssemblyFactory" /> object.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose... yet
        }

        /// <summary>
        ///     Executes the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is located.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public T Execute<T>(IntPtr address)
        {
            // Execute and join the code in a new thread
            var thread = Process.ThreadFactory.CreateAndJoin(address);
            // Return the exit code of the thread
            return thread.GetExitCode<T>();
        }

        /// <summary>
        ///     Executes the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is located.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public IntPtr Execute(IntPtr address)
        {
            return Execute<IntPtr>(address);
        }

        /// <summary>
        ///     Executes the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is located.</param>
        /// <param name="parameter">The parameter used to execute the assembly code.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public T Execute<T>(IntPtr address, dynamic parameter)
        {
            // Execute and join the code in a new thread
            IRemoteThread thread = Process.ThreadFactory.CreateAndJoin(address, parameter);
            // Return the exit code of the thread
            return thread.GetExitCode<T>();
        }

        /// <summary>
        ///     Executes the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is located.</param>
        /// <param name="parameter">The parameter used to execute the assembly code.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public IntPtr Execute(IntPtr address, dynamic parameter)
        {
            return Execute<IntPtr>(address, parameter);
        }

        /// <summary>
        ///     Executes the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is located.</param>
        /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
        /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public T Execute<T>(IntPtr address, Native.Types.CallingConventions callingConvention,
            params dynamic[] parameters)
        {
            // If we didn't attach an assembler, then this method will fail horribly
            if (Assembler == null)
                throw new NullReferenceException("No assembler supplied, method cannot continue");


            // Marshal the parameters
            var marshalledParameters =
                parameters.Select(p => MarshalValue.Marshal(Process, p)).Cast<IMarshalledValue>().ToArray();

            // Start a transaction
            AssemblyTransaction t;
            using (t = BeginTransaction())
            {
                // Get the object dedicated to create mnemonics for the given calling convention
                var calling = CallingConventionSelector.Get(callingConvention);
                // Push the parameters
                t.AddLine(calling.FormatParameters(marshalledParameters.Select(p => p.Reference).ToArray()));
                // Call the function
                t.AddLine(calling.FormatCalling(address));
                // Clean the parameters
                if (calling.Cleanup == CleanupTypes.Caller)
                    t.AddLine(calling.FormatCleaning(marshalledParameters.Length));
                // Add the return mnemonic
                t.AddLine("retn");
            }

            // Clean the marshalled parameters
            foreach (var parameter in marshalledParameters)
                parameter.Dispose();
            // Return the exit code
            return t.GetExitCode<T>();
        }

        /// <summary>
        ///     Executes the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is located.</param>
        /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
        /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public IntPtr Execute(IntPtr address, Native.Types.CallingConventions callingConvention,
            params dynamic[] parameters)
        {
            return Execute<IntPtr>(address, callingConvention, parameters);
        }

        /// <summary>
        ///     Executes asynchronously the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is located.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task<T> ExecuteAsync<T>(IntPtr address)
        {
            return Task.Run(() => Execute<T>(address));
        }

        /// <summary>
        ///     Executes asynchronously the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is located.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task<IntPtr> ExecuteAsync(IntPtr address)
        {
            return ExecuteAsync<IntPtr>(address);
        }

        /// <summary>
        ///     Executes asynchronously the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is located.</param>
        /// <param name="parameter">The parameter used to execute the assembly code.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task<T> ExecuteAsync<T>(IntPtr address, dynamic parameter)
        {
            return Task.Run(() => (Task<T>) Execute<T>(address, parameter));
        }

        /// <summary>
        ///     Executes asynchronously the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is located.</param>
        /// <param name="parameter">The parameter used to execute the assembly code.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task<IntPtr> ExecuteAsync(IntPtr address, dynamic parameter)
        {
            return ExecuteAsync<IntPtr>(address, parameter);
        }

        /// <summary>
        ///     Executes asynchronously the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is located.</param>
        /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
        /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task<T> ExecuteAsync<T>(IntPtr address, Native.Types.CallingConventions callingConvention,
            params dynamic[] parameters)
        {
            return Task.Run(() => Execute<T>(address, callingConvention, parameters));
        }

        /// <summary>
        ///     Executes asynchronously the assembly code located in the remote process at the specified address.
        /// </summary>
        /// <param name="address">The address where the assembly code is located.</param>
        /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
        /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task<IntPtr> ExecuteAsync(IntPtr address, Native.Types.CallingConventions callingConvention,
            params dynamic[] parameters)
        {
            return ExecuteAsync<IntPtr>(address, callingConvention, parameters);
        }

        /// <summary>
        ///     Assembles mnemonics and injects the corresponding assembly code into the remote process at the specified address.
        /// </summary>
        /// <param name="asm">The mnemonics to inject.</param>
        /// <param name="address">The address where the assembly code is injected.</param>
        public void Inject(string asm, IntPtr address)
        {
            Process.Memory.Write(address, Assembler.Assemble(asm, address));
        }

        /// <summary>
        ///     Assembles mnemonics and injects the corresponding assembly code into the remote process at the specified address.
        /// </summary>
        /// <param name="asm">An array containing the mnemonics to inject.</param>
        /// <param name="address">The address where the assembly code is injected.</param>
        public void Inject(string[] asm, IntPtr address)
        {
            Inject(string.Join("\n", asm), address);
        }

        /// <summary>
        ///     Assembles mnemonics and injects the corresponding assembly code into the remote process.
        /// </summary>
        /// <param name="asm">The mnemonics to inject.</param>
        /// <returns>The address where the assembly code is injected.</returns>
        public IAllocatedMemory Inject(string asm)
        {
            // Assemble the assembly code
            var code = Assembler.Assemble(asm);
            // Allocate a chunk of memory to store the assembly code
            var memory = Process.MemoryFactory.Allocate(Randomizer.GenerateString(), code.Length);
            // Inject the code
            Inject(asm, memory.BaseAddress);
            // Return the memory allocated
            return memory;
        }

        /// <summary>
        ///     Assembles mnemonics and injects the corresponding assembly code into the remote process.
        /// </summary>
        /// <param name="asm">An array containing the mnemonics to inject.</param>
        /// <returns>The address where the assembly code is injected.</returns>
        public IAllocatedMemory Inject(string[] asm)
        {
            return Inject(string.Join("\n", asm));
        }

        /// <summary>
        ///     Assembles, injects and executes the mnemonics into the remote process at the specified address.
        /// </summary>
        /// <param name="asm">The mnemonics to inject.</param>
        /// <param name="address">The address where the assembly code is injected.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public T InjectAndExecute<T>(string asm, IntPtr address)
        {
            // Inject the assembly code
            Inject(asm, address);
            // Execute the code
            return Execute<T>(address);
        }

        /// <summary>
        ///     Assembles, injects and executes the mnemonics into the remote process at the specified address.
        /// </summary>
        /// <param name="asm">The mnemonics to inject.</param>
        /// <param name="address">The address where the assembly code is injected.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public IntPtr InjectAndExecute(string asm, IntPtr address)
        {
            return InjectAndExecute<IntPtr>(asm, address);
        }

        /// <summary>
        ///     Assembles, injects and executes the mnemonics into the remote process at the specified address.
        /// </summary>
        /// <param name="asm">An array containing the mnemonics to inject.</param>
        /// <param name="address">The address where the assembly code is injected.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public T InjectAndExecute<T>(string[] asm, IntPtr address)
        {
            return InjectAndExecute<T>(string.Join("\n", asm), address);
        }

        /// <summary>
        ///     Assembles, injects and executes the mnemonics into the remote process at the specified address.
        /// </summary>
        /// <param name="asm">An array containing the mnemonics to inject.</param>
        /// <param name="address">The address where the assembly code is injected.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public IntPtr InjectAndExecute(string[] asm, IntPtr address)
        {
            return InjectAndExecute<IntPtr>(asm, address);
        }

        /// <summary>
        ///     Assembles, injects and executes the mnemonics into the remote process.
        /// </summary>
        /// <param name="asm">The mnemonics to inject.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public T InjectAndExecute<T>(string asm)
        {
            // Inject the assembly code
            using (var memory = Inject(asm))
                // Execute the code
                return Execute<T>(memory.BaseAddress);
        }

        /// <summary>
        ///     Assembles, injects and executes the mnemonics into the remote process.
        /// </summary>
        /// <param name="asm">The mnemonics to inject.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public IntPtr InjectAndExecute(string asm)
        {
            return InjectAndExecute<IntPtr>(asm);
        }

        /// <summary>
        ///     Assembles, injects and executes the mnemonics into the remote process.
        /// </summary>
        /// <param name="asm">An array containing the mnemonics to inject.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public T InjectAndExecute<T>(string[] asm)
        {
            return InjectAndExecute<T>(string.Join("\n", asm));
        }

        /// <summary>
        ///     Assembles, injects and executes the mnemonics into the remote process.
        /// </summary>
        /// <param name="asm">An array containing the mnemonics to inject.</param>
        /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
        public IntPtr InjectAndExecute(string[] asm)
        {
            return InjectAndExecute<IntPtr>(asm);
        }

        /// <summary>
        ///     Assembles, injects and executes asynchronously the mnemonics into the remote process at the specified address.
        /// </summary>
        /// <param name="asm">The mnemonics to inject.</param>
        /// <param name="address">The address where the assembly code is injected.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task<T> InjectAndExecuteAsync<T>(string asm, IntPtr address)
        {
            return Task.Run(() => InjectAndExecute<T>(asm, address));
        }

        /// <summary>
        ///     Assembles, injects and executes asynchronously the mnemonics into the remote process at the specified address.
        /// </summary>
        /// <param name="asm">The mnemonics to inject.</param>
        /// <param name="address">The address where the assembly code is injected.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task<IntPtr> InjectAndExecuteAsync(string asm, IntPtr address)
        {
            return InjectAndExecuteAsync<IntPtr>(asm, address);
        }

        /// <summary>
        ///     Assembles, injects and executes asynchronously the mnemonics into the remote process at the specified address.
        /// </summary>
        /// <param name="asm">An array containing the mnemonics to inject.</param>
        /// <param name="address">The address where the assembly code is injected.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task<T> InjectAndExecuteAsync<T>(string[] asm, IntPtr address)
        {
            return Task.Run(() => InjectAndExecute<T>(asm, address));
        }

        /// <summary>
        ///     Assembles, injects and executes asynchronously the mnemonics into the remote process at the specified address.
        /// </summary>
        /// <param name="asm">An array containing the mnemonics to inject.</param>
        /// <param name="address">The address where the assembly code is injected.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task<IntPtr> InjectAndExecuteAsync(string[] asm, IntPtr address)
        {
            return InjectAndExecuteAsync<IntPtr>(asm, address);
        }

        /// <summary>
        ///     Assembles, injects and executes asynchronously the mnemonics into the remote process.
        /// </summary>
        /// <param name="asm">The mnemonics to inject.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task<T> InjectAndExecuteAsync<T>(string asm)
        {
            return Task.Run(() => InjectAndExecute<T>(asm));
        }

        /// <summary>
        ///     Assembles, injects and executes asynchronously the mnemonics into the remote process.
        /// </summary>
        /// <param name="asm">The mnemonics to inject.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task<IntPtr> InjectAndExecuteAsync(string asm)
        {
            return InjectAndExecuteAsync<IntPtr>(asm);
        }

        /// <summary>
        ///     Assembles, injects and executes asynchronously the mnemonics into the remote process.
        /// </summary>
        /// <param name="asm">An array containing the mnemonics to inject.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task<T> InjectAndExecuteAsync<T>(string[] asm)
        {
            return Task.Run(() => InjectAndExecute<T>(asm));
        }

        /// <summary>
        ///     Assembles, injects and executes asynchronously the mnemonics into the remote process.
        /// </summary>
        /// <param name="asm">An array containing the mnemonics to inject.</param>
        /// <returns>
        ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
        ///     assembly code.
        /// </returns>
        public Task<IntPtr> InjectAndExecuteAsync(string[] asm)
        {
            return InjectAndExecuteAsync<IntPtr>(asm);
        }
    }
}