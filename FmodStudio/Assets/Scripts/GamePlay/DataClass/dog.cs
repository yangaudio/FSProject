using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;

namespace ThGold.Table
{
    [Serializable]
    public class dog : DefaultDataBase
    {
        private static DefaultDataBase _inst;
        public static DefaultDataBase Instance
        {
            get
            {
                if (_inst == null)
                {
                    _inst = new dog();
                }
                return _inst;
            }
        }
        /// <summary>
        /// 狗的ID
        /// </summary>
        [XmlAttribute("ID")]
        public override int ID{ get; set; }

        /// <summary>
        /// 狗的名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name;

        /// <summary>
        /// 狗的年龄
        /// </summary>
        [XmlAttribute("age")]
        public int age;

        /// <summary>
        /// 狗的等级
        /// </summary>
        [XmlAttribute("level")]
        public int level;

        /// <summary>
        /// 狗的攻击
        /// </summary>
        [XmlAttribute("attack")]
        public int attack;

        /// <summary>
        /// 狗的爱
        /// </summary>
        [XmlAttribute("love")]
        public float love;

        /// <summary>
        /// 狗的血量
        /// </summary>
        [XmlAttribute("health")]
        public float health;

        protected override void LoadBytesInfo()
        {
           string xmlPath = Application.dataPath + "/Resources/XmlData/dog.xml";
            XmlDocument xmlDoc = new XmlDocument();
           xmlDoc.Load(xmlPath);
             string xmlText = xmlDoc.InnerXml;
            XmlReaderSettings settings = new XmlReaderSettings();
           settings.IgnoreComments = true;
           settings.IgnoreWhitespace = true;
           settings.Async = true;
           loadData(XmlReader.Create(new StringReader(xmlText), settings));
        }

        protected async override void loadDataInfo(XmlReader reader)
        {
            if (reader == null)
                return;
            while (await reader.ReadAsync())
            {
            try
            {
                 if (reader.NodeType != XmlNodeType.Element || !reader.HasAttributes)
                     continue;
                 dog data = new dog();
                 data.ID= int.Parse(reader.GetAttribute("ID"));
                 data.Name= reader.GetAttribute("Name").ToString();
                 if(data.Name == "0")
                     data.Name = string.Empty;
                 data.age= int.Parse(reader.GetAttribute("age"));
                 data.level= int.Parse(reader.GetAttribute("level"));
                 data.attack= int.Parse(reader.GetAttribute("attack"));
                 data.love= float.Parse(reader.GetAttribute("love"));
                 data.health= float.Parse(reader.GetAttribute("health"));
                 lock (datas)
                 {
                     datas.Add(data);
                 };
            }
            catch (Exception e)
            {
                FailDispatchEvent();
                return;
            }
            }
            IsLoadSucceed = true;
            SucceedDispatchEvent(this.GetType().Name);
        }
    }
}
