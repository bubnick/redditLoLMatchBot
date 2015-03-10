using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace LolMatchBot
{
    [Serializable()]
    public class CommentToSerialize : ISerializable
    {
        private HashSet<Comment> comments;

        public HashSet<Comment> Comments
        {
            get { return this.comments; }
            set { this.comments = value; }
        }
    

        public CommentToSerialize()
        {

        }

        public CommentToSerialize(SerializationInfo info, StreamingContext context)
        {
            this.comments = (HashSet<Comment>)info.GetValue("Comments", typeof(HashSet<Comment>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Comments", this.comments);
        }
    }
}
