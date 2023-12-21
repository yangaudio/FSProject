using System;
using System.Data;
using System.IO;
using Excel;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEditor;

public static class ExcelConverter {
    static string XmlDataPath = Application.dataPath + "/Resources/XmlData";
    static string xmlfilepath = "/Resources/XmlData/";
    public static string ExcelDataPath = Application.dataPath + "/Resources/Excel"; //源Excel文件夹,xlsx格式
    static string CsClassPath = Application.dataPath + "/Scripts/GamePlay/DataClass"; //生成的c#脚本文件夹
    static string CsDataClassPath = Application.dataPath + "/Scripts/GamePlay/DataTable"; //生成的c#脚本文件夹
    static string AllCsHead = "all"; //序列化结构体的数组类.类名前缀
    static char ArrayTypeSplitChar = ','; //数组类型值拆分符: int[] 1#2#34 string[] 你好#再见 bool[] true#false ...

    [MenuItem("CustomEditor/Excel/Step3-Cs2DataCs]")]
    static void Cs2Data() {
        Init();
        Cs2DataCs();
    }

    [MenuItem("CustomEditor/Excel/Step1-ExcelToXml")]
    static void Excel2Xml2Bytes() {
        Init();
        //生成中间文件xml
        Excel2CsOrXml(false);
        //生成bytes
        //WriteBytes();
    }

    [MenuItem("CustomEditor/Excel/Step2-XMLtoScript]")]
    static void Excel2Cs_Xr() {
        Init();
        Excel2CsOrXmlByXr(true);
    }


    static void Init() {
        if (!Directory.Exists(CsClassPath)) {
            Directory.CreateDirectory(CsClassPath);
        }

        if (!Directory.Exists(XmlDataPath)) {
            Directory.CreateDirectory(XmlDataPath);
        }

        if (!Directory.Exists(CsDataClassPath)) {
            Directory.CreateDirectory(CsDataClassPath);
        }

        Debug.Log("InitDicSuccess");
    }

    public static void ConvertExcelToClass(string excelDirectory, string classDirectory) {
        string[] filePaths = Directory.GetFiles(excelDirectory, "*.xlsx");
        foreach (string filePath in filePaths) {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet dataSet = excelReader.AsDataSet();

            string className = Path.GetFileNameWithoutExtension(filePath);
            using (StreamWriter writer = new StreamWriter(classDirectory + "/" + className + ".cs")) {
                writer.WriteLine("using System;");
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine("");
                writer.WriteLine("[Serializable]");
                writer.WriteLine("public class " + className);
                writer.WriteLine("{");

                foreach (DataTable dataTable in dataSet.Tables) {
                    foreach (DataColumn column in dataTable.Columns) {
                        writer.WriteLine("    [SerializeField]");
                        writer.WriteLine("    public " + GetTypeName(column.DataType) + " " + column.ColumnName + ";");
                    }
                }

                writer.WriteLine("}");
            }
        }
    }

