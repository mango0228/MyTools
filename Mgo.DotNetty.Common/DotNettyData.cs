using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgo.DotNetty.Common
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DotNettyData
    {
        /// <summary>
        /// 唯一序列号
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object Data { get; set; }
    }
}
