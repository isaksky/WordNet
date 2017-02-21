// Decompiled with JetBrains decompiler
// Type: LAIR.IO.HashSearchTextStream
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
  /// Represents a text stream, the lines of which can be searched with a hashing search algorithm. In the hashing search algorithm,
  /// each line is mapped to a hash code. The hash code for a line is mapped to the line's position in the file. To check whether a
  /// line is present, one need only compute the hash of the queried line and read the file at the given position. Because of hash
  /// collisions, one might need to read multiple lines.
  /// </summary>
  public class HashSearchTextStream : SearchTextStream
  {
    private HashSearchTextStream.HashProviderDelegate _hashProvider;
    private HashSearchTextStream.MatchProviderDelegate _matchProvider;
    private Dictionary<int, uint[]> _hashFilePositions;

    /// <summary>Constructor</summary>
    /// <param name="stream">Stream of text to search</param>
    /// <param name="hashProvider">Hash provider</param>
    /// <param name="matchProvider">Match provider</param>
    public HashSearchTextStream(Stream stream, HashSearchTextStream.HashProviderDelegate hashProvider, HashSearchTextStream.MatchProviderDelegate matchProvider)
      : base(stream)
    {
      this._hashProvider = hashProvider;
      this._matchProvider = matchProvider;
      this.Initialize();
    }

    /// <summary>Constructor</summary>
    /// <param name="path">Path to file for which to create a search stream</param>
    /// <param name="hashProvider">Hash provider</param>
    /// <param name="matchProvider">Match provider</param>
    public HashSearchTextStream(string path, HashSearchTextStream.HashProviderDelegate hashProvider, HashSearchTextStream.MatchProviderDelegate matchProvider)
      : this((Stream) new FileStream(path, FileMode.Open), hashProvider, matchProvider)
    {
    }

    /// <summary>Initializes this search using the current stream</summary>
    private void Initialize()
    {
      this.Stream.SetPosition(0L);
      int capacity = 0;
      string str1;
      int hashCode;
      while ((str1 = this.Stream.ReadLine()) != null)
      {
        if (this._hashProvider((object) str1, HashSearchTextStream.HashType.Index, out hashCode))
          ++capacity;
      }
      this._hashFilePositions = new Dictionary<int, uint[]>(capacity);
      this.Stream.SetPosition(0L);
      uint position = 0;
      uint num = position;
      string str2;
      while ((str2 = this.Stream.ReadLine(ref position)) != null)
      {
        if (this._hashProvider((object) str2, HashSearchTextStream.HashType.Index, out hashCode))
        {
          uint[] numArray1;
          if (this._hashFilePositions.TryGetValue(hashCode, out numArray1))
          {
            uint[] numArray2 = new uint[numArray1.Length + 1];
            numArray1.CopyTo((Array) numArray2, 0);
            numArray1 = numArray2;
            this._hashFilePositions[hashCode] = numArray1;
          }
          else
          {
            numArray1 = new uint[1];
            this._hashFilePositions.Add(hashCode, numArray1);
          }
          numArray1[numArray1.Length - 1] = num;
        }
        num = position;
      }
    }

    /// <summary>Re-initializes this search stream with a new stream</summary>
    /// <param name="stream">Stream to initialize with</param>
    public override void ReInitialize(Stream stream)
    {
      base.ReInitialize(stream);
      this.Initialize();
    }

    /// <summary>Searches for a key within a specified byte range</summary>
    /// <param name="key">Key to search for</param>
    /// <param name="start">Start of byte range</param>
    /// <param name="end">End of byte range</param>
    /// <returns>Line matching key</returns>
    public override string Search(object key, long start, long end)
    {
      this.CheckSearchRange(start, end);
      int hashCode;
      if (!this._hashProvider(key, HashSearchTextStream.HashType.Search, out hashCode))
        return (string) null;
      uint[] numArray;
      if (this._hashFilePositions.TryGetValue(hashCode, out numArray))
      {
        foreach (uint num in numArray)
        {
          if ((long) num >= start && (long) num <= end)
          {
            this.Stream.SetPosition((long) num);
            string currentLine = this.Stream.ReadLine();
            if (this._matchProvider(key, currentLine))
              return currentLine;
          }
        }
      }
      return (string) null;
    }

    /// <summary>Closes this search and releases all resources</summary>
    public override void Close()
    {
      base.Close();
      if (this._hashFilePositions == null)
        return;
      this._hashFilePositions.Clear();
      this._hashFilePositions = (Dictionary<int, uint[]>) null;
    }

    /// <summary>Types of hashes requested</summary>
    public enum HashType
    {
      Index,
      Search,
    }

    /// <summary>Delegate for functions that provide hash codes</summary>
    /// <param name="toHash">Object to get hash code for</param>
    /// <param name="action">Type of hashing action performed</param>
    /// <param name="hashCode">Hash code (output)</param>
    /// <returns>True if hash should be used, false otherwise</returns>
    public delegate bool HashProviderDelegate(object toHash, HashSearchTextStream.HashType action, out int hashCode);

    /// <summary>
    /// Delegate for functions that check whether a line matches the search criteria
    /// </summary>
    /// <param name="key">Key being searched for</param>
    /// <param name="currentLine">Current line in file</param>
    /// <returns>True if line matches, false otherwise</returns>
    public delegate bool MatchProviderDelegate(object key, string currentLine);
  }
}
