
using LoggerHelper;
using ScreenManagerNS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MosaicServerToStore
{
    public class MosaicServerToXml_Helper
    {
        public static string xmlPathMosaicScreenInfo = @"xmlMosaicScreenInfo.xml";
        public static string xmlNodeMosaicScreenInfo = @"HS";

        public static string xmlPathTxInfo = @"xmlHS.xml";
        public static string xmlNodeTxInfo = @"HS";


        static XmlDocument _xmlDocumentMosaicScreenInfo = null;
        public static XmlDocument XmlDocumentMosaicScreenInfo
        {
            get
            {
                if (_xmlDocumentMosaicScreenInfo == null)
                {
                    LoadServerMosaicScreenInfo();
                }
                return _xmlDocumentMosaicScreenInfo;
            }
        }
        private static void LoadServerMosaicScreenInfo()
        {
            try
            {                
                if (_xmlDocumentMosaicScreenInfo == null)
                {
                    _xmlDocumentMosaicScreenInfo = new XmlDocument();
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.IgnoreComments = true;
                    XmlReader reader = XmlReader.Create(xmlPathMosaicScreenInfo, settings);
                    _xmlDocumentMosaicScreenInfo.Load(reader);
                    reader.Close();
                    //_xmlDocument.LoadXml(@"..\xmlHS.xml");//@"..\..\Book.xml"
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("加载xmlTxRxGroup文件出错" + ex.Message);  //若出错则异常崩溃，没有提示！
                Logger.Error("加载xmlMosaicScreenInfo文件出错：" + ex.Message);
            }
        }
        public static List<HsScreenInfo> GetXmlMosaicScreenInfo(string path,Dictionary<string, HsScreenInfo> dicName = null, Dictionary<int, HsScreenInfo> dicId = null)
        {
            List<HsScreenInfo> list = new List<HsScreenInfo>();

            if (XmlDocumentMosaicScreenInfo == null)
            {
                return list;
            }
            try
            {
                XmlNodeList nodeList = XmlDocumentMosaicScreenInfo.SelectNodes(path);  //如 HS/Equipments/Equipment  或HS/Rooms/RoomStatus
                                                                                 //XmlNodeList nodeList = xmldoc.SelectNodes(path);  //如 HS/Equipments/Equipment  或HS/Rooms/RoomStatus
                foreach (XmlNode item in nodeList)
                {
                    HsScreenInfo screenInfo = new HsScreenInfo();
                    XmlNode currentNode = item;
                    XmlAttributeCollection xmlAttr = item.Attributes;
                    foreach (XmlAttribute attr in xmlAttr)
                    {
                        if (attr.Name == nameof(screenInfo.Name))
                        {
                            screenInfo.Name = attr.Value;
                        }
                        else if (attr.Name == nameof(screenInfo.IdScreen))
                        {
                            screenInfo.IdScreen = int.Parse(attr.Value);
                        }
                        else if (attr.Name == nameof(screenInfo.Rows))
                        {
                            screenInfo.Rows = int.Parse(attr.Value);
                        }
                        else if (attr.Name == nameof(screenInfo.Columns))
                        {
                            screenInfo.Columns = int.Parse(attr.Value);
                        }
                        else if (attr.Name == nameof(screenInfo.StartX))
                        {
                            screenInfo.StartX = int.Parse(attr.Value);
                        }
                        else if (attr.Name == nameof(screenInfo.StartY))
                        {
                            screenInfo.StartY = int.Parse(attr.Value);
                        }
                        else if (attr.Name == nameof(screenInfo.WallPixW))
                        {
                            screenInfo.WallPixW = int.Parse(attr.Value);
                        }
                        else if (attr.Name == nameof(screenInfo.WallPixH))
                        {
                            screenInfo.WallPixH = int.Parse(attr.Value);
                        }
                        else if (attr.Name == nameof(screenInfo.IsMirror))
                        {
                            bool isMirror = false;
                            bool.TryParse(attr.Value, out isMirror);
                            screenInfo.IsMirror = isMirror;
                        }
                        else if (attr.Name == nameof(screenInfo.BindMasterId))
                        {
                            screenInfo.BindMasterId = int.Parse(attr.Value);
                        }
                        else if (attr.Name == nameof(screenInfo.UnitWidth))
                        {
                            screenInfo.UnitWidth = int.Parse(attr.Value);
                        }
                        else if (attr.Name == nameof(screenInfo.UnitHeight))
                        {
                            screenInfo.UnitHeight = int.Parse(attr.Value);
                        }
                        else if (attr.Name == nameof(screenInfo.GapWidth))
                        {
                            screenInfo.GapWidth = int.Parse(attr.Value);
                        }
                        else if (attr.Name == nameof(screenInfo.GapHeight))
                        {
                            screenInfo.GapHeight = int.Parse(attr.Value);
                        }
                        else if (attr.Name== "Rxid")
                        {
                            string temp = attr.Value;

                            string[] arrayStr = temp.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries); //Split 方法忽略的任何元素 separator 其值是 null 或空字符串 ("")。
                            for (int i = 0; i < arrayStr.Length; i++)
                            {
                                screenInfo.Rxid.Add(arrayStr[i]);
                            }
                        }
                    }
                    list.Add(screenInfo);
                    if (currentNode.HasChildNodes)
                    {
                        foreach (XmlNode item2 in currentNode.ChildNodes)
                        {
                            #region  Rxid----
                            //if (item2.LocalName == "BindingRxes")  //已改为在特性中处理
                            //{
                            //    foreach (XmlAttribute attr in item2.Attributes)
                            //    {
                            //        if (attr.Name == "Rxid")
                            //        {
                            //            string temp = item2.Value;

                            //            string[] arrayStr = temp.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries); //Split 方法忽略的任何元素 separator 其值是 null 或空字符串 ("")。
                            //            for (int i = 0; i < arrayStr.Length; i++)
                            //            {
                            //                screenInfo.Rxid.Add(arrayStr[i]);
                            //            }
                            //        }
                            //    }
                            //}
                            #endregion

                            #region  CurrentWins----
                            if (item2.LocalName == "CurrentWins")
                            {
                                foreach (XmlAttribute attr in item2.Attributes)
                                {
                                    if (attr.Name == "Wins")
                                    {
                                        string temp = attr.Value;

                                        string[] arrayStr = temp.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries); //Split 方法忽略的任何元素 separator 其值是 null 或空字符串 ("")。
                                        for (int i = 0; i < arrayStr.Length; i++)
                                        {
                                            string[] arrayTxRx = arrayStr[i].Split(',');
                                            if (arrayTxRx.Length<7)
                                            {
                                                Logger.Error("xml中ScreenInfo-CurrentWins格式有错误！-- ‘，’分割数量<7");
                                                continue;
                                            }
                                            HsMosaicWinInfo winInfo = new HsMosaicWinInfo();
                                            winInfo.IdWin =int.Parse(arrayTxRx[0]);
                                            winInfo.IdTx = arrayTxRx[1];
                                            winInfo.ZIndex = int.Parse(arrayTxRx[2]);
                                            winInfo.X = int.Parse(arrayTxRx[3]);
                                            winInfo.Y = int.Parse(arrayTxRx[4]);
                                            winInfo.Width = int.Parse(arrayTxRx[5]);
                                            winInfo.Height = int.Parse(arrayTxRx[6]);
                                            winInfo.IsEnable = true;
                                            screenInfo.CurrentWins.Add(winInfo);
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region  Scenes----
                            if (item2.LocalName == "Scene")
                            {
                                SceneInfo sceneInfo = new SceneInfo();
                                sceneInfo.IdScreen = screenInfo.IdScreen;
                                screenInfo.Scenes.Add(sceneInfo);
                                foreach (XmlAttribute attr in item2.Attributes)
                                {
                                    if (attr.Name == "Name")
                                    {
                                        sceneInfo.Name = attr.Value;
                                    }
                                    else if (attr.Name == "ListWins")
                                    {
                                        string temp = attr.Value;

                                        string[] arrayStr = temp.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries); //Split 方法忽略的任何元素 separator 其值是 null 或空字符串 ("")。
                                        for (int i = 0; i < arrayStr.Length; i++)
                                        {
                                            string[] arrayTxRx = arrayStr[i].Split(',');
                                            if (arrayTxRx.Length < 7)
                                            {
                                                Logger.Error("xml中SceneInfo-ListWins格式有错误！-- ‘，’分割数量<7");
                                                continue;
                                            }
                                            HsMosaicWinInfo winInfo = new HsMosaicWinInfo();
                                            winInfo.IdWin = int.Parse(arrayTxRx[0]);
                                            winInfo.IdTx = arrayTxRx[1];
                                            winInfo.ZIndex = int.Parse(arrayTxRx[2]);
                                            winInfo.X = int.Parse(arrayTxRx[3]);
                                            winInfo.Y = int.Parse(arrayTxRx[4]);
                                            winInfo.Width = int.Parse(arrayTxRx[5]);
                                            winInfo.Height = int.Parse(arrayTxRx[6]);
                                            winInfo.IsEnable = true;
                                            sceneInfo.ListWins.Add(winInfo);
                                        }
                                    }
                                }
                            }
                            #endregion
                            #region  PrePlans----
                            if (item2.LocalName == "PrePlan")
                            {
                                PrePlanInfo info = new PrePlanInfo();
                                info.IdScreen = screenInfo.IdScreen;
                                screenInfo.PrePlans.Add(info);
                                foreach (XmlAttribute attr in item2.Attributes)
                                {
                                    if (attr.Name == "Name")
                                    {
                                        info.Name = attr.Value;
                                    }
                                    else if (attr.Name == "SwiInterval")
                                    {
                                        info.SwiInterval = int.Parse(attr.Value);
                                    }
                                    //else if (attr.Name == "Scenes")
                                    //{
                                    //    string[] arrayTxRx = attr.Value.Split(',');
                                    //    foreach (var name in arrayTxRx)
                                    //    {
                                    //        if (!string.IsNullOrWhiteSpace(name))
                                    //        {
                                    //            info.SceneNames.Add(name);
                                    //        }
                                    //    }
                                    //}
                                }
                                if (item2.HasChildNodes)
                                {
                                    foreach (XmlNode item3 in item2.ChildNodes)
                                    {
                                        if (!string.IsNullOrWhiteSpace(item3.InnerText))
                                        {
                                            info.SceneNames.Add(item3.InnerText);
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }

                    if (dicName != null)
                    {
                        if (!string.IsNullOrEmpty(screenInfo.Name))
                        {
                            if (!dicName.ContainsKey(screenInfo.Name))
                            {
                                dicName.Add(screenInfo.Name, screenInfo);
                            }
                            else
                            {
                                Logger.Trace("GetXmlMosaicScreenInfo中xml的Name值有重复，将忽略！--" + screenInfo.Name);
                            }
                        }
                        else
                        {
                            Logger.Trace("GetXmlMosaicScreenInfo中xml的Name值为空/null");
                        }
                    }
                    if (dicId != null)
                    {
                        if (!dicId.ContainsKey(screenInfo.IdScreen))
                        {
                            dicId.Add(screenInfo.IdScreen, screenInfo);
                        }
                        else
                        {
                            Logger.Trace("GetXmlMosaicScreenInfo中xml的Id值有重复，将忽略！--" + screenInfo.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("读取GetXmlMosaicScreenInfo异常：" + ex.Message);
            }
            Logger.Info("GetXmlMosaicScreenInfo存储xml数据列表中数量：" + list.Count);
            
            return list;
        }


        private XmlDocument InsertXmlOneKeyMode(string path, string node, string element, List<string> attributes, List<string> values, string elementValue = null)
        {
            XmlDocument doc = null;
            try
            {
                doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = doc.CreateElement(element);
                if (!string.IsNullOrEmpty(elementValue))
                    // <person><Num>88</Num></person>  XMLHelper.Insert(path, "PersonF/person[@Name='Person2']","Num", "", "88");
                    xe.InnerText = elementValue;

                // <person> <Num ID="88" /></person>  XMLHelper.Insert(path, "PersonF/person[@Name='Person2']", "", "ID", "88");
                for (int i = 0; i < attributes.Count; i++)
                {
                    xe.SetAttribute(attributes[i], values[i]);
                }
                xn.AppendChild(xe);
            }
            catch (Exception ex)
            {
                Logger.Error("InsertXmlOneKeyMode方法异常：" + ex.Message);
            }
            return doc;
        }
        private XmlDocument AddChildXmlNode(XmlDocument doc, string element, List<string> attributes, List<string> values, string elementValue = null)
        {
            if (doc == null | attributes == null | values == null)
            {
                return doc;
            }
            if (attributes.Count == 0 | attributes.Count != values.Count)
            {
                Logger.Error("AddChildXmlNode中列表参数数量为空或特性数量不相等！");
                return doc;
            }
            try
            {
                XmlElement xe = doc.CreateElement(element);
                if (!string.IsNullOrEmpty(elementValue))
                    // <person><Num>88</Num></person>  XMLHelper.Insert(path, "PersonF/person[@Name='Person2']","Num", "", "88");
                    xe.InnerText = elementValue;

                // <person> <Num ID="88" /></person>  XMLHelper.Insert(path, "PersonF/person[@Name='Person2']", "", "ID", "88");
                for (int i = 0; i < attributes.Count; i++)
                {
                    xe.SetAttribute(attributes[i], values[i]);
                }
                doc.DocumentElement.LastChild.AppendChild(xe);  //要求为当前根目录下的最后一个节点内添加节点
            }
            catch (Exception ex)
            {
                Logger.Error("AddChildXmlNode方法异常：" + ex.Message);
            }
            return doc;
        }
        public static void AddChildXmlElements(string path, string node,string element,string childrens,List<string> InnerTexts, List<string> attributes, List<string> values)
        {
            if (InnerTexts==null|| attributes == null | values == null)
            {
                return ;
            }
            if (attributes.Count != values.Count)
            {
                Logger.Trace("AddChildXmlElements-中列表参数数量为空或特性数量不相等！");
                return ;
            }
            try
            {
                // <person> <Num ID="88" /></person>  XMLHelper.Insert(path, "PersonF/person[@Name='Person2']", "", "ID", "88");
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);

                XmlElement newNode = doc.CreateElement(element);

                for (int i = 0; i < attributes.Count; i++)
                {
                    newNode.SetAttribute(attributes[i], values[i]);
                }
                foreach (var item in InnerTexts)
                {
                    XmlElement xe = doc.CreateElement(childrens);
                    xe.InnerText = item;
                    newNode.AppendChild(xe);
                }
                xn.AppendChild(newNode);
                doc.Save(path);
            }
            catch (Exception ex)
            {
                Logger.Error("AddChildXmlElements-方法异常：" + ex.Message);
            }         
        }
         

        //----------------------------------------------基本方法----------------------------------------------
        /// <summary>
        /// 向节点中增加节点元素，属性
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">要操作的节点</param>
        /// <param name="element">要增加的节点元素，可空可不空。非空时插入新的元素，否则插入该元素的属性</param>
        /// <param name="attribute">要增加的节点属性，可空可不空。非空时插入元素值，否则插入元素值</param>
        /// <param name="value">要增加的节点值</param>
        /// 使用实例：XMLHelper.Insert(path, "PersonF/person[@Name='Person2']","Num", "ID", "88");
        /// XMLHelper.Insert(path, "PersonF/person[@Name='Person2']","Num", "", "88");
        /// XMLHelper.Insert(path, "PersonF/person[@Name='Person2']", "", "ID", "88");
        public static void InsertHsXml(string path, string node, string element, List<string> attributes, List<string> values, string elementValue = null)
        {
            if (attributes == null | values == null | attributes.Count != values.Count)
            {
                Logger.Error("Insert的参数中两个列表数量不对称或为null!,将忽略处理");
                return;
            }
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                //如果element，则增加该属性 
                if (string.IsNullOrEmpty(element))
                {
                    //如果attribute不为空，增加该属性
                    if (attributes.Count > 0)
                    {
                        XmlElement xe = (XmlElement)xn;
                        // <person Name="Person2" ID="88"> XMLHelper.Insert(path, "PersonF/person[@Name='Person2']","Num", "ID", "88");
                        for (int i = 0; i < attributes.Count; i++)
                        {
                            xe.SetAttribute(attributes[i], values[i]);
                        }
                    }
                }
                else //如果element不为空，则preson下增加节点   
                {
                    XmlElement xe = doc.CreateElement(element);
                    if (!string.IsNullOrEmpty(elementValue))
                        // <person><Num>88</Num></person>  XMLHelper.Insert(path, "PersonF/person[@Name='Person2']","Num", "", "88");
                        xe.InnerText = elementValue;

                    // <person> <Num ID="88" /></person>  XMLHelper.Insert(path, "PersonF/person[@Name='Person2']", "", "ID", "88");
                    for (int i = 0; i < attributes.Count; i++)
                    {
                        xe.SetAttribute(attributes[i], values[i]);
                    }
                    xn.AppendChild(xe);
                }
                doc.Save(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Logger.Error("InsertHsXml-异常：" + e.Message);
            }
        }


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">要删除的节点</param>
        /// <param name="attributes">属性列表，=null则删除整个节点，不为空则删除节点中的属性</param>
        ///  实例：XMLHelper.Delete(path, "PersonF/person[@Name='Person3']/ID", "");
        /// XMLHelper.Delete(path, "PersonF/person[@Name='Person3']/ID", "Num");
        public static void DeleteHsXml(string path, string node, List<string> attributes = null)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attributes == null)
                    xn.ParentNode.RemoveChild(xn);// <ID Num="999">888</ID>的整个节点将被移除  XMLHelper.Delete(path, "PersonF/person[@Name='Person3']/ID", "");
                else
                    foreach (var item in attributes)
                    {
                        xe.RemoveAttribute(item);//<ID Num="999">888</ID> 变为<ID>888</ID> XMLHelper.Delete(path, "PersonF/person[@Name='Person3']/ID", "Num");
                    }
                doc.Save(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void UpdateHsXml(string path, string node, List<string> attributes, List<string> values, string innerText = null)
        {
            if (attributes == null | values == null | attributes.Count != values.Count)
            {
                Logger.Error("UpdateHsXml的参数中两个列表数量不对称或为null!,将忽略处理");
                return;
            }
            XmlDocument doc = null;
            try
            {
                 doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;

                if (!string.IsNullOrEmpty(innerText))
                    xe.InnerText = innerText;//原<ID>2</ID> 改变:<ID>888</ID>  XMLHelper.Update(path, "PersonF/person[@Name='Person3']/ID", "", "888");
                for (int i = 0; i < attributes.Count; i++)
                {
                    xe.SetAttribute(attributes[i], values[i]); //原<ID Num="3">888</ID> 改变<ID Num="999">888</ID>    XMLHelper.Update(path, "PersonF/person[@Name='Person3']/ID", "Num", "999"); 

                }
                doc.Save(path);
                //Console.WriteLine("UpdateHsXml-保存成功："+node+values[0]);
            }
            catch (Exception e)
            {
                Logger.Error("UpdateHsXml-异常：" + e.Message);
                //Console.WriteLine(e.Message);
            } 
        }


        #region GetXmlTx
        

        #endregion


    }


}
