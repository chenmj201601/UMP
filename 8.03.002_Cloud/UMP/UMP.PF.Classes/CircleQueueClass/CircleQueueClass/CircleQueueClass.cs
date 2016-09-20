using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CircleQueueClass
{
    public class YoungCircleQueue<T>
    {
        /// <summary>
        /// 队列数组
        /// </summary>
        private T[] ICircleQueue;

        /// <summary>
        /// 队首索引
        /// </summary>
        private int IIntFront;

        /// <summary>
        /// 队尾索引
        /// </summary>
        private int IIntRear;

        /// <summary>
        /// 队列的容量大小，实际可用大小为： IIntCapacity - 1
        /// </summary>
        private int IIntCapacity;

        /// <summary>
        /// 当前队列中存在元素的总数
        /// </summary>
        private int IIntCountElement;

        /// <summary>
        /// 初始化队列大小
        /// </summary>
        /// <param name="AIntQueueSize">
        /// 队列长度，注意：不能小于 1
        /// </param>
        public YoungCircleQueue(int AIntQueueSize)
        {
            IIntCapacity = AIntQueueSize;

            ICircleQueue = new T[AIntQueueSize];

            IIntFront = IIntRear = 0;

            IIntCountElement = 0;
        }

        /// <summary>
        /// 向循环队列中添加一个元素
        /// </summary>
        /// <returns></returns>
        public bool PushElement(T APushItem)
        {
            bool LBoolReturn = true;

            try
            {
                #region 队列已满，将队列数组扩大 1 倍，重新构建队列
                if (GetNextRearIndex() == IIntFront)
                {
                    T[] LTNewQueue = new T[IIntCapacity * 2];

                    //数据容量过大，超出系统内存大小
                    if (LTNewQueue == null) { return false; }

                    //队列索引尚未回绕
                    if (IIntFront == 0)
                    {
                        //将旧队列数组数据转移到新队列数组中
                        Array.Copy(ICircleQueue, LTNewQueue, IIntCapacity);
                    }
                    else
                    {
                        //如果队列回绕，刚需拷贝再次，
                        //第一次将队首至旧队列数组最大长度的数据拷贝到新队列数组中
                        Array.Copy(ICircleQueue, IIntFront, LTNewQueue, IIntFront, IIntCapacity - IIntRear - 1);
                        //第二次将旧队列数组起始位置至队尾的数据拷贝到新队列数组中
                        Array.Copy(ICircleQueue, 0, LTNewQueue, IIntCapacity, IIntRear + 1);
                        //将队尾索引改为新队列数组的索引
                        IIntRear = IIntCapacity + 1;
                    }

                    ICircleQueue = LTNewQueue;
                    IIntCapacity *= 2;
                }
                #endregion

                IIntRear = GetNextRearIndex();
                ICircleQueue[IIntRear] = APushItem;

                IIntCountElement += 1;
            }
            catch
            {
                LBoolReturn = false;
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 弹出一个元素，注意：调用该方法前请检查队列是否为空
        /// </summary>
        /// <returns></returns>
        public T PopElement()
        {
            IIntFront = GetNextFrontIndex();
            IIntCountElement -= 1;
            return ICircleQueue[IIntFront];
        }

        /// <summary>
        /// 队列是否为空
        /// </summary>
        /// <returns></returns>
        public bool CircleQueueIsEmpty()
        {
            return IIntFront == IIntRear;
        }

        /// <summary>
        /// 队列中还存在的元素总数
        /// </summary>
        /// <returns></returns>
        public int CircleQueueElementCount()
        {
            return IIntCountElement;
        }

        /// <summary>
        /// 获取队首元素，注意：调用该方法前请检查队列是否为空
        /// </summary>
        /// <returns></returns>
        public T GetFistrElement()
        {
            return ICircleQueue[GetNextFrontIndex()];
        }

        /// <summary>
        /// 获取队尾元素,注意：调用该方法前请检查队列是否为空
        /// </summary>
        /// <returns></returns>
        public T GetEndElement()
        {
            return ICircleQueue[IIntRear];
        }

        /// <summary>
        /// 获取下一个索引
        /// </summary>
        /// <returns></returns>
        private int GetNextRearIndex()
        {
            if (IIntRear + 1 == IIntCapacity) { return 0; }
            return IIntRear + 1;
        }

        /// <summary>
        /// 获取下一个索引
        /// </summary>
        /// <returns></returns>
        private int GetNextFrontIndex()
        {
            if (IIntFront + 1 == IIntCapacity) { return 0; }
            return IIntFront + 1;
        }
    }
}
