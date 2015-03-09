using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;

namespace LolMatchBot
{
    /// <summary>
    /// A basic comment class containing the comment ID and the comments body
    /// Used for saving to our file
    /// </summary>
    class Comment
    {
        public string commentID, commentBody;

        public Comment(string commentID, string commentBody = "")
        {
            this.commentID = commentID;
            this.commentBody = commentBody;
        }
    }
}
