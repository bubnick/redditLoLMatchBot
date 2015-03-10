using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace LolMatchBot
{
    public class Serializer
    {
        public void SerializeComment(string filename, CommentToSerialize commentToSerialize)
        {
            Stream stream = File.Open(filename, FileMode.Create);

            BinaryFormatter bFormat = new BinaryFormatter();

            bFormat.Serialize(stream, commentToSerialize);

            stream.Close();
        }

        public CommentToSerialize DeSerializeComment(string filename)
        {
            CommentToSerialize commentToSerialize;

            Stream stream = File.Open(filename, FileMode.Open);

            BinaryFormatter bFormat = new BinaryFormatter();

            commentToSerialize = (CommentToSerialize)bFormat.Deserialize(stream);

            stream.Close();

            return commentToSerialize;
        }
    }    
}
