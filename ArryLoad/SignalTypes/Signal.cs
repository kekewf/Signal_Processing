using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArryLoad.SignalTypes
{

    public class WaveSignal
    {
        #region "Wave 文件头"
        /// <summary>
        /// wav音频头部信息
        /// </summary>
        public struct WaveHeader
        {
            #region "RiffChunk"
            /// <summary>
            /// RIFF标志
            /// </summary>
            public string RIFF;
            /// <summary>
            /// 文件长度
            /// </summary>
            public uint FileSize;
            /// <summary>
            /// WAVE标志
            /// </summary>
            #endregion
            public string WAVE;
            #region "FormatChunk"
            /// <summary>
            /// FORMAT标志
            /// </summary>
            public string FORMAT;
            /// <summary>
            /// Format长度
            /// </summary>
            public uint FormatSize;
            /// <summary>
            /// 编码方式
            /// </summary>
            public ushort FilePadding;
            /// <summary>
            /// 声道数目
            /// </summary>
            public ushort FormatChannels;
            /// <summary>
            /// 采样频率
            /// </summary>
            public uint SamplesPerSecond;
            /// <summary>
            /// 每秒所需字节数
            /// </summary>
            public uint AverageBytesPerSecond;
            /// <summary>
            /// 数据块对齐单位
            /// </summary>
            public ushort BytesPerSample;
            /// <summary>
            /// 单个采样所需Bit数
            /// </summary>
            public ushort BitsPerSample;
            /// <summary>
            /// 附加信息
            /// </summary>
            #endregion
            public ushort FormatExtra;
            #region "FactChunk"
            /// <summary>
            /// FACT标志
            /// </summary>
            public string FACT;
            /// <summary>
            /// Fact长度
            /// </summary>
            public uint FactSize;
            /// <summary>
            /// Fact信息
            /// </summary>
            #endregion
            public uint FactInf;
            #region "DataChunk"
            /// <summary>
            /// DATA标志
            /// </summary>
            public string DATA;
            /// <summary>
            /// Data长度
            /// </summary>
            #endregion
            public uint DataSize;
        }

        //  C#-WaveHeader
        #endregion


        #region "读Wave文件"

        /// <summary>
        /// 返回指定字节数组包含的Wave头部信息
        /// </summary>
        public WaveHeader GetWaveHeaderFromBytes(byte[] data)
        {
            WaveHeader header = new WaveHeader();
            ushort tempIndex = 0;
            header.RIFF = Convert.ToString(System.Text.Encoding.ASCII.GetChars(data, 0, 4));
            header.FileSize = System.BitConverter.ToUInt32(data, 4);
            header.WAVE = Convert.ToString(System.Text.Encoding.ASCII.GetChars(data, 8, 4));
            //FormatChunk
            header.FORMAT = Convert.ToString(System.Text.Encoding.ASCII.GetChars(data, 12, 4));
            header.FormatSize = System.BitConverter.ToUInt32(data, 16);
            header.FilePadding = System.BitConverter.ToUInt16(data, 20);
            header.FormatChannels = System.BitConverter.ToUInt16(data, 22);
            header.SamplesPerSecond = System.BitConverter.ToUInt32(data, 24);
            header.AverageBytesPerSecond = System.BitConverter.ToUInt32(data, 28);
            header.BytesPerSample = System.BitConverter.ToUInt16(data, 32);
            header.BitsPerSample = System.BitConverter.ToUInt16(data, 34);
            if (header.FormatSize == 18)
            {
                header.FormatExtra = System.BitConverter.ToUInt16(data, 36);
            }
            else
            {
                header.FormatExtra = 0;
            }
            tempIndex = (ushort)(20 + header.FormatSize);
            //FactChunk
            header.FACT = Convert.ToString(System.Text.Encoding.ASCII.GetChars(data, tempIndex, 4));
            if (header.FACT == "fact")
            {
                header.FactSize = System.BitConverter.ToUInt32(data, tempIndex + 4);
                header.FactInf = (header.FactSize == 2 ? System.BitConverter.ToUInt16(data, tempIndex + 8) : System.BitConverter.ToUInt32(data, tempIndex + 8));
                tempIndex = (ushort)(tempIndex + header.FactSize + 8);
            }
            else
            {
                header.FACT = "NULL";
                header.FactSize = 0;
                header.FactInf = 0;
            }
            //DataChunk
            header.DATA = Convert.ToString(System.Text.Encoding.ASCII.GetChars(data, tempIndex, 4));
            header.DataSize = System.BitConverter.ToUInt32(data, tempIndex + 4);
            return header;
        }

        //  C#-GetWaveHeaderFromBytes



        #endregion 

        #region "写Wave用的 文件头"
        /// <summary>
        /// 创建WAV音频文件头信息
        /// </summary>
        /// <param name="data_Len">音频数据长度</param>
        /// <param name="data_SoundCH">音频声道数</param>
        /// <param name="data_Sample">采样率，常见有：11025、22050、44100、48000、96000、19200等</param>
        /// <param name="data_SamplingBits">采样位数，常见有：4、8、12、16、24、32</param>
        /// <returns></returns>
        public static byte[] CreateWaveFileHeader(int data_Len, int data_SoundCH, int data_Sample, int data_SamplingBits)
        {
            // WAV音频文件头信息
            List<byte> WAV_HeaderInfo = new List<byte>();  // 长度应该是44个字节
            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("RIFF"));           // 4个字节：固定格式，“RIFF”对应的ASCII码，表明这个文件是有效的 "资源互换文件格式（Resources lnterchange File Format）"
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(data_Len + 44 - 8));  // 4个字节：总长度-8字节，表明从此后面所有的数据长度，小端模式存储数据
            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("WAVE"));           // 4个字节：固定格式，“WAVE”对应的ASCII码，表明这个文件的格式是WAV
            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("fmt "));           // 4个字节：固定格式，“fmt ”(有一个空格)对应的ASCII码，它是一个格式块标识
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(16));                 // 4个字节：fmt的数据块的长度（如果没有其他附加信息，通常为16），小端模式存储数据
            var fmt_Struct = new  //音频数据格式
            {
                PCM_Code = (short)1,                  // 4B，编码格式代码：常见WAV文件采用PCM脉冲编码调制格式，通常为1。
                SoundChannel = (short)data_SoundCH,   // 2B，声道数
                SampleRate = (int)data_Sample,        // 4B，每个通道的采样率：常见有：11025、22050、44100、48000、96000、19200等
                BytesPerSec = (int)(data_SamplingBits * data_Sample * data_SoundCH / 8),  // 4B，数据传输速率 = 声道数×采样频率×每样本的数据位数/8。播放软件利用此值可以估计缓冲区的大小。
                BlockAlign = (short)(data_SamplingBits * data_SoundCH / 8),               // 2B，采样帧大小 = 声道数×每样本的数据位数/8。
                SamplingBits = (short)data_SamplingBits,     // 4B，每个采样值（采样本）的位数，常见有：4、8、12、16、24、32
            };
            // 依次写入fmt数据块的数据（默认长度为16）
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.PCM_Code)); // 4B，编码格式代码：常见WAV文件采用PCM脉冲编码调制格式，通常为1。
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.SoundChannel)); // 2B，声道数
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.SampleRate)); // 4B，每个通道的采样率：常见有：11025、22050、44100、48000、96000、19200等
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.BytesPerSec));// 4B，数据传输速率 = 声道数×采样频率×每样本的数据位数/8。播放软件利用此值可以估计缓冲区的大小。
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.BlockAlign));// 2B，采样帧大小 = 声道数×每样本的数据位数/8。
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.SamplingBits)); // 4B，每个采样值（采样本）的位数，常见有：4、8、12、16、24、32
            /* 还 可以继续写入其他的扩展信息，那么fmt的长度计算要增加。*/

            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("data"));             // 4个字节：固定格式，“data”对应的ASCII码
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(data_Len));             // 4个字节：正式音频数据的长度。数据使用小端模式存放，如果是多声道，则声道数据交替存放。
            /* 到这里文件头信息填写完成，通常情况下共44个字节*/
            return WAV_HeaderInfo.ToArray();
        }

        #endregion

        #region "写Wave用的 文件头"
        /// <summary>
        /// 创建WAV音频文件头信息
        /// </summary>
        /// <param name="data_Len">音频数据长度</param>
        /// <param name="data_SoundCH">音频声道数</param>
        /// <param name="data_Sample">采样率，常见有：11025、22050、44100、48000、96000、19200等</param>
        /// <param name="data_SamplingBits">采样位数，常见有：4、8、12、16、24、32</param>
        /// <returns></returns>
        public static byte[] GenWaveFile(int data_Len, int data_SoundCH, int data_Sample, int data_SamplingBits,byte[] data)
        {
            // WAV音频文件头信息
            List<byte> WAV_HeaderInfo = new List<byte>();  // 长度应该是44个字节
            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("RIFF"));           // 4个字节：固定格式，“RIFF”对应的ASCII码，表明这个文件是有效的 "资源互换文件格式（Resources lnterchange File Format）"
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(data_Len + 44 - 8));  // 4个字节：总长度-8字节，表明从此后面所有的数据长度，小端模式存储数据
            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("WAVE"));           // 4个字节：固定格式，“WAVE”对应的ASCII码，表明这个文件的格式是WAV
            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("fmt "));           // 4个字节：固定格式，“fmt ”(有一个空格)对应的ASCII码，它是一个格式块标识
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(16));                 // 4个字节：fmt的数据块的长度（如果没有其他附加信息，通常为16），小端模式存储数据
            var fmt_Struct = new  //音频数据格式
            {
                PCM_Code = (short)1,                  // 4B，编码格式代码：常见WAV文件采用PCM脉冲编码调制格式，通常为1。
                SoundChannel = (short)data_SoundCH,   // 2B，声道数
                SampleRate = (int)data_Sample,        // 4B，每个通道的采样率：常见有：11025、22050、44100、48000、96000、19200等
                BytesPerSec = (int)(data_SamplingBits * data_Sample * data_SoundCH / 8),  // 4B，数据传输速率 = 声道数×采样频率×每样本的数据位数/8。播放软件利用此值可以估计缓冲区的大小。
                BlockAlign = (short)(data_SamplingBits * data_SoundCH / 8),               // 2B，采样帧大小 = 声道数×每样本的数据位数/8。
                SamplingBits = (short)data_SamplingBits,     // 4B，每个采样值（采样本）的位数，常见有：4、8、12、16、24、32
            };
            // 依次写入fmt数据块的数据（默认长度为16）
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.PCM_Code)); // 4B，编码格式代码：常见WAV文件采用PCM脉冲编码调制格式，通常为1。
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.SoundChannel)); // 2B，声道数
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.SampleRate)); // 4B，每个通道的采样率：常见有：11025、22050、44100、48000、96000、19200等
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.BytesPerSec));// 4B，数据传输速率 = 声道数×采样频率×每样本的数据位数/8。播放软件利用此值可以估计缓冲区的大小。
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.BlockAlign));// 2B，采样帧大小 = 声道数×每样本的数据位数/8。
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.SamplingBits)); // 4B，每个采样值（采样本）的位数，常见有：4、8、12、16、24、32
            /* 还 可以继续写入其他的扩展信息，那么fmt的长度计算要增加。*/

            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("data"));             // 4个字节：固定格式，“data”对应的ASCII码
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(data_Len));             // 4个字节：正式音频数据的长度。数据使用小端模式存放，如果是多声道，则声道数据交替存放。
            /* 到这里文件头信息填写完成，通常情况下共44个字节*/
           
            
            //将数据放入文件头的候补 ，生成全部的文件数据
            List<byte> WAV_Filedata = new List<byte>();
            WAV_Filedata.AddRange(WAV_HeaderInfo);
            WAV_Filedata.AddRange(data);
            return WAV_Filedata.ToArray();
        }
        #endregion
    }
    public class SFR
    {
        public struct SFRcomplex
        {
            public double x;
            public double y;
            public double Re;
            public double Im;
            //public SFRcomplex(double _x);
            //public SFRcomplex(double _x, double _y);
        }

        public struct SignalData
        {
            public double Freq;
            public int SampleRate;
            public double Amplitude;
            public double Phase;
            public double DC;
            public int Samples;//采样点数
            public int Harmonic;//谐波次数
            public double Noise;//噪声
            public double[] SinData;
            public double[] CosData;
            public double[] Data;
            public double[] HarmDist;//Harmonic distotion;
                                     //复数转化为 实部和虚部
                                     //x + iy 是复数  
            public double Re;//Sin 为实部
            public double Im;//Cos 为虚部

            //实部虚部至极坐标转换
            //通过下列等式使矩形坐标元素转换为极坐标元素：
            //x 为实部Rm y为虚部Im theta的结果是弧度
            //r = sqrt(x² + y²)
            //theta = arctan2(y, x) radians

            //复数转极坐标Z   r * e^(i*theta)   
            //z的形式为z = a + bi时，函数通过下列方程转换z = r * e^(i*theta)的极坐标元素：
            // r = |z| = sqrt(a² + b²)
            // theta = arg(z) = arctan2(b, a)
            //
            //极坐标至复数转换，
            ////复数z的形式为z = a + bi时，函数通过下列方程转换极坐标元素：
            //  z = r* cos(theta) + i r*sin(theta)

            //极坐标至实部虚部转换
            // 通过下列等式使极坐标元素转换为直角坐标元素：x 为实部Rm y为虚部Im
            //x = r* cos(theta)
            //y = r* sin(theta)

            public double r;
            public double theta;
        }
        public struct SweepSignalData
        {
            //设置
            public int SampleRate;
            public double DC;
            public int Samples;//采样点数
            public int MinCycleNum;
            public int MinDuration;
            public double MaxDelayTime;
            public double StartFreqency;
            public double StopFreqency;
            public int SweepType;
            public List<String> SweepISO;

            public double[] FreqList;
            public double[] Amplitude;
            public double[] Phase;
            public double[] DataIndexPos;

            //处理结果
            public double DelayPoint;
            public double[] Data;
            public int[] Harmonic;//谐波次数
            public double[] Noise;//噪声
            //public double[] SinData;
            //public double[] CosData;
         //   public double[] HarmDist;//Harmonic distotion;
                                     //复数转化为 实部和虚部
                                     //x + iy 是复数  
            public double[] Re;//Sin 为实部
            public double[] Im;//Cos 为虚部

            public double[] r;
            public double[] theta;
            //实部虚部至极坐标转换
            //通过下列等式使矩形坐标元素转换为极坐标元素：
            //x 为实部Rm y为虚部Im theta的结果是弧度
            //r = sqrt(x² + y²)
            //theta = arctan2(y, x) radians

            //复数转极坐标Z   r * e^(i*theta)   
            //z的形式为z = a + bi时，函数通过下列方程转换z = r * e^(i*theta)的极坐标元素：
            // r = |z| = sqrt(a² + b²)
            // theta = arg(z) = arctan2(b, a)
            //
            //极坐标至复数转换，
            ////复数z的形式为z = a + bi时，函数通过下列方程转换极坐标元素：
            //  z = r* cos(theta) + i r*sin(theta)

            //极坐标至实部虚部转换
            // 通过下列等式使极坐标元素转换为直角坐标元素：x 为实部Rm y为虚部Im
            //x = r* cos(theta)
            //y = r* sin(theta)

        }

    }

    public abstract class Signal
    {


        public double PI {get; set; }//= 3.1415926535897932384626433832795028841971693993751058209749445923078;
        public double A { get; protected set; }
        public double F { get; set; }
        public double Fi { get; set; }
        public double Amplitude { get; protected set; }
        public double Frequency { get; set; }

        public double PhaseDeg { get; set; }

        public int SampleRate { get; set; }

        public double SignalDC { get; set; }

        public Signal(double A, double f, double fi)
        {
            this.A = A;
            this.F = f;
            this.Fi = fi;
            this.Amplitude = A;
            this.Frequency = f;
            this.PhaseDeg = fi*Math.PI/180 ;
          //  this.SampleRate = samplerate;
            this.SignalDC = 0;
        }
        public Signal(double amplitude, double frequency, double deg,double dc,int samplerate)
        {
            this.Amplitude  = amplitude ;
            this.Frequency  = frequency ;
            this.PhaseDeg =deg;
            this.SampleRate = samplerate;
            this.SignalDC = dc;

            this.A = amplitude ;
            this.F = frequency ;
            this.Fi = deg*180/Math.PI ;
        }
       public abstract double GetValue(int x, int N);
       public abstract double GetValue(int x);

    }
}
