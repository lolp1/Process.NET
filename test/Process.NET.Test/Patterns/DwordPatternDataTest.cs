using NUnit.Framework;
using Process.NET.Patterns;

namespace Process.NET.Test.Patterns
{
    [TestFixture]
    class DwordPatternDataTest
    {
        [Test]
        [Ignore("I don't think Local can be used with PatternScanner, due to SystemAccessViolation being thrown. Fix test in future if possible.")]
        public void TestLocalDwordDataPattern()
        {
            var process = new ProcessSharp(System.Diagnostics.Process.GetCurrentProcess(), Memory.MemoryType.Local);
            var module = process.ModuleFactory.MainModule;
            var scanner = new PatternScanner(module);
            Assert.NotNull(scanner, "Failed to instantiate PatternScanner object.");
            Assert.NotNull(scanner.Data, "Failed to read local memory in to Data object.");
            var pattern = new DwordPatternData("E8 ? ? ? ? 83 C4", 1); //Most common x86 signature. CALL DWORD ADD ESP, X.
            var result = scanner.Find(pattern);
            Assert.IsTrue(result.Found, "Failed to find signature in TestLocalDwordDataPattern.");
            Assert.IsNotNull(result.Offset, "Offset was null in TestLocalDwordDataPattern.");
            Assert.IsNotNull(result.ReadAddress, "Failed to read from retrieved pattern address in TestLocalDwordDataPattern.");
        }

        [Test]
        public void TestRemoteDwordDataPattern()
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("notepad++");
            Assert.Greater(processes.Length, 0, "Failed to find a running instance of Notepad++.");
            var process = new ProcessSharp(processes[0], Process.NET.Memory.MemoryType.Remote);
            var module = process.ModuleFactory.MainModule;
            var scanner = new PatternScanner(module);
            Assert.NotNull(scanner, "Failed to instantiate PatternScanner object.");
            Assert.NotNull(scanner.Data, "Failed to read MainModule from Notepad++ in to Data object.");
            var pattern = new DwordPatternData("E8 ? ? ? ? 83 C4", 1); //Most common x86 signature. CALL DWORD ADD ESP, X.
            var result = scanner.Find(pattern);
            Assert.IsTrue(result.Found, "Failed to find signature in TestRemoteDwordDataPattern.");
            Assert.IsNotNull(result.Offset, "Offset was null in TestRemoteDwordDataPattern.");
            Assert.IsNotNull(result.ReadAddress, "Failed to read from retrieved pattern address in TestRemoteDwordDataPattern.");
        }
    }
}
