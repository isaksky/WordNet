// Decompiled with JetBrains decompiler
// Type: LAIR.Extensions.DictionaryExtensions
// Assembly: LAIR.Extensions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 647167B9-7037-4B5C-8B74-626D8FC76D80
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.Extensions.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LAIR.Extensions
{
  /// <summary>
  /// Provides extension methods for the .NET Dictionary class
  /// </summary>
  public static class DictionaryExtensions
  {
    /// <summary>Sorts dictionary keys by their corresponding values</summary>
    /// <typeparam name="KeyType">Type of key objects</typeparam>
    /// <typeparam name="ValueType">Type of value objects</typeparam>
    /// <param name="dictionary">Dictionary to sort</param>
    /// <returns>Sorted list of keys</returns>
    public static KeyType[] SortKeysByValues<KeyType, ValueType>(this Dictionary<KeyType, ValueType> dictionary)
    {
      return dictionary.SortKeysByValues<KeyType, ValueType>(false);
    }

    /// <summary>Sorts dictionary keys by their corresponding values</summary>
    /// <typeparam name="KeyType">Type of key objects</typeparam>
    /// <typeparam name="ValueType">Type of value objects</typeparam>
    /// <param name="dictionary">Dictionary to sort</param>
    /// <param name="reverse">Whether or not to reverse the sorted key list</param>
    /// <returns>Sorted list of keys</returns>
    public static KeyType[] SortKeysByValues<KeyType, ValueType>(this Dictionary<KeyType, ValueType> dictionary, bool reverse)
    {
      KeyType[] keyTypeArray = new KeyType[dictionary.Count];
      dictionary.Keys.CopyTo(keyTypeArray, 0);
      ValueType[] valueTypeArray = new ValueType[dictionary.Count];
      dictionary.Values.CopyTo(valueTypeArray, 0);
      Array.Sort<ValueType, KeyType>(valueTypeArray, keyTypeArray);
      if (reverse)
        Array.Reverse((Array) keyTypeArray);
      return keyTypeArray;
    }

    /// <summary>Sorts dictionary values by their corresponding keys</summary>
    /// <typeparam name="KeyType">Type of key objects</typeparam>
    /// <typeparam name="ValueType">Type of value objects</typeparam>
    /// <param name="dictionary">Dictionary to sort</param>
    /// <returns>Sorted list of values</returns>
    public static ValueType[] SortValuesByKeys<KeyType, ValueType>(this Dictionary<KeyType, ValueType> dictionary)
    {
      return dictionary.SortValuesByKeys<KeyType, ValueType>(false);
    }

    /// <summary>Sorts dictionary values by their corresponding keys</summary>
    /// <typeparam name="KeyType">Type of key objects</typeparam>
    /// <typeparam name="ValueType">Type of value objects</typeparam>
    /// <param name="dictionary">Dictionary to sort</param>
    /// <param name="reverse">Whether or not to reverse the sorted value list</param>
    /// <returns>Sorted list of values</returns>
    public static ValueType[] SortValuesByKeys<KeyType, ValueType>(this Dictionary<KeyType, ValueType> dictionary, bool reverse)
    {
      KeyType[] keyTypeArray = new KeyType[dictionary.Count];
      dictionary.Keys.CopyTo(keyTypeArray, 0);
      ValueType[] valueTypeArray = new ValueType[dictionary.Count];
      dictionary.Values.CopyTo(valueTypeArray, 0);
      Array.Sort<KeyType, ValueType>(keyTypeArray, valueTypeArray);
      if (reverse)
        Array.Reverse((Array) valueTypeArray);
      return valueTypeArray;
    }

    /// <summary>
    /// Adds a dictionary to the current one, merging values with key conflicts
    /// </summary>
    /// <typeparam name="KeyType">Type of keys</typeparam>
    /// <typeparam name="ValueType">Type of values</typeparam>
    /// <param name="dictionary">Dictionary to add to</param>
    /// <param name="toAdd">Dictionary to add</param>
    /// <param name="combine">Combination delegate</param>
    public static void Add<KeyType, ValueType>(this Dictionary<KeyType, ValueType> dictionary, Dictionary<KeyType, ValueType> toAdd, DictionaryExtensions.ValueCombinationDelegate<ValueType> combine)
    {
      foreach (KeyType key in toAdd.Keys)
      {
        if (!dictionary.ContainsKey(key))
          dictionary.Add(key, toAdd[key]);
        else
          dictionary[key] = combine(dictionary[key], toAdd[key]);
      }
    }

    /// <summary>Saves this dictionary to file</summary>
    /// <typeparam name="KeyType">Key type</typeparam>
    /// <typeparam name="ValueType">Value type</typeparam>
    /// <param name="dictionary">Dictionary to save</param>
    /// <param name="path">Path to save to</param>
    /// <param name="keyConversion">Conversion from keys to strings</param>
    /// <param name="valueConversion">Conversion from values to strings</param>
    /// <param name="sort">How to sort keys before printing lines</param>
    /// <param name="reverse">Whether or not to reverse the sorted key collection</param>
    /// <param name="writeCount">Whether or not to write the dictionary's count to file</param>
    /// <param name="linePrefix">Key-value line prefix</param>
    /// <param name="keyValSeparator">Key-value sparator</param>
    /// <param name="throwExceptionOnContainsSeparator">Whether or not to throw an exception if the key or value string
    /// contains the separator sequence</param>
    public static void Save<KeyType, ValueType>(this Dictionary<KeyType, ValueType> dictionary, string path, Func<KeyType, string> keyConversion, Func<ValueType, string> valueConversion, DictionaryExtensions.Sort sort, bool reverse, bool writeCount, string linePrefix, string keyValSeparator, bool throwExceptionOnContainsSeparator)
    {
      StreamWriter file = new StreamWriter(path);
      dictionary.Save<KeyType, ValueType>(file, keyConversion, valueConversion, sort, reverse, writeCount, linePrefix, keyValSeparator, throwExceptionOnContainsSeparator);
      file.Close();
    }

    /// <summary>Saves this dictionary to file</summary>
    /// <typeparam name="KeyType">Key type</typeparam>
    /// <typeparam name="ValueType">Value type</typeparam>
    /// <param name="dictionary">Dictionary to save</param>
    /// <param name="file">File to save to</param>
    /// <param name="keyConversion">Conversion from keys to strings</param>
    /// <param name="valueConversion">Conversion from values to strings</param>
    /// <param name="sort">How to sort keys before printing lines</param>
    /// <param name="reverse">Whether or not to reverse the sorted key collection</param>
    /// <param name="writeCount">Whether or not to write the dictionary's count to file</param>
    /// <param name="linePrefix">Key-value line prefix</param>
    /// <param name="keyValSeparator">Key-value sparator</param>
    /// <param name="throwExceptionOnContainsSeparator">Whether or not to throw an exception if the key or value string
    /// contains the separator sequence</param>
    public static void Save<KeyType, ValueType>(this Dictionary<KeyType, ValueType> dictionary, StreamWriter file, Func<KeyType, string> keyConversion, Func<ValueType, string> valueConversion, DictionaryExtensions.Sort sort, bool reverse, bool writeCount, string linePrefix, string keyValSeparator, bool throwExceptionOnContainsSeparator)
    {
      if (reverse && sort == DictionaryExtensions.Sort.None)
        throw new Exception("Cannot reverse without sorting");
      if (writeCount)
        file.WriteLine(dictionary.Count);
      IEnumerable<KeyType> keyTypes;
      if (sort != DictionaryExtensions.Sort.None)
      {
        KeyType[] array;
        if (sort == DictionaryExtensions.Sort.KeysByValues)
        {
          array = dictionary.SortKeysByValues<KeyType, ValueType>();
        }
        else
        {
          if (sort != DictionaryExtensions.Sort.Keys)
            throw new NotImplementedException("Sort not implemented:  " + (object) sort);
          array = dictionary.Keys.ToArray<KeyType>();
          Array.Sort<KeyType>(array);
        }
        if (reverse)
          Array.Reverse((Array) array);
        keyTypes = (IEnumerable<KeyType>) array;
      }
      else
        keyTypes = (IEnumerable<KeyType>) dictionary.Keys;
      foreach (KeyType index in keyTypes)
      {
        string str1 = keyConversion(index);
        string str2 = valueConversion(dictionary[index]);
        if (throwExceptionOnContainsSeparator && (str1.Contains(keyValSeparator) || str2.Contains(keyValSeparator)))
          throw new Exception("Neither key nor value can contain the key-value separater sequence");
        file.WriteLine(linePrefix + str1 + keyValSeparator + str2);
      }
    }

    /// <summary>
    /// Ensures that the current dictionary contains a given key. If it does not, a new key-value pair is added
    /// using the given key and the value resulting from calling the default constructor for the valueType type.
    /// </summary>
    /// <typeparam name="KeyType">Key type</typeparam>
    /// <typeparam name="ValueType">Value type</typeparam>
    /// <param name="dictionary">Dictionary</param>
    /// <param name="key">Key to ensure the existence of</param>
    /// <param name="valueType">Type of value</param>
    public static void EnsureContainsKey<KeyType, ValueType>(this Dictionary<KeyType, ValueType> dictionary, KeyType key, Type valueType)
    {
      dictionary.EnsureContainsKey<KeyType, ValueType>(key, valueType, (object[]) null);
    }

    /// <summary>
    /// Ensures that the current dictionary contains a given key. If it does not, a new key-value pair is added
    /// using the given key and the value resulting from calling the default constructor for the valueType type.
    /// </summary>
    /// <typeparam name="KeyType">Key type</typeparam>
    /// <typeparam name="ValueType">Value type</typeparam>
    /// <param name="dictionary">Dictionary</param>
    /// <param name="key">Key to ensure the existence of</param>
    /// <param name="valueType">Type of value</param>
    /// <param name="constructorParameters">Parameters to pass to the value constructor if the key needs to be added</param>
    public static void EnsureContainsKey<KeyType, ValueType>(this Dictionary<KeyType, ValueType> dictionary, KeyType key, Type valueType, params object[] constructorParameters)
    {
      if (dictionary.ContainsKey(key))
        return;
      dictionary.Add(key, (ValueType) Activator.CreateInstance(valueType, constructorParameters));
    }

    /// <summary>Different types of sort</summary>
    public enum Sort
    {
      None,
      Keys,
      KeysByValues,
    }

    /// <summary>Combines to values</summary>
    /// <typeparam name="ValueType">Type of values to combine</typeparam>
    /// <param name="value1">First value to combine</param>
    /// <param name="value2">Second value to combine</param>
    /// <returns>Combined value</returns>
    public delegate ValueType ValueCombinationDelegate<ValueType>(ValueType value1, ValueType value2);
  }
}
