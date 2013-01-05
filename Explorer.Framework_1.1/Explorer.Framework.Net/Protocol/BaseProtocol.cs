using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Explorer.Framework.Net.Protocol
{
    /// <summary>
    /// 所有协议基类
    /// </summary>    
    public abstract class BaseProtocol : IParser, IBuilder
    {
        /// <summary>
        /// 当前帧数据
        /// </summary>
        public byte[] FrameData { get; set; }

        /// <summary>
        /// 解析当前数据内容, 返回不能解析的下层协议
        /// </summary>
        /// <param name="dataContent">上层数据内容bytes</param>
        /// <returns></returns>
        public virtual byte[] Parse(byte[] dataContent)
        {
            this.FrameData = dataContent;
            return this.FrameData;
        }

        /// <summary>
        /// 把当前内容建立成协议数据
        /// </summary>
        /// <returns></returns>
        public virtual byte[] Build()
        {
            return Build(this.FrameData);
        }

        /// <summary>
        /// 把下层协议加入本层协议中, 建立新的数据
        /// </summary>
        /// <param name="dataContent">下层协议数据</param>
        /// <returns></returns>
        public virtual byte[] Build(byte[] dataContent)
        {
            this.FrameData = dataContent;
            return dataContent;
        }

        /// <summary>
        /// 直接将 byte[] 映射到 Struct 或是 Class 里, 但 Struct 和 Class 必须引用 StructLayoutAttribute 特性, 并显示的申明 Field
        /// </summary>
        /// <typeparam name="T">返回的Struct或Class的泛型</typeparam>
        /// <param name="dataContent">需要映射的数据</param>
        /// <returns></returns>
        public static T Parse<T>(byte[] dataContent)
        {
            return Parse<T>(dataContent, 0);        
        }
        
        /// <summary>
        /// 直接将 byte[] 映射到 Struct 或是 Class 里, 但 Struct 和 Class 必须引用 StructLayoutAttribute 特性, 并显示的申明 Field
        /// </summary>
        /// <typeparam name="T">返回的Struct或Class的泛型</typeparam>
        /// <param name="dataContent">需要映射的数据</param>
        /// <param name="index">开始解析的位置</param>
        /// <returns></returns>
        public static T Parse<T>(byte[] dataContent, int index)
        {
            Type classType = typeof(T);
            //得到结构体的大小
            int size = Marshal.SizeOf(typeof(T));
            //byte数组长度小于结构体的大小
            if (size > dataContent.Length - index)
            {
                //返回空
                return default(T);
            }
            //分配结构体大小的内存空间
            IntPtr intPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(dataContent, index, intPtr, size);
            //将内存空间转换为目标结构体
            T entity = (T)Marshal.PtrToStructure(intPtr, classType);
            //释放内存空间
            Marshal.FreeHGlobal(intPtr);
            //返回结构体
            return entity;
        }

        /// <summary>
        /// 直接将 byte[] 映射到 Struct 或是 Class 里, 但 Struct 和 Class 必须引用 StructLayoutAttribute 特性, 并显示的申明 Field
        /// </summary>
        /// <param name="classType">返回的Struct或Class的类型</param>
        /// <param name="dataContent">需要映射的数据</param>
        /// <returns></returns>
        public static object Parse(Type classType, byte[] dataContent)
        {
            return Parse(classType, dataContent, 0);
        }

        /// <summary>
        /// 直接将 byte[] 映射到 Struct 或是 Class 里, 但 Struct 和 Class 必须引用 StructLayoutAttribute 特性, 并显示的申明 Field
        /// </summary>
        /// <param name="classType">返回的Struct或Class的类型</param>
        /// <param name="dataContent">需要映射的数据</param>
        /// <param name="index">开始解析的位置</param>
        /// <returns></returns>
        public static object Parse(Type classType, byte[] dataContent, int index)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(classType);
            //byte数组长度小于结构体的大小
            if (size > dataContent.Length - index)
            {
                //返回空
                return null;
            }
            //分配结构体大小的内存空间
            IntPtr intPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(dataContent, index, intPtr, size);
            //将内存空间转换为目标结构体
            object entity = Marshal.PtrToStructure(intPtr, classType);
            //释放内存空间
            Marshal.FreeHGlobal(intPtr);
            //返回结构体
            return entity;
        }

        /// <summary>
        /// 直接将 Struct 或是 Class 映射到新的 byte[] 里, 但 Struct 和 Class 必须引用 StructLayoutAttribute 特性, 并显示的申明 Field
        /// </summary>
        /// <param name="entity">映射的实例</param>
        /// <returns></returns>
        public static byte[] Build(object entity)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(entity);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr intPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(entity, intPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(intPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(intPtr);
            //返回byte数组
            return bytes;
        }

        /// <summary>
        /// 直接将 Struct 或是 Class 映射到指定 byte[] 里, 但 Struct 和 Class 必须引用 StructLayoutAttribute 特性, 并显示的申明 Field
        /// </summary>
        /// <param name="entity">映射的实例</param>
        /// <param name="data">现有的bytes数据</param>
        /// <param name="index">数据的起点位置</param>
        /// <returns></returns>
        public static void Build(object entity, ref byte[] data, int index)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(entity);
            //分配结构体大小的内存空间
            IntPtr intPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(entity, intPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(intPtr, data, index, size);
            //释放内存空间
            Marshal.FreeHGlobal(intPtr);
        }
    }
}
