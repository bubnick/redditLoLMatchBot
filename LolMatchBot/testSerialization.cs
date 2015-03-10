using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LolMatchBot
{
    class testSerialization
    {
        /*static void Main(string[] args)
        {
            HashSet<Comment> comments = new HashSet<Comment>();

            for(int i = 0; i <= 5; i++)
            {
                comments.Add(new Comment("ID" + i, "Body" + i));
            }

            CommentToSerialize commentToSerialize = new CommentToSerialize();
            commentToSerialize.Comments = comments;

            Serializer serializer = new Serializer();
            serializer.SerializeComment("output.txt", commentToSerialize);
            comments = null;
            commentToSerialize = serializer.DeSerializeComment("output.txt");
            comments = commentToSerialize.Comments;
        }*/
    }
}
