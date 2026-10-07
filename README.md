# `ScuroGuardiano.Phoenix`
A thin wrapper on `execve` on Unix-like systems and `prctl` on Linux.
Windows is not supported, because we can't have nice syscalls on Windows.

## Usage
### Replace current process with new
```csharp
SacrificialArson.Instance.Execve("/bin/bash", []);
```

### Restart current process
```csharp
SacrificialArson.Instance.SelfExecve();
```

This has some caveats, it seems that .NET provide `.dll` as `Environment.GetCommandLineArgs()[0]`
instead of actual executable file. This leads to `EACCES` error when `execve` is performed on it.

As a workaround I just strip `.dll` and execute that:
```csharp
public void SelfExecve()
{
    var commandName = Environment.GetCommandLineArgs()[0];

    if (commandName.EndsWith(".dll"))
    {
        commandName = commandName.Substring(0, commandName.Length - 4);
    }
    var args = Environment.GetCommandLineArgs().AsSpan(1);
    Execve(commandName, args);
}
```
If however it is not intended behavior, you can use overload of this method:
```csharp
public void SelfExecve(bool withDotnetCommandIfDll, string dotnetCommand = "/usr/bin/dotnet")
```

like so:
```csharp
SacrificialArson.Instance.SelfExecve(withDotnetCommandIfDll: true);
```

If you don't like those methods, just use `.Execve` with command and args provided by you, for example:
```csharp
SacrificialArson.Instance.Execve("/usr/bin/env", ["dotnet", ..Environment.GetCommandLineArgs()]);
```

Please be aware that `/usr/bin/env` may not be available in your environment.

# LICENSE
MIT