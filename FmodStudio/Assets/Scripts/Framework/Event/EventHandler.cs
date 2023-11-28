using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThGold.Common;
using System;

namespace ThGold.Event
{
    public class EventHandler : MonoSingleton<EventHandler>
    {
        public EventDispatcher EventDispatcher;
        private void Awake()
        {
            EventDispatcher = new EventDispatcher(this);
        }

    }
}
