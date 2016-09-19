Process.NET 
===================
Process.NET is a tool for interacting with processes based around a library called "MemorySharp" by Jämes Ménétrey aka ZenLulz under the license on the pages linked below. Below are the mentioned authors original library's and his official website for the library.  

 https://github.com/ZenLulz/MemorySharp/
 
 http://binarysharp.com/
 
Process.NET is simply a result of both me learning to program as a newer developer interested in both C# and native code, and the lack of a few features I desired in the library I enjoyed using as a new programmer.


NuGet of latest version: 
https://www.nuget.org/packages/Process.NET/
----------


## Features and notes ##

The core features of the original MemorySharp all or mostly library still exist. However, they have been implemented as a set of interfaces instead.

 This is to allow different implementations of the already great design MemorySharp has, such as support for internal (aka, injected) operations or the need for specific implementation details.  
  > **Original features**
  
  > - Most original features listed here still exist.
  >  https://github.com/ZenLulz/MemorySharp.

 > **General feature, changes, and  additions from the original library **


> - Interface based design.
> - Keyboard and mouse hooks.
> - Pattern scanning for both functions and data patterns.
> - Reduced dependency on FASM.net and Improved x64 support.
> - Patches.


> **Internal process (aka injected) support**

> - Detours
> - Hooks
> - Fast memory reads using pointers/marshaling tricks
> - Implementation of existing features to better suite internal-process operations.

### The main interface that brings others together  ###

```csharp
    public interface IProcess : IDisposable
    {
        System.Diagnostics.Process Native { get; }
        SafeMemoryHandle Handle { get; }
        IMemory Memory { get; }
        IThreadFactory ThreadFactory { get; }
        IModuleFactory ModuleFactory { get; }
        IMemoryFactory MemoryFactory { get; }
        IWindowFactory WindowFactory { get; }
        IProcessModule this[string moduleName] { get; }
        IPointer this[IntPtr addr] { get; }
    }
```

Also, this abstraction is used to provide easier memory read/write implementations.

```csharp
    public abstract class ProcessMemory : IMemory
    {
        protected readonly SafeMemoryHandle Handle;

        protected ProcessMemory(SafeMemoryHandle handle)
        {
            Handle = handle;
        }

        public abstract byte[] Read(IntPtr intPtr, int length);

        public string Read(IntPtr intPtr, Encoding encoding, int maxLength)
        {
            var buffer = Read(intPtr, maxLength);
            var ret = encoding.GetString(buffer);
            if (ret.IndexOf('\0') != -1)
                ret = ret.Remove(ret.IndexOf('\0'));
            return ret;
        }

        public abstract T Read<T>(IntPtr intPtr);

        public T[] Read<T>(IntPtr intPtr, int length)
        {
            var buffer = new T[length];
            for (var i = 0; i < buffer.Length; i++)
                buffer[i] = Read<T>(intPtr);
            return buffer;
        }

        public abstract int Write(IntPtr intPtr, byte[] bytesToWrite);

        public void Write(IntPtr intPtr, string stringToWrite, Encoding encoding)
        {
            if (stringToWrite[stringToWrite.Length - 1] != '\0')
                stringToWrite += '\0';
            var bytes = encoding.GetBytes(stringToWrite);
            Write(intPtr, bytes);
        }


        public void Write<T>(IntPtr intPtr, T[] values)
        {
            foreach (var value in values)
                Write(intPtr, value);
        }

        public abstract void Write<T>(IntPtr intPtr, T value);
    }
```

## Examples of new (and old) features##

###  Pattern scanning  ###

The way pattern scanning has been added is through interfaces, and a default implementation for both function and data patterns have been included. In most cases, they will be all you need. Here are the default implementation examples.

Pattern scanning for a function offset:

```csharp
     public class TestClass
    {
        public readonly IMemoryPattern DataAddressPattern =
            new DwordPattern("48 8B 05 ?? ?? ?? ?? 48 85 C0 48 0F 44 05 ?? ?? ?? ?? C3", 0x40);

        public readonly IMemoryPattern FuncOffsetPattern =
            new DwordPattern("48 8B 05 ?? ?? ?? ?? 48 85 C0 48 0F 44 05 ?? ?? ?? ?? C3");

        public PatternScanResult Find(string moduleName,IMemoryPattern pattern)
        {
            var process = new ProcessSharp(System.Diagnostics.Process.GetCurrentProcess());
            process.Memory = new ExternalProcessMemory(process.Handle);
            var scanner = new PatternScanner(process[moduleName]);
            return scanner.Find(pattern);
        }
    }
```			
 


###  Internal - Implementing the base WndProc hook class to invoke code inside the main thread from your injected C# program ###


