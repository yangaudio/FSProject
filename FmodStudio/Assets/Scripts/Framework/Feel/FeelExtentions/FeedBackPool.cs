using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThGold.Common;
using ThGold.Pool;
using UnityEngine;
namespace ThGold.Feel
{
    public class FeedBackPool:MonoSingleton<FeedBackPool>
    {
        public GameObject ParticleFeedBack;
        public GameObject ParticleParent;
        public GameObjectPool ParticlePool;
        public void Start()
        {
           
        }
        public void Init()
        {
            ParticlePool = new GameObjectPool(5, ParticleFeedBack, ParticleParent.transform);
        }
    }
}
