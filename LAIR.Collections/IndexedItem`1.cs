// Decompiled with JetBrains decompiler
// Type: LAIR.Collections.Generic.IndexedItem`1
// Assembly: LAIR.Collections, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0311BD8-D14A-4FE6-8512-FA82B2AF1399
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.Collections.dll

namespace LAIR.Collections.Generic
{
  /// <summary>Represents an indexed item</summary>
  public abstract class IndexedItem<T>
  {
    private readonly T _index;

    /// <summary>Gets the index for an item</summary>
    public T Index
    {
      get
      {
        return this._index;
      }
    }

    /// <summary>Constructor</summary>
    /// <param name="index">Index of item</param>
    protected IndexedItem(T index)
    {
      this._index = index;
    }
  }
}
