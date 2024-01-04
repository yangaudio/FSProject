using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using ThGold.Common;
using Sirenix.OdinInspector;

namespace ThGold.Wwise
{
    public class WwiseManager : MonoSingleton<WwiseManager>
    {
        //PC
        private WwiseController _WwiseController => WwiseController.Instance;

        //string WwisePath = Application.streamingAssetsPath + "/Audio/GeneratedSoundBanks/Windows/SoundbanksInfo.xml";
        string WwisePath;

        private string m_WwiseGlobalPrefabName = "GlobalAudio";

        //����ʽAkInitializer
        public AkWwiseInitializationSettings LocalAkInitializer;

        //ȫ�ֿ��ƿ���

        public bool isPlay = true;

        //��������

        // public float _SoundVolume => _WwiseController.GlobalSoundValue;
        public float _SoundVolume = 1.0f;

        // public float _BGMSoundVolume => _WwiseController.GlobalBGMSoundValue;
        public float _BGMSoundVolume = 1.0f;
        [Tooltip("������")] public GameObject listener;
        [Tooltip("������")] public GameObject BGMListener;
        [Tooltip("����������λ��")] public GameObject akGameobject;

        //ӳ���
        [ShowInInspector] private Dictionary<string, string> m_BankInfoDict = new Dictionary<string, string>();
        [ShowInInspector] private Dictionary<uint, string> m_UnitInfoDict = new Dictionary<uint, string>();

        //����bank��List
        [SerializeField] private List<string> m_LoadBankList = new List<string>();

        //�洢��ʱ���¼����

        private List<uint> EventIDList = new List<uint>();

        private Dictionary<string, uint> EventIDListDic = new Dictionary<string, uint>();

        private void Awake()
        {
            DontDestroyOnLoad(this);
            InitPath();
            Init();
        }
        
        private void InitPath()
        {
#if UNITY_STANDALONE_WIN
            WwisePath = Application.streamingAssetsPath + "/Audio/GeneratedSoundBanks/Windows/SoundbanksInfo.xml";
#endif
#if UNITY_EDITOR

            WwisePath = "Assets/StreamingAssets/Audio/GeneratedSoundBanks/Windows/SoundbanksInfo.xml";
#endif
        }

        private void Init()
        {
            //��ʼ��AkInitializer ����AkWwiseInitializationSettings
            GameObject global = new GameObject(m_WwiseGlobalPrefabName);
            global.SetActive(false);
            GameObject.DontDestroyOnLoad(global);
            AkInitializer akInitializer = global.AddComponent<AkInitializer>();
            //string initSettingPath = "Assets/Resources/ScriptableObjects/AkWwiseInitializationSettings";
            
              object settingObj = Resources.Load("ScriptableObjects/AkWwiseInitializationSettings");
            // object settingObj =
            //     UnityEditor.AssetDatabase.LoadAssetAtPath<AkWwiseInitializationSettings>(initSettingPath);
            if (settingObj != null)
            {
                AkWwiseInitializationSettings settings = settingObj as AkWwiseInitializationSettings;
                akInitializer.InitializationSettings = settings;
                global.SetActive(true);
            }
            else
            {
                if (LocalAkInitializer != null)
                {
                    AkWwiseInitializationSettings settings = LocalAkInitializer as AkWwiseInitializationSettings;
                    akInitializer.InitializationSettings = settings;
                    global.SetActive(true);
                }
                else
                {
                    Debug.LogError("AkWwiseInitializationSettings NULL");
                    
                }
            }

            InitXML();
        }

        #region ����xml

        private void InitXML()
        {
            //  WWW www = new WWW(WwisePath);

            Debug.Log("InitXML");
            if (!string.IsNullOrEmpty(WwisePath))

            {
                XmlDocument XmlDoc = new XmlDocument();

                XmlDoc.Load(WwisePath);

                //���Ȼ�ȡxml�����е�SoundBank
                Debug.Log(XmlDoc);
                XmlNodeList soundBankList = XmlDoc.GetElementsByTagName("SoundBank");
                Debug.Log("soundBankList:" + soundBankList.Count);
                foreach (XmlNode node in soundBankList)
                {
                    XmlNode bankNameNode = node.SelectSingleNode("ShortName");

                    string bankName = bankNameNode.InnerText;

                    //�ж�SingleNode�������,����Init.bak��û���

                    // XmlNode eventNode = node.SelectSingleNode("IncludedEvents");
                    XmlNode eventNode = node.SelectSingleNode("Events");
                    //
                    if (eventNode != null)
                    {
                        //�õ��������е�event��һ��ӳ��
                        var obj = eventNode.SelectNodes("Event");
                        XmlNodeList eventList = null;
                        if (obj != null)
                        {
                            eventList = obj;
                        }
                        else
                        {
                            return;
                        }
                        foreach (XmlElement x1e in eventList)
                        {
                            //   m_BankInfoDict.Add(uint.Parse(x1e.Attributes["Id"].Value), bankName);
                            m_BankInfoDict.Add(x1e.Attributes["Name"].Value, bankName);
                            m_UnitInfoDict.Add(uint.Parse(x1e.Attributes["Id"].Value), x1e.Attributes["Name"].Value);
                            Debug.Log(x1e.Attributes["Name"].Value + ":" + bankName);
                        }
                    }
                }
            }

            else

            {
                Debug.LogError("·������" + WwisePath);
            }
        }

