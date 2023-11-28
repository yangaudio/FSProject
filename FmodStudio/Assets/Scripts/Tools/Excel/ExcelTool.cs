using Excel;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace EditorTool
{
#if UNITY_EDITOR
    public class ExcelConfig
    {
        /// <summary>
        /// 存放excel表文件夹的的路径，本例xecel表放在了"Assets/Excels/"当中
        /// </summary>
        public static readonly string excelsFolderPath = Application.dataPath + "/Excels/";
        public static string ExcelDataPath = Application.dataPath + "/../Excel";//源Excel文件夹,xlsx格式
        /// <summary>
        /// 存放Excel转化CS文件的文件夹路径
        /// </summary>
        public static readonly string assetPath = "Assets/Resources/DataAssets/";
    }
    public class ExcelTool
    {

        public class ExcelBuild : Editor
        {

            
            public static void CreateItemAsset()
            {
                ExcelConverter.ConvertExcelToClass(ExcelConfig.ExcelDataPath, "Assets/Scripts/DataTable");
            }
        }

    }
#endif
}
