using System;
using System.Collections.Generic;
using System.Text;

namespace ArduinoCourse.Entities.Lessons
{
    public class Test
    {
        public string Title;
        public PictureList Pics = new PictureList();
        public string Text;
        public List<string> Variants = new List<string>();
        public int Answer;
    }
}
