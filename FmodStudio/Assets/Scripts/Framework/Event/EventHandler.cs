using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThGold.Common;
using System;
using Sirenix.OdinInspector;

namespace ThGold.Event {
    public class EventHandler : MonoSingleton<EventHandler> {
        public EventDispatcher EventDispatcher;

        private void Awake() {
            if (EventDispatcher == null) {
                EventDispatcher = new EventDispatcher(this);
            }
        }
    }
}