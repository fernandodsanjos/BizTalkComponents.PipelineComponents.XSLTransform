using System;
using System.Collections.Generic;
using System.Collections;
using System.Resources;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using Microsoft.BizTalk.Streaming;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.ScalableTransformation;
using Microsoft.XLANGs.RuntimeTypes;
using System.ComponentModel.DataAnnotations;
using BizTalkComponents.Utils;
using Microsoft.BizTalk.Component.Utilities;

namespace BizTalkComponents.PipelineComponents
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    [System.Runtime.InteropServices.Guid("35A34C0D-8D73-45fd-960D-DB365CD56388")]
    public partial class XMLENDisassembler : XmlDasmComp,IDisassemblerComponent, IProbeMessage, IComponentUI
    {
        private const string _systemPropertiesNamespace = "http://schemas.microsoft.com/BizTalk/2003/system-properties";
        private string messageType = String.Empty;
        private string[] messageTypeParts;
        private XmlReader childReader = null;
        private BizTalkComponents.Utils.ContextProperty property;
        private IBaseMessage baseMsg = null;
         

        #region Xml disassembler Related Properties

        [Description("Allowed Message Type")]
        public string MessageType
        {
            get { return messageType; }
            set { messageType = value; }
        }
        /*
        [Description("Allowed Unrecognized Message")]
        public bool AllowUnrecognizedMessage
        {
            get { return xmlDisassembler.AllowUnrecognizedMessage; }
            set { xmlDisassembler.AllowUnrecognizedMessage = value; }

        }
        */
      



        [Browsable(false)]
        internal bool GenerateEmptyBodyMsg { get; set; }

        #endregion

        void IDisassemblerComponent.Disassemble(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            base.AllowUnrecognizedMessage = true;
            base.Disassemble(pContext, pInMsg);
            messageTypeParts = messageType.Split(new String[] { "#" }, StringSplitOptions.None);
            property = new BizTalkComponents.Utils.ContextProperty("MessageType", _systemPropertiesNamespace);
        }

        IBaseMessage CreateMessage(IPipelineContext pContext,XmlReader reader)
        {
            IBaseMessage outMsg = pContext.GetMessageFactory().CreateMessage();
            outMsg.AddPart("Body", pContext.GetMessageFactory().CreateMessagePart(), true);
            outMsg.Context = baseMsg.Context;
            outMsg.Context.Promote("MessageType", _systemPropertiesNamespace, messageType);
            
            outMsg.BodyPart.Data = new XmlTranslatorStream(reader.ReadSubtree());

            return outMsg;
        }
        
        IBaseMessage GetNextMessage(IPipelineContext pContext)
        {
            IBaseMessage outMsg = null;

            if (childReader == null || childReader.EOF)
            {
                baseMsg = base.GetNext(pContext);

                if (baseMsg == null)
                    return baseMsg;

                childReader = XmlReader.Create(baseMsg.BodyPart.GetOriginalDataStream());
            }

            while (childReader.Read())
            {
                if (childReader.NamespaceURI == messageTypeParts[0] && childReader.LocalName == messageTypeParts[1])
                {
                    XmlTranslatorStream trans = new XmlTranslatorStream(childReader.ReadSubtree());

                    outMsg = CreateMessage(pContext, childReader);
                    break;
                }
            }

            if (outMsg == null)
                outMsg = GetNextMessage(pContext);//make sure there is no more messages in base queue

            return outMsg;
   
            //return null if the message is null, but check if there exists more enveloped messages
        }
        IBaseMessage IDisassemblerComponent.GetNext(IPipelineContext pContext)
        {
            return GetNextMessage(pContext);
        }





        bool IProbeMessage.Probe(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            return base.Probe(pContext, pInMsg);
        }

        [Browsable(false)]
        public IntPtr Icon
        {
            get { return base.Icon; }
        }

        System.Collections.IEnumerator IComponentUI.Validate(object projectSystem)
        {
            return base.Validate(projectSystem);
        }
    } 
}
