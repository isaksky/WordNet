// Decompiled with JetBrains decompiler
// Type: LAIR.Extensions.StringExtensions
// Assembly: LAIR.Extensions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 647167B9-7037-4B5C-8B74-626D8FC76D80
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.Extensions.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace LAIR.Extensions
{
  /// <summary>Provides extension methods for strings</summary>
  public static class StringExtensions
  {
    private static Regex _punctuationRE = new Regex("[^" + "a-zA-Z0-9 " + "]");
    private static Regex _spaceRE = new Regex("\\s+");
    private static Regex _repeatedWhitespaceRE = new Regex("\\s\\s+");
    private static Regex _oneSpaceRE = new Regex("\\s");

    /// <summary>Replaces strings within a string</summary>
    /// <param name="s">String to process</param>
    /// <param name="replacements">Replacement string pairs, where the key is the string to find and the value is the replacement</param>
    /// <param name="repeatUntilNoChange">Whether or not to repeat replacement procedure until no changes are made</param>
    /// <returns>String with replacements made</returns>
    public static string Replace(this string s, Dictionary<string, string> replacements, bool repeatUntilNoChange)
    {
      bool flag = true;
      while (flag)
      {
        flag = false;
        foreach (string key in replacements.Keys)
        {
          string str = s;
          s = s.Replace(key, replacements[key]);
          if (s != str)
            flag = true;
        }
        if (!repeatUntilNoChange)
          break;
      }
      return s;
    }

    /// <summary>
    /// Removes leading and trailing punctuation from a string
    /// </summary>
    /// <param name="s">String to trim punctuation from</param>
    /// <returns>Trimmed string</returns>
    public static string TrimPunctuation(this string s)
    {
      return s.TrimPunctuation(true, true);
    }

    /// <summary>
    /// Removes leading and trailing punctuation from a string
    /// </summary>
    /// <param name="s">String to trim punctuation from</param>
    /// <param name="leading">Whether or not to trim leading punctuation</param>
    /// <param name="trailing">Whether or not to trim trailing punctuation</param>
    /// <returns>Trimmed string</returns>
    public static string TrimPunctuation(this string s, bool leading, bool trailing)
    {
      if (s.Length == 0)
        return s;
      if (leading)
      {
        int num = 0;
        while (num < s.Length && StringExtensions._punctuationRE.Match(s, num, 1).Success)
          ++num;
        s = num < s.Length ? s.Substring(num) : "";
      }
      if (trailing)
      {
        int beginning = s.Length - 1;
        while (beginning >= 0 && StringExtensions._punctuationRE.Match(s, beginning, 1).Success)
          --beginning;
        s = beginning >= 0 ? s.Substring(0, beginning + 1) : "";
      }
      return s;
    }

    /// <summary>
    /// Removes punctuation characters (any that aren't a-z, A-Z, or 0-9)
    /// </summary>
    /// <param name="s">String to process</param>
    /// <returns>String without punctuation</returns>
    public static string RemovePunctuation(this string s)
    {
      return StringExtensions._punctuationRE.Replace(s, "");
    }

    /// <summary>
    /// Replaces punctuation characters (any that aren't a-z, A-Z, or 0-9) with something else
    /// </summary>
    /// <param name="s"></param>
    /// <param name="replacement"></param>
    /// <returns></returns>
    public static string ReplacePunctuation(this string s, string replacement)
    {
      return StringExtensions._punctuationRE.Replace(s, replacement);
    }

    /// <summary>
    /// Removes all whitespace characters from a string (\s regex character class)
    /// </summary>
    /// <param name="s">String to process</param>
    /// <returns>String without whitespace</returns>
    public static string RemoveWhitespace(this string s)
    {
      return StringExtensions._spaceRE.Replace(s, "");
    }

    /// <summary>Removes repeated whitespace from a string</summary>
    /// <param name="s">String to process</param>
    /// <returns>String without repeated whitespace</returns>
    public static string RemoveRepeatedWhitespace(this string s)
    {
      return StringExtensions._repeatedWhitespaceRE.Replace(s, " ");
    }

    /// <summary>
    /// Throws an exception if any of the given characters are present in the string
    /// </summary>
    /// <param name="s">String to check</param>
    /// <param name="chars">Character(s) to disallow</param>
    public static void Disallow(this string s, params char[] chars)
    {
      foreach (char ch in chars)
      {
        if (s.Contains<char>(ch))
          throw new Exception("No '" + (object) ch + "' characters are allowed");
      }
    }

    /// <summary>
    /// Splits a string on space characters, guaranteeing a specific number of parts. Will throw an exception if the expected number of parts is not found.
    /// </summary>
    /// <param name="s">String to split</param>
    /// <param name="expectedParts">Number of parts expected</param>
    /// <returns>Parts resulting from split</returns>
    public static string[] Split(this string s, int expectedParts)
    {
      return s.Split(expectedParts, ' ');
    }

    /// <summary>
    /// Splits a string on space characters, guaranteeing a specific number of parts. Will throw an exception if the expected number of parts is not found.
    /// </summary>
    /// <param name="s">String to split</param>
    /// <param name="expectedParts">Number of parts expected</param>
    /// <param name="splitCharacters">Characters to split on</param>
    /// <returns>Parts resulting from split</returns>
    public static string[] Split(this string s, int expectedParts, params char[] splitCharacters)
    {
      string[] strArray = new string[expectedParts];
      int startIndex = 0;
      int num1 = 0;
      while (startIndex < s.Length)
      {
        int num2 = -1;
        foreach (char splitCharacter in splitCharacters)
        {
          int num3 = s.IndexOf(splitCharacter, startIndex);
          if (num3 >= 0 && (num2 == -1 || num3 < num2))
            num2 = num3;
        }
        if (num2 == -1)
          num2 = s.Length;
        strArray[num1++] = s.Substring(startIndex, num2 - startIndex);
        startIndex = num2 + 1;
        if (startIndex == s.Length)
          strArray[num1++] = "";
      }
      if (num1 != expectedParts)
        throw new Exception("Part count mismatch");
      return strArray;
    }

    /// <summary>
    /// Gets enumeration of parts within a string, delimited by space characters
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static IEnumerable<string> GetParts(this string s)
    {
      int nonSpaceIndex = 0;
      while (true)
      {
        int nextSpace;
        for (; nonSpaceIndex >= s.Length || (int) s[nonSpaceIndex] != 32; nonSpaceIndex = nextSpace)
        {
          if (nonSpaceIndex == s.Length)
          {
            yield break;
          }
          else
          {
            nextSpace = s.IndexOf(' ', nonSpaceIndex);
            if (nextSpace == -1)
              nextSpace = s.Length;
            yield return s.Substring(nonSpaceIndex, nextSpace - nonSpaceIndex);
          }
        }
        ++nonSpaceIndex;
      }
    }

    /// <summary>Converts a string to its XML-escaped version</summary>
    /// <param name="text">Text to convert</param>
    /// <returns>XML-escaped version of text</returns>
    public static string EscapeXmlElement(this string text)
    {
      MemoryStream memoryStream = new MemoryStream();
      XmlTextWriter xmlTextWriter = new XmlTextWriter((Stream) memoryStream, Encoding.UTF8);
      xmlTextWriter.WriteString(text);
      xmlTextWriter.Flush();
      memoryStream.Position = 0L;
      StreamReader streamReader = new StreamReader((Stream) memoryStream);
      string end = streamReader.ReadToEnd();
      streamReader.Close();
      xmlTextWriter.Close();
      return end;
    }

    /// <summary>Unescapes an string that has been XML-escaped</summary>
    /// <param name="text">Text to convert</param>
    /// <returns>Unescaped XML text</returns>
    public static string UnescapeXML(this string text)
    {
      MemoryStream memoryStream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter((Stream) memoryStream);
      streamWriter.Write("<s>" + text + "</s>");
      streamWriter.Flush();
      memoryStream.Position = 0L;
      XmlTextReader xmlTextReader = new XmlTextReader((Stream) memoryStream);
      xmlTextReader.Read();
      string str = xmlTextReader.ReadString();
      streamWriter.Close();
      xmlTextReader.Close();
      memoryStream.Close();
      return str;
    }

    /// <summary>Gets path relative to another path</summary>
    /// <param name="path">Base path for absolute path</param>
    /// <param name="absolutePath">Absolute path</param>
    /// <returns>Relative path</returns>
    public static string GetRelativePathTo(this string path, string absolutePath)
    {
      if (!absolutePath.StartsWith(path))
        throw new Exception("Absolute path doesn't start with base path");
      return absolutePath.Substring(path.Length).Trim(Path.DirectorySeparatorChar);
    }

    /// <summary>Gets the common initial substring between two strings</summary>
    /// <param name="string1">First string</param>
    /// <param name="string2">Second string</param>
    /// <returns>Common initial substring</returns>
    public static string GetCommonInitialSubstring(this string string1, string string2)
    {
      if (string1 == null || string2 == null)
        throw new NullReferenceException("Strings must be non-null");
      int num1 = -1;
      int num2 = Math.Min(string1.Length, string2.Length);
      while (num1 < num2 - 1 && (int) string1[num1 + 1] == (int) string2[num1 + 1])
        ++num1;
      return string1.Substring(0, num1 + 1);
    }

    /// <summary>
    /// Changes the first n characters of a string to uppercase
    /// </summary>
    /// <param name="s">String to change</param>
    /// <param name="numChars">Number of characters to change</param>
    /// <returns>Modified string</returns>
    public static string InitialCharactersToUpper(this string s, int numChars)
    {
      return s.Substring(0, numChars).ToUpper() + s.Substring(numChars);
    }

    /// <summary>
    /// Gets the index of the nth non-space character within a string
    /// </summary>
    /// <param name="s">String to search</param>
    /// <param name="n">n</param>
    /// <returns>Index of nth non-space character</returns>
    public static int GetNthNonSpaceIndex(this string s, int n)
    {
      if (n <= 0)
        throw new Exception("Invalid n:  " + (object) n);
      int index = -1;
      while (index < s.Length - 1 && n > 0)
      {
        ++index;
        if (!StringExtensions._oneSpaceRE.IsMatch(s[index].ToString()))
          --n;
      }
      if (n > 0)
        index = -1;
      return index;
    }

    /// <summary>Gets the index of the nth occurrence of a character</summary>
    /// <param name="s">String to search</param>
    /// <param name="c">Character to search for</param>
    /// <param name="n">Value of n</param>
    /// <returns>Index of the nth occurrence of c</returns>
    public static int GetNthIndexOf(this string s, char c, int n)
    {
      if (n <= 0)
        throw new Exception("Invalid n:  " + (object) n);
      int index = -1;
      while (index < s.Length - 1 && n > 0)
      {
        ++index;
        if ((int) s[index] == (int) c)
          --n;
      }
      if (n > 0)
        index = -1;
      return index;
    }

    /// <summary>
    /// Gets all indexes of a substring within the current string
    /// </summary>
    /// <param name="s">String to search</param>
    /// <param name="substring">Substring to search for</param>
    /// <returns>Indexes</returns>
    public static IEnumerable<int> GetIndexesOf(this string s, string substring)
    {
      int next;
      for (int prev = -1; (next = s.IndexOf(substring, prev + 1)) != -1; prev = next)
        yield return next;
    }

    /// <summary>Concatenates an enumeration of strings</summary>
    /// <param name="strings">Strings to concatenate</param>
    /// <param name="separator">Separator for strings</param>
    /// <returns>Concatenated string</returns>
    public static string Concatenate(this IEnumerable<string> strings, string separator)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string str in strings)
        stringBuilder.Append((stringBuilder.Length == 0 ? "" : separator) + str);
      return stringBuilder.ToString();
    }

    /// <summary>Encrypts a string using AES encryption</summary>
    /// <param name="s">String to encrypt</param>
    /// <param name="key">Encryption key to use</param>
    /// <param name="initialization">Initialization to use</param>
    /// <returns>Encrypted bytes</returns>
    public static byte[] Encrypt(this string s, byte[] key, byte[] initialization)
    {
      using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
      {
        cryptoServiceProvider.KeySize = key.Length * 8;
        cryptoServiceProvider.BlockSize = initialization.Length * 8;
        using (ICryptoTransform encryptor = cryptoServiceProvider.CreateEncryptor(key, initialization))
        {
          byte[] bytes = Encoding.Unicode.GetBytes(s);
          return encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
        }
      }
    }

    /// <summary>Decrypts a string using AES encryption</summary>
    /// <param name="bytes">Bytes to decrypt</param>
    /// <param name="key">Encryption key used to produce the bytes</param>
    /// <param name="initialization">Initialization that was used to produce the bytes</param>
    /// <returns>Unencrypted string</returns>
    public static string Decrypt(this byte[] bytes, byte[] key, byte[] initialization)
    {
      using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
      {
        cryptoServiceProvider.KeySize = key.Length * 8;
        cryptoServiceProvider.BlockSize = initialization.Length * 8;
        using (ICryptoTransform decryptor = cryptoServiceProvider.CreateDecryptor(key, initialization))
          return Encoding.Unicode.GetString(decryptor.TransformFinalBlock(bytes, 0, bytes.Length));
      }
    }
  }
}
