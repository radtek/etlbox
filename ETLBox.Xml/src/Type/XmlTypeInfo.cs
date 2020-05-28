﻿using ETLBox.DataFlow;
using System;
using System.Xml.Serialization;

namespace ETLBox.Xml
{
    internal class XmlTypeInfo : TypeInfo
    {
        internal string ElementName { get; set; }
        internal XmlTypeInfo(Type typ) : base(typ)
        {
            GatherTypeInfo();
            foreach (System.Attribute attr in Attribute.GetCustomAttributes(typ))
            {
                if (attr is XmlRootAttribute)
                {
                    var a = attr as XmlRootAttribute;
                    ElementName = a.ElementName;
                }
            }
            if (String.IsNullOrWhiteSpace(ElementName))
                ElementName = typ.Name;
        }

    }
}

