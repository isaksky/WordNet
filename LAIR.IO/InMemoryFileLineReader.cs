// Decompiled with JetBrains decompiler
// Type: LAIR.IO.InMemoryFileLineReader
// Assembly: LAIR.IO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C0BD8F14-3EDA-432D-BDE1-EA5EF25889F6
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.IO.dll

using LAIR.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace LAIR.IO
{
  /// <summary>
  /// Reads lines from a file after storing them in memory. Can be used more than once without reinitializing.
  /// </summary>
  public class InMemoryFileLineReader : FileLineReader
  {
    private string[] _lines;

    /// <summary>Gets the number of lines in the file</summary>
    public override int Count
    {
      get
      {
        return this._lines.Length;
      }
    }

    /// <summary>Constructor</summary>
    /// <param name="path">Path to file to count</param>
    /// <param name="allowBlankLines">Whether or not to allow blank lines</param>
    /// <param name="trimLines">Whether or not to trim lines when counting. Can only be true if allowBlank is false.</param>
    public InMemoryFileLineReader(string path, bool allowBlankLines = true, bool trimLines = false)
      : base(path)
    {
      this._lines = new string[File.GetNumberOfLines(this.Path, allowBlankLines, trimLines)];
      StreamReader reader = new StreamReader(this.Path);
      int num = 0;
      string line;
      while (reader.TryReadLine(out line))
        this._lines[num++] = trimLines ? line.Trim() : line;
      if (num != this._lines.Length)
        throw new Exception("Line count mismatch when reading \"" + this.Path + "\"");
    }

    /// <summary>Closes this reader and releases all memory</summary>
    public override void Close()
    {
      this._lines = (string[]) null;
    }

    /// <summary>Gets an enumerator over lines in this reader</summary>
    /// <returns>Enumerator over lines in this reader</returns>
    public override IEnumerator<string> GetEnumerator()
    {
      return ((IEnumerable<string>) this._lines).GetEnumerator();
    }
  }
}
