// ***********************************************************************
// Assembly         : DatabaseHelper
// Author           : MinhTri
// Created          : 27-12-2014
//
// Last Modified By : MinhTri
// Last Modified On : 28-12-2014
// ***********************************************************************

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseHelper
{
    /// <summary>
    /// Class Helper.
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// The _connection string
        /// </summary>
        private static string _connectionString = ConfigurationManager.ConnectionStrings["CS"].ConnectionString;

        /// <summary>
        /// Backups the database.
        /// </summary>
        /// <param name="backUpFile">The back up file.</param>
        /// <param name="strDatabaseNameToBackup">The string database name to backup.</param>
        /// <param name="csName">Name of the cs.</param>
        /// <exception cref="System.Exception">Permission to perform this operation was denied - Choose a different folder to try again.</exception>
        public static void BackupDatabase(string backUpFile, string strDatabaseNameToBackup = "QLBH_08_2014", string csName = "CS")
        {
            try
            {
                _connectionString = ConfigurationManager.ConnectionStrings[csName].ConnectionString;
                SqlConnection.ClearAllPools();
                SqlConnection connect = new SqlConnection(_connectionString);
                connect.Open();
                ServerConnection con = new ServerConnection(connect);
                Server server = new Server(con);
                Backup source1 = new Backup();
                source1.Action = BackupActionType.Database;
                source1.Database = strDatabaseNameToBackup;
                Backup source = source1;
                BackupDeviceItem destination = new BackupDeviceItem(backUpFile, DeviceType.File);
                source.Devices.Add(destination);
                source.Initialize = true;
                source.Checksum = true;
                source.ContinueAfterError = true;
                source.Incremental = false;
                source.LogTruncation = BackupTruncateLogType.Truncate;
                source.SqlBackup(server);
                connect.Close();
            }
            catch (FailedOperationException)
            {
                throw new Exception("Permission to perform this operation was denied - Choose a different folder to try again.");
            }
        }

        /// <summary>
        /// Restores the database.
        /// </summary>
        /// <param name="restoreFile">The restore file.</param>
        /// <param name="strDatabaseNameToRestore">The string database name to restore.</param>
        /// <param name="csName">Name of the cs.</param>
        public static void RestoreDatabase(string restoreFile, string strDatabaseNameToRestore = "QLBH_08_2014", string csName = "CS")
        {
            _connectionString = ConfigurationManager.ConnectionStrings[csName].ConnectionString;
            SqlConnection connect = new SqlConnection(_connectionString);
            connect.Open();
            Server server = new Server(new ServerConnection(connect));
            Restore restore1 = new Restore();
            restore1.Database = strDatabaseNameToRestore;
            Restore restore2 = restore1;
            if (server.Databases[strDatabaseNameToRestore] != null)
            {
                server.KillAllProcesses(strDatabaseNameToRestore);
            }
            restore2.Devices.AddDevice(restoreFile, DeviceType.File);
            restore2.ReplaceDatabase = true;
            restore2.PercentCompleteNotification = 10;
            restore2.SqlRestore(server);
            server.Databases[strDatabaseNameToRestore].SetOnline();
            connect.Close();
        }
    }
}
