using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_dash.Model
{
    public class Report
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }

        public Report(string name, string id, string path, string description)
        {
            Description = description;
            Name = name;
            Id = id;
            Path = path;
        }
    }
}
