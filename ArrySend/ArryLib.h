#pragma once
#include <iostream>
#include <ctime>

#include <cstring>
#include <complex>

#include "fftw3.h"

#include <vector>

#define _AFXDLL
#define PI (3.14159265358979323846264338327950288419716939937510)

#define WINDOWS_N 12//滑动平均滤波计算平均值时所取的点数


extern "C" _declspec(dllexport) void test_double_group(double *test, int num);//数组传递实验用
extern "C" _declspec(dllexport) void test_double_2groups(double **test, int row, int col);//数组传递实验用
extern "C" _declspec(dllexport) int  Corrd(const double *pInSoundArr, const int inLen, const double *pOutSoundArr, const int outLen, double *OutCorr);
extern "C" _declspec(dllexport) int  XCorrA(const short *pInSoundArr, const int inLen, const float *pOutSoundArr, const int outLen);
//extern "C" _declspec(dllexport) int GetDelayTimePos(const short *pInSoundArr, const int inLen, const float *pOutSoundArr, const int outLen, const int iAutodelay);
//extern "C" _declspec(dllexport) int GetDelayTimePos(const short *pInSoundArr, const int inLen, const float *pOutSoundArr, const int outLen, const int iAutodelay);
//
extern "C" _declspec(dllexport) void IndexInit();
extern "C" _declspec(dllexport) int  HanningWind(int N, double **w);
//extern "C" _declspec(dllexport) int  HanningWinf(int N, double **w);
extern "C" _declspec(dllexport) void Smooth(double *data, int size);

int m_OutM;
int m_InN;
double *m_x;
double *m_y;
double *m_in_a;
std::complex<double> *m_out_a;
double *m_in_b;
std::complex<double> *m_out_b;
std::complex<double> *m_in_rev;
double *m_out_rev;

double *GetInFloatData(void)
{
	return m_y;
};

double *GetOutFloatData(void)
{
	return m_x;
};

//extern "C" _declspec(dllexport) void Smooth(float *data, int size);