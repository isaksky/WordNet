// Decompiled with JetBrains decompiler
// Type: LAIR.Collections.Generic.Permutor`1
// Assembly: LAIR.Collections, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0311BD8-D14A-4FE6-8512-FA82B2AF1399
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.Collections.dll

using System;
using System.Collections.Generic;

namespace LAIR.Collections.Generic
{
  /// <summary>Provides permutations of a sequence of items</summary>
  /// <typeparam name="T">Type of items to permute</typeparam>
  public class Permutor<T>
  {
    private T[] _items;
    private int _permutationLength;

    /// <summary>Constructor</summary>
    /// <param name="items">Items to permute</param>
    /// <param name="permutationLength">Length of permutation to return (-1 for all permutations)</param>
    public Permutor(T[] items, int permutationLength = -1)
    {
      this._items = items;
      this._permutationLength = permutationLength;
      if (this._permutationLength < -1 || this._permutationLength > this._items.Length)
        throw new ArgumentOutOfRangeException("permutationLength", "Must be >= -1 and <= length of items");
      if (this._items == null)
        throw new ArgumentNullException("items", "Cannot be null");
      if (this._items.Length == 0)
        throw new ArgumentException("items", "Must have items");
    }

    /// <summary>Gets enumerator over permutations of items</summary>
    /// <returns>Enumerator over permutations of items</returns>
    public IEnumerator<T[]> GetEnumerator()
    {
      if (this._permutationLength == 0)
        yield return new T[0];
      else if (this._items.Length == 1)
      {
        yield return this._items;
      }
      else
      {
        for (int i = 0; i < this._items.Length; ++i)
        {
          T[] otherItems = new T[this._items.Length - 1];
          int curr = 0;
          for (int index = 0; index < this._items.Length; ++index)
          {
            if (index != i)
              otherItems[curr++] = this._items[index];
          }
          foreach (T[] objArray in new Permutor<T>(otherItems, this._permutationLength > 0 ? this._permutationLength - 1 : -1))
          {
            T[] permutation = new T[objArray.Length + 1];
            permutation[0] = this._items[i];
            for (int index = 0; index < objArray.Length; ++index)
              permutation[index + 1] = objArray[index];
            yield return permutation;
          }
        }
      }
    }
  }
}
