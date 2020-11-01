using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace ARMaxWrapper
{
    // support for the following:
    // 0. List save contents (1 argument)
    // 1. Extract contents to folder ()
    // 2. Add/Replace file in save 
    // 3. Delete file from Save

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void  Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }
            
            string maxFile = args[0];
            if (File.Exists(maxFile))
            {
                string userOption = "";
                if (args.Length > 1)
                {
                    if (args[1].StartsWith("-a:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        userOption = args[1].Substring(3);
                        AddFileToSave(maxFile, userOption);
                    }
                    else if (args[1].StartsWith("-d:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        userOption = args[1].Substring(3);
                        RemoveFileFromSave(maxFile, userOption);
                    }
                    else if (args[1].StartsWith("-e:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        userOption = args[1].Substring(3);
                        ExtractSaveContents(maxFile, userOption);
                    }
                    else if (args[1].StartsWith("-l", StringComparison.InvariantCultureIgnoreCase))
                        ListContents(maxFile);
                    else
                        PrintHelp();
                }
                else
                    PrintHelp();
            }
            else
            {
                Console.WriteLine("Error! File '{0}' does not exist", maxFile);
                PrintHelp();
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine(
@"ARMaxWrapper demonstration program
Usage:
    ARMaxWrapper.exe <max filename> <options>
Options:
    -h            print help message
    -l            List save file contents
    -a:<file>     Add/replace file in save
    -e:<dirname>  Extract all save contents to directory 'dirname'
    -d:<filename> Delete file from save
Examples:
    # List contents:
        ARMaxWrapper.exe mySave.max -l 
    # Extract all save contents to 'Extract_Folder'
        ARMaxWrapper.exe mySave.max -e:Extract_Folder 
    # Replace 'icon.sys' in the save file
        ARMaxWrapper.exe mySave.max -a:.\better_one\icon.sys
    # Add 'junk.txt' to the save file 
        ARMaxWrapper.exe mySave.max -a:.\junk.txt
    # delete 'junk.txt' from the save file 
        ARMaxWrapper.exe mySave.max -d:junk.txt"
                );
        }

        private static void RemoveFileFromSave(string maxFileName, string fileToRemove)
        {
            ARMaxNativeMethods.InitMaxSave();
            ARMaxNativeMethods.LoadSave(maxFileName);

            int result = ARMaxNativeMethods.FileExistsInSavePos(fileToRemove);
            if (result > 0)
            {
                ARMaxNativeMethods.DeleteFileInSave(result);
                ARMaxNativeMethods.SaveMaxFile(maxFileName);
                Console.WriteLine("Removed '{0}' from save file '{1}'", fileToRemove, maxFileName);
            }            
            ARMaxNativeMethods.FreeMaxSave();
        }

        /// <summary>
        /// Adds/replaces file in save.
        /// </summary>
        private static void AddFileToSave(string maxFileName, string fileToAdd)
        {
            if (File.Exists(fileToAdd))
            {
                int result = -1;
                ARMaxNativeMethods.InitMaxSave();
                ARMaxNativeMethods.LoadSave(maxFileName);
                
                int lastSlash = fileToAdd.LastIndexOf('\\') + 1;
                string shortName = fileToAdd.Substring(lastSlash);

                result = ARMaxNativeMethods.FileExistsInSavePos(shortName);
                if (result > 0)
                    result = ARMaxNativeMethods.ReplaceFileInSave(shortName, fileToAdd);
                else
                    result = ARMaxNativeMethods.AddFileToSave(fileToAdd);
                
                ARMaxNativeMethods.SaveMaxFile(maxFileName);
                ARMaxNativeMethods.FreeMaxSave();
                Console.WriteLine("Added file '{0}' to save '{1}'.", shortName, maxFileName);
            }
            else
            {
                Console.WriteLine("File '{0}' does not exist", fileToAdd);
            }
        }

        private static void ExtractSaveContents(string maxFileName, string folderName)
        {
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            if (!folderName.EndsWith("\\"))
                folderName += "\\";

            ARMaxNativeMethods.InitMaxSave();
            ARMaxNativeMethods.LoadSave(maxFileName);
            StringBuilder buff = new StringBuilder(256);
            int result = -1;
            int numFiles = ARMaxNativeMethods.NumberOfFiles();
            for (int i = 1; i <= numFiles; i++) // there is no '0'th file
            {
                try
                {
                    result = ARMaxNativeMethods.ExtractAFile(i, folderName);
                    if (result != 0)
                        Console.Write("'ARMaxNativeMethods.ExtractAFile' Failed; code = {0}", result);
                }
                catch (Exception exc)
                {
                    Console.Error.WriteLine("Error calling 'ARMaxNativeMethods.GetRootDir()' LastError:{0}\n{1}",
                        System.Runtime.InteropServices.Marshal.GetLastWin32Error(), exc.Message );
                }
            }
            ARMaxNativeMethods.FreeMaxSave();
            Console.WriteLine("Extracted contents to folder '{0}'", folderName);
        }

        private static void ListContents(string maxFileName)
        {
            ARMaxNativeMethods.InitMaxSave();
            ARMaxNativeMethods.LoadSave(maxFileName);
            StringBuilder buff = new StringBuilder(256);
            int numFiles = ARMaxNativeMethods.NumberOfFiles();
            int fileSize = -1;
            int result = 0;
            Console.WriteLine("Listing contents of {0}:", maxFileName);
            for (int i = 1; i < numFiles + 1; i++) // there is no 0th file
            {
                try
                {
                    result = ARMaxNativeMethods.FileDetails(i, buff, 256, ref fileSize);
                    Console.WriteLine(buff.ToString());
                }
                catch (Exception exc)
                {
                    Console.Error.WriteLine("Error calling 'ARMaxNativeMethods.FileDetails({0})' LastError:{1}",
                        i, Marshal.GetLastWin32Error());
                }
                buff.Length = 0;
            }
            ARMaxNativeMethods.FreeMaxSave();
        }

    }
}
