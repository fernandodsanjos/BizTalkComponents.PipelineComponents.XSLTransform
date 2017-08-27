using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace BizTalkComponents.PipelineComponents
{
    public class BtsConnectionHelper
    {
        private const string REG_KEY_BTS_ADMINISTRATION = @"SOFTWARE\Microsoft\BizTalk Server\3.0\Administration";
        private const string MgmtDBName = "MgmtDBName";
        private const string MgmtDBServer = "MgmtDBServer";

        private static string mgmtdb_connection = String.Empty;

        public static string MgmtDBConnectionString
        {
            get
            {
                if (mgmtdb_connection == String.Empty)
                {
                    RegistryKey hKey = Registry.LocalMachine.OpenSubKey(REG_KEY_BTS_ADMINISTRATION, false);
                    mgmtdb_connection = String.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;"
                        , hKey.GetValue(MgmtDBServer)
                        , hKey.GetValue(MgmtDBName));
                }

                return mgmtdb_connection;

            }
        }
    }
}
