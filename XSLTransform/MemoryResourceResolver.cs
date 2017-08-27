using System;
using System.Collections.Specialized;
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

    
    public enum PortDirection
    {
        receive = 1,
        send = 2
    }
    public class MemoryResourceResolver : XmlUrlResolver
    {
        static Dictionary<string, byte[]> includes = null;
        string m_portName = String.Empty;
        PortDirection m_portDirection = PortDirection.receive;

        private Dictionary<string, byte[]> Includes
        {
            get
            {
                if(includes == null)
                    includes = new Dictionary<string, byte[]>();

                return includes;
            }

        }

        public MemoryResourceResolver(string PortName, PortDirection PortDirection):base()
        {
            m_portName = PortName;
            m_portDirection = PortDirection;
        }

        public override object GetEntity(Uri absoluteUri,
          string role, Type ofObjectToReturn)
        {

           
            string fileName = absoluteUri.Segments[absoluteUri.Segments.Length - 1];
            string key = String.Format("{0}#{1}#{2}",m_portDirection,m_portName,fileName);

            if(Includes.ContainsKey(key))
            {
                return new MemoryStream(Includes[key]);
            }
            else
            {
                object content = null;

                using (SqlConnection connection = new SqlConnection(BtsConnectionHelper.MgmtDBConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(String.Format("SELECT cabContent FROM [adpl_sat] join [bts_{0}port] on [applicationId] = [nApplicationID] and [nvcName] = '{1}' where luid = concat(applicationId,':','{2}')", m_portDirection, m_portName, fileName), connection))
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
                        Includes.Add(key, CabBytes);

                    }
                    //default utf-8, so the files must be in utf-8
                    return new MemoryStream(CabBytes);


                }

            }
           
           
            throw new ArgumentException(String.Format("Resource file {0} could not be found in the same application as the {1} port {2}",fileName,m_portDirection,m_portName));
   
            

        }
    }
}
