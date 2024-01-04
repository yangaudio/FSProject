using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

public static class WwiseEditor {
    private static string WwisePath = Application.streamingAssetsPath + "/Audio/GeneratedSoundBanks/Windows/SoundbanksInfo.xml";
    private static string CsClassPath = "Assets/Scripts/Framework/Wwise";

    [MenuItem("CustomEditor/Wwise/CreateWwiseEvent")]
    static void ToEventName() {
        CreateWwiseEvent();
    }

    private static void CreateWwiseEvent() {
        ReadXML();
    }

    private static void ReadXML() {
        Dictionary<uint, string> m_UnitInfoDict = new Dictionary<uint, string>();
        Dictionary<string, string> BankEventDic = new Dictionary<string, string>();
        List<string> names = new List<string>();
        if (!string.IsNullOrEmpty(WwisePath)) {
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(WwisePath);
            //首先获取xml中所有的SoundBank
            XmlNodeList soundBankList = XmlDoc.GetElementsByTagName("SoundBank");
            foreach (XmlNode node in soundBankList) {
                XmlNode bankNameNode = node.SelectSingleNode("ShortName");
                string bankName = bankNameNode.InnerText;
                //判断SingleNode存在与否,比如Init.bak就没这个
                // XmlNode eventNode = node.SelectSingleNode("IncludedEvents");
                XmlNode eventNode = node.SelectSingleNode("Events");
                //
                if (eventNode != null) {
                    //拿到其中所有的event做一个映射
                    var obj = eventNode.SelectNodes("Event");
                    XmlNodeList eventList = null;
                    if (obj != null) {
                        eventList = obj;
                    }
                    else {
                        return;
                    }

                    foreach (XmlElement x1e in eventList) {
                        //   m_BankInfoDict.Add(uint.Parse(x1e.Attributes["Id"].Value), bankName);
                        m_UnitInfoDict.Add(uint.Parse(x1e.Attributes["Id"].Value), x1e.Attributes["Name"].Value);
                        BankEventDic.Add(bankName,x1e.Attributes["Name"].Value);
                        names.Add(x1e.Attributes["Name"].Value);
                        WriteWwiseCs(names);
                    }
                }
            }
        }

        else {
            Debug.LogError("路径出错" + WwisePath);
        }
    }

    private static void WriteWwiseCs(List<string> names) {
        WriteCs(names);
    }

    static void WriteCs(List<string> descs) {
        string className = "WwiseEventName";
        try {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("namespace ThGold.Wwise {");
            //stringBuilder.AppendLine("    [Serializable]");
            stringBuilder.AppendLine("    public static class " + className + " {");
            for (int i = 0; i < descs.Count; i++) {
                stringBuilder.AppendLine("        /// <summary>");
                stringBuilder.AppendLine("        /// " + descs[i]);
                stringBuilder.AppendLine("        /// </summary>");
                stringBuilder.AppendLine("        public const string " + descs[i] + " = \"" + descs[i] + "\";");
                stringBuilder.Append("\n");
            }
            stringBuilder.Append("\n");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");
 
            string csPath = CsClassPath + "/" + className + ".cs";
            if (File.Exists(csPath)) {
                File.Delete(csPath);
            }

            using (StreamWriter sw = new StreamWriter(csPath)) {
                sw.Write(stringBuilder);
                Debug.Log("生成:" + csPath);
            }
        }
        catch (System.Exception e) {
            Debug.LogError("写入CS失败:" + e.Message);
            throw;
        }
    }
}