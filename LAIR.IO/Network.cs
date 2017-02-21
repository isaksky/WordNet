// Decompiled with JetBrains decompiler
// Type: LAIR.IO.Network
// Assembly: LAIR.IO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C0BD8F14-3EDA-432D-BDE1-EA5EF25889F6
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.IO.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace LAIR.IO
{
  /// <summary>Provides convenience routines for network IO</summary>
  public static class Network
  {
    /// <summary>Gets IP addresses associated with the local host</summary>
    /// <returns></returns>
    public static IEnumerable<IPAddress> GetLocalIpAddresses()
    {
      return (IEnumerable<IPAddress>) Dns.GetHostEntry(Dns.GetHostName()).AddressList;
    }

    /// <summary>Downloads a file from a URI to a local path</summary>
    /// <param name="uri">URI to download</param>
    /// <param name="path">Path to download to</param>
    public static void Download(string uri, string path)
    {
      Console.Out.Write("Downloading \"" + uri + "\" to \"" + path + "\"...");
      WebRequest webRequest = WebRequest.Create(uri);
      webRequest.Method = "GET";
      using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
      {
        using (WebResponse response = webRequest.GetResponse())
        {
          using (Stream responseStream = response.GetResponseStream())
          {
            long num1 = 0;
            long num2 = (long) Math.Pow(2.0, 20.0);
            long num3 = 5L * num2;
            byte[] buffer = new byte[65536];
            int count;
            while ((count = responseStream.Read(buffer, 0, buffer.Length)) > 0)
            {
              fileStream.Write(buffer, 0, count);
              num1 += (long) count;
              num3 -= (long) count;
              if (num3 <= 0L)
              {
                num3 = 5L * num2;
                Console.Out.Write(string.Format("{0:0.00}", (object) ((double) num1 / (double) num2)) + " MB...");
              }
            }
            fileStream.Close();
            response.Close();
            responseStream.Close();
            Console.Out.WriteLine("download finished.");
          }
        }
      }
    }
  }
}
