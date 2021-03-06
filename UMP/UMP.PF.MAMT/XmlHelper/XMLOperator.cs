﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;

namespace XmlHelper
{
    public class XMLOperator
    {
        public XMLOperator()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }
        //声明一个XmlDocument空对象
        protected  XmlDocument XmlDoc = new XmlDocument();

        /// <summary>
        /// 构造函数，导入Xml文件
        /// </summary>
        /// <param name="xmlFile">文件虚拟路径</param>
        public XMLOperator(string xmlFile)
        {
            try
            {
                XmlDoc.Load(xmlFile); //载入Xml文档
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~XMLOperator()
        {
            XmlDoc = null; //释放XmlDocument对象
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath">文件虚拟路径</param>
        public void Save(string filePath)
        {
            try
            {
                XmlDoc.Save(filePath);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据Xml文件的节点路径，返回一个DataSet数据集
        /// </summary>
        /// <param name="XmlPathNode">Xml文件的某个节点</param>
        /// <returns></returns>
        public  DataSet GetDs(string XmlPathNode)
        {
            DataSet ds = new DataSet();
            try
            {
                System.IO.StringReader read = new System.IO.StringReader(XmlDoc.SelectSingleNode(XmlPathNode).OuterXml);
                ds.ReadXml(read); //利用DataSet的ReadXml方法读取StringReader文件流
                read.Close();
            }
            catch
            { }
            return ds;
        }

        /// <summary>
        /// 属性查询，返回属性值
        /// </summary>
        /// <param name="XmlPathNode">属性所在的节点</param>
        /// <param name="Attrib">属性</param>
        /// <returns></returns>
        public string SelectAttrib(XmlNode node, string Attrib)
        {
            string _strNode = "";
            try
            {
                _strNode = node.Attributes[Attrib].Value;
            }
            catch
            { }
            return _strNode;
        }

        /// <summary>
        /// 节点查询，返回节点值
        /// </summary>
        /// <param name="XmlPathNode">节点的路径</param>
        /// <returns></returns>
        public string SelectNodeText(string XmlPathNode)
        {
            string _nodeTxt = XmlDoc.SelectSingleNode(XmlPathNode).InnerText;
            if (_nodeTxt == null || _nodeTxt == "")
                return "";
            else
                return _nodeTxt;
        }

        public XmlNode SelectNode(string XmlPathNode, string strInnerText)
        {
            XmlNode resultNode = null;
            XmlNodeList lstNodes = XmlDoc.SelectNodes(XmlPathNode);
            if (lstNodes.Count > 0)
            {
                foreach (XmlNode node in lstNodes)
                {
                    if (node.InnerText.Equals(strInnerText))
                    {
                        resultNode = node;
                        break;
                    }
                }
            }
            return resultNode;
        }

        /// <summary>
        /// 根据属性查找节点
        /// </summary>
        /// <param name="XmlPathNode"></param>
        /// <param name="strAttrName"></param>
        /// <param name="strAttrValue"></param>
        /// <returns></returns>
        public XmlNode SelectNodeByAttribute(string XmlPathNode, string strAttrName, string strAttrValue)
        {
            XmlNode resultNode = null;
            XmlNodeList lstNodes = XmlDoc.SelectNodes(XmlPathNode);
            if (lstNodes.Count > 0)
            {
                foreach (XmlNode node in lstNodes)
                {
                    if (node.Attributes[strAttrName].Value.Equals(strAttrValue))
                    {
                        resultNode = node;
                        break;
                    }
                }
            }
            return resultNode;
        }

        /// <summary>
        /// 获取子节点个数
        /// </summary>
        /// <param name="XmlPathNode">父节点</param>
        /// <returns></returns>
        public int NodeCount(string XmlPathNode)
        {
            int i = 0;
            try
            {
                i = XmlDoc.SelectSingleNode(XmlPathNode).ChildNodes.Count;
            }
            catch
            {
                i = 0;
            }
            return i;
        }

        /// <summary>
        /// 更新一个节点的内容(从文档第一行开始往下找符合该路径的节点)
        /// </summary>
        /// <param name="XmlPathNode">节点的路径</param>
        /// <param name="Content">新的节点值</param>
        /// <returns></returns>
        public bool UpdateNode(string XmlPathNode, string NodeContent)
        {
            try
            {
                XmlDoc.SelectSingleNode(XmlPathNode).InnerText = NodeContent;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 修改指定节点的内容
        /// </summary>
        /// <param name="node"></param>
        /// <param name="NodeContent"></param>
        /// <returns></returns>
        public bool UpdateNode(XmlNode node, string NodeContent)
        {
            try
            {
                node.InnerText = NodeContent;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 更新N个节点值
        /// </summary>
        /// <param name="XmlParentNode">父节点</param>
        /// <param name="XmlNode">子节点</param>
        /// <param name="NodeContent">子节点内容</param>
        /// <returns></returns>
        public bool UpdateNode(string XmlParentNode, List<string> XmlNode, List<string> NodeContent)
        {
            try
            {
                //根据节点数组循环修改节点值
                for (int i = 0; i < XmlNode.Count; i++)
                {
                    XmlDoc.SelectSingleNode(XmlParentNode + "/" + XmlNode[i]).InnerText = NodeContent[i];
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 修改属性
        /// </summary>
        /// <param name="XmlPathNode">属性所在的节点</param>
        /// <param name="Attrib">属性名</param>
        /// <param name="Content">属性值</param>
        /// <returns></returns>
        public bool UpdateAttrib(string XmlPathNode, string Attrib, string AttribContent)
        {
            try
            {
                //修改属性值
                XmlDoc.SelectSingleNode(XmlPathNode).Attributes[Attrib].Value = AttribContent;
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="MainNode">属性所在节点</param>
        /// <param name="Attrib">属性名</param>
        /// <param name="AttribContent">属性值</param>
        /// <returns></returns>
        public bool InsertAttrib(string MainNode, string Attrib, string AttribContent)
        {
            try
            {
                XmlElement objNode = (XmlElement)XmlDoc.SelectSingleNode(MainNode); //强制转化成XmlElement对象
                objNode.SetAttribute(Attrib, AttribContent); //XmlElement对象添加属性方法   
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 插入一个节点，带N个子节点
        /// </summary>
        /// <param name="MainNode">插入节点的父节点</param>
        /// <param name="ChildNode">插入节点的元素名</param>
        /// <param name="Element">插入节点的子节点名数组</param>
        /// <param name="Content">插入节点的子节点内容数组</param>
        /// <returns></returns>
        public bool InsertNode(string MainNode, string ChildNode, List<string> Element, List<string> Content)
        {
            try
            {
                XmlNode objRootNode = XmlDoc.SelectSingleNode(MainNode); //声明XmlNode对象
                XmlElement objChildNode = XmlDoc.CreateElement(ChildNode); //创建XmlElement对象
                objRootNode.AppendChild(objChildNode);
                for (int i = 0; i < Element.Count; i++) //循环插入节点元素
                {
                    XmlElement objElement = XmlDoc.CreateElement(Element[i]);
                    objElement.InnerText = Content[i];
                    objChildNode.AppendChild(objElement);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除一个节点
        /// </summary>
        /// <param name="Node">要删除的节点</param>
        /// <returns></returns>
        public bool DeleteNode(string Node)
        {
            try
            {
                //XmlNode的RemoveChild方法来删除节点及其所有子节点
                XmlDoc.SelectSingleNode(Node).ParentNode.RemoveChild(XmlDoc.SelectSingleNode(Node));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
