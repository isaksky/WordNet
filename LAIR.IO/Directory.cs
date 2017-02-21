// Decompiled with JetBrains decompiler
// Type: LAIR.IO.Directory
// Assembly: LAIR.IO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C0BD8F14-3EDA-432D-BDE1-EA5EF25889F6
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.IO.dll

using LAIR.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LAIR.IO
{
  /// <summary>Provides additional filesystem routines</summary>
  public class Directory
  {
    private static object _getTemporaryDirectoryLock = new object();

    /// <summary>
    /// Gets numbered list of paths in a directory. Each path in given directory must contain exactly one integer.
    /// </summary>
    /// <param name="directory">Path to directory</param>
    /// <param name="pattern">Pattern for files to match</param>
    /// <param name="excludeEndings">File endings to exclude from the returned paths</param>
    /// <returns>List of paths, indexed by number</returns>
    public static Dictionary<int, string> GetNumberedPaths(string directory, string pattern, params string[] excludeEndings)
    {
      if (!System.IO.Directory.Exists(directory))
        throw new DirectoryNotFoundException("Invalid directory");
      Regex regex1 = new Regex("^[^\\d]*(?<index>\\d+)[^\\d]*$");
      Regex regex2 = new Regex(pattern);
      Dictionary<int, string> dictionary = new Dictionary<int, string>();
      foreach (string file in System.IO.Directory.GetFiles(directory))
      {
        bool flag = false;
        foreach (string excludeEnding in excludeEndings)
        {
          if (file.EndsWith(excludeEnding))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          Match match1 = regex1.Match(file);
          Match match2 = regex2.Match(file);
          if (match1.Success && match2.Success)
          {
            int key = int.Parse(match1.Groups["index"].Value);
            dictionary.Add(key, file);
          }
        }
      }
      return dictionary;
    }

    /// <summary>Combines numbered files into a single file</summary>
    /// <param name="directory">Path to directory containing files to combine</param>
    /// <param name="outputPath">Path to output file</param>
    /// <param name="ignoreBlankLines">Whether or not to ignore blank lines</param>
    public static void CombineNumberedFiles(string directory, string outputPath, bool ignoreBlankLines)
    {
      StreamWriter streamWriter = new StreamWriter(outputPath);
      foreach (string sortValuesByKey in Directory.GetNumberedPaths(directory, ".*").SortValuesByKeys<int, string>())
      {
        StreamReader streamReader = new StreamReader(sortValuesByKey);
        string str1;
        while ((str1 = streamReader.ReadLine()) != null)
        {
          string str2 = str1.Trim();
          if (!ignoreBlankLines || !(str2 == ""))
            streamWriter.WriteLine(str2);
        }
      }
      streamWriter.Close();
    }

    /// <summary>Finds a directory</summary>
    /// <param name="directory">Directory to search</param>
    /// <param name="pattern">Pattern to search for</param>
    /// <returns>Directory path</returns>
    public static string FindDirectory(string directory, string pattern)
    {
      string[] directories = System.IO.Directory.GetDirectories(directory, pattern, SearchOption.AllDirectories);
      if (directories.Length > 1)
        throw new Exception("Multiple directories found");
      return directories[0];
    }

    /// <summary>Finds a file</summary>
    /// <param name="directory">Directory to search</param>
    /// <param name="pattern">Pattern to search for</param>
    /// <returns>File path</returns>
    public static string FindFile(string directory, string pattern)
    {
      string[] files = System.IO.Directory.GetFiles(directory, pattern, SearchOption.AllDirectories);
      if (files.Length > 1)
        throw new Exception("Multiple files found");
      return files[0];
    }

    /// <summary>
    /// Gets the path to a new temporary directory, which will be created before this method returns.
    /// </summary>
    /// <param name="parentDirectory">Parent of temporary directory</param>
    /// <returns>Path to temporary directory</returns>
    public static string GetTemporaryDirectory(string parentDirectory = null)
    {
      lock (Directory._getTemporaryDirectoryLock)
      {
        string local_0 = parentDirectory == null ? Path.GetTempPath() : parentDirectory;
        string local_1 = (string) null;
        while (local_1 == null)
        {
          string local_2 = Path.GetTempFileName();
          System.IO.File.Delete(local_2);
          local_1 = Path.Combine(local_0, Path.GetFileName(local_2).RemovePunctuation());
          try
          {
            if (System.IO.Directory.Exists(local_1))
              local_1 = (string) null;
            else
              System.IO.Directory.CreateDirectory(local_1);
          }
          catch (Exception exception_0)
          {
            local_1 = (string) null;
          }
        }
        return local_1;
      }
    }

    /// <summary>Prompts the user for a directory</summary>
    /// <param name="description">Description in browser</param>
    /// <returns>Directory, or null if none</returns>
    public static string PromptForDirectory(string description)
    {
      return Directory.PromptForDirectory(description, (string) null);
    }

    /// <summary>Prompts the user for a directory</summary>
    /// <param name="description">Description in browser</param>
    /// <param name="startDirectory">Starting directory</param>
    /// <returns>Directory, or null if none</returns>
    public static string PromptForDirectory(string description, string startDirectory)
    {
      FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
      if (startDirectory != null && System.IO.Directory.Exists(startDirectory))
        folderBrowserDialog.SelectedPath = startDirectory;
      folderBrowserDialog.Description = description;
      if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        return folderBrowserDialog.SelectedPath;
      return (string) null;
    }
  }
}
