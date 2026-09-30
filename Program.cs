using System.Security.Cryptography;

namespace FolderSynchronization;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length != 4 || !int.TryParse(args[2], out int syncInterval) || syncInterval <= 0)
        {
            Console.WriteLine("Usage: FolderSynchronization <source_folder> <destination_folder> <sync_interval_in_seconds> <log_file_path>");
            return 2;
        }

        string sourceFolder = args[0];
        string backupFolder = args[1];
        string logFilePath = args[3];

        // Check if source folder exists
        if (!Directory.Exists(sourceFolder))
        {
            Console.Error.WriteLine($"Source folder doesn't exists {sourceFolder}");
            return 3;
        }

        // Check if source folder is different from backup one
        if (sourceFolder == backupFolder)
        {
            Console.Error.WriteLine($"Source folder {sourceFolder} and backup folder {backupFolder} are identical");
            return 3;
        }

        Synchronize(sourceFolder, backupFolder);
        return 0;
    }

    static void Synchronize(string sourcePath, string backupPath)
    {
        if (!Directory.Exists(backupPath))
        {
            Directory.CreateDirectory(backupPath);
        }

        // Get paths of all subdirectories in source folder
        string[] sourceDirectories = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)
            .OrderBy(path => Path.GetRelativePath(sourcePath, path).Length)
            .ToArray();

        // Get paths of all files from source folder
        string[] sourceFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
        
        var sourceDirectorySet = new HashSet<string>();

        // Create missing source folders in backup folder
        foreach (string sourceDirectory in sourceDirectories)
        {
            string relativePath = Path.GetRelativePath(sourcePath, sourceDirectory);
            sourceDirectorySet.Add(relativePath);
            string backupDirectory = Path.Combine(backupPath, relativePath);
           
            if (File.Exists(backupDirectory))
            {
                File.Delete(backupDirectory);
            }

            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }
        }

        var sourceFileSet = new HashSet<string>();

        // Copy the files
        foreach(string sourceFile in sourceFiles)
        {
            string relativePath = Path.GetRelativePath(sourcePath, sourceFile);
            sourceFileSet.Add(relativePath);
            string backupFile = Path.Combine(backupPath, relativePath);

            bool existed = File.Exists(backupFile);
            if (existed && FilesAreEqual(backupFile, sourceFile))
            {
                continue;
            }

            File.Copy(sourceFile, backupFile, true);
        }

        // Remove non source files from backup folder
        foreach (string backupFile in Directory.GetFiles(backupPath, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(backupPath, backupFile);
            if (!sourceFileSet.Contains(relativePath))
            {
                File.Delete(backupFile);
            }
        }

        // Remove non source directories from backup folder
        string[] backupDirectories = Directory.GetDirectories(backupPath, "*", SearchOption.AllDirectories)
            .OrderByDescending(path => path.Length)
            .ToArray();
        foreach (string backupDirectory in backupDirectories)
        {
            string relativePath = Path.GetRelativePath(backupPath, backupDirectory);
            if (!sourceDirectorySet.Contains(relativePath))
            {
                Directory.Delete(backupDirectory);
            }
        }
    }

    private static bool FilesAreEqual(string firstPath, string secondPath)
    {
        using FileStream first = File.OpenRead(firstPath);
        using FileStream second = File.OpenRead(secondPath);
        return SHA256.HashData(first).SequenceEqual(SHA256.HashData(second));
    }
}

