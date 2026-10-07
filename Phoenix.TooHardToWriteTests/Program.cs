// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using ScuroGuardiano.Phoenix;

Console.WriteLine($"Started process {Process.GetCurrentProcess().Id}");
Console.WriteLine("Performing SelfExecve after 3 seconds");
Thread.Sleep(3000);

// SacrificialArson.Instance.SelfExecve(withDotnetCommandIfDll: true);
SacrificialArson.Instance.SelfExecve();