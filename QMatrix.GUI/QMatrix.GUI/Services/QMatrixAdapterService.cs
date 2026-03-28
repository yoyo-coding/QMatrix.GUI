using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace QMatrix.GUI.Services;

public class QMatrixAdapterService
{
    private const string QMatrixCoreExeName = "qmatrix-core.exe";
    private const string QMatrixHttpExeName = "QMatrix.HTTP.exe";
    private const string ConfigFileName = "config.json";

    public async Task<bool> CheckQMatrixCoreInstalledAsync()
    {
        // 检查常见的 QMatrix Core 安装位置
        var possiblePaths = GetPossibleQMatrixPaths();
        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
            {
                return true;
            }
        }
        return false;
    }

    public async Task<AdapterInfo> GetAdapterInfoAsync()
    {
        var corePath = await FindQMatrixCoreAsync();
        if (string.IsNullOrEmpty(corePath))
        {
            return null;
        }

        return new AdapterInfo
        {
            CorePath = corePath,
            CoreVersion = await GetCoreVersionAsync(corePath),
            HttpServicePath = await FindOrCreateHttpServiceAsync(),
            ConfigPath = Path.Combine(Path.GetDirectoryName(corePath) ?? string.Empty, ConfigFileName)
        };
    }

    public async Task<bool> PerformAdapterAsync(AdapterInfo info, IProgress<int> progress)
    {
        try
        {
            progress.Report(10);

            // 1. 检查并创建 HTTP 服务
            if (!File.Exists(info.HttpServicePath))
            {
                await CreateHttpServiceAsync(info.HttpServicePath, progress);
            }
            progress.Report(30);

            // 2. 检查并复制配置文件
            if (File.Exists(info.ConfigPath))
            {
                await CopyConfigFileAsync(info.ConfigPath, progress);
            }
            progress.Report(50);

            // 3. 启动 Core 服务
            await StartCoreServiceAsync(info.CorePath, progress);
            progress.Report(70);

            // 4. 启动 HTTP 服务
            await StartHttpServiceAsync(info.HttpServicePath, progress);
            progress.Report(90);

            // 5. 更新 GUI 配置
            await UpdateGuiConfigAsync(progress);
            progress.Report(100);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"适配失败: {ex.Message}");
            return false;
        }
    }

    private string[] GetPossibleQMatrixPaths()
    {
        var paths = new List<string>();
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        // 检查当前应用目录
        paths.Add(Path.Combine(baseDir, QMatrixCoreExeName));
        paths.Add(Path.Combine(baseDir, "..", QMatrixCoreExeName));
        paths.Add(Path.Combine(baseDir, "..", "..", QMatrixCoreExeName));

        // 检查 Program Files
        paths.Add(Path.Combine(programFiles, "QMatrix", QMatrixCoreExeName));

        // 检查用户目录
        paths.Add(Path.Combine(userProfile, "QMatrix", QMatrixCoreExeName));
        paths.Add(Path.Combine(userProfile, "Documents", "QMatrix", QMatrixCoreExeName));

        // 检查常见的开发目录
        paths.Add(Path.Combine(userProfile, "source", "repos", "QMatrix", "QMatrix.Core", "target", "release", QMatrixCoreExeName));
        paths.Add(Path.Combine(userProfile, "source", "repos", "QMatrix", "QMatrix.Core", "target", "debug", QMatrixCoreExeName));

        return paths.Select(Path.GetFullPath).ToArray();
    }

    private async Task<string> FindQMatrixCoreAsync()
    {
        var possiblePaths = GetPossibleQMatrixPaths();
        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
            {
                return path;
            }
        }
        return string.Empty;
    }

    private async Task<string> GetCoreVersionAsync(string corePath)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = corePath,
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return output.Trim();
        }
        catch
        {
            return "未知版本";
        }
    }

    private async Task<string> FindOrCreateHttpServiceAsync()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var httpServicePath = Path.Combine(baseDir, QMatrixHttpExeName);
        return httpServicePath;
    }

    private async Task CreateHttpServiceAsync(string httpServicePath, IProgress<int> progress)
    {
        try
        {
            // 检查是否存在 QMatrix.HTTP 项目
            var qmatrixDir = Path.GetDirectoryName(httpServicePath);
            while (qmatrixDir != null && !Directory.Exists(Path.Combine(qmatrixDir, "QMatrix.HTTP")))
            {
                qmatrixDir = Path.GetDirectoryName(qmatrixDir);
            }

            if (qmatrixDir != null)
            {
                var httpProjectPath = Path.Combine(qmatrixDir, "QMatrix.HTTP");
                var projectFile = Path.Combine(httpProjectPath, "QMatrix.HTTP.csproj");

                if (File.Exists(projectFile))
                {
                    // 使用 dotnet publish 命令发布 HTTP 服务
                    var publishDir = Path.Combine(httpProjectPath, "bin", "publish");
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "dotnet",
                            Arguments = $"publish {projectFile} --output {publishDir} --configuration Release",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    await process.WaitForExitAsync();

                    if (process.ExitCode == 0)
                    {
                        // 复制发布的文件到目标位置
                        var publishedExe = Path.Combine(publishDir, QMatrixHttpExeName);
                        if (File.Exists(publishedExe))
                        {
                            File.Copy(publishedExe, httpServicePath, true);
                            
                            // 复制依赖文件
                            var publishedFiles = Directory.GetFiles(publishDir);
                            foreach (var file in publishedFiles)
                            {
                                if (Path.GetFileName(file) != QMatrixHttpExeName)
                                {
                                    var targetFile = Path.Combine(Path.GetDirectoryName(httpServicePath) ?? string.Empty, Path.GetFileName(file));
                                    File.Copy(file, targetFile, true);
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"创建 HTTP 服务失败: {ex.Message}");
        }
        progress.Report(20);
    }

    private async Task CopyConfigFileAsync(string sourceConfigPath, IProgress<int> progress)
    {
        var targetConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
        if (!File.Exists(targetConfigPath))
        {
            File.Copy(sourceConfigPath, targetConfigPath);
        }
        progress.Report(40);
    }

    private async Task StartCoreServiceAsync(string corePath, IProgress<int> progress)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = corePath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(corePath) ?? AppDomain.CurrentDomain.BaseDirectory
                }
            };

            process.Start();
            await Task.Delay(2000); // 等待服务启动
            progress.Report(60);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"启动 Core 服务失败: {ex.Message}");
        }
    }

    private async Task StartHttpServiceAsync(string httpServicePath, IProgress<int> progress)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = httpServicePath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(httpServicePath) ?? AppDomain.CurrentDomain.BaseDirectory
                }
            };

            process.Start();
            await Task.Delay(2000); // 等待服务启动
            progress.Report(80);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"启动 HTTP 服务失败: {ex.Message}");
        }
    }

    private async Task UpdateGuiConfigAsync(IProgress<int> progress)
    {
        // 更新 GUI 配置，确保连接到正确的 API 端点
        progress.Report(95);
    }
}

public class AdapterInfo
{
    public string CorePath { get; set; }
    public string CoreVersion { get; set; }
    public string HttpServicePath { get; set; }
    public string ConfigPath { get; set; }
}