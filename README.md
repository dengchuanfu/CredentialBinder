# CredentialBinder

Windows shared-folder credential binding tool. Enter a server address, user name, and password, then select "Bind credential". The tool writes a Windows domain-password credential for that server to the current user's Credential Manager.

## Output

Run the following from the project directory to produce a small, single executable:

```powershell
& 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe' CredentialBinder.csproj /p:Configuration=Release
```

The executable is at `bin\Release\CredentialBinder.exe`. It targets .NET Framework 4.0 and runs on current Windows 10 and Windows 11 installations, which include a compatible .NET Framework runtime.

## Workstation service

Each bind replaces any existing Windows credential for the same server, then automatically requests administrator approval to restart the Workstation service. This temporarily disconnects all current shared-folder connections so future access uses the new credential.

## Address input

These address forms are supported:

- `\\fileserver\share`
- `fileserver`
- `192.168.1.10`
- `smb://fileserver/share`

The share portion is intentionally not stored: Windows network credentials apply per server, so `\\fileserver\team` and `\\fileserver\finance` use the same saved credential.
