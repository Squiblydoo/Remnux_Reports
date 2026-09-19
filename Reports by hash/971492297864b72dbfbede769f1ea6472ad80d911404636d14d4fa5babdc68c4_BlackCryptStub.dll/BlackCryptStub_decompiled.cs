using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: AssemblyTitle("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCompany("BlackCrypt Labs Inc")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
[assembly: TargetFramework(".NETCoreApp,Version=v8.0", FrameworkDisplayName = ".NET 8.0")]
[assembly: AssemblyVersion("1.0.0.0")]
[module: RefSafetyRules(11)]
internal static class Program
{
	private const string ServerUrlPlaceholder = "https://blackcryptknight.com";

	private static readonly string LogPath = Path.Combine(Path.GetTempPath(), "BlackCrypt-stub.log");

	private static void Log(string message)
	{
		try
		{
			File.AppendAllText(LogPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}{Environment.NewLine}");
		}
		catch
		{
		}
	}

	private static string DetectOsArchitecture()
	{
		return RuntimeInformation.OSArchitecture switch
		{
			Architecture.X64 => "x64", 
			Architecture.X86 => "x86", 
			Architecture.Arm64 => "arm64", 
			_ => "x64", 
		};
	}

	private static HttpClient CreateHttpClient()
	{
		return new HttpClient(new HttpClientHandler
		{
			AllowAutoRedirect = true,
			UseProxy = true,
			DefaultProxyCredentials = CredentialCache.DefaultCredentials,
			AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate)
		})
		{
			Timeout = TimeSpan.FromMinutes(30.0),
			DefaultRequestHeaders = { { "User-Agent", "BlackCryptStub/2.0" } }
		};
	}

	private static async Task<int> Main()
	{
		_ = 7;
		try
		{
			Log("Stub started.");
			string serverUrl = "https://blackcryptknight.com".TrimEnd('/');
			if (string.IsNullOrWhiteSpace(serverUrl) || serverUrl.Contains("%%"))
			{
				Fail("This installer was not configured with a server address. Please contact your administrator.");
				return 2;
			}
			Match match = Regex.Match(Path.GetFileName(Environment.ProcessPath ?? string.Empty), "_([A-Za-z0-9]{6})\\.exe$", RegexOptions.IgnoreCase);
			if (!match.Success)
			{
				Fail("This installer's filename is missing its enrollment code. Please do not rename the file.");
				return 3;
			}
			string code = match.Groups[1].Value.ToUpperInvariant();
			string arch = DetectOsArchitecture();
			Log($"Installer code {code}, detected OS architecture {arch}, server {serverUrl}");
			using HttpClient http = CreateHttpClient();
			string resolveJson = string.Empty;
			HttpStatusCode? resolveStatus = null;
			for (int attempt = 1; attempt <= 4; attempt++)
			{
				try
				{
					using (HttpResponseMessage resp = await http.GetAsync(serverUrl + "/api/build/distribution/" + code + "/resolve"))
					{
						resolveStatus = resp.StatusCode;
						if (resp.IsSuccessStatusCode)
						{
							resolveJson = await resp.Content.ReadAsStringAsync();
							break;
						}
						Log($"Resolve attempt {attempt} returned {resp.StatusCode}.");
						if (resp.StatusCode == HttpStatusCode.NotFound || resp.StatusCode == HttpStatusCode.Gone)
						{
							Fail("This installer code is no longer valid or has expired. Please request a new installer from your administrator.");
							return 5;
						}
					}
					goto IL_040e;
				}
				catch (Exception ex)
				{
					Log($"Resolve attempt {attempt} threw: {ex.Message}");
					goto IL_040e;
				}
				IL_040e:
				await Task.Delay(TimeSpan.FromSeconds(Math.Min(8, attempt * 2)));
			}
			if (string.IsNullOrWhiteSpace(resolveJson))
			{
				Fail($"Could not reach the BlackCrypt server ({(resolveStatus.HasValue ? ((int)resolveStatus.Value).ToString() : "no response")}). Please check this computer's internet/proxy connection and try again. Details were written to {LogPath}.");
				return 4;
			}
			string effectiveServer = serverUrl;
			string buildId;
			string path;
			using (JsonDocument jsonDocument = JsonDocument.Parse(resolveJson))
			{
				JsonElement rootElement = jsonDocument.RootElement;
				buildId = (rootElement.TryGetProperty("buildId", out var value) ? (value.GetString() ?? string.Empty) : string.Empty);
				path = (rootElement.TryGetProperty("outputFilename", out var value2) ? (value2.GetString() ?? ("BlackcryptAgent_" + code + ".exe")) : ("BlackcryptAgent_" + code + ".exe"));
				if (rootElement.TryGetProperty("serverUrl", out var value3) && !string.IsNullOrWhiteSpace(value3.GetString()))
				{
					effectiveServer = value3.GetString().TrimEnd('/');
				}
			}
			if (string.IsNullOrWhiteSpace(buildId))
			{
				Fail("This installer code is no longer valid. Please request a new installer from your administrator.");
				return 5;
			}
			string text = Path.GetFileName(path);
			if (string.IsNullOrWhiteSpace(text) || !Regex.IsMatch(text, "_" + Regex.Escape(code) + "\\.exe$", RegexOptions.IgnoreCase))
			{
				text = "BlackcryptAgent_" + code + ".exe";
			}
			string stagingDir = Path.Combine(Path.GetTempPath(), "BlackCryptStub", Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(stagingDir);
			string outPath = Path.Combine(stagingDir, text);
			bool downloaded = false;
			HttpStatusCode? downloadStatus = null;
			string lastError = string.Empty;
			for (int attempt = 1; attempt <= 4; attempt++)
			{
				if (downloaded)
				{
					break;
				}
				try
				{
					string requestUri = $"{effectiveServer}/api/build/{buildId}/download?asset=exe&arch={arch}";
					using HttpResponseMessage resp = await http.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);
					downloadStatus = resp.StatusCode;
					if (!resp.IsSuccessStatusCode)
					{
						Log($"Download attempt {attempt} returned {resp.StatusCode}.");
						if (resp.StatusCode == HttpStatusCode.NotFound || resp.StatusCode == HttpStatusCode.Gone)
						{
							Fail("The agent for this installer is not available on the server yet (build missing). Please ask your administrator to publish/rebuild the release, then run this installer again.");
							return 7;
						}
						await Task.Delay(TimeSpan.FromSeconds(Math.Min(8, attempt * 2)));
						continue;
					}
					await using (FileStream fs = File.Create(outPath))
					{
						await resp.Content.CopyToAsync(fs);
					}
					if (new FileInfo(outPath).Length > 0)
					{
						downloaded = true;
						continue;
					}
					lastError = "Downloaded file was empty.";
					Log(lastError);
				}
				catch (Exception ex2)
				{
					lastError = ex2.Message;
					Log($"Download attempt {attempt} threw: {ex2.Message}");
					await Task.Delay(TimeSpan.FromSeconds(Math.Min(8, attempt * 2)));
				}
			}
			if (!downloaded)
			{
				Fail($"Failed to download the BlackCrypt agent ({(downloadStatus.HasValue ? ((int)downloadStatus.Value).ToString() : "network error")}). {lastError} Please check this computer's internet/proxy/antivirus settings and try again. Details were written to {LogPath}.");
				try
				{
					Directory.Delete(stagingDir, recursive: true);
				}
				catch
				{
				}
				return 6;
			}
			Log("Agent downloaded to " + outPath + ". Launching.");
			using Process process = Process.Start(new ProcessStartInfo
			{
				FileName = outPath,
				UseShellExecute = true
			});
			process?.WaitForExit();
			try
			{
				Directory.Delete(stagingDir, recursive: true);
			}
			catch
			{
			}
			Log("Stub finished successfully.");
			return 0;
		}
		catch (Exception ex3)
		{
			Log($"Fatal: {ex3}");
			Fail("Installation failed: " + ex3.Message);
			return 1;
		}
	}

	private static void Fail(string message)
	{
		Log("FAIL: " + message);
		string caption = "Setup";
		try
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? string.Empty);
			if (!string.IsNullOrWhiteSpace(fileNameWithoutExtension))
			{
				caption = fileNameWithoutExtension;
			}
		}
		catch
		{
		}
		try
		{
			MessageBox(IntPtr.Zero, message, caption, 16u);
		}
		catch
		{
		}
	}

	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	private static extern int MessageBox(nint hWnd, string text, string caption, uint type);
}
You are not using the latest version of the tool, please update.
Latest version is '11.0.0.9375' (yours is '9.1.0.7988')
