using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using TXTconverter.App.Commands;
using TXTconverter.App.Constants;
using TXTconverter.App.Helpers;
using TXTconverter.App.Interfaces;
using TXTconverter.App.Models;
using TXTconverter.App.Services;
using TXTconverter.App.Utilities;
using TXTconverter.App.ViewModels;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: AssemblyTitle("TXTconverter")]
[assembly: AssemblyDescription("TXTconverter PDF Utility Application")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("TXTconverter")]
[assembly: AssemblyCopyright("Copyright © 2026 TXTconverter")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: ThemeInfo(/*Could not decode attribute arguments.*/)]
[assembly: AssemblyFileVersion("1.1.0.6")]
[assembly: TargetFramework(".NETFramework,Version=v4.6", FrameworkDisplayName = ".NET Framework 4.6")]
[assembly: AssemblyVersion("1.1.0.6")]
namespace TXTconverter.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					resourceMan = new ResourceManager("TXTconverter.Properties.Resources", typeof(Resources).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal Resources()
		{
		}
	}
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)(object)SettingsBase.Synchronized((SettingsBase)(object)new Settings());

		public static Settings Default => defaultInstance;
	}
}
namespace TXTconverter.App
{
	public class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			((Application)this).OnStartup(e);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			((Application)this).StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
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
	public class MainWindow : Window, IComponentConnector
	{
		internal Button CloseButton;

		private bool _contentLoaded;

		public MainWindow()
		{
			InitializeComponent();
			((FrameworkElement)this).DataContext = new MainViewModel();
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			((Window)this).Close();
		}

