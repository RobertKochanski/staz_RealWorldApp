using RealWorldApp.Commons.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldApp.Commons.Models.CommentModel
{
    public class CommentResponseModelContainer
    {
        public CommentResponseModel Comment { get; set; }
    }

    public class CommentResponseModelContainerList
    {
        public List<CommentResponseModel> Comments { get; set; }
    }

    public class CommentResponseModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Body { get; set; }
        public ProfileResponse Author { get; set; }
    }
}
