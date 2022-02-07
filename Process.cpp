#include "stdafx.h"
#include <windows.h>
#include <process.h>    /* _beginthread, _endthread */
#include <ole2.h>
#include <mmsystem.h>
#include <memory.h>
#include <math.h>
#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include "process.h"
#include <iostream>
using namespace std;


short vers = 0x120;  // initial

#define MPI  3.1415926535897932384626433832795


short int left1[384000];
short int right1[384000];
short int left2[384000];
short int right2[384000];

wstring OperatingSystem;
char DirectoryPath[160];
char filename[160];
char projectname[80];


//int NUM_CHANNELS;
FILE *outfile;
short teststate = 0;
int startindex1;
//int WavIndex;
long startindex2;
long startindex3;
long indexoffset;
short stimbuff[4 * SAMPLERATE];  // Stimulus is 2 second long, stereo at 48K rate
short testtonebuff[2 * SAMPLERATE];  // Test Tone is 1 second long, stereo at 48K rate
long stimbuffindex = 0;
long input_index1 = 0;
long input_index2 = 0;
long input_index3 = 0;

float delayval = (float)0.0;
double playbackgain = (double)1.0;
float refmic;
float testmic;
float Noiseref;
float Noisetest1;
float Noisetest2;
float Noisetest3;
int  TotalFreqNum = 0;
float refmicin1[384000];  // up to 4 seconds of data
float refmicin2[384000];
float refmicin3[384000];
float testmicin1[384000];
float testmicin2[384000];
float testmicin3[384000];

short pcmout[384000];

float refbuffer[SM_BUFFER_SIZESO4];
float testbuffer[SM_BUFFER_SIZESO4];

float dBtestbuffer1[NTODO];
float dBtestbuffer2[NTODO];
float dBtestbuffer3[NTODO];
float dBrefbuffer[NTODO];
float LPdBtestbuffer1[NTODO];
float LPdBtestbuffer2[NTODO];
float LPdBtestbuffer3[NTODO];
float LPdBrefbuffer[NTODO];


// 14 frequencies evaluated by DFT.
float fcoefs[NTODO];
float icoefs[NTODO];
float rcoefs[NTODO];
float rval;
float ival;
float frefstate1[NTODO];
float frefstate2[NTODO];
float fteststate11[NTODO];
float fteststate21[NTODO];
float fteststate12[NTODO];
float fteststate22[NTODO];
float fteststate13[NTODO];
float fteststate23[NTODO];

float noiseteststate11[NTODO];
float noiseteststate21[NTODO];
float noiseteststate12[NTODO];
float noiseteststate22[NTODO];
float noiseteststate13[NTODO];
float noiseteststate23[NTODO];
float noiserefstate1[NTODO];
float noiserefstate2[NTODO];
float noisevalref[NTODO];
float noisevaltest1[NTODO];
float noisevaltest2[NTODO];
float noisevaltest3[NTODO];
float dBnoiserefbuffer[NTODO];
float dBnoisetestbuffer1[NTODO];
float dBnoisetestbuffer2[NTODO];
float dBnoisetestbuffer3[NTODO];
float anoiseref;
float anoisetest1;
float anoisetest2;
float anoisetest3;
float mvalref[NTODO];  // measured magnitudes^2
float mvaltest1[NTODO];
float mvaltest2[NTODO];
float mvaltest3[NTODO];

float distcoefs[NTODO];		// coeficient for fundamental generation at 1K
// coeficient for 2nd harmonic detection
// coeficient for 3rd harmonic detection
float diststate1[NTODO];
float diststate2[NTODO];
float rdistcoefs[NTODO];
float idistcoefs[NTODO];
float disttest[NTODO];  // distortion results, 1K, 2K, 3K
//==2020-5-30 增加Refmic Distortion
float DistRefCoefs[NTODO];		// coeficient for fundamental generation at 1K
// coeficient for 2nd harmonic detection
// coeficient for 3rd harmonic detection
float DistRefState1[NTODO];
float DistRefState2[NTODO];
float RDistRefCoefs[NTODO];
float IDistRefcoefs[NTODO];
float DistRefTest[NTODO];  // distortion results, 1K, 2K, 3K


bool CubePro;
float energylevel;
float Phaseref[NTODO];
float Phasetest1[NTODO];
float Phasetest2[NTODO];
float Phasetest3[NTODO];
float Rvaltest1[NTODO];
float Rvaltest2[NTODO];
float Rvaltest3[NTODO];
float Ivaltest1[NTODO];
float Ivaltest2[NTODO];
float Ivaltest3[NTODO];
float Rvalref[NTODO];
float Ivalref[NTODO];

float impcoef;
float impstate1;
float impstate2;
float rimpcoef;
float iimpcoef;
float imptest;  // impedance test result
int  ZeroHZ;
int SetMic = 0;
float state1, state2, stateA, stateB, fval, enrgy, tone;


float ThdTime;
float SealTime;
float SensTime;
float FrphTime;
float NoiseTime;
float THDCalibTime;

float SensFreq;///计算归一化频点用
float SensSpl;

float THDFreq = 1000.0;
float THDSpl;

float LeakFreq;
float LeakSpl;

int Freqnum;//Freq Resp 
int ThdFreqnum = 0;//THD  
int LeakFreqnum = 0;//Leak 
int ImpFreqnum = 0;//Imp Freq Resp 
float ThdFreqs[NTODO];

int SweepType = 3;

float SpeakerAmp[NTODO];//各频点幅值
float SpeakerPhase[NTODO];//各频点相位值
bool InitFreq = false;//是否设置频率数组
bool InitAmp = false;//是否初始化过各频点增益

float SignalTimeSet[NTODO];//各频点幅值

//信号处理起始点
int Dist_Start_Point;// (SAMPLERATE*0.2) 
int Dist_End_point;// DIST_START + (SAMPLERATE*0.2);
// impedance test starts for cycles after 0.25 seconds  (increased to 512 10/30/06)
//#define IMP_START (384)  //(256) //(8000 + 200 + 128)

// impedance test ends 4 cycles early   // 200 samples is the silent period between L and R 1Khz tones.
//#define IMP_END (384 + 9600) //(256 + 6400)  //(16000 + 200 - 256)

// Freq start stop not as critical, but should capture entire 1 second period plus some of the silence on each end.
// 从0.7S-0.9S范围获取数据
int Freq_Start_Point;// (SAMPLERATE*(ThdTime + SealTime + SensTime + 0.2) + 200 * SpecTime)  //(SAMPLERATE*0.7)
int Freq_End_Point;//  Freq_Start_Point + (int)(SAMPLERATE*(FrphTime - 0.4))  //(FREQ_START + (SAMPLERATE*0.2))

// Noise testing lasts 0.5 second, minus 4 cycles at 1K on each end  1.8S-2.0S
int Noise_Start_Point; //(SAMPLERATE*1.8)
int Noise_End_Point; //(NOISE_START+(SAMPLERATE*0.2))

int StartFreqInt = 0;
int StopFreqInt = 0;


//int THDCalibTime;//THDClibTime
//float LeakTime;

int StartFreq;//开始频率
int StopFreq;//终止频率

//test freqs point process 
int fpoint;
float freqND;
//THIS USED IN BOSE ANC2D
float freqs[NTODO];/* = {
	20.0,22.4,25.0,28.0,31.5,35.5,40.0,	45.0,50.0,56.0,63.0,71.0,80.0,90.0,
	100.0,112.0,125.0,140.0,160.0,180.0,200.0,224.0,250.0,280.0,315.0,355.0,
	400.0,450.0,500.0,560.0,630.0,710.0,800.0,900.0,
	1000.0,
	1120.0,1250.0,1400.0,1600.0,1800.0,2000.0,2240.0,2500.0,2800.0,3150.0,3550.0,
	4000.0,4500.0,5000.0,5600.0,6300.0,7100.0,8000.0,9000.0,
	10000.0};*/
float freqsR1d3[21] = {
	1.0, 2.0, 4.0, 8.0, 16.0, 31.5, 63.0, 125.0, 250.0, 500.0, 1000.0,
	2000.0, 4000.0, 8000.0, 16000.0, 31500.0, 63000.0, 125000.0, 250000.0, 500000.0,
	1000000.0 };
float freqsR10[51] = {
	1.00, 1.25, 1.60, 2.00, 2.50, 3.15, 4.00, 5.00, 6.30, 8.00,
	10.00, 12.50, 16.00, 20.00, 25.00, 31.50, 40.00, 50.00, 63.00, 80.00,
	100.00, 125.00, 160.00, 200.00, 250.00, 315.00, 400.00, 500.00, 630.00, 800.00,
	1000.00, 1250.00, 1600.00, 2000.00, 2500.00, 3150.00, 4000.00, 5000.00, 6300.00, 8000.00,
	10000.00, 12500.00, 16000.00, 20000.00, 25000.00, 31500.00, 40000.00, 50000.00, 63000.00, 80000.00,
	100000.00 };
float freqsR20[101] = {
	1.00, 1.12, 1.25, 1.40, 1.60, 1.80, 2.00, 2.24, 2.50, 2.80, 3.15, 3.55, 4.00, 4.50, 5.00, 5.60, 6.30, 7.10, 8.00, 9.00,
	10.00, 11.20, 12.50, 14.00, 16.00, 18.00, 20.00, 22.40, 25.00, 28.00, 31.50, 35.50, 40.00, 45.00, 50.00, 56.00, 63.00, 71.00, 80.00, 90.00,
	100.00, 112.00, 125.00, 140.00, 160.00, 180.00, 200.00, 224.00, 250.00, 280.00, 315.00, 355.00, 400.00, 450.00, 500.00, 560.00, 630.00, 710.00, 800.00, 900.00,
	1000.00, 1120.00, 1250.00, 1400.00, 1600.00, 1800.00, 2000.00, 2240.00, 2500.00, 2800.00, 3150.00, 3550.00, 4000.00, 4500.00, 5000.00, 5600.00, 6300.00, 7100.00, 8000.00, 9000.00,
	10000.00, 11200.00, 12500.00, 14000.00, 16000.00, 18000.00, 20000.00, 22400.00, 25000.00, 28000.00, 31500.00, 35500.00, 40000.00, 45000.00, 50000.00, 56000.00, 63000.00, 71000.00, 80000.00, 90000.00,
	100000.00 };
