using System;
using System.Collections.Generic;
using System.IO;

namespace ArduinoCourse.Entities.Lessons
{
    public class PictureList
    {
        public List<string> Pics = new List<string>();

        public IEnumerable<Stream> GetPics()
        {
            foreach (string s in Pics)
            {
                string path = string.Format("{0}{1}", Environment.CurrentDirectory, s);
                byte[] buffer = File.ReadAllBytes(path);
                MemoryStream stream = new MemoryStream(buffer);
                yield return stream;
            }
        }
    }
}
