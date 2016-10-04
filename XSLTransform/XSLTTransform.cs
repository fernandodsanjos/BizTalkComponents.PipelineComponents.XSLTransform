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
using System.ComponentModel.DataAnnotations;
using BizTalkComponents.Utils;

namespace BizTalkComponents.PipelineComponents
{
    /// <summary>
    ///  Transforms original message stream using streaming scalable transformation via provided map specification.
    /// </summary>
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [ComponentCategory(CategoryTypes.CATID_Decoder)]
    [ComponentCategory(CategoryTypes.CATID_Encoder)]
    [System.Runtime.InteropServices.Guid("35A34C0D-8D73-45fd-960D-DB365CD56371")]
    public partial class XSLTTransform : IBaseComponent
    {
        private string _mapName = "";
        private const string _systemPropertiesNamespace = "http://schemas.microsoft.com/BizTalk/2003/system-properties";
        /// <summary>
        /// One or more piped Map specification to be applied to original message.
        /// </summary>	
        [RequiredRuntime]
        [Description("One or more piped fully qualified BizTalk map name(s).")]
        public string MapName
        {
            get { return _mapName; }
            set { _mapName = value; }
        }

        #region IComponent Members

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            if (!string.IsNullOrEmpty(_mapName))
            {
                MarkableForwardOnlyEventingReadStream stream =
                       new MarkableForwardOnlyEventingReadStream(
                           pInMsg.BodyPart.GetOriginalDataStream());

                string messageType = (string)pInMsg.Context.Read("MessageType", _systemPropertiesNamespace);

                if (messageType == String.Empty)
                {
                    stream.MarkPosition();
                    //Thanks to http://maximelabelle.wordpress.com/2010/07/08/determining-the-type-of-an-xml-message-in-a-custom-pipeline-component/
                    messageType = Microsoft.BizTalk.Streaming.Utils.GetDocType(stream);
                    stream.ResetPosition();
                }


                TransformMetaData _map = FindFirstMapMatch(messageType);

                if (_map == null)
                {
                    System.Diagnostics.Debug.WriteLine("No match for map could be made for message type: " + messageType);
                }
                else
                {

                    pInMsg.BodyPart.Data = TransformMessage(stream, _map, pInMsg);

                    pContext.ResourceTracker.AddResource(stream);
                }
            }


            return pInMsg;
        }



        #endregion

        internal TransformMetaData FindFirstMapMatch(string message)
        {
            string[] mapsArray = _mapName.Split(new char[] { '|' }, StringSplitOptions.None);
            TransformMetaData mapMatch = null;

            for (int i = 0; i < mapsArray.Length; i++)
            {
                try
                {
                    Type mapType = Type.GetType(mapsArray[i], true);
                    TransformMetaData map = TransformMetaData.For(mapType);

                    SchemaMetadata sourceSchema = map.SourceSchemas[0];

                    if (sourceSchema.SchemaName == message)
                    {
                        mapMatch = map;
                        break;
                    }
                }
                catch (Exception ex)
                {

                    throw new ApplicationException(string.Format("Error while trying to load MapType specification: {0}", mapsArray[i]), ex);
                }

            }

            return mapMatch;

        }

        /// <summary>
        /// Transforms original stream using streaming scalable transformation from BizTalk API.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        internal Stream TransformMessage(Stream inputStream, TransformMetaData map, IBaseMessage pInMsg)
        {
            BTSXslTransform transform = null;
            XsltArgumentList args = null;
            Context ext = null;
            SchemaMetadata targetSchema = targetSchema = map.TargetSchemas[0];

               
            // targetSchema.SchemaName => MessageType
            //targetSchema.ReflectedType.AssemblyQualifiedName => FullyQualified
              
            //It is possible to add a param but then you you need to work arounds in the map
            // map.ArgumentList.AddParam
            
            //The statement bellow caused me some problems that was solved by creating the XsltArgumentList instead
            //args = map.ArgumentList;
                

            try
            {
                transform = map.StreamingTransform;

                ext = new Context();

                for (int i = 0; i < pInMsg.Context.CountProperties; i++)
                {
                    string name;
                    string ns;
                    string value = pInMsg.Context.ReadAt(i, out name, out ns).ToString();
                    ext.Add(name, value, ns);

                }
                //MessageID is not located in the context
                //It is possible to add any information that should be available from the map
                ext.Add("MessageID", pInMsg.MessageID.ToString());


                args = new XsltArgumentList();
                //args.AddExtensionObject("http://www.w3.org/1999/XSL/Transform", ext); strangely it seams i cannot use this namespace in vs 2012, but it worked in vs 2010
                args.AddExtensionObject("urn:schemas-microsoft-com:xslt", ext);
                

                VirtualStream outputStream = new VirtualStream(VirtualStream.MemoryFlag.AutoOverFlowToDisk);
                transform.Transform(inputStream, args, outputStream, new XmlUrlResolver());
                outputStream.Seek(0, SeekOrigin.Begin);

                pInMsg.Context.Promote("MessageType", _systemPropertiesNamespace, targetSchema.SchemaName);
                pInMsg.Context.Promote("SchemaStrongName", _systemPropertiesNamespace, targetSchema.ReflectedType.AssemblyQualifiedName);
                //pInMsg.MessageID.ToString()
                
               
                return outputStream;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Error while trying to transform using MapType specification: {0}", _mapName), ex);
            }
        }

        /// <summary>
        /// Loads configuration property for component.
        /// </summary>
        /// <param name="pb">Configuration property bag.</param>
        /// <param name="errlog">Error status (not used in this code).</param>
        public void Load(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, Int32 errlog)
        {
            _mapName = PropertyBagHelper.ReadPropertyBag(pb,"MapName", _mapName);

        }

        /// <summary>
        /// Saves current component configuration into the property bag.
        /// </summary>
        /// <param name="pb">Configuration property bag.</param>
        /// <param name="fClearDirty">Not used.</param>
        /// <param name="fSaveAllProperties">Not used.</param>
        public void Save(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, bool fClearDirty, bool fSaveAllProperties)
        {
            PropertyBagHelper.WritePropertyBag(pb, "MapName", _mapName);

        }
       
    }


    
}
