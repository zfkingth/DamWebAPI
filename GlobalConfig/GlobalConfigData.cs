using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Hammergo.GlobalConfig
{

    public class GlobalConfigData
    {

        string _serviceURI;
        [Description("�����ַ���,�벻Ҫ����޸�")]
        /// <summary>
        /// ���ݿ������ַ���
        /// </summary>
        public string ServiceURI
        {
            get
            {
                return _serviceURI;
            }
            set
            {
                _serviceURI = value;
            }
        }

        List<ParamInfo> _constParamsList = null;
        [Description("���������б������ò���ʱ��Ԥ��ѡ��")]
        /// <summary>
        /// ���������б�
        /// </summary>
        public List<ParamInfo> ConstParamsList
        {
            get
            {
                return _constParamsList;
            }

            set
            {
                _constParamsList = value;

            }
        }


        List<ParamInfo> _defaultParamsList = null;
        [Description("��������,��������б�,���趨�Ĳ���С��λ����������ĵ�С��λ��")]
        /// <summary>
        /// ��������,��������б�
        /// </summary>
        public List<ParamInfo> DefaultParamsList
        {
            get
            {
                return _defaultParamsList;
            }

            set
            {
                _defaultParamsList = value;

            }
        }

        List<double> _errorValList = null;
        [Description("�쳣ֵ�б�")]
        /// <summary>
        /// �쳣ֵ�б�
        /// </summary>
        public List<double> ErrorValList
        {
            get
            {
                return _errorValList;
            }

            set
            {
                _errorValList = value;

            }
        }


        List<LineStyleInfo> _lineStyleInfoList = null;
        [Description("ͼ������������Զ����б�,����ͼ�δ��������ò�����")]
        /// <summary>
        /// ͼ������������Զ����б�
        /// </summary>
        public List<LineStyleInfo> LineStyleInfoList
        {
            get
            {
                return _lineStyleInfoList;
            }

            set
            {
                _lineStyleInfoList = value;

            }
        }


        private string _noDataConvertString;
        [Description("����Ϊ��ʱ�����е�ת���ַ���")]
        /// <summary>
        /// ����Ϊ��ʱ�����е�ת���ַ���
        /// </summary>
        public string NoDataConvertString
        {
            get
            {
                return _noDataConvertString;
            }
            set
            {
                _noDataConvertString = value;
            }
        }


        private string _errorConvertString;
        [Description("���������쳣ֵʱ,�����е�ת���ַ���")]
        /// <summary>
        /// ���������쳣ֵʱ,�����е�ת���ַ���
        /// </summary>
        public string ErrorConvertString
        {
            get
            {
                return _errorConvertString;
            }
            set
            {
                _errorConvertString = value;
            }
        }


        private int _lastedRecordNum=15;

        [Description("����ԭʼ��������ʾ������ݵĸ���")]
        /// <summary>
        /// ����ԭʼ��������ʾ������ݵĸ���
        /// </summary>
        public int LastedRecordNum
        {
            get
            {
                return _lastedRecordNum;
            }
            set
            {
                _lastedRecordNum = value;
            }
        }

        private int _changeRangeNumForMonth = 4;

        [Description("�±仯�������¸���������")]
        /// <summary>
        /// �±仯�������¸���������
        /// </summary>
        public int  ChangeRangeNumForMonth
        {
            get
            {
                return _changeRangeNumForMonth;
            }
            set
            {
                _changeRangeNumForMonth = value;
            }
        }


        private int _changeRangeNumForYear = 15;

        [Description("��仯�������¸���������")]
        /// <summary>
        /// ��仯�������¸���������
        /// </summary>
        public int ChangeRangeNumForYear
        {
            get
            {
                return _changeRangeNumForYear;
            }
            set
            {
                _changeRangeNumForYear = value;
            }
        }

        private double _daysNumForNear = 3;

        [Description("��������ʱ��ָ�������Ŀɸ���������������С��")]
        /// <summary>
        /// ��������ʱ��ָ�������Ŀɸ���������������С��
        /// </summary>
        public double DaysNumForNear
        {
            get
            {
                return _daysNumForNear;
            }
            set
            {
                _daysNumForNear = value;
            }
        }


        private double _graphicWidth = 700;
        [Description("ͼ�ο��")]
        /// <summary>
        ///ͼ���ұ����հ�����Ŀ��
        /// </summary>
        public double GraphicWidth
        {
            get
            {
                return _graphicWidth;
            }
            set
            {
                _graphicWidth = value;
            }
        }

        private double _graphicHeight= 260;
        [Description("ͼ�θ߶�")]
        /// <summary>
        ///ͼ�εײ����հ�����ĸ߶�
        /// </summary>
        public double GraphicHeight
        {
            get
            {
                return _graphicHeight;
            }
            set
            {
                _graphicHeight = value;
            }
        }


        string _skinName;
        [Description("��ǰ����ʹ�õ�Ƥ������")]
        /// <summary>
        /// ��ǰ����ʹ�õ�Ƥ������
        /// </summary>
        public string SkinName
        {
            get
            {
                return _skinName;
            }
            set
            {
                _skinName = value;
            }
        }

        private double _checkTimes = 1.3;

        [Description("��������������ʱ�����仯��ʱ�Ƿ�����ʾ���ٽ籶������ֵ����ǰ�������ݵ����仯�����������ݱ仯���ٽ�ֵ")]
        /// <summary>
        /// ��������������ʱ�����仯��ʱ�Ƿ�����ʾ���ٽ籶������ֵ����ǰ�������ݵ����仯�����������ݱ仯���ٽ�ֵ
        /// </summary>
        public double CheckTimes
        {
            get
            {
                return _checkTimes;
            }
            set
            {
                _checkTimes = value;
            }
        }


        string _registerCode;
        [Description("ע����")]
        /// <summary>
        /// ��ǰ����ʹ�õ�Ƥ������
        /// </summary>
        public string RegisterCode
        {
            get
            {
                return _registerCode;
            }
            set
            {
                _registerCode = value;
            }
        }


    }
}
