// Decompiled with JetBrains decompiler
// Type: LAIR.Collections.Generic.Set`1
// Assembly: LAIR.Collections, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0311BD8-D14A-4FE6-8512-FA82B2AF1399
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.Collections.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LAIR.Collections.Generic
{
  /// <summary>Represents a set of unique items</summary>
  /// <typeparam name="T">Type of elements in the set</typeparam>
  [Serializable]
  public class Set<T> : IEnumerable<T>, IEnumerable
  {
    /// <summary>Contains the first slot number for each hash code</summary>
    private int[] _hashIndexFirstSlotIndex;
    /// <summary>Slots used to store element data and hash codes</summary>
    private Set<T>.Slot[] _slots;
    /// <summary>
    /// The number of slots that are in use (i.e. filled with data) or have been used and marked as empty
    /// later on. This differs from _slots.Length, as not all of the array will be in use.
    /// </summary>
    private int _numUsedSlots;
    /// <summary>
    /// The index of the first slot in the empty slots chain. Remove() prepends the cleared slot to
    /// the empty chain. Add() fills the first slot in the empty slots chain with the added element
    /// (or increases the number of touched slots if the empty slots chain is empty).
    /// </summary>
    private int _nextEmptySlotIndex;
    /// <summary>The number of items in this set</summary>
    private int _count;
    /// <summary>Equality comparer for elements in the set</summary>
    private IEqualityComparer<T> _comparer;
    /// <summary>
    /// The number of changes made to this set. Used by enumerators to detect changes and invalidate themselves.
    /// </summary>
    private int _numChangesMade;
    /// <summary>
    /// Whether or not to throw exceptions when duplicate elements are added to the set
    /// </summary>
    private bool _throwExceptionOnDuplicateAdd;
    /// <summary>
    /// Whether or not to throw exceptions when elements are not removed
    /// </summary>
    private bool _throwExceptionOnFailedRemove;
    /// <summary>Whether or not this set is read-only</summary>
    private bool _isReadOnly;
    /// <summary>
    /// Ratio of number of elements to the size of the hash index array. Values smaller than one result in quicker lookups
    /// and higher memory usage. Values larger than one result in slower lookups and lower memory usage.
    /// </summary>
    private float _loadFactor;

    /// <summary>
    /// Gets or sets whether or not this set is read-only (default:  false)
    /// </summary>
    public bool IsReadOnly
    {
      get
      {
        return this._isReadOnly;
      }
      set
      {
        this._isReadOnly = value;
      }
    }

    /// <summary>
    /// Gets or sets whether or not to throw exceptions when duplicate elements are added to the set (default:  true)
    /// </summary>
    public bool ThrowExceptionOnDuplicateAdd
    {
      get
      {
        return this._throwExceptionOnDuplicateAdd;
      }
      set
      {
        if (this._isReadOnly && value != this._throwExceptionOnDuplicateAdd)
          throw new Exception("Set is read-only");
        this._throwExceptionOnDuplicateAdd = value;
      }
    }

    /// <summary>
    /// Gets or sets whether or not to throw exceptions when elements are not removed (default:  true)
    /// </summary>
    public bool ThrowExceptionOnFailedRemove
    {
      get
      {
        return this._throwExceptionOnFailedRemove;
      }
      set
      {
        if (this._isReadOnly && value != this._throwExceptionOnFailedRemove)
          throw new Exception("Set is read-only");
        this._throwExceptionOnFailedRemove = value;
      }
    }

    /// <summary>Gets the number of elements in this set</summary>
    public int Count
    {
      get
      {
        return this._count;
      }
    }

    /// <summary>Constructor</summary>
    public Set()
    {
      this.Init(0, (IEqualityComparer<T>) null);
    }

    /// <summary>Constructor</summary>
    /// <param name="capacity">Number of elements that can be added without resizing</param>
    public Set(int capacity)
    {
      this.Init(capacity, (IEqualityComparer<T>) null);
    }

    /// <summary>Constructor</summary>
    /// <param name="capacity">Number of elements that can be added without resizing</param>
    /// <param name="comparer">Comparer to use</param>
    public Set(int capacity, IEqualityComparer<T> comparer)
    {
      this.Init(capacity, comparer);
    }

    /// <summary>Constructor</summary>
    /// <param name="capacity">Number of elements that can be added without resizing</param>
    /// <param name="throwExceptionOnDuplicateAdd">Whether or not to throw an exception if a duplicate element is added</param>
    public Set(int capacity, bool throwExceptionOnDuplicateAdd)
      : this(capacity)
    {
      this._throwExceptionOnDuplicateAdd = throwExceptionOnDuplicateAdd;
    }

    /// <summary>Constructor</summary>
    /// <param name="throwExceptionOnDuplicateAdd">Whether or not to throw an exception if a duplicate element is added</param>
    public Set(bool throwExceptionOnDuplicateAdd)
    {
      this.Init(0, (IEqualityComparer<T>) null);
      this._throwExceptionOnDuplicateAdd = throwExceptionOnDuplicateAdd;
    }

    /// <summary>Constructor</summary>
    /// <param name="elements">Elements to add to set</param>
    public Set(ICollection<T> elements)
      : this(elements, (IEqualityComparer<T>) null)
    {
    }

    /// <summary>Constructor</summary>
    /// <param name="elements">Elements to add to set</param>
    /// <param name="throwExceptionOnDuplicateAdd">Whether or not to throw an exception if a duplicate element is added</param>
    public Set(ICollection<T> elements, bool throwExceptionOnDuplicateAdd)
    {
      if (elements == null)
        throw new ArgumentNullException("collection");
      this.Init(elements.Count, (IEqualityComparer<T>) null);
      this._throwExceptionOnDuplicateAdd = throwExceptionOnDuplicateAdd;
      this.AddRange((IEnumerable<T>) elements);
    }

    /// <summary>Constructor</summary>
    /// <param name="comparer">Comparer to use</param>
    public Set(IEqualityComparer<T> comparer)
    {
      this.Init(0, comparer);
    }

    /// <summary>Constructor</summary>
    /// <param name="elements">Elements to construct set from</param>
    /// <param name="comparer">Comparer to use</param>
    public Set(ICollection<T> elements, IEqualityComparer<T> comparer)
    {
      if (elements == null)
        throw new ArgumentNullException("collection");
      this.Init(elements.Count, comparer);
      this.AddRange((IEnumerable<T>) elements);
    }

    /// <summary>Constructor</summary>
    /// <param name="path">Path to saved set</param>
    /// <param name="stringConversion">Function from strings to objects of type T</param>
    public Set(string path, Func<string, T> stringConversion)
    {
      if (!File.Exists(path))
        throw new FileNotFoundException("Invalid path to saved MySet");
      StreamReader streamReader = new StreamReader(path);
      bool flag1 = bool.Parse(streamReader.ReadLine());
      bool flag2 = bool.Parse(streamReader.ReadLine());
      bool flag3 = bool.Parse(streamReader.ReadLine());
      this.Init(int.Parse(streamReader.ReadLine()), (IEqualityComparer<T>) null);
      this._isReadOnly = flag1;
      this._throwExceptionOnDuplicateAdd = flag2;
      this._throwExceptionOnFailedRemove = flag3;
      string str;
      while ((str = streamReader.ReadLine()) != null)
        this.Add(stringConversion(str));
    }

    /// <summary>Equals operator override</summary>
    /// <param name="set1">First set</param>
    /// <param name="set2">Second set</param>
    /// <returns>True if sets are equal and false otherwise</returns>
    public static bool operator ==(Set<T> set1, Set<T> set2)
    {
      if ((object) set1 == null && (object) set2 == null)
        return true;
      if ((object) set1 == null ^ (object) set2 == null)
        return false;
      return set1.Equals((object) set2);
    }

    /// <summary>Not equal operator override</summary>
    /// <param name="set1">First set</param>
    /// <param name="set2">Second set</param>
    /// <returns>True if sets are equal and false otherwise</returns>
    public static bool operator !=(Set<T> set1, Set<T> set2)
    {
      return !(set1 == set2);
    }

    /// <summary>Initializes the set</summary>
    /// <param name="capacity">Number of elements that can be added without resizing</param>
    /// <param name="comparer">Comparer for set</param>
    private void Init(int capacity, IEqualityComparer<T> comparer)
    {
      if (capacity < 0)
        throw new ArgumentOutOfRangeException("capacity");
      if (capacity == 0)
        capacity = 1;
      this._comparer = comparer;
      if (this._comparer == null)
        this._comparer = (IEqualityComparer<T>) EqualityComparer<T>.Default;
      this._numUsedSlots = 0;
      this._numChangesMade = 0;
      this._nextEmptySlotIndex = -1;
      this._throwExceptionOnDuplicateAdd = this._throwExceptionOnFailedRemove = true;
      this._isReadOnly = false;
      this._loadFactor = 1f;
      int length = (int) ((double) capacity / (double) this._loadFactor);
      if (length == 0)
        length = 1;
      this._hashIndexFirstSlotIndex = new int[length];
      for (int index = 0; index < this._hashIndexFirstSlotIndex.Length; ++index)
        this._hashIndexFirstSlotIndex[index] = -1;
      this._slots = new Set<T>.Slot[capacity];
    }

    /// <summary>Adds an element to this set</summary>
    /// <param name="element">Element to add</param>
    /// <returns>True if element was added and false if it was already present</returns>
    public bool Add(T element)
    {
      if (this._isReadOnly)
        throw new Exception("Set is read-only");
      int hashCode;
      int hashIndex;
      this.Hash(element, this._hashIndexFirstSlotIndex.Length - 1, out hashCode, out hashIndex);
      if (this.Contains(hashIndex, hashCode, element))
      {
        if (this._throwExceptionOnDuplicateAdd)
          throw new Exception("Duplicate element added to set");
        return false;
      }
      int index;
      if (this._nextEmptySlotIndex == -1)
      {
        if (this._count + 1 > this._slots.Length)
        {
          this.Resize();
          this.Hash(element, this._hashIndexFirstSlotIndex.Length - 1, out hashCode, out hashIndex);
        }
        index = this._numUsedSlots++;
      }
      else
      {
        index = this._nextEmptySlotIndex;
        this._nextEmptySlotIndex = this._slots[this._nextEmptySlotIndex].NextSlotIndex;
      }
      this._slots[index].Element = element;
      this._slots[index].ElementHashCode = hashCode;
      this._slots[index].NextSlotIndex = this._hashIndexFirstSlotIndex[hashIndex];
      this._slots[index].IsFilled = true;
      this._hashIndexFirstSlotIndex[hashIndex] = index;
      ++this._count;
      ++this._numChangesMade;
      return true;
    }

    /// <summary>Adds a range of elements to this set</summary>
    /// <param name="elements">Elements to add</param>
    public void AddRange(IEnumerable<T> elements)
    {
      foreach (T element in elements)
        this.Add(element);
    }

    /// <summary>Removes an element from this set</summary>
    /// <param name="element">Element to remove</param>
    /// <returns>True if element was removed and false otherwise</returns>
    public bool Remove(T element)
    {
      if (this._isReadOnly)
        throw new Exception("Set is read-only");
      int hashCode;
      int hashIndex;
      this.Hash(element, this._hashIndexFirstSlotIndex.Length - 1, out hashCode, out hashIndex);
      int nextSlotIndex = this._hashIndexFirstSlotIndex[hashIndex];
      if (nextSlotIndex == -1)
      {
        if (this._throwExceptionOnFailedRemove)
          throw new Exception("Failed to remove element");
        return false;
      }
      int index = -1;
      do
      {
        Set<T>.Slot slot = this._slots[nextSlotIndex];
        if (slot.ElementHashCode != hashCode || !this._comparer.Equals(element, this._slots[nextSlotIndex].Element))
        {
          index = nextSlotIndex;
          nextSlotIndex = slot.NextSlotIndex;
        }
        else
          break;
      }
      while (nextSlotIndex != -1);
      if (nextSlotIndex == -1)
      {
        if (this._throwExceptionOnFailedRemove)
          throw new Exception("Failed to remove element");
        return false;
      }
      if (index == -1)
        this._hashIndexFirstSlotIndex[hashIndex] = this._slots[nextSlotIndex].NextSlotIndex;
      else
        this._slots[index].NextSlotIndex = this._slots[nextSlotIndex].NextSlotIndex;
      this._slots[nextSlotIndex].Element = default (T);
      this._slots[nextSlotIndex].ElementHashCode = 0;
      this._slots[nextSlotIndex].NextSlotIndex = this._nextEmptySlotIndex;
      this._slots[nextSlotIndex].IsFilled = false;
      this._nextEmptySlotIndex = nextSlotIndex;
      --this._count;
      ++this._numChangesMade;
      return true;
    }

    /// <summary>Removes a range of elements from this set</summary>
    /// <param name="elements">Elements to remove</param>
    public void RemoveRange(IEnumerable<T> elements)
    {
      foreach (T element in elements)
        this.Remove(element);
    }

    /// <summary>Removes elements that satisfy a predicate</summary>
    /// <param name="predicate">Predicate to check</param>
    /// <returns>Number of elements removed</returns>
    public int RemoveWhere(Predicate<T> predicate)
    {
      if (predicate == null)
        throw new ArgumentNullException("predicate");
      Set<T> set = new Set<T>();
      int num = 0;
      foreach (T element in this)
      {
        if (predicate(element))
        {
          set.Add(element);
          ++num;
        }
      }
      this.RemoveRange((IEnumerable<T>) set);
      return num;
    }

    /// <summary>Clears elements from this set</summary>
    public void Clear()
    {
      if (this._isReadOnly)
        throw new Exception("Set is read-only");
      for (int index = 0; index < this._hashIndexFirstSlotIndex.Length; ++index)
        this._hashIndexFirstSlotIndex[index] = -1;
      Array.Clear((Array) this._slots, 0, this._slots.Length);
      this._nextEmptySlotIndex = -1;
      this._numUsedSlots = 0;
      this._count = 0;
      ++this._numChangesMade;
    }

    /// <summary>Checks whether an element is in this set</summary>
    /// <param name="hashIndex">Hash index of element</param>
    /// <param name="hashCode">Hash code of element</param>
    /// <param name="element">Element to check for</param>
    /// <returns>True if element is present and false otherwise</returns>
    private bool Contains(int hashIndex, int hashCode, T element)
    {
      Set<T>.Slot slot;
      for (int nextSlotIndex = this._hashIndexFirstSlotIndex[hashIndex]; nextSlotIndex != -1; nextSlotIndex = slot.NextSlotIndex)
      {
        slot = this._slots[nextSlotIndex];
        if (slot.ElementHashCode == hashCode && this._comparer.Equals(element, slot.Element))
          return true;
      }
      return false;
    }

    /// <summary>Checks whether this set contains an element</summary>
    /// <param name="element">Element to check for</param>
    /// <returns>True if element is present and false otherwise</returns>
    public bool Contains(T element)
    {
      int hashCode;
      int hashIndex;
      this.Hash(element, this._hashIndexFirstSlotIndex.Length - 1, out hashCode, out hashIndex);
      return this.Contains(hashIndex, hashCode, element);
    }

    /// <summary>Gets a copy of the current set</summary>
    /// <returns>Copy of the current set</returns>
    public Set<T> Copy()
    {
      Set<T> set = new Set<T>(this._count, this._comparer);
      set.ThrowExceptionOnDuplicateAdd = this._throwExceptionOnDuplicateAdd;
      set.ThrowExceptionOnFailedRemove = this._throwExceptionOnFailedRemove;
      set.AddRange((IEnumerable<T>) this);
      set.IsReadOnly = this._isReadOnly;
      return set;
    }

    /// <summary>Copies elements in this set into an array</summary>
    /// <param name="array">Array into which elements should be copied</param>
    public void CopyTo(T[] array)
    {
      this.CopyTo(array, 0, this._count);
    }

    /// <summary>Copies elements in this set into an array</summary>
    /// <param name="array">Array into which elements should be copied</param>
    /// <param name="index">Index in array at which copying should begin</param>
    public void CopyTo(T[] array, int index)
    {
      this.CopyTo(array, index, this._count);
    }

    /// <summary>Copies elements in this set into an array</summary>
    /// <param name="array">Array into which elements should be copied</param>
    /// <param name="index">Index in array at which copying should begin</param>
    /// <param name="count">Maximum number of element to be copied</param>
    public void CopyTo(T[] array, int index, int count)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (index < 0 || index > array.Length)
        throw new ArgumentOutOfRangeException("index", "index must be >= 0 and <= array.length");
      if (array.Length - index < count)
        throw new ArgumentException("Destination array is not large enough to hold the requested elements");
      int index1 = 0;
      for (int index2 = 0; index1 < this._numUsedSlots && index2 < count; ++index1)
      {
        if (this._slots[index1].IsFilled)
        {
          array[index++] = this._slots[index1].Element;
          ++index2;
        }
      }
    }

    /// <summary>Converts the set to an array</summary>
    /// <returns>Array of elements</returns>
    public T[] ToArray()
    {
      T[] array = new T[this._count];
      this.CopyTo(array, 0);
      return array;
    }

    /// <summary>Saves this set to disk</summary>
    /// <param name="path">Path to save to</param>
    /// <param name="elementConversion">Function from elements to strings</param>
    public void Save(string path, Func<T, string> elementConversion)
    {
      StreamWriter streamWriter = new StreamWriter(path);
      streamWriter.WriteLine(this._isReadOnly.ToString() + Environment.NewLine + (object) this._throwExceptionOnDuplicateAdd + Environment.NewLine + (object) this._throwExceptionOnFailedRemove + Environment.NewLine + (object) this._count);
      foreach (T obj in this)
        streamWriter.WriteLine(elementConversion(obj));
      streamWriter.Close();
    }

    /// <summary>Resizes the set to accommodate the number of elements</summary>
    private void Resize()
    {
      if (this._nextEmptySlotIndex != -1 || this._numUsedSlots != this._slots.Length || this._count != this._slots.Length)
        throw new Exception("Set.Resize() not valid when empty slots remain");
      int nextCapacity = Set<T>.CapacityHelper.GetNextCapacity(this._slots.Length);
      int length = (int) ((double) nextCapacity / (double) this._loadFactor);
      if (length == 0)
        length = 1;
      int[] numArray = new int[length];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = -1;
      Set<T>.Slot[] slotArray = new Set<T>.Slot[nextCapacity];
      int num = 0;
      for (int index = 0; index < this._hashIndexFirstSlotIndex.Length; ++index)
      {
        int nextSlotIndex = this._hashIndexFirstSlotIndex[index];
        while (nextSlotIndex != -1)
        {
          int hashCode;
          int hashIndex;
          this.Hash(this._slots[nextSlotIndex].Element, numArray.Length - 1, out hashCode, out hashIndex);
          slotArray[nextSlotIndex].Element = this._slots[nextSlotIndex].Element;
          slotArray[nextSlotIndex].ElementHashCode = hashCode;
          slotArray[nextSlotIndex].NextSlotIndex = numArray[hashIndex];
          slotArray[nextSlotIndex].IsFilled = true;
          numArray[hashIndex] = nextSlotIndex;
          nextSlotIndex = this._slots[nextSlotIndex].NextSlotIndex;
          ++num;
        }
      }
      if (num != this._count)
        throw new Exception("Resize failed to move all elements");
      this._hashIndexFirstSlotIndex = numArray;
      this._slots = slotArray;
    }

    /// <summary>Gets a hash code and index for an element</summary>
    /// <param name="element">Element to get hash code and index for</param>
    /// <param name="maxHashIndex">Maximum hash index that should be returned</param>
    /// <param name="hashCode">Hash code for element</param>
    /// <param name="hashIndex">Hash index for element</param>
    private void Hash(T element, int maxHashIndex, out int hashCode, out int hashIndex)
    {
      hashCode = (object) element != null ? this._comparer.GetHashCode(element) : int.MinValue;
      hashIndex = (hashCode & int.MaxValue) % (maxHashIndex + 1);
    }

    /// <summary>
    /// Checks whether or not this set is identical to another
    /// </summary>
    /// <param name="obj">Set to compare this one to</param>
    /// <returns>True if sets are identical, false otherwise</returns>
    public override bool Equals(object obj)
    {
      if ((object) (obj as Set<T>) == null)
        return false;
      Set<T> set = obj as Set<T>;
      if (this.Count != set.Count)
        return false;
      foreach (T element in this)
      {
        if (!set.Contains(element))
          return false;
      }
      return true;
    }

    /// <summary>Gets hash code for this set</summary>
    /// <returns>Hash code for this set</returns>
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <summary>Gets number of elements in set</summary>
    /// <returns></returns>
    public override string ToString()
    {
      return this._count.ToString() + " element" + (this._count != 1 ? (object) "s" : (object) "");
    }

    /// <summary>
    /// Intersects the current set with another, removing elements from the current set that are not in the other.
    /// </summary>
    /// <param name="other">Set to intersect the current one with</param>
    public void IntersectWith(Set<T> other)
    {
      Set<T> set = new Set<T>();
      foreach (T element in this)
      {
        if (!other.Contains(element))
          set.Add(element);
      }
      this.RemoveRange((IEnumerable<T>) set);
    }

    /// <summary>Gets Dice overlap coefficient with another set</summary>
    /// <param name="other">Other set</param>
    /// <returns>Dice overlap coefficient</returns>
    public float GetDiceOverlapWith(Set<T> other)
    {
      return (float) (2 * this.Intersect<T>((IEnumerable<T>) other).Count<T>()) / (float) (this._count + other.Count);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return (IEnumerator<T>) new Set<T>.Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new Set<T>.Enumerator(this);
    }

    /// <summary>Gets enumerator for this set</summary>
    /// <returns>Enumerator for this set</returns>
    public Set<T>.Enumerator GetEnumerator()
    {
      return new Set<T>.Enumerator(this);
    }

    /// <summary>A single slot of element storage</summary>
    [Serializable]
    private struct Slot
    {
      /// <summary>Element in slot</summary>
      public T Element;
      /// <summary>Hash code of element in slot</summary>
      public int ElementHashCode;
      /// <summary>
      /// Index of next slot whose element hash code collided with the current one
      /// </summary>
      public int NextSlotIndex;
      /// <summary>Whether the slot is filled</summary>
      public bool IsFilled;
    }

    /// <summary>Represents an enumerator over a Set object</summary>
    public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
    {
      private Set<T> _set;
      private T _currentElement;
      private int _currentSlotIndex;
      private int _numSetChangesWhenCreated;

      /// <summary>Gets the current element in the enumeration</summary>
      public T Current
      {
        get
        {
          this.CheckState();
          if (this._currentSlotIndex < 0)
            throw new InvalidOperationException("Current is not valid");
          return this._currentElement;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      /// <summary>Constructor</summary>
      /// <param name="set">Set to iterate over</param>
      internal Enumerator(Set<T> set)
      {
        this._set = set;
        this._currentElement = default (T);
        this._currentSlotIndex = -1;
        this._numSetChangesWhenCreated = set._numChangesMade;
        if (set == (Set<T>) null)
          throw new ArgumentNullException("set", "Cannot enumerate over null sets");
      }

      /// <summary>Moves to the next element in the set</summary>
      /// <returns>True if move was successful and false otherwise</returns>
      public bool MoveNext()
      {
        this.CheckState();
        if (this._currentSlotIndex == this._set._numUsedSlots - 1)
          return false;
        while (++this._currentSlotIndex < this._set._numUsedSlots)
        {
          if (this._set._slots[this._currentSlotIndex].IsFilled)
          {
            this._currentElement = this._set._slots[this._currentSlotIndex].Element;
            return true;
          }
        }
        return false;
      }

      /// <summary>Disposes the current enumerator</summary>
      public void Dispose()
      {
        this._set = (Set<T>) null;
      }

      /// <summary>
      /// Makes sure the state of the enumerated set hasn't changed
      /// </summary>
      private void CheckState()
      {
        if (this._set == (Set<T>) null)
          throw new ObjectDisposedException("Set has been disposed");
        if (this._set._numChangesMade != this._numSetChangesWhenCreated)
          throw new InvalidOperationException("Set was modified while it was being iterated over");
      }

      void IEnumerator.Reset()
      {
        throw new NotSupportedException("Set enumerators cannot be reset");
      }
    }

    /// <summary>
    /// Provides helper functionality for setting the capacity
    /// </summary>
    private static class CapacityHelper
    {
      private static readonly int[] _primeCapacities = new int[35]
      {
        3,
        11,
        19,
        37,
        73,
        109,
        163,
        251,
        367,
        557,
        823,
        1237,
        1861,
        2777,
        4177,
        6247,
        9371,
        14057,
        21089,
        31627,
        47431,
        71143,
        106721,
        160073,
        240101,
        360163,
        540217,
        810343,
        1215497,
        1823231,
        2734867,
        4102283,
        6153409,
        9230113,
        13845163
      };

      /// <summary>Checks whether an integer is prime</summary>
      /// <param name="x">Integet to check</param>
      /// <returns>True if integer is prime, false otherwise</returns>
      public static bool IsPrime(int x)
      {
        if ((x & 1) == 0)
          return x == 2;
        int num1 = (int) Math.Sqrt((double) x) + 1;
        int num2 = 3;
        while (num2 <= num1)
        {
          if (x % num2 == 0)
            return false;
          num2 += 2;
        }
        return true;
      }

      /// <summary>
      /// Calculates the next prime number larger than a given number
      /// </summary>
      /// <param name="x">Number</param>
      /// <returns>Next prime number greater than given number</returns>
      private static int CalculateNextPrime(int x)
      {
        int num = x;
        int x1 = (num & 1) != 0 ? num + 2 : num + 1;
        while (x1 >= x)
        {
          if (Set<T>.CapacityHelper.IsPrime(x1))
            return x1;
          x1 += 2;
        }
        throw new OverflowException("Failed to get next prime number");
      }

      /// <summary>Gets the next capacity larger than a given capacity</summary>
      /// <param name="currentCapacity">Current capacity</param>
      /// <returns>Next capacity larger than a given capacity</returns>
      public static int GetNextCapacity(int currentCapacity)
      {
        int x = currentCapacity << 1 | 1;
        for (int index = 0; index < Set<T>.CapacityHelper._primeCapacities.Length; ++index)
        {
          if (x <= Set<T>.CapacityHelper._primeCapacities[index])
            return Set<T>.CapacityHelper._primeCapacities[index];
        }
        if (Set<T>.CapacityHelper.IsPrime(x))
          return x;
        return Set<T>.CapacityHelper.CalculateNextPrime(x);
      }
    }
  }
}
