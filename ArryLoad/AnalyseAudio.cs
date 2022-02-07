using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ArryLoad.SignalTypes
{
    class AnalyseAudio
    {
       
        public static void unwrap(double[] phase, out double[] unwrapped_phase)
        {
            /*
             Unwrap by changing deltas between values to 2 * pi complement.

             Unwrap radian phase phase by changing absolute jumps greater than discont to their 2 * pi complement along the given axis.

             :param phase: input array
             :return unwrapped_phase: output array
             */

            // Maximum discontinuity between values, default is pi.
            double discont = Math.PI;
            double PI = Math.PI;
            unwrapped_phase = new double[phase.Length];
            int length_phase = phase.Length;
            //for (int i=0;i<length_phase;i++)
            //{
            //    unwrapped_phase[i] = 0; 
            //}

            double[] diff_phase = new double[length_phase - 1];
            double[] diff_phase_mod = new double[length_phase - 1];//            vector<double> diff_phase_mod(length_phase -1);
            double[] ph_correct = new double[length_phase - 1];//             vector<double> ph_correct(length_phase -1);
                                                               // 不改数组大小   unwrapped_phase.resize(length_phase);
            unwrapped_phase[0] = phase[0];
            double cum_sum_ph_correct = 0;
            for (int i = 0; i < length_phase - 1; i++)
            {
                diff_phase[i] = phase[i + 1] - phase[i];
                diff_phase_mod[i] = mod(diff_phase[i] + PI, 2 * PI) - PI;
                if (diff_phase_mod[i] == -PI && diff_phase[i] > 0)
                {
                    diff_phase_mod[i] = PI;
                }

                ph_correct[i] = diff_phase_mod[i] - diff_phase[i];
                if (Math.Abs(diff_phase[i]) < discont)
                {
                    ph_correct[i] = 0;
                }

                cum_sum_ph_correct += ph_correct[i];
                unwrapped_phase[i + 1] = phase[i + 1] + cum_sum_ph_correct;
            }
        }
        public static double mod(double a, double b)
        {
            return a % b;
        }
        public static double[] fft_phase(alglib.complex[] s, int n)
        {
            double fdat1, fdat2;//float
            //  if(FFT_modus[FFT_calcu_num][m] > 30)
            double[] phase = new double[n];
            //n=s.Length;
            for (int i = 0; i < n; i++)
            {
                fdat1 = s[i].x;     //实部
                fdat2 = s[i].y;     //虚部
                double angle = 0;
                if (fdat1 < 0)
                {
                    fdat1 *= (-1);
                }

                //			Lcd_ClearPage(7);  
                //    lcd_show_num(0,7,s[1].real*10);
                //   lcd_show_num(40,7,s[1].imag*10);

                //  fdat2 = (float)Math.Sqrt((fdat1 * fdat1) + (fdat2 * fdat2));
                fdat1 = fdat1 / fdat2;
                //    fdat2 = (float)Math.Asin(fdat1);
                //     fdat2 = (float)(fdat2 * 18000 / 3.1415926);
                fdat2 = Math.Atan(fdat2) * 180 / 3.1415926;

                if (i > 0)
                {
                    if (Math.Abs((phase[i - 1] - fdat2)) > 160)
                    {
                        if (phase[i - 1] - fdat2 > 0)
                        {
                            fdat2 = fdat2 + 180;
                        }
                        if (phase[i - 1] - fdat2 < 0)
                        {
                            fdat2 = fdat2 - 180;
                        }
                    }
                }
                ////将角度调整到0 ~ 360度
                //if (s[i].x > 0)
                //{
                //    if (s[i].y > 0)
                //    {                                   //第二象限 90 ~ 180
                //        angle = (18000 - fdat2);
                //    }
                //    else
                //    {
                //        if (s[i].y == 0)
                //        {                                //90度
                //            angle = 9000;
                //        }
                //        else
                //        {                                //第一象限 0 ~ 90
                //            angle = fdat2;
                //        }
                //    }
                //}
                //else
                //{
                //    if (s[i].x == 0)
                //    {
                //        if (s[i].y > 0)
                //        {                                //180度
                //            angle = 18000;
                //        }
                //        else
                //        {                                //0度
                //            angle = 0;
                //        }
                //    }
                //    else
                //    {
                //        if (s[1].y > 0)
                //        {                                //第三象限 180 ~ 270
                //            angle = (18000 + fdat2);
                //        }
                //        else
                //        {
                //            if (s[1].y == 0)
                //            {                             //270度
                //                angle = 27000;
                //            }
                //            else
                //            {                             //第四象限 270 ~ 360
                //                angle = (36000 - fdat2);
                //            }
                //        }
                //    }
                //}
                angle = fdat2;
                phase[i] = angle;
            }

            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < n; i++)
                {

                    if (i > 0)
                    {
                        if (Math.Abs((phase[i - 1] - phase[i])) > 160)
                        {
                            if (phase[i - 1] - phase[i] > 0)
                            {
                                phase[i] = phase[i] + 180;
                            }
                            if (phase[i - 1] - phase[i] < 0)
                            {
                                phase[i] = phase[i] - 180;
                            }
                        }
                    }
                    //  phase[i] = angle;
                }

            }

            return phase;

            //	angle = fdat2;
        }


        #region 相关性运算int Correlation(IntputData,ReferenceData,OutputData)
        public static int Correlation(double[] IntputData, double[] ReferenceData, int n)
        {//参考Steven W. Smith的 实用数字信号处理 7.3章节  https://www.dspguide.com/ch7/3.htm     
            int i = 0;
            int j = 0;
            int inlen = IntputData.Length;
            int reflen = ReferenceData.Length;
            int OutLen = inlen + reflen - 1;
            double[] OutputData = new double[OutLen];//重新初始化数组大小

            //反转输入信号
            ////此步是相关与卷积的重要差别！
            double[] inData = new double[reflen];
            inData = flip_impulse_array(IntputData);

            for (i = 0; i < inlen; i++)
            {
                for (j = 0; j < reflen; j++)
                {
                    OutputData[i + j] = OutputData[i + j] + inData[i] * ReferenceData[j];
                }
            }    //}
                 //    writefile(OutputData, n);
            int iPos = IndexPos(OutputData, inlen, reflen);
            return iPos;
        }

        public static double[] flip_impulse_array(double[] data)
        {//参考Steven W. Smith的 实用数字信号处理 7.3章节  https://www.dspguide.com/ch7/3.htm   
            //信号反转（一般为参考信号反转，因为数据量小）
            int DataLen = data.Length;
            double[] tmp = new double[DataLen];
            int i = 0;
            int j = 0;

            for (i = DataLen - 1, j = 0; i >= 0; i--, j++)
                tmp[j] = data[i];

            //for (j = 0; j < DataLen; j++)
            //    data[j] = tmp[j];
            return tmp;
        }
        #endregion


        public static int IndexPos(double[] OutputData, int inlen, int reflen)
        {

            //那个数据长就用那个，主要是怕搜索不到位置
            int HalfLen = (inlen > reflen) ? inlen : reflen;
            HalfLen = inlen;
            int Dataindex = 0;
            int iPos = 0;
            double max;
            double xcorr;
            double tmpdouble;
            max = System.Math.Abs(OutputData[0]);

            for (int idx = 0; idx < HalfLen; idx++)
            {
                tmpdouble = System.Math.Abs(OutputData[idx]);
                if (max < tmpdouble)
                {
                    max = tmpdouble;
                    iPos = idx;
                    xcorr = max;
                }
            }
            //printf("max= [%f] \n", max);
            //printf("ipos= [%d] \n", iPos);
            Dataindex = HalfLen - iPos;
            return Dataindex;
        }

        /// <summary>
        /// 保存数据data到文件的处理过程；
        /// </summary>
        /// <param name="data"></param>

        public static double[] CshapCorr(double[] datain, double[] dataso)
        {
            int inlen = datain.Length;
            int outlen = dataso.Length;
            double[] Corr = new double[inlen + outlen - 1];
            int N = (inlen > outlen) ? inlen : outlen;
            double sxy;
            int delay; int i; int j;
            for (delay = -N + 1; delay < N; delay++)
            {
                sxy = 0;
                for (i = 0; i < outlen; i++)
                {
                    j = i + delay;
                    if ((j < 0) || (j >= N))  //The series are no wrapped,so the value is ignored
                        continue;
                    else
                        sxy += (datain[i] * dataso[j]);
                }

                Corr[delay + N - 1] = sxy;
            }
            return Corr;
        }

        public static alglib.complex[] alglibFFTr1d(double[] datain)
        {
            //=======将数据限制在2的N次方内用于FFT计算！
            double l2 = Math.Log((datain.Length), 2);
            int l2int = (int)l2;
            if (l2int < l2)
            {
                l2int = l2int + 1;//数据只能多不能少！
            }

            int FFTinlen = (int)Math.Pow(2, l2int);//只取得 2^l2int 次方的数据

            double[] dataCorr = new double[FFTinlen];//inlen - uci];inlen
            //数据补零，用于FFT运算
            for (int i = 0; i < FFTinlen; i++)
            {
                if (datain.Length > i)
                {
                    dataCorr[i] = datain[i];
                }
                else
                {
                    dataCorr[i] = 0;
                }
            }

            alglib.complex[] fdata;
            alglib.fftr1d(dataCorr, out fdata);
            Console.WriteLine("===alglib FFT=====" + DateTime.UtcNow.ToString());//+ "iPos: " 
            return fdata;
        }

        public static SFR.SFRcomplex SFR_DFT(double[] data, double Freq, int SampleRate)
        {
            SFR.SFRcomplex result;
            //SinusoidSignal(double Amplitude, double Frequency, double PhaseDeg, double SignalDC, int SampleRate)
            Signal sinSignal = new SinusoidSignal(1, Freq, 0,0, SampleRate);
            Signal cosSignal = new SinusoidSignal(1, Freq, 90,0, SampleRate);//90度后移 为COS信号波形

            int iFFTLen = data.Length;
            float[] pfIn = new float[iFFTLen];

            float[] SinData = new float[iFFTLen];
            float[] CosData = new float[iFFTLen];
            float offset = 0;//计算整个信号的直流偏置
            float fSinResp = 0;
            float fCosResp = 0;

            for (int i = 0; i < iFFTLen; i++)
            {// (float)(Math.Sin(2 * Math.PI * Freq * i / SampleRate + (phaseDeg * Math.PI / 180)) + DC);
                SinData[i] = (float)sinSignal.GetValue(i);
                CosData[i] = (float)cosSignal.GetValue(i);
                offset = (float) (offset +  data[i]);
            }
           offset /= iFFTLen;

            //   是否归一化待定
            //   for (int j = 0; j < iFFTLen; j++)
            //   {
            //       // fValue = (float)datain[j] / 32767.0;
            //       pfIn[j] = (float)(pfIn[j] - offset);  
            //   }

            double fValue = 0.0;//+ iStartPos
            for (int j = 0; j < iFFTLen; j++)
            {
                fValue = (float)data[j]-offset ;//去掉直流偏置 / 32767.0;
                pfIn[j] = (float)fValue;
                fSinResp += (pfIn[j] * SinData[j]);
                fCosResp += (pfIn[j] * CosData[j]);
            }

            fSinResp /= (float)(iFFTLen);
            fSinResp *= 2;
            fCosResp /= (float)(iFFTLen);
            fCosResp *= 2;

            //加窗后 幅度值需要修正，相位是否需要修正尚未实验确定
            //if (bUseWindow)
            //{
            //    pRespResultArr[i] = 20.0 * log10(2.0 * sqrt(fSinResp * fSinResp + fCosResp * fCosResp));
            //}
            //else
            //{[i]
            //double pRespResultArr = 20.0 * Math.Log10(Math.Sqrt((fSinResp * fSinResp) + (fCosResp * fCosResp)));
            //  }[i]


            //double pPhaseResultArr = Math.Atan2(fCosResp, fSinResp) * 180.0 / Math.PI;

            result.Re = fSinResp;
            result.Im = fCosResp;
            result.x = 20.0 * Math.Log10(Math.Sqrt((result.Re * result.Re) + (result.Im * result.Im)));// pRespResultArr;
            result.y = Math.Atan2(result.Im, result.Re) * 180.0 / Math.PI;// pPhaseResultArr;
            //Console.WriteLine("===FFT data Write File=====" + "ampl: " + result.x.ToString());
            //Console.WriteLine("===FFT data Write File=====" + "phase: " + result.y.ToString());
            return result;
        }

        public static SFR.SignalData SFR_DFT(SFR.SignalData  datain)
        {
            SFR.SignalData result=datain ;
        //    result = datain;
            //SinusoidSignal(double Amplitude, double Frequency, double PhaseDeg, double SignalDC, int SampleRate)
            Signal sinSignal = new SinusoidSignal(1, datain.Freq, 0, 0, datain.SampleRate);
            Signal cosSignal = new SinusoidSignal(1, datain.Freq, 90, 0, datain.SampleRate);//90度后移 为COS信号波形

            int iFFTLen = datain.Data.Length ;
            float[] pfIn = new float[iFFTLen];

            double[] SinData = new double[iFFTLen];
            double[] CosData = new double [iFFTLen];
            double  offset = 0;//计算整个信号的直流偏置
            double fSinResp = 0;
            double fCosResp = 0;

            for (int i = 0; i < iFFTLen; i++)
            {// (float)(Math.Sin(2 * Math.PI * Freq * i / SampleRate + (phaseDeg * Math.PI / 180)) + DC);
                SinData[i] =  sinSignal.GetValue(i);
                CosData[i] = cosSignal.GetValue(i);
                offset = offset + datain.Data[i];
            }
            offset /= iFFTLen;

            //   是否归一化待定
            //   for (int j = 0; j < iFFTLen; j++)
            //   {
            //       // fValue = (float)datain[j] / 32767.0;
            //       pfIn[j] = (float)(pfIn[j] - offset);  
            //   }

            double fValue = 0.0;//+ iStartPos
            for (int j = 0; j < iFFTLen; j++)
            {
                fValue = (float)datain.Data[j] - offset;//去掉直流偏置 / 32767.0;
                pfIn[j] = (float)fValue;
                fSinResp += (pfIn[j] * SinData[j]);
                fCosResp += (pfIn[j] * CosData[j]);
            }

            fSinResp /= (float)(iFFTLen);
            fSinResp *= 2;
            fCosResp /= (float)(iFFTLen);
            fCosResp *= 2;

            //加窗后 幅度值需要修正，相位是否需要修正尚未实验确定
            //if (bUseWindow)
            //{
            //    pRespResultArr[i] = 20.0 * log10(2.0 * sqrt(fSinResp * fSinResp + fCosResp * fCosResp));
            //}
            //else
            //{[i]
            //double pRespResultArr = 20.0 * Math.Log10(Math.Sqrt((fSinResp * fSinResp) + (fCosResp * fCosResp)));
            //  }[i]


            //double pPhaseResultArr = Math.Atan2(fCosResp, fSinResp) * 180.0 / Math.PI;
            result.CosData = CosData;
            result.SinData = SinData;
            result.Samples = iFFTLen;
            result.Re = fSinResp;
            result.Im = fCosResp;
            result.Amplitude  = 20.0 * Math.Log10(Math.Sqrt((result.Re * result.Re) + (result.Im * result.Im)));// pRespResultArr;
            double PI = 3.141592653589793238462643383279502884197169399;
            //PI = 3.1415926535897932384626433832795028841971693993751058209749445923078;
            double tv = Math.Atan2(result.Im, result.Re) * 180.0 / PI;// result.Im / result.Re * 180.0 / Math.PI;//
                                                                          //    result.Phase =Math.Atan(tv) * 180.0 / Math.PI;
            if (tv < 0)
                tv = 360 + tv;//将±180 改为0-360
            result.Phase = tv;// Math.Atan2(result.Im ,result.Re) * 180.0 / Math.PI;//Atan2 返回值为-Pi~ Pi     pPhaseResultArr;
            //Console.WriteLine("===FFT data Write File=====" + "ampl: " + result.x.ToString());
            //Console.WriteLine("===FFT data Write File=====" + "phase: " + result.y.ToString());
            return result;
        }
        public static alglib.complex[] AlglibFFT(double[] data)//暂未实验通过
        {
            //一定条件下与NI Labview结果相同，
            //单一频率的测试结果，尚未与结果完全一致！
            //待继续研究
            //int datainlen = inlen - uci;
            ////int  datainlent =(int) Math.Floor(Math.Log(datainlen, 2))+1;//向下取整再加1
            ////int datainFFTlen = 1 << datainlent;//求 2^X 另一种算法(只限于X为正整数)！
            //////int datainlentint = (int)Math.Log(datainlen, 2);
            //////if (datainlentint < datainlen)
            //////    datainlentint = datainlentint + 1;

            //////datainlent =(int) Math.Pow(datainlentint, 2);
            /////


            //int datainFFTlen = inlen - uci;
            //double[] datainFFT = new double[datainFFTlen];

            //for (int i = 0; i < datainFFTlen; i++)
            //{

            //    if (i<datainlen)
            //    {
            //        datainFFT[i] = datain[i]; 
            //    }

            //    if (i >= datainlen)
            //    {
            //        datainFFT[i] = 0;
            //    };
            //} 




            //========Alglib FFT 运算
            alglib.complex[] fftdata;
            fftdata = alglibFFTr1d(data);

            //  fftdata = DLLImportHelper.alglibFFTr1d(datainFFT);


            int SampleRate = 48000;//数据的采样率
            double df = SampleRate / data.Length;// inlen; //频率间隔 df=fs/n
            int fr = fftdata.Length;// / 2;
            double[] ampdata = new double[fr];
            double[] phdata = new double[fr];

            for (int i = 0; i < fr; i++)
            {
                double x = fftdata[i].x / fr;
                double y = fftdata[i].y / fr;
                ampdata[i] = x;// 10 * Math.Log10(Math.Sqrt(x*x + y*y));//
                //double tanRadianValue2 = Math.Atan(tanValue2);//求弧度值
                //double tanAngleValue2                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              = tanRadianValue2 / Math.PI * 180;//求角度
                phdata[i] = y;//  (Math.Atan2(x,y)) * (180/ Math.PI);// Math.Atan((y / x));// 

            }
            return fftdata; 
        }



        public static alglib.complex[] AlglibFFT(SFR.SignalData datain)//暂未实验通过
        {
            //一定条件下与NI Labview结果相同，
            //单一频率的测试结果，尚未与结果完全一致！
            //待继续研究
            //int datainlen = inlen - uci;
            ////int  datainlent =(int) Math.Floor(Math.Log(datainlen, 2))+1;//向下取整再加1
            ////int datainFFTlen = 1 << datainlent;//求 2^X 另一种算法(只限于X为正整数)！
            //////int datainlentint = (int)Math.Log(datainlen, 2);
            //////if (datainlentint < datainlen)
            //////    datainlentint = datainlentint + 1;

            //////datainlent =(int) Math.Pow(datainlentint, 2);
            /////


            //int datainFFTlen = inlen - uci;
            //double[] datainFFT = new double[datainFFTlen];

            //for (int i = 0; i < datainFFTlen; i++)
            //{

            //    if (i<datainlen)
            //    {
            //        datainFFT[i] = datain[i]; 
            //    }

            //    if (i >= datainlen)
            //    {
            //        datainFFT[i] = 0;
            //    };
            //} 




            //========Alglib FFT 运算
            alglib.complex[] fftdata;
            fftdata = alglibFFTr1d(datain.Data );

            //  fftdata = DLLImportHelper.alglibFFTr1d(datainFFT);


            int SampleRate = datain.SampleRate;//数据的采样率
            double df = SampleRate / datain.Data .Length;// inlen; //频率间隔 df=fs/n
            int fr = fftdata.Length;// / 2;
            double[] ampdata = new double[fr];
            double[] phdata = new double[fr];

            for (int i = 0; i < fr; i++)
            {
                double x = fftdata[i].x / fr;
                double y = fftdata[i].y / fr;
                ampdata[i] = x;// 10 * Math.Log10(Math.Sqrt(x*x + y*y));//
                //double tanRadianValue2 = Math.Atan(tanValue2);//求弧度值
                //double tanAngleValue2                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              = tanRadianValue2 / Math.PI * 180;//求角度
                phdata[i] = y;//  (Math.Atan2(x,y)) * (180/ Math.PI);// Math.Atan((y / x));// 

            }
            return fftdata;
        }



        public static int  Xcorrr1d(double[] datain,double[] dataso,out double[] corrdata,out double[] Xdata)
        {
            int uci = 0;
            int inlen = datain.Length;
            int outlen = dataso.Length;
            //   //     datain = DLLImportHelper.HanningWin(filen, datain);
            int corrlen = inlen + outlen - 1; //2 * ((inlen > outlen) ? inlen : outlen)  - 1;
            double[] corr = new double[corrlen];
            corrdata = new double[corrlen];
            //计算方式 运算时间差异(单位:秒)
            //=======================================================
            //        输入信号        参考信号         计算时间
            //Alglib     2               2                 1
            //C#函数     2               2                 50      
            //C#函数     2               0.5               9            
            //C#函数     1               0.5               5      
            //C动态库    2               2                 40           
            //C动态库    2               0.5               6           
            //C动态库    1               0.5               4 
            // alglib.xparams _params;
           // Console.WriteLine("===aglib Correlation=====" + "iPos: " + DateTime.UtcNow.ToString());
            alglib.corrr1d(datain, inlen, dataso, outlen, out corr);//,alglib .xparams _params
            uci = IndexPos(corr, inlen, outlen);
            uci = inlen - uci;
            corrdata = corr;//将相关数据输出
            // alglib.corrr1d(datain, inlen, dataso, outlen, corr);

            ////C# 函数方式进行计算  耗时长放弃使用
            //uci = Correlation(datain, dataso, 10);
            //Console.WriteLine("===C# Correlation=====" + "iPos: " + uci.ToString());

            //动态库（未使用第三方库）中函数方式进行计算   2021-11-4 计算速度还是耗时长放弃使用
            // uci = DLLImportHelper.Corrd(datain,datain.Length ,dataso,dataso.Length ,corr)-1;  //corr = CshapCorr(datain, dataso);Correlation

            //Console.WriteLine("===C Correlation=====" + "iPos: " + uci.ToString());

            //Console.WriteLine("===RC=====");



            //将定位后的数据输出
            double[] dataTemp = new double[inlen - uci];
            for (int i = 0; i < inlen - uci; i++)
            {
                dataTemp[i] = datain[i + uci];
            }
            //datain = new double[inlen - uci];
            //for (int i = 0; i < inlen - uci; i++)
            //{
            //    datain[i] = dataTemp[i];
            //}
            Xdata = dataTemp;
            return uci;
        }
        public static double[] HanningWin(double[] data, out double[] dataout)
        {
            int n;
            //double *ret;
            int N = data.Length;
            double fRatio = 0.0;
            double PI = Math.PI;// 3.1415926535897932;
            double[] ret = new double[N];// (double*)malloc(N * sizeof(double));
            dataout = new double[N];                         //
            for (n = 0; n < N; n++)
            {
                fRatio = (double)n / (double)(N - 1);
                ret[n] = data[n] * 0.5 * (1 - System.Math.Cos(2 * PI * fRatio));// *(ret + n) = 0.5 * (1 - cos(2 * PI * fRatio));
            }
            //   data = ret;

            //窗口函数 进行处理
            for (n = 0; n < N; n++)
            {
                dataout[n] = data[n] * ret[n];
            }

            return dataout;
        }

    }

    //=============DLL 动态库接口
    class DLLImportHelper
    {
        #region "传递数组实验"
        //================         一维数组处理        =====================
        //==============     2021-10-20   数组传递实验用     ===============
        //extern "C" _declspec(dllexport) void test_double_group(double *test, int num);

        [DllImport("ArrySend.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void test_double_group(double[] test, int num);


        //================     二维数组处理         ========================
        //==============     2021-10-20   数组传递实验用     ===============
        //extern "C" _declspec(dllexport) void test_double_2groups(double **test, int row, int col);

        [DllImport("ArrySend.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void test_double_2groups(double*[] matrix, int row, int col);

        public static void Assist_test_double_2groups(double[,] myArray, int row, int col)//,int num
        {
            //获取行维度
            int rows = myArray.GetUpperBound(0) + 1;
            //获取列维度
            int cols = myArray.GetUpperBound(1) + 1;

            unsafe
            {
                //fp是myArray[0][0]的地址，是整个二维数组的地址，
                //也是二维数组第一行第0个元素的地址，
                fixed (double* fp = myArray)
                {
                    //定义指针数组的数组名及维度
                    double*[] farr = new double*[rows];
                    //遍历，把每一行开始的地址，存入指针数组中
                    for (int i = 0; i < rows; i++)
                    {
                        farr[i] = fp + i * cols;
                    }
                    //调用dll中对应的函数
                    DLLImportHelper.test_double_2groups(farr, row, col);
                }
            }
        }

        #endregion
        //=================================================================================================

        #region "动态库调用"
        //一般此功能不用，在 GetDelayTimePos 中会调用 （iAutodelay=3 时） 
        //extern "C" _declspec(dllexport) int  XCorr(const short *pInSoundArr, const int inLen, const float *pOutSoundArr, const int outLen);
        [DllImport("ArrySend.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int XCorrA(double[] TestSignal, int TestSignalLen, double[] RefSignal, int RefSignalLen);


        //extern "C" _declspec(dllexport) int  XCorr(const short *pInSoundArr, const int inLen, const float *pOutSoundArr, const int outLen);
        [DllImport("ArrySend.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Corrd(double[] TestSignal, int TestSignalLen, double[] RefSignal, int RefSignalLen, double[] CorrData);

        ////extern "C" _declspec(dllexport) int  Corrf(const double *pInSoundArr, const int inLen, double *pOutSoundArr, const int outLen, double *OutCorr);
        //[DllImport("ArrySend.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int Corrf(double[] TestSignal, int TestSignalLen, double[] RefSignal, int RefSignalLen, double[] CorrData);


        [DllImport("ProcessAudioDll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int Corrf(double[] TestSignal, int TestSignalLen, double[] RefSignal, int RefSignalLen, double[] CorrData);
        static private extern void InitAudio();//得到所加载DLL模块中函数的地址;//得到所加载DLL模块中函数的地址

        //extern "C" _declspec(dllexport) int GetDelayTimePos(const short *pInSoundArr, const int inLen, const float *pOutSoundArr, const int outLen, const int iAutodelay);
        //[DllImport("ArrySend.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int GetDelayTimePos(double[] TestSignal, int TestSignalLen, double[] RefSignal, int RefSignalLen, int iAutodelay);

        //extern "C" _declspec(dllexport) void IndexInit();

        [DllImport("ArrySend.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void IndexInit();

        //extern "C" _declspec(dllexport) int  HanningWin(int N, double **w);
        //extern "C" _declspec(dllexport) void Smooth(double *data, int size);
        //public static void InitAudioDLL()
        //{
        //    InitAudio();
        //}

        #endregion 

        //private void CreateFLow(int fsml, int f_kernel, double x)
        //{
        //    int M = f_kernel;
        //    double k = 500 / fsml;
        //    double sum = 0;
        //    double[]  LHZ = new double[M];
        //    double PI = 3.1415926535897932;
        //    double FC = x * k;

        //    for (int i = 0; i < M; i++)
        //    {
        //        if ((i - M / 2) == 0)
        //            //信号LHZ，
        //            //中点的值为 2 * PI * FC， 即 2 * PI * x * 500/fsml.
        //            //其实这个 For 循环只是使用了同一个计算公式，
        //            //这个 if 分支把 ((i - M / 2) == 0) 的情况拎出来，
        //            //是为了避免下面 除以 (i-M/2) 时数值出现错误。
        //            //手工计算的话， 
        //            //信号中点的值刚好为 sin( 2 * PI * FC ) * ( 0.54 - 0.46* cos( 2 * PI * 1/2 ))， ...
        //            // = sin( 2 * PI * FC ) * ( 0.54 - 0.46* ( -1 ))， ...
        //            // = sin( 2 * PI * FC).
        //            // 原因： x趋于0时， sin(x)/x 趋于1。
        //            //                  
        //            //                                         CodeAnt
        //            //                                        2011.04.11
        //            LHZ[i] = 2 * PI * FC;
        //        if (0 > (i - M / 2) | 0 < (i - M / 2))


        //            LHZ[i] = Math.Sin(2 * PI * FC * (i - M / 2)) / (i - M / 2);

        //        // (0.54 - 0.46 * Math.Cos(2 * PI * i / M))即是汉宁窗，
        //        // 只是 0.54 和0.46 两个系数作了微调。
        //        // 至于前面乘上的 Sin(2 * PI * FC * (i - M / 2))/(i - M /2), 是一个系数,
        //        // 可能是作者的优化。
        //        //                  
        //        //                                         CodeAnt
        //        //                                        2011.04.11
        //        LHZ[i] = LHZ[i] * (0.54 - 0.46 * Math.Cos(2 * PI * i / M));
        //    }

        //    //以下为求信号LHZ中各点值在整列信号中所占的百分比，
        //    //并用百分比覆盖原信号
        //    sum = 0;
        //    for (int i = 0; i < M; i++)
        //    {
        //        sum = sum + LHZ[i];
        //    }
        //    for (int i = 0; i < M; i++)
        //    {
        //        LHZ[i] = LHZ[i] / sum;
        //    }
        //}


    }


}

