// Decompiled with JetBrains decompiler
// Type: LAIR.IO.Email
// Assembly: LAIR.IO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C0BD8F14-3EDA-432D-BDE1-EA5EF25889F6
// Assembly location: C:\Users\isak\dev\words\Public\LAIR.IO.dll

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace LAIR.IO
{
  /// <summary>Provides simply access to email transmission</summary>
  public class Email
  {
    /// <summary>Sends an email via SMTP</summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <param name="enableSSL"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="fromEmail"></param>
    /// <param name="fromName"></param>
    /// <param name="toEmail"></param>
    /// <param name="toName"></param>
    /// <param name="replyToEmailNames"></param>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    /// <param name="bodyIsHTML"></param>
    public static void SendViaSMTP(string host, int port, bool enableSSL, string username, string password, string fromEmail, string fromName, string toEmail, string toName, IEnumerable<Tuple<string, string>> replyToEmailNames, string subject, string body, bool bodyIsHTML)
    {
      SmtpClient smtpClient = new SmtpClient(host);
      smtpClient.UseDefaultCredentials = false;
      smtpClient.Credentials = (ICredentialsByHost) new NetworkCredential(username, password);
      smtpClient.Port = port;
      smtpClient.EnableSsl = enableSSL;
      MailMessage message = new MailMessage(new MailAddress(fromEmail, fromName), new MailAddress(toEmail, toName));
      if (replyToEmailNames != null)
      {
        foreach (Tuple<string, string> replyToEmailName in replyToEmailNames)
          message.ReplyToList.Add(new MailAddress(replyToEmailName.Item1, replyToEmailName.Item2));
      }
      message.Subject = subject;
      message.SubjectEncoding = Encoding.UTF8;
      message.Body = body;
      message.BodyEncoding = Encoding.UTF8;
      message.IsBodyHtml = bodyIsHTML;
      smtpClient.Send(message);
    }
  }
}
