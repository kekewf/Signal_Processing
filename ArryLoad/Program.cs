using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO ;
using System.Diagnostics;
using  ArryLoad.SignalTypes;
//using NAudio;
//using static     ArryLoad.SignalTypes.AnalyseAudio;
//using NAudio.Wave;


//public  struct ST_AUDIO_DEVICE_INFO_LIST
//{
//    int ID;
//    char[] name=new char[1024];
//    int inputChannels;
//    int outputChannels;
//};

//public   ST_AUDIO_DEVICE_INFO_LIST  arrList; 
namespace ArryLoad
{
    //public partial class Form1 : Form
    //{
    //    public Form1()
    //    {
    //        InitializeComponent();
    //        //start with a not so high amplitude of 10 (because the sound may hurt your ears) 
    //        Beep.BeepChirpLinearHold(100, 100, 7100, 2000);
    //        Beep.BeepChirpLinear(100, 100, 7100, 2000);
    //        Beep.BeepBeepBeep(100, 7100, 2000);
    //        Beep.BeepChirpExpHold(100, 100, 5000, 2000, 0.00001, true);
    //        Beep.BeepChirpExp(100, 100, 5000, 2000, 0.00001, true);
    //        Beep.BeepBeepBeep(100, 5000, 2000);
    //    }
    //}

    class Program
    {
        //NAudioRecorder recorder;
        //NAudioReader aggregator;


