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
    public class dtnodes : DefaultDataBase
    {
        private static DefaultDataBase _inst;
        public static DefaultDataBase Instance
        {
            get
            {
                if (_inst == null)
                {
                    _inst = new dtnodes();
                }
                return _inst;
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        [XmlAttribute("ID")]
        public override int ID{ get; set; }

        /// <summary>
        /// 树ID
        /// </summary>
        [XmlAttribute("TreeID")]
        public int TreeID;

        /// <summary>
        /// 节点类型
        /// </summary>
        [XmlAttribute("NodeType")]
        public int NodeType;

        /// <summary>
        /// 父节点ID
        /// </summary>
        [XmlAttribute("ParentID")]
        public int ParentID;

        /// <summary>
        /// 说话人ID
        /// </summary>
        [XmlAttribute("SpeakerID")]
        public int SpeakerID;

        /// <summary>
        /// 节点Tag
        /// </summary>
        [XmlAttribute("Tag")]
        public String Tag;

        /// <summary>
        /// 对话
        /// </summary>
        [XmlAttribute("Content")]
        public string Content;

        /// <summary>
        /// 选项个数
        /// </summary>
        [XmlAttribute("ChoiceNum")]
        public int ChoiceNum;

        /// <summary>
        /// 选项1文本内容
        /// </summary>
        [XmlAttribute("Content_1")]
        public string Content_1;

        /// <summary>
        /// 选项2文本内容
        /// </summary>
        [XmlAttribute("Content_2")]
        public string Content_2;

        /// <summary>
        /// 选项3文本内容
        /// </summary>
        [XmlAttribute("Content_3")]
        public string Content_3;

        /// <summary>
        /// 选项4文本内容
        /// </summary>
        [XmlAttribute("Content_4")]
        public string Content_4;

        /// <summary>
        /// 判断类型
        /// </summary>
        [XmlAttribute("CheckType")]
        public int CheckType;

        /// <summary>
        /// 判断变量名
        /// </summary>
        [XmlAttribute("CheckName")]
        public string CheckName;

        /// <summary>
        /// 判断值
        /// </summary>
        [XmlAttribute("CheckValue")]
        public string CheckValue;

        protected override void LoadBytesInfo()
        {
           string xmlPath = Application.dataPath + "/Resources/XmlData/dtnodes.xml";
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
                 dtnodes data = new dtnodes();
                 data.ID= int.Parse(reader.GetAttribute("ID"));
                 data.TreeID= int.Parse(reader.GetAttribute("TreeID"));
                 data.NodeType= int.Parse(reader.GetAttribute("NodeType"));
                 data.ParentID= int.Parse(reader.GetAttribute("ParentID"));
                 data.SpeakerID= int.Parse(reader.GetAttribute("SpeakerID"));
                 data.Content= reader.GetAttribute("Content").ToString();
                 if(data.Content == "0")
                     data.Content = string.Empty;
                 data.ChoiceNum= int.Parse(reader.GetAttribute("ChoiceNum"));
                 data.Content_1= reader.GetAttribute("Content_1").ToString();
                 if(data.Content_1 == "0")
                     data.Content_1 = string.Empty;
                 data.Content_2= reader.GetAttribute("Content_2").ToString();
                 if(data.Content_2 == "0")
                     data.Content_2 = string.Empty;
                 data.Content_3= reader.GetAttribute("Content_3").ToString();
                 if(data.Content_3 == "0")
                     data.Content_3 = string.Empty;
                 data.Content_4= reader.GetAttribute("Content_4").ToString();
                 if(data.Content_4 == "0")
                     data.Content_4 = string.Empty;
                 data.CheckType= int.Parse(reader.GetAttribute("CheckType"));
                 data.CheckName= reader.GetAttribute("CheckName").ToString();
                 if(data.CheckName == "0")
                     data.CheckName = string.Empty;
                 data.CheckValue= reader.GetAttribute("CheckValue").ToString();
                 if(data.CheckValue == "0")
                     data.CheckValue = string.Empty;
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
