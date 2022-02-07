using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace ArryLoad
{
    /// <summary>
    /// NAudio音频文件读取播放类
    /// </summary>
    public class NAudioReader : ISampleProvider
    {
        public event EventHandler<MaxSampleEventArgs> MaximumCalculated;
        private float maxValue;
        private float minValue;
        public int NotificationCount { get; set; }
        int count;

        private readonly ISampleProvider source;

        private readonly int channels;
        public NAudioReader(ISampleProvider source, int fftLength = 1024)
        {
            channels = source.WaveFormat.Channels;
            if (!IsPowerOfTwo(fftLength))
            {
                throw new ArgumentException("FFT Length must be a power of two");
            }
            this.source = source;
            NotificationCount = source.WaveFormat.SampleRate / 100;
        }

        static bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }


        public WaveFormat WaveFormat => source.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            var samplesRead = source.Read(buffer, offset, count);
            for (int n = 0; n < samplesRead; n += channels)
            {
                Add(buffer[n + offset]);
            }
            return samplesRead;
        }

        private void Add(float value)
        {
            maxValue = Math.Max(maxValue, value);
            minValue = Math.Min(minValue, value);
            count++;
            if (count >= NotificationCount && NotificationCount > 0)
            {
                MaximumCalculated?.Invoke(this, new MaxSampleEventArgs(minValue, maxValue));
                Reset();
            }
        }

        public void Reset()
        {
            count = 0;
            maxValue = minValue = 0;
        }

        internal void Start()
        {
        }

        internal void Stop()
        {
        }
    }
}