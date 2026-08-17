using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ChilledWindows.Properties;

namespace ChilledWindows;

public class MainWindow : Window, IComponentConnector
{
	private TransformGroup fTransformGroup = new TransformGroup();

	private TransformGroup gTransGroup = new TransformGroup();

	private TranslateTransform gTransTransform = new TranslateTransform();

	private ScaleTransform gScaleTransform = new ScaleTransform();

	private ScaleTransform fFlipTrans = new ScaleTransform();

	private ScaleTransform FlipTrans1 = new ScaleTransform();

	private ScaleTransform FlipTrans2 = new ScaleTransform();

	private RotateTransform fRotateTrans = new RotateTransform();

	private int screenWidth;

	private int screenHeight;

	private DispatcherTimer dt = new DispatcherTimer();

	private int[] flipTimes = new int[96]
	{
		126, 129, 133, 136, 140, 143, 147, 150, 155, 158,
		162, 165, 169, 172, 176, 179, 184, 187, 191, 194,
		198, 201, 205, 208, 213, 216, 220, 223, 227, 230,
		234, 237, 242, 245, 249, 252, 256, 259, 263, 266,
		271, 274, 278, 281, 285, 286, 288, 292, 295, 297,
		300, 303, 307, 310, 314, 317, 321, 324, 329, 332,
		336, 339, 343, 346, 350, 353, 355, 358, 361, 365,
		368, 372, 375, 379, 382, 387, 390, 394, 397, 401,
		404, 408, 411, 416, 419, 423, 426, 430, 433, 437,
		0, 0, 0, 0, 0, 0
	};

	private int[] flipTimes2 = new int[40]
	{
		440, 443, 449, 454, 456, 460, 468, 476, 481, 486,
		497, 501, 504, 506, 512, 514, 518, 522, 527, 531,
		533, 536, 540, 548, 552, 557, 561, 563, 569, 572,
		576, 580, 582, 585, 0, 0, 0, 0, 0, 0
	};

	private int[] flipTimes1 = new int[14]
	{
		454, 476, 497, 512, 531, 548, 569, 585, 0, 0,
		0, 0, 0, 0
	};

	private int flipIndex;

	private int flipIndex1;

	private int flipIndex2;

	private int frameIndex;

	private bool refreshFirstFlips = true;

	private bool refreshSecondFlips = true;

	internal MediaElement mediaElement;

	internal Rectangle bg;

	internal Image firstBg;

	internal Label label;

	internal Grid twoGrid;

	internal Image bg2;

	internal Image bg3;

	private bool _contentLoaded;

	public MainWindow()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		Type? typeFromProgID = Type.GetTypeFromProgID("Shell.Application");
		object target = Activator.CreateInstance(typeFromProgID);
		typeFromProgID.InvokeMember("MinimizeAll", BindingFlags.InvokeMethod, null, target, null);
		Thread.Sleep(300);
		screenWidth = (int)SystemParameters.PrimaryScreenWidth;
		screenHeight = (int)SystemParameters.PrimaryScreenHeight;
		Bitmap val = new Bitmap(screenWidth, screenHeight);
		Graphics val2 = Graphics.FromImage((Image)(object)val);
		try
		{
			val2.CopyFromScreen(0, 0, 0, 0, ((Image)val).Size);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		InitializeComponent();
		((Window)this).WindowState = (WindowState)0;
		((Window)this).WindowStyle = (WindowStyle)0;
		((Window)this).Topmost = true;
		((Window)this).WindowState = (WindowState)2;
		ImageSource source = (ImageSource)(object)BitmapToImageSource(val);
		firstBg.Source = source;
		bg2.Source = source;
		bg3.Source = source;
		((UIElement)firstBg).RenderTransformOrigin = new Point(0.5, 0.5);
		fTransformGroup.Children.Add((Transform)(object)fFlipTrans);
		fTransformGroup.Children.Add((Transform)(object)fRotateTrans);
		((UIElement)firstBg).RenderTransform = (Transform)(object)fTransformGroup;
		((UIElement)bg2).RenderTransformOrigin = new Point(0.5, 0.5);
		((UIElement)bg2).RenderTransform = (Transform)(object)FlipTrans1;
		((UIElement)bg3).RenderTransformOrigin = new Point(0.5, 0.5);
		((UIElement)bg3).RenderTransform = (Transform)(object)FlipTrans2;
		((UIElement)twoGrid).RenderTransformOrigin = new Point(0.0, 0.0);
		gTransGroup.Children.Add((Transform)(object)gTransTransform);
		gTransGroup.Children.Add((Transform)(object)gScaleTransform);
		((UIElement)twoGrid).RenderTransform = (Transform)(object)gTransGroup;
		File.WriteAllBytes("chilledwindows.mp4", Resources.Chilled_Windows);
		mediaElement.Source = new Uri("chilledwindows.mp4", UriKind.Relative);
		dt.Tick += Dt_Tick;
		dt.Interval = new TimeSpan(0, 0, 0, 0, 10);
		dt.Start();
	}

