using System;
using System.Collections.Generic;
using System.Resources;
using System.Drawing;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using Microsoft.BizTalk.Streaming;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.ScalableTransformation;
using Microsoft.XLANGs.RuntimeTypes;
using BizTalkComponents.Utils;

namespace BizTalkComponents.PipelineComponents
{
    /// <summary>
    ///  Transforms original message stream using streaming scalable transformation via provided map specification.
    /// </summary>
    public partial class XMLMLDisassembler:IBaseComponent
    {
      

        #region IBaseComponent Members

        public  string Description
        {
            get { return "Multi Level Disassemble Pipeline Component"; }
        }

        public  string Name
        {
            get { return "XML Multi Level Receive"; }
        }

        public new string Version
        {
            get { return "1.0.0"; }
        }

        #endregion

        public  void GetClassID(out Guid classID)
        {
            classID = new Guid("35A34C0D-8D73-45fd-960D-DB365CD56399");
        }

    }

}
