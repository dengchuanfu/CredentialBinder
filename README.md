# 共享盘凭据绑定工具

适用于 Windows 的轻量工具。填写共享地址、账号和密码后，工具会将凭据写入当前 Windows 用户的凭据管理器，方便后续访问共享盘。

当前版本：`0.0.1`。本工具无需安装，双击即可运行。

## 功能

- 支持输入服务器名、IP 地址、UNC 共享路径或 SMB 地址。
- 同一服务器已有凭据时，直接用新输入的账号和密码覆盖。
- 绑定后自动请求管理员授权并重启 `Workstation` 服务，清除现有 SMB 会话，使后续共享访问使用新凭据。
- 密码不会保存到工具的配置文件中，仅写入 Windows 凭据管理器。

## 使用方法

1. 双击运行 `CredentialBinder.exe`。
2. 输入共享地址、账号和密码。
3. 点击“绑定凭据”，并在 Windows 弹出的管理员授权窗口中确认。
4. 等待 `Workstation` 服务重启完成后，再访问共享盘。

支持的地址形式：

- `\\fileserver\share`
- `fileserver`
- `192.168.1.10`
- `smb://fileserver/share`

> 重启 `Workstation` 服务会临时断开当前所有共享盘连接，请先保存正在使用的网络文件。

## 下载

从 [GitHub Releases](https://github.com/dengchuanfu/CredentialBinder/releases) 下载最新的 `CredentialBinder.exe`，双击即可运行。

## 构建

在项目目录执行：

```powershell
& 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe' CredentialBinder.csproj /p:Configuration=Release
```

生成的可执行文件位于 `bin\Release\CredentialBinder.exe`。
