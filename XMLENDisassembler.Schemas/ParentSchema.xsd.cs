namespace BizTalkComponents.PipelineComponents {
    using Microsoft.XLANGs.BaseTypes;
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [SchemaType(SchemaTypeEnum.Document)]
    [Schema(@"http://BizTalkComponents.PipelineComponents.ParentSchema",@"Parent")]
    [BodyXPath(@"/*[local-name()='Parent' and namespace-uri()='http://BizTalkComponents.PipelineComponents.ParentSchema']")]
    [System.SerializableAttribute()]
    [SchemaRoots(new string[] {@"Parent"})]
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"BizTalkComponents.PipelineComponents.ChildSchema", typeof(global::BizTalkComponents.PipelineComponents.ChildSchema))]
    public sealed class ParentSchema : Microsoft.XLANGs.BaseTypes.SchemaBase {
        
        [System.NonSerializedAttribute()]
        private static object _rawSchema;
        
        [System.NonSerializedAttribute()]
        private const string _strSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns=""http://BizTalkComponents.PipelineComponents.ParentSchema"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" xmlns:ns0=""http://BizTalkComponents.PipelineComponents.ChildSchema"" targetNamespace=""http://BizTalkComponents.PipelineComponents.ParentSchema"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:import schemaLocation=""BizTalkComponents.PipelineComponents.ChildSchema"" namespace=""http://BizTalkComponents.PipelineComponents.ChildSchema"" />
  <xs:annotation>
    <xs:appinfo>
      <b:schemaInfo is_envelope=""yes"" />
      <b:references>
        <b:reference targetNamespace=""http://BizTalkComponents.PipelineComponents.ChildSchema"" />
      </b:references>
    </xs:appinfo>
  </xs:annotation>
  <xs:element name=""Parent"">
    <xs:annotation>
      <xs:appinfo>
        <b:recordInfo body_xpath=""/*[local-name()='Parent' and namespace-uri()='http://BizTalkComponents.PipelineComponents.ParentSchema']"" />
      </xs:appinfo>
    </xs:annotation>
    <xs:complexType>
      <xs:sequence>
        <xs:element name=""TransId"" type=""xs:integer"" />
        <xs:element name=""TransDate"" type=""xs:date"" />
        <xs:element maxOccurs=""unbounded"" ref=""ns0:Child"" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        public ParentSchema() {
        }
        
        public override string XmlContent {
            get {
                return _strSchema;
            }
        }
        
        public override string[] RootNodes {
            get {
                string[] _RootElements = new string [1];
                _RootElements[0] = "Parent";
                return _RootElements;
            }
        }
        
        protected override object RawSchema {
            get {
                return _rawSchema;
            }
            set {
                _rawSchema = value;
            }
        }
    }
}