        private uint GetEventIDByName(string EventName)
        {
            foreach (var Event in m_UnitInfoDict)
            {
                if (Event.Value.Equals(EventName))
                {
                    return Event.Key;
                }
            }

            return 0;
        }

        #endregion

        public void PlaySoundBGM(string soundName, GameObject gameObject)
        {
            if (string.IsNullOrEmpty(soundName))
            {
                return;
            }

            if (!isPlay)
            {
                return;
            }

            PlayEvent("Play", soundName, gameObject, _BGMSoundVolume, true, VolumeType.BGM);
        }

        public void PlaySound(string soundName, GameObject gameObject)
        {
            if (string.IsNullOrEmpty(soundName))
            {
                return;
            }

            if (!isPlay)
            {
                return;
            }

            PlayEvent("Play", soundName, gameObject, _SoundVolume, true);
        }

        private void PlayEvent(string handName, string eventName, GameObject gameObject, float volume,
            bool finishCallback = false, VolumeType type = VolumeType.Sound)
        {
            string resourceName = eventName;
            string bankName;

            //m_BankInfoDict��Event��SoundBank��ӳ���ϵ�ֵ�
            if (!m_BankInfoDict.TryGetValue(resourceName, out bankName))
            {
                Debug.LogError(string.Format("����event��{0}��ʧ��,û���ҵ�������SoundBank", resourceName));
                return;
            }

            if (!m_LoadBankList.Contains(bankName))
            {
                //����SoundBank
                AkBankManager.LoadBank(bankName, false, false);
                m_LoadBankList.Add(bankName);
            }

            if (gameObject == null)
            {
                //����һ��Ԥ���壬Ԥ�����ǿյ�
                //gameObject = new GameObject("obj_temp");
                Debug.LogError("WwsieManager:Broadcast GameObject IsNull should set currently gameobject");
                return;
            }

            if (gameObject.GetComponent<AkGameObj>() == null)
            {
                gameObject.AddComponent<AkGameObj>();
                Debug.LogError(
                    "WwsieManager:Broadcast GameObject haven't Component<AkGameObj> should check gameobject");
            }
            
            uint eventID = AkSoundEngine.PostEvent(resourceName, (UnityEngine.GameObject)gameObject, 1, AkCallback, null);
            Debug.Log("eventID:" + eventID);
            //��������
            if (type == VolumeType.BGM)
            {
                AKRESULT aKRESULT = AkSoundEngine.SetGameObjectOutputBusVolume(gameObject, BGMListener, volume);
                Debug.Log("SetBGM");
                if (aKRESULT == AKRESULT.AK_Fail)
                {
                    Debug.Log(string.Format("eventID:{0},aKRESULT{1},GameObject:{2},listener:{3},volume:{4}", eventID,
                        aKRESULT, gameObject.name, listener.name, volume));
                }
            }
            if (type == VolumeType.Sound)
            {
                AKRESULT aKRESULT = AkSoundEngine.SetGameObjectOutputBusVolume(gameObject, listener, volume);
                Debug.Log("SetSound");
                if (aKRESULT == AKRESULT.AK_Fail)
                {
                    Debug.Log(string.Format("eventID:{0},aKRESULT{1},GameObject:{2},listener:{3},volume:{4}", eventID,
                        aKRESULT, gameObject.name, listener.name, volume));
                }
            }
        }

        public void StopAll()
        {
            AkSoundEngine.StopAll(listener);
            AkSoundEngine.StopAll(BGMListener);
            AkSoundEngine.StopAll(akGameobject);
        }

        public void SwtichState(string stateName, string state)
        {
            AkSoundEngine.SetState(stateName, state);
        }
        public void StopSound(string EventName, GameObject gameObject)
        {
            Stop(EventName, gameObject);
        }

        private void Stop(string in_pszEventName, GameObject in_gameObjectID)
        {
            uint id = GetEventIDByName(in_pszEventName);
            StopSoundByID(id, in_gameObjectID);
        }

        private void StopSoundByID(uint soundId, GameObject soundSource = null, int transitionDuration = 0,
            AkCurveInterpolation curveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear)
        {
            var result = AkSoundEngine.ExecuteActionOnEvent(soundId, AkActionOnEventType.AkActionOnEventType_Stop,
                soundSource == null ? gameObject : soundSource, transitionDuration, curveInterpolation);
            if (result != AKRESULT.AK_Success)
            {
                Debug.Log(string.Format("Stop event with id <{0}> failed.", soundId));
                return;
            }
        }


        private void AkCallback(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
        {
            //callback
        }

        /// <summary>
        /// ����RTPC
        /// </summary>
        /// <param name="RTPCName"></param>
        /// <param name="value"></param>
        public void SetRTPC(string RTPCName, float value)
        {
            AkSoundEngine.SetRTPCValue(RTPCName, value);
        }

        private bool CheckEventIsPlaying(GameObject gameObject, uint eventid)
        {
            uint[] uints = new uint[] { };
            var u = (uint) 0;
            AKRESULT aKRESULT = AkSoundEngine.GetPlayingIDsFromGameObject(gameObject, ref u, uints);
            bool exist = uints.Any(x => x == eventid);
            Debug.Log("Exist:" + exist + "; Uid" + u + "; uints" + uints + "; eventid" + eventid + "; aKRESULT:" +
                      aKRESULT);
            return exist;
        }
    }
}