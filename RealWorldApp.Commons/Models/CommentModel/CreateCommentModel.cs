using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldApp.Commons.Models.CommentModel
{
    public class CreateCommentModelContainer
    {
        public CreateCommentModel Comment { get; set; }
    }

    public class CreateCommentModel
    {
        public string Body { get; set; }
    }
}
