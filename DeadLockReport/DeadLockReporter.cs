using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace DeadLockReport
{
    internal class DeadLockReporter
    {
        private readonly ReportMailOptions _mailOptions;

        public DeadLockReporter(ReportMailOptions mailOptions)
        {
            if (mailOptions == null) throw new ArgumentNullException("mailOptions");
            _mailOptions = mailOptions;
        }

        public void SendReport(DeadLock deadLock)
        {
            const string deadlockGraphFolder = "graph";
            var dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, deadlockGraphFolder));
            if (!dir.Exists)
            {
                dir.Create();
            }

            var graphFileName = string.Format("deadlock-{0}.xdl", deadLock.TimeStamp.ToString("yyyyMMddHHmmssfff"));
            var path = Path.Combine(dir.FullName, graphFileName);

            deadLock.DeadLockGraph.Save(path);

            using (var smtpClient = new SmtpClient(_mailOptions.Host, _mailOptions.Port))
            using (var message = new MailMessage {From = new MailAddress(_mailOptions.From)})
            using (var attachment = new Attachment(path))
            {
                smtpClient.EnableSsl = _mailOptions.EnableSsl;
                smtpClient.Credentials = new NetworkCredential(_mailOptions.UserName, _mailOptions.Password);

                foreach (var to in _mailOptions.To)
                {
                    message.To.Add(new MailAddress(to));
                }

                foreach (var cc in _mailOptions.Cc)
                {
                    message.CC.Add(new MailAddress(cc));
                }

                foreach (var bcc in _mailOptions.Bcc)
                {
                    message.Bcc.Add(new MailAddress(bcc));
                }

                var enc = Encoding.GetEncoding("ISO-2022-JP");

                message.Subject = EncodeMailHeader(EncodeMailHeader(string.Format("デッドロックが発生しました。 {0}", deadLock.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff"))));

                message.IsBodyHtml = false;
                message.BodyEncoding = enc;
                message.Body = @"デッドロックが発生しました。
添付のデッドロックグラフを確認して下さい。";

                message.Attachments.Add(attachment);

                smtpClient.Send(message);
            }

            Console.WriteLine(deadLock.TimeStamp);
        }

        private static string EncodeMailHeader(string subject)
        {
            var enc = Encoding.GetEncoding("ISO-2022-JP");
            var s64 = Convert.ToBase64String(enc.GetBytes(subject), Base64FormattingOptions.None);
            return string.Format("=?{0}?B?{1}?=", enc.HeaderName, s64);
        }
    }
}