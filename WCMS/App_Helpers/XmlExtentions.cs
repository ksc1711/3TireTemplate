using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace WCMS.Web
{
    public static class XmlExtentions
    {
        /// <summary>
        /// json type string To XDocument
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static XDocument jsonToXDocument(this string jsonString)
        {
            try
            {
                XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonString);

                using (var nodeReader = new XmlNodeReader(doc))
                {
                    nodeReader.MoveToContent();
                    return XDocument.Load(nodeReader);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// json type string To XDocument
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static XDocument xmlToXDocument(this string xmlString)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                if(xmlString.Contains("<?xml version=\"1.0\" encoding=\"utf-8\"?>"))
                {
                    //xmlString = xmlString.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
                }
                doc.LoadXml(xmlString);

                using (var nodeReader = new XmlNodeReader(doc))
                {
                    nodeReader.MoveToContent();
                    return XDocument.Load(nodeReader);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public static string xmlSerialize<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                var stringWriter = new System.IO.StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }
    }
}