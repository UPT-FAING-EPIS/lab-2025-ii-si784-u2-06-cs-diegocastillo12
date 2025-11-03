using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UPTSiteTests
{
    [TestClass]
    public class PlaywrightInstaller
    {
        // Runs once before any tests in the assembly. Executes the generated playwright.ps1
        // installer script located in the test assembly output directory.
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            try
            {
                var assemblyPath = typeof(PlaywrightInstaller).Assembly.Location;
                var assemblyDir = Path.GetDirectoryName(assemblyPath) ?? AppContext.BaseDirectory;
                var scriptPath = Path.Combine(assemblyDir, "playwright.ps1");

                if (!File.Exists(scriptPath))
                {
                    // No installer script found; nothing to do.
                    return;
                }

                string runner;
                string args;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    runner = "powershell";
                    args = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\" install";
                }
                else
                {
                    runner = "pwsh";
                    args = $"-NoProfile -File \"{scriptPath}\" install";
                }

                var psi = new ProcessStartInfo(runner, args)
                {
                    WorkingDirectory = assemblyDir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                };

                using var proc = Process.Start(psi);
                if (proc == null)
                    throw new InvalidOperationException($"Failed to start process '{runner}' to install Playwright browsers.");

                string stdout = proc.StandardOutput.ReadToEnd();
                string stderr = proc.StandardError.ReadToEnd();
                proc.WaitForExit();

                if (proc.ExitCode != 0)
                {
                    throw new InvalidOperationException($"Playwright install failed (exit {proc.ExitCode}).\nstdout:\n{stdout}\nstderr:\n{stderr}");
                }
            }
            catch (Exception ex)
            {
                // Surface installer errors early so autograder logs contain the reason.
                throw new InvalidOperationException("Failed to run Playwright installer in AssemblyInit", ex);
            }
        }
    }
}
