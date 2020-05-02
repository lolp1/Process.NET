/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2014 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

namespace Process.NET.Marshaling
{
    /// <summary>
    ///     The factory to create instance of the <see cref="MarshalledValue{T}" /> class.
    /// </summary>
    /// <remarks>
    ///     A factory pattern is used because C# 5.0 constructor doesn't support type inference.
    ///     More info from Eric Lippert here :
    ///     http://stackoverflow.com/questions/3570167/why-cant-the-c-sharp-constructor-infer-type
    /// </remarks>
    public static class MarshalValue
    {
        /// <summary>
        ///     Marshals a given value into the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value. It can be a primitive or reference data type.</typeparam>
        /// <param name="memorySharp">The concerned process.</param>
        /// <param name="value">The value to marshal.</param>
        /// <returns>The return value is an new instance of the <see cref="MarshalledValue{T}" /> class.</returns>
        public static MarshalledValue<T> Marshal<T>(IProcess memorySharp, T value)
        {
            return new MarshalledValue<T>(memorySharp, value);
        }
    }
}