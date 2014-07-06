using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Server
{
    public class XmlEditor
    {
        private string m_path;
        private string m_RootNode;

        private XmlDocument mXmlDocument;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlEditor"/> class.
        /// </summary>
        /// <param name="xmlPath">The path for the file. This can be a local file, or a HTTP URL (A web address).</param>
        /// <param name="rootNode">The root node of the Xml File</param>
        public XmlEditor(string xmlPath, string rootNode) {
            m_path = xmlPath;
            m_RootNode = rootNode;
            CheckXMLFile();
            LoadDocument();
        }

        /// <summary>
        /// Checks if the XML File exists, and if not, creates a new one.
        /// </summary>
        private void CheckXMLFile() {
            if (File.Exists(m_path) != true) {
                StringBuilder fn = new StringBuilder();
                fn.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                fn.AppendLine(string.Format("<{0}>", m_RootNode));
                fn.AppendLine(string.Format("</{0}>", m_RootNode));
                File.WriteAllText(m_path, fn.ToString());
            }
        }

        private void LoadDocument() {
            mXmlDocument = new XmlDocument();
            mXmlDocument.Load(m_path);
        }

        public void Save() {
            mXmlDocument.Save(m_path);
        }

        public void ReloadDocument() {
            LoadDocument();
        }

        public void LoadNewDocument(string xmlPath) {
            m_path = xmlPath;
            CheckXMLFile();
            LoadDocument();
        }

        public void Dispose() {
            mXmlDocument = null;
        }

        #region "Settings"
        /// <summary>
        /// Saves the setting to the default node.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="Value">The value.</param>
        public void SaveSetting(string Key, string Value) {
            SaveSetting(Key, Value, "Settings");
        }

        /// <summary>
        /// Saves the setting to the specified node.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="Value">The value.</param>
        /// <param name="XmlNode">The node.</param>
        public void SaveSetting(string Key, string Value, string XmlNode) {
            CheckXMLFile();
            XmlElement node = (XmlElement)(mXmlDocument.DocumentElement.SelectSingleNode((("/" + m_RootNode + "/") + XmlNode + "/add[@key=\"") + Key + "\"]"));
            if (true) {
                if ((node != null)) {
                    node.Attributes.GetNamedItem("value").Value = Value;
                } else {
                    node = mXmlDocument.CreateElement("add");
                    node.SetAttribute("key", Key);
                    node.SetAttribute("value", Value);
                    XmlNode Root = mXmlDocument.DocumentElement.SelectSingleNode(("/" + m_RootNode + "/") + XmlNode);
                    if ((Root != null)) {
                        Root.AppendChild(node);
                    } else {
                        try {
                            Root = mXmlDocument.DocumentElement.SelectSingleNode("/" + m_RootNode + "");
                            Root.AppendChild(mXmlDocument.CreateElement(XmlNode));
                            Root = mXmlDocument.DocumentElement.SelectSingleNode(("/" + m_RootNode + "/") + XmlNode);
                            Root.AppendChild(node);
                        } catch (Exception ex) {
                            throw new Exception("An error occured while saving the value", ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the setting from the default node.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <returns></returns>
        public string GetSetting(string Key) {
            return GetSetting(Key, "Settings");
        }

        /// <summary>
        /// Gets the setting from the specified node.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="XmlNode">The node.</param>
        /// <returns></returns>
        public string GetSetting(string Key, string XmlNode) {
            CheckXMLFile();
            XmlNode node = mXmlDocument.DocumentElement.SelectSingleNode((("/" + m_RootNode + "/") + XmlNode + "/add[@key=\"") + Key + "\"]");
            if (node != null) {
                return node.Attributes.GetNamedItem("value").Value;
            } else {
                return "";
            }
        }

        /// <summary>
        /// Gets the setting from the default node.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <returns></returns>
        public string TryGetSetting(string Key) {
            try {
                return GetSetting(Key, "Settings");
            } catch {
                return "";
            }
        }

        /// <summary>
        /// Gets the setting from the specified node.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="XmlNode">The node.</param>
        /// <returns></returns>
        public string TryGetSetting(string Key, string XmlNode) {
            try {
                CheckXMLFile();
                XmlNode node = mXmlDocument.DocumentElement.SelectSingleNode((("/" + m_RootNode + "/") + XmlNode + "/add[@key=\"") + Key + "\"]");
                if (node != null) {
                    return node.Attributes.GetNamedItem("value").Value;
                } else {
                    return "";
                }
            } catch {
                return "";
            }
        }

        #endregion

        #region "XML"
        /// <summary>
        /// Gets the XmlNode with the selected key.
        /// </summary>
        /// <param name="Key">The key to look for.</param>
        /// <param name="Node">The node to look in.</param>
        /// <returns></returns>
        public XmlNode GetNode(string Key, string Node) {
            CheckXMLFile();
            XmlNode returnValue = mXmlDocument.DocumentElement.SelectSingleNode((("/" + m_RootNode + "/") + Node + "/add[@key=\"") + Key + "\"]");
            return returnValue;
        }

        /// <summary>
        /// Gets the XmlNode of the selected key. This will look in the "Settings" node.
        /// </summary>
        /// <param name="Key">The key to look for.</param>
        /// <returns></returns>
        public XmlNode GetNode(string Key) {
            return GetNode(Key, "Settings");
        }

        /// <summary>
        /// Gets the value of an attribute of an XML Node.
        /// </summary>
        /// <param name="XmlNode">The XML node.</param>
        /// <param name="Attribute">The attribute.</param>
        /// <returns></returns>
        public string GetAttributeValue(XmlNode XmlNode, string Attribute) {
            if (XmlNode != null) {
                return XmlNode.Attributes.GetNamedItem(Attribute).Value;
            } else {
                return "";
            }
        }

        /// <summary>
        /// Gets the value of an attribute of an XML Node. The XML Node is first found, using the Key.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="Node">The node.</param>
        /// <param name="Attribute">The attribute.</param>
        /// <returns></returns>
        public string GetAttributeValue(string Key, string Node, string Attribute) {
            XmlNode node = GetNode(Key, Node);
            if (node != null) {
                return GetAttributeValue(GetNode(Key, Node), Attribute);
            } else {
                return "";
            }
        }

        /// <summary>
        /// Gets the value of an attribute of an XML Node. The XML Node is first found, using the Key. Looks in the "Settings" node for the key.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="Attribute">The attribute.</param>
        /// <returns></returns>
        public string GetAttributeValue(string Key, string Attribute) {
            return GetAttributeValue(Key, "Settings", Attribute);
        }

        /// <summary>
        /// Gets the value of an attribute of an XML Node.
        /// </summary>
        /// <param name="XmlNode">The XML node.</param>
        /// <param name="Attribute">The attribute.</param>
        /// <returns></returns>
        public string TryGetAttributeValue(XmlNode XmlNode, string Attribute) {
            try {
                if (XmlNode != null) {
                    return XmlNode.Attributes.GetNamedItem(Attribute).Value;
                } else {
                    return "";
                }
            } catch {
                return "";
            }
        }

        /// <summary>
        /// Gets the value of an attribute of an XML Node. The XML Node is first found, using the Key.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="Node">The node.</param>
        /// <param name="Attribute">The attribute.</param>
        /// <returns></returns>
        public string TryGetAttributeValue(string Key, string Node, string Attribute) {
            try {
                XmlNode node = GetNode(Key, Node);
                if (node != null) {
                    return GetAttributeValue(node, Attribute);
                } else {
                    return "";
                }
            } catch {
                return "";
            }
        }

        /// <summary>
        /// Gets the value of an attribute of an XML Node. The XML Node is first found, using the Key. Looks in the "Settings" node for the key.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="Attribute">The attribute.</param>
        /// <returns></returns>
        public string TryGetAttributeValue(string Key, string Attribute) {
            try {
                return GetAttributeValue(Key, "Settings", Attribute);
            } catch {
                return "";
            }
        }

        /// <summary>
        /// Gets all child nodes in the specified parent node.
        /// </summary>
        /// <param name="Node">The parent node.</param>
        /// <returns></returns>
        public XmlNodeList GetAllNodes(string Node) {
            CheckXMLFile();
            XmlNode xmlnode = mXmlDocument.DocumentElement.SelectSingleNode(Node);
            XmlNode Root = mXmlDocument.DocumentElement.SelectSingleNode(Node);
            if (Root == null) {
                try {
                    Root = mXmlDocument.DocumentElement.SelectSingleNode("/" + m_RootNode + "");
                    xmlnode = Root.AppendChild(mXmlDocument.CreateElement(Node));
                } catch (Exception ex) {
                    throw new Exception("An error occured while saving the value", ex);
                }
            }
            if (xmlnode != null) {
                return xmlnode.ChildNodes;
            } else {
                return null;
            }
        }


        /// <summary>
        /// Gets all child nodes in the specified parent node.
        /// </summary>
        /// <param name="Node">The parent node.</param>
        /// <returns></returns>
        public XmlNodeList GetAllNodes(XmlNode Node) {
            CheckXMLFile();
            XmlNode xmlnode = mXmlDocument.DocumentElement.SelectSingleNode(Node.Name);
            XmlNode Root = mXmlDocument.DocumentElement.SelectSingleNode(Node.Name);
            if (Root == null) {
                try {
                    Root = mXmlDocument.DocumentElement.SelectSingleNode("/" + m_RootNode + "");
                    xmlnode = Root.AppendChild(mXmlDocument.CreateElement(Node.Name));
                } catch (Exception ex) {
                    throw new Exception("An error occured while saving the value", ex);
                }
            }
            if (xmlnode != null) {
                return xmlnode.ChildNodes;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Gets all child nodes in the root node.
        /// </summary>
        /// <returns></returns>
        public XmlNodeList GetAllNodes() {
            CheckXMLFile();
            if (mXmlDocument.DocumentElement.ParentNode != null) {
                return mXmlDocument.DocumentElement.SelectSingleNode("/" + m_RootNode).ChildNodes;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Saves a new node with the specified attributes.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="XmlNode">The parent node.</param>
        /// <param name="Attributes">The attributes.</param>
        /// <returns></returns>
        public XmlNode SaveNode(string Key, string XmlNode, XmlAttributes Attributes) {
            CheckXMLFile();
            XmlElement node = (XmlElement)(mXmlDocument.DocumentElement.SelectSingleNode((("/" + m_RootNode + "/") + XmlNode + "/add[@key=\"") + Key + "\"]"));
            if (node != null) {
                foreach (string strKey in Attributes.Keys) {
                    node.SetAttribute(strKey, Attributes[strKey]);
                }
                return node;
            } else {
                node = mXmlDocument.CreateElement("add");
                node.SetAttribute("key", Key);
                foreach (string strKey in Attributes.Keys) {
                    node.SetAttribute(strKey.Replace(" ", ""), Attributes[strKey]);
                }
                XmlNode Root = mXmlDocument.DocumentElement.SelectSingleNode(("/" + m_RootNode + "/") + XmlNode);
                if (Root != null) {
                    Root.AppendChild(node);
                    return node;
                } else {
                    try {
                        Root = mXmlDocument.DocumentElement.SelectSingleNode("/" + m_RootNode + "");
                        Root.AppendChild(mXmlDocument.CreateElement(XmlNode));
                        Root = mXmlDocument.DocumentElement.SelectSingleNode(("/" + m_RootNode + "/") + XmlNode);
                        Root.AppendChild(node);
                        return node;
                    } catch (Exception ex) {
                        throw new Exception("An error occured while saving the value", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the specified node.
        /// </summary>
        /// <param name="XmlNode">The XML node.</param>
        /// <param name="Node">The parent node.</param>
        public void DeleteNode(XmlNode XmlNode, string Node) {
            CheckXMLFile();
            XmlNode xmlnodeToDelete = mXmlDocument.DocumentElement.SelectSingleNode((("/" + m_RootNode + "/") + Node + "/add[@key=\"") + GetAttributeValue(XmlNode, "key") + "\"]");
            xmlnodeToDelete.ParentNode.RemoveChild(xmlnodeToDelete);
        }

        /// <summary>
        /// Deletes the specified node.
        /// </summary>
        /// <param name="Key">The key of the node to delete.</param>
        /// <param name="Node">The parent node.</param>
        public void DeleteNode(string Key, string Node) {
            CheckXMLFile();
            XmlNode xmlnode = mXmlDocument.DocumentElement.SelectSingleNode((("/" + m_RootNode + "/") + Node + "/add[@key=\"") + Key + "\"]");
            xmlnode.ParentNode.RemoveChild(xmlnode);
        }


        public XmlNode RootXmlNode {
            get {
                CheckXMLFile();
                return mXmlDocument.DocumentElement.SelectSingleNode("/" + m_RootNode);
            }
        }

        public void SaveAttribute(string Key, string Node, string Name, string Value) {
            CheckXMLFile();
            XmlElement Xmlnode = (XmlElement)(mXmlDocument.DocumentElement.SelectSingleNode((("/" + m_RootNode + "/") + Node + "/add[@key=\"") + Key + "\"]"));
            Xmlnode.SetAttribute(Name, Value);
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// Gets or sets the root node of the Xml File.
        /// </summary>
        public string RootNode {
            get { return m_RootNode; }
            set { m_RootNode = value; }
        }

        /// <summary>
        /// Gets the file path of the XML File.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath {
            get { return m_path; }
        }
        #endregion

    }
}
