using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Client
{
    class XmlUtils
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string xml)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            StringReader sr = new StringReader(xml);
            T obj = (T)xs.Deserialize(sr);
            sr.Close();
            sr.Dispose();
            return obj;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string XmlSerializer<T>(T t)
        {
            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);
            XmlSerializer xs = new XmlSerializer(typeof(T));
            StringWriter sw = new StringWriter();
            xs.Serialize(sw, t, xsn);
            string str = sw.ToString();
            sw.Close();
            sw.Dispose();
            return str;
        }

        //下面两个实现同样的功能，备用
        //原本是采用序列化的方式将图片和声音转为字符串加到xml中去
        //现在采用base64编码，无需序列化

        /// <summary>
        /// 将Object类型对象(注：必须是可序列化的对象)转换为二进制序列字符串
        /// </summary>
        /// <param name="obj">字节对象</param>
        /// <returns></returns>
        public static string SerializeObject2(object obj)
        {
            IFormatter formatter = new BinaryFormatter();
            string result = string.Empty;
            using (MemoryStream stream=new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                byte[] buffer= new byte[stream.Length];
                buffer = stream.ToArray();
                result = Convert.ToBase64String(buffer);
                stream.Flush();
            }
            return result;
        }

        /// <summary>
        /// 将二进制序列字符串转换为Object类型对象
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static object DeserializeObject2(string str)
        {
            IFormatter formatter = new BinaryFormatter();
            byte[] byt = Convert.FromBase64String(str);
            object obj = null;
            using (Stream stream = new MemoryStream(byt, 0, byt.Length))
            {
                obj = formatter.Deserialize(stream);
            }
            return obj;
        }
    }
}
