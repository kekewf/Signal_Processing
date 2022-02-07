using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

namespace ArryLoad
{
    class NAudioRecorder : ISpeechRecorder
    {
        public event EventHandler<MaxSampleEventArgs> MaximumCalculated;
        private float maxValue;
        private float minValue;
        public int NotificationCount { get; set; }
        int count;

        public bool PerformFFT { get; set; }
        private readonly int fftLength;

        private readonly int channels;

        public WaveIn waveSource = null;
        public WaveFileWriter waveFile = null;
        private string fileName = string.Empty;
        public event Action<int> DataAvailable;

        public NAudioRecorder()
        {
            fftLength = 1024;
            channels = 2;
            if (!IsPowerOfTwo(fftLength))
            {
                throw new ArgumentException("FFT Length must be a power of two");
            }
        }

        static bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        /// <summary>
        /// 开始录音
        /// </summary>
        public void StartRec()
        {
            waveSource = new WaveIn();
            waveSource.WaveFormat = new WaveFormat(16000, 16, 2); // 16bit,16KHz,Mono的录音格式
            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

            waveFile = new WaveFileWriter(fileName, waveSource.WaveFormat);
            waveSource.StartRecording();
        }

        /// <summary>
        /// 停止录音
        /// </summary>
        public void StopRec()
        {
            if (waveSource != null)
            {
                waveSource.StopRecording();
            }
        }

        /// <summary>
        /// 录音结束后保存的文件路径
        /// </summary>
        /// <param name="fileName">保存wav文件的路径名</param>
        public void SetFileName(string fileName)
        {
            this.fileName = fileName;
        }


        /// <summary>
        /// 开始录音回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveFile != null)
            {
                waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                waveFile.Flush();

                int secondsRecorded = (int)(waveFile.Length / waveFile.WaveFormat.AverageBytesPerSecond);
                DataAvailable?.Invoke(secondsRecorded);

                float[] sts = new float[e.Buffer.Length / channels];
                int outIndex = 0;
                for (int n = 0; n < e.Buffer.Length; n += channels)
                {
                    sts[outIndex++] = BitConverter.ToInt16(e.Buffer, n) / 32768f;
                }

                for (int n = 0; n < sts.Length; n += channels)
                {
                    Add(sts[n]);
                }
            }
        }

        private void Add(float value)
        {
            maxValue = Math.Max(maxValue, value);
            minValue = Math.Min(minValue, value);
            count++;
            if (count >= waveSource.WaveFormat.SampleRate / 100)
            {
                MaximumCalculated?.Invoke(this, new MaxSampleEventArgs(minValue, maxValue));
                count = 0;
                maxValue = 0;
                minValue = 0;
            }
        }



        /// <summary>
        /// 录音结束回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }

            if (waveFile != null)
            {
                waveFile.Dispose();
                waveFile = null;
            }
        }

        public void Reset()
        {
            count = 0;
            maxValue = minValue = 0;
        }
    }

    public class MaxSampleEventArgs : EventArgs
    {
        [DebuggerStepThrough]
        public MaxSampleEventArgs(float minValue, float maxValue)
        {
            MaxSample = maxValue;
            MinSample = minValue;
        }
        public float MaxSample { get; private set; }
        public float MinSample { get; private set; }
    }

    public class FftEventArgs : EventArgs
    {
        [DebuggerStepThrough]
        public FftEventArgs(Complex[] result)
        {
            Result = result;
        }
        public Complex[] Result { get; private set; }
    }
}