```csharp

    public class WindowHook : WndProcHook
    {        

        public enum UserMessage
        {
            SayHi,
            SayBye
        }

        public WindowHook(IntPtr handle) : base(handle,"ExampleWndProc")
        {
        }

        public bool HandleUserMessage(IntPtr wpparam)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch ((UserMessage)wpparam)
            {
                case UserMessage.SayHi:
                    MessageBox.Show("Hi");
                    return true;
                case UserMessage.SayBye:
                    MessageBox.Show("Bye");
                    return true;
            }
            return false;
        }

        public void Invoke(UserMessage msg)
        {
            SendMessage((int) WindowsMessages.User, (IntPtr) msg);
        }

        protected override IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == (int) WindowsMessages.User && HandleUserMessage(wParam))
                return IntPtr.Zero;

            return base.WndProc(hWnd, msg, wParam, lParam);
        }
    }
```

And an example of using it:

```csharp
    public class TestClass
    {
        private WindowHook _window;

        public void Install(IntPtr handle)
        {
            _window = new WindowHook(handle);
            _window.Enable();
            _window.Invoke(UserMessage.SayHi);
        }

        public void Uninstall()
        {
            _window.Invoke(WindowHook.UserMessage.SayBye);
            _window.Disable();
        }
    }

```

### Keyboard / Mouse hooks ###

A basic keyboard hook use

```csharp
    public class TestClass
    {
        private KeyboardHook _keyboardHook;

        public void Install(string name)
        {
            _keyboardHook = new KeyboardHook(name);
            _keyboardHook.KeyDownEvent += args =>
            {
                if (args.IsAltPressed && args.Key == Keys.A)
                    Console.WriteLine("The A and alt keys were pressed together.");

            };
        }
    }

```
And the mouse hook

```csharp
    public class TestClass
    {
        private MouseHook _mouseHook;

        public void Install()
        {
           _mouseHook = new MouseHook();
            _mouseHook.LeftButtonDown +=
                (sender, args) =>
                    Console.WriteLine($"The mouse was at the position: {args.Position} when left clicked.");
            _mouseHook.Enable();
        }
    }
```


### Remote and unmanaged code execution ###

 Using the Marshal.GetDelegateForFunctionPointer<T> to call user32.dll messagebox

```csharp
    public static class Program
    {
        public static ProcessSharp ProcessSharp { get; set; }
        public delegate void MessageBox(IntPtr hWnd, string text, string caption, uint type);

        public static void Main(string[] args)
        {
            ProcessSharp  = new ProcessSharp(System.Diagnostics.Process.GetCurrentProcess());
            ProcessSharp.Memory = new ExternalProcessMemory(process.Handle);		
            // get a process function instance for MessageBox function in user32.dll
            var processFunction = ProcessSharp.ModuleFactory["user32"]["MessageBoxA"];
            // create a delegate for it.
            var @delegate = processFunction.GetDelegate<MessageBox>();
            // show a message box using the user32.dll MessageBox api.
            @delegate.Invoke(IntPtr.Zero, "Hello world!", "title", 0);
        }
    }
```


 Implementing the assembly factory the way MemorySharp does with FASM.Net and using it

```csharp

    public class Fasm32Assembler : IAssembler
    {
        public byte[] Assemble(string asm)
        {
            return Assemble(asm, IntPtr.Zero);
        }

        public byte[] Assemble(string asm, IntPtr baseAddress)
        {
            asm = $"use32\norg 0x{baseAddress.ToInt64():X8}\n" + asm;
            return FasmNet.Assemble(asm);
        }
    }
	
    public static class Program
    {
        public static ProcessSharp ProcessSharp { get; set; }
        public static AssemblyFactory Factory { get; set; }
        public static void Main(string[] args)
        {
            ProcessSharp = new ProcessSharp(System.Diagnostics.Process.GetProcessesByName("ProcessName").FirstOrDefault());
            ProcessSharp.Memory = new ExternalProcessMemory(ProcessSharp.Handle);
            Factory = new AssemblyFactory(ProcessSharp, new Fasm32Assembler());
            // int(int input) => input * 2;
            var processFunction = ProcessSharp.ModuleFactory["SomeLib.dll"]["SomeFunc"];
            var a = Factory.Execute<int>(processFunction.BaseAddress, 5);
            // out put would be 10.
            Console.WriteLine(a);

            // All the classic examples from the MemorySharp lib are applicable.
            
            var address = IntPtr.Zero;
            // Execute code and get the return value as boolean
            var ret = Factory.Execute<bool>(address);
            Console.WriteLine(ret.ToString());

            var parameterA = new IntPtr(0x500);
            var point = Factory.Execute<Point>(address,CallingConventions.Stdcall, parameterA, "parameterB");
            Console.WriteLine(point.ToString());
			
			
            // Inject mnemonics
            Factory.Inject(
                new[]
                    {
            "push 0",
            "add esp, 4",
            "retn"
                    },
                address);

            // Inject and execute code lazily.
            using (var t = Factory.BeginTransaction())
            {
                t.AddLine("mov eax, {0}", address);
                t.AddLine("call eax");
                t.AddLine("retn");
            }
        }
      }
```

