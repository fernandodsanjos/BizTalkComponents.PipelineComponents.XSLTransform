namespace BizTalkComponents.PipelineComponents {
    using Microsoft.XLANGs.BaseTypes;
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [SchemaType(SchemaTypeEnum.Document)]
    [Schema(@"http://BizTalkComponents.PipelineComponents.MLDocument",@"Order")]
    [BodyXPath(@"/*[local-name()='Order' and namespace-uri()='http://BizTalkComponents.PipelineComponents.MLDocument']/*[local-name()='Lines' and namespace-uri()='']")]
    [System.SerializableAttribute()]
    [SchemaRoots(new string[] {@"Order"})]
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"BizTalkComponents.PipelineComponents.ProductionItemType", typeof(global::BizTalkComponents.PipelineComponents.ProductionItemType))]
    public sealed class OrderType : Microsoft.XLANGs.BaseTypes.SchemaBase {
        
        [System.NonSerializedAttribute()]
        private static object _rawSchema;
        
        [System.NonSerializedAttribute()]
        private const string _strSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns=""http://BizTalkComponents.PipelineComponents.MLDocument"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" targetNamespace=""http://BizTalkComponents.PipelineComponents.MLDocument"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:include schemaLocation=""BizTalkComponents.PipelineComponents.ProductionItemType"" />
  <xs:annotation>
    <xs:appinfo>
      <b:schemaInfo is_envelope=""yes"" root_reference=""Order"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" />
    </xs:appinfo>
  </xs:annotation>
  <xs:element name=""Order"">
    <xs:annotation>
      <xs:appinfo>
        <b:recordInfo body_xpath=""/*[local-name()='Order' and namespace-uri()='http://BizTalkComponents.PipelineComponents.MLDocument']/*[local-name()='Lines' and namespace-uri()='']"" />
      </xs:appinfo>
    </xs:annotation>
    <xs:complexType>
      <xs:sequence>
        <xs:element name=""Lines"">
          <xs:complexType>
            <xs:sequence>
              <xs:element maxOccurs=""unbounded"" ref=""Item"" />
            </xs:sequence>
          </xs:complexType>
        </xs:element>
      </xs:sequence>
      <xs:attribute name=""Id"" type=""xs:string"" />
      <xs:attribute name=""date"" type=""xs:date"" />
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        public OrderType() {
        }
        
        public override string XmlContent {
            get {
                return _strSchema;
            }
        }
        
        public override string[] RootNodes {
            get {
                string[] _RootElements = new string [1];
                _RootElements[0] = "Order";
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
