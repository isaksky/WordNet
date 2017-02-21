// Decompiled with JetBrains decompiler
// Type: LAIR.Collections.Generic.IndexedSet`2
// Assembly: LAIR.Collections, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0311BD8-D14A-4FE6-8512-FA82B2AF1399
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.Collections.dll

using System.Collections;
using System.Collections.Generic;

namespace LAIR.Collections.Generic
{
  /// <summary>Represents an indexable set</summary>
  /// <typeparam name="ItemType">Type of items to store</typeparam>
  /// <typeparam name="IndexType">Type of index to use on items</typeparam>
  public class IndexedSet<ItemType, IndexType> : IEnumerable<ItemType>, IEnumerable where ItemType : IndexedItem<IndexType>
  {
    private Dictionary<IndexType, ItemType> _indexItem;

    /// <summary>Gets the item for an index</summary>
    /// <param name="index">Index of item to get</param>
    /// <returns>Item</returns>
    public ItemType this[IndexType index]
    {
      get
      {
        return this._indexItem[index];
      }
    }

    /// <summary>Gets the number of items in this set</summary>
    public int Count
    {
      get
      {
        return this._indexItem.Count;
      }
    }

    /// <summary>Constructor</summary>
    /// <param name="initialCapacity">Initial capacity</param>
    public IndexedSet(int initialCapacity)
    {
      this._indexItem = new Dictionary<IndexType, ItemType>(initialCapacity);
    }

    /// <summary>Adds an item</summary>
    /// <param name="item">Item to add</param>
    /// <returns>True if item was new</returns>
    public void Add(ItemType item)
    {
      this._indexItem.Add(item.Index, item);
    }

    /// <summary>Adds a range of items</summary>
    /// <param name="items">Items to add</param>
    public void AddRange(IEnumerable<ItemType> items)
    {
      foreach (ItemType itemType in items)
        this.Add(itemType);
    }

    /// <summary>Gets item by index</summary>
    /// <param name="index">Index of item to fetch</param>
    /// <returns>Item</returns>
    public ItemType Get(IndexType index)
    {
      return this._indexItem[index];
    }

    /// <summary>Tries to get an item for an index</summary>
    /// <param name="index">Index</param>
    /// <param name="item">Item</param>
    /// <returns>True if item was found and false otherwise</returns>
    public bool TryGet(IndexType index, out ItemType item)
    {
      return this._indexItem.TryGetValue(index, out item);
    }

    /// <summary>Removes an item from this set</summary>
    /// <param name="item">Item to remove</param>
    public void Remove(ItemType item)
    {
      this._indexItem.Remove(item.Index);
    }

    /// <summary>Gets enumerator over items</summary>
    /// <returns></returns>
    public IEnumerator<ItemType> GetEnumerator()
    {
      return (IEnumerator<ItemType>) this._indexItem.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
