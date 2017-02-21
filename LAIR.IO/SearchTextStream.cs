// Decompiled with JetBrains decompiler
// Type: LAIR.IO.SearchTextStream
// Assembly: LAIR.IO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C0BD8F14-3EDA-432D-BDE1-EA5EF25889F6
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.IO.dll

using System;
using System.IO;

namespace LAIR.IO
{
  /// <summary>
  /// Represents a text stream, the lines of which can be searched.
  /// </summary>
  public abstract class SearchTextStream
  {
    private StreamReader _stream;

    /// <summary>Gets the stream searched by this instance</summary>
    public StreamReader Stream
    {
      get
      {
        return this._stream;
      }
    }

    /// <summary>Constructor</summary>
    /// <param name="stream">Stream to search</param>
    protected SearchTextStream(System.IO.Stream stream)
    {
      this._stream = new StreamReader(stream);
    }

    /// <summary>Searches for a key</summary>
    /// <param name="key">Key to search for</param>
    /// <returns>Line matching key, or null if key was not found</returns>
    public string Search(object key)
    {
      if (this._stream.BaseStream.Length == 0L)
        return (string) null;
      return this.Search(key, 0L, this._stream.BaseStream.Length - 1L);
    }

    /// <summary>Searches for a key within a specified byte range</summary>
    /// <param name="key">Key to search for</param>
    /// <param name="start">Start of byte range</param>
    /// <param name="end">End of byte range</param>
    /// <returns>Line matching key</returns>
    public abstract string Search(object key, long start, long end);

    /// <summary>Checks the search range</summary>
    /// <param name="start">Start of range</param>
    /// <param name="end">End of range</param>
    protected void CheckSearchRange(long start, long end)
    {
      if (start < 0L)
        throw new ArgumentOutOfRangeException("start", "Start byte position must be non-negative");
      if (end >= this.Stream.BaseStream.Length)
        throw new ArgumentOutOfRangeException("end", "End byte position must be less than the length of the stream");
      if (start > end)
        throw new ArgumentOutOfRangeException("start", "Start byte position must be less than or equal to end byte position");
    }

    /// <summary>Closes this search stream and releases all resources</summary>
    public virtual void Close()
    {
      if (this._stream == null)
        return;
      this._stream.Close();
      this._stream = (StreamReader) null;
    }

    /// <summary>Re-initializes this search stream with a new stream</summary>
    /// <param name="stream">Stream to initialize with</param>
    public virtual void ReInitialize(System.IO.Stream stream)
    {
      this.Close();
      this._stream = new StreamReader(stream);
    }
  }
}