    static void Excel2CsOrXml(bool isCs) {
        if (!isCs) {
            Directory.Delete(XmlDataPath, true);
            Directory.CreateDirectory(XmlDataPath);
        }
        else {
            Directory.Delete(CsClassPath, true);
            Directory.CreateDirectory(CsClassPath);
        }

        string[] excelPaths = Directory.GetFiles(ExcelDataPath, "*.xlsx");
        for (int e = 0; e < excelPaths.Length; e++) {
            //0.读Excel
            string className; //类型名
            string[] names; //字段名
            string[] types; //字段类型
            string[] descs; //字段描述
            List<string[]> datasList; //数据

            try {
                string excelPath = excelPaths[e]; //excel路径  
                className = Path.GetFileNameWithoutExtension(excelPath).ToLower();
                FileStream fileStream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                // 表格数据全部读取到result里
                DataSet result = excelDataReader.AsDataSet();
                // 获取表格列数
                int columns = result.Tables[0].Columns.Count;
                // 获取表格行数
                int rows = result.Tables[0].Rows.Count;
                // 根据行列依次读取表格中的每个数据
                names = new string[columns];
                types = new string[columns];
                descs = new string[columns];
                datasList = new List<string[]>();
                for (int r = 0; r < rows; r++) {
                    string[] curRowData = new string[columns];
                    for (int c = 0; c < columns; c++) {
                        //解析：获取第一个表格中指定行指定列的数据
                        string value = result.Tables[0].Rows[r][c].ToString();
                        if (value.StartsWith("^")) {
                            value = "cehuaUse" + c;
                        }

                        //清除前两行的变量名、变量类型 首尾空格
                        if (r < 2) {
                            value = value.TrimStart(' ').TrimEnd(' ');
                        }

                        curRowData[c] = value;
                    }

                    //解析：第一行类变量名
                    if (r == 0) {
                        names = curRowData;
                    } //解析：第二行类变量类型
                    else if (r == 1) {
                        types = curRowData;
                    } //解析：第三行类变量描述
                    else if (r == 2) {
                        descs = curRowData;
                    } //解析：第三行开始是数据
                    else {
                        datasList.Add(curRowData);
                    }
                }
            }
            catch (System.Exception exc) {
                Debug.LogError("请关闭Excel:" + exc.Message);
                return;
            }

            if (isCs) {
                //写Cs
                WriteCs(className, names, types, descs);
            }
            else {
                //写Xml
                WriteXml(className, names, types, datasList);
            }
        }

        AssetDatabase.Refresh();
    }

