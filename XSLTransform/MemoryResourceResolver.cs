using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace BizTalkComponents.PipelineComponents
{


    public class MemoryResourceResolver : XmlUrlResolver
    {

        private static ConcurrentDictionary<string, byte[]> includes = null;
        string m_mapAssembly = String.Empty;

        private ConcurrentDictionary<string, byte[]> Includes
        {
            get
            {
                if (includes == null)
                    includes = new ConcurrentDictionary<string, byte[]>();

                return includes;
            }

        }

        public MemoryResourceResolver(string mapAssembly)
            : base()
        {
            m_mapAssembly = mapAssembly;
        }

        public override object GetEntity(Uri absoluteUri,
          string role, Type ofObjectToReturn)
        {


            string fileName = absoluteUri.Segments[absoluteUri.Segments.Length - 1];
            string key = String.Format("{0}#{1}", m_mapAssembly, fileName);

            if (Includes.ContainsKey(key))
            {
                return new MemoryStream(Includes[key]);
            }
            else
            {
                object content = null;

                using (SqlConnection connection = new SqlConnection(BtsConnectionHelper.MgmtDBConnectionString))
                {
                    string sqlText = String.Format(@"SELECT top 1 res.[cabContent] FROM [BizTalkMgmtDb].[dbo].[adpl_sat] as ass join [BizTalkMgmtDb].[dbo].[adpl_sat] as res on
                    ass.luid = '{0}'
                    and ass.[applicationId] = res.[applicationId]
                    where res.luid like concat(ass.[applicationId] ,'%:{1}')", m_mapAssembly, fileName);

                    using (SqlCommand command = new SqlCommand(sqlText, connection))
                    {
                        connection.Open();
                        content = command.ExecuteScalar();

                    }
                }

                if (content != null)
                {
                    SqlBinary cabContent = new SqlBinary((byte[])content);
                    MemoryStream stream = new MemoryStream(cabContent.Value);
                    byte[] CabBytes = null;

                    using (CabFile file = new CabFile(stream))
                    {
                        // file.EntryExtract += CabEntryExtract;
                        file.ExtractEntries();
                        CabBytes = file.Entries[0].Data;
                        Includes.TryAdd(key, CabBytes);

                    }
                    //default utf-8, so the files must be in utf-8
                    return new MemoryStream(CabBytes);


                }

            }


            throw new ArgumentException(String.Format("Resource file {0} could not be found!", fileName));



        }
    }
}
