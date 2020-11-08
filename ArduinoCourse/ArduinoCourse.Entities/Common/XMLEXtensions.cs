using ArduinoCourse.Entities.Lessons;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ArduinoCourse.Entities.Common
{
    public static class XMLEXtensions
    {
        #region PictureList
        public static XElement ToXML(this PictureList list)
        {
            XElement element = new XElement("Pics");

            foreach(string path in list.Pics)
            {
                element.Add(new XElement("Pic", path));
            }

            return element;
        }

        public static PictureList ToPictureList(this XElement element)
        {
            PictureList res = new PictureList();
            foreach(XElement e in element.Elements("Pic"))
            {
                res.Pics.Add(e.Value);
            }
            return res;
        }
        #endregion

        #region Theory
        public static XElement ToXML(this Theory t)
        {
            XElement element = new XElement("Theory");

            element.Add(new XElement("Title", t.Title));

            element.Add(new XElement("Text", t.Text));

            element.Add(t.Pics.ToXML());

            return element;
        }

        public static Theory ToTheory(this XElement element)
        {
            Theory res = new Theory();

            res.Title = element.Element("Title").Value;

            res.Text = element.Element("Text").Value;

            res.Pics = element.Element("Pics").ToPictureList();

            return res;
        }
        #endregion

        #region Test
        public static XElement ToXML(this Test t)
        {
            XElement element = new XElement("Test");

            element.Add(new XElement("Title", t.Title));

            element.Add(t.Pics.ToXML());

            element.Add(new XElement("Text", t.Text));

            XElement variants = new XElement("Variants");
            foreach(string variant in t.Variants)
            {
                variants.Add("Variant", variant);
            }
            element.Add(variants);

            element.Add(new XElement("Answer", t.Answer));

            return element;
        }

        public static Test ToTest(this XElement element)
        {
            Test res = new Test();

            res.Title = element.Element("Title").Value;

            res.Pics = element.Element("Pics").ToPictureList();

            res.Text = element.Element("Text").Value;

            XElement variants = element.Element("Variants");
            foreach(var variant in variants.Elements("Variant"))
            {
                res.Variants.Add(variant.Value);
            }

            res.Answer = int.Parse(element.Element("Answer").Value);

            return res;
        }
        #endregion

        #region Lesson
        public static XElement ToXML(this Lesson lesson)
        {
            XElement theories = new XElement("Theories");

            foreach (var t in lesson.Theories)
            {
                theories.Add(t.ToXML());
            }

            XElement tests = new XElement("Tests");

            foreach (var t in lesson.Tests)
            {
                theories.Add(t.ToXML());
            }

            XElement element = new XElement("Lesson", 
                new XElement("Title", lesson.Title),
                theories, tests);


            return element;
        }

        public static Lesson ToLesson(this XElement element)
        {
            Lesson res = new Lesson();

            res.Title = element.Element("Title").Value;

            foreach (var t in element.Element("Theories").Elements("Theory"))
            {
                res.Theories.Add(t.ToTheory());
            }

            foreach (var t in element.Element("Tests").Elements("Test"))
            {
                res.Tests.Add(t.ToTest());
            }

            return res;
        }
        #endregion
    }
}