    static void Excel2CsOrXmlByXr(bool isCs) {
        if (!isCs) {
            Directory.Delete(XmlDataPath, true);
            Directory.CreateDirectory(XmlDataPath);
        }
        else {
            Directory.Delete(CsClassPath, true);
            Directory.CreateDirectory(CsClassPath);
        }

        string[] excelPaths = Directory.GetFiles(ExcelDataPath, "*.xlsx");
        for (int e = 0; e < excelPaths.Length; e++) {
            //0.读Excel
            string className; //类型名
            string[] names; //字段名
            string[] types; //字段类型
            string[] descs; //字段描述
            List<string[]> datasList; //数据

            try {
                string excelPath = excelPaths[e]; //excel路径  
                className = Path.GetFileNameWithoutExtension(excelPath).ToLower();
                FileStream fileStream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                // 表格数据全部读取到result里
                DataSet result = excelDataReader.AsDataSet();
                // 获取表格列数
                int columns = result.Tables[0].Columns.Count;
                // 获取表格行数
                int rows = result.Tables[0].Rows.Count;
                // 根据行列依次读取表格中的每个数据
                names = new string[columns];
                types = new string[columns];
                descs = new string[columns];
                datasList = new List<string[]>();
                for (int r = 0; r < rows; r++) {
                    string[] curRowData = new string[columns];
                    for (int c = 0; c < columns; c++) {
                        //解析：获取第一个表格中指定行指定列的数据
                        string value = result.Tables[0].Rows[r][c].ToString();
                        if (value.StartsWith("^")) {
                            value = "cehuaUse" + c;
                        }

                        //清除前两行的变量名、变量类型 首尾空格
                        if (r < 2) {
                            value = value.TrimStart(' ').TrimEnd(' ');
                        }

                        curRowData[c] = value;
                    }

                    //解析：第一行类变量名
                    if (r == 0) {
                        names = curRowData;
                    } //解析：第二行类变量类型
                    else if (r == 1) {
                        string[] rawTypes = new string[curRowData.Length];
                        for (int i = 0; i < curRowData.Length; i++) {
                            rawTypes[i] = curRowData[i].ToLower();
                        }

                        // 将小写类型转为对应的 .NET 类型
                        Type[] _types = new Type[curRowData.Length];
                        for (int i = 0; i < rawTypes.Length; i++) {
                            // 判断是否为数组类型
                            bool isArray = rawTypes[i].EndsWith("[]");

                            // 判断是否为列表类型
                            bool isList = rawTypes[i].StartsWith("list<") && rawTypes[i].EndsWith(">");

                            Debug.Log($"i: {i}, isList: {isList}, isArray: {isArray}, rawTypes[i]: {rawTypes[i]}");

                            string typeName = "";
                            /*string typeName = isArray || isList
                                ? rawTypes[i].Substring(isList ? 5 : 0, rawTypes[i].Length - (isArray ? 2 : (isList ? 1 : 0)))
                                : rawTypes[i];*/
                            if (isArray || isList) {
                                if (isArray) {
                                    typeName = rawTypes[i].Substring(0, rawTypes[i].Length - 2);
                                }
                                else {
                                    typeName = rawTypes[i].Substring(5, rawTypes[i].Length - 6);
                                }
                            }
                            else {
                                typeName = rawTypes[i];
                            }

                            Debug.Log($"rawTypes[i]: {rawTypes[i]}, isList: {isList}, typeName: {typeName}");
                            Debug.Log($"i: {i}");
                            switch (typeName) {
                                case "int":
                                    _types[i] = isArray ? typeof(int[]) : (isList ? GetListType<int>() : typeof(int));
                                    break;
                                case "string":
                                    _types[i] = isArray ? typeof(string[]) : (isList ? GetListType<string>() : typeof(string));
                                    break;
                                case "float":
                                    _types[i] = isArray ? typeof(float[]) : (isList ? GetListType<float>() : typeof(float));
                                    break;
                                case "bool":
                                    _types[i] = isArray ? typeof(bool[]) : (isList ? GetListType<bool>() : typeof(bool));
                                    break;
                                // 可以根据需要添加其他类型的处理
                                default:
                                    Debug.LogWarning($"未知的类型：{rawTypes[i]}，默认为 object 类型。");
                                    _types[i] = isArray ? typeof(object[]) : (isList ? GetListType<object>() : typeof(object));
                                    break;
                            }
                        }

                        string[] typeNames = _types.Select(GetFriendlyTypeName).ToArray();
                        types = typeNames;
                    } //解析：第三行类变量描述
                    else if (r == 2) {
                        descs = curRowData;
                    } //解析：第三行开始是数据
                    else {
                        datasList.Add(curRowData);
                    }
                }
            }
            catch (System.Exception exc) {
                Debug.LogError("请关闭Excel:" + exc.Message);
                Debug.LogError("堆栈信息：" + exc.StackTrace);
                return;
            }

            if (isCs) {
                //写Cs
                WriteCsByXr(className, names, types, descs);
            }
            else {
                //写Xml
                WriteXml(className, names, types, datasList);
            }
        }

        AssetDatabase.Refresh();
    }

    static Type GetListType<T>() {
        return typeof(List<T>);
    }

    private static string GetFriendlyTypeName(Type type) {
        // 判断是否为 float 类型
        if (type == typeof(float) || type == typeof(Single)) {
            return "float";
        }

        if (type == typeof(Int32)) {
            return "int";
        }

        if (!type.IsGenericType) {
            return type.Name;
        }
        string typeName = type.Name;
        int backtickIndex = typeName.IndexOf('`');
        if (backtickIndex > 0) {
            typeName = typeName.Substring(0, backtickIndex);
        }

        Type[] genericArguments = type.GetGenericArguments();
        string genericArgumentsString = string.Join(", ", genericArguments.Select(GetFriendlyTypeName));

        return $"{typeName}<{genericArgumentsString}>";
    }