float freqsR40[201] = {
	1.00, 1.06, 1.12, 1.18, 1.25, 1.32, 1.40, 1.50, 1.60, 1.70, 1.80, 1.90,
	2.00, 2.12, 2.24, 2.36, 2.50, 2.65, 2.80, 3.00, 3.15, 3.35, 3.55, 3.75,
	4.00, 4.25, 4.50, 4.75, 5.00, 5.30, 5.60, 6.00, 6.30, 6.70, 7.10, 7.50,
	8.00, 8.50, 9.00, 9.50, 10.00, 10.60, 11.20, 11.80, 12.50, 13.20, 14.00, 15.00,
	16.00, 17.00, 18.00, 19.00, 20.00, 21.20, 22.40, 23.60, 25.00, 26.50, 28.00, 30.00,
	31.50, 33.50, 35.50, 37.50, 40.00, 42.50, 45.00, 47.50, 50.00, 53.00, 56.00, 60.00,
	63.00, 67.00, 71.00, 75.00, 80.00, 85.00, 90.00, 95.00, 100.00, 106.00, 112.00, 118.00,
	125.00, 132.00, 140.00, 150.00, 160.00, 170.00, 180.00, 190.00, 200.00, 212.00, 224.00, 236.00,
	250.00, 265.00, 280.00, 300.00, 315.00, 335.00, 355.00, 375.00, 400.00, 425.00, 450.00, 475.00,
	500.00, 530.00, 560.00, 600.00, 630.00, 670.00, 710.00, 750.00, 800.00, 850.00, 900.00, 950.00,
	1000.00, 1060.00, 1120.00, 1180.00, 1250.00, 1320.00, 1400.00, 1500.00, 1600.00, 1700.00, 1800.00, 1900.00,
	2000.00, 2120.00, 2240.00, 2360.00, 2500.00, 2650.00, 2800.00, 3000.00, 3150.00, 3350.00, 3550.00, 3750.00,
	4000.00, 4250.00, 4500.00, 4750.00, 5000.00, 5300.00, 5600.00, 6000.00, 6300.00, 6700.00, 7100.00, 7500.00,
	8000.00, 8500.00, 9000.00, 9500.00, 10000.00, 10600.00, 11200.00, 11800.00, 12500.00, 13200.00, 14000.00, 15000.00,
	16000.00, 17000.00, 18000.00, 19000.00, 20000.00, 21200.00, 22400.00, 23600.00, 25000.00, 26500.00, 28000.00, 30000.00,
	31500.00, 33500.00, 35500.00, 37500.00, 40000.00, 42500.00, 45000.00, 47500.00, 50000.00, 53000.00, 56000.00, 60000.00,
	63000.00, 67000.00, 71000.00, 75000.00, 80000.00, 85000.00, 90000.00, 95000.00, 100000.00 };
float freqsR80[401] = {
	1.00, 1.03, 1.06, 1.09, 1.12, 1.15, 1.18, 1.22,
	1.25, 1.28, 1.32, 1.36, 1.40, 1.45, 1.50, 1.55,
	1.60, 1.65, 1.70, 1.75, 1.80, 1.85, 1.90, 1.95,
	2.00, 2.06, 2.12, 2.18, 2.24, 2.30, 2.36, 2.43,
	2.50, 2.58, 2.65, 2.72, 2.80, 2.90, 3.00, 3.07,
	3.15, 3.25, 3.35, 3.45, 3.55, 3.65, 3.75, 3.87,
	4.00, 4.12, 4.25, 4.37, 4.50, 4.62, 4.75, 4.87,
	5.00, 5.15, 5.30, 5.45, 5.60, 5.80, 6.00, 6.15,
	6.30, 6.50, 6.70, 6.90, 7.10, 7.30, 7.50, 7.75,
	8.00, 8.25, 8.50, 8.75, 9.00, 9.25, 9.50, 9.75,
	10.0, 10.3, 10.6, 10.9, 11.2, 11.5, 11.8, 12.2,
	12.5, 12.8, 13.2, 13.6, 14.0, 14.5, 15.0, 15.5,
	16.0, 16.5, 17.0, 17.5, 18.0, 18.5, 19.0, 19.5,
	20.0, 20.6, 21.2, 21.8, 22.4, 23.0, 23.6, 24.3,
	25.0, 25.8, 26.5, 27.2, 28.0, 29.0, 30.0, 30.7,
	31.5, 32.5, 33.5, 34.5, 35.5, 36.5, 37.5, 38.7,
	40.0, 41.2, 42.5, 43.7, 45.0, 46.2, 47.5, 48.7,
	50.0, 51.5, 53.0, 54.5, 56.0, 58.0, 60.0, 61.5,
	63.0, 65.0, 67.0, 69.0, 71.0, 73.0, 75.0, 77.5,
	80.0, 82.5, 85.0, 87.5, 90.0, 92.5, 95.0, 97.5,
	100.0, 103.0, 106.0, 109.0, 112.0, 115.0, 118.0, 122.0,
	125.0, 128.0, 132.0, 136.0, 140.0, 145.0, 150.0, 155.0,
	160.0, 165.0, 170.0, 175.0, 180.0, 185.0, 190.0, 195.0,
	200.0, 206.0, 212.0, 218.0, 224.0, 230.0, 236.0, 243.0,
	250.0, 258.0, 265.0, 272.0, 280.0, 290.0, 300.0, 307.0,
	315.0, 325.0, 335.0, 345.0, 355.0, 365.0, 375.0, 387.0,
	400.0, 412.0, 425.0, 437.0, 450.0, 462.0, 475.0, 487.0,
	500.0, 515.0, 530.0, 545.0, 560.0, 580.0, 600.0, 615.0,
	630.0, 650.0, 670.0, 690.0, 710.0, 730.0, 750.0, 775.0,
	800.0, 825.0, 850.0, 875.0, 900.0, 925.0, 950.0, 975.0,
	1000.0, 1030.0, 1060.0, 1090.0, 1120.0, 1150.0, 1180.0, 1220.0,
	1250.0, 1280.0, 1320.0, 1360.0, 1400.0, 1450.0, 1500.0, 1550.0,
	1600.0, 1650.0, 1700.0, 1750.0, 1800.0, 1850.0, 1900.0, 1950.0,
	2000.0, 2060.0, 2120.0, 2180.0, 2240.0, 2300.0, 2360.0, 2430.0,
	2500.0, 2580.0, 2650.0, 2720.0, 2800.0, 2900.0, 3000.0, 3070.0,
	3150.0, 3250.0, 3350.0, 3450.0, 3550.0, 3650.0, 3750.0, 3870.0,
	4000.0, 4120.0, 4250.0, 4370.0, 4500.0, 4620.0, 4750.0, 4870.0,
	5000.0, 5150.0, 5300.0, 5450.0, 5600.0, 5800.0, 6000.0, 6150.0,
	6300.0, 6500.0, 6700.0, 6900.0, 7100.0, 7300.0, 7500.0, 7750.0,
	8000.0, 8250.0, 8500.0, 8750.0, 9000.0, 9250.0, 9500.0, 9750.0,
	10000.0, 10300.0, 10600.0, 10900.0, 11200.0, 11500.0, 11800.0, 12200.0,
	12500.0, 12800.0, 13200.0, 13600.0, 14000.0, 14500.0, 15000.0, 15500.0,
	16000.0, 16500.0, 17000.0, 17500.0, 18000.0, 18500.0, 19000.0, 19500.0,
	20000.0, 20600.0, 21200.0, 21800.0, 22400.0, 23000.0, 23600.0, 24300.0,
	25000.0, 25800.0, 26500.0, 27200.0, 28000.0, 29000.0, 30000.0, 30700.0,
	31500.0, 32500.0, 33500.0, 34500.0, 35500.0, 36500.0, 37500.0, 38700.0,
	40000.0, 41200.0, 42500.0, 43700.0, 45000.0, 46200.0, 47500.0, 48700.0,
	50000.0, 51500.0, 53000.0, 54500.0, 56000.0, 58000.0, 60000.0, 61500.0,
	63000.0, 65000.0, 67000.0, 69000.0, 71000.0, 73000.0, 75000.0, 77500.0,
	80000.0, 82500.0, 85000.0, 87500.0, 90000.0, 92500.0, 95000.0, 97500.0,
	100000.0 };

float freqsHead[] = { //R10  +8    // R20 +4    R40 +2  R80 +0
	1.00, 1.03, 1.06, 1.09, 1.12, 1.15, 1.18, 1.22,
	1.25, 1.28, 1.32, 1.36, 1.40, 1.45, 1.50, 1.55,
	1.60, 1.65, 1.70, 1.75, 1.80, 1.85, 1.90, 1.95,
	2.00, 2.06, 2.12, 2.18, 2.24, 2.30, 2.36, 2.43,
	2.50, 2.58, 2.65, 2.72, 2.80, 2.90, 3.00, 3.07,
	3.15, 3.25, 3.35, 3.45, 3.55, 3.65, 3.75, 3.87,
	4.00, 4.12, 4.25, 4.37, 4.50, 4.62, 4.75, 4.87,
	5.00, 5.15, 5.30, 5.45, 5.60, 5.80, 6.00, 6.15,
	6.30, 6.50, 6.70, 6.90, 7.10, 7.30, 7.50, 7.75,
	8.00, 8.25, 8.50, 8.75, 9.00, 9.25, 9.50, 9.75,
	10.0 };
//This is used in the BOSE ANC2D
float aweightings[NTODO];/* = {  // dB A weightings of frequencies listed above.
	(float)-40.0,     // replace -40 with actual weightings LATER   DEBUG.....
	(float)-40.0,(float)-40.0,(float)-40.0,(float)-40.0,(float)-40.0,(float)-40.0,
	(float)-40.0,(float)-40.0,(float)-30.19,(float)-29.14,(float)-28.17,(float)-26.99,
	(float)-26.17,(float)-25.16,(float)-24.24,(float)-23.38,(float)-22.40,(float)-21.50,
	(float)-20.67,(float)-19.91,(float)-19.19,(float)-18.40,(float)-17.67,(float)-16.99,
	(float)-16.26,(float)-15.58,(float)-14.86,(float)-14.04,(float)-13.29,(float)-12.61/*,
	(float)-11.98,(float)-11.40,(float)-10.86,(float)-10.26,(float)-9.71,(float)-9.20,(float)-8.65,
	(float)-8.11,(float)-7.61,(float)-7.01,(float)-6.60,(float)-6.09,(float)-5.63,(float)-5.21,
	(float)-4.73,(float)-4.30,(float)-3.91,(float)-3.55,(float)-3.23,(float)-2.87,(float)-2.55,
	(float)-2.17,(float)-1.91,(float)-1.60,(float)-1.33,(float)-1.08,(float)-0.81,(float)-0.57,
	(float)-0.36,(float)-0.18,(float)-0.01,(float)0.16,(float)0.31,(float)0.45,(float)0.58,
	(float)0.70,(float)0.81,(float)0.93,(float)1.02,(float)1.09,(float)1.15,(float)1.20,(float)1.23,
	(float)1.26,(float)1.28,(float)1.29,(float)1.29,(float)1.29,(float)1.27,(float)1.24,(float)1.20,
	(float)1.15,(float)1.10,(float)1.03,(float)0.95,(float)0.85,(float)0.75,(float)0.65,(float)0.54};*/

void SetFreqAmp(int len, double* fl)//外部程序设置各个频率点的幅值unsigned short 
{
	int i;
	float volue=0.0;
	for (i = 0; i < len; i++)
	{
		volue = fl[i] / 20;
		SpeakerAmp[i] = pow(10, volue);
	}
		/*SpeakerAmp[i] = fl[i];*/
	InitAmp = true;//启用新的幅度值!
}

void SetPhases(int len, double* fl)///外部程序设置各个频率点的相位 unsigned short SetPhases(len, fl);
{
	int i;
	for (i = 0; i < len; i++){
		int a = (fl[i] + 0.005) * 100;
		double c = a / 100;
		SpeakerPhase[i] = (float)c;//保留2位小数 fl[i]
	}//		SpeakerPhase[i] = fl[i];
}

void SetFreq(int len, double* fl)//外部程序设置各个频率点
{
	int i;
	TotalFreqNum = len;//设置频点总数量
	InitFreq = true;//已经初始化,启用新频率值
	for (i = 0; i < len; i++){
		int a = (fl[i] + 0.005) * 100;//保留2
		double c = a / 100;
		freqs[i] = (float)c;//保留2位小数  fl[i]
	}
}

