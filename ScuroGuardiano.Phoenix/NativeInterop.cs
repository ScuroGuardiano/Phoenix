using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ScuroGuardiano.Phoenix
{
    internal static partial class NativeInterop
    {
        [SupportedOSPlatform("linux")]
        [LibraryImport("libc", EntryPoint = "prctl", SetLastError = true)]
        public static partial int PrCtl(int op, CLong sig);

        [SupportedOSPlatform("linux")]
        [SupportedOSPlatform("freebsd")]
        [SupportedOSPlatform("macos")]
        [LibraryImport("libc", EntryPoint = "execve", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        public static partial int Execve(string path, ReadOnlySpan<string> argv, ReadOnlySpan<string> envp);
    }
}
