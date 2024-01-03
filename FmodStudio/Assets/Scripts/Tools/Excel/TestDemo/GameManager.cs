using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using ThGold.Event;
using ThGold.Table;
using UnityEngine;
using Event = ThGold.Event.Event;
using EventHandler = ThGold.Event.EventHandler;

public class GameManager : MonoBehaviour {
    [ShowInInspector]
    public mergeitemsdata datas;

    public EventHandler EventHandler;
    private void Start() {
        EventHandler.EventDispatcher.AddEventListener(CustomEvent.DataTableLoadFailed, DataTableLoadFailed);
        /*dtnodesdata _dtnodesdata = new dtnodesdata();
        dogdata _dogdata = new dogdata();
        catdata _catdata = new catdata();*/
        StartCoroutine(LoadModules());
    }

    private void DataTableLoadFailed(IEvent ievent) {
        Debug.Log("Event" + ievent.data + "失败了？");
    }

    public IEnumerator LoadModules() {
        yield return new WaitForSeconds(2.0f); // 等待2秒
        datas = new mergeitemsdata();
    }
}