// Decompiled with JetBrains decompiler
// Type: LAIR.IO.FileLineReader
// Assembly: LAIR.IO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C0BD8F14-3EDA-432D-BDE1-EA5EF25889F6
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.IO.dll

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace LAIR.IO
{
  /// <summary>Reads lines from a file</summary>
  public abstract class FileLineReader : IEnumerable<string>, IEnumerable
  {
    private string _path;

    /// <summary>Gets the path to the file to read</summary>
    public string Path
    {
      get
      {
        return this._path;
      }
    }

    /// <summary>Gets the number of lines in the file</summary>
    public abstract int Count { get; }

    /// <summary>Constructor</summary>
    /// <param name="path">Path to file to count</param>
    protected FileLineReader(string path)
    {
      this._path = path;
      if (!System.IO.File.Exists(this._path))
        throw new FileNotFoundException("Invalid file:  " + this._path);
    }

    /// <summary>Closes this reader</summary>
    public abstract void Close();

    /// <summary>Gets an enumerator over lines in the file</summary>
    /// <returns>Enumerator over lines</returns>
    public abstract IEnumerator<string> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
