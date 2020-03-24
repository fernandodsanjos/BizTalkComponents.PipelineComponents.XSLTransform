namespace BizTalkComponents.PipelineComponents.Send
{
    using System;
    using System.Collections.Generic;
    using Microsoft.BizTalk.PipelineOM;
    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Component.Interop;
    
    
    public sealed class XSLTransform : Microsoft.BizTalk.PipelineOM.SendPipeline
    {
        
        private const string _strPipeline = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Document xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instanc"+
"e\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" MajorVersion=\"1\" MinorVersion=\"0\">  <Description /> "+
" <CategoryId>8c6b051c-0ff5-4fc2-9ae5-5016cb726282</CategoryId>  <FriendlyName>Transmit</FriendlyName"+
">  <Stages>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"1\" Name=\"Pre-Assemble\" minO"+
"ccurs=\"0\" maxOccurs=\"-1\" execMethod=\"All\" stageId=\"9d0e4101-4cce-4536-83fa-4a5040674ad6\" />      <Co"+
"mponents>        <Component>          <Name>BizTalkComponents.PipelineComponents.XSLTTransform,BizTa"+
"lkComponents.PipelineComponents.XSLTransform, Version=1.1.0.1, Culture=neutral, PublicKeyToken=47190"+
"f56632fbc76</Name>          <ComponentName>XSLT Transformation</ComponentName>          <Description"+
">Pipeline Component to apply BizTalk map with context as argument.</Description>          <Version>1"+
".0.0</Version>          <Properties>            <Property Name=\"MapName\" />            <Property Nam"+
"e=\"Parameters\" />            <Property Name=\"MapRequired\">              <Value xsi:type=\"xsd:boolean"+
"\">false</Value>            </Property>          </Properties>          <CachedDisplayName>XSLT Trans"+
"formation</CachedDisplayName>          <CachedIsManaged>true</CachedIsManaged>        </Component>  "+
"    </Components>    </Stage>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"2\" Name=\""+
"Assemble\" minOccurs=\"0\" maxOccurs=\"1\" execMethod=\"All\" stageId=\"9d0e4107-4cce-4536-83fa-4a5040674ad6"+
"\" />      <Components />    </Stage>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"3\""+
" Name=\"Encode\" minOccurs=\"0\" maxOccurs=\"-1\" execMethod=\"All\" stageId=\"9d0e4108-4cce-4536-83fa-4a5040"+
"674ad6\" />      <Components />    </Stage>  </Stages></Document>";
        
        private const string _versionDependentGuid = "6e59cd96-ac11-4590-b633-729686bc4210";
        
        public XSLTransform()
        {
            Microsoft.BizTalk.PipelineOM.Stage stage = this.AddStage(new System.Guid("9d0e4101-4cce-4536-83fa-4a5040674ad6"), Microsoft.BizTalk.PipelineOM.ExecutionMode.all);
            IBaseComponent comp0 = Microsoft.BizTalk.PipelineOM.PipelineManager.CreateComponent("BizTalkComponents.PipelineComponents.XSLTTransform,BizTalkComponents.PipelineComponents.XSLTransform, Version=1.1.0.1, Culture=neutral, PublicKeyToken=47190f56632fbc76");;
            if (comp0 is IPersistPropertyBag)
            {
                string comp0XmlProperties = "<?xml version=\"1.0\" encoding=\"utf-16\"?><PropertyBag xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-inst"+
"ance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">  <Properties>    <Property Name=\"MapName\" />    "+
"<Property Name=\"Parameters\" />    <Property Name=\"MapRequired\">      <Value xsi:type=\"xsd:boolean\">f"+
"alse</Value>    </Property>  </Properties></PropertyBag>";
                PropertyBag pb = PropertyBag.DeserializeFromXml(comp0XmlProperties);;
                ((IPersistPropertyBag)(comp0)).Load(pb, 0);
            }
            this.AddComponent(stage, comp0);
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
