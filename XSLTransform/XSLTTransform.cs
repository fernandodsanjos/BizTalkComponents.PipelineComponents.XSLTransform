using System;
using System.Collections.Generic;
using System.Resources;
using System.Drawing;
using System.Collections;
using System.Collections.Concurrent;
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
using Microsoft.BizTalk.Component.Utilities;

namespace BizTalkComponents.PipelineComponents
{
    /// <summary>
    ///  Transforms original message stream using streaming scalable transformation via provided map specification.
    /// </summary>
 
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [System.Runtime.InteropServices.Guid("35A34C0D-8D73-45fd-960D-DB365CD56371")]
    public partial class XSLTTransform : IBaseComponent
    {
        public static ConcurrentDictionary<TransformMetaData, BTSXslTransform> transforms = null;
        //Added Map cache
        public static ConcurrentDictionary<string, ConcurrentDictionary<string,TransformMetaData>> maps = null;

        public static ConcurrentDictionary<string, TransformMetaData> dynamiMaps = null;

        // private PortDirection m_portDirection;

        private string _mapName = String.Empty;
        private string _parameters = String.Empty;
        private string pipelineAssembly = String.Empty;
        private bool _mapRequired = true;

        private const string _systemPropertiesNamespace = "http://schemas.microsoft.com/BizTalk/2003/system-properties";

        private ConcurrentDictionary<string, TransformMetaData> DynamicMaps
        {

            get
            {
                if (dynamiMaps == null)
                    dynamiMaps = new ConcurrentDictionary<string, TransformMetaData>();

                return dynamiMaps;
            }
        }

        private ConcurrentDictionary<string, ConcurrentDictionary<string, TransformMetaData>> Maps
        {

            get
            {
                if (maps == null)
                    maps = new ConcurrentDictionary<string, ConcurrentDictionary<string, TransformMetaData>>();

                return maps;
            }
        }

        private ConcurrentDictionary<TransformMetaData, BTSXslTransform> Transforms
        {

            get
            {
                if (transforms == null)
                    transforms = new ConcurrentDictionary<TransformMetaData, BTSXslTransform>();

                return transforms;
            }
        }
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

        [Description("One or more static parameters in the format [name]=[value], use pipe to specify multiple parameters")]
        public string Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        [DisplayName("Map Required")]
        [Description("At least one map must match")]
        [DefaultValue(true)]
        public bool MapRequired
        {
            get { return _mapRequired; }
            set { _mapRequired = value; }
        }

        #region IComponent Members

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
           
            pipelineAssembly = pContext.PipelineName.Substring(pContext.PipelineName.IndexOf(",") + 1).TrimStart();

            /*
            string stageID = pContext.StageID.ToString("D");
            m_portDirection = PortDirection.send;
            if(stageID == CategoryTypes.CATID_Decoder || stageID == CategoryTypes.CATID_Validate || stageID == CategoryTypes.CATID_PartyResolver)
                m_portDirection = PortDirection.receive;
            */

            /*2019-05-20 Add map dynamically*/
            //Microsoft.XLANGs.BaseTypes.XmlQName TransformHint = new BTS.SendPortTransformHint().QName; -- Could not use SendPortTransformHint as this would run the map in a "normal mode" and IsDynamic did not need to be set.
            _mapName = _mapName.Trim();

            //Specify dynamic map in any context with the name XSLTransform
            string map = GetTransform(pInMsg.Context);

            if (string.IsNullOrEmpty(map) == false)
            {
                if (string.IsNullOrEmpty(_mapName))
                {
                    _mapName = map;
                }
                else
                {
                    _mapName = String.Format("{0}|{1}", _mapName, map);//Add the map last
                }
            }