        static void Main(string[] args)
        {
            //main

            //===================================
            //========    一维数组传递
            double[] test = { 1, 2, 3 };
            //   DLLImportHelper.test_double_group(test,3);

            //DLLImportHelper.test_double_group(test,3);
            //for (int i = 0; i < test.Count(); i++)
            //{
            //     MessageBox.Show(test[i].ToString());
            //        Console.WriteLine(test[i]); //printf("%d", test[i]);
            //   }

            //      Console.Read();
            //=========================================
            //========    二维数组传递
            //double*[]为指针数组，存放二维数组表格数据类型的每一行row的首元素的地址；
            //辅助函数定义
            // int r = 5;
            // int c = 4;
            // double[,] matrix = new double[r,c];

            // for (int i = 0; i < matrix.GetUpperBound(0) + 1; i++)
            // {
            //     for (int j = 0; j < matrix.GetUpperBound(1) + 1; j++)
            //     {
            //         matrix[i, j] = (i + 1) * (j + 1);
            //         //   Console.WriteLine(matrix[i, j]); //   MessageBox.Show(matrix[99, 99].ToString());
            //     }

            //     //   Console.WriteLine("========"); //   MessageBox.Show(matrix[99, 99].ToString());
            // }
            // DLLImportHelper.Assist_test_double_2groups(matrix, r, c);
            // for (int i = 0; i < matrix.GetUpperBound(0) + 1; i++)
            // {
            //     for (int j = 0; j < matrix.GetUpperBound(1) + 1; j++)
            //     {
            //         Console.WriteLine(matrix[i, j]); //   MessageBox.Show(matrix[99, 99].ToString());
            //     }

            //     Console.WriteLine("========"); //   MessageBox.Show(matrix[99, 99].ToString());
            // }

            //// string rightS = "";// str.Substring(str.Length - 1);
            // int filens = requestMethod(@"D:\WindowFunction.h");
            //            String[] datains = new String[filens];

            //            StreamReader src = new StreamReader(@"D:\WindowFunction.h");
            // int lis = 0;
            // while (!src.EndOfStream)
            // {
            //     String str = src.ReadLine();
            //     if (str != "")
            //     {
            //         datains[lis] = str;
            //     }
            //     lis++;
            // }
            // src.Close();
            // String phs = "";
            // for (int i = 0; i < datains.Length; i++)
            // {
            //     phs = datains[i].ToString();
            //     phs=phs.Substring(3, phs.Length - 3);
            //     phs = SavaProcess(@"WindowFunction.txt", phs);//"corr.csv", s.ToString  str.Substring(str.Length - 1)
            //     //if (i<inlen)
            //     // ph = SavaProcess(@"datain.txt", datain[i].ToString());//"corr.csv", s.ToString
            // }
            // //int inlen = filens;

            // Console.WriteLine("===OK====="); 

            #region 读取文本文件
            int filen = requestMethod(@"D:\BeepChirpExp.txt");
            //if (filen % 2 > 0)
            //    filen = filen + 1;
            //if (filen > 48000)
            //{
            //    filen = 48000;
            //}
            double[] datain = new double[filen];

            StreamReader sr = new StreamReader(@"D:\BeepChirpExp.txt");
            int li = 0;
            while (!sr.EndOfStream)
            {
                string str = sr.ReadLine();
                //if (li < 48000)
                //{
                if (str != "")
                {
                    datain[li] = Convert.ToDouble(str);
                }

                //   }
                //   txt += str + "\n";
                li++;
                //     Console.WriteLine(str);
            }

            if (li < filen)
            {
                for (int i = li; i < filen; i++)

                    datain[i] = 0;
            }

            sr.Close();

            int inlen = filen;

            Console.WriteLine("===R0=====");
            filen = requestMethod(@"D:\BeepChirpExp - Ref.txt");
            //if (filen % 2 > 0)
            //    filen = filen + 1;

            //if (filen > 24000)
            //{ filen = 24000; }

            double[] dataso = new double[filen];


            sr = new StreamReader(@"D:\BeepChirpExp - Ref.txt");
            li = 0;
            while (!sr.EndOfStream)
            {
                string str = sr.ReadLine();
                //if (li<24000)
                //{
                if (str != "")
                {
                    dataso[li] = Convert.ToDouble(str);
                }

                // }
                //   txt += str + "\n";
                li++;
                //         Console.WriteLine(str);
            }

            if (li < filen)
            {
                for (int i = li; i < filen; i++)

                    dataso[i] = 0;
            }
            int outlen = filen;

            sr.Close();
            Console.WriteLine("===RS=====");

            //for (int i = 0; i < dataso.Length; i++)
            //{
            //    String phh = SavaProcess(@"RShanning.txt", dataso[i].ToString());//"corr.csv", s.ToString

            //}

            #endregion


            #region  "计算获取相对位置，截取需要的数据"
            int uci = 0;
            double[] corr;
            double[] ProceData; 
            uci = AnalyseAudio.Xcorrr1d(datain, dataso,out corr,out ProceData );
            Console.WriteLine("===RS=====" + uci.ToString() );
            // double[] dataTemp = new double[inlen - uci];
            //for (int i = 0; i < inlen - uci; i++)
            //{
            //    dataTemp[i] = datain[i + uci];
            //}
            datain = new double[inlen - uci];
            for (int i = 0; i < inlen - uci; i++)
            {
                datain[i] = ProceData[i];
            }

            #endregion
            #region  "计算结果"
            int SampleRate = 48000;//数据的采样率
            double freq = 1000;
            SFR.SignalData signaldata=new SFR.SignalData();
            signaldata.Freq = freq ;
            signaldata.Data = datain;
            signaldata.SampleRate = SampleRate;
            double PI = 3.141592653589793238462643383279502884197169399;
            //PI = 3.1415926535897932384626433832795028841971693993751058209749445923078;
            //可以用两种输入方式,计算结果，第一种直接计算一个频率，第二种计算多个频率更方便方便
            //  SFR.SFRcomplex resultdata = SFR_DFT(datain, freq, SampleRate);
            SFR.SignalData result = AnalyseAudio.SFR_DFT(signaldata);

            Console.WriteLine("===FFT data =====" + "Amp: " + result.Amplitude.ToString("0.0000000000000000000000000"));
            Console.WriteLine("===FFT data =====" + "Phase: " + result.Phase.ToString("###0.00000000000000"));
            //  phdata = fft_phase(fdata,fr);
            //writefile(ampdata, 10000, df);
            //writefile(phdata, 10001, df);
            // Console.WriteLine("===FFT data Write File=====" + "Amp: " + result.Amplitude.ToString());

            #endregion
            //     Beep.BeepChirpLinearHold(100, 10, 7100, 2000);
            Single  fstart = 20;
            double fstop =10000;
            double amplite = 500;//最大为1000
            double Duration = 500;//中心频率
           // Beep.BeepChirpLinear(SampleRate,amplite , fstart, fstop, Duration);
         //   Beep.BeepBeepBeep(100, 7100, 2000);
          //  Beep.BeepChirpExpHold(100, 10, 7100, 2000, 0.00001, true);
            Beep.BeepChirpExp(SampleRate,50, amplite, fstart, fstop, Duration, 0.00001, true,16);// 0.00001
                                                                                           //   Beep.BeepBeepBeep(100, 5000, 2000);
                                                                                           //Console.WriteLine(" ===== END ===== ");
                                                                                           //Console.Read();


            Console.WriteLine(" ===== END ===== ");
            Console.Read();

           String   audioFile = @"D:\BeepChirpExp.wav";






    }