void SetSignalTimeParameter(int len, double* fl)//2020-11-25外部程序设置各个时间
{
  if (len>0) ThdTime = fl[0];
  if (len>1) SealTime = fl[1];
  if (len>2) SensTime = fl[2];
  if (len>3) FrphTime = fl[3];
  if (len>4) NoiseTime = fl[4];
  if (len>5) THDCalibTime = fl[5];
}

void SetSignalSensParameter(int len, double* fl)//2020-11-25外部程序设置SensFreq SPL  用于控制整个信号与失真信号的比例！
{
	if (len>0) SensFreq = fl[0];
	if (len>1) SensSpl = fl[1];
}


void SetSignalTHDParameter(int len, double* fl)//2020-11-25外部程序设置灵敏度频率与SPL  用于控制整个信号与失真信号的比例！
{
	if (len>0) THDFreq = fl[0];
	if (len>1) THDSpl = fl[1];
}

void SetSignalLeakParameter(int len, double* fl)//2020-11-25 外部程序设置漏声频率与声压  用于控制整个信号与失真信号的比例！
{
	if (len>0) LeakFreq = fl[0];
	if (len>1) LeakSpl = fl[1];
}

void SetSignalFreqsParameter(int len, double* fl)//2020-11-25 外部程序设置各项测试频率点数！
{
	if (len>0) Freqnum = fl[0];
	if (len>1) ThdFreqnum = fl[1];
	if (len>2) LeakFreqnum = fl[2];
	if (len>3) ImpFreqnum = fl[3];
	//if (len>4) NoiseTime = fl[4];
	//if (len>5) THDCalibTime = fl[5];
}

void SetSignalTHDFreqsParameter(int len, double* fl)//2020-11-25 外部程序设置各项测试频率点数！
{
	ThdFreqs[0] = THDFreq;
	if (ThdFreqnum <= len)
	{
		if (ThdFreqnum > 1)
		{
			for (int i = 0; i < ThdFreqnum; i++)
			{
				ThdFreqs[i] = fl[i];
			}
		}

	}
}
//===========================================================================================================
//===========Gen Signal
void GenSweepSignal()
{
	////扫频文件
	//outfile = fopen("Sweeptone.raw", "wb");
	//float FreqLevel(Freqnum);
	//float FreqPhase(Freqnum);
	//for (findex = 0; findex < Freqnum; findex++){
	//	fcoefs[findex] = (float)2.0 * (float)cos(((float)2.0)*((float)M_PI)*freqs[findex] / (float)SAMPLERATE);
	//	fstate2[findex] = (float)1.0;
	//	fstate1[findex] = ((float)0.5) * fcoefs[findex];
	//}

	//// now randomize phases
	//for (findex = 0; findex < Freqnum; findex++)
	//{
	//	int j, rval;
	//	float sum, tempf;
	//	sum = 0;
	//	rval = rand();
	//	for (j = 0; j < rval; j++) {
	//		sum += tempf = fcoefs[findex] * fstate1[findex] - fstate2[findex];
	//		fstate2[findex] = fstate1[findex];
	//		fstate1[findex] = tempf;
	//	}
	//}
	//int wtime;
	//int SamNum; //Samples Number
	//for (findex = 0; findex < Freqnum; findex++){
	//	//200Hz以前为50ms  以后  信号时间为5ms
	//	if ((float)10 / freqs[findex]>(float)0.05)  SamNum = (int)(SAMPLERATE*(float)10 / freqs[findex]);
	//	else SamNum = (int)(SAMPLERATE / 20);
	//	for (samplenum = 0; samplenum < SamNum; samplenum++)
	//	{
	//		register float tempf;
	//		register float sum;
	//		findex = 0;
	//		tempf = fcoefs[findex] * fstate1[findex] - fstate2[findex];
	//		fstate2[findex] = fstate1[findex];
	//		fstate1[findex] = tempf;
	//		sum = (float)(32767.0*0.7079)*tempf *SpeakerAmp[findex];//各个频点增益调整
	//		intsum = (short)(sum);
	//		fwrite(&intsum, 2, 1, outfile);
	//		//		printf("%d,\n", intsum);
	//		intsum = 0;
	//		fwrite(&intsum, 2, 1, outfile);
	//		//		printf("%d,\n", intsum);
	//	}
	//}
	//fclose(outfile);
}

//float Freqs[NTODO];

//float FrFreqs[NTODO];
//float FrLevel[NTODO];
//float FrPhase[NTODO];
//float LeakFreqs[NTODO];
//float LeakLevel[NTODO];
//float LeakPhase[NTODO];
//float ThdLevel[NTODO];
//float ThdPhase[NTODO];
//float ImpFreqs[NTODO];
//float ImpLevel[NTODO];
//float ImpPhase[NTODO];

