// Decompiled with JetBrains decompiler
// Type: LAIR.IO.LineSearchTextStream
// Assembly: LAIR.IO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C0BD8F14-3EDA-432D-BDE1-EA5EF25889F6
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.IO.dll

using LAIR.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace LAIR.IO
{
  /// <summary>Provides line-based access to a text stream</summary>
  public class LineSearchTextStream : SearchTextStream
  {
    private Dictionary<int, long> _linePosition;

    /// <summary>Gets the number of lines in the text stream</summary>
    public int Count
    {
      get
      {
        return this._linePosition.Count;
      }
    }

    /// <summary>Constructor</summary>
    /// <param name="path">Path to file to search</param>
    /// <param name="linePositionCachePath">Path to file that stores the line position cache. Computing line positions can be a time consuming
    /// process. The good news is that it only needs to be done once and can then be stored. This file stores the line positions.</param>
    public LineSearchTextStream(string path, string linePositionCachePath)
      : base((Stream) new FileStream(path, FileMode.Open, FileAccess.Read))
    {
      if (System.IO.File.Exists(linePositionCachePath))
      {
        StreamReader streamReader = new StreamReader(linePositionCachePath);
        int capacity = int.Parse(streamReader.ReadLine());
        this._linePosition = new Dictionary<int, long>(capacity);
        for (int index = 0; index < capacity; ++index)
        {
          string str = streamReader.ReadLine();
          int length = str.IndexOf(' ');
          this._linePosition.Add(int.Parse(str.Substring(0, length)), long.Parse(str.Substring(length + 1)));
        }
        if (streamReader.ReadLine() != null)
          throw new Exception("Extra lines in cache file");
        streamReader.Close();
      }
      else
      {
        int capacity = 0;
        this.Stream.SetPosition(0L);
        while (this.Stream.ReadLine() != null)
          ++capacity;
        this._linePosition = new Dictionary<int, long>(capacity);
        this.Stream.SetPosition(0L);
        int key1 = 0;
        long num = 0;
        string lineEnding = File.GetLineEnding(path);
        string str;
        while ((str = this.Stream.ReadLine()) != null)
        {
          this._linePosition.Add(key1, num);
          ++key1;
          num += (long) this.Stream.CurrentEncoding.GetByteCount(str + lineEnding);
        }
        if (this._linePosition.Count != capacity)
          throw new Exception("Line count mismatch");
        if (linePositionCachePath == null)
          return;
        StreamWriter streamWriter = new StreamWriter(linePositionCachePath);
        streamWriter.WriteLine(this._linePosition.Count);
        foreach (int key2 in this._linePosition.Keys)
          streamWriter.WriteLine(key2.ToString() + " " + (object) this._linePosition[key2]);
        streamWriter.Close();
      }
    }

    /// <summary>Constructor</summary>
    /// <param name="path">Path to file to search</param>
    public LineSearchTextStream(string path)
      : this(path, (string) null)
    {
    }

    /// <summary>Searches for a line</summary>
    /// <param name="key">Line to search for</param>
    /// <param name="start">Start byte</param>
    /// <param name="end">End byte</param>
    /// <returns>Line</returns>
    public override string Search(object key, long start, long end)
    {
      long position;
      if (!this._linePosition.TryGetValue((int) key, out position))
        throw new Exception("Invalid line for search:  " + key);
      lock (this.Stream)
      {
        this.Stream.SetPosition(position);
        return this.Stream.ReadLine();
      }
    }
  }
}
