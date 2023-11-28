using System;
using System.Collections.Generic;
using ThGold.Event;
using EventHandler = ThGold.Event.EventHandler;
using UnityEngine;
namespace ThGold.Table
{
    public abstract class LoadDataBase
    {
        private bool isLoadDataSucceed = false;
        private bool isLoadDataListSucceed = false;
        public bool IsLoadDataSucceed { get => isLoadDataListSucceed; set { isLoadDataSucceed = value; } }
        public bool IsLoadDataListSucceed => isLoadDataListSucceed;
        protected List<DefaultDataBase> ConfigList = new List<DefaultDataBase>();
        public LoadDataBase()
        {
            //EventHandler.Instance.EventDispatcher.AddEventListener(CustomEvent.DataTableLoadSucceed, OnLoadDataSucceed);
            InitLoadDataConfig();
            Init();
            LoadDataTables();
        }
        public LoadDataBase(float delayLoadData)
        {
            //EventHandler.Instance.EventDispatcher.AddEventListener(CustomEvent.DataTableLoadSucceed, OnLoadDataSucceed);
            InitLoadDataConfig();
            Init();
            LoadDataTables();
            //Timers.inst.Add(delayLoadData, 1, DelayLoadDataTables);
        }

        private void DelayLoadDataTables(object param)
        {
            LoadDataTables();
        }

        private void LoadDataTables()
        {
            if (IsLoadDataListSucceed)
            {
                if (ConfigList.Count > 0)
                    // EventHandler.Instance.EventDispatcher.DispatchEvent(CustomEvent.DataTableLoadSucceed, ConfigList[0].ToString());
                return;
            }
            bool isLoadALl = true;
            for (int i = 0; i < ConfigList.Count; i++)
            {
                if (!ConfigList[i].IsLoadSucceed)
                {
                    isLoadALl = false;
                    ConfigList[i].LoadBytes();
                }
            }
            if (isLoadALl)
            {
                isLoadDataListSucceed = true;
                LoadDataComplete();
            }
        }

        /// <summary>
        /// 初始化函数
        /// </summary>       
        public virtual void Init()
        {
            //LoadDataTable();
        }
        [Obsolete("这种加载方法已经过时了;使用 InitLoadDataConfig / LoadDataComplete 来初始化表", true)]
        public virtual void Refresh()
        {
            LoadDataTable();
        }
        /// <summary>
        /// 加载数据
        /// </summary>
        [Obsolete("这种加载方法已经过时了;使用 InitLoadDataConfig / LoadDataComplete 来初始化表", true)]
        public virtual void LoadDataTable()
        {
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// 初始化 加载数据表 list
        /// </summary>
        public virtual void InitLoadDataConfig()
        {
        }
        /// <summary>
        /// 加载数据表 list 完成
        /// </summary>
        public virtual void LoadDataComplete(IEvent i)
        {

        }
        public virtual void LoadDataComplete(Action action = null)
        {
            action?.Invoke();
        }
        /// <summary>
        /// 加载数据表 list 完成
        /// </summary>
        public virtual void LoadDataCompleteCallBack()
        {

        }
        /// <summary>
        /// 加载事件
        /// </summary>        
        public void OnLoadDataSucceed(IEvent context)
        {
            Debug.Log("Event----"+context.data);
            if (isLoadDataListSucceed || context == null || context.data == null)
            {
                return;
            }
            string flag = context.data.ToString();
            DefaultDataBase ddb = ConfigList.Find((config) =>
            {
                return config.ToString() == flag;
            });
            if (ddb != null)
            {
                ddb = ConfigList.Find((config) =>
                {
                    return config.IsLoadSucceed == false;
                });
                if (ddb == null)
                {
                    isLoadDataListSucceed = true;
                    LoadDataComplete();
                }
            }
        }
    }
}