void GenSignal()
{
	int SamplesRate;

	//float LeakTime = 0;
	//long  TimeLine = 0;
	float SpecTime = 0;

	//FILE *outfile;

	//float fcoefs[NTODO];
	float fstate1[NTODO];
	float fstate2[NTODO];

	short intsum;
	unsigned long samplenum;

	//float ThdTime;
	//float SealTime;
	//float SensTime;
	//float FrphTime;
	//float NoiseTime;
	//float THDCalibTime;

	////float SensFreq;//外面已经有了/计算归一化频点用
	////float SensSpl; 外面已经有了	float SensSpl;
	////float THDFreq = 1000.0; 外面已经有了
	////float THDSpl; 外面已经有了	//float THDSpl;
	//float LeakFreq;
	//float LeakSpl;
	//int Freqnum;//Freq Resp 
	//int ThdFreqnum = 0;//THD  
	//int LeakFreqnum = 0;//Leak 
	//int ImpFreqnum = 0;//Imp Freq Resp 
	//float ThdFreqs[NTODO];


	//int SweepType = 3; 外面已经有了

	//FILE *fpRead = fopen("SignalSetup.txt", "r");
	////if (fpRead == NULL)	return 0;
	//////取得需要处理的数据
	//////SensTime=0.5;SealTime=0.5 ThdTime=0.5;FrphTime=1;NoiseTime=0.5;THDClibTime=4; //声压测试4s
	//////                                     失真      漏声      灵敏度      频响相位    噪声      声压校准
	//fscanf(fpRead, "%f,%f,%f,%f,%f,%f\n", &ThdTime, &SealTime, &SensTime, &FrphTime, &NoiseTime, &THDCalibTime);//	fscanf(fpRead, "%d\n", &SamplesRate);
	//fscanf(fpRead, "%f,%f\n", &SensFreq, &SensSpl);
	//fscanf(fpRead, "%f,%f\n", &THDFreq, &THDSpl);
	//fscanf(fpRead, "%f,%f\n", &LeakFreq, &LeakSpl);//漏声频率，漏声声压(一般为最大值)
	//fscanf(fpRead, "%d,%d,%d,%d\n", &Freqnum, &ThdFreqnum, &LeakFreqnum, &ImpFreqnum);

	//ThdFreqs[0] = THDFreq;
	//if (ThdFreqnum > 1)
	//{
	//	for (int i = 0; i < ThdFreqnum; i++)
	//	{
	//		fscanf(fpRead, "%f\n", &ThdFreqs[i]);//float ThdFreqs[NTODO],ThdLevel[NTODO],ThdPhase[NTODO];
	//		//		printf("%f \n", ThdFreqs[i]);
	//	}
	//}
	//fclose(fpRead);
	int i;

	double amps;
	double phas;

	//==================频率个数强制刷新，防止更改！
	Freqnum = TotalFreqNum;
	//==============


	for (i = 0; i < TotalFreqNum; i++) {
		distcoefs[i] = ((float)2.0 * (float)cos(((float)2.0)*((float)MPI)*((float)THDFreq *(i + 1)) / ((float)SAMPLERATE)));
		rdistcoefs[i] = (float)cos(((float)2.0)*((float)MPI)*((float)THDFreq *(i + 1)) / ((float)SAMPLERATE));
		idistcoefs[i] = (float)sin(((float)2.0)*((float)MPI)*((float)THDFreq *(i + 1)) / ((float)SAMPLERATE));
		//==2020-5-30 Ref通道失真
		DistRefCoefs[i] = ((float)2.0 * (float)cos(((float)2.0)*((float)MPI)*((float)THDFreq *(i + 1)) / ((float)SAMPLERATE)));
		RDistRefCoefs[i] = (float)cos(((float)2.0)*((float)MPI)*((float)THDFreq *(i + 1)) / ((float)SAMPLERATE));
		IDistRefcoefs[i] = (float)sin(((float)2.0)*((float)MPI)*((float)THDFreq *(i + 1)) / ((float)SAMPLERATE));

	}


	short findex = 0;
	int SenFP;
	int THDFP;
	for (findex = 0; findex < Freqnum; findex++)
	{
		if (freqs[findex] == SensFreq) SenFP = findex;
		if (freqs[findex] == THDFreq) THDFP = findex;
	}

	ZeroHZ = SenFP + 1;
	//for (i = 0; i < TotalFreqNum; i++)
	//{
	//	if (freqs[i] = SensFreq)  ZeroHZ = i + 1;
	//}
	//============testtone file write==================================================
	for (findex = 0; findex < ThdFreqnum; findex++)
	{
		if (ThdFreqnum == 1) ThdFreqs[findex] = THDFreq;
		fcoefs[findex] = (float)2.0 * (float)cos(((float)2.0)*((float)M_PI)*ThdFreqs[findex] / (float)SAMPLERATE);
		//}
		//for (findex = 0; findex < ThdFreqnum; findex++)
		//{
		fstate2[findex] = (float)1.0;
		fstate1[findex] = ((float)0.5) * fcoefs[findex];
	}

	outfile = fopen("testtone.raw", "wb");

	// put out full scale -3 dB 1Khz tone left channel  （THD 0.25S）
	for (findex = 0; findex < ThdFreqnum; findex++)
		fcoefs[findex] = (float)2.0 * (float)cos(((float)2.0)*((float)M_PI)*ThdFreqs[findex] / (float)SAMPLERATE);

	for (findex = 0; findex < ThdFreqnum; findex++)
	{
		fstate2[findex] = (float)1.0;
		fstate1[findex] = ((float)0.5) * fcoefs[findex];
	}
	for (samplenum = 0; samplenum < SAMPLERATE* THDCalibTime; samplenum++)
	{
		register float tempf;
		register float sum;
		if (ThdFreqnum > 1)
		{
			for (findex = 0; findex < ThdFreqnum; findex++) {
				tempf = fcoefs[findex] * fstate1[findex] - fstate2[findex];
				/*		if (Freqs[findex] < (float) 990.00) sum += ((float)400.0 * tempf);
				else if (Freqs[findex] >(float)1010.00) sum += ((float)100.0*tempf);
				else */
				sum += ((float)3276.70*0.7079)*tempf;  // This is 1Khz value*0.707*volue
				fstate2[findex] = fstate1[findex];
				fstate1[findex] = tempf;
			}

		}
		else //只有一个频率点
		{
			findex = 0;
			tempf = fcoefs[findex] * fstate1[findex] - fstate2[findex];
			fstate2[findex] = fstate1[findex];
			fstate1[findex] = tempf;

			//tempf = fcoefs[THDFP] * fstate1[THDFP] - fstate2[THDFP];
			//fstate2[THDFP] = fstate1[THDFP];
			//fstate1[THDFP] = tempf;
			sum = (float)(32767.0*0.7079)*tempf;
		}

		intsum = (short)(sum);
		fwrite(&intsum, 2, 1, outfile);
		//		printf("%d,\n", intsum);
		intsum = 0;
		fwrite(&intsum, 2, 1, outfile);
		//		printf("%d,\n", intsum);
	}
	fclose(outfile);

	//================Stimulate file write==================================================================
	for (findex = 0; findex < Freqnum; findex++)
		fcoefs[findex] = (float)2.0 * (float)cos(((float)2.0)*((float)M_PI)*freqs[findex] / (float)SAMPLERATE);

	for (findex = 0; findex < Freqnum; findex++)
	{
		fstate2[findex] = (float)1.0;
		fstate1[findex] = ((float)0.5) * fcoefs[findex];
	}
	outfile = fopen("stimulus.raw", "wb");

	if (ThdTime > 0)
	{
		// put out full scale -3 dB 1Khz tone left channel  （THD 0.25S）
		for (samplenum = 0; samplenum < SAMPLERATE* ThdTime; samplenum++) {
			register float tempf;
			register float sum;

			tempf = fcoefs[THDFP] * fstate1[THDFP] - fstate2[THDFP];
			fstate2[THDFP] = fstate1[THDFP];
			fstate1[THDFP] = tempf;

			sum = (float)(32767.0*0.7079)*tempf;
			intsum = (short)sum;
			fwrite(&intsum, 2, 1, outfile);
			//			printf("%d,\n", intsum);
			intsum = 0;
			fwrite(&intsum, 2, 1, outfile);
			//			printf("%d,\n", intsum);
		}
		// put out samples of silence  静默，等待扬声器静止(SAMPLERATE/16)
		for (samplenum = 0; samplenum < 200l; samplenum++) {
			intsum = (short)0;
			fwrite(&intsum, 2, 1, outfile);
			//			printf("%d,\n", intsum);
			intsum = 0;
			fwrite(&intsum, 2, 1, outfile);
			//			printf("%d,\n", intsum);
		}
	}

	if (SealTime > 0)
	{//put out full scale -3 dB 1Khz tone right channel  （seal low freq 
		for (samplenum = 0; samplenum < SAMPLERATE*SealTime; samplenum++) {
			register float tempf;
			register float sum;

			tempf = fcoefs[THDFP] * fstate1[THDFP] - fstate2[THDFP];
			fstate2[THDFP] = fstate1[THDFP];
			fstate1[THDFP] = tempf;

			sum = (float)(32767.0*0.7079)*tempf;
			intsum = 0;
			fwrite(&intsum, 2, 1, outfile);
			//			printf("%d,\n", intsum);
			intsum = (short)sum;
			fwrite(&intsum, 2, 1, outfile);
			//			printf("%d,\n", intsum);
		}
		// put out samples of silence  静默，等待扬声器静止SAMPLERATE / 16
		for (samplenum = 0; samplenum < 200l; samplenum++) {
			intsum = (short)0;
			fwrite(&intsum, 2, 1, outfile);
			//			printf("%d,\n", intsum);
			intsum = 0;
			fwrite(&intsum, 2, 1, outfile);
			//			printf("%d,\n", intsum);
		}
	}

	// put out full scale 94 dB 1Khz tone left channel  （Sens 0.25S）
	float volue;
	//计算 94dB与THD声压差值 的倍数，用于后面94dB灵敏度信号的处理
	volue = SensSpl - THDSpl;
	volue = volue / 20;
	volue = pow(10, volue);

	if (SensTime > 0)
	{
		for (samplenum = 0; samplenum < SAMPLERATE *SensTime; samplenum++) {
			register float tempf;
			register float sum;

			tempf = fcoefs[SenFP] * fstate1[SenFP] - fstate2[SenFP];
			fstate2[SenFP] = fstate1[SenFP];
			fstate1[SenFP] = tempf;

			sum = (float)(32767.0*0.7079*volue)*tempf;
			intsum = (short)sum;
			fwrite(&intsum, 2, 1, outfile);//		printf("%d,\n", intsum);//		intsum = 0;
			//	sum = (float)(32767.0*0.7079)*tempf;
			//	intsum = (short)sum;
			intsum = 0;
			fwrite(&intsum, 2, 1, outfile);
			//			printf("%d,\n", intsum);
		}

		// put out samples of silence  静默，等待扬声器静止 SAMPLERATE / 16
		if (SAMPLERATE*SensTime > 0)
		{
			for (samplenum = 0; samplenum < 200l; samplenum++) {
				intsum = (short)0;
				fwrite(&intsum, 2, 1, outfile);
				//				printf("%d,\n", intsum);
				intsum = 0;
				fwrite(&intsum, 2, 1, outfile);
				//				printf("%d,\n", intsum);
			}
		}
		//// put out full scale 1Khz tone right channel at 110dB SPL (set to be full scale)
		//
		//for (samplenum = 0; samplenum < 12000l; samplenum++) {
		//	register float tempf;k
		//	register float sum;
		//
		//	tempf =  fcoefs[SenFP] * fstate1[SenFP] - fstate2[SenFP];
		//	fstate2[SenFP] = fstate1[SenFP];
		//	fstate1[SenFP] = tempf;
		//
		//	sum = (float)(32767.0)*tempf;
		//	intsum = 0;
		//	fwrite(&intsum,2,1,outfile);
		//	printf("%d,\n",intsum);
		//	intsum = (short)sum;
		//	fwrite(&intsum,2,1,outfile);
		//	printf("%d,\n",intsum);
		//}
		//// put out 200 samples of silence
		//for (samplenum = 0; samplenum < 200l; samplenum++) {
		//	intsum = (short)0;
		//	fwrite(&intsum,2,1,outfile);
		//	printf("%d,\n",intsum);
		//	intsum = 0;
		//	fwrite(&intsum,2,1,outfile);
		//	printf("%d,\n",intsum);
		//}
		fstate2[SenFP] = (float)1.0;
		fstate1[SenFP] = ((float)0.5) * fcoefs[SenFP];
	}
	// 写 Freqresp 音源时 之前再次初始化一次
	for (findex = 0; findex < Freqnum; findex++)
		fcoefs[findex] = (float)2.0 * (float)cos(((float)2.0)*((float)M_PI)*freqs[findex] / (float)SAMPLERATE);


	for (findex = 0; findex < Freqnum; findex++) {
		fstate2[findex] = (float)1.0;
		fstate1[findex] = ((float)0.5) * fcoefs[findex];
	}

	// now randomize phases
	for (findex = 0; findex < Freqnum; findex++)
	{
		int j, rval;
		float sum, tempf;
		sum = 0;
		rval = rand();
		for (j = 0; j < rval; j++) {
			sum += tempf = fcoefs[findex] * fstate1[findex] - fstate2[findex];
			fstate2[findex] = fstate1[findex];
			fstate1[findex] = tempf;
		}
	}

	if (FrphTime > 0)//总时间小于4S!
	{
		int maxShortInt = 0;
		float SMR = 0.0;//生成的信号与最大值比
		//	float frs = SAMPLERATE * FrphTime;
		//因为Phase 测试发现，信号不是周期性的，即使错一个字节，有时候相位高频会大幅变化！
		//信号源改为每0.1S一个周期（最低10Hz），采样数据为0.2S (最低5Hz),理论上可以解决这个问题！
		float Svolue[SAMPLERATE *4];
		float S_FrphTime = FrphTime / 0.1;

		for (samplenum = 0; samplenum < (int)(SAMPLERATE * FrphTime); samplenum++) {
			register float tempf;
			register float sum;
			sum = (float)0.0;

			for (findex = 0; findex < Freqnum; findex++) {
				tempf = fcoefs[findex] * fstate1[findex] - fstate2[findex];

				if (InitAmp == false){//没有设置各频点幅值的话，使用默认处理，不单独处理各频点幅值。
					//2019-7-23 Q80   发现 20Hz低频不稳,提高信号源低频信号。
					if (freqs[findex] < (float) 50.00)    sum += ((float)3276.70*0.7079)*volue*tempf * 3.3;//提高10dB
					else if (freqs[findex] < (float)100.00) sum += ((float)3276.70*0.7079)*volue*tempf * 2;//提高6dB
					else
						sum += ((float)3276.70*0.7079)*volue*tempf;  // This is 1Khz value*0.707  //100Hz以上 不提高信号幅度
				}
				else
				{
					sum += ((float)3276.70*0.7079)*volue*tempf * SpeakerAmp[findex];  //根据定制设置各个频点的幅度
				}
				fstate2[findex] = fstate1[findex];
				fstate1[findex] = tempf;
			}
			if (fabs(sum) > maxShortInt)  maxShortInt = fabs(sum); //用绝对值!
			Svolue[samplenum] = sum;
		}
		//将数字信号的最大幅度值定为 （先衰减为-3dBFS 再乘以 信号需要的增益或衰减值）
		SMR = (float)(32767.0*0.7079) / maxShortInt;
		SMR = SMR * volue;

	//	for (int i = 0; i < (int)(S_FrphTime); i++){   //重复相同的周期数据用于测试
		for (samplenum = 0; samplenum < (int)(SAMPLERATE * FrphTime); samplenum++) {
			register float sum;
			sum = (float)SMR *Svolue[samplenum];//	sum = sum * (float)(150.0 / ((float)Freqnum));	//	sum = sum  / (float)Freqnum;
			intsum = (short)sum;
			fwrite(&intsum, 2, 1, outfile);	//printf("%d,\n", intsum);
			intsum = 0;
			fwrite(&intsum, 2, 1, outfile);	//printf("%d,\n", intsum);
		}
		//}

	}

	// put out samples of silence

	//将间隔时间从噪声测试部分减出来
	for (samplenum = 0; samplenum < ((SAMPLERATE *NoiseTime) - (SpecTime * 200)); samplenum++) {
		register float sum;
		sum = (float)0.0;
		intsum = (short)sum;
		fwrite(&intsum, 2, 1, outfile);
		//		printf("%d,\n", intsum);
		intsum = 0;
		fwrite(&intsum, 2, 1, outfile);
		//	printf("%d,\n", intsum);
	}

	fclose(outfile);

	// 信号位置点确定：ThdTime,SealTime, SensTime, FrphTime, NoiseTime, THDCalibTime
	// 信号长度要求：ThdTime>0.3,SealTime, SensTime, FrphTime>0.4, NoiseTime>0.1, THDCalibTime
	Dist_Start_Point = (int)SAMPLERATE*0.2;//信号0.2S以后的数据
	Dist_End_point = Dist_Start_Point + (int)SAMPLERATE*(ThdTime - 0.3);//取 （去掉开始0.2s以后到结束的0.1S以前的) 数据

	if (ThdTime > 0) SpecTime = SpecTime + 1;
	if (SealTime > 0) SpecTime = SpecTime + 1;
	if (SensTime > 0) SpecTime = SpecTime + 1;
	//取 （去掉开始0.3s以后到结束的0.4S以前的) 数据
	Freq_Start_Point = (int)(SAMPLERATE*(ThdTime + SealTime + SensTime + 0.3) + 200 * SpecTime);//信号0.3S以后的数据
	Freq_End_Point = Freq_Start_Point + (int)(SAMPLERATE*(FrphTime - 0.7));//去掉开始0.3S与结束的0.4S信号数据

	Noise_Start_Point = (int)(SAMPLERATE*(ThdTime + SealTime + SensTime + FrphTime + 0.1) + 200 * SpecTime);//信号0.1S以后的数据
	Noise_End_Point = Noise_Start_Point + (int)(SAMPLERATE*(NoiseTime - 0.2));// - 200 * SpecTime;

}


