using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: dotnet run <inputDirectory> <outputDirectory> <frameRate>");
            return;
        }

        string inputDirectory = args[0];
        string outputDirectory = args[1];
        string frameRate = args[2];

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        foreach (string filePath in Directory.GetFiles(inputDirectory, "*.mp4"))
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string movieOutputDirectory = Path.Combine(outputDirectory, fileName);

            if (!Directory.Exists(movieOutputDirectory))
            {
                Directory.CreateDirectory(movieOutputDirectory);
            }

            ExtractFrames(filePath, movieOutputDirectory, frameRate);
        }
    }

    static void ExtractFrames(string inputFilePath, string outputDirectory, string frameRate)
    {
        (int width, int height) = GetVideoResolution(inputFilePath);

        if (width < 1024 || height < 1024)
        {
            outputDirectory = Path.Combine(outputDirectory, "low_resolution");
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
        }

        string ffmpegPath = GetFFmpegPath();
        string arguments = $"-i \"{inputFilePath}\" -vf \"fps={frameRate},scale=1024:1024:force_original_aspect_ratio=decrease\" \"{outputDirectory}/frame_%08d.png\"";

        ProcessStartInfo processStartInfo = new(ffmpegPath, arguments)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process process = new();
        process.StartInfo = processStartInfo;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            Console.WriteLine($"Error extracting frames: {error}");
        }
        else
        {
            Console.WriteLine(output);
        }
    }

    static (int width, int height) GetVideoResolution(string inputFilePath)
    {
        string ffprobePath = GetFFprobePath();
        string arguments = $"-v error -select_streams v:0 -show_entries stream=width,height -of csv=s=x:p=0 \"{inputFilePath}\"";

        ProcessStartInfo processStartInfo = new(ffprobePath, arguments)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process process = new();
        process.StartInfo = processStartInfo;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Error getting video resolution: {process.StandardError.ReadToEnd()}");
        }

        string[] dimensions = output.Trim().Split('x');
        return (int.Parse(dimensions[0]), int.Parse(dimensions[1]));
    }

    static string GetFFmpegPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return @"path\to\ffmpeg.exe";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "/usr/bin/ffmpeg"; // Adjust this path if ffmpeg is installed elsewhere
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported OS platform");
        }
    }

    static string GetFFprobePath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return @"path\to\ffprobe.exe";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "/usr/bin/ffprobe"; // Adjust this path if ffprobe is installed elsewhere
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported OS platform");
        }
    }
}
