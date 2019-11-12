using System;
using System.Collections;
using System.Xml;
using System.Configuration;

using WCMS.FrameWork.UtilityLibrary;

namespace WCMS.FrameWork.Data.Configuration
{

    /// <summary>
    /// appSettings등을 포함하고 있는 XML 설정파일을 읽어서 Dictionary로 관리하는 클래스
    /// </summary>
    public class XmlConfigReader : IDictionary 
    {
        // Fields
        protected string _configFile;
        protected IDictionary appSettingsDictionary;

        /// <summary>
        /// 생성자
        /// </summary>
        public XmlConfigReader()
        {
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="configFile">config 파일 경로</param>
        public XmlConfigReader(string configFile)
            : this(configFile, "appSettings")
        {
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="configFile">config 파일 경로</param>
        /// <param name="settingName">요소 명</param>
        public XmlConfigReader(string configFile, string settingName)
        {
            this._configFile = configFile;
            this.LoadConfig(settingName);
        }

        /// <summary>
        /// 제공된 키와 값이 있는 요소를 IDictionary 개체에 추가합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void Add(object key, object value)
        {
            this.appSettingsDictionary.Add(key, value);
        }

        /// <summary>
        /// IDictionary  개체에서 요소를 모두 제거합니다.
        /// </summary>
        public virtual void Clear()
        {
            this.appSettingsDictionary.Clear();
        }

        /// <summary>
        /// IDictionary  개체에 지정된 키가 있는 요소가 포함되어 있는지 여부를 확인합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool Contains(object key)
        {
            return this.appSettingsDictionary.Contains(key);
        }

        /// <summary>
        /// 특정 Array 인덱스에서 시작하여 ICollection의 요소를 Array에 복사합니다. (ICollection에서 상속됨)
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public virtual void CopyTo(Array array, int arrayIndex)
        {
            this.appSettingsDictionary.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// IDictionary  개체의 IDictionaryEnumerator 개체를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public virtual IDictionaryEnumerator GetEnumerator()
        {
            return null;
        }

        protected void LoadConfig(string settingName)
        {
            if (this.appSettingsDictionary == null)
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    XmlTextReader reader = new XmlTextReader(this._configFile);
                    xmlDoc.Load(reader);
                    reader.Close();
                    
                    XmlNodeList nodeList = xmlDoc.GetElementsByTagName(settingName);
                    foreach (XmlNode node in nodeList) 
                    {
                        if (node.LocalName == settingName)
                        {
                            this.appSettingsDictionary = (IDictionary)new DictionarySectionHandler().Create(null, null, node);
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// IDictionary  개체에서 지정된 키를 가진 요소를 제거합니다.
        /// </summary>
        /// <param name="key"></param>
        public virtual void Remove(object key)
        {
            this.appSettingsDictionary.Remove(key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }

        /// <summary>
        /// ICollection 에 포함된 요소 수를 가져옵니다. (ICollection에서 상속됨)
        /// </summary>
        public virtual int Count
        {
            get
            {
                return this.appSettingsDictionary.Count;
            }
        }

        /// <summary>
        /// IDictionary  개체의 크기가 고정되어 있는지 여부를 나타내는 값을 가져옵니다.
        /// </summary>
        public virtual bool IsFixedSize
        {
            get
            {
                return this.appSettingsDictionary.IsFixedSize;
            }
        }

        /// <summary>
        /// IDictionary  개체가 읽기 전용인지 여부를 나타내는 값을 가져옵니다.
        /// </summary>
        public virtual bool IsReadOnly
        {
            get
            {
                return this.appSettingsDictionary.IsReadOnly;
            }
        }

        /// <summary>
        /// ICollection 에 대한 액세스가 동기화되어 스레드로부터 안전하게 보호되는지 여부를 나타내는 값을 가져옵니다. (ICollection에서 상속됨)
        /// </summary>
        public virtual bool IsSynchronized
        {
            get
            {
                return this.appSettingsDictionary.IsSynchronized;
            }
        }

        public virtual object this[object key]
        {
            get
            {
                string text = null;
                if (this.appSettingsDictionary != null)
                {
                    text = this.appSettingsDictionary[key] as string;
                }
                return text;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// IDictionary  개체의 키를 포함하는 ICollection 개체를 가져옵니다.
        /// </summary>
        public virtual ICollection Keys
        {
            get
            {
                return this.appSettingsDictionary.Keys;
            }
        }

        /// <summary>
        /// ICollection 에 대한 액세스를 동기화하는 데 사용할 수 있는 개체를 가져옵니다. (ICollection에서 상속됨)
        /// </summary>
        public virtual object SyncRoot
        {
            get
            {
                return this.appSettingsDictionary.SyncRoot;
            }
        }

        /// <summary>
        /// IDictionary  개체의 값이 포함된 ICollection 개체를 가져옵니다.
        /// </summary>
        public virtual ICollection Values
        {
            get
            {
                return this.appSettingsDictionary.Values;
            }
        }

        protected T GetValue<T>(object key, T defaultValue) where T : IConvertible
        {
            return GlobalConvert.ChangeType<T>(this[key], defaultValue);
        }
    }
}
