// Decompiled with JetBrains decompiler
// Type: LAIR.Extensions.IListExtensions
// Assembly: LAIR.Extensions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 647167B9-7037-4B5C-8B74-626D8FC76D80
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.Extensions.dll

using System;
using System.Collections.Generic;

namespace LAIR.Extensions
{
  /// <summary>Extensions for IList objects</summary>
  public static class IListExtensions
  {
    /// <summary>Randomizes the list</summary>
    /// <typeparam name="T">Type of list to randomize</typeparam>
    /// <param name="list">List to randomize</param>
    public static void Randomize<T>(this IList<T> list)
    {
      list.Randomize<T>(new Random());
    }

    /// <summary>Randomizes the list</summary>
    /// <typeparam name="T">Type of list to randomize</typeparam>
    /// <param name="list">List to randomize</param>
    /// <param name="r">Random number generator to use</param>
    public static void Randomize<T>(this IList<T> list, Random r)
    {
      list.Randomize<T>(0, list.Count - 1, r);
    }

    /// <summary>Randomizes the list</summary>
    /// <typeparam name="T">Type of list to randomize</typeparam>
    /// <param name="list">List to randomize</param>
    /// <param name="start">Start of list to randomize</param>
    /// <param name="end">End of range to randomize</param>
    /// <param name="r">Random number generator to use</param>
    public static void Randomize<T>(this IList<T> list, int start, int end, Random r)
    {
      for (int index1 = start; index1 <= end; ++index1)
      {
        int index2 = r.Next(start, end + 1);
        T obj = list[index1];
        list[index1] = list[index2];
        list[index2] = obj;
      }
    }

    /// <summary>Removes elements in a list based on an indicator list</summary>
    /// <typeparam name="T">Type of list</typeparam>
    /// <param name="list">List</param>
    /// <param name="remove">Array of boolean values indicating which elements to remove</param>
    public static void RemoveAtMultiple<T>(this IList<T> list, bool[] remove)
    {
      if (list.Count != remove.Length)
        throw new Exception("Remove list must be the same length as the list itself");
      for (int length = remove.Length; length >= 0; --length)
      {
        if (remove[length])
          list.RemoveAt(length);
      }
    }
  }
}