        public static int requestMethod(String _fileName)
        //读取txt文件中总行数的方法
        {
            Stopwatch sw = new Stopwatch();
            var path = _fileName;
            int lines = 0;

            //按行读取
            sw.Restart();
            using (var sr = new StreamReader(path))
            {
                var ls = "";
                while ((ls = sr.ReadLine()) != null)
                {
                    lines++;
                }
            }
            sw.Stop();
            return lines;
        }

        public static void writefile(double[] data, int n, double df)
        {
            String ph = "";
            for (int i = 0; i < data.Length; i++)
            {
                string wstr = (i * df).ToString() + "," + data[i].ToString();
                ph = SavaProcess(@"corr" + n.ToString() + ".txt", wstr);//"corr.csv", s.ToString
                                                                        //if (i<inlen)
                                                                        // ph = SavaProcess(@"datain.txt", datain[i].ToString());//"corr.csv", s.ToString
            }
            Console.WriteLine("===Wirte data " + n.ToString() + "OK=====");

        }

        public static String SavaProcess(String FileName, String data)
        {
            //System.DateTime currentTime = System.DateTime.Now;
            ////获取当前日期的前一天转换成ToFileTime
            //string strYMD = currentTime.AddDays(-1).ToString("yyyyMMdd");
            ////按照日期建立一个文件名
            //string FileName = "MyFileSend" + strYMD + ".txt";
            ////设置目录
            string CurDir = System.AppDomain.CurrentDomain.BaseDirectory;//+ @"SaveDir"
                                                                         ////判断路径是否存在
                                                                         //if (!System.IO.Directory.Exists(CurDir))
                                                                         //{
                                                                         //    System.IO.Directory.CreateDirectory(CurDir);
                                                                         //}
                                                                         //不存在就创建
            String FilePath = CurDir + FileName;
            //文件追加方式添加内容
            System.IO.StreamWriter file = new System.IO.StreamWriter(FilePath, true);
            //保存数据到文件
            file.WriteLine(data);
            //关闭文件
            file.Close();
            //释放对象
            file.Dispose();
            return FilePath;
        }


    }
  


    public class Beep
    {
        //testmethod that holds the maxFreq
        //original by JohnWein: http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/18fe83f0-5658-4bcf-bafc-2e02e187eb80
       
        public static String SavaProcess(String FileName, String data)
        {
            //System.DateTime currentTime = System.DateTime.Now;
            ////获取当前日期的前一天转换成ToFileTime
            //string strYMD = currentTime.AddDays(-1).ToString("yyyyMMdd");
            ////按照日期建立一个文件名
            //string FileName = "MyFileSend" + strYMD + ".txt";
            ////设置目录
            string CurDir = System.AppDomain.CurrentDomain.BaseDirectory;//+ @"SaveDir"
                                                                         ////判断路径是否存在
                                                                         //if (!System.IO.Directory.Exists(CurDir))
                                                                         //{
                                                                         //    System.IO.Directory.CreateDirectory(CurDir);
                                                                         //}
                                                                         //不存在就创建
            String FilePath = CurDir + FileName;
            //文件追加方式添加内容
            System.IO.StreamWriter file = new System.IO.StreamWriter(FilePath, true);
            //保存数据到文件
            file.WriteLine(data);
            //关闭文件
            file.Close();
            //释放对象
            file.Dispose();
            return FilePath;
        }


