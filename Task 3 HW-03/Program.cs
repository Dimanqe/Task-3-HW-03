using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_2_HW_03
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string URL = @"C:\\Users\\dsank\\Desktop\\Test";
            var initialSize = GetDirectorySize(URL);

            if (!Directory.Exists(URL))
            {
                Console.WriteLine("Указанная папка не существует.");
                return;
            }
            try
            {
                Console.WriteLine($"Исходный размер папки: {(GetDirectorySize(URL))}");
                CleanFolder(URL, TimeSpan.FromMinutes(30), out int deletedFilesCount, out long freedSpace);
                Console.WriteLine($"Удалено файлов: {deletedFilesCount}");
                Console.WriteLine($"Освобождено места: {freedSpace} байт");
                Console.WriteLine("Очистка завершена.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Ошибка доступа: у вас нет прав для удаления файлов из этой папки.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        static void CleanFolder(string folderPath, TimeSpan maxAge, out int deletedFilesCount, out long freedSpace)

        {
            deletedFilesCount = 0;
            freedSpace = 0;
            var directoryInfo = new DirectoryInfo(folderPath);
            foreach (var file in directoryInfo.GetFiles())
            {
                if (DateTime.Now - file.LastAccessTime > maxAge)
                {
                    freedSpace = +file.Length;
                    file.Delete();
                    deletedFilesCount++;
                }
            }
            foreach (var subfolder in directoryInfo.GetDirectories())
            {
                CleanFolder(subfolder.FullName, maxAge, out int subfolderDeletedFilesCount, out long subfolderFreedSpace);
                deletedFilesCount += subfolderDeletedFilesCount;
                freedSpace += subfolderFreedSpace;
                if (subfolder.GetFiles().Length == 0 && subfolder.GetDirectories().Length == 0)
                {
                    subfolder.Delete();
                }
            }
        }
        static long GetDirectorySize(string path)
        {
            long size = 0;
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    size += file.Length;
                }
                foreach (DirectoryInfo subDirectory in directoryInfo.GetDirectories())
                {
                    size += GetDirectorySize(subDirectory.FullName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении размера папки: {ex.Message}");
            }
            return size;
        }
    }
}
