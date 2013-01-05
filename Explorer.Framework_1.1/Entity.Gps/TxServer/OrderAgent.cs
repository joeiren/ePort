using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Explorer.Framework.Data.ORM;

namespace Entity.Gps.TxServer
{
    [ORMTable(Name = "orderAgent")]
    public class OrderAgent : ORMEntity
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        [ORMColumn(Name = "orderAgentId", IsIdentity = true, IsAutoFill = true, IsPrimaryKey = true, DbType = System.Data.DbType.Int32)]
        public int? OrderAgentId
        {
            get { return _orderAgentId; }
            set
            {
                _orderAgentId = value;
                ReportPropertyChanged("OrderAgentId");
            }
        }int? _orderAgentId;

        /// <summary>
        /// 
        /// </summary>
        [ORMColumn(Name = "orderAgentName", DbType = System.Data.DbType.String, Size = 50)]
        public string OrderAgentName
        {
            get { return _orderAgentName; }
            set
            {
                _orderAgentName = value;
                ReportPropertyChanged("OrderAgentName");
            }
        }string _orderAgentName;

        /// <summary>
        /// 
        /// </summary>
        [ORMColumn(Name = "orderAgentNo", DbType = System.Data.DbType.String, Size = 50)]
        public string OrderAgentNo
        {
            get { return _orderAgentNo; }
            set
            {
                _orderAgentNo = value;
                ReportPropertyChanged("OrderAgentNo");
            }
        }string _orderAgentNo;

        /// <summary>
        /// 
        /// </summary>
        [ORMColumn(Name = "linkMan", DbType = System.Data.DbType.String, Size = 18)]
        public string LinkMan
        {
            get { return _linkMan; }
            set
            {
                _linkMan = value;
                ReportPropertyChanged("LinkMan");
            }
        }string _linkMan;

        /// <summary>
        /// 
        /// </summary>
        [ORMColumn(Name = "telephone", DbType = System.Data.DbType.String, Size = 50)]
        public string Telephone
        {
            get { return _telephone; }
            set
            {
                _telephone = value;
                ReportPropertyChanged("Telephone");
            }
        }string _telephone;

        /// <summary>
        /// 
        /// </summary>
        [ORMColumn(Name = "remark", DbType = System.Data.DbType.String, Size = 200)]
        public string Remark
        {
            get { return _remark; }
            set
            {
                _remark = value;
                ReportPropertyChanged("Remark");
            }
        }string _remark;

        /// <summary>
        /// 
        /// </summary>
        [ORMColumn(Name = "VGroupID", DbType = System.Data.DbType.Int64, Size = 64)]
        public Int64? VGroupID
        {
            get { return _vGroupID; }
            set
            {
                _vGroupID = value;
                ReportPropertyChanged("VGroupID");
            }
        }Int64? _vGroupID;

        /// <summary>
        /// 
        /// </summary>
        [ORMColumn(Name = "fax", DbType = System.Data.DbType.String, Size = 50)]
        public string Fax
        {
            get { return _fax; }
            set
            {
                _fax = value;
                ReportPropertyChanged("Fax");
            }
        }string _fax;

        /// <summary>
        /// 
        /// </summary>
        [ORMColumn(Name = "mobile", DbType = System.Data.DbType.String, Size = 50)]
        public string Mobile
        {
            get { return _mobile; }
            set
            {
                _mobile = value;
                ReportPropertyChanged("Mobile");
            }
        }string _mobile;

        /// <summary>
        /// 
        /// </summary>
        [ORMColumn(Name = "address", DbType = System.Data.DbType.String, Size = 200)]
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                ReportPropertyChanged("Address");
            }
        }string _address;
    }
}
