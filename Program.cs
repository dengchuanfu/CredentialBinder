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
        private readonly ComboBox shareLocationComboBox = new ComboBox();
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
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(460, 382);
            BackColor = Color.White;

            var title = new Label
            {
                AutoSize = true,
                Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 71, 120),
                Location = new Point(28, 23),
                Text = "\u516c\u53f8\u5171\u4eab\u76d8\u51ed\u636e\u7ed1\u5b9a"
            };
            var hint = new Label
            {
                AutoSize = true,
                ForeColor = Color.FromArgb(90, 90, 90),
                Location = new Point(30, 58),
                Text = "\u9009\u62e9\u5171\u4eab\u76d8\u540e\u8f93\u5165\u60a8\u7684\u8d26\u53f7\u548c\u5bc6\u7801\u3002"
            };

            ConfigureField(shareLocationComboBox, 116);
            shareLocationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            shareLocationComboBox.Items.Add(new ShareLocation("\u6df1\u5733\u5171\u4eab\u76d8", "192.168.10.200"));
            shareLocationComboBox.Items.Add(new ShareLocation("\u5e7f\u897f\u5171\u4eab\u76d8", "172.16.1.166"));
            shareLocationComboBox.SelectedIndex = 0;

            ConfigureField(userNameTextBox, 156);
            ConfigureField(passwordTextBox, 196);
            passwordTextBox.UseSystemPasswordChar = true;

            AddFieldLabel("\u5171\u4eab\u76d8", 119);
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
            statusLabel.Location = new Point(28, 351);
            statusLabel.Size = new Size(410, 24);
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;

            var shortcutTitle = new Label
            {
                AutoSize = true,
                ForeColor = Color.FromArgb(74, 74, 74),
                Location = new Point(28, 296),
                Text = "\u684c\u9762\u5feb\u6377\u65b9\u5f0f"
            };

            Button shenzhenShareButton = CreateShortcutButton(
                "\u6df1\u5733\u5171\u4eab\u76d8\u5feb\u6377\u65b9\u5f0f",
                28,
                "\u6df1\u5733\u5171\u4eab\u76d8",
                "\\\\192.168.10.200\\\u5c91\u79d1\u79d1\u6280\\\u6df1\u5733\u5404\u90e8\u95e8\u6587\u4ef6\u5171\u4eab");
            Button guangxiShareButton = CreateShortcutButton(
                "\u5e7f\u897f\u5171\u4eab\u76d8\u5feb\u6377\u65b9\u5f0f",
                166,
                "\u5e7f\u897f\u5171\u4eab\u76d8",
                "\\\\172.16.1.166");
            Button shenzhenScanButton = CreateShortcutButton(
                "\u6df1\u5733\u626b\u63cf\u5feb\u6377\u65b9\u5f0f",
                304,
                "\u6df1\u5733\u626b\u63cf",
                "\\\\192.168.10.200\\\u5c91\u79d1\u79d1\u6280\\\u6df1\u5733\u5404\u90e8\u95e8\u6587\u4ef6\u5171\u4eab\\\u626b\u63cf");

            AcceptButton = bindButton;
            Controls.AddRange(new Control[]
            {
                title, hint, shareLocationComboBox, userNameTextBox, passwordTextBox,
                showPassword, bindButton, shortcutTitle, shenzhenShareButton,
                guangxiShareButton, shenzhenScanButton, statusLabel
            });
        }

        private Button CreateShortcutButton(string text, int left, string shortcutName, string targetPath)
        {
            var button = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(left, 315),
                Size = new Size(124, 28),
                Text = text,
                UseVisualStyleBackColor = true
            };
            button.Click += delegate
            {
                try
                {
                    DesktopShortcut.Create(shortcutName, targetPath);
                    statusLabel.ForeColor = Color.FromArgb(31, 112, 66);
                    statusLabel.Text = shortcutName + "\u5feb\u6377\u65b9\u5f0f\u5df2\u521b\u5efa\u3002";
                }
                catch (Exception exception)
                {
                    ShowError("\u521b\u5efa\u5feb\u6377\u65b9\u5f0f\u5931\u8d25\uff1a" + exception.Message);
                }
            };
            return button;
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

        private static void ConfigureField(Control control, int top)
        {
            control.Location = new Point(116, top - 3);
            control.Size = new Size(322, 25);
            control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void BindButton_Click(object sender, EventArgs e)
        {
            var selectedLocation = shareLocationComboBox.SelectedItem as ShareLocation;
            if (selectedLocation == null)
            {
                ShowError("\u8bf7\u9009\u62e9\u5171\u4eab\u76d8\u3002");
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
                WindowsCredentialStore.SaveNetworkCredential(
                    selectedLocation.Server,
                    userNameTextBox.Text.Trim(),
                    passwordTextBox.Text);
                credentialSaved = true;
                passwordTextBox.Clear();
                statusLabel.ForeColor = Color.FromArgb(74, 74, 74);
                statusLabel.Text = "\u6b63\u5728\u91cd\u542f Workstation \u670d\u52a1...";
                Application.DoEvents();
                WorkstationService.RestartAndWait();
                statusLabel.ForeColor = Color.FromArgb(31, 112, 66);
                statusLabel.Text = selectedLocation.Name + "\u51ed\u636e\u5df2\u7ed1\u5b9a\uff0c\u5171\u4eab\u8fde\u63a5\u5df2\u5237\u65b0\u3002";
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

    internal sealed class ShareLocation
    {
        internal ShareLocation(string name, string server)
        {
            Name = name;
            Server = server;
        }

        internal string Name { get; private set; }

        internal string Server { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }

    internal static class DesktopShortcut
    {
        internal static void Create(string shortcutName, string targetPath)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string shortcutPath = System.IO.Path.Combine(desktopPath, shortcutName + ".lnk");
            Type shellType = Type.GetTypeFromProgID("WScript.Shell");
            if (shellType == null)
            {
                throw new InvalidOperationException("Windows \u5feb\u6377\u65b9\u5f0f\u7ec4\u4ef6\u4e0d\u53ef\u7528\u3002");
            }

            object shell = null;
            object shortcut = null;
            try
            {
                shell = Activator.CreateInstance(shellType);
                shortcut = shellType.InvokeMember(
                    "CreateShortcut",
                    System.Reflection.BindingFlags.InvokeMethod,
                    null,
                    shell,
                    new object[] { shortcutPath });
                Type shortcutType = shortcut.GetType();
                shortcutType.InvokeMember(
                    "TargetPath",
                    System.Reflection.BindingFlags.SetProperty,
                    null,
                    shortcut,
                    new object[] { targetPath });
                shortcutType.InvokeMember(
                    "Save",
                    System.Reflection.BindingFlags.InvokeMethod,
                    null,
                    shortcut,
                    null);
            }
            finally
            {
                ReleaseComObject(shortcut);
                ReleaseComObject(shell);
            }
        }

        private static void ReleaseComObject(object value)
        {
            if (value != null && Marshal.IsComObject(value))
            {
                Marshal.FinalReleaseComObject(value);
            }
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
