namespace DeadLockReport
{
    internal class ReportMailOptions
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public bool EnableSsl { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string From { get; set; }

        public string[] To { get; set; }

        public string[] Cc { get; set; }

        public string[] Bcc { get; set; }
    }
}