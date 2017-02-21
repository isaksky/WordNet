// Decompiled with JetBrains decompiler
// Type: LAIR.IO.OnDiskFileLineReader
// Assembly: LAIR.IO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C0BD8F14-3EDA-432D-BDE1-EA5EF25889F6
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.IO.dll

using System.Collections.Generic;

namespace LAIR.IO
{
  /// <summary>
  /// Reads lines from a file without storing them in memory. Can be used more than once without reinitializing.
  /// </summary>
  public class OnDiskFileLineReader : FileLineReader
  {
    /// <summary>Gets the number of lines in the file</summary>
    public override int Count
    {
      get
      {
        int num = 0;
        foreach (string str in (FileLineReader) this)
          ++num;
        return num;
      }
    }

    /// <summary>Constructor</summary>
    /// <param name="path">Path to file to count</param>
    public OnDiskFileLineReader(string path)
      : base(path)
    {
    }

    /// <summary>Closes the file read by this reader</summary>
    public override void Close()
    {
    }

    /// <summary>Gets an enumerator over lines in the file</summary>
    /// <returns>Enumerator over lines in the file</returns>
    public override IEnumerator<string> GetEnumerator()
    {
      foreach (string readLine in System.IO.File.ReadLines(this.Path))
        yield return readLine;
    }
  }
}
