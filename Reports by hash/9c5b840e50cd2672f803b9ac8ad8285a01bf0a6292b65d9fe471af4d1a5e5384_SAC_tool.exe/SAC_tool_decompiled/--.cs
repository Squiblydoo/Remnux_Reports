using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

internal static class _0002_0015
{
	private enum _0002
	{

	}

	private sealed class _000E
	{
		private Stream m__0002;

		private byte[] m__000E;

		public _000E(Stream _0002)
		{
			this.m__0002 = _0002;
			m__000E = new byte[4];
		}

		public Stream _0002()
		{
			return this.m__0002;
		}

		public short _0002()
		{
			_0002(2);
			return (short)(m__000E[0] | (m__000E[1] << 8));
		}

		public int _0002()
		{
			_0002(4);
			return m__000E[0] | (m__000E[1] << 8) | (m__000E[2] << 16) | (m__000E[3] << 24);
		}

		private static void _0002()
		{
			throw new EndOfStreamException();
		}

		private void _0002(int _0002)
		{
			int num = 0;
			int num2 = 0;
			if (_0002 == 1)
			{
				num2 = this.m__0002.ReadByte();
				if (num2 == -1)
				{
					_0002_0015._000E._0002();
				}
				m__000E[0] = (byte)num2;
				return;
			}
			do
			{
				num2 = this.m__0002.Read(m__000E, num, _0002 - num);
				if (num2 == 0)
				{
					_0002_0015._000E._0002();
				}
				num += num2;
			}
			while (num < _0002);
		}

		public void _0002()
		{
			Stream stream = this.m__0002;
			this.m__0002 = null;
			stream?.Close();
			m__000E = null;
		}

		public byte[] _0002(int _0002)
		{
			if (_0002 < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			byte[] array = new byte[_0002];
			int num = 0;
			do
			{
				int num2 = this.m__0002.Read(array, num, _0002);
				if (num2 == 0)
				{
					break;
				}
				num += num2;
				_0002 -= num2;
			}
			while (_0002 > 0);
			if (num != array.Length)
			{
				byte[] array2 = new byte[num];
				Buffer.BlockCopy(array, 0, array2, 0, num);
				array = array2;
			}
			return array;
		}
	}

	private static _0002 _000E_2005;

	private static ConcurrentDictionary<int, string> m__0002;

	private static byte[] _0005;

	private static int _0008;

	private static byte[] _0003;

	private static short _0006;

	private static _000E m__000E;

	private static int _0002_2005;

	private static int _000F;

