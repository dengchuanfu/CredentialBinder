using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CredentialBinder
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CredentialBinderForm());
        }
    }

    internal sealed class CredentialBinderForm : Form
    {
        private readonly TextBox addressTextBox = new TextBox();
        private readonly TextBox userNameTextBox = new TextBox();
        private readonly TextBox passwordTextBox = new TextBox();
        private readonly Button bindButton = new Button();
        private readonly Label statusLabel = new Label();

        public CredentialBinderForm()
        {
            Icon applicationIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            if (applicationIcon != null)
            {
                Icon = applicationIcon;
            }

            Text = "\u5171\u4eab\u76d8\u51ed\u636e\u7ed1\u5b9a";
            Font = new Font("Microsoft YaHei UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = true;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(460, 292);
            BackColor = Color.White;

            var title = new Label
            {
                AutoSize = true,
                Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 71, 120),
                Location = new Point(28, 23),
                Text = "\u7ed1\u5b9a\u5171\u4eab\u76d8\u51ed\u636e"
            };
            var hint = new Label
            {
                AutoSize = true,
                ForeColor = Color.FromArgb(90, 90, 90),
                Location = new Point(30, 58),
                Text = "\u51ed\u636e\u4f1a\u4fdd\u5b58\u5230 Windows \u51ed\u636e\u7ba1\u7406\u5668\u5e76\u5237\u65b0\u5171\u4eab\u8fde\u63a5\u3002"
            };

            ConfigureField(addressTextBox, 116);
            ConfigureField(userNameTextBox, 156);
            ConfigureField(passwordTextBox, 196);
            passwordTextBox.UseSystemPasswordChar = true;

            AddFieldLabel("\u5171\u4eab\u5730\u5740", 119);
            AddFieldLabel("\u8d26\u53f7", 159);
            AddFieldLabel("\u5bc6\u7801", 199);

            var showPassword = new CheckBox
            {
                AutoSize = true,
                Location = new Point(348, 220),
                Text = "\u663e\u793a\u5bc6\u7801"
            };
            showPassword.CheckedChanged += delegate { passwordTextBox.UseSystemPasswordChar = !showPassword.Checked; };

            bindButton.BackColor = Color.FromArgb(28, 99, 171);
            bindButton.FlatAppearance.BorderSize = 0;
            bindButton.FlatStyle = FlatStyle.Flat;
            bindButton.ForeColor = Color.White;
            bindButton.Location = new Point(28, 244);
            bindButton.Size = new Size(124, 34);
            bindButton.Text = "\u7ed1\u5b9a\u51ed\u636e";
            bindButton.UseVisualStyleBackColor = false;
            bindButton.Click += BindButton_Click;

            statusLabel.AutoEllipsis = true;
            statusLabel.ForeColor = Color.FromArgb(74, 74, 74);
            statusLabel.Location = new Point(166, 251);
            statusLabel.Size = new Size(272, 24);
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;

            AcceptButton = bindButton;
            Controls.AddRange(new Control[]
            {
                title, hint, addressTextBox, userNameTextBox, passwordTextBox,
                showPassword, bindButton, statusLabel
            });
        }

        private void AddFieldLabel(string text, int top)
        {
            Controls.Add(new Label
            {
                AutoSize = true,
                Location = new Point(28, top),
                Text = text
            });
        }

        private static void ConfigureField(TextBox textBox, int top)
        {
            textBox.Location = new Point(116, top - 3);
            textBox.Size = new Size(322, 25);
            textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void BindButton_Click(object sender, EventArgs e)
        {
            string server;
            try
            {
                server = NetworkAddress.GetServerName(addressTextBox.Text);
            }
            catch (ArgumentException exception)
            {
                ShowError(exception.Message);
                return;
            }

            if (string.IsNullOrWhiteSpace(userNameTextBox.Text))
            {
                ShowError("\u8bf7\u8f93\u5165\u8d26\u53f7\u3002");
                userNameTextBox.Focus();
                return;
            }

            if (passwordTextBox.Text.Length == 0)
            {
                ShowError("\u8bf7\u8f93\u5165\u5bc6\u7801\u3002");
                passwordTextBox.Focus();
                return;
            }

            bool credentialSaved = false;
            try
            {
                bindButton.Enabled = false;
                WindowsCredentialStore.SaveNetworkCredential(server, userNameTextBox.Text.Trim(), passwordTextBox.Text);
                credentialSaved = true;
                passwordTextBox.Clear();
                // 清除现有 SMB 会话，确保下一次访问使用刚写入的新凭据。
                statusLabel.ForeColor = Color.FromArgb(74, 74, 74);
                statusLabel.Text = "\u6b63\u5728\u91cd\u542f Workstation \u670d\u52a1...";
                Application.DoEvents();
                WorkstationService.RestartAndWait();
                statusLabel.ForeColor = Color.FromArgb(31, 112, 66);
                statusLabel.Text = "\u5df2\u7ed1\u5b9a\u5230 " + server + "\uff0c\u5171\u4eab\u8fde\u63a5\u5df2\u5237\u65b0\u3002";
            }
            catch (Win32Exception exception)
            {
                ShowError((credentialSaved ? "\u51ed\u636e\u5df2\u7ed1\u5b9a\uff0c\u4f46\u670d\u52a1\u91cd\u542f\u5931\u8d25\uff1a" : "\u7ed1\u5b9a\u5931\u8d25\uff1a") + exception.Message);
            }
            catch (Exception exception)
            {
                ShowError((credentialSaved ? "\u51ed\u636e\u5df2\u7ed1\u5b9a\uff0c\u4f46\u670d\u52a1\u91cd\u542f\u5931\u8d25\uff1a" : "\u7ed1\u5b9a\u5931\u8d25\uff1a") + exception.Message);
            }
            finally
            {
                bindButton.Enabled = true;
            }
        }

        private void ShowError(string message)
        {
            statusLabel.ForeColor = Color.FromArgb(180, 38, 38);
            statusLabel.Text = message;
        }
    }

    internal static class WorkstationService
    {
        internal static void RestartAndWait()
        {
            const string command = "$ErrorActionPreference='Stop'; $service=Get-Service -Name 'LanmanWorkstation'; if ($service.Status -ne 'Stopped') { Stop-Service -InputObject $service -Force; $service.WaitForStatus([System.ServiceProcess.ServiceControllerStatus]::Stopped,[TimeSpan]::FromSeconds(30)); $service.Refresh() }; Start-Service -InputObject $service; $service.WaitForStatus([System.ServiceProcess.ServiceControllerStatus]::Running,[TimeSpan]::FromSeconds(30))";
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-NoProfile -NonInteractive -WindowStyle Hidden -Command \"" + command + "\"",
                UseShellExecute = true,
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (Process process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    throw new InvalidOperationException("\u65e0\u6cd5\u542f\u52a8 Workstation \u670d\u52a1\u91cd\u542f\u64cd\u4f5c\u3002");
                }

                if (!process.WaitForExit(45000))
                {
                    throw new TimeoutException("Workstation \u670d\u52a1\u91cd\u542f\u8d85\u65f6\u3002");
                }

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException("Workstation \u670d\u52a1\u91cd\u542f\u5931\u8d25\u3002");
                }
            }
        }
    }

    #if false
    internal sealed class CredentialVerificationException : Exception
    {
        internal CredentialVerificationException(string message) : base(message)
        {
        }
    }

    internal static class NetworkShareVerifier
    {
        private const uint ResourceConnected = 1;
        private const uint ResourceTypeDisk = 1;
        private const uint ConnectTemporary = 4;
        private const int ErrorSessionCredentialConflict = 1219;
        private const int ErrorMoreData = 234;
        private const int ErrorNoMoreItems = 259;

        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        private static extern int WNetAddConnection2(
            [In] ref NetResource netResource,
            string password,
            string userName,
            uint flags);

        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        private static extern int WNetOpenEnum(
            uint scope,
            uint type,
            uint usage,
            IntPtr netResource,
            out IntPtr enumHandle);

        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        private static extern int WNetEnumResource(
            IntPtr enumHandle,
            ref uint count,
            IntPtr buffer,
            ref uint bufferSize);

        [DllImport("mpr.dll")]
        private static extern int WNetCloseEnum(IntPtr enumHandle);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NetResource
        {
            public uint Scope;
            public uint Type;
            public uint DisplayType;
            public uint Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        internal static void VerifyShareAccess(string sharePath, string userName, string password)
        {
            var resource = new NetResource
            {
                Type = ResourceTypeDisk,
                RemoteName = sharePath
            };
            int result = WNetAddConnection2(ref resource, password, userName, ConnectTemporary);
            if (result == ErrorSessionCredentialConflict)
            {
                throw new CredentialVerificationException("\u670d\u52a1\u5668\u5df2\u5b58\u5728\u4f7f\u7528\u5176\u4ed6\u8d26\u53f7\u7684 SMB \u8fde\u63a5\uff0c\u8bf7\u52fe\u9009\u201c\u91cd\u542f Workstation \u540e\u9a8c\u8bc1\u201d\u540e\u91cd\u8bd5\u3002");
            }

            if (result != 0)
            {
                throw new Win32Exception(result);
            }
        }

        internal static bool HasExistingConnectionToServer(string server)
        {
            IntPtr enumHandle;
            int openResult = WNetOpenEnum(ResourceConnected, ResourceTypeDisk, 0, IntPtr.Zero, out enumHandle);
            if (openResult != 0)
            {
                throw new Win32Exception(openResult);
            }

            try
            {
                uint bufferSize = 16384;
                while (true)
                {
                    IntPtr buffer = Marshal.AllocHGlobal((int)bufferSize);
                    try
                    {
                        uint count = UInt32.MaxValue;
                        int result = WNetEnumResource(enumHandle, ref count, buffer, ref bufferSize);
                        if (result == ErrorNoMoreItems)
                        {
                            return false;
                        }

                        if (result == ErrorMoreData)
                        {
                            continue;
                        }

                        if (result != 0)
                        {
                            throw new Win32Exception(result);
                        }

                        int itemSize = Marshal.SizeOf(typeof(NetResource));
                        for (int index = 0; index < count; index++)
                        {
                            IntPtr itemPointer = IntPtr.Add(buffer, index * itemSize);
                            var resource = (NetResource)Marshal.PtrToStructure(itemPointer, typeof(NetResource));
                            if (IsConnectionToServer(resource.RemoteName, server))
                            {
                                return true;
                            }
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(buffer);
                    }
                }
            }
            finally
            {
                WNetCloseEnum(enumHandle);
            }
        }

        private static bool IsConnectionToServer(string remoteName, string server)
        {
            if (string.IsNullOrEmpty(remoteName))
            {
                return false;
            }

            string value = remoteName.TrimStart('\\', '/');
            int separator = value.IndexOfAny(new[] { '\\', '/' });
            string remoteServer = separator < 0 ? value : value.Substring(0, separator);
            return string.Equals(remoteServer, server, StringComparison.OrdinalIgnoreCase);
        }
    }

    #endif

    internal static class NetworkAddress
    {
        internal static string GetServerName(string address)
        {
            string value = Normalize(address);
            int separator = value.IndexOfAny(new[] { '\\', '/' });
            string server = separator < 0 ? value : value.Substring(0, separator);
            ValidateServer(server);
            return server;
        }

        private static string Normalize(string address)
        {
            string value = (address ?? string.Empty).Trim();
            if (value.StartsWith("smb://", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(6);
            }

            return value.TrimStart('\\', '/');
        }

        private static void ValidateServer(string server)
        {
            if (server.Length == 0 || server.IndexOfAny(new[] { ':', '*', '?', '\"', '<', '>', '|' }) >= 0)
            {
                throw new ArgumentException("\u8bf7\u8f93\u5165\u6709\u6548\u7684\u670d\u52a1\u5668\u5730\u5740\u3002");
            }
        }
    }

    internal static class WindowsCredentialStore
    {
        private const uint CredTypeDomainPassword = 2;
        private const uint CredPersistLocalMachine = 2;

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CredWrite([In] ref Credential userCredential, uint flags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct Credential
        {
            public uint Flags;
            public uint Type;
            public string TargetName;
            public string Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public uint CredentialBlobSize;
            public IntPtr CredentialBlob;
            public uint Persist;
            public uint AttributeCount;
            public IntPtr Attributes;
            public string TargetAlias;
            public string UserName;
        }

        internal static void SaveNetworkCredential(string server, string userName, string password)
        {
            // 密码仅在调用 Windows 凭据 API 所需的非托管缓冲区中短暂存在。
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            IntPtr passwordPointer = Marshal.AllocCoTaskMem(passwordBytes.Length);

            try
            {
                Marshal.Copy(passwordBytes, 0, passwordPointer, passwordBytes.Length);
                var credential = new Credential
                {
                    Type = CredTypeDomainPassword,
                    TargetName = server,
                    CredentialBlobSize = (uint)passwordBytes.Length,
                    CredentialBlob = passwordPointer,
                    Persist = CredPersistLocalMachine,
                    UserName = userName
                };

                if (!CredWrite(ref credential, 0))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            finally
            {
                Array.Clear(passwordBytes, 0, passwordBytes.Length);
                if (passwordPointer != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(passwordPointer);
                }
            }
        }
    }
}
