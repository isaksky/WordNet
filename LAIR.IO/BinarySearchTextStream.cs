// Decompiled with JetBrains decompiler
// Type: LAIR.IO.BinarySearchTextStream
// Assembly: LAIR.IO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C0BD8F14-3EDA-432D-BDE1-EA5EF25889F6
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.IO.dll

using LAIR.Extensions;
using System;
using System.IO;

namespace LAIR.IO
{
  /// <summary>
  /// Represents a stream of text, the lines of which can be searched with a binary search algorithm. Any type of stream can be used
  /// allowing searches on-disk (e.g., using FileStream) or in-memory (e.g., using MemoryStream).
  /// </summary>
  public class BinarySearchTextStream : SearchTextStream
  {
    private BinarySearchTextStream.SearchComparisonDelegate _searchComparison;

    /// <summary>Constructor</summary>
    /// <param name="stream">Stream to search</param>
    /// <param name="searchComparison">Search comparison function</param>
    public BinarySearchTextStream(Stream stream, BinarySearchTextStream.SearchComparisonDelegate searchComparison)
      : base(stream)
    {
      this._searchComparison = searchComparison;
    }

    /// <summary>Constructor</summary>
    /// <param name="path">Path to file for which to create a search stream</param>
    /// <param name="searchComparison">Search comparison function</param>
    public BinarySearchTextStream(string path, BinarySearchTextStream.SearchComparisonDelegate searchComparison)
      : this((Stream) new FileStream(path, FileMode.Open, FileAccess.Read), searchComparison)
    {
    }

    /// <summary>Searches a range in the stream</summary>
    /// <param name="key">Search key</param>
    /// <param name="start">Start byte position of search</param>
    /// <param name="end">End byte position of search</param>
    /// <returns>Desired line, or null if none was found</returns>
    public override string Search(object key, long start, long end)
    {
      this.CheckSearchRange(start, end);
      while (start <= end)
      {
        this.Stream.BaseStream.Position = (long) ((double) (start + end) / 2.0);
        int num1 = 0;
        while (this.Stream.BaseStream.Position > 0L)
        {
          int num2;
          if ((num2 = this.Stream.BaseStream.ReadByte()) == -1)
            throw new Exception("Failed to read byte");
          char ch = (char) num2;
          if (++num1 <= 1 || (int) ch != 10)
            this.Stream.BaseStream.Position -= 2L;
          else
            break;
        }
        long position1 = this.Stream.BaseStream.Position;
        uint position2 = (uint) position1;
        if ((long) position2 != position1)
          throw new Exception("uint overflow");
        this.Stream.DiscardBufferedData();
        string currentLine = this.Stream.ReadLine(ref position2);
        --position2;
        int num3 = this._searchComparison(key, currentLine);
        if (num3 == 0)
          return currentLine;
        if (num3 < 0)
          end = position1 - 1L;
        else if (num3 > 0)
          start = (long) (position2 + 1U);
      }
      return (string) null;
    }

    /// <summary>
    /// Delegate for functions that direct the search by comparing the search key to the current line. The return value
    /// of such a function should be -1 if the search key (first parameter) comes before the current line (second
    /// parameter), 1 if the search key comes after the current line, and 0 if the current line is the desired line.
    /// </summary>
    /// <param name="key">Search key</param>
    /// <param name="currentLine">Current line in the stream</param>
    /// <returns>Described in summary</returns>
    public delegate int SearchComparisonDelegate(object key, string currentLine);
  }
}
