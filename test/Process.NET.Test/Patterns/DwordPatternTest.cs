using NUnit.Framework;
using Process.NET.Patterns;

namespace Process.NET.Test.Patterns
{
    [TestFixture]
    class DwordPatternTest
    {
        [Test]
        [Ignore("I don't think Local can be used with PatternScanner, due to SystemAccessViolation being thrown. Fix test in future if possible.")]
        public void TestLocalDwordPattern()
        {
            var process = new ProcessSharp(System.Diagnostics.Process.GetCurrentProcess(), Memory.MemoryType.Local);
            var module = process.ModuleFactory.MainModule;
            var scanner = new PatternScanner(module);
            Assert.NotNull(scanner, "Failed to instantiate PatternScanner object.");
            Assert.NotNull(scanner.Data, "Failed to read local memory in to Data object.");
            var pattern = new DwordPattern("E8 ? ? ? ? 83 C4"); //Most common x86 signature. CALL DWORD ADD ESP, X.
            var result = scanner.Find(pattern);
            Assert.IsTrue(result.Found, "Failed to find signature in TestLocalDwordPattern.");
            Assert.IsNotNull(result.Offset, "Offset was null in TestLocalDwordPattern.");
        }

        [Test]
        public void TestRemoteBHMPattern()
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("notepad++");
            Assert.Greater(processes.Length, 0, "Failed to find a running instance of Notepad++.");
            var process = new ProcessSharp(processes[0], Process.NET.Memory.MemoryType.Remote);
            var module = process.ModuleFactory.MainModule;
            var scanner = new PatternScanner(module);
            Assert.NotNull(scanner, "Failed to instantiate PatternScanner object.");
            Assert.NotNull(scanner.Data, "Failed to read MainModule from Notepad++ in to Data object.");
            var pattern = new DwordPattern("E8 ? ? ? ? 83 C4", PatternScannerAlgorithm.BoyerMooreHorspool); //Most common x86 signature. CALL DWORD ADD ESP, X.
            var result = scanner.Find(pattern);
            Assert.IsTrue(result.Found, "Failed to find signature in TestRemoteDwordPattern.");
            Assert.That(result.Offset > -1, "TestRemoteNaivePattern offset was not greater than -1.");
        }

        [Test]
        public void TestRemoteNaivePattern()
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("notepad++");
            Assert.Greater(processes.Length, 0, "Failed to find a running instance of Notepad++.");
            var process = new ProcessSharp(processes[0], Process.NET.Memory.MemoryType.Remote);
            var module = process.ModuleFactory.MainModule;
            var scanner = new PatternScanner(module);
            Assert.NotNull(scanner, "Failed to instantiate PatternScanner object.");
            Assert.NotNull(scanner.Data, "Failed to read MainModule from Notepad++ in to Data object.");
            var pattern = new DwordPattern("E8 ? ? ? ? 83 C4", PatternScannerAlgorithm.Naive); //Most common x86 signature. CALL DWORD ADD ESP, X.
            var result = scanner.Find(pattern);
            Assert.IsTrue(result.Found, "Failed to find signature in TestRemoteDwordPattern.");
            Assert.That(result.Offset > -1, "TestRemoteNaivePattern offset was not greater than -1.");
        }

        [Test]
        public void TestUnfindableBMHPattern()
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("notepad++");
            Assert.Greater(processes.Length, 0, "Failed to find a running instance of Notepad++.");
            var process = new ProcessSharp(processes[0], Process.NET.Memory.MemoryType.Remote);
            var module = process.ModuleFactory.MainModule;
            var scanner = new PatternScanner(module);
            Assert.NotNull(scanner, "Failed to instantiate PatternScanner object.");
            Assert.NotNull(scanner.Data, "Failed to read MainModule from Notepad++ in to Data object.");
            var pattern = new DwordPattern("69 42 06 66 11 22 33 44 55 66 77 88 99", PatternScannerAlgorithm.BoyerMooreHorspool); //Most common x86 signature. CALL DWORD ADD ESP, X.
            var result = scanner.Find(pattern);
            Assert.IsFalse(result.Found, "TestUnfindableBMHPattern yielded an offset when it wasn't supposed to.");
            Assert.That(result.Offset == 0, "Offset was not special number 0");
        }

        [Test]
        public void TestUnfindableNaivePattern()
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("notepad++");
            Assert.Greater(processes.Length, 0, "Failed to find a running instance of Notepad++.");
            var process = new ProcessSharp(processes[0], Process.NET.Memory.MemoryType.Remote);
            var module = process.ModuleFactory.MainModule;
            var scanner = new PatternScanner(module);
            Assert.NotNull(scanner, "Failed to instantiate PatternScanner object.");
            Assert.NotNull(scanner.Data, "Failed to read MainModule from Notepad++ in to Data object.");
            var pattern = new DwordPattern("69 42 06 66 11 22 33 44 55 66 77 88 99"); //Most common x86 signature. CALL DWORD ADD ESP, X.
            var result = scanner.Find(pattern);
            Assert.IsFalse(result.Found, "TestUnfindableNaivePattern yielded an offset when it wasn't supposed to.");
            Assert.That(result.Offset == 0, "Offset was not special number 0");
        }
    }
}
