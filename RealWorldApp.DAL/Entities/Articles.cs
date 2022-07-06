using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldApp.DAL.Entities
{
    public class Articles : BaseEntitie
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ArticleText { get; set; }
        ICollection<Tags> Tags { get; set; }

        public int UserId { get; set; }
    }
}
