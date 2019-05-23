using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.Caching;

namespace CLSLibrary
{
    /// <summary>
    /// 包含XML文件操作的一系列方法
    /// </summary>
    public class XMLHelper
    {
        private XmlDocument _xmlDoc = null;

        /// <summary>
        /// 当前处理的xml文件
        /// </summary>
        public XmlDocument xmlDoc
        {
            get { return _xmlDoc; }
        }

        /// <summary>
        /// 是否存在节点
        /// <para>filepath-文件绝对路径</para>
        /// <para>xpath-节点路径，xpath形式</para>
        /// </summary>
        /// <returns></returns>
        public bool HasNode(string filepath, string xpath)
        {
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(filepath);
                XmlNodeList xList = xDoc.SelectNodes(xpath);
                return xList.Count > 0;
            }
            catch { return false; }
        }

        /// <summary>
        /// 读取xml文件某节点的文本
        /// <para>filepath-文件绝对路径</para>
        /// <para>xpath-节点路径，xpath形式</para>
        /// </summary>
        /// <returns></returns>
        public string ReadXText(string filepath, string xpath)
        {
            string result = null;
            XmlDocument xDoc = new XmlDocument();
            try
            {
                if (!HasNode(filepath, xpath))
                {
                    throw new Exception("不存在节点： " + xpath);
                }
                xDoc.Load(filepath);
                result = xDoc.SelectNodes(xpath).Item(0).InnerText.Trim();
            }
            catch { throw; }
            return result;
        }


        /// <summary>
        /// 读取xml文件某节点的属性值
        /// <para>filepath-文件绝对路径</para>
        /// <para>xpath-节点路径，xpath形式</para>
        /// <para>attrname-要获取的属性名称</para>
        /// </summary>
        /// <returns></returns>
        public string ReadXValue(string filepath, string xpath, string attrname)
        {
            string result = null;
            XmlDocument xDoc = new XmlDocument();
            try
            {
                if (!HasNode(filepath, xpath))
                {
                    throw new Exception("不存在节点： " + xpath);
                }
                xDoc.Load(filepath);
                result = xDoc.SelectNodes(xpath).Item(0).Attributes[attrname].Value;
            }
            catch { throw; }
            return result;
        }

        /// <summary>
        /// 获取符合条件的节点的集合，返回属性名称和属性值的字典集合
        /// <para>filepath-文件绝对路径</para>
        /// <para>xpath-节点路径，xpath形式</para>
        /// <para>attributenames-节点属性名，为null表示选取所有</para>
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, string>> ReadXMLValues(string filepath, string xpath, string[] attributenames = null)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            XmlDocument xDoc = new XmlDocument();
            try
            {
                if (!HasNode(filepath, xpath))
                {
                    throw new Exception("不存在节点： " + xpath);
                }
                xDoc.Load(filepath);
                XmlNodeList xList = xDoc.SelectNodes(xpath);
                foreach (XmlNode xnode in xList)
                {
                    Dictionary<string, string> xdic = new Dictionary<string, string>();
                    foreach (XmlAttribute xattr in xnode.Attributes)
                    {
                        if (null == attributenames || attributenames.Contains(xattr.Name))
                        {
                            xdic.Add(xattr.Name, xattr.Value);
                        }
                    }
                    result.Add(xdic);
                }
            }
            catch { throw; }
            return result;
        }
    }

    /// <summary>
    /// 自定义配置帮助类
    /// </summary>
    public class ConfigHelper : System.Dynamic.DynamicObject
    {
        private XMLHelper _xh = null;
        private MemoryCache _mCache = null;

        public ConfigHelper(string configFilePath)
        {
            _xh = new XMLHelper();
            _mCache = MemoryCache.Default;
            MyConfigFilePath = configFilePath;
        }

        /// <summary>
        /// MyConfig的动态当前实例
        /// </summary>
        public dynamic Dynamic
        {
            get
            {
                return this;
            }
        }

        public string MyConfigFilePath
        {
            get;
            set;
        }

        /// <summary>
        /// 读取配置文件的配置节文本
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public string GetConfigValueFromFile(string configXPath)
        {
            return _xh.ReadXText(MyConfigFilePath, configXPath);
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public string GetConfigValueByXPath(string configXPath)
        {
            if (!IsCacheExists(configXPath))
            {
                AddCacheItem(configXPath);
            }
            return _mCache[configXPath].ToString();
        }

        /// <summary>
        /// 返回特定类型的配置项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configXPath"></param>
        /// <returns></returns>
        public T GetConfigValueByXPath<T>(string configXPath)
        {
            try
            {
                string strVal = GetConfigValueByXPath(configXPath);
                T t = (T)Convert.ChangeType(strVal, typeof(T));
                return t;
            }
            catch { return default(T); }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public string GetConfigValueByKey(string configKey)
        {
            string configXPath = "//" + configKey;
            return GetConfigValueByXPath(configXPath);
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public T GetConfigValueByKey<T>(string configKey)
        {
            string configXPath = "//" + configKey;
            return GetConfigValueByXPath<T>(configXPath);
        }

        /// <summary>
        /// 缓存项是否存在
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public bool IsCacheExists(string cacheKey)
        {
            return _mCache.Contains(cacheKey);
        }

        /// <summary>
        /// 读取配置到缓存中
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        private void AddCacheItem(string cacheKey)
        {
            string configXPath = cacheKey;
            string configValue = GetConfigValueFromFile(configXPath);
            CacheItem item = new CacheItem(cacheKey, configValue);
            CacheItemPolicy policy = new CacheItemPolicy();
            List<string> fileList = new List<string>() { MyConfigFilePath };
            HostFileChangeMonitor fileMonitor = new HostFileChangeMonitor(fileList);
            policy.ChangeMonitors.Add(fileMonitor);
            if (IsCacheExists(cacheKey))
            {
                _mCache.Set(item, policy);
            }
            else
            {
                _mCache.Add(item, policy);
            }
        }

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            try
            {
                result = GetConfigValueByKey(binder.Name);
                return true;
            }
            catch
            {
                result = null;
                return true;
            }
        }

        #region 具体配置项

        #endregion
    }
}
