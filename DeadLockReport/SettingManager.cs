using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace DeadLockReport
{
    internal class SettingManager
    {
        public string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
        }

        public string GetSessionName()
        {
            return ConfigurationManager.AppSettings["SessionName"];
        }

        private const string DatFilePath = "timestamp.dat";

        public DateTime GetSinceDateTime()
        {

            if (!File.Exists(DatFilePath))
            {
                SetSinceDateTime(DateTime.Now);
            }

            var t = File.ReadAllText(DatFilePath);

            DateTime timestamp;
            if (!DateTime.TryParse(t, out timestamp))
            {
                var ret = DateTime.Now;

                SetSinceDateTime(ret);
                return ret;
            }

            return timestamp;
        }

        public void SetSinceDateTime(DateTime d)
        {
            var file = new FileInfo(DatFilePath);
            if (!file.Exists)
            {
                using (file.Create())
                {
                    
                }
            }

            using (var sw = file.CreateText())
            {
                sw.Write(d.ToString("yyyy/MM/dd HH:mm:ss.fff"));
            }
        }

        public ReportMailOptions GetReportMailOptions()
        {
            var opt = new ReportMailOptions();

            var host = ConfigurationManager.AppSettings["Host"];
            if (string.IsNullOrWhiteSpace(host)) throw new InvalidOperationException("Host");
            opt.Host = host;

            int port;
            if (!int.TryParse(ConfigurationManager.AppSettings["Port"], out port))
            {
                throw new InvalidOperationException("Port");
            }
            opt.Port = port;

            bool enableSsl;
            if (!bool.TryParse(ConfigurationManager.AppSettings["EnableSsl"], out enableSsl))
            {
                throw new InvalidOperationException("EnableSsl");
            }
            opt.EnableSsl = enableSsl;

            var userName = ConfigurationManager.AppSettings["UserName"];
            if (string.IsNullOrWhiteSpace(userName)) throw new InvalidOperationException("UserName");
            opt.UserName = userName;

            opt.Password = ConfigurationManager.AppSettings["Password"];

            var from = ConfigurationManager.AppSettings["From"];
            if (string.IsNullOrWhiteSpace(from)) throw new InvalidOperationException("From");
            opt.From = from;

            var to = ConfigurationManager.AppSettings["To"].Replace(" ", "");
            opt.To = !string.IsNullOrWhiteSpace(to) ? to.Split(',') : new string[0];

            var cc = ConfigurationManager.AppSettings["Cc"].Replace(" ", "");
            opt.Cc = !string.IsNullOrWhiteSpace(cc) ? cc.Split(',') : new string[0];

            var bcc = ConfigurationManager.AppSettings["Bcc"].Replace(" ", "");
            opt.Bcc = !string.IsNullOrWhiteSpace(bcc) ? bcc.Split(',') : new string[0];

            if (!opt.To.Any() && !opt.Cc.Any() && !opt.Bcc.Any())
            {
                throw new InvalidOperationException("to & cc & bcc empty");
            }

            return opt;
        }
    }
}