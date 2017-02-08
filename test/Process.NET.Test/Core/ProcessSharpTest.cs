using NUnit.Framework;

namespace Process.NET.Test
{
    [TestFixture]
    public class ProcessSharpTest
    {
        [Test]
        public void TestLocalMemory()
        {
            var process = new ProcessSharp(System.Diagnostics.Process.GetCurrentProcess(), Memory.MemoryType.Local);
            Assert.That(process.Memory.GetType() == typeof(Memory.LocalProcessMemory), "Failed to find LocalProcessMemory given MemoryType.Local");
        }

        [Test]
        public void TestRemoteMemory()
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("notepad++");
            Assert.Greater(processes.Length, 0, "Failed to find a running instance of Notepad++.");
            var process = new ProcessSharp(processes[0], Process.NET.Memory.MemoryType.Remote);
            Assert.That(process.Memory.GetType() == typeof(Memory.ExternalProcessMemory), "Failed to find ExternalProcessMemory given MemoryType.Remote");
        }
    }
}
