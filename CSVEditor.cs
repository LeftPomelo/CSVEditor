using System;
using Excel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
public class CSVEditor : EditorWindow
{
    [MenuItem("Tools/生成c#解析脚本")]
    static void CreateCSharpCode()
    {
        CSVEditor Csv = EditorWindow.CreateWindow<CSVEditor>();
        Csv.Show();
        Csv.Init();
    }

    public void Init()
    {

    }

    public string PathFile;
    string biaoti = @"/*------------------------------------------------------------------------------
该文件是通过自动生成的，不能改
作者：
日期：
----------------------------------------------------------------------------------------*/";

    void ReaderConfigFile(string path,string JsonFileName)
    {
        string[] fileStr = File.ReadAllLines(path);
        PathFile = path;
        CreateCS(fileStr, JsonFileName);
    }

    void CreateCS(string[] reflectFileName,string JsonFileName)
    {
        string CSPath = $"{Application.dataPath + "/Script"}/{JsonFileName}.cs";
        StreamWriter streamwriter = new StreamWriter(CSPath);
        string tabKey = "\t";
        string[] argumentType = reflectFileName[1].Split(',');
        string[] argumentName = reflectFileName[0].Split(',');

        string time = DateTime.Now.ToString();
        streamwriter.WriteLine(biaoti.Replace("#1", time));
        streamwriter.WriteLine(analysis());
        streamwriter.WriteLine($"public class {JsonFileName}");
        streamwriter.WriteLine("{");
        for (int i = 0; i < argumentType.Length; i++)
        {
            streamwriter.WriteLine($"{tabKey}public {argumentType[i]} {argumentName[i]};");
        }

        streamwriter.WriteLine("}");
        streamwriter.WriteLine($"public class JsonToCsv{JsonFileName}");
        streamwriter.WriteLine("{");

        streamwriter.WriteLine($"{tabKey}public List<{JsonFileName}> {JsonFileName}_list = new List<{JsonFileName}>();");
        streamwriter.WriteLine($"{tabKey}public void JsonToCsvOpen()");
        streamwriter.WriteLine($"{tabKey}" + "{");
        streamwriter.WriteLine($"{tabKey}{tabKey}string json = \"{PathFile}\";");
        streamwriter.WriteLine($"{tabKey}{tabKey}string[] fileStr = File.ReadAllLines(json);");
        streamwriter.WriteLine($"{tabKey}{tabKey}for (int i = 2; i < fileStr.Length; i++)" + "{");
        streamwriter.WriteLine($"{tabKey}{tabKey}{tabKey}string[] list_open = fileStr[i].Split(',');");
        streamwriter.WriteLine($"{tabKey}{tabKey}{tabKey}{JsonFileName} jsons = new {JsonFileName}();");
        for (int i = 0; i < argumentType.Length; i++)
        {
            if (argumentType[i] == "int")
            {
                streamwriter.WriteLine($"{tabKey}{tabKey}{tabKey}jsons.{argumentName[i]} = int.Parse(list_open[{i}]);");
            }
            else if (argumentType[i] == "string")
            {
                streamwriter.WriteLine($"{tabKey}{tabKey}{tabKey}jsons.{argumentName[i]} = list_open[{i}];");
            }
            else if (argumentType[i] == "float")
            {
                streamwriter.WriteLine($"{tabKey}{tabKey}{tabKey}jsons.{argumentName[i]} = float.Parse(list_open[{i}]);");
            }
        }
        streamwriter.WriteLine($"{tabKey}{tabKey}{tabKey}{JsonFileName}_list.Add(jsons);" + "}");

        streamwriter.WriteLine($"{tabKey}" + "}");
        streamwriter.WriteLine($"{tabKey}public List<{JsonFileName}> data()" + "{");
        streamwriter.WriteLine($"{tabKey}{tabKey}JsonToCsvOpen();");
        streamwriter.WriteLine($"{tabKey}return {JsonFileName}_list;" + "}");

        streamwriter.WriteLine("}");

        streamwriter.Flush();
        streamwriter.Close();
        AssetDatabase.Refresh();
        Process.Start(CSPath);
    }
    string analysis()
    {
        string str = null;
        str += $"using UnityEngine;\r\n";
        str += $"using UnityEngine.UI;\r\n";
        str += $"using System;\r\n";
        str += $"using System.Collections;\r\n";
        str += $"using UnityEditor;\r\n";
        str += $"using System.IO;\r\n";
        str += $"using System.Collections.Generic;\r\n";
        return str;
    }



    TextAsset text;
    private void OnGUI()
    {
        GUILayout.Label("使用手册：");
        GUILayout.Label("1.要有个UTF-8的CSV文件，并且第一行放类型，下面的放数据");
        GUILayout.Label("请选择一个CSV文件");
        TextAsset newT = EditorGUILayout.ObjectField(text,typeof(TextAsset),false) as TextAsset;
        if (newT!= text)
        {
            text = newT;
        }
        if (GUILayout.Button("生成"))
        {
            if (text!=null)
            {
                string path = AssetDatabase.GetAssetPath(text);
                if (path.EndsWith(".csv"))
                {
                    ReaderConfigFile(path, text.name + "AutoCreate");
                    AssetDatabase.Refresh();
                }
            }

        }
    }
}
