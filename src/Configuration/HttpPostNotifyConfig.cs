using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using dackup.Extensions;

namespace dackup.Configuration
{
    [Serializable]
    public class HttpPostNotifyConfig : NotifyConfigBase
    {
        [XmlElement(ElementName = "header")]
        public NameValueElementCollection Headers { get; set; }
    }

    [Serializable]
    public class HttpPostNotifyConfigCollection : KeyedCollection<string, HttpPostNotifyConfig>
    {
        protected override string GetKeyForItem(HttpPostNotifyConfig item)
        {
            return item.Name;
        }
    }
}