	[MethodImpl(MethodImplOptions.NoInlining)]
	static _0002_0015()
	{
		int num = 1144053463;
		int num2 = 0x60828A51 ^ num;
		_0002_0015.m__0002 = new ConcurrentDictionary<int, string>();
		int num3 = 2;
		StackTrace stackTrace = new StackTrace(num3, fNeedFileInfo: false);
		num3 -= 2;
		StackFrame frame = stackTrace.GetFrame(num3);
		int num4 = num3;
		if (frame == null)
		{
			stackTrace = new StackTrace();
			num4 = 1;
			frame = stackTrace.GetFrame(num4);
		}
		int num5 = -(~(-(~(-(~(~(-(~((num + -1925579307) ^ num2))))))))) ^ ~(-(~(-(-(~(~(-(~((-1935436988 ^ num) + num2)))))))));
		MethodBase methodBase = frame?.GetMethod();
		if (frame != null)
		{
			num5 ^= -(~(~(-(~(-(~(-(~((237537874 - num) ^ num2)))))))));
		}
		Type type = methodBase?.DeclaringType;
		if (type == typeof(RuntimeMethodHandle))
		{
			_000E_2005 = (_0002)4 | _000E_2005;
			num5 ^= (num ^ 0x6082897A) - num2 + num3;
		}
		else if (type == null)
		{
			if (_0002(stackTrace, num4))
			{
				_000E_2005 |= (_0002)16;
				num5 ^= -(~(~(-(~(-(~(-(~((528405878 - num) ^ num2))))))))) - num3;
			}
			else
			{
				num5 ^= -(~(~(-(-(~(-(~(-(~(~(num + -528423685 - num2)))))))))));
				_000E_2005 |= (_0002)1;
			}
		}
		else
		{
			_000E_2005 = (_0002)16 | _000E_2005;
			num5 ^= ~(-(-(~(-(~(~(-(-(~(~(num ^ -1619187635 ^ num2))))))))))) - num3;
		}
		_0002_2005 = num5 + _0002_2005;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal static string _0002(int _0002)
	{
		if (_0002_0015.m__0002.TryGetValue(_0002, out var value))
		{
			return value;
		}
		return _0002_0015._0002(_0002, _000E: true);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static string _0002(int _0002, bool _000E)
	{
		int num = 286385411;
		int num2 = -382752286 + num;
		string value = null;
		byte[] array;
		int num20;
		int num21;
		int num22;
		int num24;
		byte[] array4;
		byte[] array3;
		int num23;
		while (true)
		{
			lock (_0002_0015.m__0002)
			{
				int num7;
				if (_0002_0015.m__000E == null)
				{
					Assembly executingAssembly = Assembly.GetExecutingAssembly();
					Assembly callingAssembly;
					try
					{
						callingAssembly = Assembly.GetCallingAssembly();
					}
					catch (PlatformNotSupportedException)
					{
						callingAssembly = executingAssembly;
					}
					_000F |= (191562090 - num) ^ num2;
					StringBuilder stringBuilder = new StringBuilder();
					int num3 = (num ^ 0xB524BE4) - num2;
					stringBuilder.Append((char)(num3 >> 16)).Append((char)num3);
					num3 = (-346778652 - num) ^ num2;
					stringBuilder.Append((char)num3).Append((char)(num3 >> 16));
					num3 = (num ^ 0xB544BEC) - num2;
					stringBuilder.Append((char)num3).Append((char)(num3 >> 16));
					num3 = (num + -918959648) ^ num2;
					stringBuilder.Append((char)(num3 >> 16)).Append((char)num3);
					num3 = 919631399 - num + num2;
					stringBuilder.Append((char)num3).Append((char)(num3 >> 16));
					Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(stringBuilder.ToString());
					int num4 = 2;
					StackTrace stackTrace = new StackTrace(num4, fNeedFileInfo: false);
					_000F ^= ((num ^ -347058904) - num2) | num4;
					num4 -= 2;
					StackFrame frame = stackTrace.GetFrame(num4);
					int num5 = num4;
					if (frame == null)
					{
						stackTrace = new StackTrace();
						num5 = 1;
						frame = stackTrace.GetFrame(num5);
					}
					MethodBase methodBase = frame?.GetMethod();
					_000F ^= num4 + (-190018408 + num + num2);
					Type type = methodBase?.DeclaringType;
					if (frame == null)
					{
						_000F ^= (190102361 - num) ^ num2;
					}
					bool flag = type == typeof(RuntimeMethodHandle);
					_000F ^= num + -190018376 + num2;
					if (!flag)
					{
						flag = type == null;
						if (flag)
						{
							if (_0002_0015._0002(stackTrace, num5))
							{
								flag = false;
							}
							else
							{
								_000F ^= (190102393 - num) ^ num2;
							}
						}
					}
					if (flag == (stackTrace != null))
					{
						_000F ^= 32;
					}
					_000F ^= (num + -190012034 + num2) | (1 + num4);
					_0002_0015.m__000E = new _000E(manifestResourceStream);
					short num6 = (short)(_0002_0015.m__000E._0002() ^ (short)(-(~(~(-(-(~(-(~(~((num ^ -347040124) - num2)))))))))));
					if (num6 == 0)
					{
						_0006 = (short)(_0002_0015.m__000E._0002() ^ (short)(~(-(-(~(~(-(~(-(~((num + -382749617) ^ num2)))))))))));
					}
					else
					{
						_0003 = _0002_0015.m__000E._0002(num6);
					}
					callingAssembly = executingAssembly;
					AssemblyName assemblyName = _0002_0015._0002(callingAssembly);
					_0005 = _0002_0015._0002(assemblyName);
					num7 = _0002_2005;
					_0002_2005 = 0;
					long num8 = _0005_2005._0002();
					num7 ^= (int)num8;
					num7 ^= 305132473 - num - num2;
					num7 ^= (num + -683020483) ^ num2;
					int num9 = num7;
					int num10 = 0;
					global::_0005<int> obj = null;
					int num11 = 0;
					int num12 = 0;
					int num13 = 0;
					int num14 = 0;
					int num15 = 0;
					num13 = num9;
					num14 = num13 ^ (-220073689 - num + num2);
					num12 = num14 * (-382746993 + num - num2) % (-352901621 ^ num ^ num2);
					num11 = 0;
					obj = null;
					num11 = num + -382752169 - num2;
					num15 = num12;
					num10 = 0;
					obj = ((global::_0003<int>)new _0008._0003(382752284 - num + num2)
					{
						_000F = num15
					}).GetEnumerator();
					try
					{
						while (((_0006)obj)._0006_2002_2001_0002())
						{
							num10 = obj._0006_2002_2001_0002();
							num12 ^= num10 - num11;
							num11 -= num12 + 3 >> 8;
						}
					}
					finally
					{
						obj?._000F_2002_2001_0002();
					}
					num7 ^= -382037228 + num - num2 + -(~(-(~(~(-(-(~(~((382752045 - num) ^ num2)))))))));
					int num16 = num12;
					num7 ^= -(~(-(~(~(-(~(-(~(-(~(2142686920 + num - num2)))))))))));
					num7 = num16 + num7;
					_000F = (_000F & (651187600 - num + num2)) ^ ((num + -382750882) ^ num2);
					_0008 = num7;
					if (((uint)_000E_2005 & (uint)(-(~(~(-(~(-(~(-(~(-(~(190018516 - num - num2))))))))))))) == 0)
					{
						_000F = (-382779300 + num) ^ num2;
					}
				}
				else
				{
					num7 = _0008;
				}
				if (_000F == 382796248 - num + num2)
				{
					value = new string(new char[3]
					{
						(char)(num ^ -347051074 ^ num2),
						'0',
						(char)(-382752198 + num - num2)
					});
					return value;
				}
				int num17 = _0002 ^ ((-983980603 - num) ^ num2) ^ num7;
				num17 ^= (0x8BAD669 ^ num) + num2;
				_0002_0015.m__000E._0002().Position = num17;
				if (_0003 != null)
				{
					array = _0003;
				}
				else
				{
					short num18 = ((_0006 != -1) ? _0006 : ((short)(_0002_0015.m__000E._0002() ^ ((num + -190045902) ^ num2) ^ num17)));
					if (num18 == 0)
					{
						array = null;
					}
					else
					{
						array = _0002_0015.m__000E._0002(num18);
						for (int num19 = 0; num19 != array.Length; num19 = 1 + num19)
						{
							array[num19] ^= (byte)(_0008 >> ((num19 & 3) << 3));
						}
					}
				}
				num20 = _0002_0015.m__000E._0002() ^ num17 ^ -(~(~(-(~(-(~(-(~(-1177450231 - num - num2))))))))) ^ num7;
				if (num20 == ((-190018536 + num) ^ num2))
				{
					byte[] array2 = _0002_0015.m__000E._0002(4);
					_0002 = ((0x4651BEF4 ^ num) + num2) ^ num7;
					_0002 = (array2[2] | (array2[3] << 16) | (array2[0] << 8) | (array2[1] << 24)) ^ -_0002;
					goto IL_0013;
				}
				num21 = num + -381144472 - num2;
				num22 = _000F;
				num23 = num22 - 12;
				num24 = num20;
				num20 &= (num ^ 0x4AF9419) + num2;
				array3 = _0002_0015.m__000E._0002(num20);
				array4 = _0005;
			}
			break;
			IL_0013:
			if (_0002_0015.m__0002.TryGetValue(_0002, out value))
			{
				return value;
			}
		}
		bool flag2 = (num24 & ((-1456494110 + num) ^ num2)) != 0;
		bool flag3 = (num24 & ((0xB506BE6 ^ num) - num2)) != 0;
		bool flag4 = (num24 & ((1764731362 + num) ^ num2)) != 0;
		byte[] array5 = array;
		byte[] array6 = array3;
		byte[] array7 = array5;
		byte b = 0;
		byte b2 = 0;
		byte b3 = 0;
		uint num25 = 0u;
		int num26 = 0;
		ushort num27 = 0;
		byte b4 = 0;
		int num28 = 0;
		b = array7[1];
		num28 = array6.Length;
		b2 = (byte)((num28 + 11) ^ (b + 7));
		num25 = (uint)((array7[0] | (array7[2] << 8)) + (b2 << 3));
		num26 = 0;
		num27 = 0;
		while (num26 < num28)
		{
			if ((1 & num26) == 0)
			{
				num25 = (uint)((int)num25 * ((num ^ 0x14D0501B) + num2) + ((-380294365 + num) ^ num2));
				num27 = (ushort)(num25 >> 16);
			}
			b3 = (byte)num27;
			num27 >>= 8;
			b4 = array6[num26];
			array6[num26] = (byte)(b4 ^ b ^ (3 + b2) ^ b3);
			num26++;
			b2 = b4;
		}
		array3 = array6;
		if (array4 != null != (num22 != num21))
		{
			for (int num29 = 0; num29 < num20; num29 = 1 + num29)
			{
				byte b5 = array4[7 & num29];
				b5 = (byte)((b5 << 3) | (b5 >> 5));
				array3[num29] ^= b5;
			}
		}
		byte[] array8;
		int num30;
		if (!flag3)
		{
			array8 = array3;
			num30 = num20;
		}
		else
		{
			num30 = array3[2] | (array3[0] << 16) | (array3[3] << 8) | (array3[1] << 24);
			array8 = new byte[num30];
			_0002_0015._0002(array3, 4, array8);
		}
		if (flag2 && num23 == num21 - 12)
		{
			char[] array9 = new char[num30];
			for (int i = 0; i < num30; i++)
			{
				array9[i] = (char)array8[i];
			}
			value = new string(array9);
		}
		else
		{
			char[] array10 = new char[num30 / 2];
			int num31 = 0;
			for (int num32 = 0; num32 < num30; num32 = 2 + num32)
			{
				array10[num31++] = (char)(array8[num32] | (array8[1 + num32] << 8));
			}
			value = new string(array10);
		}
		num23 += (0x14AF9499 ^ num) + num2 + (num23 & 3) << 5;
		if (num23 != num21 - 12 + (((-382752361 + num) ^ num2) + ((num21 - 12) & 3) << 5))
		{
			int num33 = (num20 + _0002) ^ ((-346145698 ^ num) - num2) ^ (num23 & ((190017771 - num) ^ num2));
			StringBuilder stringBuilder = new StringBuilder();
			int num3 = (-347051458 ^ num) - num2;
			stringBuilder.Append((char)(byte)num3);
			value = num33.ToString(stringBuilder.ToString());
		}
		if (!flag4 && _000E)
		{
			value = string.Intern(value);
			_0002_0015.m__0002[_0002] = value;
			if (_0002_0015.m__0002.Count == ((num + -382752342) ^ num2))
			{
				lock (_0002_0015.m__0002)
				{
					if (_0002_0015.m__000E != null)
					{
						_0002_0015.m__000E._0002();
						_0002_0015.m__000E = null;
						_0003 = null;
						_0005 = null;
					}
				}
			}
		}
		return value;
	}

	private static AssemblyName _0002(Assembly _0002)
	{
		try
		{
			return _0002.GetName();
		}
		catch
		{
			return new AssemblyName(_0002.FullName);
		}
	}

	private static byte[] _0002(AssemblyName _0002)
	{
		byte[] array = _0002.GetPublicKeyToken();
		if (array != null && array.Length == 0)
		{
			array = null;
		}
		return array;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static bool _0002(StackTrace _0002, int _000E)
	{
		Assembly assembly = _0002.GetFrame(++_000E)?.GetMethod()?.DeclaringType?.Assembly;
		if (assembly != null)
		{
			AssemblyName assemblyName = _0002_0015._0002(assembly);
			byte[] array = _0002_0015._0002(assemblyName);
			if (array != null && array.Length == 8 && array[0] == 183 && array[7] == 137)
			{
				return true;
			}
		}
		while (true)
		{
			StackFrame frame = _0002.GetFrame(++_000E);
			if (frame == null)
			{
				break;
			}
			assembly = frame.GetMethod()?.DeclaringType?.Assembly;
			if (assembly != null && assembly == typeof(_0002_0015).Assembly)
			{
				return true;
			}
		}
		return false;
	}

	private static void _0002(byte[] _0002, int _000E, byte[] _0003)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 128;
		int num4 = _0003.Length;
		while (num < num4)
		{
			if ((num3 <<= 1) == 256)
			{
				num3 = 1;
				num2 = _0002[_000E++];
			}
			if ((num2 & num3) != 0)
			{
				int num5 = (_0002[_000E] >> 2) + 3;
				int num6 = ((_0002[_000E] << 8) | _0002[_000E + 1]) & 0x3FF;
				_000E += 2;
				int num7 = num - num6;
				if (num7 < 0)
				{
					break;
				}
				while (--num5 >= 0 && num < num4)
				{
					_0003[num++] = _0003[num7++];
				}
			}
			else
			{
				_0003[num++] = _0002[_000E++];
			}
		}
	}
}
