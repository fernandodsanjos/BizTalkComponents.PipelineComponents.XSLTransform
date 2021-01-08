using Microsoft.BizTalk.ScalableTransformation;
using Microsoft.XLANGs.BaseTypes;
using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Microsoft.XLANGs.RuntimeTypes;

namespace BizTalkComponents.PipelineComponents
{
    public class TransformMetaData
    {
        private static MetadataCache _transforms = new MetadataCache(new MetadataCreator(TransformMetaData._creator));
        private readonly XmlDeclaration _xmlDecl;
        private readonly TransformBase _transformBase;

        private Type _mapType = null;
        private SchemaMetadata[] _sourceSchemas;
        private SchemaMetadata[] _targetSchemas;
        private string mapAssembly = String.Empty;


        /// <summary>
        /// Full assembly-qualifiedname of the map
        /// </summary>
        public string AssemblyQualifiedName
        {

            get
            {
                return _mapType.AssemblyQualifiedName;
            }

        }
        
        /// <summary>
        /// Assembly of the map
        /// </summary>
        public string Assembly
        {

            get
            {
                return mapAssembly;
            }

        }

        public  string XmlContent { 
            
            get
            {
                return _transformBase.XmlContent;
            }
            
       }
       
        public XsltArgumentList ArgumentList
        {
            get
            {
                return this._transformBase.TransformArgs;
            }
        }

        public XmlDeclaration Declaration
        {
            get
            {
                return this._xmlDecl;
            }
        }

        public SchemaMetadata[] SourceSchemas
        {
            get
            {
                return this._resolveSchemas(this._transformBase.SourceSchemas, ref this._sourceSchemas);
            }
        }

        public SchemaMetadata[] TargetSchemas
        {
            get
            {
                return this._resolveSchemas(this._transformBase.TargetSchemas, ref this._targetSchemas);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TransformMetaData);
        }

        public bool Equals(TransformMetaData obj)
        {
            return obj != null && obj.SourceSchemas[0] == this.SourceSchemas[0];
        }


        internal TransformMetaData(Type transformBaseType)
        {
            this.mapAssembly = transformBaseType.Assembly.ToString();
            this._transformBase = (TransformBase)Activator.CreateInstance(transformBaseType);
            this._mapType = transformBaseType;
            this._xmlDecl = TransformMetaData._getXmlDeclaration(this._transformBase);

        }

        private static object _creator(Type t)
        {
            return (object)new TransformMetaData(t);
        }

        public static TransformMetaData For(Type t)
        {
            return TransformMetaData._transforms.For(t) as TransformMetaData;
        }

        private SchemaMetadata _resolveSchema(string schemaRef)
        {
            return SchemaMetadata.For(SchemaBase.FindReferencedSchemaType(this._transformBase.GetType(), schemaRef));
        }

        private SchemaMetadata[] _resolveSchemas(string[] schemaRefs)
        {
            SchemaMetadata[] schemaMetadataArray = new SchemaMetadata[schemaRefs.Length];
            for (int index = 0; index < schemaMetadataArray.Length; ++index)
                schemaMetadataArray[index] = this._resolveSchema(schemaRefs[index]);
            return schemaMetadataArray;
        }

        private SchemaMetadata[] _resolveSchemas(string[] schemaRefs, ref SchemaMetadata[] resultRef)
        {
            SchemaMetadata[] schemaMetadataArray = resultRef;
            if (schemaMetadataArray == null)
            {
                lock (this)
                {
                    schemaMetadataArray = resultRef;
                    if (schemaMetadataArray == null)
                    {
                        schemaMetadataArray = this._resolveSchemas(schemaRefs);
                        Memory.Barrier();
                        resultRef = schemaMetadataArray;
                    }
                }
            }
            return schemaMetadataArray;
        }

        private static XmlDeclaration _getXmlDeclaration(TransformBase t)
        {
            XmlDocument xmlDocument = XmlHelpers.CreateXmlDocument();
            xmlDocument.LoadXml(t.XmlContent);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//xsl:output", nsmgr);
            if (xmlNode == null || xmlNode.Attributes.GetNamedItem("omit-xml-declaration") != null && xmlNode.Attributes.GetNamedItem("omit-xml-declaration").Value == "yes")
                return (XmlDeclaration)null;
            string version = xmlNode.Attributes.GetNamedItem("version") == null ? "1.0" : xmlNode.Attributes.GetNamedItem("version").Value;
            string encoding = xmlNode.Attributes.GetNamedItem("encoding") == null ? "UTF-8" : xmlNode.Attributes.GetNamedItem("encoding").Value;
            string standalone = xmlNode.Attributes.GetNamedItem("standalone") == null ? "no" : xmlNode.Attributes.GetNamedItem("standalone").Value;
            return xmlDocument.CreateXmlDeclaration(version, encoding, standalone);
        }

        
    }
}
