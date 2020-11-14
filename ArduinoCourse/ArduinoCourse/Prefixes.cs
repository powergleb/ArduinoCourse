using System;
using System.Collections.Generic;
using System.Text;

namespace ArduinoCourse
{
    public enum Prefixes
    {
        Error,
        ToMenu,
        LessonAtMenu,
        LessonTheory,
        LessonTest,
        LessonTestAnswer
    }

    public static class PrefixesExtensions
    {
        public static Prefixes ToPrefix(this string s)
        {
            switch (s)
            {
                case "mm":
                    return Prefixes.ToMenu;
                case "lm":
                    return Prefixes.LessonAtMenu;
                case "lt":
                    return Prefixes.LessonTheory;
                case "lq":
                    return Prefixes.LessonTest;
                case "la":
                    return Prefixes.LessonTestAnswer;
            }
            return Prefixes.Error;
        }
        public static int Parts(this Prefixes prefix)
        {
            switch (prefix)
            {
                case Prefixes.ToMenu:
                    return 1;
                case Prefixes.LessonAtMenu:
                    return 2;
                case Prefixes.LessonTheory:
                    return 3;
                case Prefixes.LessonTest:
                    return 3;
                case Prefixes.LessonTestAnswer:
                    return 4;
            }
            return -1;
        }
    }
}
