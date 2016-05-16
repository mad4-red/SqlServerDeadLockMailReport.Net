using System;
using System.Xml;

namespace DeadLockReport
{
    internal class DeadLock
    {
        public XmlDocument DeadLockGraph { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public DeadLock(string deadLockGraph, DateTime timeStamp)
        {
            var xdoc = new XmlDocument();
            xdoc.LoadXml(string.Format("<deadlock-list>{0}</deadlock-list>",deadLockGraph));

            DeadLockGraph = xdoc;
            TimeStamp = timeStamp;
        }
    }
}