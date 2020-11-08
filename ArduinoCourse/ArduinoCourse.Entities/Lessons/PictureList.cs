using System;
using System.Collections.Generic;
using System.IO;

namespace ArduinoCourse.Entities.Lessons
{
    public class PictureList
    {
        public List<string> Pics = new List<string>();

        public IEnumerable<FileStream> GetPics()
        {
            foreach (string s in Pics)
            {
                string path = string.Format("{0}{1}", Environment.CurrentDirectory, s);
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    yield return stream;
                }
            }
        }
    }
}
