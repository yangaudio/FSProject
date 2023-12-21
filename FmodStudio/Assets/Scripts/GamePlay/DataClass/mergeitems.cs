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
    public class mergeitems : DefaultDataBase
    {
        private static DefaultDataBase _inst;
        public static DefaultDataBase Instance
        {
            get
            {
                if (_inst == null)
                {
                    _inst = new mergeitems();
                }
                return _inst;
            }
        }
        /// <summary>
        /// ID
        /// </summary>
        [XmlAttribute("ID")]
        public override int ID{ get; set; }

        /// <summary>
        /// 合成的ID
        /// </summary>
        [XmlAttribute("ResultID")]
        public int ResultID;

        /// <summary>
        /// id1
        /// </summary>
        [XmlAttribute("ItemID1")]
        public int ItemID1;

        /// <summary>
        /// id2
        /// </summary>
        [XmlAttribute("ItemID2")]
        public int ItemID2;

        /// <summary>
        /// 列表
        /// </summary>
        [XmlAttribute("ItemList")]
        public List<int> ItemList;

        protected override void LoadBytesInfo()
        {
           string xmlPath = Application.dataPath + "/Resources/XmlData/mergeitems.xml";
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
                 mergeitems data = new mergeitems();
                 data.ID= int.Parse(reader.GetAttribute("ID"));
                 data.ResultID= int.Parse(reader.GetAttribute("ResultID"));
                 data.ItemID1= int.Parse(reader.GetAttribute("ItemID1"));
                 data.ItemID2= int.Parse(reader.GetAttribute("ItemID2"));
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