void processinit()  // call to process initialization
{
	int i = 0;

	//频率总数老是出错，
	//	int StartFreqInt = 0;
	//	int StopFreqInt = 0;
	float Ri = 10.0;
	//先赋值后使用，是为了只用具体值不使用表达式!便于以后变量的调整
	//默认为初始化定义的内容
	i = DIST_START;
	Dist_Start_Point = i;
	i = DIST_END;
	Dist_End_point = i;
	i = FREQ_START;
	Freq_Start_Point = i;
	i = FREQ_END;
	Freq_End_Point = i;
	i = NOISE_START;
	Noise_Start_Point = i;
	i = NOISE_END;
	Noise_End_Point = i;



	//FILE *fpWire = fopen("FreqPointM.txt", "w+");
	//fprintf(fpWire, "Start1%f\n",Ri);
	//fprintf(fpWire, "i,Freq, startin, Stopint,Ri ,SF,STPF \n");
	if (InitFreq == false){//如果频率点没有初始化，进行初始化
		// 寻找测试的起止频率
		for (i = 0; i < 400; i++)//从0开始计算400个频率点（兼容1/24 OCT）
		{
			float Freqt = 0;
			if (SweepType == 3) Ri = freqsR10[i];//R10
			if (SweepType == 6)	Ri = freqsR20[i];//R20
			if (SweepType == 12) Ri = freqsR40[i];//R40
			if (SweepType == 24) Ri = freqsR80[i];//R80

			if (Ri < StartFreq) //包含开始频率
			{
				StartFreqInt = i;//开始频率计算位置
			}
			freqs[i - StartFreqInt] = Ri;
			//fprintf(fpWire, "i: %i,Freq: %f, startint: %i, Stopint %i, SF %d, STPF %d\n", i, Ri, StartFreqInt, StopFreqInt,  StartFreq, StopFreq);

			if (Ri > StopFreq)//包含结束频率
			{
				StopFreqInt = i;//结束频率计算位置
				//fprintf(fpWire, "i: %i,Freq: %f, startint: %i, Stopint %i,SF %d, STPF %d\n", i, Ri, StartFreqInt, StopFreqInt, StartFreq, StopFreq);
				break;
			}

		}
		TotalFreqNum = StopFreqInt - StartFreqInt;
		InitFreq = true;//已经初始化
	}


	//fprintf(fpWire, "%d,%d End \n", StartFreqInt, StopFreqInt);
	////fprintf(fpWire, "fasdfasdfafadsfadsf\n");
	//fclose(fpWire);

	srand((unsigned)time(NULL));

	// set the frequency coeficient for the DFT frequencies used in test

	for (i = 0; i < TotalFreqNum; i++) {
		fcoefs[i] = ((float)2.0 * (float)cos(((float)2.0)*((float)MPI)*((float)freqs[i]) / ((float)SAMPLERATE)));
		icoefs[i] = (float)(sin(2.0 * MPI * freqs[i] / ((float)SAMPLERATE)));
		rcoefs[i] = ((float)0.5)* fcoefs[i];  // cosine values,
		frefstate2[i] = (float)0.0;
		frefstate1[i] = (float)0.0;
		fteststate11[i] = (float)0.0;
		fteststate21[i] = (float)0.0;
		fteststate12[i] = (float)0.0;
		fteststate22[i] = (float)0.0;
		fteststate13[i] = (float)0.0;
		fteststate23[i] = (float)0.0;
		noiserefstate2[i] = (float)0.0;
		noiserefstate1[i] = (float)0.0;
		noiseteststate11[i] = (float)0.0;
		noiseteststate21[i] = (float)0.0;
		noiseteststate12[i] = (float)0.0;
		noiseteststate22[i] = (float)0.0;
		noiseteststate13[i] = (float)0.0;
		noiseteststate23[i] = (float)0.0;
	}

	////THD  3次谐波 放到 信号生成 功能里进行处理
	//for (i = 0; i < TotalFreqNum; i++) {
	//	distcoefs[i] = ((float)2.0 * (float)cos(((float)2.0)*((float)MPI)*((float)THDFreq*(i + 1)) / ((float)SAMPLERATE)));
	//	rdistcoefs[i] = (float)cos(((float)2.0)*((float)MPI)*((float)THDFreq*(i + 1)) / ((float)SAMPLERATE));
	//	idistcoefs[i] = (float)sin(((float)2.0)*((float)MPI)*((float)THDFreq*(i + 1)) / ((float)SAMPLERATE));
	//}
	//		distcoefs[1] = ((float)2.0 * (float)cos(((float)2.0)*((float)MPI)*((float)200.0) / ((float)SAMPLERATE)));
	//		distcoefs[2] = ((float)2.0 * (float)cos(((float)2.0)*((float)MPI)*((float)300.0) / ((float)SAMPLERATE)));
	//		rdistcoefs[1] = (float)cos(((float)2.0)*((float)MPI)*((float)200.0) / ((float)SAMPLERATE));
	//		rdistcoefs[2] = (float)cos(((float)2.0)*((float)MPI)*((float)300.0) / ((float)SAMPLERATE));
	//		idistcoefs[1] = (float)sin(((float)2.0)*((float)MPI)*((float)200.0) / ((float)SAMPLERATE));
	//		idistcoefs[2] = (float)sin(((float)2.0)*((float)MPI)*((float)300.0) / ((float)SAMPLERATE));
	impcoef = ((float)2.0 * (float)cos(((float)2.0)*((float)MPI)*((float)1000.0) / ((float)SAMPLERATE)));
	rimpcoef = (float)cos(((float)2.0)*((float)MPI)*((float)1000.0) / ((float)SAMPLERATE));
	iimpcoef = (float)sin(((float)2.0)*((float)MPI)*((float)1000.0) / ((float)SAMPLERATE));

	input_index1 = 0;
	input_index2 = 0;
	input_index3 = 0;
	teststate = 0;
	energylevel = 5000;
}

short write_pcm_file = 0;

