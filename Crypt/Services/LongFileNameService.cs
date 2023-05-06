// Long file name handling library freely made available to the general public on Stackoverflow by user:
// https://stackoverflow.com/users/37643/wolf5
// as a response to this question:
// https://stackoverflow.com/questions/5188527/how-to-deal-with-files-with-a-name-longer-than-259-characters
//


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

internal static class LongFile
{
    private const int MAX_PATH = 260;

    internal static bool Exists(string path)
    {
        if (path.Length < MAX_PATH) return System.IO.File.Exists(path);
        var attr = NativeMethods.GetFileAttributesW(GetWin32LongPath(path));
        return (attr != NativeMethods.INVALID_FILE_ATTRIBUTES && ((attr & NativeMethods.FILE_ATTRIBUTE_ARCHIVE) == NativeMethods.FILE_ATTRIBUTE_ARCHIVE));
    }

    internal static void Delete(string path)
    {
        if (path.Length < MAX_PATH) System.IO.File.Delete(path);
        else
        {
            bool ok = NativeMethods.DeleteFileW(GetWin32LongPath(path));
            if (!ok) ThrowWin32Exception();
        }
    }

    internal static void AppendAllText(string path, string contents)
    {
        AppendAllText(path, contents, Encoding.Default);
    }

    internal static void AppendAllText(string path, string contents, Encoding encoding)
    {
        if (path.Length < MAX_PATH)
        {
            System.IO.File.AppendAllText(path, contents, encoding);
        }
        else
        {
            var fileHandle = CreateFileForAppend(GetWin32LongPath(path));
            using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Write))
            {
                var bytes = encoding.GetBytes(contents);
                fs.Position = fs.Length;
                fs.Write(bytes, 0, bytes.Length);
            }
        }
    }

    internal static void WriteAllText(string path, string contents)
    {
        WriteAllText(path, contents, Encoding.Default);
    }

    internal static void WriteAllText(string path, string contents, Encoding encoding)
    {
        if (path.Length < MAX_PATH)
        {
            System.IO.File.WriteAllText(path, contents, encoding);
        }
        else
        {
            var fileHandle = CreateFileForWrite(GetWin32LongPath(path));

            using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Write))
            {
                var bytes = encoding.GetBytes(contents);
                fs.Write(bytes, 0, bytes.Length);
            }
        }
    }

    internal static void WriteAllBytes(string path, byte[] bytes)
    {
        if (path.Length < MAX_PATH)
        {
            System.IO.File.WriteAllBytes(path, bytes);
        }
        else
        {
            var fileHandle = CreateFileForWrite(GetWin32LongPath(path));

            using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Write))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }
    }

    internal static void WriteBytes(string path, long position, byte[] bytes)
    {
        if (path.Length < MAX_PATH)
        {
            using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                fs.Position = position;
                fs.Write(bytes, 0, bytes.Length);
            }
        }
        else
        {
            var fileHandle = CreateFileForAppend(GetWin32LongPath(path));

            using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Write))
            {
                fs.Position = position;
                fs.Write(bytes, 0, bytes.Length);
            }
        }
    }

    internal static void Copy(string sourceFileName, string destFileName)
    {
        Copy(sourceFileName, destFileName, false);
    }

    internal static void Copy(string sourceFileName, string destFileName, bool overwrite)
    {
        if (sourceFileName.Length < MAX_PATH && (destFileName.Length < MAX_PATH)) System.IO.File.Copy(sourceFileName, destFileName, overwrite);
        else
        {
            var ok = NativeMethods.CopyFileW(GetWin32LongPath(sourceFileName), GetWin32LongPath(destFileName), !overwrite);
            if (!ok) ThrowWin32Exception();
        }
    }

    internal static void Move(string sourceFileName, string destFileName)
    {
        if (sourceFileName.Length < MAX_PATH && (destFileName.Length < MAX_PATH)) System.IO.File.Move(sourceFileName, destFileName);
        else
        {
            var ok = NativeMethods.MoveFileW(GetWin32LongPath(sourceFileName), GetWin32LongPath(destFileName));
            if (!ok) ThrowWin32Exception();
        }
    }

    internal static string ReadAllText(string path)
    {
        return ReadAllText(path, Encoding.Default);
    }

    internal static string ReadAllText(string path, Encoding encoding)
    {
        if (path.Length < MAX_PATH) { return System.IO.File.ReadAllText(path, encoding); }
        var fileHandle = GetFileHandle(GetWin32LongPath(path));

        using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Read))
        {
            var data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            return encoding.GetString(data);
        }
    }

    internal static string[] ReadAllLines(string path)
    {
        return ReadAllLines(path, Encoding.Default);
    }

    internal static string[] ReadAllLines(string path, Encoding encoding)
    {
        if (path.Length < MAX_PATH) { return System.IO.File.ReadAllLines(path, encoding); }
        var fileHandle = GetFileHandle(GetWin32LongPath(path));

        using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Read))
        {
            var data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            var str = encoding.GetString(data);
            if (str.Contains("\r")) return str.Split(new[] { "\r\n" }, StringSplitOptions.None);
            return str.Split('\n');
        }
    }

    internal static byte[] ReadAllBytes(string path)
    {
        if (path.Length < MAX_PATH) return System.IO.File.ReadAllBytes(path);
        var fileHandle = GetFileHandle(GetWin32LongPath(path));

        using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Read))
        {
            var data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            return data;
        }
    }

    internal static byte[] ReadBytes(string path, long position, int length)
    {
        if (path.Length < MAX_PATH)
        {
            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var data = new byte[length];
                fs.Position = position;
                fs.Read(data, 0, length);
                return data;
            }
        }
        else
        {
            var fileHandle = GetFileHandle(GetWin32LongPath(path));

            using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Read))
            {
                var data = new byte[length];
                fs.Position = position;
                fs.Read(data, 0, length);
                return data;
            }
        }
    }

    internal static FileAttributes GetAttributes(string path)
    {
        if (path.Length < MAX_PATH)
        {
            return System.IO.File.GetAttributes(path);
        }
        else
        {
            var longFilename = GetWin32LongPath(path);
            return (FileAttributes)NativeMethods.GetFileAttributesW(path);
        }
    }

    internal static void SetAttributes(string path, FileAttributes attributes)
    {
        if (path.Length < MAX_PATH)
        {
            System.IO.File.SetAttributes(path, attributes);
        }
        else
        {
            var longFilename = GetWin32LongPath(path);
            NativeMethods.SetFileAttributesW(longFilename, (int)attributes);
        }
    }

    #region Helper methods

    private static SafeFileHandle CreateFileForWrite(string filename)
    {
        if (filename.Length >= MAX_PATH) filename = GetWin32LongPath(filename);
        SafeFileHandle hfile = NativeMethods.CreateFile(filename, (int)NativeMethods.FILE_GENERIC_WRITE, NativeMethods.FILE_SHARE_NONE, IntPtr.Zero, NativeMethods.CREATE_ALWAYS, 0, IntPtr.Zero);
        if (hfile.IsInvalid) ThrowWin32Exception();
        return hfile;
    }

    internal static SafeFileHandle CreateFileForAppend(string filename)
    {
        if (filename.Length >= MAX_PATH) filename = GetWin32LongPath(filename);
        SafeFileHandle hfile = NativeMethods.CreateFile(filename, (int)NativeMethods.FILE_GENERIC_WRITE, NativeMethods.FILE_SHARE_NONE, IntPtr.Zero, NativeMethods.CREATE_NEW, 0, IntPtr.Zero);
        if (hfile.IsInvalid)
        {
            hfile = NativeMethods.CreateFile(filename, (int)NativeMethods.FILE_GENERIC_WRITE, NativeMethods.FILE_SHARE_NONE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
            if (hfile.IsInvalid) ThrowWin32Exception();
        }
        return hfile;
    }

    internal static SafeFileHandle GetFileHandle(string filename)
    {
        if (filename.Length >= MAX_PATH) filename = GetWin32LongPath(filename);
        SafeFileHandle hfile = NativeMethods.CreateFile(filename, (int)NativeMethods.FILE_GENERIC_READ, NativeMethods.FILE_SHARE_READ, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
        if (hfile.IsInvalid) ThrowWin32Exception();
        return hfile;
    }

    internal static SafeFileHandle GetFileHandleWithWrite(string filename)
    {
        if (filename.Length >= MAX_PATH) filename = GetWin32LongPath(filename);
        SafeFileHandle hfile = NativeMethods.CreateFile(filename, (int)(NativeMethods.FILE_GENERIC_READ | NativeMethods.FILE_GENERIC_WRITE | NativeMethods.FILE_WRITE_ATTRIBUTES), NativeMethods.FILE_SHARE_NONE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
        if (hfile.IsInvalid) ThrowWin32Exception();
        return hfile;
    }

    internal static System.IO.FileStream GetFileStream(string filename, FileAccess access = FileAccess.Read)
    {
        var longFilename = GetWin32LongPath(filename);
        SafeFileHandle hfile;
        if (access == FileAccess.Write)
        {
            hfile = NativeMethods.CreateFile(longFilename, (int)(NativeMethods.FILE_GENERIC_READ | NativeMethods.FILE_GENERIC_WRITE | NativeMethods.FILE_WRITE_ATTRIBUTES), NativeMethods.FILE_SHARE_NONE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
        }
        else
        {
            hfile = NativeMethods.CreateFile(longFilename, (int)NativeMethods.FILE_GENERIC_READ, NativeMethods.FILE_SHARE_READ, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
        }

        if (hfile.IsInvalid) ThrowWin32Exception();

        return new System.IO.FileStream(hfile, access);
    }

    [DebuggerStepThrough]
    internal static void ThrowWin32Exception()
    {
        int code = Marshal.GetLastWin32Error();
        if (code != 0)
        {
            throw new System.ComponentModel.Win32Exception(code);
        }
    }

    internal static string GetWin32LongPath(string path)
    {
        if (path.StartsWith(@"\\?\")) return path;

        if (path.StartsWith("\\"))
        {
            path = @"\\?\UNC\" + path.Substring(2);
        }
        else if (path.Contains(":"))
        {
            path = @"\\?\" + path;
        }
        else
        {
            var currdir = Environment.CurrentDirectory;
            path = Combine(currdir, path);
            while (path.Contains("\\.\\")) path = path.Replace("\\.\\", "\\");
            path = @"\\?\" + path;
        }
        return path.TrimEnd('.'); ;
    }

    private static string Combine(string path1, string path2)
    {
        return path1.TrimEnd('\\') + "\\" + path2.TrimStart('\\').TrimEnd('.'); ;
    }

    #endregion

    internal static void SetCreationTime(string path, DateTime creationTime)
    {
        long cTime = 0;
        long aTime = 0;
        long wTime = 0;

        using (var handle = GetFileHandleWithWrite(path))
        {
            NativeMethods.GetFileTime(handle, ref cTime, ref aTime, ref wTime);
            var fileTime = creationTime.ToFileTimeUtc();
            if (!NativeMethods.SetFileTime(handle, ref fileTime, ref aTime, ref wTime))
            {
                throw new Win32Exception();
            }
        }
    }

    internal static void SetLastAccessTime(string path, DateTime lastAccessTime)
    {
        long cTime = 0;
        long aTime = 0;
        long wTime = 0;

        using (var handle = GetFileHandleWithWrite(path))
        {
            NativeMethods.GetFileTime(handle, ref cTime, ref aTime, ref wTime);

            var fileTime = lastAccessTime.ToFileTimeUtc();
            if (!NativeMethods.SetFileTime(handle, ref cTime, ref fileTime, ref wTime))
            {
                throw new Win32Exception();
            }
        }
    }

    internal static void SetLastWriteTime(string path, DateTime lastWriteTime)
    {
        long cTime = 0;
        long aTime = 0;
        long wTime = 0;

        using (var handle = GetFileHandleWithWrite(path))
        {
            NativeMethods.GetFileTime(handle, ref cTime, ref aTime, ref wTime);

            var fileTime = lastWriteTime.ToFileTimeUtc();
            if (!NativeMethods.SetFileTime(handle, ref cTime, ref aTime, ref fileTime))
            {
                throw new Win32Exception();
            }
        }
    }

    internal static DateTime GetLastWriteTime(string path)
    {
        long cTime = 0;
        long aTime = 0;
        long wTime = 0;

        using (var handle = GetFileHandleWithWrite(path))
        {
            NativeMethods.GetFileTime(handle, ref cTime, ref aTime, ref wTime);

            return DateTime.FromFileTimeUtc(wTime);
        }
    }

}

internal class LongDirectory
{
    private const int MAX_PATH = 248;

    internal static void CreateDirectory(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        if (path.Length < MAX_PATH)
        {
            System.IO.Directory.CreateDirectory(path);
        }
        else
        {
            var paths = GetAllPathsFromPath(GetWin32LongPath(path));
            foreach (var item in paths)
            {
                if (!LongExists(item))
                {
                    var ok = NativeMethods.CreateDirectory(item, IntPtr.Zero);
                    if (!ok)
                    {
                        ThrowWin32Exception();
                    }
                }
            }
        }
    }

    internal static void Delete(string path)
    {
        Delete(path, false);
    }

    internal static void Delete(string path, bool recursive)
    {
        if (path.Length < MAX_PATH && !recursive)
        {
            System.IO.Directory.Delete(path, false);
        }
        else
        {
            if (!recursive)
            {
                bool ok = NativeMethods.RemoveDirectory(GetWin32LongPath(path));
                if (!ok) ThrowWin32Exception();
            }
            else
            {
                DeleteDirectories(new string[] { GetWin32LongPath(path) });
            }
        }
    }

    private static void DeleteDirectories(string[] directories)
    {
        foreach (string directory in directories)
        {
            string[] files = LongDirectory.GetFiles(directory, null, System.IO.SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                LongFile.Delete(file);
            }
            directories = LongDirectory.GetDirectories(directory, null, System.IO.SearchOption.TopDirectoryOnly);
            DeleteDirectories(directories);
            bool ok = NativeMethods.RemoveDirectory(GetWin32LongPath(directory));
            if (!ok) ThrowWin32Exception();
        }
    }

    internal static bool Exists(string path)
    {
        if (path.Length < MAX_PATH) return System.IO.Directory.Exists(path);
        return LongExists(GetWin32LongPath(path));
    }

    private static bool LongExists(string path)
    {
        var attr = NativeMethods.GetFileAttributesW(path);
        return (attr != NativeMethods.INVALID_FILE_ATTRIBUTES && ((attr & NativeMethods.FILE_ATTRIBUTE_DIRECTORY) == NativeMethods.FILE_ATTRIBUTE_DIRECTORY));
    }


    internal static string[] GetDirectories(string path)
    {
        return GetDirectories(path, null, SearchOption.TopDirectoryOnly);
    }

    internal static string[] GetDirectories(string path, string searchPattern)
    {
        return GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
    }

    internal static string[] GetDirectories(string path, string searchPattern, System.IO.SearchOption searchOption)
    {
        searchPattern = searchPattern ?? "*";
        var dirs = new List<string>();
        internalGetDirectories(path, searchPattern, searchOption, ref dirs);
        return dirs.ToArray();
    }

    private static void internalGetDirectories(string path, string searchPattern, System.IO.SearchOption searchOption, ref List<string> dirs)
    {
        NativeMethods.WIN32_FIND_DATA findData;
        IntPtr findHandle = NativeMethods.FindFirstFile(System.IO.Path.Combine(GetWin32LongPath(path), searchPattern), out findData);

        try
        {
            if (findHandle != new IntPtr(-1))
            {

                do
                {
                    if ((findData.dwFileAttributes & System.IO.FileAttributes.Directory) != 0)
                    {
                        if (findData.cFileName != "." && findData.cFileName != "..")
                        {
                            string subdirectory = System.IO.Path.Combine(path, findData.cFileName);
                            dirs.Add(GetCleanPath(subdirectory));
                            if (searchOption == SearchOption.AllDirectories)
                            {
                                internalGetDirectories(subdirectory, searchPattern, searchOption, ref dirs);
                            }
                        }
                    }
                } while (NativeMethods.FindNextFile(findHandle, out findData));
                NativeMethods.FindClose(findHandle);
            }
            else
            {
                ThrowWin32Exception();
            }
        }
        catch (Exception)
        {
            NativeMethods.FindClose(findHandle);
            throw;
        }
    }

    internal static string[] GetFiles(string path)
    {
        return GetFiles(path, null, SearchOption.TopDirectoryOnly);
    }

    internal static string[] GetFiles(string path, string searchPattern)
    {
        return GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
    }

    internal static string[] GetFiles(string path, string searchPattern, System.IO.SearchOption searchOption)
    {
        searchPattern = searchPattern ?? "*";

        var files = new List<string>();
        var dirs = new List<string> { path };

        if (searchOption == SearchOption.AllDirectories)
        {
            //Add all the subpaths
            dirs.AddRange(LongDirectory.GetDirectories(path, null, SearchOption.AllDirectories));
        }

        foreach (var dir in dirs)
        {
            NativeMethods.WIN32_FIND_DATA findData;
            IntPtr findHandle = NativeMethods.FindFirstFile(System.IO.Path.Combine(GetWin32LongPath(dir), searchPattern), out findData);

            try
            {
                if (findHandle != new IntPtr(-1))
                {

                    do
                    {
                        if ((findData.dwFileAttributes & System.IO.FileAttributes.Directory) == 0)
                        {
                            string filename = System.IO.Path.Combine(dir, findData.cFileName);
                            files.Add(GetCleanPath(filename));
                        }
                    } while (NativeMethods.FindNextFile(findHandle, out findData));
                    NativeMethods.FindClose(findHandle);
                }
            }
            catch (Exception)
            {
                NativeMethods.FindClose(findHandle);
                throw;
            }
        }

        return files.ToArray();
    }

    internal static void Move(string sourceDirName, string destDirName)
    {
        if (sourceDirName.Length < MAX_PATH || destDirName.Length < MAX_PATH)
        {
            System.IO.Directory.Move(sourceDirName, destDirName);
        }
        else
        {
            var ok = NativeMethods.MoveFileW(GetWin32LongPath(sourceDirName), GetWin32LongPath(destDirName));
            if (!ok) ThrowWin32Exception();
        }
    }

    internal static string TrimPath(string path)
    {
        if (path.Replace(":\\", "").Length == 1)
            return path;

        return path.Trim('\\');
    }

    internal static string GetDirectoryName(string path)
    {
        var index = path.LastIndexOf("\\");
        if (index != -1) return path.Substring(0, index);
        else return String.Empty;
    }

    internal static string GetJustDirectoryName(string path)
    {
        var index = path.LastIndexOf("\\");
        if (index != -1) return TrimPath(path.Substring(index));
        else return String.Empty;
    }

    internal static bool IsDirectoryEmpty(string path)
    {
        if (LongDirectory.Exists(path))
        {
            var searchPattern = "*";

            NativeMethods.WIN32_FIND_DATA findData;
            IntPtr findHandle = NativeMethods.FindFirstFile(System.IO.Path.Combine(GetWin32LongPath(path), searchPattern), out findData);

            try
            {
                if (findHandle != new IntPtr(-1))
                {
                    do
                    {
                        if ((findData.dwFileAttributes & System.IO.FileAttributes.Directory) == 0)
                            return false;

                        if ((findData.dwFileAttributes & System.IO.FileAttributes.Directory) != 0)
                            if (findData.cFileName != "." && findData.cFileName != "..")
                            {
                                return false;
                            }
                    } while (NativeMethods.FindNextFile(findHandle, out findData));

                    NativeMethods.FindClose(findHandle);
                }
            }
            catch (Exception)
            {
                NativeMethods.FindClose(findHandle);
                throw;
            }

            return true;
        }
        else throw new ArgumentException("Path is not directory.");
    }

    #region Helper methods

    [DebuggerStepThrough]
    internal static void ThrowWin32Exception()
    {
        int code = Marshal.GetLastWin32Error();
        if (code != 0)
        {
            throw new System.ComponentModel.Win32Exception(code);
        }
    }

    internal static string GetWin32LongPath(string path)
    {

        if (path.StartsWith(@"\\?\")) return path;

        var newpath = path;
        if (newpath.StartsWith("\\"))
        {
            newpath = @"\\?\UNC\" + newpath.Substring(2);
        }
        else if (newpath.Contains(":"))
        {
            newpath = @"\\?\" + newpath;
        }
        else
        {
            var currdir = Environment.CurrentDirectory;
            newpath = Combine(currdir, newpath);
            while (newpath.Contains("\\.\\")) newpath = newpath.Replace("\\.\\", "\\");
            newpath = @"\\?\" + newpath;
        }
        return newpath.TrimEnd('.');
    }

    internal static string GetCleanPath(string path)
    {
        if (path.StartsWith(@"\\?\UNC\")) return @"\\" + path.Substring(8);
        if (path.StartsWith(@"\\?\")) return path.Substring(4);
        return path;
    }

    private static List<string> GetAllPathsFromPath(string path)
    {
        bool unc = false;
        var prefix = @"\\?\";
        if (path.StartsWith(prefix + @"UNC\"))
        {
            prefix += @"UNC\";
            unc = true;
        }
        var split = path.Split('\\');
        int i = unc ? 6 : 4;
        var list = new List<string>();
        var txt = "";

        for (int a = 0; a < i; a++)
        {
            if (a > 0) txt += "\\";
            txt += split[a];
        }
        for (; i < split.Length; i++)
        {
            txt = Combine(txt, split[i]);
            list.Add(txt);
        }

        return list;
    }

    internal static string Combine(string path1, string path2)
    {
        return path1.TrimEnd('\\') + "\\" + path2.TrimStart('\\').TrimEnd('.');
    }

    #endregion
}

internal static class NativeMethods
{
    internal const int FILE_ATTRIBUTE_ARCHIVE = 0x20;
    internal const int INVALID_FILE_ATTRIBUTES = -1;

    internal const int FILE_READ_DATA = 0x0001;
    internal const int FILE_WRITE_DATA = 0x0002;
    internal const int FILE_APPEND_DATA = 0x0004;
    internal const int FILE_READ_EA = 0x0008;
    internal const int FILE_WRITE_EA = 0x0010;

    internal const int FILE_READ_ATTRIBUTES = 0x0080;
    internal const int FILE_WRITE_ATTRIBUTES = 0x0100;

    internal const int FILE_SHARE_NONE = 0x00000000;
    internal const int FILE_SHARE_READ = 0x00000001;

    internal const int FILE_ATTRIBUTE_READONLY = 0x1;
    internal const int FILE_ATTRIBUTE_HIDDEN = 0x2;
    internal const int FILE_ATTRIBUTE_DIRECTORY = 0x10;

    internal const long FILE_GENERIC_WRITE = STANDARD_RIGHTS_WRITE |
                                                FILE_WRITE_DATA |
                                                FILE_WRITE_ATTRIBUTES |
                                                FILE_WRITE_EA |
                                                FILE_APPEND_DATA |
                                                SYNCHRONIZE;

    internal const long FILE_GENERIC_READ = STANDARD_RIGHTS_READ |
                                            FILE_READ_DATA |
                                            FILE_READ_ATTRIBUTES |
                                            FILE_READ_EA |
                                            SYNCHRONIZE;

    internal const long READ_CONTROL = 0x00020000L;
    internal const long STANDARD_RIGHTS_READ = READ_CONTROL;
    internal const long STANDARD_RIGHTS_WRITE = READ_CONTROL;

    internal const long SYNCHRONIZE = 0x00100000L;

    internal const int CREATE_NEW = 1;
    internal const int CREATE_ALWAYS = 2;
    internal const int OPEN_EXISTING = 3;

    internal const int MAX_PATH = 260;
    internal const int MAX_ALTERNATE = 14;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WIN32_FIND_DATA
    {
        internal System.IO.FileAttributes dwFileAttributes;
        internal System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
        internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
        internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
        internal uint nFileSizeHigh; //changed all to uint, otherwise you run into unexpected overflow
        internal uint nFileSizeLow;  //|
        internal uint dwReserved0;   //|
        internal uint dwReserved1;   //v
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
        internal string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ALTERNATE)]
        internal string cAlternate;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool CopyFileW(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int GetFileAttributesW(string lpFileName);


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool DeleteFileW(string lpFileName);


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool MoveFileW(string lpExistingFileName, string lpNewFileName);


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool SetFileTime(SafeFileHandle hFile, ref long lpCreationTime, ref long lpLastAccessTime, ref long lpLastWriteTime);


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool GetFileTime(SafeFileHandle hFile, ref long lpCreationTime, ref long lpLastAccessTime, ref long lpLastWriteTime);


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool FindClose(IntPtr hFindFile);


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool RemoveDirectory(string path);


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool CreateDirectory(string lpPathName, IntPtr lpSecurityAttributes);


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int SetFileAttributesW(string lpFileName, int fileAttributes);
}