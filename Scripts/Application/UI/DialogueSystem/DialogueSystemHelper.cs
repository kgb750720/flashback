using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public static class DialogueSystemHelper
{
        #region XML
    /// <summary>
    /// 读取xml文件中的对话内容
    /// </summary>
    /// <param name="fileName">xml文件名，不需要加后缀</param>
    /// <param name="nodeName">对话节点名</param>
    /// <returns></returns>
    public static XmlNodeList GetDialogFromXml(string fileName, string nodeName)
    {
        XmlDocument xmlDoc = new XmlDocument();
        TextAsset tt = (TextAsset)Resources.Load(Consts.XmlDir + fileName);
        xmlDoc.LoadXml(tt.text);
        Resources.UnloadAsset(tt);
        XmlNodeList xmlNode = xmlDoc.GetElementsByTagName(nodeName);

        XmlNodeList xmlNlist = xmlNode[0].SelectNodes("content");

        return xmlNlist;
    }
    /// <summary>
    /// 向xml文件中添加对话，若该对话已存在默认更新对话内容
    /// </summary>
    /// <param name="fileName">xml文件名，不需要加后缀</param>
    /// <param name="dialogName">该对话的名称</param>
    /// <param name="dialogContent">对话内容</param>
    /// <param name="isUpdata">若该节点已存在，是否直接更新该节点内容，默认为true</param>
    public static void AddDialogToXml(string fileName, string dialogName, string[] dialogContent, bool isUpdata = true)
    {
        string path = Application.dataPath + "/Resources/" + Consts.XmlDir + fileName + ".xml";

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlElement root = xmlDoc.DocumentElement;//获取根节点
        XmlNodeList nodeList = root.GetElementsByTagName(dialogName);
        if (nodeList.Count > 0)
        {
            if (isUpdata)
            {
                Debug.Log("更新");
                UpdateNode(fileName, dialogName, dialogContent);
            }
            else
            {
                Debug.Log("位置：Utilities.cs，原因：该节点已存在");
                return;
            }

        }
        else
        {
            XmlElement subRoot = xmlDoc.CreateElement(dialogName);
            for (int i = 0; i < dialogContent.Length; i++)
            {
                XmlElement t = xmlDoc.CreateElement("a");
                t.InnerText = dialogContent[i];
                subRoot.AppendChild(t);
            }
            root.AppendChild(subRoot);
            xmlDoc.Save(path);
        }
        
    }
    /// <summary>
    /// 更新节点内容
    /// </summary>
    /// <param name="fileName">xml文件名</param>
    /// <param name="dialogName">对话节点名</param>
    /// <param name="dialogContent">更新内容</param>
    public static void UpdateNode(string fileName, string dialogName, string[] dialogContent)
    {
        string path = Application.dataPath + "/Resources/" + Consts.XmlDir + fileName + ".xml";
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlElement root = xmlDoc.DocumentElement;//获取根节点
        XmlNode subNode = root.SelectSingleNode(dialogName);
        if (subNode == null)
        {
            Debug.Log("位置：Utilities.cs，原因：该节点不存在，请先添加该节点");
            return;
        }
        root.RemoveChild(subNode);
        Debug.Log("删除节点" + subNode.Name);
        xmlDoc.Save(path);
        AddDialogToXml(fileName, dialogName, dialogContent);
    }
   
    public static string[] GetStrsFromXml(string fileName,string strName)
    {
        XmlDocument xmlDoc = new XmlDocument();
        // xmlDoc.Load(Application.dataPath + "/Resources/" + fileName + ".xml");
        TextAsset tt = (TextAsset)Resources.Load(Consts.XmlDir + fileName);
        xmlDoc.LoadXml(tt.text);
        Resources.UnloadAsset(tt);
        XmlNodeList xmlNode = xmlDoc.GetElementsByTagName(strName);
        XmlNodeList xmlNlist = xmlNode[0].SelectNodes("a");
        string[] temp = new string[xmlNlist.Count];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = xmlNlist[i].InnerText.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Trim();
            //string l_strResult = str.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");

            //temp[i] = xmlNlist[i].InnerText.Replace("\n","").Trim();
        }
        return temp;
    }
    #endregion
}
