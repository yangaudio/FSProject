using System;
using System.Collections;
using System.Collections.Generic;
using ThGold.Event;
using ThGold.Table;
using UnityEngine;
using Event = ThGold.Event.Event;
using EventHandler = ThGold.Event.EventHandler;

public class GameManager : MonoBehaviour {
    private void Start() {
        EventHandler.Instance.EventDispatcher.AddEventListener(CustomEvent.DataTableLoadFailed, DataTableLoadFailed);
        /*dtnodesdata _dtnodesdata = new dtnodesdata();
        dogdata _dogdata = new dogdata();
        catdata _catdata = new catdata();*/
    }

    private void DataTableLoadFailed(IEvent ievent) {
        Debug.Log("Event" + ievent.data + "失败了？");
    }
}