		private void HeaderBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)e.ButtonState == 1)
			{
				((Window)this).DragMove();
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri uri = new Uri("/TXTconverter;component/mainwindow.xaml", UriKind.Relative);
				Application.LoadComponent((object)this, uri);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			switch (connectionId)
			{
			case 1:
				((UIElement)(Border)target).MouseLeftButtonDown += new MouseButtonEventHandler(HeaderBar_MouseLeftButtonDown);
				break;
			case 2:
				CloseButton = (Button)target;
				((ButtonBase)CloseButton).Click += new RoutedEventHandler(CloseButton_Click);
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
namespace TXTconverter.App.Views
{
	public class ConvertView : UserControl, IComponentConnector
	{
		private bool _contentLoaded;

		public ConvertView()
		{
			InitializeComponent();
		}

		private void OnDragOver(object sender, DragEventArgs e)
		{
			e.Effects = (DragDropEffects)(DragDropHelper.HasFiles(e) ? 1 : 0);
			((RoutedEventArgs)e).Handled = true;
		}

		private void OnDrop(object sender, DragEventArgs e)
		{
			string[] droppedFiles = DragDropHelper.GetDroppedFiles(e);
			if (droppedFiles != null && ((FrameworkElement)this).DataContext is ConvertViewModel convertViewModel)
			{
				convertViewModel.AddDroppedFiles(droppedFiles);
			}
		}

		private void OnBrowseClick(object sender, MouseButtonEventArgs e)
		{
			if (((FrameworkElement)this).DataContext is ConvertViewModel convertViewModel && convertViewModel.BrowseCommand.CanExecute(null))
			{
				convertViewModel.BrowseCommand.Execute(null);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri uri = new Uri("/TXTconverter;component/views/convertview.xaml", UriKind.Relative);
				Application.LoadComponent((object)this, uri);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			switch (connectionId)
			{
			case 1:
				((UIElement)(ConvertView)target).Drop += new DragEventHandler(OnDrop);
				((UIElement)(ConvertView)target).DragOver += new DragEventHandler(OnDragOver);
				break;
			case 2:
				((UIElement)(Border)target).MouseLeftButtonUp += new MouseButtonEventHandler(OnBrowseClick);
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
	public class MergeView : UserControl, IComponentConnector
	{
		private bool _contentLoaded;

		public MergeView()
		{
			InitializeComponent();
		}

		private void OnDragOver(object sender, DragEventArgs e)
		{
			e.Effects = (DragDropEffects)(DragDropHelper.HasFiles(e) ? 1 : 0);
			((RoutedEventArgs)e).Handled = true;
		}

		private void OnDrop(object sender, DragEventArgs e)
		{
			string[] droppedFiles = DragDropHelper.GetDroppedFiles(e);
			if (droppedFiles != null && ((FrameworkElement)this).DataContext is MergeViewModel mergeViewModel)
			{
				mergeViewModel.AddDroppedFiles(droppedFiles);
			}
		}

		private void OnBrowseClick(object sender, MouseButtonEventArgs e)
		{
			if (((FrameworkElement)this).DataContext is MergeViewModel mergeViewModel && mergeViewModel.BrowseCommand.CanExecute(null))
			{
				mergeViewModel.BrowseCommand.Execute(null);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri uri = new Uri("/TXTconverter;component/views/mergeview.xaml", UriKind.Relative);
				Application.LoadComponent((object)this, uri);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			switch (connectionId)
			{
			case 1:
				((UIElement)(MergeView)target).Drop += new DragEventHandler(OnDrop);
				((UIElement)(MergeView)target).DragOver += new DragEventHandler(OnDragOver);
				break;
			case 2:
				((UIElement)(Border)target).MouseLeftButtonUp += new MouseButtonEventHandler(OnBrowseClick);
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
namespace TXTconverter.App.Utilities
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
}
namespace TXTconverter.App.Services
{
	public class FileValidationService : IFileValidationService
	{
		public OperationResult Validate(IList<FileItem> files, PdfOperation operation)
		{
			if (files == null || files.Count == 0)
			{
				return OperationResult.Fail("No files selected.");
			}
			switch (operation)
			{
			case PdfOperation.ConvertToPdf:
				if (files.Count != 1)
				{
					return OperationResult.Fail("Please select exactly one file for conversion.");
				}
				if (!FileHelper.IsExtensionAllowed(files[0].FullPath, AppConstants.SupportedInputExtensions))
				{
					return OperationResult.Fail("Unsupported file type. Supported: " + string.Join(", ", AppConstants.SupportedInputExtensions));
				}
				break;
			case PdfOperation.MergePdf:
				if (files.Count < 2)
				{
					return OperationResult.Fail("Please select at least two PDF files to merge.");
				}
				foreach (FileItem file in files)
				{
					if (!FileHelper.IsExtensionAllowed(file.FullPath, AppConstants.PdfExtension))
					{
						return OperationResult.Fail("All files must be PDFs for merging. Invalid: " + file.FileName);
					}
				}
				break;
			case PdfOperation.CompressPdf:
				if (files.Count != 1)
				{
					return OperationResult.Fail("Please select exactly one PDF file for compression.");
				}
				if (!FileHelper.IsExtensionAllowed(files[0].FullPath, AppConstants.PdfExtension))
				{
					return OperationResult.Fail("Selected file must be a PDF.");
				}
				break;
			}
			return OperationResult.Ok("Validation passed.");
		}
	}
	public interface IPdfUtilsService
	{
		Task MergeAsync(List<string> inputPdfs, string outputPdf);

		Task ConvertToPdfAsync(string inputFile, string outputPdf);
	}
	public class PdfUtilsService : IPdfUtilsService
	{
		private readonly ITempWorkspaceService _tempWorkspace;

		public PdfUtilsService(ITempWorkspaceService tempWorkspace = null)
		{
			_tempWorkspace = tempWorkspace ?? new TempWorkspaceService();
		}

		public async Task MergeAsync(List<string> inputPdfs, string outputPdf)
		{
			if (inputPdfs == null || inputPdfs.Count == 0)
			{
				throw new ArgumentException("At least one PDF file is required for merging.");
			}
			await Task.Run(delegate
			{
				try
				{
					using PdfDocument pdfDocument = new PdfDocument();
					foreach (string inputPdf in inputPdfs)
					{
						using PdfDocument pdfDocument2 = PdfReader.Open(inputPdf, PdfDocumentOpenMode.Import);
						int pageCount = pdfDocument2.PageCount;
						for (int i = 0; i < pageCount; i++)
						{
							PdfPage page = pdfDocument2.Pages[i];
							pdfDocument.AddPage(page);
						}
					}
					pdfDocument.Save(outputPdf);
				}
				catch (Exception ex)
				{
					throw new IOException("Failed to merge PDFs: " + ex.Message, ex);
				}
			});
		}

		public async Task ConvertToPdfAsync(string inputFile, string outputPdf)
		{
			if (string.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
			{
				throw new FileNotFoundException("Input file not found.");
			}
			string text = Path.GetExtension(inputFile).ToLower();
			try
			{
				if (text != null)
				{
					int length = text.Length;
					if (length != 4)
					{
						if (length == 5)
						{
							char c = text[1];
							if (c != 'd')
							{
								if (c != 'j')
								{
									if (c == 'x' && text == ".xlsx")
									{
										goto IL_01ef;
									}
								}
								else if (text == ".jpeg")
								{
									goto IL_01d2;
								}
							}
							else if (text == ".docx")
							{
								goto IL_01db;
							}
						}
					}
					else
					{
						char c = text[1];
						if ((uint)c <= 106u)
						{
							if (c != 'd')
							{
								if (c == 'j' && text == ".jpg")
								{
									goto IL_01d2;
								}
							}
							else if (text == ".doc")
							{
								goto IL_01db;
							}
						}
						else if (c != 'p')
						{
							if (c == 'x' && text == ".xls")
							{
								goto IL_01ef;
							}
						}
						else if (text == ".png")
						{
							goto IL_01d2;
						}
					}
				}
				throw new NotSupportedException("File type '" + text + "' is not supported for conversion.");
				IL_01ef:
				RunInSta(delegate
				{
					ConvertExcelToPdf(inputFile, outputPdf);
				});
				return;
				IL_01db:
				RunInSta(delegate
				{
					ConvertWordToPdf(inputFile, outputPdf);
				});
				return;
				IL_01d2:
				await Task.Run(delegate
				{
					ConvertImageToPdf(inputFile, outputPdf);
				});
			}
			catch (Exception ex)
			{
				throw new IOException("Failed to convert file to PDF: " + ex.Message, ex);
			}
		}

		private void RunInSta(Action action)
		{
			if (action == null)
			{
				return;
			}
			Exception threadException = null;
			Thread thread = new Thread((ThreadStart)delegate
			{
				try
				{
					action();
				}
				catch (Exception ex)
				{
					threadException = ex;
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.IsBackground = true;
			thread.Start();
			thread.Join();
			if (threadException == null)
			{
				return;
			}
			throw threadException;
		}

		private void ConvertImageToPdf(string imagePath, string outputPdf)
		{
			try
			{
				using PdfDocument pdfDocument = new PdfDocument();
				PdfPage pdfPage = pdfDocument.AddPage();
				using (XImage xImage = XImage.FromFile(imagePath))
				{
					pdfPage.Width = xImage.PointWidth;
					pdfPage.Height = xImage.PointHeight;
					using XGraphics xGraphics = XGraphics.FromPdfPage(pdfPage);
					xGraphics.DrawImage(xImage, 0.0, 0.0, pdfPage.Width, pdfPage.Height);
				}
				pdfDocument.Save(outputPdf);
			}
			catch (Exception ex)
			{
				throw new IOException("Failed to convert image to PDF: " + ex.Message, ex);
			}
		}

		private void ConvertWordToPdf(string inputFile, string outputPdf)
		{
			dynamic val = null;
			try
			{
				string fullPath = Path.GetFullPath(inputFile);
				string fullPath2 = Path.GetFullPath(outputPdf);
				if (!File.Exists(fullPath))
				{
					throw new FileNotFoundException("Input file not found: " + fullPath);
				}
				Type? typeFromProgID = Type.GetTypeFromProgID("Word.Application");
				_ = typeFromProgID == null;
				val = Activator.CreateInstance(typeFromProgID);
				val.Visible = false;
				val.DisplayAlerts = false;
				Thread.Sleep(500);
				object obj = fullPath;
				object missing = Type.Missing;
				object obj2 = 17;
				dynamic val2 = val.Documents.Open(obj, missing, false, true);
				try
				{
					val2.ExportAsFixedFormat(fullPath2, obj2);
				}
				finally
				{
					val2.Close(false);
				}
			}
			catch (COMException ex)
			{
				throw new IOException($"Failed to convert Word document to PDF (COM Error: 0x{ex.ErrorCode:X8}): {ex.Message}", ex);
			}
			catch (Exception ex2)
			{
				throw new IOException("Failed to convert Word document to PDF: " + ex2.Message, ex2);
			}
			finally
			{
				if (val != null)
				{
					try
					{
						val.Quit(0);
						Marshal.ReleaseComObject(val);
					}
					catch
					{
					}
				}
			}
		}

		private void ConvertExcelToPdf(string inputFile, string outputPdf)
		{
			dynamic val = null;
			try
			{
				string fullPath = Path.GetFullPath(inputFile);
				string fullPath2 = Path.GetFullPath(outputPdf);
				if (!File.Exists(fullPath))
				{
					throw new FileNotFoundException("Input file not found: " + fullPath);
				}
				Type? typeFromProgID = Type.GetTypeFromProgID("Excel.Application");
				_ = typeFromProgID == null;
				val = Activator.CreateInstance(typeFromProgID);
				val.Visible = false;
				val.DisplayAlerts = false;
				Thread.Sleep(500);
				object obj = fullPath;
				object missing = Type.Missing;
				dynamic val2 = val.Workbooks.Open(obj, missing, false, missing, missing, missing, true, missing, missing, missing, missing, missing, missing, missing, missing);
				try
				{
					val2.ExportAsFixedFormat(0, fullPath2);
				}
				finally
				{
					val2.Close(false);
				}
			}
			catch (COMException ex)
			{
				throw new IOException($"Failed to convert Excel file to PDF (COM Error: 0x{ex.ErrorCode:X8}): {ex.Message}", ex);
			}
			catch (Exception ex2)
			{
				throw new IOException("Failed to convert Excel file to PDF: " + ex2.Message, ex2);
			}
			finally
			{
				if (val != null)
				{
					try
					{
						val.Quit();
						Marshal.ReleaseComObject(val);
					}
					catch
					{
					}
				}
			}
		}
	}
	public class PlaceholderPdfConvertService : IPdfConvertService
	{
		private readonly IPdfUtilsService _pdfUtils;

		public PlaceholderPdfConvertService(IPdfUtilsService pdfUtils = null)
		{
			_pdfUtils = pdfUtils ?? new PdfUtilsService();
		}

		public OperationResult Convert(FileItem inputFile, string outputPath)
		{
			if (inputFile == null || string.IsNullOrEmpty(inputFile.FullPath) || !File.Exists(inputFile.FullPath))
			{
				return OperationResult.Fail("Input file not found.");
			}
			string fullPath = inputFile.FullPath;
			if (string.IsNullOrEmpty(outputPath))
			{
				string tempDirectory = new TempWorkspaceService().GetTempDirectory();
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullPath);
				outputPath = Path.Combine(tempDirectory, fileNameWithoutExtension + ".pdf");
			}
			try
			{
				_pdfUtils.ConvertToPdfAsync(fullPath, outputPath).GetAwaiter().GetResult();
				return OperationResult.Ok(outputPath);
			}
			catch
			{
				return OperationResult.Fail("Conversion failed: Microsoft Office not installed or not accessible.");
			}
		}
	}
	public class PlaceholderPdfMergeService : IPdfMergeService
	{
		private readonly IPdfUtilsService _pdfUtils;

		public PlaceholderPdfMergeService(IPdfUtilsService pdfUtils = null)
		{
			_pdfUtils = pdfUtils ?? new PdfUtilsService();
		}

		public OperationResult Merge(IList<FileItem> inputFiles, string outputPath)
		{
			if (inputFiles == null || inputFiles.Count < 2)
			{
				return OperationResult.Fail("At least two input files are required for merge.");
			}
			List<string> list = inputFiles.Select((FileItem f) => f.FullPath).ToList();
			if (list.Any((string p) => string.IsNullOrEmpty(p) || !File.Exists(p)))
			{
				return OperationResult.Fail("One or more input files were not found.");
			}
			if (string.IsNullOrEmpty(outputPath))
			{
				outputPath = Path.Combine(new TempWorkspaceService().GetTempDirectory(), "merged.pdf");
			}
			try
			{
				_pdfUtils.MergeAsync(list, outputPath).GetAwaiter().GetResult();
				return OperationResult.Ok(outputPath);
			}
			catch
			{
				return OperationResult.Fail("Merge failed: Please try again.");
			}
		}
	}
	public interface ITempWorkspaceService
	{
		string GetTempDirectory();

		void CleanupTempDirectory();

		string CreateSessionDirectory();
	}
	public class TempWorkspaceService : ITempWorkspaceService
	{
		private string _sessionDirectory;

		private readonly string _baseTempPath;

		public TempWorkspaceService()
		{
			_baseTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TxTconverter", "temp");
		}

		public string GetTempDirectory()
		{
			if (string.IsNullOrEmpty(_sessionDirectory))
			{
				_sessionDirectory = CreateSessionDirectory();
			}
			return _sessionDirectory;
		}

		public string CreateSessionDirectory()
		{
			string path = Guid.NewGuid().ToString("N").Substring(0, 8);
			string text = Path.Combine(_baseTempPath, path);
			try
			{
				Directory.CreateDirectory(text);
				return text;
			}
			catch (Exception innerException)
			{
				throw new IOException("Failed to create temp directory at " + text, innerException);
			}
		}

		public void CleanupTempDirectory()
		{
			if (string.IsNullOrEmpty(_sessionDirectory) || !Directory.Exists(_sessionDirectory))
			{
				return;
			}
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(_sessionDirectory);
				FileInfo[] files = directoryInfo.GetFiles();
				foreach (FileInfo fileInfo in files)
				{
					try
					{
						fileInfo.Delete();
					}
					catch
					{
					}
				}
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				foreach (DirectoryInfo directoryInfo2 in directories)
				{
					try
					{
						directoryInfo2.Delete(recursive: true);
					}
					catch
					{
					}
				}
				Directory.Delete(_sessionDirectory, recursive: true);
			}
			catch (Exception)
			{
			}
		}
	}
}
namespace TXTconverter.App.Models
{
	public class FileItem
	{
		public string FullPath { get; set; }

		public string FileName => Path.GetFileName(FullPath);

		public string Extension => Path.GetExtension(FullPath);

		public long SizeBytes { get; set; }

		public FileItem()
		{
		}

		public FileItem(string fullPath)
		{
			FullPath = fullPath;
			if (File.Exists(fullPath))
			{
				SizeBytes = new FileInfo(fullPath).Length;
			}
		}

		public override string ToString()
		{
			return FileName;
		}
	}
	public class OperationResult
	{
		public bool Success { get; set; }

		public string Message { get; set; }

		public static OperationResult Ok(string message = "Operation completed successfully.")
		{
			return new OperationResult
			{
				Success = true,
				Message = message
			};
		}

		public static OperationResult Fail(string message)
		{
			return new OperationResult
			{
				Success = false,
				Message = message
			};
		}
	}
	public enum PdfOperation
	{
		ConvertToPdf,
		MergePdf,
		CompressPdf
	}
}
namespace TXTconverter.App.Interfaces
{
	public interface IFileValidationService
	{
		OperationResult Validate(IList<FileItem> files, PdfOperation operation);
	}
	public interface IPdfConvertService
	{
		OperationResult Convert(FileItem inputFile, string outputPath);
	}
	public interface IPdfMergeService
	{
		OperationResult Merge(IList<FileItem> inputFiles, string outputPath);
	}
}
namespace TXTconverter.App.Helpers
{
	public static class DragDropHelper
	{
		public static string[] GetDroppedFiles(DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				return e.Data.GetData(DataFormats.FileDrop) as string[];
			}
			return null;
		}

		public static bool HasFiles(DragEventArgs e)
		{
			return e.Data.GetDataPresent(DataFormats.FileDrop);
		}
	}
}
namespace TXTconverter.App.Converters
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool flag = value is bool && (bool)value;
			if (parameter != null && parameter.ToString() == "Invert")
			{
				flag = !flag;
			}
			return (object)(Visibility)((!flag) ? 2 : 0);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class StepToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || parameter == null)
			{
				return (object)(Visibility)2;
			}
			return (object)(Visibility)((!(value.ToString() == parameter.ToString())) ? 2 : 0);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
namespace TXTconverter.App.Constants
{
	public static class AppConstants
	{
		public const string AppName = "TXTconverter";

		public const string AppDisplayName = "TXTconverter - PDF Utility Suite";

		public const string DefaultInstallFolder = "TXTconverter";

		public const string AppExecutableName = "TXTconverter.exe";

		public const string UninstallerExecutableName = "Uninstaller.exe";

		public const string DesktopShortcutName = "TXTconverter.lnk";

		public const string StartMenuFolderName = "TXTconverter";

		public const string StartMenuShortcutName = "TXTconverter.lnk";

		public static readonly string[] SupportedInputExtensions = new string[7] { ".doc", ".docx", ".xlsx", ".xls", ".png", ".jpg", ".jpeg" };

		public static readonly string[] PdfExtension = new string[1] { ".pdf" };

		public const string FileFilterAllSupported = "Supported Files|*.doc;*.docx;*.xlsx;*.xls;*.png;*.jpg;*.jpeg|All Files|*.*";

		public const string FileFilterPdf = "PDF Files|*.pdf|All Files|*.*";
	}
}
namespace TXTconverter.App.Commands
{
	public class RelayCommand : ICommand
	{
		private readonly Action<object> _execute;

		private readonly Predicate<object> _canExecute;

		public event EventHandler CanExecuteChanged
		{
			add
			{
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				CommandManager.RequerySuggested -= value;
			}
		}

		public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
		{
			_execute = execute ?? throw new ArgumentNullException("execute");
			_canExecute = canExecute;
		}

		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}

		public bool CanExecute(object parameter)
		{
			if (_canExecute != null)
			{
				return _canExecute(parameter);
			}
			return true;
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}
	}
}
namespace TXTconverter.App.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		private int _selectedTabIndex;

		private string _statusMessage;

		private string _fileStatusMessage;

		public ConvertViewModel ConvertViewModel { get; private set; }

		public MergeViewModel MergeViewModel { get; private set; }

		public int SelectedTabIndex
		{
			get
			{
				return _selectedTabIndex;
			}
			set
			{
				if (SetProperty(ref _selectedTabIndex, value, "SelectedTabIndex"))
				{
					UpdateFileStatus();
				}
			}
		}

		public string StatusMessage
		{
			get
			{
				return _statusMessage;
			}
			set
			{
				SetProperty(ref _statusMessage, value, "StatusMessage");
			}
		}

		public string FileStatusMessage
		{
			get
			{
				return _fileStatusMessage;
			}
			set
			{
				SetProperty(ref _fileStatusMessage, value, "FileStatusMessage");
			}
		}

		public MainViewModel()
		{
			StatusMessage = "Ready";
			FileStatusMessage = "No file loaded";
			PlaceholderPdfConvertService convertService = new PlaceholderPdfConvertService();
			PlaceholderPdfMergeService mergeService = new PlaceholderPdfMergeService();
			FileValidationService validationService = new FileValidationService();
			ConvertViewModel = new ConvertViewModel(convertService, validationService);
			MergeViewModel = new MergeViewModel(mergeService, validationService);
			ConvertViewModel.PropertyChanged += OnChildStatusChanged;
			MergeViewModel.PropertyChanged += OnChildStatusChanged;
		}

		private void OnChildStatusChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "StatusMessage" || e.PropertyName == "CurrentStep")
			{
				UpdateFileStatus();
			}
		}

		private void UpdateFileStatus()
		{
			switch (_selectedTabIndex)
			{
			case 0:
				FileStatusMessage = ConvertViewModel.StatusMessage;
				StatusMessage = ((ConvertViewModel.CurrentStep == 1) ? "Processing" : "Ready");
				break;
			case 1:
				FileStatusMessage = MergeViewModel.StatusMessage;
				StatusMessage = ((MergeViewModel.CurrentStep == 1) ? "Processing" : "Ready");
				break;
			default:
				FileStatusMessage = "No file loaded";
				StatusMessage = "Ready";
				break;
			}
		}
	}
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetProperty<T>(ref T field, T value, string propertyName)
		{
			if (object.Equals(field, value))
			{
				return false;
			}
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}
	}
	public class ConvertViewModel : ViewModelBase
	{
		private readonly IPdfConvertService _convertService;

		private readonly IFileValidationService _validationService;

		private string _statusMessage;

		private int _currentStep;

		private int _progressValue;

		private string _progressText;

		private string _selectedFormat;

		private string _fileInfoText;

		private string _outputFilePath;

		private DispatcherTimer _progressTimer;

		public ObservableCollection<FileItem> SelectedFiles { get; private set; }

		public ObservableCollection<string> FormatOptions { get; private set; }

		public string StatusMessage
		{
			get
			{
				return _statusMessage;
			}
			set
			{
				SetProperty(ref _statusMessage, value, "StatusMessage");
			}
		}

		public int CurrentStep
		{
			get
			{
				return _currentStep;
			}
			set
			{
				SetProperty(ref _currentStep, value, "CurrentStep");
				((RelayCommand)DownloadCommand)?.RaiseCanExecuteChanged();
			}
		}

		public int ProgressValue
		{
			get
			{
				return _progressValue;
			}
			set
			{
				SetProperty(ref _progressValue, value, "ProgressValue");
				ProgressText = value + "%";
			}
		}

		public string ProgressText
		{
			get
			{
				return _progressText;
			}
			set
			{
				SetProperty(ref _progressText, value, "ProgressText");
			}
		}

		public string SelectedFormat
		{
			get
			{
				return _selectedFormat;
			}
			set
			{
				SetProperty(ref _selectedFormat, value, "SelectedFormat");
			}
		}

		public string FileInfoText
		{
			get
			{
				return _fileInfoText;
			}
			set
			{
				SetProperty(ref _fileInfoText, value, "FileInfoText");
			}
		}

		public bool HasFile => SelectedFiles.Count > 0;

		public ICommand BrowseCommand { get; private set; }

		public ICommand RemoveFileCommand { get; private set; }

		public ICommand ConvertCommand { get; private set; }

		public ICommand ClearCommand { get; private set; }

		public ICommand DownloadCommand { get; private set; }

		public ICommand StartOverCommand { get; private set; }

		public ConvertViewModel(IPdfConvertService convertService, IFileValidationService validationService)
		{
			_convertService = convertService;
			_validationService = validationService;
			SelectedFiles = new ObservableCollection<FileItem>();
			FormatOptions = new ObservableCollection<string> { "docx to pdf" };
			SelectedFormat = FormatOptions[0];
			StatusMessage = "No file loaded";
			ProgressText = "0%";
			CurrentStep = 0;
			BrowseCommand = new RelayCommand(OnBrowse);
			RemoveFileCommand = new RelayCommand(OnRemoveFile, (object _) => SelectedFiles.Count > 0);
			ConvertCommand = new RelayCommand(OnConvert, (object _) => SelectedFiles.Count == 1);
			ClearCommand = new RelayCommand(OnClear, (object _) => SelectedFiles.Count > 0);
			DownloadCommand = new RelayCommand(OnDownload, (object _) => CurrentStep == 2);
			StartOverCommand = new RelayCommand(OnStartOver);
			SelectedFiles.CollectionChanged += delegate
			{
				OnPropertyChanged("HasFile");
			};
		}

		private void OnBrowse(object parameter)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			OpenFileDialog val = new OpenFileDialog
			{
				Filter = "Supported Files|*.doc;*.docx;*.xlsx;*.xls;*.png;*.jpg;*.jpeg|All Files|*.*",
				Multiselect = false,
				Title = "Select a file to convert to PDF"
			};
			if (((CommonDialog)val).ShowDialog() == true)
			{
				SelectedFiles.Clear();
				FileItem item = new FileItem(((FileDialog)val).FileName);
				SelectedFiles.Add(item);
				UpdateFileInfo(item);
				StatusMessage = "File loaded";
			}
		}

		private void UpdateFileInfo(FileItem item)
		{
			string text = ((double)item.SizeBytes / 1048576.0).ToString("F1");
			string text2 = item.Extension.TrimStart(new char[1] { '.' }).ToUpperInvariant();
			FileInfoText = "Original Size: " + text + " MB | Type: " + text2;
		}

		private void OnRemoveFile(object parameter)
		{
			SelectedFiles.Clear();
			FileInfoText = string.Empty;
			StatusMessage = "No file loaded";
		}

		private void OnConvert(object parameter)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Expected O, but got Unknown
			OperationResult operationResult = _validationService.Validate(SelectedFiles.ToList(), PdfOperation.ConvertToPdf);
			if (!operationResult.Success)
			{
				StatusMessage = operationResult.Message;
				return;
			}
			CurrentStep = 1;
			ProgressValue = 0;
			StatusMessage = "Processing";
			_progressTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(50.0)
			};
			_progressTimer.Tick += delegate
			{
				ProgressValue += 2;
				if (ProgressValue >= 100)
				{
					_progressTimer.Stop();
					ProgressValue = 100;
					Task.Run(delegate
					{
						OperationResult result = _convertService.Convert(SelectedFiles[0], null);
						((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
						{
							_outputFilePath = (result.Success ? result.Message : null);
							CurrentStep = 2;
							StatusMessage = (result.Success ? "Conversion completed" : result.Message);
						});
					});
				}
			};
			_progressTimer.Start();
		}

		private void OnDownload(object parameter)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected O, but got Unknown
			SaveFileDialog val = new SaveFileDialog
			{
				Filter = "PDF Files|*.pdf|All Files|*.*",
				FileName = ((SelectedFiles.Count > 0) ? (Path.GetFileNameWithoutExtension(SelectedFiles[0].FileName) + ".pdf") : "converted.pdf")
			};
			if (((CommonDialog)val).ShowDialog() != true)
			{
				return;
			}
			try
			{
				if (!string.IsNullOrEmpty(_outputFilePath) && File.Exists(_outputFilePath))
				{
					File.Copy(_outputFilePath, ((FileDialog)val).FileName, overwrite: true);
					StatusMessage = "File saved to: " + ((FileDialog)val).FileName;
				}
				else
				{
					StatusMessage = "No output file available to save.";
				}
			}
			catch (Exception ex)
			{
				StatusMessage = "Failed to save file: " + ex.Message;
			}
		}

		private void OnStartOver(object parameter)
		{
			OnClear(null);
			CurrentStep = 0;
		}

		private void OnClear(object parameter)
		{
			SelectedFiles.Clear();
			FileInfoText = string.Empty;
			ProgressValue = 0;
			CurrentStep = 0;
			StatusMessage = "No file loaded";
		}

		public void AddDroppedFiles(string[] filePaths)
		{
			if (CurrentStep == 0)
			{
				SelectedFiles.Clear();
				if (filePaths != null && filePaths.Length != 0)
				{
					FileItem item = new FileItem(filePaths[0]);
					SelectedFiles.Add(item);
					UpdateFileInfo(item);
					StatusMessage = "File loaded";
				}
			}
		}
	}
	public class MergeViewModel : ViewModelBase
	{
		private readonly IPdfMergeService _mergeService;

		private readonly IFileValidationService _validationService;

		private string _statusMessage;

		private int _currentStep;

		private int _progressValue;

		private string _progressText;

		private string _outputFilePath;

		private DispatcherTimer _progressTimer;

		public ObservableCollection<FileItem> SelectedFiles { get; private set; }

		public string StatusMessage
		{
			get
			{
				return _statusMessage;
			}
			set
			{
				SetProperty(ref _statusMessage, value, "StatusMessage");
			}
		}

		public int CurrentStep
		{
			get
			{
				return _currentStep;
			}
			set
			{
				SetProperty(ref _currentStep, value, "CurrentStep");
				((RelayCommand)DownloadCommand)?.RaiseCanExecuteChanged();
			}
		}

		public int ProgressValue
		{
			get
			{
				return _progressValue;
			}
			set
			{
				SetProperty(ref _progressValue, value, "ProgressValue");
				ProgressText = value + "%";
			}
		}

		public string ProgressText
		{
			get
			{
				return _progressText;
			}
			set
			{
				SetProperty(ref _progressText, value, "ProgressText");
			}
		}

		public bool HasFiles => SelectedFiles.Count > 0;

		public ICommand BrowseCommand { get; private set; }

		public ICommand RemoveFileCommand { get; private set; }

		public ICommand MergeCommand { get; private set; }

		public ICommand ClearCommand { get; private set; }

		public ICommand DownloadCommand { get; private set; }

		public ICommand StartOverCommand { get; private set; }

		public MergeViewModel(IPdfMergeService mergeService, IFileValidationService validationService)
		{
			_mergeService = mergeService;
			_validationService = validationService;
			SelectedFiles = new ObservableCollection<FileItem>();
			StatusMessage = "No file loaded";
			ProgressText = "0%";
			CurrentStep = 0;
			BrowseCommand = new RelayCommand(OnBrowse);
			RemoveFileCommand = new RelayCommand(OnRemoveFile, (object _) => SelectedFiles.Count > 0);
			MergeCommand = new RelayCommand(OnMerge, (object _) => SelectedFiles.Count >= 2);
			ClearCommand = new RelayCommand(OnClear, (object _) => SelectedFiles.Count > 0);
			DownloadCommand = new RelayCommand(OnDownload, (object _) => CurrentStep == 2);
			StartOverCommand = new RelayCommand(OnStartOver);
			SelectedFiles.CollectionChanged += delegate
			{
				OnPropertyChanged("HasFiles");
			};
		}

		private void OnBrowse(object parameter)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			OpenFileDialog val = new OpenFileDialog
			{
				Filter = "PDF Files|*.pdf|All Files|*.*",
				Multiselect = true,
				Title = "Select PDF files to merge"
			};
			if (((CommonDialog)val).ShowDialog() == true)
			{
				string[] fileNames = ((FileDialog)val).FileNames;
				foreach (string fullPath in fileNames)
				{
					SelectedFiles.Add(new FileItem(fullPath));
				}
				StatusMessage = SelectedFiles.Count + " file(s) loaded";
			}
		}

		private void OnRemoveFile(object parameter)
		{
			if (parameter is FileItem item)
			{
				SelectedFiles.Remove(item);
			}
			else if (SelectedFiles.Count > 0)
			{
				SelectedFiles.RemoveAt(SelectedFiles.Count - 1);
			}
			StatusMessage = ((SelectedFiles.Count > 0) ? (SelectedFiles.Count + " file(s) loaded") : "No file loaded");
		}

		private void OnMerge(object parameter)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Expected O, but got Unknown
			OperationResult operationResult = _validationService.Validate(SelectedFiles.ToList(), PdfOperation.MergePdf);
			if (!operationResult.Success)
			{
				StatusMessage = operationResult.Message;
				return;
			}
			CurrentStep = 1;
			ProgressValue = 0;
			StatusMessage = "Processing";
			_progressTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(50.0)
			};
			_progressTimer.Tick += delegate
			{
				ProgressValue += 2;
				if (ProgressValue >= 100)
				{
					_progressTimer.Stop();
					ProgressValue = 100;
					Task.Run(delegate
					{
						OperationResult result = _mergeService.Merge(SelectedFiles.ToList(), null);
						((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
						{
							_outputFilePath = (result.Success ? result.Message : null);
							CurrentStep = 2;
							StatusMessage = (result.Success ? "Merge completed" : result.Message);
						});
					});
				}
			};
			_progressTimer.Start();
		}

		private void OnDownload(object parameter)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			SaveFileDialog val = new SaveFileDialog
			{
				Filter = "PDF Files|*.pdf|All Files|*.*",
				FileName = "merged.pdf"
			};
			if (((CommonDialog)val).ShowDialog() != true)
			{
				return;
			}
			try
			{
				if (!string.IsNullOrEmpty(_outputFilePath) && File.Exists(_outputFilePath))
				{
					File.Copy(_outputFilePath, ((FileDialog)val).FileName, overwrite: true);
					StatusMessage = "File saved to: " + ((FileDialog)val).FileName;
				}
				else
				{
					StatusMessage = "No output file available to save.";
				}
			}
			catch (Exception ex)
			{
				StatusMessage = "Failed to save file: " + ex.Message;
			}
		}

		private void OnStartOver(object parameter)
		{
			OnClear(null);
			CurrentStep = 0;
		}

		private void OnClear(object parameter)
		{
			SelectedFiles.Clear();
			ProgressValue = 0;
			CurrentStep = 0;
			StatusMessage = "No file loaded";
		}

		public void AddDroppedFiles(string[] filePaths)
		{
			if (CurrentStep == 0 && filePaths != null)
			{
				foreach (string fullPath in filePaths)
				{
					SelectedFiles.Add(new FileItem(fullPath));
				}
				StatusMessage = SelectedFiles.Count + " file(s) loaded";
			}
		}
	}
}
