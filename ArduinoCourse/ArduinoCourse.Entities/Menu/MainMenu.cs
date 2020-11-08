using ArduinoCourse.Entities.Lessons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArduinoCourse.Entities.Menu
{
    public class MainMenu
    {
        public List<string> LessonsPaths = new List<string>();
        public List<string> LessonsFiles = new List<string>();
        public List<Lesson> Lessons = new List<Lesson>();
    }
}