            if (string.IsNullOrEmpty(_mapName))
            {
                if(_mapRequired)
                {
                    throw new ArgumentNullException("MapName");
                }
                else {
                    return pInMsg;
                }
               
            }
            else
            {
                MarkableForwardOnlyEventingReadStream stream =
                       new MarkableForwardOnlyEventingReadStream(
                           pInMsg.BodyPart.GetOriginalDataStream());

                
                string schemaStrongName = null;
                string messageType = null;
                ContextProperty property = null;

                if ((schemaStrongName = (string)pInMsg.Context.Read("SchemaStrongName", _systemPropertiesNamespace)) != null)
                {
                    if(schemaStrongName.StartsWith("Microsoft.XLANGs.BaseTypes.Any") == false)
                    {
                        property = new ContextProperty("SchemaStrongName", _systemPropertiesNamespace);
                        messageType = schemaStrongName;
                    }
               
                }

                if (messageType == null)
                {
                    messageType = (string)pInMsg.Context.Read("MessageType", _systemPropertiesNamespace);

                    //In cases where XmlDocument is used in orchestration, revert to check MessageType
                    property = new ContextProperty("MessageType", _systemPropertiesNamespace);

                }

                if (messageType == null)
                {
                    
                    property = new ContextProperty("MessageType", _systemPropertiesNamespace);
                    stream.MarkPosition();
                    //Thanks to http://maximelabelle.wordpress.com/2010/07/08/determining-the-type-of-an-xml-message-in-a-custom-pipeline-component/
                    messageType = Microsoft.BizTalk.Streaming.Utils.GetDocType(stream);

                    stream.ResetPosition();
                }

                TransformMetaData _map = FindFirstMapMatch(property, messageType);

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

        

        private string GetTransform(IBaseMessageContext context)
        {
            for (int i = 0; i < context.CountProperties; i++)
            {
                string name;
                string ns;
               
                object obj = context.ReadAt(i, out name, out ns);

                if(name == "XSLTransform")
                {
                    context.Write(name, ns, null);//Remove context as we do not want it to interfere
                    return (string)obj;
                    
                }
            }

            return null;
        }

        #endregion

        internal void AddParameters(XsltArgumentList args)
        {
            string[] parametersArray = _parameters.Split(new char[] { '|' }, StringSplitOptions.None);

            foreach (var parameter in parametersArray)
            {
                string[] parameterArray = parameter.Split(new char[] { '=' }, StringSplitOptions.None);

                if(parameterArray.Length == 2)
                {
                    args.RemoveParam(parameterArray[0], "");//remove existing param, save to call remove even if the param does not exist

                    args.AddParam(parameterArray[0], "", parameterArray[1]);
                    
                }
            }

        }

       
        //internal TransformMetaData FindFirstMapMatch(ContextProperty property, string value, string dynamicMap)
        //{
           
        //    if(String.IsNullOrWhiteSpace(dynamicMap))
        //        return FindFirstMapMatch(property, value);

        //    TransformMetaData mapMatch = null;

        //    if (DynamicMaps.ContainsKey(dynamicMap))
        //        return DynamicMaps[dynamicMap];

        //    try
        //    {

        //        Type mapType = Type.GetType(dynamicMap, true);

        //        mapMatch = TransformMetaData.For(mapType);

        //        SchemaMetadata sourceSchema = mapMatch.SourceSchemas[0];

        //        if (property.PropertyName == "SchemaStrongName")
        //        {

        //            if (sourceSchema.ReflectedType.AssemblyQualifiedName == value)
        //            {
        //                DynamicMaps.TryAdd(value, mapMatch);
        //                return mapMatch;


        //            }

        //        }
        //        else if (property.PropertyName == "MessageType" && sourceSchema.SchemaName == value)
        //        {
        //            DynamicMaps.TryAdd(value, mapMatch);
        //            return mapMatch;
        //        }

        //        mapMatch = null;

        //    }
        //    catch (Exception ex)
        //    {

        //        throw new ApplicationException(string.Format("Error while trying to load MapType specification: {0}", dynamicMap), ex);
        //    }


        //    //If context specified map is not found
        //    return FindFirstMapMatch(property, value);
        //}

        internal TransformMetaData FindFirstMapMatch(ContextProperty property, string value)
        {
            string[] mapsArray = _mapName.Split(new char[] { '|' }, StringSplitOptions.None);
            TransformMetaData mapMatch = null;

            //Check Cache
            if (Maps.ContainsKey(value))
            {
                for (int i = 0; i < mapsArray.Length; i++)
                {
                    if(Maps[value].TryGetValue(mapsArray[i], out mapMatch) == true)
                        return mapMatch;
                }

            }
            else
            {
                //Add cache for this messagetype
                Maps.TryAdd(value,new ConcurrentDictionary<string, TransformMetaData>());
            }

            //Check MapCache
            for (int i = 0; i < mapsArray.Length; i++)
            {
                try
                {
                    //When map exists in the same assembly as the pipeline you do not need to specify assembly
                    if (mapsArray[i].Contains(",") == false)
                        mapsArray[i] = String.Format("{0}, {1}", mapsArray[i], pipelineAssembly);

                    Type mapType = Type.GetType(mapsArray[i], true);

                    mapMatch = TransformMetaData.For(mapType);
                   
                    SchemaMetadata sourceSchema = mapMatch.SourceSchemas[0];

                    

                    if(property.PropertyName == "SchemaStrongName" )
                    {

                        if(sourceSchema.ReflectedType.AssemblyQualifiedName == value)
                        {
                            Maps[value].TryAdd(mapsArray[i], mapMatch);
                            break;
                        }
                        
                    }
                    else if(property.PropertyName == "MessageType" && sourceSchema.SchemaName == value)
                    {
                        Maps[value].TryAdd(mapsArray[i], mapMatch);
                        break;
                    }

                    mapMatch = null;
                   
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
            
            XsltArgumentList args = null;
            Context ext = null;
            SchemaMetadata targetSchema = targetSchema = map.TargetSchemas[0];

            string portname = String.Empty;

            
            //It is possible to add a param but then you you need to work arounds in the map
            // map.ArgumentList.AddParam
            
            //The statement bellow caused me some problems that was solved by creating the XsltArgumentList instead
            //args = map.ArgumentList;
                

            try
            {
               

                ext = new Context(pInMsg.Context, pInMsg.MessageID.ToString());

                args = map.ArgumentList;//Include BizTalk extensions
                //args.AddExtensionObject("http://www.w3.org/1999/XSL/Transform", ext); strangely it seams i cannot use this namespace in vs 2012, but it worked in vs 2010
                args.RemoveExtensionObject("urn:schemas-microsoft-com:xslt");
                args.AddExtensionObject("urn:schemas-microsoft-com:xslt", ext);

                AddParameters(args);

                //2017-08-23 Added intermidiate stream as Transform kills the original stream,
                //this is a problem if the incomming message is a enveloped message.
                // XmlTranslatorStream stm = new XmlTranslatorStream(XmlReader.Create(inputStream));
                //Opted to use non disposable stream instead
                NonDisposableStream stm = new NonDisposableStream(inputStream);
                VirtualStream outputStream = new VirtualStream(VirtualStream.MemoryFlag.AutoOverFlowToDisk);

                //TODO test to add declaration and see what happens with params!!!!!!!!!!!!!!!!!!!!!!!!

                BTSXslTransform btsXslTransform = null;
                string str = map.Assembly;
                if (Transforms.ContainsKey(map))
                {
                    btsXslTransform = Transforms[map];
                }
                else
                {
                   
                    btsXslTransform = new BTSXslTransform();
                    XmlTextReader xmlTextReader = new XmlTextReader((TextReader)new StringReader(map.XmlContent));
                    btsXslTransform.Load((XmlReader)xmlTextReader, new MemoryResourceResolver(map.Assembly), (System.Security.Policy.Evidence)null);

                    Transforms.TryAdd(map, btsXslTransform);
                }


                

                btsXslTransform.Transform(stm, args, outputStream, null);
                outputStream.Seek(0, SeekOrigin.Begin);

                pInMsg.Context.Write("XSLTransform", "PipelineComponents.XSLTransform",map.AssemblyQualifiedName);

                pInMsg.Context.Promote("MessageType", _systemPropertiesNamespace, targetSchema.SchemaName);
                //Changed to Write as SchemaStrongName could exceed 255 chars
                pInMsg.Context.Write("SchemaStrongName", _systemPropertiesNamespace, targetSchema.ReflectedType.AssemblyQualifiedName);
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
            _parameters = PropertyBagHelper.ReadPropertyBag(pb, "Parameters", _parameters);
            _mapRequired = PropertyBagHelper.ReadPropertyBag(pb, "MapRequired", _mapRequired);
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
            PropertyBagHelper.WritePropertyBag(pb, "Parameters", _parameters);
            PropertyBagHelper.WritePropertyBag(pb, "MapRequired", _mapRequired);

        }
       
    }


    
}
