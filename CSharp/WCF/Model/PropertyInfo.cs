using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateProgrammeCode.CSharp.WCF.Model
{
    /// <summary>
    /// 属性信息
    /// </summary>
    public class PropertyInfo : INotifyPropertyChanged
    {
        string _PropertyName;

        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName
        {
            get { return _PropertyName; }
            set
            {
                _PropertyName = value;
                this.OnPropertyChanged("PropertyName");
            }
        }

        string _PropertyNote;
        /// <summary>
        /// 注释
        /// </summary>
        public string PropertyNote
        {
            get { return _PropertyNote; }
            set
            {
                _PropertyNote = value;
                this.OnPropertyChanged("PropertyNote");
            }
        }

        string _PropertyType;
        /// <summary>
        /// 属性类型
        /// </summary>
        public string PropertyType
        {
            get { return _PropertyType; }
            set
            {
                _PropertyType = value;
                this.OnPropertyChanged("PropertyType");
            }
        }

        string _PropertyDBType;
        /// <summary>
        /// 属性数据库类型
        /// </summary>
        public string PropertyDBType
        {
            get { return _PropertyDBType; }
            set
            {
                _PropertyDBType = value;
                this.OnPropertyChanged("PropertyDBType");
            }
        }



        #region 实现INotifyPropertyChanged接口
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
