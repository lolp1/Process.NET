using System;
using System.Text;
using Process.NET.Memory;
using Process.NET.Utilities;

namespace Process.NET.Marshaling
{
    /// <summary>
    ///     Class marshalling a value into the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the value. It can be a primitive or reference data type.</typeparam>
    public class MarshalledValue<T> : IMarshalledValue
    {
        /// <summary>
        ///     The reference of the <see cref="Process" /> object.
        /// </summary>
        protected readonly IProcess Process;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarshalledValue{T}" /> class.
        /// </summary>
        /// <param name="process">The reference of the <see cref="Process" /> object.</param>
        /// <param name="value">The value to marshal.</param>
        public MarshalledValue(IProcess process, T value)
        {
            // Save the parameters
            Process = process;
            Value = value;
            // Marshal the value
            Marshal();
        }

        public T Value { get; }

        /// <summary>
        ///     The memory allocated where the value is fully written if needed. It can be unused.
        /// </summary>
        public IAllocatedMemory Allocated { get; private set; }

        /// <summary>
        ///     The reference of the value. It can be directly the value or a pointer.
        /// </summary>
        public IntPtr Reference { get; private set; }

        /// <summary>
        ///     Releases all resources used by the <see cref="AllocatedMemory" /> object.
        /// </summary>
        public void Dispose()
        {
            // Free the allocated memory
            Allocated?.Dispose();
            // Set the pointer to zero
            Reference = IntPtr.Zero;
            // Avoid the finalizer
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~MarshalledValue()
        {
            Dispose();
        }

        /// <summary>
        ///     Marshals the value into the remote process.
        /// </summary>
        private void Marshal()
        {
            // If the type is string, it's a special case
            if (typeof (T) == typeof (string))
            {
                var text = Value.ToString();
                // Allocate memory in the remote process (string + '\0')
                Allocated = Process.MemoryFactory.Allocate(Randomizer.GenerateString(), text.Length + 1);
                // Write the value
                Allocated.Write(0, text, Encoding.UTF8);
                // Get the pointer
                Reference = Allocated.BaseAddress;
            }
            else
            {
                // For all other types
                // Convert the value into a byte array
                var byteArray = MarshalType<T>.ObjectToByteArray(Value);

                // If the value can be stored directly in registers
                if (MarshalType<T>.CanBeStoredInRegisters)
                    // Convert the byte array into a pointer
                    Reference = MarshalType<IntPtr>.ByteArrayToObject(byteArray);
                else
                {
                    // It's a bit more complicated, we must allocate some space into
                    // the remote process to store the value and get its pointer

                    // Allocate memory in the remote process
                    Allocated = Process.MemoryFactory.Allocate(Randomizer.GenerateString(), MarshalType<T>.Size);
                    // Write the value
                    Allocated.Write(0, Value);
                    // Get the pointer
                    Reference = Allocated.BaseAddress;
                }
            }
        }
    }
}