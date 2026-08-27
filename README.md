# 共享盘凭据绑定工具

适用于 Windows 的轻量工具。填写共享地址、账号和密码后，工具会将凭据写入当前 Windows 用户的凭据管理器，方便后续访问共享盘。

当前版本：`0.0.3`。本工具无需安装，双击即可运行。

## 功能

- 内置“深圳共享盘”和“广西共享盘”两个地址选项，无需员工手动输入服务器地址。
- 深圳共享盘使用 `192.168.10.200`，广西共享盘使用 `172.16.1.166`。
- 同一共享盘已有凭据时，直接用新输入的账号和密码覆盖。
- 绑定后自动请求管理员授权并重启 `Workstation` 服务，清除现有 SMB 会话，使后续共享访问使用新凭据。
- 密码不会保存到工具的配置文件中，仅写入 Windows 凭据管理器。
- 可在桌面创建深圳共享盘、广西共享盘和深圳扫描目录的快捷方式。

桌面快捷方式对应的目标地址：

- 深圳共享盘：`\\192.168.10.200\岑科科技\深圳各部门文件共享`
- 广西共享盘：`\\172.16.1.166`
- 深圳扫描：`\\192.168.10.200\岑科科技\深圳各部门文件共享\扫描`

## 使用方法

1. 双击运行 `CredentialBinder.exe`。
2. 选择共享盘，输入账号和密码。
3. 点击“绑定凭据”，并在 Windows 弹出的管理员授权窗口中确认。
4. 等待 `Workstation` 服务重启完成后，再访问共享盘。

> 重启 `Workstation` 服务会临时断开当前所有共享盘连接，请先保存正在使用的网络文件。

## 下载

从 [GitHub Releases](https://github.com/dengchuanfu/CredentialBinder/releases) 下载最新的 `CredentialBinder.exe`，双击即可运行。

## 构建

在项目目录执行：

```powershell
& 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe' CredentialBinder.csproj /p:Configuration=Release
```

生成的可执行文件位于 `bin\Release\CredentialBinder.exe`。