    static void Cs2DataCs() {
        if (!Directory.Exists(CsDataClassPath)) {
            Directory.CreateDirectory(CsDataClassPath);
        }

        string[] CsPaths = Directory.GetFiles(CsClassPath, "*.cs");
        if (CsPaths.Length <= 0) {
            Debug.LogError(CsClassPath + "路径中没有Cs文件");
        }

        for (int e = 0; e < CsPaths.Length; e++) {
            //0.读Excel
            string className; //类型名
            try {
                string excelPath = CsPaths[e]; //cs路径  
                className = Path.GetFileNameWithoutExtension(excelPath).ToLower();
            }
            catch (System.Exception exc) {
                Debug.LogError("请关闭Excel:" + exc.Message);
                return;
            }

            WriteCsByXrOperation(className);
        }

        AssetDatabase.Refresh();
    }

    static string MapTypeToName(Type type) {
        // 判断是否为 float 类型
        if (type == typeof(float) || type == typeof(Single)) {
            return "float";
        }
        // 添加其他类型的映射
        // else if (type == typeof(int) || type == typeof(Int32))
        // {
        //     return "int";
        // }
        // 其他类型的映射...

        // 如果没有匹配到特定类型，返回类型的默认名称
        return type.Name;
    }

    static void WriteCsByXr(string className, string[] names, string[] types, string[] descs) {
        try {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;");
            stringBuilder.AppendLine("using System.Collections.Generic;");
            stringBuilder.AppendLine("using System.IO;");
            stringBuilder.AppendLine("using System.Runtime.Serialization.Formatters.Binary;");
            stringBuilder.AppendLine("using System.Xml.Serialization;");
            stringBuilder.AppendLine("using System.Xml;");
            stringBuilder.AppendLine("using UnityEngine;");
            stringBuilder.Append("\n");
            stringBuilder.AppendLine("namespace ThGold.Table");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    [Serializable]");
            stringBuilder.AppendLine("    public class " + className + " : DefaultDataBase");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        private static DefaultDataBase _inst;");
            stringBuilder.AppendLine("        public static DefaultDataBase Instance");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            get");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                if (_inst == null)");
            stringBuilder.AppendLine("                {");
            stringBuilder.AppendLine("                    _inst = new " + className + "();");
            stringBuilder.AppendLine("                }");
            stringBuilder.AppendLine("                return _inst;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("        }");
            for (int i = 0; i < names.Length; i++) {
                stringBuilder.AppendLine("        /// <summary>");
                stringBuilder.AppendLine("        /// " + descs[i]);
                stringBuilder.AppendLine("        /// </summary>");
                stringBuilder.AppendLine("        [XmlAttribute(\"" + names[i] + "\")]");

                string type = types[i];
                if (type.Contains("[]")) {
                    //type = type.Replace("[]", "");
                    //stringBuilder.AppendLine("        public List<" + type + "> " + names[i] + ";");

                    //可选代码：
                    //用_name字段去反序列化，name取_name.item的值,直接返回list<type>。
                    //因为xml每行可能有多个数组字段，这样就多了一层变量item，所以访问的时候需要.item才能取到list<type>
                    //因此用额外的一个变量直接返回List<type>。
                    type = type.Replace("[]", "");
                    stringBuilder.AppendLine("        public List<" + type + "> " + names[i] + "");
                    stringBuilder.AppendLine("        {");
                    stringBuilder.AppendLine("            get");
                    stringBuilder.AppendLine("            {");
                    stringBuilder.AppendLine("                if (_" + names[i] + " != null)");
                    stringBuilder.AppendLine("                {");
                    stringBuilder.AppendLine("                    return _" + names[i] + ".item;");
                    stringBuilder.AppendLine("                }");
                    stringBuilder.AppendLine("                return null;");
                    stringBuilder.AppendLine("            }");
                    stringBuilder.AppendLine("        }");
                    stringBuilder.AppendLine("        [XmlElementAttribute(\"" + names[i] + "\")]");
                    stringBuilder.AppendLine("        public " + type + "Array _" + names[i] + ";");
                }
                else {
                    if (names[i] == "ID")
                        stringBuilder.AppendLine("        public override " + type + " " + names[i] + "{ get; set; }");
                    else
                        stringBuilder.AppendLine("        public " + type + " " + names[i] + ";");
                }

                stringBuilder.Append("\n");
            }

            stringBuilder.AppendLine("        protected override void LoadBytesInfo()");
            stringBuilder.AppendLine("        {");
            // stringBuilder.AppendLine("            Utils.LoadXMLByBundleByThreadAsync(\"" + className + ".xml\", loadData, CustPackageName.DataTable, CustPackageName.PlatformLobby);");
            
            stringBuilder.AppendLine("           string xmlPath = Application.dataPath + \"" + xmlfilepath + className + ".xml\";");
            stringBuilder.AppendLine("            XmlDocument xmlDoc = new XmlDocument();");
            stringBuilder.AppendLine("           xmlDoc.Load(xmlPath);");
            stringBuilder.AppendLine("             string xmlText = xmlDoc.InnerXml;");
            stringBuilder.AppendLine("            XmlReaderSettings settings = new XmlReaderSettings();");
            stringBuilder.AppendLine("           settings.IgnoreComments = true;");
            stringBuilder.AppendLine("           settings.IgnoreWhitespace = true;");
            stringBuilder.AppendLine("           settings.Async = true;");
            stringBuilder.AppendLine("           loadData(XmlReader.Create(new StringReader(xmlText), settings));");
            //
            stringBuilder.AppendLine("        }");
            stringBuilder.Append("\n");
            stringBuilder.AppendLine("        protected async override void loadDataInfo(XmlReader reader)");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            if (reader == null)");
            stringBuilder.AppendLine("                return;");
            stringBuilder.AppendLine("            while (await reader.ReadAsync())");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("            try");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                 if (reader.NodeType != XmlNodeType.Element || !reader.HasAttributes)");
            stringBuilder.AppendLine("                     continue;");
            stringBuilder.AppendLine("                 " + className + " data = new " + className + "();");
            for (int i = 0; i < names.Length; i++) {
                string type = types[i];
                if (type == "int") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= int.Parse(reader.GetAttribute(\"" + names[i] + "\"));");
                }
                else if (type == "long") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= long.Parse(reader.GetAttribute(\"" + names[i] + "\"));");
                }
                else if (type == "float") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= float.Parse(reader.GetAttribute(\"" + names[i] + "\"));");
                }
                else if (type == "string") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= reader.GetAttribute(\"" + names[i] + "\").ToString();");

                    stringBuilder.AppendLine("                 if(data." + names[i] + " == \"0\")");
                    stringBuilder.AppendLine("                     data." + names[i] + " = string.Empty;");
                }
                else if (type == "bool") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= bool.Parse(reader.GetAttribute(\"" + names[i] + "\"));");
                }
                else if (type.EndsWith("[]")) {
                    // 处理数组类型
                    string elementType = type.Substring(0, type.Length - 2); // 移除 "[]" 获取元素类型
                    stringBuilder.AppendLine($"                 data.{names[i]}= reader.GetAttribute(\"{names[i]}\").Split(',').Select(x => {ParseValue(elementType, "x")}).ToArray();");
                }
                else if (type.StartsWith("list<") && type.EndsWith(">")) {
                    // 处理列表类型
                    string elementType = type.Substring(5, type.Length - 6); // 移除 "list<" 和 ">"
                    stringBuilder.AppendLine($"                 data.{names[i]}= reader.GetAttribute(\"{names[i]}\").Split(',').Select(x => {ParseValue(elementType, "x")}).ToList();");
                }
            }

