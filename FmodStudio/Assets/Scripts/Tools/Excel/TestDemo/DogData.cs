/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThGold.Table;
public class DogData : LoadDataBase
{
    public Dictionary<int, DogDataInfo> DogInfoDic;
    public static DogData Instance;
    public override void Init()
    {
        Instance = this;
        DogInfoDic = new Dictionary<int, DogDataInfo>();
    }
    public override void InitLoadDataConfig()
    {
        ConfigList.Add(dog.Instance);
    }

    public override void LoadDataComplete()
    {
        var iter = ConfigList.GetEnumerator();
        List<DefaultDataBase> ls;
        DefaultDataBase ddb;
        while (iter.MoveNext())
        {
            ddb = iter.Current;
            ls = iter.Current.getdatas();
            if (ls == null)
            {
                continue;
            }
            if (ddb is dog)
            {
                for (int i = 0; i < ls.Count; i++)
                {
                    DogInfoDic.Add(ls[i].ID, new DogDataInfo((dog)ls[i]));
                }
            }
        }
        ls = null;
        ddb = null;
        iter.Dispose();

    }
    

    public override void Reset()
    {
    }
}
public class DogDataInfo
{
    public int ID;
    public string name;
    public int age;
    public int level;
    public int attack;
    public float health;
    public DogDataInfo(dog dog)
    {
        ID = dog.ID;
        name = dog.Name;
        age = dog.age;
        level = dog.level;
        attack = dog.attack;
        health = dog.health;
    }
}

/*
             string xmlPath = Application.dataPath + "/Resources/XmlData/dog.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            string xmlText = xmlDoc.InnerXml;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            settings.Async = true;
            loadData(XmlReader.Create(new StringReader(xmlText), settings));
     #1#
     */
