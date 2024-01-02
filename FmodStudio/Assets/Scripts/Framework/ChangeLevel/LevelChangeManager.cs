using System;
using System.Collections;
using System.Collections.Generic;
using ThGold.Common;
using UnityEngine;
using ThGold.Event;
using EventHandler = ThGold.Event.EventHandler;

namespace ThGold {
    public class LevelChangeManager : MonoSingleton<LevelChangeManager> {
        List<ILevelChange> AllLevelChangeObjs = new List<ILevelChange>();

        private void Start() {
            
        }
    }
}