        //original by JohnWein: http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/18fe83f0-5658-4bcf-bafc-2e02e187eb80
        public static void BeepChirpLinear(int SampleRate,double  Amplitude, double   startFrequency, double  maxFrequency, double  Duration)
        {
            double A = ((Amplitude * (System.Math.Pow(2, 15))) / 1000) - 1;
            double frq = startFrequency;

            int Samples =(int) (SampleRate * Duration) / 1000;
            double frqDiff = ((double)maxFrequency - startFrequency) / Samples / 2;
            int Bytes = Samples * 4;
            //                       //定义写文件流
            string path = @"D:\BeepChirpLinear.wav";
        //    FileStream fsw = new FileStream(path, FileMode.OpenOrCreate);
            //            14             //写入的内容
            //15             string inputStr = "Learn Advanced C Sharp";
            //            16             //字符串转byte[]
            //17             byte[] writeBytes = Encoding.UTF8.GetBytes(inputStr);
            //            18             //写入
            //19             fsw.Write(writeBytes, 0, writeBytes.Length);
            //            20             //关闭文件流
            //21             fsw.Close();
            short[] wavedata = new short [Samples*2];
            int[] Hdr = { 0X46464952, 36 + Bytes, 0X45564157, 0X20746D66, 16, 0X20001, SampleRate, SampleRate*4, 0X100004, 0X61746164, Bytes };

            for (int i = 0; i < Hdr.Length; i++)
                wavedata[i] =(byte) Hdr[i];
            int hdrl = Hdr.Length;
            using (MemoryStream MS = new MemoryStream(36 + Bytes))
            {
                using (BinaryWriter BW = new BinaryWriter(MS))
                {
                    short Sample=0;
                    double tt = 0;
                    for (int I = 0; I < Hdr.Length; I++)
                    {
                        BW.Write(Hdr[I]);
                    }
                    for (int T = 0; T < Samples; T++)
                    {
                        //frq += frqDiff;
                        //double DeltaFT = 2 * Math.PI * frq / 44100.0;
                        //short Sample = System.Convert.ToInt16(A * Math.Sin(DeltaFT * T));

                        //wiki ft + kt
                        tt = T /(double) SampleRate;
                        Sample = System.Convert.ToInt16(Math.Sin(2 * Math.PI * (startFrequency + (frqDiff * T)) * tt) * A);

                        BW.Write(Sample);
                        BW.Write(Sample);
                        //wavedata[T] = Sample;
                        //wavedata[T+1] = Sample;
                        SavaProcess(@"BeepChirpLinear.txt", Sample.ToString());
                    }
                    for (int T = 0; T < Samples; T += 2)
                    {
                        //frq += frqDiff;
                        //double DeltaFT = 2 * Math.PI * frq / 44100.0;
                        //short Sample = System.Convert.ToInt16(A * Math.Sin(DeltaFT * T));

                        //wiki ft + kt
                        tt = T / (double)SampleRate;
                        Sample = System.Convert.ToInt16(Math.Sin(2 * Math.PI * (startFrequency + (frqDiff * T)) * tt) * A);

                        wavedata[T] =Sample;

                        tt = (T+1) / (double)SampleRate;
                        Sample = System.Convert.ToInt16(Math.Sin(2 * Math.PI * (startFrequency + (frqDiff * T)) * tt) * A);
                        wavedata[T + 1] = Sample;
                    }
                    BW.Flush();
                    MS.Seek(0, SeekOrigin.Begin);
                    //using (SoundPlayer SP = new SoundPlayer(MS))
                    //{
                    //    SP.PlaySync();
                    //}
           //     fsw.Write(wavedata, 0,wavedata.Length  );
                }
            }
        }

