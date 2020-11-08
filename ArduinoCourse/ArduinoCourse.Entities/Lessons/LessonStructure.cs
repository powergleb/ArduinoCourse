using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ArduinoCourse.Entities.Lessons
{
    public class LessonStructure
    {
        public XElement element = new XElement("Lesson",
                new XElement("Title", "test_lesson_1"),
                new XElement("Theories",
                    new XElement("Theory",
                        new XElement("Title", "info11"),
                        new XElement("Text", "test_text_11"),
                        new XElement("Pics")),
                    new XElement("Theory",
                        new XElement("Title", "info11"),
                        new XElement("Text", "test_text_11"),
                        new XElement("Pics",
                            new XElement("Pic", "\\lessons\\test_lesson_1\\pic\\pic1.jpg")))),
                new XElement("Tests",
                    new XElement("Test",
                        new XElement("Title", "test11"),
                        new XElement("Pics"),
                        new XElement("Text", "test_test_11"),
                        new XElement("Variants",
                            new XElement("Variant", "False1"),
                            new XElement("Variant", "False2"),
                            new XElement("Variant", "True"),
                            new XElement("Variant", "False3")),
                        new XElement("Answer", 2)),
                    new XElement("Test",
                        new XElement("Title", "test12"),
                        new XElement("Pics",
                         new XElement("Pic", "\\lessons\\test_lesson_1\\pic\\pic1.jpg")),
                        new XElement("Text", "test_test_12"),
                        new XElement("Variants",
                            new XElement("Variant", "False1"),
                            new XElement("Variant", "False2"),
                            new XElement("Variant", "True"),
                            new XElement("Variant", "False3")),
                        new XElement("Answer", 2))));
    }
}
