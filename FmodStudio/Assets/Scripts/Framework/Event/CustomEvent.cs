using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThGold.Event
{
    public class CustomEvent
    {
        public const string Buy = "Buy";
        public const string TimeOut = "TimeOut";
        public const string TimeReset = "TimeReset";
        public const string CompleteInter = "CompleteInter";
        public const string MeetNpc = "MeetNpc";
        public const string CreateNpc = "CreateNpc";
        public const string GameOver = "GameOver";

        public const string DataTableLoadSucceed = "DataTableLoadSucceed";
        public const string DataTableLoadFailed = "DataTableLoadFailed";
    }
}