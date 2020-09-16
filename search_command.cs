namespace WpfApp1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class search_command
    {
        public int id { get; set; }

        public string request { get; set; }

        public string command { get; set; }
    }
}
