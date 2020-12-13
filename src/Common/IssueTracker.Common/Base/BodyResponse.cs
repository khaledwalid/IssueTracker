using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Common.Base
{
    public class BodyResponse
    {
        public string Message { get; set; }
        public object Response { get; set; }
        public IEnumerable<string> ValidationErrors { get; set; }
    }
}
