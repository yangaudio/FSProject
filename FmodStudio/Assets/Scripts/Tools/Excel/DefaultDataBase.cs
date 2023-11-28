using System.Collections.Generic;
using System.Xml;
using Excel;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;
using ThGold.Event;
namespace ThGold.Table
{
    public abstract class DefaultDataBase
    {
        protected List<DefaultDataBase> datas = new List<DefaultDataBase>();
        private bool isLoadSucceed = false;
        private bool isLoading = false;
        public bool IsLoadSucceed
        {
            get { return isLoadSucceed; }
            set { isLoadSucceed = value; }
        }
        public abstract int ID { get; set; }
        protected abstract void LoadBytesInfo();
        protected abstract void loadDataInfo(XmlReader doc);
        internal void LoadBytes()
        {
            if (isLoadSucceed)
                return;
            if (isLoading)
                return;
            isLoading = true;
            LoadBytesInfo();

        }
        protected void loadData(XmlReader obj)
        {
            XmlReader doc = obj;
            if (doc == null)
            {
                FailDispatchEvent();
                return;
            }
            try
            {
                loadDataInfo(doc);
                return;
            }
            catch (System.Exception)
            {
                FailDispatchEvent();
            }
            finally
            {
                doc.Dispose();
            }
            Debug.LogError("loadData Error: Table=" + this.ToString());
        }
        internal List<DefaultDataBase> getdatas()
        {
            if (isLoadSucceed)
            {
                return datas;
            }
            else
            {
                return null;
            }
        }
        protected virtual void SucceedDispatchEvent()
        {
            EventHandler.Instance.EventDispatcher.DispatchEvent(CustomEvent.DataTableLoadSucceed, this.ToString());
        }
        protected virtual void SucceedDispatchEvent(string childClassName)
        {
            EventHandler.Instance.EventDispatcher.DispatchEvent(childClassName+CustomEvent.DataTableLoadSucceed, this.ToString());
        }
        protected virtual void FailDispatchEvent()
        {
            EventHandler.Instance.EventDispatcher.DispatchEvent(CustomEvent.DataTableLoadFailed, this.ToString());
        }
    }
}