### Samples of the original library functionality included with this library below are included for more exposure of the features. The original documentation of them is better and still apples. ###


**Window operations**
```csharp

  		    var process  = new ProcessSharp(System.Diagnostics.Process.GetCurrentProcess());
            process.Memory = new ExternalProcessMemory(process.Handle);
            // Find Scintilla
            var scintilla = ProcessSharp.WindowFactory.GetWindowsByClassName("Scintilla").FirstOrDefault();
            // If scintilla was found, write something
            scintilla?.Keyboard.Write("Hello, World!");
			
			
			var process  = new ProcessSharp(System.Diagnostics.Process.GetCurrentProcess());
            process.Memory = new ExternalProcessMemory(process.Handle);
            // Get the window
            var window = process.WindowFactory.MainWindow;
            // Activate it to be in foreground
            window.Activate();
            // Move the cursor
            window.Mouse.MoveTo(0, 0);
            // Perform a left click
            window.Mouse.ClickLeft();
			
			var process  = new ProcessSharp(System.Diagnostics.Process.GetCurrentProcess());
            process.Memory = new ExternalProcessMemory(process.Handle);

            // Get the window
            var window = process.WindowFactory.MainWindow;
            // Press the bottom arrow down and repeat the message every 20ms
            window.Keyboard.Press(Keys.Down, TimeSpan.FromMilliseconds(20));
            // Wait 3 seconds
            Thread.Sleep(3000);
            // Release the key
            window.Keyboard.Release(Keys.Down);
```

**Memory operations**
```csharp


			var process  = new ProcessSharp(System.Diagnostics.Process.GetCurrentProcess());
            process.Memory = new ExternalProcessMemory(process.Handle);

            var address = IntPtr.Zero;
            // Read an array of 3 integers 
            var integers = process.Memory.Read<int>(address, 3);
            foreach (var integer in integers)
                Console.WriteLine(integer);
            // Write a string
            process.Memory.Write(address, "I love managed languages.");
			
			
			var process  = new ProcessSharp(System.Diagnostics.Process.GetCurrentProcess());
            process.Memory = new ExternalProcessMemory(process.Handle);

            var address = IntPtr.Zero;
            var offset = 0x500;
            // Read an array of 3 integers at address + offset.
            var integersA = process[address].Read<int>(offset, 3);
            foreach(var integer in integersA)
                Console.WriteLine(integer);
            // You can also do it from a module instance.
            var moduleName = "SomeModule.dll";
            var integersB = process[moduleName].Read<int>(offset, 3);
            foreach (var integer in integersB)
                Console.WriteLine(integer);
            // Write a string.
            process[address].Write(offset, "I love managed languages.");
            process[moduleName].Write(offset, "I love managed languages.");
            
            

```

## Credits ##

 - Jämes Ménétrey aka ZenLulz for writing the MemorySharp library.
 - Apoc for his good post and GreyMagic library from which Detours and Patches are implemented.
 - Zat from unknowncheats.me for just good post and decency to new programmers such as myself in general.
 - aganonki from unknowncheats.me for his HackTools library and help with writing better more flexible code in general.
 - Jadd @ ownedcore.com for his help with everything in general, but most specifically his WndProc hook example which got me started programming more seriously as the newb I was and am.
 - miceiken for his icedflake project to reference.
 - jeffora for his extmemory project to reference
 - aevitas for his BlueRain project and Orion project to reference and is actually where the the base memory abstraction is based from.


### Links to check out ###

 -  http://binarysharp.com/ (ZenLulz's website )
 - https://github.com/aevitas
 - https://github.com/aganonki
 - https://github.com/BigMo (Zats github)
 - https://github.com/jeffora 
 - https://github.com/miceiken
 - https://github.com/Dramacydal/DirtyDeeds/tree/master/DirtyDeeds
 - https://github.com/Dramacydal/WowMoPObjMgrTest 
 - https://github.com/unknowndev/CoolFish
 - http://blog.ntoskr.nl (Jadds blog)
 - http://www.ownedcore.com/
 - http://www.unknowncheats.me/
 - http://www.blizzhackers.cc/
