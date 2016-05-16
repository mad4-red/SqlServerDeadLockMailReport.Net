using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DeadLockReport
{
    internal class DeadLockGraphReader
    {
        private readonly string _connectionString;

        private readonly string _sessionName;

        public DeadLockGraphReader(string connectionString, string sessionName)
        {
            _connectionString = connectionString;
            _sessionName = sessionName;
        }

        public DeadLock[] GetDeadLocks(DateTime since)
        {
            using (var cn = new SqlConnection(_connectionString))
            using (var cm = cn.CreateCommand())
            using (var da = new SqlDataAdapter(cm))
            using (var dt = new DataTable())
            {
                cm.CommandType = CommandType.Text;
                cm.CommandText = @"
select
    deadlock = event_data.event.query('(data/value/deadlock)[1]'),
    timestamp = dateadd(minute,datediff(minute,sysutcdatetime(),sysdatetime()),event_data.event.value('@timestamp', 'datetime'))
from (
    select
        target_data = convert(xml, target_data)
    from sys.dm_xe_session_targets session_targets
    inner join sys.dm_xe_sessions sessions
    on  sessions.address = session_targets.event_session_address
    where
        sessions.name = @session_name
    and session_targets.target_name = 'ring_buffer'
) sessions
cross apply sessions.target_data.nodes('/RingBufferTarget/event') event_data (event)
where
    event_data.event.value('@name','sysname') = 'xml_deadlock_report'
and dateadd(minute,datediff(minute,sysutcdatetime(),sysdatetime()),event_data.event.value('@timestamp', 'datetime')) > @since
";

                var p1 = cm.CreateParameter();
                p1.ParameterName = "session_name";
                p1.SqlDbType = SqlDbType.VarChar;
                p1.Size = 256;
                p1.SqlValue = _sessionName;
                cm.Parameters.Add(p1);

                var p2 = cm.CreateParameter();
                p2.ParameterName = "since";
                p2.SqlDbType = SqlDbType.DateTime;
                p2.SqlValue = since;
                cm.Parameters.Add(p2);

                try
                {
                    cn.Open();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        return new DeadLock[0];
                    }

                    var l = new List<DeadLock>();

                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var dr = dt.Rows[i];
                        var deadlockGraph = dr[0].ToString();

                        var timestamp = (DateTime)dr[1];

                        l.Add(new DeadLock(deadlockGraph, timestamp));
                    }

                    return l.ToArray();
                }
                finally
                {
                    cn.Close();
                    cn.Dispose();
                }
            }
        }
    }
}