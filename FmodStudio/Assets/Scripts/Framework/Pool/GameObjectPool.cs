using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ThGold.Pool
{
    public class GameObjectPool
    {
        public int initialSize;

        private List<GameObject> PoolList;
        public GameObject prefab;
        public Transform Parent;

        public GameObjectPool(int initSize, GameObject prefab,Transform parent)
        {
             this.prefab = prefab;
             this.Parent = parent;
            PoolList = new List<GameObject>();
            //Debug.Log("Init" + prefab.gameObject.name);
            initialSize = initSize;
            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = GameObject.Instantiate(prefab);
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(parent);
                PoolList.Add(obj);
            }
        }

        public GameObject Get()
        {
            foreach (GameObject obj in PoolList)
            {
                if (!obj.gameObject.activeSelf)
                {
                    obj.gameObject.SetActive(true);
                    return obj;
                }
            }

            GameObject newObj = GameObject.Instantiate(prefab);
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(Parent);
            PoolList.Add(newObj);

            return newObj;
        }

        public void Return(GameObject obj)
        {
            obj.gameObject.SetActive(false);
            PoolList.Remove(obj);
            PoolList.Add(obj);
        }

        public List<GameObject> GetAllObjects()
        {
            return PoolList;
        }

        public void ReturnAll()
        {
            for (int i = 0; i < PoolList.Count; i++)
            {
                PoolList[i].gameObject.SetActive(false);
                PoolList.Remove(PoolList[i]);
                PoolList.Add(PoolList[i]);
            }
        }
    }
}