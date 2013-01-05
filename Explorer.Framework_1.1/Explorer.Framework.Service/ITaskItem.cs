using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Service
{
    /// <summary>
    /// 独立任务单元
    /// </summary>
    public interface ITaskItem
    {
        /// <summary>
        /// 任务处理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        object Process(object args);
    }
}
