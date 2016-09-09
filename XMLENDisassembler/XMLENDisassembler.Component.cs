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
    ///  Extended functionality
    ///  1) Normalize envelope path, by removing invalid child records
    /// </summary>
    public partial class XMLENDisassembler: IBaseComponent,IPersistPropertyBag
    {

      
        #region IBaseComponent Members

        public string Description
        {
            get { return "Envelope Normalize XML Disassembler"; }
        }

        public string Name
        {
            get { return "Envelope Normalize Disassembler"; }
        }

        public string Version
        {
            get { return "1.0.0"; }
        }

        #endregion

        void IPersistPropertyBag.GetClassID(out Guid classID)
        {
            classID = new Guid("35A34C0D-8D73-45fd-960D-DB365CD56388");
        }

        void IPersistPropertyBag.InitNew()
        {
            base.InitNew();
        }
        /// <summary>
        /// Loads configuration property for component.
        /// </summary>
        /// <param name="pb">Configuration property bag.</param>
        /// <param name="errlog">Error status (not used in this code).</param>
        void IPersistPropertyBag.Load(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, Int32 errlog)
        {

            messageType = BizTalkComponents.Utils.PropertyBagHelper.ReadPropertyBag(pb, "MessageType", messageType);
            base.Load(pb, errlog);

        }

        /// <summary>
        /// Saves current component configuration into the property bag.
        /// </summary>
        /// <param name="pb">Configuration property bag.</param>
        /// <param name="fClearDirty">Not used.</param>
        /// <param name="fSaveAllProperties">Not used.</param>
        void IPersistPropertyBag.Save(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, bool fClearDirty, bool fSaveAllProperties)
        {
            BizTalkComponents.Utils.PropertyBagHelper.WritePropertyBag(pb, "MessageType", messageType);
            base.Save(pb, fClearDirty, fSaveAllProperties);

        }
        
    }

}