void processdata()
{
	//====================================================================================================================================================================
	// look at energy other  than 1Khz in the reference mic channel during 1st second and energy in the 1khz tone,  pick fist place where 1khz tone is most of the energy.
	//====================================================================================================================================================================
	{ register int i;
	tone = (float)0.0;
	enrgy = (float)0.0;
	state1 = (float)0.0;
	state2 = (float)0.0;
	stateA = (float)0.0;
	stateB = (float)0.0;
	startindex1 = 0;
	//register int WavIndex = 0;

	for (i = 0; i <= 2 * SAMPLERATE; i++) {
		refmicin1[i] = (float)left1[i];
		testmicin1[i] = (float)right1[i];
		testmicin2[i] = (float)left2[i];
		testmicin3[i] = (float)right2[i];
	}
	//使用哪个通道作为参考通道，防止不使用参考通道测试单只产品的情况
	if (SetMic)
	{//====================================================================================================================================================================
		// look at energy other  than 1Khz in the mic1 channel during 1st second and energy in the 1khz tone,  pick fist place where 1khz tone is most of the energy.
		//====================================================================================================================================================================
		//{ register int i;
		//    tone = (float)0.0;
		//	enrgy = (float)0.0;
		//	state1 = (float)0.0;
		//	state2 = (float)0.0;
		//	stateA = (float)0.0;
		//	stateB = (float)0.0;
		//	
		////	FILE *fpWrite = fopen("enrgy_Test.txt", "w");
		for (i = 0; i <= 2 * SAMPLERATE; i++) {
			fval = testmicin1[i] + state1 * distcoefs[0] * ((float)0.9) - state2 * ((float)0.81);     // Pole at 1Khz, not deep though
			state2 = state1;
			state1 = fval;

			tone += (fval*fval / ((float)100.0)) - tone;  // integrated 1Khz energy

			fval = testmicin1[i] - stateA * distcoefs[0] + stateB;  // Zero at 1Khz
			stateB = stateA;
			stateA = testmicin1[i];
			enrgy += fval * fval;  // total integrated energy
			tone = tone * (float)0.99;  // leakage
			enrgy = enrgy * (float)0.99;  // leakage
			if (tone > energylevel*(enrgy + (float)(100.0))) {  // 500 experimentally determined.
				//for Apple J5
				//startindex2 = i-480; 
				//for Bose ANC & VNC
				startindex1 = i;
				break;
			}
			//'	fprintf(fpWrite, "%f,%d,%f,%f \n", refmicin1[i], startindex1, tone, energylevel*(enrgy + (float)(100.0)));
			////		fprintf(fpWrite, "%f,%f,%f,%f,%f,%f y \n", refmicin1[i], fval, tone, enrgy, i, energylevel*(enrgy + (float)(100.0)));
		}
		////	fclose(fpWrite);
		//}
	}
	else
	{
		//FILE *fpWrite = fopen("enrgy_Ref.txt", "w");
		////'	fprintf(fpWrite, "%f,%d,%f,%f \n", refmicin1[i], startindex1, tone, energylevel*(enrgy + (float)(100.0)));
		for (i = 0; i <= SAMPLERATE; i++)
		{
			fval = refmicin1[i] + state1 * distcoefs[0] * ((float)0.9) - state2 * ((float)0.81);     // Pole at 100hz, not deep though
			state2 = state1;
			state1 = fval;

			tone += (fval*fval / ((float)1000.0)) - tone;  // integrated 1Khz energy		//tone = (fval*fval);
			fval = refmicin1[i] - stateA * distcoefs[0] + stateB;  // Zero at 1Khz
			stateB = stateA;
			stateA = refmicin1[i];
			enrgy += fval * fval;  // total integrated energy

			tone = tone * (float)0.99;  // leakage
			enrgy = enrgy * (float)0.99;  // leakage
			//WavIndex = WavIndex +1 ;
			//if (tone > ((float)500)*(enrgy + (float)(100.0))) {  // 5000, 100000 experimentally determined.
			if (tone > energylevel*(enrgy + (float)(100.0)))   // 500 experimentally determined.break;
			{
				//for Apple J5
				//startindex1 = i-480; 
				//for Bose ANC & VNC
				//	fprintf(fpWrite, "%f \n", startindex1);
				startindex1 = i;
				break;
			}
		//	//'	fprintf(fpWrite, "%f,%d,%f,%f \n", refmicin1[i], startindex1, tone, energylevel*(enrgy + (float)(100.0)));
		//	fprintf(fpWrite, "%f,%f,%f,%f,%f,%f y \n", refmicin1[i], fval, tone, enrgy, i, energylevel*(enrgy + (float)(100.0)));
		}
	//	fclose(fpWrite);
	}
	}

	//====================================================================================================================================================================
	// look at energy other  than 1Khz in the mic1 channel during 1st second and energy in the 1khz tone,  pick fist place where 1khz tone is most of the energy.
	//====================================================================================================================================================================
	//{ register int i;
	//    tone = (float)0.0;
	//	enrgy = (float)0.0;
	//	state1 = (float)0.0;
	//	state2 = (float)0.0;
	//	stateA = (float)0.0;
	//	stateB = (float)0.0;
	//	
	//	for (i = 0; i <= 2*SAMPLERATE; i++) {  
	//  			fval=testmicin1[i]+state1*distcoefs[0]*((float)0.9) - state2*((float)0.81);     // Pole at 1Khz, not deep though
	//		state2=state1;
	//		state1=fval;

	//		tone += (fval*fval/((float)100.0)) - tone;  // integrated 1Khz energy
	//
	//		fval = testmicin1[i] - stateA*distcoefs[0] + stateB;  // Zero at 1Khz
	//		stateB = stateA;
	//		stateA = testmicin1[i];
	//		enrgy +=fval*fval;  // total integrated energy
	//		tone  = tone * (float)0.99;  // leakage
	//		enrgy = enrgy * (float)0.99;  // leakage
	//		if (tone > energylevel*(enrgy+(float)(100.0))) {  // 500 experimentally determined.
	//			//for Apple J5
	//			//startindex2 = i-480; 
	//			//for Bose ANC & VNC
	//			startindex1 = i; 
	//			break;
	//		}
	//	}
	//}
	//====================================================================================================================================================================
	// look at energy other  than 1Khz in the mic2 channel during 1st second and energy in the 1khz tone,  pick fist place where 1khz tone is most of the energy.
	//====================================================================================================================================================================
	/*	{ register int i;
			tone = (float)0.0;
			enrgy = (float)0.0;
			state1 = (float)0.0;
			state2 = (float)0.0;
			stateA = (float)0.0;
			stateB = (float)0.0;

			for (i = 0; i <= 2*SAMPLERATE; i++) {
			fval=testmicin2[i]+state1*distcoefs[0]*((float)0.9) - state2*((float)0.81);     // Pole at 1Khz, not deep though
			state2=state1;
			state1=fval;

			tone += (fval*fval/((float)100.0)) - tone;  // integrated 1Khz energy

			fval = testmicin2[i] - stateA*distcoefs[0] + stateB;  // Zero at 1Khz
			stateB = stateA;
			stateA = testmicin2[i];

			enrgy +=fval*fval;  // total integrated energy

			tone  = tone * (float)0.99;  // leakage
			enrgy = enrgy * (float)0.99;  // leakage

			if (tone > energylevel*(enrgy+(float)(100.0))) {  // 500 experimentally determined.
			//for Apple J5
			//startindex2 = i-480;

			//for Bose ANC & VNC
			startindex2 = i;
			break;
			}
			}
			}
			*/
	//====================================================================================================================================================================
	// look at energy other  than 1Khz in the mic3 channel during 1st second and energy in the 1khz tone,  pick fist place where 1khz tone is most of the energy.
	//====================================================================================================================================================================
	/*	{ register int i;
			tone = (float)0.0;
			enrgy = (float)0.0;
			state1 = (float)0.0;
			state2 = (float)0.0;
			stateA = (float)0.0;
			stateB = (float)0.0;
			for (i = 0; i <= 2*SAMPLERATE; i++) {
			fval=testmicin3[i]+state1*distcoefs[0]*((float)0.9) - state2*((float)0.81);     // Pole at 1Khz, not deep though
			state2=state1;
			state1=fval;

			tone += (fval*fval/((float)100.0)) - tone;  // integrated 1Khz energy

			fval = testmicin3[i] - stateA*distcoefs[0] + stateB;  // Zero at 1Khz
			stateB = stateA;
			stateA = testmicin3[i];

			enrgy +=fval*fval;  // total integrated energy

			tone  = tone * (float)0.99;  // leakage
			enrgy = enrgy * (float)0.99;  // leakage

			if (tone > energylevel*(enrgy+(float)(100.0))) {  // 500 experimentally determined.
			//for Apple J5
			//startindex3 = i-480;

			//for Bose ANC & VNC
			startindex3 = i;
			break;
			}
			}
			}
			*/
	//====================================================================================================================================================================
	// Align Mic2 startindex2 with RefMic startindex1.
	//====================================================================================================================================================================
	/*	{ register int i, j, k;
			register int newstartindex2;
			register float sumfreqprod;
			register float summax;
			summax = (float)0.0;
			indexoffset = 0;
			newstartindex2 = startindex2;
			for (k = - COROFFSET * 2; k <= 0; k++) {
			sumfreqprod = (float)0.0;
			j = startindex2 + FREQ_START + k;
			for (i = startindex1 + FREQ_START - COROFFSET; i < startindex1 + FREQ_START + CORLENGTH - COROFFSET; i++) {
			sumfreqprod = sumfreqprod + refmicin1[i] * testmicin2[j];
			j++;
			}
			if (sumfreqprod > summax) {
			summax = sumfreqprod;
			indexoffset = k + COROFFSET;
			newstartindex2 = startindex2 + k + COROFFSET;
			}
			}
			startindex2 = newstartindex2;
			}
			*/
	//====================================================================================================================================================================
	// Align Mic3 startindex3 with RefMic startindex1.
	//====================================================================================================================================================================
	/*	{ register int i, j, k;
			register int newstartindex3;
			register float sumfreqprod;
			register float summax;
			summax = (float)0.0;
			indexoffset = 0;
			newstartindex3 = startindex3;
			for (k = - COROFFSET * 2; k <= 0; k++) {
			sumfreqprod = (float)0.0;
			j = startindex3 + FREQ_START + k;
			for (i = startindex1 + FREQ_START - COROFFSET; i < startindex1 + FREQ_START + CORLENGTH - COROFFSET; i++) {
			sumfreqprod = sumfreqprod + refmicin1[i] * testmicin3[j];
			j++;
			}
			if (sumfreqprod > summax) {
			summax = sumfreqprod;
			indexoffset = k + COROFFSET;
			newstartindex3 = startindex3 + k + COROFFSET;
			}
			}
			startindex3 = newstartindex3;
			}
			*/
	//====================================================================================================================================================================
	// Ouput PCM Files
	//====================================================================================================================================================================
	if (write_pcm_file) {
		
		strcpy(filename, "");
		strcat(filename, DirectoryPath);
		strcat(filename, "pcmoutrawdata-ref-mic1.raw");
		//MessageBoxA(NULL, filename, "Output Files", MB_OK | MB_ICONSTOP);
		outfile = fopen(filename, "wb");
		if (outfile != NULL) {
			long i, j;
			for (i = 0, j = 0; i < STIMLEN * SAMPLERATE; i++) {
				pcmout[j++] = (short)refmicin1[i];  // left
				pcmout[j++] = (short)testmicin1[i];  //right
			}
			fwrite(pcmout, j, 2, outfile);
			fclose(outfile);
		}

		strcpy(filename, "");
		strcat(filename, DirectoryPath);
		strcat(filename, "pcmout-1.raw");
		outfile = fopen(filename, "wb");
		if (outfile != NULL) {
			long i, j;
			for (i = 0, j = 0; i < startindex1; i++) {
				pcmout[j++] = (short)refmicin1[i];  // left
				pcmout[j++] = (short)testmicin1[i];  //right
			}
			fwrite(pcmout, j, 2, outfile);
		}
		fclose(outfile);

		strcpy(filename, "");
		strcat(filename, DirectoryPath);
		strcat(filename, "pcmout-ref-mic1.raw");
		outfile = fopen(filename, "wb");
		if (outfile != NULL) {
			long i, j;
			for (i = startindex1, j = 0; i < STIMLEN * SAMPLERATE; i++) {
				pcmout[j++] = (short)refmicin1[i];  // left
				pcmout[j++] = (short)testmicin1[i];  //right
			}
			fwrite(pcmout, j, 2, outfile);
			fclose(outfile);
		}


		/*
		strcpy(filename,"");
		strcat(filename, DirectoryPath);
		strcat(filename, "pcmout-ref-mic2.raw");
		outfile = fopen(filename,"wb");
		if (outfile != NULL) {
		long i,j, k;
		k = startindex1;
		for (i = startindex2, j=0; i < 2*SAMPLERATE; i++) {
		pcmout[j++] = (short) refmicin1[k];  // left
		pcmout[j++] = (short) testmicin2[i];  //right
		k++;
		}
		fwrite (pcmout,j,2,outfile);
		fclose(outfile);
		}
		strcpy(filename,"");
		strcat(filename, DirectoryPath);
		strcat(filename, "pcmout-ref-mic3.raw");
		outfile = fopen(filename,"wb");
		if (outfile != NULL) {
		long i,j, k;
		k = startindex1;
		for (i = startindex3, j=0; i < 2*SAMPLERATE; i++) {
		pcmout[j++] = (short) refmicin1[k];  // left
		pcmout[j++] = (short) testmicin3[i];  //right
		k++;
		}
		fwrite (pcmout,j,2,outfile);
		fclose(outfile);
		}
		*/

		strcpy(filename, "");
		strcat(filename, DirectoryPath);
		strcat(filename, "pcmoutfreq-ref-mic1.raw");
		outfile = fopen(filename, "wb");
		if (outfile != NULL) {
			long i, j;
			//for (i = startindex1 + FREQ_START, j = 0; i < startindex1 + FREQ_END; i++) {
			for (i = startindex1 + Freq_Start_Point, j = 0; i < startindex1 + Freq_End_Point; i++) {
				pcmout[j++] = (short)refmicin1[i];  // left
				pcmout[j++] = (short)testmicin1[i];  //right
			}
			fwrite(pcmout, j, 2, outfile);
			fclose(outfile);
		}

		/*
		strcpy(filename,"");
		strcat(filename, DirectoryPath);
		strcat(filename, "pcmoutfreq-ref-mic2.raw");
		outfile = fopen(filename,"wb");
		if (outfile != NULL) {
		long i,j, k;
		k = startindex1 + FREQ_START;
		for (i = startindex2 + FREQ_START, j = 0; i < startindex2 + FREQ_END ; i++) {
		pcmout[j++] = (short) refmicin1[k];  // left
		pcmout[j++] = (short) testmicin2[i];  //right
		k++;
		}
		fwrite (pcmout,j,2,outfile);
		fclose(outfile);
		}

		strcpy(filename,"");
		strcat(filename, DirectoryPath);
		strcat(filename, "pcmoutfreq-ref-mic3.raw");
		outfile = fopen(filename,"wb");
		if (outfile != NULL) {
		long i,j, k;
		k = startindex1 + FREQ_START;
		for (i = startindex3 + FREQ_START, j = 0; i < startindex3 + FREQ_END ; i++) {
		pcmout[j++] = (short) refmicin1[k];  // left
		pcmout[j++] = (short) testmicin3[i];  //right
		k++;
		}
		fwrite (pcmout,j,2,outfile);
		fclose(outfile);
		}

		*/

		strcpy(filename, "");
		strcat(filename, DirectoryPath);
		strcat(filename, "pcmoutnoise-ref-mic1.raw");
		outfile = fopen(filename, "wb");
		if (outfile != NULL) {
			long i, j, k;
			k = startindex1 + Freq_Start_Point; //k = startindex1 + FREQ_START;
			//	for (i = startindex1 + NOISE_START, j = 0; i < startindex1 + NOISE_END; i++) {
			for (i = startindex1 + Noise_Start_Point, j = 0; i < startindex1 + Noise_End_Point; i++) {
				pcmout[j++] = (short)refmicin1[i];  // left
				pcmout[j++] = (short)testmicin1[i];  //right
				k++;
			}
			fwrite(pcmout, j, 2, outfile);
			fclose(outfile);
		}


		strcpy(filename, "");
		strcat(filename, DirectoryPath);
		strcat(filename, "pcmoutdist-ref-mic1.raw");
		outfile = fopen(filename, "wb");
		if (outfile != NULL) {
			long i, j, k;
			k = startindex1 + Freq_Start_Point; //k = startindex1 + FREQ_START;
			//for (i = startindex1 + DIST_START, j = 0; i < startindex1 + DIST_END; i++) {
			for (i = startindex1 + Dist_Start_Point, j = 0; i < startindex1 + Dist_End_point; i++) {
				pcmout[j++] = (short)refmicin1[i];  // left
				pcmout[j++] = (short)testmicin1[i];  //right
				k++;
			}
			fwrite(pcmout, j, 2, outfile);
			fclose(outfile);
		}

		//strcpy(filename,"");
		//strcat(filename, DirectoryPath);
		//strcat(filename, "pcmoutimp-ref-mic1.raw");
		//outfile = fopen(filename,"wb");
		//if (outfile != NULL) {
		//	long i,j, k;
		//	k = startindex1 + FREQ_START;
		//	for (i = startindex1 + IMP_START, j = 0; i < startindex1 + IMP_END ; i++) {  
		//		pcmout[j++] = (short) refmicin1[i];  // left
		//		pcmout[j++] = (short) testmicin1[i];  //right
		//		k++;
		//	}
		//	fwrite (pcmout,j,2,outfile);
		//	fclose(outfile);
		//}


	}



	//====================================================================================================================================================================
	// TestMic
	// do 1Khz, 2Khz and 3Khz DFT's on channels for distortion, starting at "startindex"
	//====================================================================================================================================================================

	{
		register int i, j;
		register float fval;
		for (j = 0; j < 10; j++) {//11次谐波
			//	for (i = startindex1 + DIST_START, diststate1[j] = diststate2[j] = (float)0.0; i < startindex1 + DIST_END; i++) {
			for (i = startindex1 + Dist_Start_Point, diststate1[j] = diststate2[j] = (float)0.0; i < startindex1 + Dist_End_point; i++) {
				fval = testmicin1[i] + diststate1[j] * distcoefs[j] - diststate2[j];
				diststate2[j] = diststate1[j];
				diststate1[j] = fval;
			}// i
		}// j
	}  // end of block

	{
		int j;
		float fval;
		for (j = 0; j < 3; j++) {
			rval = rdistcoefs[j] * diststate1[j] - diststate2[j]; //cos* svi1 -  svi2
			ival = diststate1[j] * idistcoefs[j];  // sin * svi1
			fval = rval * rval + ival * ival;
			disttest[j] = fval;
		}   // mvals now have average energy in them
	}

	//====================================================================================================================================================================
	// RefMic
	// do 1Khz, 2Khz and 3Khz DFT's on channels for distortion, starting at "startindex"
	//====================================================================================================================================================================

	{
		register int i, j;
		register float fval;
		for (j = 0; j < 10; j++) {//11次谐波
			//	for (i = startindex1 + DIST_START, diststate1[j] = diststate2[j] = (float)0.0; i < startindex1 + DIST_END; i++) {
			for (i = startindex1 + Dist_Start_Point, DistRefState1[j] = DistRefState2[j] = (float)0.0; i < startindex1 + Dist_End_point; i++) {
				fval = refmicin1[i] + DistRefState1[j] * DistRefCoefs[j] - DistRefState2[j];
				DistRefState2[j] = DistRefState1[j];
				DistRefState1[j] = fval;
			}// i
		}// j
	}  // end of block

	{
		int j;
		float fval;
		for (j = 0; j < 3; j++) {
			rval = RDistRefCoefs[j] * DistRefState1[j] - DistRefState2[j]; //cos* svi1 -  svi2
			ival = DistRefState1[j] * IDistRefcoefs[j];  // sin * svi1
			fval = rval * rval + ival * ival;
			DistRefTest[j] = fval;
		}   // mvals now have average energy in them
	}

	//====================================================================================================================================================================
	// end - do 1Khz, 2Khz and 3Khz DFT's on channels for distortion, starting at "startindex"
	//====================================================================================================================================================================

	// do the 1Khz impedance on the right channel


	//{ register int i;
	//  register float fval;	
	//	for (i = startindex1+IMP_START, impstate1=impstate2=(float)0.0; i < startindex1+IMP_END;i++) {  
	//   		fval=testmicin1[i]+impstate1*impcoef - impstate2;
	//		impstate2=impstate1;
	//		impstate1=fval;
	//	}// i
	//
	//	rval = rimpcoef*impstate1 - impstate2; //cos* svi1 -  svi2
	//	ival = impstate1 * iimpcoef;  // sin * svi1
	//	fval = rval * rval + ival * ival;
	//	imptest= fval;
	//}  // end of block

	//====================================================================================================================================================================
	/* do dft on input ref mic buffer using single pole filters on unit circle */
	//====================================================================================================================================================================
	{ register int i, j;
	float fval;
	for (j = 0; j < TotalFreqNum; j++) {
		//	for (i = startindex1 + FREQ_START; i < startindex1 + FREQ_END; i++) {
		for (i = startindex1 + Freq_Start_Point; i < startindex1 + Freq_End_Point; i++) {
			fval = refmicin1[i] + frefstate1[j] * fcoefs[j] - frefstate2[j];
			frefstate2[j] = frefstate1[j];
			frefstate1[j] = fval;
		}// j
	}// i
	}  // end of block

	//====================================================================================================================================================================
	/* do dft on input mic1 buffer using single pole filters on unit circle */
	//====================================================================================================================================================================
	{ register int i, j;
	float fval;
	for (j = 0; j < TotalFreqNum; j++) {
		//for (i = startindex1 + FREQ_START; i < startindex1 + FREQ_END; i++) {
		for (i = startindex1 + Freq_Start_Point; i < startindex1 + Freq_End_Point; i++) {
			fval = testmicin1[i] + fteststate11[j] * fcoefs[j] - fteststate21[j];
			fteststate21[j] = fteststate11[j];
			fteststate11[j] = fval;
		}// j
	}// i
	}  // release i,j

	//====================================================================================================================================================================
	/* do dft on input mic2 buffer using single pole filters on unit circle */
	//====================================================================================================================================================================
	{ register int i, j;
	float fval;
	for (j = 0; j < TotalFreqNum; j++) {
		//for (i = startindex2 + FREQ_START; i < startindex2 + FREQ_END; i++) {
		for (i = startindex2 + Freq_Start_Point; i < startindex2 + Freq_End_Point; i++) {
			fval = testmicin2[i] + fteststate12[j] * fcoefs[j] - fteststate22[j];
			fteststate22[j] = fteststate12[j];
			fteststate12[j] = fval;
		}// j
	}// i
	}  // release i,j

	//====================================================================================================================================================================
	/* do dft on input mic3 buffer using single pole filters on unit circle */
	//====================================================================================================================================================================
	{ register int i, j;
	float fval;
	for (j = 0; j < TotalFreqNum; j++) {
		//for (i = startindex3 + FREQ_START; i < startindex3 + FREQ_END; i++) {
		for (i = startindex3 + Freq_Start_Point; i < startindex3 + Freq_End_Point; i++) {
			fval = testmicin3[i] + fteststate13[j] * fcoefs[j] - fteststate23[j];
			fteststate23[j] = fteststate13[j];
			fteststate13[j] = fval;
		}// j
	}// i
	}  // release i,j


	//====================================================================================================================================================================
	// now calculate and 1st order lpf energy in each freq for this buffer
	//====================================================================================================================================================================
	{ register short j;
	register float fval;

	for (j = 0; j < TotalFreqNum; j++) {
		rval = rcoefs[j] * frefstate1[j] - frefstate2[j]; //cos* svi1 -  svi2
		ival = frefstate1[j] * icoefs[j];  // sin * svi1
		fval = rval * rval + ival * ival;
		mvalref[j] = fval;
		Rvalref[j] = rval;
		Ivalref[j] = ival;
	}

	for (j = 0; j < TotalFreqNum; j++) {
		rval = rcoefs[j] * fteststate11[j] - fteststate21[j]; //cos* svi1 -  svi2
		ival = fteststate11[j] * icoefs[j];  // sin * svi1
		fval = rval * rval + ival * ival;
		mvaltest1[j] = fval;
		Rvaltest1[j] = rval;
		Ivaltest1[j] = ival;
	}

	for (j = 0; j < TotalFreqNum; j++) {
		rval = rcoefs[j] * fteststate12[j] - fteststate22[j]; //cos* svi1 -  svi2
		ival = fteststate12[j] * icoefs[j];  // sin * svi1
		fval = rval * rval + ival * ival;
		mvaltest2[j] = fval;
		Rvaltest2[j] = rval;
		Ivaltest2[j] = ival;
	}

	for (j = 0; j < TotalFreqNum; j++) {
		rval = rcoefs[j] * fteststate13[j] - fteststate23[j]; //cos* svi1 -  svi2
		ival = fteststate13[j] * icoefs[j];  // sin * svi1
		fval = rval * rval + ival * ival;
		mvaltest3[j] = fval;
		Rvaltest3[j] = rval;
		Ivaltest3[j] = ival;
	}

	// mvals now have average energy in them
	// now clear out stuff for next buffer

	for (j = 0; j < TotalFreqNum; j++) {
		frefstate1[j] = (float)0.0;
		frefstate2[j] = (float)0.0;
		fteststate11[j] = (float)0.0;
		fteststate21[j] = (float)0.0;
		fteststate12[j] = (float)0.0;
		fteststate22[j] = (float)0.0;
		fteststate13[j] = (float)0.0;
		fteststate23[j] = (float)0.0;
	}

	}  // release j

	//====================================================================================================================================================================
	//  end - now calculate and 1st order lpf energy in each freq for this buffer
	//====================================================================================================================================================================

	//====================================================================================================================================================================
	// Now normalize and calculate Logs of energy
	//====================================================================================================================================================================
	{
		register short i;
		for (i = 0; i < TotalFreqNum; i++) {
			LPdBrefbuffer[i] = dBrefbuffer[i] = ((float)10.0) * (float)log10((double)mvalref[i]);
			LPdBtestbuffer1[i] = dBtestbuffer1[i] = ((float)10.0) * (float)log10((double)mvaltest1[i]);

			if (N_CHANNELS > 2) {
				LPdBtestbuffer2[i] = dBtestbuffer2[i] = ((float)10.0) * (float)log10((double)mvaltest2[i]);
			}
			if (N_CHANNELS > 3) {
				LPdBtestbuffer3[i] = dBtestbuffer3[i] = ((float)10.0) * (float)log10((double)mvaltest3[i]);
			}

			if (Rvalref[i] > (float)0.0) {
				Phaseref[i] = (float)(((180.0 / MPI)*atan((double)(Ivalref[i] / Rvalref[i]))));
			}
			else {
				Phaseref[i] = (float)(((180.0 / MPI)*atan((double)(Ivalref[i] / Rvalref[i])) + 180.0));
			}
			if (Rvaltest1[i] > (float)0.0) {
				Phasetest1[i] = (float)(((180.0 / MPI)*atan((double)(Ivaltest1[i] / Rvaltest1[i]))));
			}
			else {
				Phasetest1[i] = (float)(((180.0 / MPI)*atan((double)(Ivaltest1[i] / Rvaltest1[i])) + 180.0));
			}
			if (Rvaltest2[i] > (float)0.0) {
				Phasetest2[i] = (float)(((180.0 / MPI)*atan((double)(Ivaltest2[i] / Rvaltest2[i]))));
			}
			else {
				Phasetest2[i] = (float)(((180.0 / MPI)*atan((double)(Ivaltest2[i] / Rvaltest2[i])) + 180.0));
			}
			if (Rvaltest3[i] > (float)0.0) {
				Phasetest3[i] = (float)(((180.0 / MPI)*atan((double)(Ivaltest3[i] / Rvaltest3[i]))));
			}
			else {
				Phasetest3[i] = (float)(((180.0 / MPI)*atan((double)(Ivaltest3[i] / Rvaltest3[i])) + 180.0));
			}
		} // for loop
	}


	// Do Phase unwrapping on Phasetest[i] and PhaseRef[i] signals here instead of in VB.   
	//Because this C code returns the difference between the two, the phase unwrapping
	// must be done before any math on the phase, such as difference between ref mic and test mic

	{
		float lastphase1, lastphase2, lastphase3, lastphaseref = (float)0.0;
		float tempphase1, tempphase2, tempphase3, tempphaseref = (float)0.0;
		float phasewrap1 = (float)0.0;
		float phasewrap2 = (float)0.0;
		float phasewrap3 = (float)0.0;
		float  phasewrapref = (float)0.0;
		register short i;

		lastphaseref = Phaseref[0];
		lastphase1 = Phasetest1[0];
		lastphase2 = Phasetest2[0];
		lastphase3 = Phasetest3[0];

		for (i = 1; i < TotalFreqNum; i++) {  // 
			tempphaseref = Phaseref[i];
			if (tempphaseref >(float)90.0 && lastphaseref < (float)-90.0) {
				phasewrapref += -1;

			}
			if (tempphaseref < (float)-90.0 && lastphaseref >(float)90.0) {
				phasewrapref += 1;

			}
			Phaseref[i] = tempphaseref + ((float)360)* phasewrapref;
			lastphaseref = tempphaseref;

			tempphase1 = Phasetest1[i];
			if (tempphase1 > (float)90.0 && lastphase1 < (float)-90.0) {
				phasewrap1 += (float)-.0;

			}
			if (tempphase1 < (float)-90.0 && lastphase1 >(float)90.0) {
				phasewrap1 += (float)1.0;

			}
			Phasetest1[i] = tempphase1 + ((float)360)* phasewrap1;
			lastphase1 = tempphase1;

			tempphase2 = Phasetest2[i];
			if (tempphase2 > (float)90.0 && lastphase2 < (float)-90.0) {
				phasewrap2 += (float)-.0;

			}
			if (tempphase2 < (float)-90.0 && lastphase2 >(float)90.0) {
				phasewrap2 += (float)1.0;

			}
			Phasetest2[i] = tempphase2 + ((float)360)* phasewrap2;
			lastphase2 = tempphase2;


			tempphase3 = Phasetest3[i];
			if (tempphase3 > (float)90.0 && lastphase3 < (float)-90.0) {
				phasewrap3 += (float)-.0;

			}
			if (tempphase3 < (float)-90.0 && lastphase3 >(float)90.0) {
				phasewrap3 += (float)1.0;

			}
			Phasetest3[i] = tempphase3 + ((float)360)* phasewrap3;
			lastphase3 = tempphase3;

		}// next i
	}  // done phase unwrapping




	// Do A weighted noise

	//* do dft on input ref mic buffer using single pole filters on unit circle */
	{ register int i, j;
	register float fval;
	for (j = 0; j < TotalFreqNum; j++) {
		//for (i = startindex1 + NOISE_START; i < startindex1 + NOISE_END; i++) {
		for (i = startindex1 + Noise_Start_Point; i < startindex1 + Noise_End_Point; i++) {
			fval = refmicin1[i] + noiserefstate1[j] * fcoefs[j] - noiserefstate2[j];
			noiserefstate2[j] = noiserefstate1[j];
			noiserefstate1[j] = fval;
		}// j
	}// i
	}  // end of block

	//* do dft on input test mic1 buffer using single pole filters on unit circle */
	if (N_CHANNELS == 2) {
		{ register int i, j;
		float fval;
		for (j = 0; j < TotalFreqNum; j++) {
			//for (i = startindex1 + NOISE_START; i < startindex1 + NOISE_END; i++) {
			for (i = startindex1 + Noise_Start_Point; i < startindex1 + Noise_End_Point; i++) {
				fval = testmicin1[i] + noiseteststate11[j] * fcoefs[j] - noiseteststate21[j];
				noiseteststate21[j] = noiseteststate11[j];
				noiseteststate11[j] = fval;
			}// j
		}// i
		}  // release i,j
	}

	//* do dft on input test mic2 buffer using single pole filters on unit circle */
	if (N_CHANNELS == 3) {
		{ register int i, j;
		float fval;
		for (j = 0; j < TotalFreqNum; j++) {
			//for (i = startindex2 + NOISE_START; i < startindex2 + NOISE_END; i++) {
			for (i = startindex2 + Noise_Start_Point; i < startindex2 + Noise_End_Point; i++) {
				fval = testmicin2[i] + noiseteststate12[j] * fcoefs[j] - noiseteststate22[j];
				noiseteststate22[j] = noiseteststate12[j];
				noiseteststate12[j] = fval;
			}// j
		}// i
		}  // release i,j
	}
	//* do dft on input test mic3 buffer using single pole filters on unit circle */
	if (N_CHANNELS == 4) {
		{ register int i, j;
		float fval;
		for (j = 0; j < TotalFreqNum; j++) {
			//for (i = startindex3 + NOISE_START; i < startindex3 + NOISE_END; i++) {
			for (i = startindex3 + Noise_Start_Point; i < startindex3 + Noise_End_Point; i++) {
				fval = testmicin3[i] + noiseteststate13[j] * fcoefs[j] - noiseteststate23[j];
				noiseteststate23[j] = noiseteststate13[j];
				noiseteststate13[j] = fval;
			}// j
		}// i
		}  // release i,j
	}

	{ register short j;
	register float fval;
	// now calculate  energy in each freq for this buffer (reference buffer)
	for (j = 0; j < TotalFreqNum; j++) {
		rval = rcoefs[j] * noiserefstate1[j] - noiserefstate2[j]; //cos* svi1 -  svi2
		ival = noiserefstate1[j] * icoefs[j];  // sin * svi1
		fval = rval * rval + ival * ival;
		noisevalref[j] = fval;
	}

	// now calculate  energy in each freq for this buffer (test mic1 buffer)
	if (N_CHANNELS == 2) {
		for (j = 0; j < TotalFreqNum; j++) {
			rval = rcoefs[j] * noiseteststate11[j] - noiseteststate21[j]; //cos* svi1 -  svi2
			ival = noiseteststate11[j] * icoefs[j];  // sin * svi1
			fval = rval * rval + ival * ival;
			noisevaltest1[j] = fval;
		}
	}
	// now calculate  energy in each freq for this buffer (test mic2 buffer)
	if (N_CHANNELS == 3) {
		for (j = 0; j < TotalFreqNum; j++) {
			rval = rcoefs[j] * noiseteststate12[j] - noiseteststate22[j]; //cos* svi1 -  svi2
			ival = noiseteststate12[j] * icoefs[j];  // sin * svi1
			fval = rval * rval + ival * ival;
			noisevaltest2[j] = fval;
		}
	}
	// now calculate  energy in each freq for this buffer (test mic3 buffer)
	if (N_CHANNELS == 4) {
		for (j = 0; j < TotalFreqNum; j++) {
			rval = rcoefs[j] * noiseteststate13[j] - noiseteststate23[j]; //cos* svi1 -  svi2
			ival = noiseteststate13[j] * icoefs[j];  // sin * svi1
			fval = rval * rval + ival * ival;
			noisevaltest3[j] = fval;
		}
	}
	// mvals now have average energy in them
	// now clear out stuff for next buffer
	for (j = 0; j < TotalFreqNum; j++) {
		noiserefstate1[j] = (float)0.0;
		noiserefstate2[j] = (float)0.0;
		noiseteststate11[j] = (float)0.0;
		noiseteststate21[j] = (float)0.0;
		noiseteststate12[j] = (float)0.0;
		noiseteststate22[j] = (float)0.0;
		noiseteststate13[j] = (float)0.0;
		noiseteststate23[j] = (float)0.0;
	}

	}  // release j


	{
		register short i;
		anoiseref = (float)0.0;
		anoisetest1 = (float)0.0;
		anoisetest2 = (float)0.0;
		anoisetest3 = (float)0.0;
		for (i = 0; i < TotalFreqNum; i++) {
			dBnoiserefbuffer[i] = ((float)10.0) * (float)log10((double)noisevalref[i] + 0.000001);
			dBnoisetestbuffer1[i] = ((float)10.0) * (float)log10((double)noisevaltest1[i] + 0.000001);
			dBnoisetestbuffer2[i] = ((float)10.0) * (float)log10((double)noisevaltest2[i] + 0.000001);
			dBnoisetestbuffer3[i] = ((float)10.0) * (float)log10((double)noisevaltest3[i] + 0.000001);
			dBnoiserefbuffer[i] += aweightings[i];
			dBnoisetestbuffer1[i] += aweightings[i];
			dBnoisetestbuffer2[i] += aweightings[i];
			dBnoisetestbuffer3[i] += aweightings[i];
			anoiseref += (float)pow(10.0, (double)(dBnoiserefbuffer[i] / ((float)10.0)));
			anoisetest1 += (float)pow(10.0, (double)(dBnoisetestbuffer1[i] / ((float)10.0)));
			anoisetest2 += (float)pow(10.0, (double)(dBnoisetestbuffer2[i] / ((float)10.0)));
			anoisetest3 += (float)pow(10.0, (double)(dBnoisetestbuffer3[i] / ((float)10.0)));
		} // for loop
		Noiseref = (float)(10.0*log10((double)(anoiseref)));
		Noisetest1 = (float)(10.0*log10((double)(anoisetest1)));
		Noisetest2 = (float)(10.0*log10((double)(anoisetest2)));
		Noisetest3 = (float)(10.0*log10((double)(anoisetest3)));
	} // release i

	//reset all for next time

	input_index1 = 0;
	input_index2 = 0;
	input_index3 = 0;
	{ long i;

	for (i = 0; i < TotalFreqNum; i++) {
		frefstate2[i] = (float)0.0;
		frefstate1[i] = (float)0.0;
		fteststate11[i] = (float)0.0;
		fteststate21[i] = (float)0.0;
		fteststate12[i] = (float)0.0;
		fteststate22[i] = (float)0.0;
		fteststate13[i] = (float)0.0;
		fteststate23[i] = (float)0.0;
		noiserefstate2[i] = (float)0.0;
		noiserefstate1[i] = (float)0.0;
		noiseteststate11[i] = (float)0.0;
		noiseteststate21[i] = (float)0.0;
		noiseteststate12[i] = (float)0.0;
		noiseteststate22[i] = (float)0.0;
		noiseteststate13[i] = (float)0.0;
		noiseteststate23[i] = (float)0.0;
	}
	for (i = 0; i < 384000; i++) {
		left1[i] = (short)0.0;  //left
		left2[i] = (short)0.0;  //left
		right1[i] = (short)0.0;  //right
		right2[i] = (short)0.0;  //right
		refmicin1[i] = (float)0.0;  //left
		refmicin2[i] = (float)0.0;  //left
		refmicin3[i] = (float)0.0;  //left
		testmicin1[i] = (float) 0.0;  //right
		testmicin2[i] = (float) 0.0;  //right
		testmicin3[i] = (float) 0.0;  //right
	}  // clear buffers for next test.

	}// release  long i
	teststate = 0;
}
