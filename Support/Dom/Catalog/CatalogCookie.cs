using System.Runtime.Serialization;

namespace Dom.Catalog
{
    [DataContract]
    public class CatalogCookie
    {
        public CatalogCookie()
        {
        }

        public CatalogCookie(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Name}={Value}";
        }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}
