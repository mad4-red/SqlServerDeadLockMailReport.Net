using System;
using System.Linq;

namespace DeadLockReport
{
    class Program
    {
        static void Main()
        {
            var settingManager = new SettingManager();

            var connectionString = settingManager.GetConnectionString();
            var sessionName = settingManager.GetSessionName();

            var since = settingManager.GetSinceDateTime();

            var reader = new DeadLockGraphReader(connectionString, sessionName);
            var deadLocks = reader.GetDeadLocks(since);

            var mailOptions = settingManager.GetReportMailOptions();
            var reporter = new DeadLockReporter(mailOptions);

            if (!deadLocks.Any())
            {
                return;
            }

            foreach (var deadLock in deadLocks)
            {
                reporter.SendReport(deadLock);
            }

            settingManager.SetSinceDateTime(deadLocks.Max(x => x.TimeStamp));
        }
    }
}
