using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace speakDemoApp
{
    public class WaveInfo
    {
        /// <summary>
        /// 数据流
        /// </summary>
        private FileStream m_WaveData;

        private bool m_WaveBool = false;

        private RIFF_WAVE_Chunk _Header = new RIFF_WAVE_Chunk();
        private Format_Chunk _Format = new Format_Chunk();
        private Fact_Chunk _Fact = new Fact_Chunk();
        private Data_Chunk _Data = new Data_Chunk();
        public WaveInfo(string WaveFileName)
        {
            m_WaveData = new FileStream(WaveFileName, FileMode.Open);
            try
            {
                LoadWave();
                m_WaveData.Close();
            }
            catch
            {
                m_WaveData.Close();
            }
        }
        private void LoadWave()
        {
            #region RIFF_WAVE_Chunk
            byte[] _Temp4 = new byte[4];
            byte[] _Temp2 = new byte[2];
            m_WaveData.Read(_Temp4, 0, 4);
            if (_Temp4[0] != _Header.szRiffID[0] || _Temp4[1] != _Header.szRiffID[1] || _Temp4[2] != _Header.szRiffID[2] || _Temp4[3] != _Header.szRiffID[3]) return;
            m_WaveData.Read(_Temp4, 0, 4);
            _Header.dwRiffSize = BitConverter.ToUInt32(_Temp4, 0);
            m_WaveData.Read(_Temp4, 0, 4);
            if (_Temp4[0] != _Header.szRiffFormat[0] || _Temp4[1] != _Header.szRiffFormat[1] || _Temp4[2] != _Header.szRiffFormat[2] || _Temp4[3] != _Header.szRiffFormat[3]) return;

            #endregion
            #region Format_Chunk
            m_WaveData.Read(_Temp4, 0, 4);
            if (_Temp4[0] != _Format.ID[0] || _Temp4[1] != _Format.ID[1] || _Temp4[2] != _Format.ID[2]) return;
            m_WaveData.Read(_Temp4, 0, 4);
            _Format.Size = BitConverter.ToUInt32(_Temp4, 0);
            long _EndWave = _Format.Size + m_WaveData.Position;
            m_WaveData.Read(_Temp2, 0, 2);
            _Format.FormatTag = BitConverter.ToUInt16(_Temp2, 0);
            m_WaveData.Read(_Temp2, 0, 2);
            _Format.Channels = BitConverter.ToUInt16(_Temp2, 0);
            m_WaveData.Read(_Temp4, 0, 4);
            _Format.SamlesPerSec = BitConverter.ToUInt32(_Temp4, 0);
            m_WaveData.Read(_Temp4, 0, 4);
            _Format.AvgBytesPerSec = BitConverter.ToUInt32(_Temp4, 0);
            m_WaveData.Read(_Temp2, 0, 2);
            _Format.BlockAlign = BitConverter.ToUInt16(_Temp2, 0);
            m_WaveData.Read(_Temp2, 0, 2);
            _Format.BitsPerSample = BitConverter.ToUInt16(_Temp2, 0);
            m_WaveData.Position += _EndWave - m_WaveData.Position;
            #endregion
            m_WaveData.Read(_Temp4, 0, 4);
            if (_Temp4[0] == _Fact.ID[0] && _Temp4[1] == _Fact.ID[1] && _Temp4[2] == _Fact.ID[2] && _Temp4[3] == _Fact.ID[3])
            {
                #region  Fact_Chunk
                m_WaveData.Read(_Temp4, 0, 4);
                _Fact.Size = BitConverter.ToUInt32(_Temp4, 0);
                m_WaveData.Position += _Fact.Size;
                #endregion
                m_WaveData.Read(_Temp4, 0, 4);
            }
            if (_Temp4[0] == _Data.ID[0] && _Temp4[1] == _Data.ID[1] && _Temp4[2] == _Data.ID[2] && _Temp4[3] == _Data.ID[3])
            {
                #region Data_Chunk
                m_WaveData.Read(_Temp4, 0, 4);
                _Data.Size = BitConverter.ToUInt32(_Temp4, 0);
                _Data.FileBeginIndex = m_WaveData.Position;
                _Data.FileOverIndex = m_WaveData.Position + _Data.Size;
                m_Second = (double)_Data.Size / (double)_Format.AvgBytesPerSec;
                #endregion
            }

            m_WaveBool = true;
        }
        #region 文件定义
        /// <summary>
        /// 文件头
        /// </summary>
        private class RIFF_WAVE_Chunk
        {
            /// <summary>
            /// 文件前四个字节 为RIFF
            /// </summary>
            public byte[] szRiffID = new byte[] { 0x52, 0x49, 0x46, 0x46 };   // 'R','I','F','F'
            /// <summary>
            /// 数据大小 这个数字等于+8 =文件大小
            /// </summary>
            public uint dwRiffSize = 0;
            /// <summary>
            ///WAVE文件定义 为WAVE
            /// </summary>
            public byte[] szRiffFormat = new byte[] { 0x57, 0x41, 0x56, 0x45 }; // 'W','A','V','E'         
        }
        /// <summary>
        /// 声音内容定义
        /// </summary>
        private class Format_Chunk
        {
            /// <summary>
            /// 固定为  是"fmt "字后一位为0x20
            /// </summary>
            public byte[] ID = new byte[] { 0x66, 0x6D, 0x74, 0x20 };
            /// <summary>
            /// 区域大小
            /// </summary>
            public uint Size = 0;
            /// <summary>
            /// 记录着此声音的格式代号，例如1-WAVE_FORMAT_PCM， 2-WAVE_F0RAM_ADPCM等等。 
            /// </summary>
            public ushort FormatTag = 1;
            /// <summary>
            /// 声道数目，1--单声道；2--双声道
            /// </summary>
            public ushort Channels = 2;
            /// <summary>
            /// 采样频率  一般有11025Hz（11kHz）、22050Hz（22kHz）和44100Hz（44kHz）三种
            /// </summary>
            public uint SamlesPerSec = 0;
            /// <summary>
            /// 每秒所需字节数
            /// </summary>
            public uint AvgBytesPerSec = 0;
            /// <summary>
            /// 数据块对齐单位(每个采样需要的字节数)
            /// </summary>
            public ushort BlockAlign = 0;
            /// <summary>
            /// 音频采样大小 
            /// </summary>
            public ushort BitsPerSample = 0;
            /// <summary>
            /// ???
            /// </summary>
            public byte[] Temp = new byte[2];
        }
        /// <summary>
        /// FACT
        /// </summary>
        private class Fact_Chunk
        {
            /// <summary>
            /// 文件前四个字节 为fact
            /// </summary>
            public byte[] ID = new byte[] { 0x66, 0x61, 0x63, 0x74 };   // 'f','a','c','t'
            /// <summary>
            /// 数据大小
            /// </summary>
            public uint Size = 0;
            /// <summary>
            /// 临时数据
            /// </summary>
            public byte[] Temp;
        }
        /// <summary>
        /// 数据区
        /// </summary>
        private class Data_Chunk
        {
            /// <summary>
            /// 文件前四个字节 为RIFF
            /// </summary>
            public byte[] ID = new byte[] { 0x64, 0x61, 0x74, 0x61 };   // 'd','a','t','a'
            /// <summary>
            /// 大小
            /// </summary>
            public uint Size = 0;
            /// <summary>
            /// 开始播放的位置
            /// </summary>
            public long FileBeginIndex = 0;
            /// <summary>
            /// 结束播放的位置
            /// </summary>
            public long FileOverIndex = 0;
        }
        #endregion
        #region 属性
        /// <summary>
        /// 是否成功打开文件
        /// </summary>
        public bool WaveBool { get { return m_WaveBool; } }
        private double m_Second = 0;
        /// <summary>
        /// 秒单位
        /// </summary>
        public double Second { get { return m_Second; } }
        /// <summary>
        /// 格式
        /// </summary>
        public string FormatTag
        {
            get
            {
                switch (_Format.FormatTag)
                {
                    case 1:
                        return "PCM";
                    case 2:
                        return "Microsoft ADPCM";
                    default:
                        return "Un";
                }
            }
        }
        /// <summary>
        /// 频道
        /// </summary>
        public ushort Channels { get { return _Format.Channels; } }
        /// <summary>
        /// 采样级别
        /// </summary>
        public string SamlesPerSec
        {
            get
            {
                switch (_Format.SamlesPerSec)
                {
                    case 11025:
                        return "11kHz";
                    case 22050:
                        return "22kHz";
                    case 44100:
                        return "44kHz";
                    default:
                        return _Format.SamlesPerSec.ToString() + "Hz";
                }
            }
        }
        /// <summary>
        /// 采样大小
        /// </summary>
        public ushort BitsPerSample { get { return _Format.BitsPerSample; } }
        #endregion
    }
}