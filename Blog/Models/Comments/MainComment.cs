using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Models.Comments
{
    public class MainComment : Comment
    {
        public List<SubComment> SubComents { get; set; }
    }
}
