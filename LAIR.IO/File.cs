// Decompiled with JetBrains decompiler
// Type: LAIR.IO.File
// Assembly: LAIR.IO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C0BD8F14-3EDA-432D-BDE1-EA5EF25889F6
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.IO.dll

using LAIR.Extensions;
using System;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace LAIR.IO
{
  /// <summary>Provides additional filesystem routines</summary>
  public class File
  {
    /// <summary>Gets the number of lines in a file</summary>
    /// <param name="path">Path to file to count</param>
    /// <param name="allowBlankLines">Whether or not to allow blank lines</param>
    /// <param name="trimLines">Whether or not to trim lines when counting</param>
    /// <returns>Number of lines in file</returns>
    public static int GetNumberOfLines(string path, bool allowBlankLines = true, bool trimLines = false)
    {
      StreamReader reader = new StreamReader(path);
      int num = 0;
      string line;
      while (reader.TryReadLine(out line))
      {
        if (!allowBlankLines && (line == "" || trimLines && line.Trim() == ""))
          throw new Exception("Unexpected blank line in file \"" + path + "\"");
        ++num;
      }
      return num;
    }

    /// <summary>
    /// Gets the line ending used for a file - either "\r", "\n", or "\r\n", or "" if no line ending is found
    /// </summary>
    /// <param name="path">Path to file</param>
    /// <returns>Line ending</returns>
    public static string GetLineEnding(string path)
    {
      FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
      char[] chArray = new char[2];
      while ((int) chArray[0] != 13 && (int) chArray[0] != 10 && fileStream.Position < fileStream.Length)
      {
        chArray[0] = chArray[1];
        chArray[1] = (char) fileStream.ReadByte();
      }
      fileStream.Close();
      if ((int) chArray[0] != 13 && (int) chArray[0] != 10)
      {
        chArray[0] = chArray[1];
        chArray[1] = char.MinValue;
      }
      string str = "";
      if ((int) chArray[0] == 13)
        str = (int) chArray[1] != 10 ? "\r" : "\r\n";
      else if ((int) chArray[0] == 10)
        str = "\n";
      return str;
    }

    /// <summary>Prompts the user for a path to open</summary>
    /// <param name="windowTitle">Title of window</param>
    /// <returns>Selected path, or null for no file</returns>
    public static string PromptForOpenPath(string windowTitle)
    {
      return File.PromptForOpenPath(windowTitle, System.IO.Directory.GetDirectoryRoot("."));
    }

    /// <summary>Prompts the user for a path to open</summary>
    /// <param name="windowTitle">Title of window</param>
    /// <param name="initialDirectory">Initial directory</param>
    /// <returns>Selected path, or null for no file</returns>
    public static string PromptForOpenPath(string windowTitle, string initialDirectory)
    {
      return File.PromptForOpenPath(windowTitle, initialDirectory, (string) null);
    }

    /// <summary>Prompts the user for a path to open</summary>
    /// <param name="windowTitle">Title of window</param>
    /// <param name="initialDirectory">Initial directory</param>
    /// <param name="filter">File filter to display</param>
    /// <returns>Selected path, or null for no file</returns>
    public static string PromptForOpenPath(string windowTitle, string initialDirectory, string filter)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Title = windowTitle;
      openFileDialog.InitialDirectory = initialDirectory;
      openFileDialog.Filter = filter;
      if (openFileDialog.ShowDialog() == DialogResult.OK && System.IO.File.Exists(openFileDialog.FileName))
        return openFileDialog.FileName;
      return (string) null;
    }

    /// <summary>Prompts the user for a path to save to</summary>
    /// <param name="windowTitle">Title of window</param>
    /// <returns>Selected path, or null for no file</returns>
    public static string PromptForSavePath(string windowTitle)
    {
      return File.PromptForSavePath(windowTitle, (string) null);
    }

    /// <summary>Prompts the user for a path to save to</summary>
    /// <param name="windowTitle">Title of window</param>
    /// <param name="filter">File type filter to use, or null for no filter</param>
    /// <returns>Selected path, or null for no file</returns>
    public static string PromptForSavePath(string windowTitle, string filter)
    {
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.Title = windowTitle;
      saveFileDialog.Filter = filter;
      if (filter != null)
        saveFileDialog.AddExtension = true;
      if (saveFileDialog.ShowDialog() == DialogResult.OK)
        return saveFileDialog.FileName;
      return (string) null;
    }

    /// <summary>Samples lines from a file</summary>
    /// <param name="inputPath">Path to input file</param>
    /// <param name="outputPath">Path to output file</param>
    /// <param name="percentage">Percentage to sample. Sampling is done randomly, so the exact number of sampled lines might differ slightly for small sample sizes.</param>
    public static void SampleLines(string inputPath, string outputPath, float percentage)
    {
      StreamWriter streamWriter = new StreamWriter(outputPath);
      Random random = new Random();
      foreach (string readLine in System.IO.File.ReadLines(inputPath))
      {
        if (random.NextDouble() < (double) percentage)
          streamWriter.WriteLine(readLine);
      }
      streamWriter.Close();
    }

    /// <summary>Compresses a file using the gzip algorithm</summary>
    /// <param name="inputPath">Path to input file</param>
    /// <param name="outputPath">Path to output file (can be same as input file)</param>
    /// <param name="overwriteOutputPath">Whether or not to overwrite the output file if it already exists</param>
    public static void Compress(string inputPath, string outputPath, bool overwriteOutputPath)
    {
      string tempFileName = Path.GetTempFileName();
      GZipStream gzipStream = new GZipStream((Stream) new FileStream(tempFileName, FileMode.Create, FileAccess.Write), CompressionMode.Compress);
      byte[] buffer = new byte[1048576];
      FileStream fileStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
      int count;
      while ((count = fileStream.Read(buffer, 0, buffer.Length)) > 0)
        gzipStream.Write(buffer, 0, count);
      gzipStream.Close();
      fileStream.Close();
      if (System.IO.File.Exists(outputPath))
      {
        if (!overwriteOutputPath)
          throw new IOException("File already exists at \"" + outputPath + "\" and it will not be overwritten. Cannot write compressed output there.");
        System.IO.File.Delete(outputPath);
      }
      System.IO.File.Move(tempFileName, outputPath);
    }

    /// <summary>Decompresses a file using the gzip algorithm</summary>
    /// <param name="inputPath">Path to input file</param>
    /// <param name="outputPath">Path to output file (can be same as input file)</param>
    /// <param name="overwriteOutputPath">Whether or not to overwrite the output file if it already exists</param>
    public static void Decompress(string inputPath, string outputPath, bool overwriteOutputPath)
    {
      string tempFileName = Path.GetTempFileName();
      StreamWriter streamWriter = new StreamWriter(tempFileName);
      StreamReader streamReader = new StreamReader((Stream) new GZipStream((Stream) new FileStream(inputPath, FileMode.Open, FileAccess.Read), CompressionMode.Decompress));
      string str;
      while ((str = streamReader.ReadLine()) != null)
        streamWriter.WriteLine(str);
      streamReader.Close();
      streamWriter.Close();
      if (System.IO.File.Exists(outputPath))
      {
        if (!overwriteOutputPath)
          throw new IOException("File already exists at \"" + outputPath + "\" and it will not be overwritten. Cannot write decompressed output there.");
        System.IO.File.Delete(outputPath);
      }
      System.IO.File.Move(tempFileName, outputPath);
    }
  }
}
