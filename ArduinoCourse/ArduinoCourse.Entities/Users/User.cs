using ArduinoCourse.Entities.Lessons;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArduinoCourse.Entities.Users
{
    public class User
    {
        public long Id;

        public int ActualLesson;
        public int ActualLessonActualTheory;
        public int ActualLessonActualTest;

        public Lesson CurrentLesson;
        public int CurrentLessonActualTheory;
        public int CurrentLessonActualTest;
    }
}
