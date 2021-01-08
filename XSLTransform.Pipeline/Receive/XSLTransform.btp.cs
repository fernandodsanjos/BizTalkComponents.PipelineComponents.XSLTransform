namespace BizTalkComponents.PipelineComponents.Receive
{
    using System;
    using System.Collections.Generic;
    using Microsoft.BizTalk.PipelineOM;
    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Component.Interop;
    
    
    public sealed class XSLTransform : Microsoft.BizTalk.PipelineOM.ReceivePipeline
    {
        
        private const string _strPipeline = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Document xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instanc"+
"e\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" MajorVersion=\"1\" MinorVersion=\"0\">  <Description /> "+
" <CategoryId>f66b9f5e-43ff-4f5f-ba46-885348ae1b4e</CategoryId>  <FriendlyName>Receive</FriendlyName>"+
"  <Stages>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"1\" Name=\"Decode\" minOccurs=\""+
"0\" maxOccurs=\"-1\" execMethod=\"All\" stageId=\"9d0e4103-4cce-4536-83fa-4a5040674ad6\" />      <Component"+
"s />    </Stage>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"2\" Name=\"Disassemble\" "+
"minOccurs=\"0\" maxOccurs=\"-1\" execMethod=\"FirstMatch\" stageId=\"9d0e4105-4cce-4536-83fa-4a5040674ad6\" "+
"/>      <Components>        <Component>          <Name>Microsoft.BizTalk.Component.XmlDasmComp,Micro"+
"soft.BizTalk.Pipeline.Components, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35<"+
"/Name>          <ComponentName>XML disassembler</ComponentName>          <Description>Streaming XML "+
"disassembler</Description>          <Version>1.0</Version>          <Properties>            <Propert"+
"y Name=\"EnvelopeSpecNames\">              <Value xsi:type=\"xsd:string\" />            </Property>     "+
"       <Property Name=\"EnvelopeSpecTargetNamespaces\">              <Value xsi:type=\"xsd:string\" />  "+
"          </Property>            <Property Name=\"DocumentSpecNames\">              <Value xsi:type=\"x"+
"sd:string\" />            </Property>            <Property Name=\"DocumentSpecTargetNamespaces\">      "+
"        <Value xsi:type=\"xsd:string\" />            </Property>            <Property Name=\"AllowUnrec"+
"ognizedMessage\">              <Value xsi:type=\"xsd:boolean\">false</Value>            </Property>    "+
"        <Property Name=\"ValidateDocument\">              <Value xsi:type=\"xsd:boolean\">false</Value> "+
"           </Property>            <Property Name=\"RecoverableInterchangeProcessing\">              <V"+
"alue xsi:type=\"xsd:boolean\">false</Value>            </Property>            <Property Name=\"HiddenPr"+
"operties\">              <Value xsi:type=\"xsd:string\">EnvelopeSpecTargetNamespaces,DocumentSpecTarget"+
"Namespaces</Value>            </Property>            <Property Name=\"DtdProcessing\">              <V"+
"alue xsi:type=\"xsd:string\" />            </Property>          </Properties>          <CachedDisplayN"+
"ame>XML disassembler</CachedDisplayName>          <CachedIsManaged>true</CachedIsManaged>        </C"+
"omponent>      </Components>    </Stage>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID"+
"=\"3\" Name=\"Validate\" minOccurs=\"0\" maxOccurs=\"-1\" execMethod=\"All\" stageId=\"9d0e410d-4cce-4536-83fa-"+
"4a5040674ad6\" />      <Components>        <Component>          <Name>BizTalk.PipelineComponents.Writ"+
"eContext,BizTalk.PipelineComponents.WriteTypedContext, Version=1.0.0.0, Culture=neutral, PublicKeyTo"+
"ken=2dbe33dac94b8972</Name>          <ComponentName>Context Writer</ComponentName>          <Descrip"+
"tion>Writes typed context properties</Description>          <Version>1.0.0.0</Version>          <Pro"+
"perties>            <Property Name=\"Namespaces\">              <Value xsi:type=\"xsd:string\">http://my"+
"ns#custom;</Value>            </Property>            <Property Name=\"Properties\">              <Valu"+
"e xsi:type=\"xsd:string\">&lt;ArrayOfContextValue xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance"+
"\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"&gt;&lt;ContextValue&gt;&lt;Prefix&gt;custom&lt;/Prefi"+
"x&gt;&lt;Value&gt;map&lt;/Value&gt;&lt;Key&gt;XSLTransform&lt;/Key&gt;&lt;Namespace&gt;http://myns&l"+
"t;/Namespace&gt;&lt;Code&gt;String&lt;/Code&gt;&lt;/ContextValue&gt;&lt;/ArrayOfContextValue&gt;</Va"+
"lue>            </Property>            <Property Name=\"HiddenProperties\">              <Value xsi:ty"+
"pe=\"xsd:string\">Properties,Namespaces</Value>            </Property>            <Property Name=\"cust"+
"om.XSLTransform\">              <Value xsi:type=\"xsd:string\">map</Value>            </Property>      "+
"    </Properties>          <CachedDisplayName>Context Writer</CachedDisplayName>          <CachedIsM"+
"anaged>true</CachedIsManaged>        </Component>        <Component>          <Name>BizTalkComponent"+
"s.PipelineComponents.XSLTTransform,BizTalkComponents.PipelineComponents.XSLTransform, Version=1.1.0."+
"10, Culture=neutral, PublicKeyToken=47190f56632fbc76</Name>          <ComponentName>XSLT Transformat"+
"ion</ComponentName>          <Description>Pipeline Component to apply BizTalk map with context as ar"+
"gument.</Description>          <Version>1.0.0</Version>          <Properties>            <Property N"+
"ame=\"MapName\" />            <Property Name=\"Parameters\" />            <Property Name=\"MapRequired\"> "+
"             <Value xsi:type=\"xsd:boolean\">false</Value>            </Property>          </Propertie"+
"s>          <CachedDisplayName>XSLT Transformation</CachedDisplayName>          <CachedIsManaged>tru"+
"e</CachedIsManaged>        </Component>      </Components>    </Stage>    <Stage>      <PolicyFileSt"+
"age _locAttrData=\"Name\" _locID=\"4\" Name=\"ResolveParty\" minOccurs=\"0\" maxOccurs=\"-1\" execMethod=\"All\""+
" stageId=\"9d0e410e-4cce-4536-83fa-4a5040674ad6\" />      <Components />    </Stage>  </Stages></Docum"+
"ent>";
        
        private const string _versionDependentGuid = "c7f4b7ea-8a58-474b-b73e-db3d09a14f1a";
        
        public XSLTransform()
        {
            Microsoft.BizTalk.PipelineOM.Stage stage = this.AddStage(new System.Guid("9d0e4105-4cce-4536-83fa-4a5040674ad6"), Microsoft.BizTalk.PipelineOM.ExecutionMode.firstRecognized);
            IBaseComponent comp0 = Microsoft.BizTalk.PipelineOM.PipelineManager.CreateComponent("Microsoft.BizTalk.Component.XmlDasmComp,Microsoft.BizTalk.Pipeline.Components, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");;
            if (comp0 is IPersistPropertyBag)
            {
                string comp0XmlProperties = "<?xml version=\"1.0\" encoding=\"utf-16\"?><PropertyBag xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-inst"+
"ance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">  <Properties>    <Property Name=\"EnvelopeSpecNam"+
"es\">      <Value xsi:type=\"xsd:string\" />    </Property>    <Property Name=\"EnvelopeSpecTargetNamesp"+
"aces\">      <Value xsi:type=\"xsd:string\" />    </Property>    <Property Name=\"DocumentSpecNames\">   "+
"   <Value xsi:type=\"xsd:string\" />    </Property>    <Property Name=\"DocumentSpecTargetNamespaces\"> "+
"     <Value xsi:type=\"xsd:string\" />    </Property>    <Property Name=\"AllowUnrecognizedMessage\">   "+
"   <Value xsi:type=\"xsd:boolean\">false</Value>    </Property>    <Property Name=\"ValidateDocument\"> "+
"     <Value xsi:type=\"xsd:boolean\">false</Value>    </Property>    <Property Name=\"RecoverableInterc"+
"hangeProcessing\">      <Value xsi:type=\"xsd:boolean\">false</Value>    </Property>    <Property Name="+
"\"HiddenProperties\">      <Value xsi:type=\"xsd:string\">EnvelopeSpecTargetNamespaces,DocumentSpecTarge"+
"tNamespaces</Value>    </Property>    <Property Name=\"DtdProcessing\">      <Value xsi:type=\"xsd:stri"+
"ng\" />    </Property>  </Properties></PropertyBag>";
                PropertyBag pb = PropertyBag.DeserializeFromXml(comp0XmlProperties);;
                ((IPersistPropertyBag)(comp0)).Load(pb, 0);
            }
            this.AddComponent(stage, comp0);
            stage = this.AddStage(new System.Guid("9d0e410d-4cce-4536-83fa-4a5040674ad6"), Microsoft.BizTalk.PipelineOM.ExecutionMode.all);
            IBaseComponent comp1 = Microsoft.BizTalk.PipelineOM.PipelineManager.CreateComponent("BizTalk.PipelineComponents.WriteContext,BizTalk.PipelineComponents.WriteTypedContext, Version=1.0.0.0, Culture=neutral, PublicKeyToken=2dbe33dac94b8972");;
            if (comp1 is IPersistPropertyBag)
            {
                string comp1XmlProperties = "<?xml version=\"1.0\" encoding=\"utf-16\"?><PropertyBag xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-inst"+
"ance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">  <Properties>    <Property Name=\"Namespaces\">   "+
"   <Value xsi:type=\"xsd:string\">http://myns#custom;</Value>    </Property>    <Property Name=\"Proper"+
"ties\">      <Value xsi:type=\"xsd:string\">&lt;ArrayOfContextValue xmlns:xsi=\"http://www.w3.org/2001/X"+
"MLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"&gt;&lt;ContextValue&gt;&lt;Prefix&gt"+
";custom&lt;/Prefix&gt;&lt;Value&gt;map&lt;/Value&gt;&lt;Key&gt;XSLTransform&lt;/Key&gt;&lt;Namespace"+
"&gt;http://myns&lt;/Namespace&gt;&lt;Code&gt;String&lt;/Code&gt;&lt;/ContextValue&gt;&lt;/ArrayOfCon"+
"textValue&gt;</Value>    </Property>    <Property Name=\"HiddenProperties\">      <Value xsi:type=\"xsd"+
":string\">Properties,Namespaces</Value>    </Property>    <Property Name=\"custom.XSLTransform\">      "+
"<Value xsi:type=\"xsd:string\">map</Value>    </Property>  </Properties></PropertyBag>";
                PropertyBag pb = PropertyBag.DeserializeFromXml(comp1XmlProperties);;
                ((IPersistPropertyBag)(comp1)).Load(pb, 0);
            }
            this.AddComponent(stage, comp1);
            IBaseComponent comp2 = Microsoft.BizTalk.PipelineOM.PipelineManager.CreateComponent("BizTalkComponents.PipelineComponents.XSLTTransform,BizTalkComponents.PipelineComponents.XSLTransform, Version=1.1.0.10, Culture=neutral, PublicKeyToken=47190f56632fbc76");;
            if (comp2 is IPersistPropertyBag)
            {
                string comp2XmlProperties = "<?xml version=\"1.0\" encoding=\"utf-16\"?><PropertyBag xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-inst"+
"ance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">  <Properties>    <Property Name=\"MapName\" />    "+
"<Property Name=\"Parameters\" />    <Property Name=\"MapRequired\">      <Value xsi:type=\"xsd:boolean\">f"+
"alse</Value>    </Property>  </Properties></PropertyBag>";
                PropertyBag pb = PropertyBag.DeserializeFromXml(comp2XmlProperties);;
                ((IPersistPropertyBag)(comp2)).Load(pb, 0);
            }
            this.AddComponent(stage, comp2);
        }
        
        public override string XmlContent
        {
            get
            {
                return _strPipeline;
            }
        }
        
        public override System.Guid VersionDependentGuid
        {
            get
            {
                return new System.Guid(_versionDependentGuid);
            }
        }
    }
}
