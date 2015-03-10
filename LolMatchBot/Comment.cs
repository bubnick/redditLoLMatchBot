using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;
using System.Runtime.Serialization;

namespace LolMatchBot
{
    /// <summary>
    /// A basic comment class containing the comment ID and the comments body
    /// Used for saving to our file
    /// </summary>
    [Serializable()]
    public class Comment : ISerializable
    {
        public string commentID, commentBody;

        public Comment(string commentID, string commentBody = "")
        {
            this.commentID = commentID;
            this.commentBody = commentBody;
        }

        public Comment(SerializationInfo info, StreamingContext context)
        {
            this.commentID = (string) info.GetValue("commentID", typeof(string));
            this.commentBody = (string) info.GetValue("commentBody", typeof(string));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("commentID", this.commentID);
            info.AddValue("commentBody", this.commentBody);
        }
    }
}
