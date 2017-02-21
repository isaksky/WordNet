// Decompiled with JetBrains decompiler
// Type: LAIR.IO.StandardOutWriter
// Assembly: LAIR.IO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C0BD8F14-3EDA-432D-BDE1-EA5EF25889F6
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.IO.dll

using LAIR.Extensions;
using System;
using System.IO;
using System.Text;

namespace LAIR.IO
{
  /// <summary>
  /// Alternative writer for standard out. Pass to Console.SetOut to use.
  /// </summary>
  public class StandardOutWriter : TextWriter
  {
    private string _path;
    private StreamWriter _file;
    private bool _writeTimestamp;
    private TextWriter[] _otherOutputs;
    private bool _previousWriteNewLine;

    /// <summary>Gets the encoding used by this writer (always UTF8)</summary>
    public override Encoding Encoding
    {
      get
      {
        return Encoding.UTF8;
      }
    }

    /// <summary>Constructor</summary>
    /// <param name="path">Path to file to write output to</param>
    /// <param name="writeTimestamp">Whether or not to write a timestamp with each message</param>
    /// <param name="append">Whether or not to append to the output file</param>
    /// <param name="otherOutputs">Other outputs to use</param>
    public StandardOutWriter(string path, bool writeTimestamp, bool append, params TextWriter[] otherOutputs)
    {
      this._path = path;
      this._writeTimestamp = writeTimestamp;
      this._otherOutputs = otherOutputs;
      this._file = new StreamWriter(path, append);
      this._file.AutoFlush = true;
      this._previousWriteNewLine = true;
    }

    /// <summary>Truncates the output file and restarts the log</summary>
    public virtual void Clear()
    {
      lock (this._file)
      {
        this._file.Close();
        this._file = new StreamWriter(this._path);
        this._file.AutoFlush = true;
      }
    }

    /// <summary>
    /// Writes a string to output. If newlines are present in the passed value they will be written.
    /// </summary>
    /// <param name="value"></param>
    public override void Write(string value)
    {
      if (value.Trim() == "")
        return;
      this.Write(value, false);
    }

    /// <summary>
    /// Writes a string to output. If newlines are present in the passed value they will be removed.
    /// </summary>
    /// <param name="value"></param>
    public override void WriteLine(string value)
    {
      value = value.Replace('\r', ' ').Replace('\n', ' ').RemoveRepeatedWhitespace().Trim();
      if (value == "")
        return;
      this.Write(value, true);
    }

    /// <summary>
    /// Writes a string to output, optionally followed by a newline
    /// </summary>
    /// <param name="value"></param>
    /// <param name="newLine"></param>
    /// <returns>Value that was written</returns>
    protected virtual string Write(string value, bool newLine)
    {
      value = (!this._writeTimestamp || !this._previousWriteNewLine ? "" : DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ":  ") + value + (newLine ? Environment.NewLine : "");
      lock (this)
        base.Write(value);
      lock (this._file)
        this._file.Write(value);
      foreach (TextWriter otherOutput in this._otherOutputs)
      {
        lock (otherOutput)
        {
          try
          {
            otherOutput.Write(value);
          }
          catch (Exception exception_0)
          {
            throw new Exception("Failed to write to other output from LogWriter:  " + exception_0.Message);
          }
        }
      }
      this._previousWriteNewLine = newLine;
      return value;
    }

    /// <summary>Closes this writer</summary>
    public override void Close()
    {
      base.Close();
      this._file.Close();
    }
  }
}