            stringBuilder.AppendLine("                 lock (datas)");
            stringBuilder.AppendLine("                 {");
            stringBuilder.AppendLine("                     datas.Add(data);");
            stringBuilder.AppendLine("                 };");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            catch (Exception e)");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                FailDispatchEvent();");
            stringBuilder.AppendLine("                return;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            IsLoadSucceed = true;");
            stringBuilder.AppendLine("            SucceedDispatchEvent(this.GetType().Name);");
            stringBuilder.AppendLine("        }");
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
    private static string ParseValue(string elementType, string value)
    {
        switch (elementType.ToLower())
        {
            case "int":
                return $"int.Parse({value})";
            case "long":
                return $"long.Parse({value})";
            case "float":
                return $"float.Parse({value})";
            case "string":
                return $"{value}";
            case "bool":
                return $"bool.Parse({value})";
            // 添加其他类型的处理
            default:
                return $"({elementType})({value})"; // 默认情况，尝试直接转换
        }
    }
    static void WriteCsByXrOperation(string className) {
        try {
            string dataname = className + "data";
            string datanames = className + "datas";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;");
            stringBuilder.AppendLine("using System.Collections.Generic;");
            stringBuilder.AppendLine("using System.IO;");
            stringBuilder.AppendLine("using System.Runtime.Serialization.Formatters.Binary;");
            stringBuilder.AppendLine("using System.Xml.Serialization;");
            stringBuilder.AppendLine("using System.Xml;");
            stringBuilder.AppendLine("using UnityEngine;");
            stringBuilder.AppendLine("using ThGold.Event;");
            stringBuilder.AppendLine("using EventHandler = ThGold.Event.EventHandler;");
            stringBuilder.Append("\n");
            stringBuilder.AppendLine("namespace ThGold.Table");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    [Serializable]");
            stringBuilder.AppendLine("public class " + dataname + " : LoadDataBase");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    public Dictionary<int, " + className + "> Datas;");
            stringBuilder.AppendLine("    public static " + dataname + " Instance;");
            stringBuilder.AppendLine("    public override void Init()");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        Instance = this;");
            stringBuilder.AppendLine("        Datas = new Dictionary<int, " + className + ">();");
            stringBuilder.AppendLine("        EventHandler.Instance.EventDispatcher.AddEventListener(\"" + className +
                                     "\"+CustomEvent.DataTableLoadSucceed,LoadDataComplete,EventDispatcherAddMode.SINGLE_SHOT);");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("    public override void InitLoadDataConfig()");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        ConfigList.Add(" + className + ".Instance);");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("    public override void LoadDataComplete(IEvent e)");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        var iter = ConfigList.GetEnumerator();");
            stringBuilder.AppendLine("        List<DefaultDataBase> ls;");
            stringBuilder.AppendLine("        DefaultDataBase ddb;");
            stringBuilder.AppendLine("        while (iter.MoveNext())");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            ddb = iter.Current;");
            stringBuilder.AppendLine("            ls = iter.Current.getdatas();");
            stringBuilder.AppendLine("            if (ls == null)");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                continue;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            if (ddb is " + className + ")");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                for (int i = 0; i < ls.Count; i++)");
            stringBuilder.AppendLine("                {");
            stringBuilder.AppendLine("                    Datas.Add(ls[i].ID, (" + className + ")ls[i]);");
            //测试Debug
            stringBuilder.AppendLine("                    Debug.Log(Datas[ls[i].ID].ID);");
            stringBuilder.AppendLine("                }");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("        ls = null;");
            stringBuilder.AppendLine("        ddb = null;");
            stringBuilder.AppendLine("        iter.Dispose();");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("    public override void Reset()");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("   }");
            stringBuilder.AppendLine("  }");
            string csPath = CsDataClassPath + "/" + datanames + ".cs";
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

    private static string GetTypeName(Type type) {
        if (type == typeof(int)) {
            return "int";
        }
        else if (type == typeof(float)) {
            return "float";
        }
        else if (type == typeof(string)) {
            return "string";
        }
        else {
            return "UnknownType";
        }
    }

    static void WriteCs(string className, string[] names, string[] types, string[] descs) {
        try {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;");
            stringBuilder.AppendLine("using System.Collections.Generic;");
            stringBuilder.AppendLine("using System.IO;");
            stringBuilder.AppendLine("using System.Runtime.Serialization.Formatters.Binary;");
            stringBuilder.AppendLine("using System.Xml.Serialization;");
            stringBuilder.AppendLine("using System.Xml;");
            stringBuilder.AppendLine("using UnityEngine;");
            stringBuilder.Append("\n");
            stringBuilder.AppendLine("namespace ThGold.Table");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    [Serializable]");
            stringBuilder.AppendLine("    public class " + className + " : DefaultDataBase");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        private static DefaultDataBase _inst;");
            stringBuilder.AppendLine("        public static DefaultDataBase Instance");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            get");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                if (_inst == null)");
            stringBuilder.AppendLine("                {");
            stringBuilder.AppendLine("                    _inst = new " + className + "();");
            stringBuilder.AppendLine("                }");
            stringBuilder.AppendLine("                return _inst;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("        }");
            for (int i = 0; i < names.Length; i++) {
                stringBuilder.AppendLine("        /// <summary>");
                stringBuilder.AppendLine("        /// " + descs[i]);
                stringBuilder.AppendLine("        /// </summary>");
                stringBuilder.AppendLine("        [XmlAttribute(\"" + names[i] + "\")]");

                string type = types[i];
                if (type.Contains("[]")) {
                    //type = type.Replace("[]", "");
                    //stringBuilder.AppendLine("        public List<" + type + "> " + names[i] + ";");

                    //可选代码：
                    //用_name字段去反序列化，name取_name.item的值,直接返回list<type>。
                    //因为xml每行可能有多个数组字段，这样就多了一层变量item，所以访问的时候需要.item才能取到list<type>
                    //因此用额外的一个变量直接返回List<type>。
                    type = type.Replace("[]", "");
                    stringBuilder.AppendLine("        public List<" + type + "> " + names[i] + "");
                    stringBuilder.AppendLine("        {");
                    stringBuilder.AppendLine("            get");
                    stringBuilder.AppendLine("            {");
                    stringBuilder.AppendLine("                if (_" + names[i] + " != null)");
                    stringBuilder.AppendLine("                {");
                    stringBuilder.AppendLine("                    return _" + names[i] + ".item;");
                    stringBuilder.AppendLine("                }");
                    stringBuilder.AppendLine("                return null;");
                    stringBuilder.AppendLine("            }");
                    stringBuilder.AppendLine("        }");
                    stringBuilder.AppendLine("        [XmlElementAttribute(\"" + names[i] + "\")]");
                    stringBuilder.AppendLine("        public " + type + "Array _" + names[i] + ";");
                }
                else {
                    if (names[i] == "ID")
                        stringBuilder.AppendLine("        public override " + type + " " + names[i] + "{ get; set; }");
                    else
                        stringBuilder.AppendLine("        public " + type + " " + names[i] + ";");
                }

                stringBuilder.Append("\n");
            }

            stringBuilder.AppendLine("        protected override void LoadBytesInfo()");
            stringBuilder.AppendLine("        {");
            //stringBuilder.AppendLine("            Utils.LoadXMLByBundle(\"" + className + ".xml\", loadData, CustPackageName.DataTable, CustPackageName.PlatformLobby);");
            //stringBuilder.AppendLine("            Utils.LoadXMLByBundleThread(\"" + className + ".xml\", loadData, CustPackageName.DataTable, CustPackageName.PlatformLobby);");

            //临时 没有绑定ab包的情况下
            stringBuilder.AppendLine("           string xmlPath = Application.dataPath + \"" + xmlfilepath + className + ".xml\";");
            stringBuilder.AppendLine("            XmlDocument xmlDoc = new XmlDocument();");
            stringBuilder.AppendLine("           xmlDoc.Load(xmlPath);");
            stringBuilder.AppendLine("             string xmlText = xmlDoc.InnerXml;");
            stringBuilder.AppendLine("            XmlReaderSettings settings = new XmlReaderSettings();");
            stringBuilder.AppendLine("           settings.IgnoreComments = true;");
            stringBuilder.AppendLine("           settings.IgnoreWhitespace = true;");
            stringBuilder.AppendLine("           settings.Async = true;");
            stringBuilder.AppendLine("           loadData(XmlReader.Create(new StringReader(xmlText), settings));");
            //
            stringBuilder.AppendLine("        }");
            stringBuilder.Append("\n");
            stringBuilder.AppendLine("        protected override void loadDataInfo(XmlDocument doc)");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            XmlNode xn = doc.SelectSingleNode(\"" + AllCsHead + className + "\");");
            stringBuilder.AppendLine("            XmlNodeList xnl = xn.ChildNodes;");
            stringBuilder.AppendLine("            for (int i = 0; i < xnl.Count; i++)");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                 " + className + " data = new " + className + "();");
            stringBuilder.AppendLine("                 XmlElement xe = (XmlElement)xnl[i];");
            for (int i = 0; i < names.Length; i++) {
                string type = types[i];
                if (type == "int") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= int.Parse(xe.GetAttribute(\"" + names[i] + "\"));");
                }
                else if (type == "float") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= float.Parse(xe.GetAttribute(\"" + names[i] + "\"));");
                }
                else if (type == "string") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= xe.GetAttribute(\"" + names[i] + "\").ToString();");

                    stringBuilder.AppendLine("                 if(data." + names[i] + " == \"0\")");
                    stringBuilder.AppendLine("                     data." + names[i] + " = string.Empty;");
                }
                else if (type == "bool") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= bool.Parse(xe.GetAttribute(\"" + names[i] + "\"));");
                }
            }

            stringBuilder.AppendLine("                 lock (datas)");
            stringBuilder.AppendLine("                 {");
            stringBuilder.AppendLine("                     datas.Add(data);");
            stringBuilder.AppendLine("                 };");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("        }");
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

    static void WriteXml(string className, string[] names, string[] types, List<string[]> datasList) {
        try {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            stringBuilder.AppendLine("<" + AllCsHead + className + ">");
            //stringBuilder.AppendLine("<" + className + "s>");
            for (int d = 0; d < datasList.Count; d++) {
                stringBuilder.Append("\t<" + className + " ");
                //单行数据
                string[] datas = datasList[d];
                //填充属性节点
                for (int c = 0; c < datas.Length; c++) {
                    string type = types[c];
                    if (!type.Contains("[]")) {
                        string name = names[c];
                        string value = datas[c];
                        value = ValueReplaceLinefeed(value);
                        stringBuilder.Append(name + "=\"" + value + "\"" + (c == datas.Length - 1 ? "" : " "));
                    }
                }

                stringBuilder.Append(">\n");
                //填充子元素节点(数组类型字段)
                for (int c = 0; c < datas.Length; c++) {
                    string type = types[c];
                    if (type.Contains("[]")) {
                        string name = names[c];
                        string value = datas[c];
                        string[] values = value.Split(ArrayTypeSplitChar);
                        stringBuilder.AppendLine("\t\t<" + name + ">");
                        for (int v = 0; v < values.Length; v++) {
                            stringBuilder.AppendLine("\t\t\t<item>" + ValueReplaceLinefeed(values[v]) + "</item>");
                        }

                        stringBuilder.AppendLine("\t\t</" + name + ">");
                    }
                }

                stringBuilder.AppendLine("\t</" + className + ">");
            }

            //stringBuilder.AppendLine("</" + className + "s>");
            stringBuilder.AppendLine("</" + AllCsHead + className + ">");

            string xmlPath = XmlDataPath + "/" + className + ".xml";
            if (File.Exists(xmlPath)) {
                File.Delete(xmlPath);
            }

            using (StreamWriter sw = new StreamWriter(xmlPath)) {
                sw.Write(stringBuilder);
                Debug.Log("生成文件:" + xmlPath);
            }
        }
        catch (System.Exception e) {
            Debug.LogError("写入Xml失败:" + e.Message);
        }
    }

    static string ValueReplaceLinefeed(string value) {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        return value.Replace("\\n", "&#x000A;");
    }
}