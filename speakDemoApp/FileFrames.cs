using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace speakDemoApp
{

    /// <summary>
    /// 消息封包类
    /// </summary>
    public class DataFramePacker
    {
        /*
		 * 消息包注意：
		 * 1.第一位始终是2(ASCII码50)
		 * 2.第二位到第九位是一个long类型的整数，代表消息编号
		 * 3.第十位到第十三位是一个int类型的整数，代表消息内容总长度
		 * 4.第十四位到第十七位是一个int类型的整数，代表分包的总数
		 * 5.第十八位到第二十一位是一个int类型的整数，代表当前的分包编号
		 * 6.第二十二位表示是否需要返回一个确认标识(1/0)
		 * 7.第二十三到第三十一位是保留的(Reserved)
		 * 8.第三十二字节以后是数据包
		 * */

        public static int MAX_UDP_PACKAGE_LENGTH = 1032;
        public static int MAX_UDP_CONTENT_LENGTH = 1024;
        public static int MAX_UDP_HEAD_LENGTH = 8;

        /// <summary>
        ///将原始字节流分帧打包数组
        /// </summary>
        /// <param name="originalData">要打包的消息对象</param>
        /// <returns></returns>
        public static List<Byte[]> BuildPackFrame(Byte[] originalData, byte ty, byte isConfirm)
        {
            List<Byte[]> farmes = new List<byte[]>();
            Byte[] head = new Byte[MAX_UDP_HEAD_LENGTH];

            if (originalData.Length > 0)
            {
                //0 数据类型
                head[0] = ty;
                ushort farmeCount = (ushort)(originalData.Length / MAX_UDP_CONTENT_LENGTH);
                int mod = originalData.Length % MAX_UDP_CONTENT_LENGTH;

                //0-1  帧总数
                byte[] fc = BitConverter.GetBytes((ushort)(farmeCount + (mod > 0 ? 1 : 0)));
                head[1] = fc[1];//高位
                head[2] = fc[0];//低位

                //是否需要确认信息 0否/1是 
                head[7] = isConfirm;
                for (int index = 0; index < farmeCount; index++)
                {
                    Byte[] oneFrame = new Byte[MAX_UDP_PACKAGE_LENGTH];

                    //2-3 帧序号
                    ushort i = (ushort)(index + 1);
                    byte[] ind = BitConverter.GetBytes(i);
                    head[3] = ind[1];//高位
                    head[4] = ind[0];//低位

                    //帧长度 无符号整数2字节
                    ushort length = (ushort)MAX_UDP_PACKAGE_LENGTH;
                    byte[] lengb = BitConverter.GetBytes(length);
                    head[5] = lengb[1];//高位
                    head[6] = lengb[0];//低位

                    //拷贝帧头
                    Array.Copy(head, 0, oneFrame, 0, MAX_UDP_HEAD_LENGTH);
                    //拷贝数据
                    Array.Copy(originalData, index * MAX_UDP_CONTENT_LENGTH, oneFrame, 8, MAX_UDP_CONTENT_LENGTH);
                    //添加一帧
                    farmes.Add(oneFrame);
                }
                if (mod > 0)
                {
                    Byte[] oneFrame = new Byte[MAX_UDP_HEAD_LENGTH + mod];

                    //2-3 帧序号
                    //fc = BitConverter.GetBytes(farmeCount + (mod > 0 ? 1 : 0));
                    head[3] = fc[1];//高位
                    head[4] = fc[0];//低位

                    //帧长度 无符号整数2字节
                    ushort length = (ushort)(MAX_UDP_HEAD_LENGTH + mod);
                    byte[] lengb = BitConverter.GetBytes(length);
                    head[5] = lengb[1];//高位
                    head[6] = lengb[0];//低位

                    //拷贝帧头
                    Array.Copy(head, 0, oneFrame, 0, MAX_UDP_HEAD_LENGTH);
                    //拷贝数据
                    Array.Copy(originalData, farmeCount * MAX_UDP_CONTENT_LENGTH, oneFrame, 8, mod);
                    //添加一帧
                    farmes.Add(oneFrame);
                }
            }
            return farmes;
        }


        public static Byte[] BuildOriginalData(List<Byte[]> farmes)
        {
            int orignalLength = 0;
            if (farmes.Count == 0)
            {
                return null;
            }
            if (farmes[(farmes.Count - 1)].Length < MAX_UDP_PACKAGE_LENGTH)
            {
                orignalLength = (farmes.Count - 1) * MAX_UDP_CONTENT_LENGTH + (farmes[(farmes.Count - 1)].Length - MAX_UDP_HEAD_LENGTH);
            }
            else
            {
                orignalLength = farmes.Count * MAX_UDP_CONTENT_LENGTH;
            }

            Byte[] OriginalData = new Byte[orignalLength];//

            //逐帧向原始数据数组中添加
            foreach (var one in farmes)
            {
                //帧总数
                byte[] dataByte = new byte[2];
                dataByte[0] = one[2];
                dataByte[1] = one[1];
                //将testByte 转回成ushort
                ushort farmeCount = BitConverter.ToUInt16(dataByte, 0);

                //帧序号
                dataByte[0] = one[4];
                dataByte[1] = one[3];
                ushort farmeIndex = BitConverter.ToUInt16(dataByte, 0);

                //帧长度
                dataByte[0] = one[6];
                dataByte[1] = one[5];
                ushort farmeLength = BitConverter.ToUInt16(dataByte, 0);

                Array.Copy(one, 8, OriginalData, (farmeIndex - 1) * MAX_UDP_CONTENT_LENGTH, farmeLength - MAX_UDP_HEAD_LENGTH);

            }

            //返回原始数据
            return OriginalData;
        }

        public static ushort FarmeIndex(Byte[] data)
        {
            byte[] dataByte = new byte[2];
            //帧序号
            dataByte[0] = data[4];
            dataByte[1] = data[3];
            ushort farmeIndex = BitConverter.ToUInt16(dataByte, 0);
            return farmeIndex;
        }

        public static ushort FarmeTotal(Byte[] data)
        {
            byte[] dataByte = new byte[2];
            //帧序号
            dataByte[0] = data[2];
            dataByte[1] = data[1];
            ushort Total = BitConverter.ToUInt16(dataByte, 0);
            return Total;
        }

        public static ushort FarmeLength(Byte[] data)
        {
            byte[] dataByte = new byte[2];
            //帧序号
            dataByte[0] = data[6];
            dataByte[1] = data[5];
            ushort Length = BitConverter.ToUInt16(dataByte, 0);
            return Length;
        }

        public static ushort FarmeDataType(Byte[] data)
        {
            byte[] dataByte = new byte[2];
            //帧序号
            dataByte[1] = data[0];
            ushort type = BitConverter.ToUInt16(dataByte, 0);
            return type;
        }

        public static ushort FarmeSetData(Byte[] data)
        {
            byte[] dataByte = new byte[2];
            //帧序号
            dataByte[1] = data[7];
            ushort set = BitConverter.ToUInt16(dataByte, 0);
            return set;
        }

    }

    public class sendFarmesData
    {
        public IPEndPoint MultCommIpEndPoint;
        public List<IPEndPoint> UnicastReceiver;

        public List<Byte[]> OriginalData;

        public sendFarmesData()
        {
            UnicastReceiver = new List<IPEndPoint>();
        }
    }
}
