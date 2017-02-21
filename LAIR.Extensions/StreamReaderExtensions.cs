// Decompiled with JetBrains decompiler
// Type: LAIR.Extensions.StreamReaderExtensions
// Assembly: LAIR.Extensions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 647167B9-7037-4B5C-8B74-626D8FC76D80
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.Extensions.dll

using System;
using System.IO;

namespace LAIR.Extensions
{
  /// <summary>Extensions for the StreamReader class</summary>
  public static class StreamReaderExtensions
  {
    /// <summary>
    /// Reads a line from a reader, tracking the stream position.
    /// </summary>
    /// <param name="reader">Reader to read from</param>
    /// <param name="position">Position of next read operation, passed in as the current reader position.</param>
    /// <returns>Line</returns>
    public static string ReadLine(this StreamReader reader, ref uint position)
    {
      string str = reader.ReadLine();
      if (str == null)
        return str;
      uint num = position;
      position += (uint) reader.CurrentEncoding.GetByteCount(str + Environment.NewLine);
      if (position < num)
        throw new Exception("Reader position wrapped around");
      return str;
    }

    /// <summary>
    /// Tries to read a line from a reader. After the last line is read, this function will close the given reader. This is
    /// handy because it lets you write while loops such as while(file.ReadLine(out line)){}.
    /// </summary>
    /// <param name="reader">Reader to read from</param>
    /// <param name="line">Line that was read, or null if no line was read.</param>
    /// <returns>True if line was read, false otherwise</returns>
    public static bool TryReadLine(this StreamReader reader, out string line)
    {
      line = reader.ReadLine();
      if (line == null)
        reader.Close();
      return line != null;
    }

    /// <summary>
    /// Sets a StreamReader to a given position and discards any buffered data
    /// </summary>
    /// <param name="reader">StreamReader to reset</param>
    /// <param name="position">Position to reset to</param>
    public static void SetPosition(this StreamReader reader, long position)
    {
      reader.BaseStream.Position = position;
      reader.DiscardBufferedData();
    }
  }
}