	private void Dt_Tick(object sender, EventArgs e)
	{
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		frameIndex = (int)Math.Floor(mediaElement.Position.TotalMilliseconds / 33.33333);
		((ContentControl)label).Content = "Frame:" + frameIndex;
		if (frameIndex == 438)
		{
			refreshFirstFlips = false;
			((UIElement)firstBg).Visibility = (Visibility)1;
			((UIElement)twoGrid).Visibility = (Visibility)0;
		}
		if (frameIndex == 585)
		{
			refreshSecondFlips = false;
		}
		if (frameIndex == 622)
		{
			((UIElement)bg).Visibility = (Visibility)1;
			double num = (double)screenWidth * 0.13817330210772832;
			double num2 = (double)screenHeight * (17.0 / 48.0);
			DoubleAnimation val = new DoubleAnimation(0.0, num * 3.0, Duration.op_Implicit(TimeSpan.FromMilliseconds(500.0)));
			DoubleAnimation val2 = new DoubleAnimation(0.0, num2 * 3.0, Duration.op_Implicit(TimeSpan.FromMilliseconds(500.0)));
			DoubleAnimation val3 = new DoubleAnimation(1.0, 0.3, Duration.op_Implicit(TimeSpan.FromMilliseconds(500.0)));
			DoubleAnimation val4 = new DoubleAnimation(1.0, 0.3, Duration.op_Implicit(TimeSpan.FromMilliseconds(500.0)));
			((Animatable)gTransTransform).BeginAnimation(TranslateTransform.XProperty, (AnimationTimeline)(object)val);
			((Animatable)gTransTransform).BeginAnimation(TranslateTransform.YProperty, (AnimationTimeline)(object)val2);
			((Animatable)gScaleTransform).BeginAnimation(ScaleTransform.ScaleXProperty, (AnimationTimeline)(object)val3);
			((Animatable)gScaleTransform).BeginAnimation(ScaleTransform.ScaleYProperty, (AnimationTimeline)(object)val4);
		}
		if (frameIndex == 665)
		{
			((UIElement)twoGrid).Visibility = (Visibility)1;
		}
		if (frameIndex == 1260)
		{
			File.Delete("chilledwindows.mp4");
			Application.Current.Shutdown();
		}
		if (refreshFirstFlips)
		{
			if (flipTimes[flipIndex] <= frameIndex)
			{
				flipIndex++;
				fFlipTrans.ScaleX = ((fFlipTrans.ScaleX == -1.0) ? 1 : (-1));
			}
			if (frameIndex == 286)
			{
				fRotateTrans.Angle = -20.0;
			}
		}
		else if (refreshSecondFlips)
		{
			if (flipTimes1[flipIndex1] <= frameIndex)
			{
				flipIndex1++;
				FlipTrans1.ScaleX = ((FlipTrans1.ScaleX == -1.0) ? 1 : (-1));
			}
			if (flipTimes2[flipIndex2] <= frameIndex)
			{
				flipIndex2++;
				FlipTrans2.ScaleX = ((FlipTrans2.ScaleX == -1.0) ? 1 : (-1));
			}
		}
	}

	private BitmapImage BitmapToImageSource(Bitmap bitmap)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		using MemoryStream memoryStream = new MemoryStream();
		((Image)bitmap).Save((Stream)memoryStream, ImageFormat.Bmp);
		memoryStream.Position = 0L;
		BitmapImage val = new BitmapImage();
		val.BeginInit();
		val.StreamSource = memoryStream;
		val.CacheOption = (BitmapCacheOption)1;
		val.EndInit();
		return val;
	}

	private void Window_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)e.Key == 18)
		{
			fFlipTrans.ScaleX = ((fFlipTrans.ScaleX == -1.0) ? 1 : (-1));
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/ChilledWindows;component/mainwindow.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((UIElement)(MainWindow)target).KeyDown += new KeyEventHandler(Window_KeyDown);
			break;
		case 2:
			mediaElement = (MediaElement)target;
			break;
		case 3:
			bg = (Rectangle)target;
			break;
		case 4:
			firstBg = (Image)target;
			break;
		case 5:
			label = (Label)target;
			break;
		case 6:
			twoGrid = (Grid)target;
			break;
		case 7:
			bg2 = (Image)target;
			break;
		case 8:
			bg3 = (Image)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