        //original by JohnWein: http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/18fe83f0-5658-4bcf-bafc-2e02e187eb80
        public static void BeepChirpExp(int SampleRate, int TotalTime ,double  Amplitude, double  startFrequency, double  maxFrequency, double  Duration, double expGrowth, bool autoCorrect,int SamplingBits)
        {   
            int SamplingBitsbyte = SamplingBits / 8;  //计算音频中一个数据需要几个字节
            double A = ((Amplitude * (System.Math.Pow(2, (SamplingBits-1)))) / 1000) - 1;//计算幅度最大值

            decimal tempnum = SampleRate * TotalTime/1000;//时间单位是毫秒    需要生成多少数据              
            int Samples =(int) Math.Ceiling(tempnum);// (int) (SampleRate * Duration) / 100000;

         //   int Bytes = Samples * 4;
          //  int[] Hdr = { 0X46464952, 36 + Bytes, 0X45564157, 0X20746D66, 16, 0X20001, SampleRate, SampleRate*4, 0X100004, 0X61746164, Bytes };
            // WAV音频文件头信息
            List<byte> WAV_Filedata = new List<byte>();  // 长度应该是44个字节
            WAV_Filedata.AddRange(WaveSignal.CreateWaveFileHeader(Samples* SamplingBitsbyte*2, 2, SampleRate, SamplingBits));//Samples*4 是因为2个byte 是16位，并且是 2个通道的信号

            if (autoCorrect)
            {
                double oldExp = expGrowth;
                expGrowth = (Math.Pow((double)maxFrequency / startFrequency, 1.0 / Samples) - 1.0);
                //MessageBox.Show((startFrequency * Math.Pow(1.0 + expGrowth, Samples)).ToString());
              
                //if (expGrowth < oldExp)//2021-11-26
                //    MessageBox.Show(expGrowth.ToString() + "\nOldVal: " + oldExp.ToString() + "\nDiff: " + (oldExp - expGrowth).ToString());
            }

            //using (MemoryStream MS = new MemoryStream(36 + Bytes))
            //{
            //    using (BinaryWriter BW = new BinaryWriter(MS))
            //    {

                    byte[] wavedata = new byte[Samples*2];//2通道
                    double tt = 0;
                    short Sample = 0;
            //for (int I = 0; I < Hdr.Length; I++)                  
            //{
            //    BW.Write(Hdr[I]);
            //}
            for (int T = 0; T < Samples; T++)
            {
                tt = T / (double)SampleRate;
                double expGrowthVal = (Math.Pow(1.0 + expGrowth, T) - 1.0) / Math.Log(Math.Pow(1.0 + expGrowth, T));

                //wiki f0*k^t
                Sample = (short)(Math.Sin(2 * Math.PI * startFrequency * expGrowthVal * tt) * A);

                //BW.Write(Sample);
                //BW.Write(Sample);

                SavaProcess(@"BeepChirpExp.txt", Sample.ToString());
            }
                    int i = 0;
            byte[] TempData;
            for (int T = 0; T < Samples; T++)
            {
                tt = T / (double)SampleRate;
                double expGrowthVal = (Math.Pow(1.0 + expGrowth, T) - 1.0) / Math.Log(Math.Pow(1.0 + expGrowth, T));
                Sample = (short)(Math.Sin(2 * Math.PI * startFrequency * expGrowthVal * tt) * A);// System.Convert.ToInt16((Math.Sin(2 * Math.PI * startFrequency * expGrowthVal * tt) * A));

               TempData= BitConverter.GetBytes(Sample);//wavedata[T] = BitConverter.GetBytes(Sample)[0];
                WAV_Filedata.AddRange(TempData);
                WAV_Filedata.AddRange(TempData);// WAV_Filedata.AddRange(BitConverter.GetBytes(Sample));//wavedata[T + 1] = BitConverter.GetBytes(Sample)[0];
                //byte[] da = new byte[32];
                //if (T > 4400)
                //    da = BitConverter.GetBytes(Sample);
                //i += 2;

            }
              //       WAV_Filedata.AddRange(wavedata);


            string path = @"D:\BeepChirpExp.wav";
             FileStream fsw = new FileStream(path, FileMode.OpenOrCreate);
            fsw.Write(WAV_Filedata.ToArray(), 0, WAV_Filedata.ToArray().Length);
            fsw.Close();
            //BW.Flush();
            //MS.Seek(0, SeekOrigin.Begin);
            //using (SoundPlayer SP = new SoundPlayer(M                                                                                                                                                             S))
            //{
            //    SP.PlaySync();
            //}

            //    }
            //   // byte[] waveAlldata = new byte[Samples];
            //    //WaveSignal.WaveHeader headerdata = new WaveSignal.WaveHeader();
            //    //waveAlldata = WaveSignal.CreateWaveFileHeader(Samples, 2, SampleRate, 16);
            //    //   GenWaveFile(int data_Len, int data_SoundCH, int data_Sample, int data_SamplingBits, byte[] data)



            //}
        }
    }
}
