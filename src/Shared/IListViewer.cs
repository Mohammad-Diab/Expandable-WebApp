using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class IListViewer<T>
    {
        public List<T> Items { get; set; }

        public List<Property> Properties { get; set; }

        public int ItemsCount { get; set; }

        public int ItemsPerPage { get; set; }

        public int PagesCount { get; set; }

        public int CurrentPage { get; set; }

        public IListViewer()
        {
            Items = new List<T>();
            Properties = new List<Property>();
            ItemsCount = 0;
            ItemsPerPage = 10;
            PagesCount = 0;
        }

        public IListViewer(List<T> Items, List<Property> Properties, int ItemsCount, int ItemsPerPage, int CurrentPage)
        {
            this.Items = Items;
            this.Properties = Properties;
            this.ItemsCount = ItemsCount;
            this.ItemsPerPage = ItemsPerPage;
            this.CurrentPage = CurrentPage;
            PagesCount = (int)Math.Ceiling((ItemsCount + 0.0) / ItemsPerPage);
        }
    }
}
