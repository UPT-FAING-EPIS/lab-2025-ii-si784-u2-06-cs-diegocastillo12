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

                // Try pwsh first (preferred on modern runners). If not available or fails,
                // fall back to powershell (Windows). Capture outputs from each attempt.
                var attempts = new (string runner, string args)[]
                {
                    ("pwsh", $"-NoProfile -File \"{scriptPath}\" install"),
                    ("powershell", $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\" install")
                };

                Exception lastEx = null;
                var logFile = Path.Combine(assemblyDir, "playwright-install.log");
                File.AppendAllText(logFile, $"Playwright installer started at {DateTime.UtcNow:O}\n");
                for (int i = 0; i < attempts.Length; i++)
                {
                    var (runnerCmd, runnerArgs) = attempts[i];
                    try
                    {
                        File.AppendAllText(logFile, $"Attempt {i + 1}: runner={runnerCmd}, args={runnerArgs}\n");
                        Console.WriteLine($"[PlaywrightInstaller] Attempt {i + 1}: starting '{runnerCmd}' {runnerArgs}");

                        var psi = new ProcessStartInfo(runnerCmd, runnerArgs)
                        {
                            WorkingDirectory = assemblyDir,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                        };

                        using var proc = Process.Start(psi);
                        if (proc == null)
                        {
                            var msg = $"Failed to start process '{runnerCmd}' to install Playwright browsers.";
                            File.AppendAllText(logFile, msg + "\n");
                            throw new InvalidOperationException(msg);
                        }

                        string stdout = proc.StandardOutput.ReadToEnd();
                        string stderr = proc.StandardError.ReadToEnd();
                        proc.WaitForExit();

                        File.AppendAllText(logFile, $"ExitCode={proc.ExitCode}\nstdout:\n{stdout}\nstderr:\n{stderr}\n");
                        Console.WriteLine($"[PlaywrightInstaller] {runnerCmd} exited {proc.ExitCode}");

                        if (proc.ExitCode == 0)
                        {
                            File.AppendAllText(logFile, "Playwright install succeeded.\n");
                            Console.WriteLine("[PlaywrightInstaller] Playwright install succeeded");
                            // Success — stop attempting further runners.
                            return;
                        }

                        // Non-zero exit code: record and try next.
                        lastEx = new InvalidOperationException($"Playwright install failed with '{runnerCmd}' (exit {proc.ExitCode}).\nstdout:\n{stdout}\nstderr:\n{stderr}");
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(logFile, $"Attempt {i + 1} exception: {ex}\n");
                        Console.WriteLine($"[PlaywrightInstaller] Attempt {i + 1} exception: {ex.Message}");
                        // Keep the exception and try next fallback.
                        lastEx = ex;
                        continue;
                    }
                }

                // If we reach here, all attempts failed — throw the last observed exception with context.
                throw new InvalidOperationException("All attempts to run the Playwright installer failed.", lastEx);
            }
            catch (Exception ex)
            {
                // Surface installer errors early so autograder logs contain the reason.
                throw new InvalidOperationException("Failed to run Playwright installer in AssemblyInit", ex);
            }
        }
    }
}
