using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevHub.Models
{
    public class EnvironmentAction
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconGlyph { get; set; } = string.Empty;
        public string PathOrCommand { get; set; } = string.Empty;
        public string Arguments { get; set; } = string.Empty;
    }
}
