using System.Xml.Serialization;

namespace org.xepher.model
{
    [XmlRoot(ElementName = "appupdate")]
    public class AppUpdate
    {
        [XmlElement(ElementName = "appVersionCode")]
        public int VersionCode;
        [XmlElement(ElementName = "appName")]
        public string Name;
        [XmlElement(ElementName = "appUrl")]
        public string Url;
        [XmlElement(ElementName = "appDesc")]
        public string Desc;
        [XmlElement(ElementName = "appForceUpdate")]
        public bool ForceUpdate;
        [XmlElement(ElementName = "appSize")]
        public double Size;
    }
}
