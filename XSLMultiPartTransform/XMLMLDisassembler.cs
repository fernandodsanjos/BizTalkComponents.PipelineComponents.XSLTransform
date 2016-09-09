using System;
using System.Collections.Generic;
using System.Collections;
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
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.ScalableTransformation;
using Microsoft.XLANGs.RuntimeTypes;
using System.ComponentModel.DataAnnotations;
using BizTalkComponents.Utils;
using Microsoft.Samples.BizTalk.Pipelines.CustomComponent;

namespace BizTalkComponents.PipelineComponents
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    [System.Runtime.InteropServices.Guid("35A34C0D-8D73-45fd-960D-DB365CD56399")]
    public partial class XMLMLDisassembler : XmlDasmComp, IDisassemblerComponent
    {
        List<XmlDasmComp> dissasemblers = new List<XmlDasmComp>();
       // Microsoft.BizTalk.Component.Utilities.SchemaList schemas; TODO remove reference
        XmlDasmComp current = null;
        int currIndex = 0;

        void IDisassemblerComponent.Disassemble(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            base.Disassemble(pContext, pInMsg);
            dissasemblers.Add(this);
            current = this;
        }

        IBaseMessage IDisassemblerComponent.GetNext(IPipelineContext pContext)
        {
            IBaseMessage msg = current.GetNext(pContext);

            IBaseMessageContext context = msg.Context;
            string name = String.Empty;
            string ns = String.Empty;

            for (int i = 0; i < msg.Context.CountProperties; i++)
            {
                object value = msg.Context.ReadAt(i,out name,out ns);

                System.Diagnostics.Trace.Write(String.Format("{0}:{1}", name, value));
            }
             


            if(msg != null)
            {

               System.IO.Stream seekableReadOnlyStream = new SeekableReadOnlyStream(msg.BodyPart.GetOriginalDataStream());

               pContext.ResourceTracker.AddResource(seekableReadOnlyStream);
               msg.BodyPart.Data = seekableReadOnlyStream;

               XmlDasmComp dasm = new XmlDasmComp();
               dasm.InitNew();
               dasm.DocumentSpecNames = base.DocumentSpecNames;
               dasm.EnvelopeSpecNames = base.EnvelopeSpecNames;
               dasm.Disassemble(pContext,msg);
               dissasemblers.Add(dasm);


               seekableReadOnlyStream.Seek(0, SeekOrigin.Begin);


               //msg.BodyPart.Data = seekableReadOnlyStream;

            }else{
                currIndex ++;
                if(dissasemblers.Count > currIndex)
                {
                    current = dissasemblers[currIndex];
                    msg = current.GetNext(pContext);
                }
                
            }
            

            return msg;
        }

        private Stream GetMessageStream(IBaseMessage msg, IPipelineContext context)
        {
            var stream = msg.BodyPart.GetOriginalDataStream();

            if (!stream.CanSeek)
            {
                var readStream = new ReadOnlySeekableStream(stream);

                if (context != null)
                {
                    context.ResourceTracker.AddResource(readStream);
                }

                msg.BodyPart.Data = readStream;
                stream = readStream;
            }
            return stream;
        }
        
    } 
}
