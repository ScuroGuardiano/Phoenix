using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ScuroGuardiano.Phoenix
{
    /// <summary>
    /// Ways to kill yourself and spawn new process in your place
    /// </summary>
    /// <remarks>
    /// <i>The wings of wonder</i><br/>
    /// <i>A new life arising</i><br/>
    /// <i>Sacrificial arson</i><br/>
    /// <i>From fire to carbon</i><br/>
    /// <i>The ashes crowning</i><br/>
    /// <i>A call to weakness</i><br/>
    /// <i>Escape of the phoenix</i><br/>
    /// <br/>
    /// Evergrey - Escape of the Phoenix
    /// </remarks>
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("freebsd")]
    [SupportedOSPlatform("macos")]
    public class SacrificialArson
    {
        internal SacrificialArson() { }

        public static SacrificialArson Instance { get; } = new();

        /// <summary>
        /// <para>
        /// Basically restarts process.
        /// </para>
        /// <para>
        /// .NET provides as `Environment.GetCommandLineArgs()[0]` dll with managed code instead of actual used executable.
        /// This method will use `dotnet Environment.GetCommandLineArgs()[0] ..args` to pass to evecve
        /// </para>
        /// </summary>
        [DoesNotReturn]
        public void SelfExecve(bool withDotnetCommandIfDll, string dotnetCommand = "/usr/bin/dotnet")
        {
            if (!withDotnetCommandIfDll)
            {
                SelfExecve();
                return;
            }

            var commandName = Environment.GetCommandLineArgs()[0];
            int argsStart = 1;

            // Fucking dotnet gives .dll name instead of binary name. I just strip .dll extension and should work.
            if (commandName.EndsWith(".dll"))
            {
                commandName = dotnetCommand;
                argsStart = 0;
            }

            var args = Environment.GetCommandLineArgs().AsSpan(argsStart);
            Execve(commandName, args);
        }
        /// <summary>
        /// <para>
        /// Performs evecve with this process command, arguments and environment variables.
        /// Basically restarts process.
        /// </para>
        /// <para>
        /// If process command ends with `.dll` then it will be striped, because dotnet provides dll as command name instead of real executable:
        /// <code>
        /// /MyProject.dll -> /MyProject
        /// </code>
        /// </para>
        /// </summary>
        [DoesNotReturn]
        public void SelfExecve()
        {
            var commandName = Environment.GetCommandLineArgs()[0];

            // Fucking dotnet gives .dll name instead of binary name. I just strip .dll extension and should work.
            if (commandName.EndsWith(".dll"))
            {
                commandName = commandName.Substring(0, commandName.Length - 4);
            }
            var args = Environment.GetCommandLineArgs().AsSpan(1);
            Execve(commandName, args);
        }

        /// <summary>
        /// <para>
        /// Performs execve syscall for given command with provided arguments.
        /// <b>Important! args should not contain command name in [0] position.</b>
        /// </para>
        /// <para>
        /// This method passes environment variables from current process
        /// </para>
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="args">Arguments to that command</param>
        [DoesNotReturn]
        public void Execve(string command, ReadOnlySpan<string> args)
        {
            Execve(command, args, Environment.GetEnvironmentVariables());
        }

        /// <summary>
        /// <para>
        /// Performs execve syscall for given command with provided arguments and environment variables.
        /// <b>Important! args should not contain command name in [0] position.</b>
        /// </para>
        /// <para>
        /// This method passes environment variables from current process
        /// </para>
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="args">Arguments to that command</param>
        /// <param name="environmentVariables">Dictionary with environment variables to pass to new process</param>
        [DoesNotReturn]
        public virtual void Execve(string command, ReadOnlySpan<string> args, IDictionary environmentVariables)
        {
            string[] argv = new string[args.Length + 2];
            argv[0] = command;
            args.CopyTo(argv.AsSpan(1));
            argv[^1] = null;

            string[] envp = environmentVariables
                .Cast<DictionaryEntry>()
                .Select(e => $"{e.Key}={e.Value}")
                .Append(null)
                .ToArray();

            NativeInterop.Execve(command, argv, envp);
            throw new Exception($"execve() failed: {Marshal.GetLastPInvokeError()} {Marshal.GetLastPInvokeErrorMessage()}");
        }

        /// <summary>
        /// Calls prctl with provided signal before execve
        /// </summary>
        /// <param name="sig"></param>
        /// <returns></returns>
        [SupportedOSPlatform("linux")]
        public static SacrificialArsonWithSigOnParentDeath WithSigOnParentDeath(int sig)
        {
            return new SacrificialArsonWithSigOnParentDeath(sig);
        }

        /// <summary>
        /// Calls prctl with <see cref="LinuxProcessSignals.SIGHUP"/> signal
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("linux")]
        public static SacrificialArsonWithSigOnParentDeath WithSigHupOnParentDeath()
        {
            return WithSigOnParentDeath(LinuxProcessSignals.SIGHUP);
        }
    }

    [SupportedOSPlatform("linux")]
    public class SacrificialArsonWithSigOnParentDeath : SacrificialArson
    {
        internal SacrificialArsonWithSigOnParentDeath(int sig)
        {
            _sig = sig;
        }
        // ReSharper disable InconsistentNaming
        private const int PR_SET_PDEATHSIG = 1;
        // ReSharper restore InconsistentNaming

        private readonly int _sig;

        /// <inheritdoc/>
        public override void Execve(string command, ReadOnlySpan<string> args, IDictionary environmentVariables)
        {
            NativeInterop.PrCtl(PR_SET_PDEATHSIG, new CLong(_sig));
            base.Execve(command, args, environmentVariables);
        }
    }
}
