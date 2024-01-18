using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThGold.Common;
using MoreMountains.Feedbacks;
using static MoreMountains.Feedbacks.MMF_Fade;
using MoreMountains.FeedbacksForThirdParty;
using System;
using UnityEngine.SceneManagement;
using ThGold.Pool;
namespace ThGold.Feel
{
    /// <summary>
    /// 主要是对象池我估计很多,统一在Handler存储
    /// </summary>
    public class FeedBackPoolHandler
    {
        private FeedBackManager _feedBackManager;
        private Dictionary<string, MMFeedbacks> _feedbacksDic;


        #region Pools
        //需要什么Pools添加什么Pools
        #endregion
        public FeedBackPoolHandler()
        {

        }
        public FeedBackPoolHandler(FeedBackManager _manager, Dictionary<string, MMFeedbacks> _dic)
        {
            _feedBackManager = _manager;
            _feedbacksDic = _dic;
        }
    }
}
