namespace BizTalkComponents.PipelineComponents {
    using Microsoft.XLANGs.BaseTypes;
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [SchemaType(SchemaTypeEnum.Document)]
    [Schema(@"http://XSLTransform.Schema.Schema",@"MotherOf_x0020_ALLRoots")]
    [System.SerializableAttribute()]
    [SchemaRoots(new string[] {@"MotherOf_x0020_ALLRoots"})]
    public sealed class Schema_Transform_Source : Microsoft.XLANGs.BaseTypes.SchemaBase {
        
        [System.NonSerializedAttribute()]
        private static object _rawSchema;
        
        [System.NonSerializedAttribute()]
        private const string _strSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns=""http://XSLTransform.Schema.Schema"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" targetNamespace=""http://XSLTransform.Schema.Schema"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element name=""MotherOf_x0020_ALLRoots"">
    <xs:complexType>
      <xs:sequence>
        <xs:element name=""Record"">
          <xs:complexType>
            <xs:sequence>
              <xs:element name=""ID"" type=""xs:string"" />
              <xs:element minOccurs=""0"" name=""MessageID"" type=""xs:string"" />
            </xs:sequence>
          </xs:complexType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        public Schema_Transform_Source() {
        }
        
        public override string XmlContent {
            get {
                return _strSchema;
            }
        }
        
        public override string[] RootNodes {
            get {
                string[] _RootElements = new string [1];
                _RootElements[0] = "MotherOf_x0020_ALLRoots";
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
