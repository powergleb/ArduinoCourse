using System;
using System.Collections.Generic;
using System.Text;

namespace ArduinoCourse.Entities.Lessons
{
    public class Lesson
    {
        public string Title;
        public List<Theory> Theories = new List<Theory>();
        public List<Test> Tests = new List<Test>();
    }
}
