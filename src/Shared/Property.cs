using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class Property
    {
        public string Name { get; set; }

        public string Label { get; set; }

        public int Order { get; set; }

        public PropertyType Type { get; set; }

        public bool IsFilterable { get; set; }

        public Property()
        {
            Name = "";
            Label = "";
            Order = 0;
            Type = PropertyType.String;
            IsFilterable = false;
        }

        public Property(string Name, string Label, int Order, PropertyType Type, bool IsFilterable)
        {
            this.Name = Name;
            this.Label = Label;
            this.Order = Order;
            this.IsFilterable = IsFilterable;
            this.Type = Type;
        }
    }


    public enum PropertyType
    {
        Hidden = 0,
        String = 1,
        Number = 2
    }
}
