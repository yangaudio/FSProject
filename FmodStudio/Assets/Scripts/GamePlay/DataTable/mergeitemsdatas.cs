using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;
using ThGold.Event;
using EventHandler = ThGold.Event.EventHandler;

namespace ThGold.Table
{
    [Serializable]
public class mergeitemsdata : LoadDataBase
{
    public Dictionary<int, mergeitems> Datas;
    public static mergeitemsdata Instance;
    public override void Init()
    {
        Instance = this;
        Datas = new Dictionary<int, mergeitems>();
        EventHandler.Instance.EventDispatcher.AddEventListener("mergeitems"+CustomEvent.DataTableLoadSucceed,LoadDataComplete,EventDispatcherAddMode.SINGLE_SHOT);
    }
    public override void InitLoadDataConfig()
    {
        ConfigList.Add(mergeitems.Instance);
    }
    public override void LoadDataComplete(IEvent e)
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
            if (ddb is mergeitems)
            {
                for (int i = 0; i < ls.Count; i++)
                {
                    Datas.Add(ls[i].ID, (mergeitems)ls[i]);
                    Debug.Log(Datas[ls[i].ID].ID);
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
  }
