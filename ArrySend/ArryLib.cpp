
#include "ArryLib.h"
#include "fftw3.h"
#include <iostream>
#include <windows.h>
#include <WinBase.h>
#include <ctime>
#include <stdlib.h>

#include "../alglib/alglibinternal.h"
#include "../alglib/alglibmisc.h"
#include "../alglib/diffequations.h"
#include "../alglib/linalg.h"
#include "../alglib/optimization.h"
#include "../alglib/solvers.h"
#include "../alglib/statistics.h"
#include "../alglib/dataanalysis.h"
#include "../alglib/specialfunctions.h"
#include "../alglib/integration.h"
#include "../alglib/fasttransforms.h"
#include "../alglib/interpolation.h"
#pragma comment(lib,"libfftw3-3.lib")

using namespace alglib_impl;

using namespace std;
#define RESAMPLE_STEP (0)

int  HanningWind(int N, double **w)
{
	int n;
	double *ret;
	register double fRatio = 0.0;

	ret = (double *)malloc(N * sizeof(double));
	if (ret == NULL)
		return 1;

	for (n = 0; n < N; n++)
	{
		fRatio = (double)n / (double)(N - 1);
		*(ret + n) = 0.5 * (1 - cos(2 * PI * fRatio));
	}

	*w = ret;
	
	return 1;
}

int  HanningWind(int SamplesNum, double *data)
{
	int n;
	double *ret;
	register double fRatio = 0.0;

	ret = new double[SamplesNum];
	if (ret == NULL)
		return 1;

	for (n = 0; n < SamplesNum; n++)
	{
		fRatio = (double)n / (double)(SamplesNum - 1);
		ret[n] = 0.5 * (1 - cos(2 * PI * fRatio));
	}

	data = ret;

	return 1;
}


/*上面两句在使用下面这个函数的时候加到程序的开头*/
void Smooth(double *data, int size)
{
	double Sum1 = 0;
	for (int j = 0; j< size; j++)
	{
		if (j< WINDOWS_N / 2)
		{
			for (int k = 0; k<WINDOWS_N; k++)
			{
				Sum1 += data[j + k];
			}
			data[j] = Sum1 / WINDOWS_N;
		}
		else
		{
			if (j < size - WINDOWS_N / 2)
			{
				for (int k = 0; k < WINDOWS_N / 2; k++)
				{
					Sum1 += (data[j + k] + data[j - k]);
				}
				data[j] = Sum1 / WINDOWS_N;
			}
			else
			{
				for (int k = 0; k < size - j; k++)
				{
					Sum1 += data[j + k];
				}
				for (int k = 0; k < (WINDOWS_N - size + j); k++)
				{
					Sum1 += data[j - k];
				}
				data[j] = Sum1 / WINDOWS_N;
			}
		}
		Sum1 = 0;
	}
}

//void Smooth(float *data, int size)
//{
//	double Sum1 = 0;
//	for (int j = 0; j< size; j++)
//	{
//		if (j< WINDOWS_N / 2)
//		{
//			for (int k = 0; k<WINDOWS_N; k++)
//			{
//				Sum1 += data[j + k];
//			}
//			data[j] = Sum1 / WINDOWS_N;
//		}
//		else
//		{
//			if (j < size - WINDOWS_N / 2)
//			{
//				for (int k = 0; k < WINDOWS_N / 2; k++)
//				{
//					Sum1 += (data[j + k] + data[j - k]);
//				}
//				data[j] = Sum1 / WINDOWS_N;
//			}
//			else
//			{
//				for (int k = 0; k < size - j; k++)
//				{
//					Sum1 += data[j + k];
//				}
//				for (int k = 0; k < (WINDOWS_N - size + j); k++)
//				{
//					Sum1 += data[j - k];
//				}
//				data[j] = Sum1 / WINDOWS_N;
//			}
//		}
//		Sum1 = 0;
//	}
//}
//
_declspec(dllexport) void IndexInit()
{
	m_OutM = 0;
	m_InN = 0;

	m_x = NULL;
	m_y = NULL;
	m_in_a = NULL;
	m_out_a = NULL;
	m_in_b = NULL;
	m_out_b = NULL;
	m_in_rev = NULL;
	m_out_rev = NULL;
}

_declspec(dllexport)  int  Corrd(const double *pInSoundArr, const int inLen, const double *pOutSoundArr, const int outLen,double *w)
{
	int iPos = 0;
	//void corr(int *u, int m, int *v, int n, int *w){
	int k = 0;
	int i = 0;//求和次数
	int wLength = inLen + outLen - 1;//相关输出序列的长度
	//double *tmparry = new double[inLen];
	//*tmparry = *pInSoundArr;
	//HanningWind(inLen, &tmparry);//输入信号使用汉宁窗处理一下

	//double  *w = new double[wLength];
	for (k = 0; k<wLength; k++)
	{
		*(w + k) = 0;
		for (i = (0>k + 1 - inLen ? 0 : k + 1 - inLen); i <= (k < outLen - 1 ? k : outLen - 1); i++){
			*(w + k) += (*(pInSoundArr + k - i)) *(*(pOutSoundArr + outLen - 1 - i));
		}		
	}
	//	return  *w;
		//	版权声明：本文为CSDN博主「xuanyu321」的原创文章，遵循CC 4.0 BY - SA版权协议，转载请附上原文出处链接及本声明。
		//原文链接：https ://blog.csdn.net/xuanyu321/article/details/48752601
	//结果数组值翻转
	for (int i = 0, j = wLength - 1; i<j; i++, j--)
	{
		double tmp = *(w + j);
		*(w + j) = *(w + i);
		*(w + i) = tmp;
	}

	double max;
	double xcorr;
	double tmpdouble;
	int HalfLen = (inLen>outLen) ? inLen : outLen;
	max = abs(w[0]);
	for (int idx = 0; idx < HalfLen; idx++)
	{
		tmpdouble =abs(w[idx]);
		if (max <= tmpdouble)
		{
			max = tmpdouble;
			iPos = idx;
			xcorr = max;
		}
	}
	printf("max= [%f] \n", max);
	iPos = HalfLen - iPos;
	printf("ipos= [%d] \n", iPos);
	return iPos;
}

//
_declspec(dllexport)  int  XCorrA(const double *pInSoundArr, const int inLen, const double *pOutSoundArr, const int outLen)
{
	static int iPosOld = 0;
	int iPos = 0;
	register double fValue;
	double xcorr = 0;//int  XCorr(const short *pInSoundArr, const int inLen, const float *pOutSoundArr, const int outLen, double &xcorr)

	//可以将 m_OutM 和 m_InN 变为内部变量
	m_OutM = outLen >> RESAMPLE_STEP;
	m_InN = inLen >> RESAMPLE_STEP;

	double *pPattern = new double[m_OutM];
	double  *pSignal = new double[m_InN];
	double  *pResult = new double[m_InN + m_OutM];

	for (int i = 0; i < m_InN; i++)
	{
		fValue = ((double)pInSoundArr[i << RESAMPLE_STEP] / 32767.0);
		pSignal[i] = fValue;
	}

	for (int i = 0; i < m_OutM; i++)
	{
		fValue = (double)pOutSoundArr[i << RESAMPLE_STEP];
		pPattern[i] = fValue;
	}

	alglib::real_1d_array pattern;
	pattern.attach_to_ptr(m_OutM, pPattern);

	alglib::real_1d_array signal;
	signal.attach_to_ptr(m_InN, pSignal);

	alglib::real_1d_array r;
	r.attach_to_ptr(m_InN + m_OutM, pResult);

	alglib::corrr1d(signal, m_InN, pattern, m_OutM, r, alglib::serial);

	double max = -100000000;
	const register int HalfLen = m_InN;

	for (int idx = 0; idx < HalfLen; idx++)
	{
		if (max <= r[idx])
		{
			max = r[idx];
			iPos = idx;
			xcorr = max;
		}
	}

	iPos = (iPos << RESAMPLE_STEP);

	if (iPos < 0)
	{
		iPos = iPosOld;
	}

	if (!(iPos > 0 && xcorr > 1.0) || m_InN - iPos < m_OutM)
	{
		iPos = iPosOld;
	}
	else
	{
		iPosOld = iPos;
	}

	printf("off = %d [%f,%f,%f,%f,%f]\n", iPos, r[iPos - 5], r[iPos - 4], r[iPos - 3], r[iPos - 2], r[iPos - 1]);
	printf("max= [%f] \n", max);
	printf("off =[%f,%f,%f,%f,%f] \n", r[iPos + 1], r[iPos + 2], r[iPos + 3], r[iPos + 4], r[iPos + 5]);


	if (pPattern != NULL)
	{
		delete[] pPattern;
		pPattern = NULL;
	}
	if (pSignal != NULL)
	{
		delete[] pSignal;
		pSignal = NULL;
	}

	if (pResult != NULL)
	{
		delete[] pResult;
		pResult = NULL;
	}

	return iPos;
}

//_declspec(dllexport)  int GetDelayTimePos(const short *pInSoundArr, const int inLen, const float *pOutSoundArr, const int outLen,  const int iAutodelay)
//{
//	int iPos = 0;
//	static int iPosOld = 0;
//	register double fMax = 0;
//	//int GetDelayTimePos(const short *pInSoundArr, const int inLen, const float *pOutSoundArr, const int outLen, double &xcorr, const int iAutodelay)
//	double xcorr = 0;
//	if (inLen < outLen)
//	{
//		printf("off = 0 (in count should bigger!)\n");
//		return iPosOld;
//	}
//	int HalfLen = (inLen>outLen) ? inLen : outLen;
//	double  *datain = new double[HalfLen];
//	double  *dataref = new double[HalfLen];
//
//	for (int i = 0; i < inLen; i++)
//		datain[i] = pInSoundArr[i];
//
//	for (int i = 0; i < outLen; i++)
//		dataref[i] = pInSoundArr[i];
//
//	if (iAutodelay == 3)
//	{
//		return XCorrA(datain, inLen, dataref, outLen);
//	}
//
//	m_OutM = outLen >> RESAMPLE_STEP;
//	m_InN = inLen >> RESAMPLE_STEP;
//
//	m_x = new double[m_InN];
//	m_y = new double[m_InN];
//
//	m_out_a = new std::complex<double>[m_InN];
//	m_out_b = new std::complex<double>[m_InN];
//
//	m_in_rev = new std::complex<double>[m_InN];
//	m_out_rev = new double[m_InN];
//
//	for (int i = 0; i < m_InN; i++)
//	{
//		m_y[i] = ((float)pInSoundArr[i << RESAMPLE_STEP] / 32767.0);
//	}
//
//	for (int i = 0; i < m_OutM; i++)
//	{
//		m_x[i] = (float)pOutSoundArr[i << RESAMPLE_STEP];
//	}
//
//	for (int i = m_OutM; i < m_InN; i++)
//	{
//		m_x[i] = 0;
//	}
//
//	// Plans for forward FFTs
//	fftw_plan plan_fwd_a = fftw_plan_dft_r2c_1d(m_InN, m_x, reinterpret_cast<fftw_complex*>(m_out_a), FFTW_ESTIMATE);
//	fftw_plan plan_fwd_b = fftw_plan_dft_r2c_1d(m_InN, m_y, reinterpret_cast<fftw_complex*>(m_out_b), FFTW_ESTIMATE);
//
//	// Plan for reverse FFT
//	fftw_plan plan_rev = fftw_plan_dft_c2r_1d(m_InN, reinterpret_cast<fftw_complex*>(m_in_rev), m_out_rev, FFTW_ESTIMATE);
//
//
//	// Calculate the forward FFTs
//	fftw_execute(plan_fwd_a);
//	fftw_execute(plan_fwd_b);
//
//	const register int halfLen = m_InN >> 1;
//	// Multiply in frequency domain
//	switch (iAutodelay)
//	{
//	case 0:
//
//		for (int idx = 0; idx < halfLen; idx++)
//		{
//			m_in_rev[idx] = std::conj(m_out_a[idx]) * (m_out_b[idx]);
//
//			fMax = std::sqrt(abs(m_out_b[idx]) * abs(m_out_a[idx]));
//			if (fMax > 0)
//			{
//				m_in_rev[idx] /= fMax;
//			}
//		}
//		break;
//
//	case 1:
//		for (int idx = 0; idx < halfLen; idx++)
//		{
//			m_in_rev[idx] = std::conj(m_out_a[idx]) * (m_out_b[idx]);
//
//			fMax = std::sqrt(abs(m_out_b[idx] * std::conj(m_out_b[idx])) * abs(m_out_a[idx] * std::conj(m_out_a[idx])));
//			if (fMax > 0)
//			{
//				m_in_rev[idx] /= fMax;
//			}
//		}
//		break;
//	case 2:
//		for (int idx = 0; idx < halfLen; idx++)
//		{
//			m_in_rev[idx] = std::conj(m_out_a[idx]) * (m_out_b[idx]);
//			fMax = abs(m_in_rev[idx]);
//			if (fMax > 0)
//			{
//				m_in_rev[idx] /= fMax;
//			}
//		}
//		break;
//	case 4:
//
//		for (int idx = 0; idx < halfLen; idx++)
//		{
//			m_in_rev[idx] = std::conj(m_out_a[idx]) * (m_out_b[idx]);
//
//			fMax = abs(m_out_a[idx] * std::conj(m_out_a[idx]));
//			if (fMax > 0)
//			{
//				m_in_rev[idx] /= fMax;
//			}
//		}
//		break;
//	default:
//		for (int idx = 0; idx < halfLen; idx++)
//		{
//			m_in_rev[idx] = std::conj(m_out_a[idx]) * (m_out_b[idx]);
//
//			fMax = std::sqrt(abs(m_out_b[idx]) * abs(m_out_a[idx]));
//			if (fMax > 0)
//			{
//				m_in_rev[idx] /= fMax;
//			}
//		}
//		break;
//	}
//
//	// Calculate the backward FFT
//	fftw_execute(plan_rev);
//
//	float max = 0;
//
//	for (int idx = 0; idx < m_InN; idx++)
//	{
//		if (max <= m_out_rev[idx])
//		{
//			max = m_out_rev[idx];
//			iPos = idx;
//			xcorr = max;
//		}
//	}
//	// Clean up
//	fftw_destroy_plan(plan_fwd_a);
//	fftw_destroy_plan(plan_fwd_b);
//	fftw_destroy_plan(plan_rev);
//
//	fftw_cleanup();
//
//	iPos = (iPos << RESAMPLE_STEP);
//
//	if (!(xcorr > 1.0) || iPos  < 0 || m_InN - iPos < m_OutM)
//	{
//		iPos = iPosOld;
//	}
//	else
//	{
//		iPosOld = iPos;
//	}
//
//
//	if (iPos > 5)
//	{
//		printf("off = %d [%f,%f,%f,%f,%f]  [%f]  [%f,%f,%f,%f,%f] \n", iPos, m_out_rev[iPos - 5], m_out_rev[iPos - 4], m_out_rev[iPos - 3], m_out_rev[iPos - 2], m_out_rev[iPos - 1], max, m_out_rev[iPos + 1], m_out_rev[iPos + 2], m_out_rev[iPos + 3], m_out_rev[iPos + 4], m_out_rev[iPos + 5]);
//	}
//	else
//	{
//		printf("off = %d\n", iPos);
//	}
//
//	if (iPos < 0)
//	{
//		iPos = 0;
//	}
//
//	if (m_x != NULL)
//	{
//		delete[] m_x;
//		m_x = NULL;
//	}
//
//	if (m_y != NULL)
//	{
//		delete[] m_y;
//		m_y = NULL;
//	}
//
//	if (m_out_a != NULL)
//	{
//		delete[] m_out_a;
//		m_out_a = NULL;
//	}
//
//	if (m_out_b != NULL)
//	{
//		delete[] m_out_b;
//		m_out_b = NULL;
//	}
//
//	if (m_in_rev != NULL)
//	{
//		delete[] m_in_rev;
//		m_in_rev = NULL;
//	}
//
//	if (m_out_rev != NULL)
//	{
//		delete[] m_out_rev;
//		m_out_rev = NULL;
//	}
//
//	return iPos;
//}

//一维数组处理
_declspec(dllexport) void test_double_group(double *test, int num)
{
	int j = 1;
	for (int i = 0; i <num; i++)
	{
		test[i] += 1;
	}
}
//二维数据处理
_declspec(dllexport) void test_double_2groups(double **test, int row, int col)
{
	double x = 1;
	double y = 1;
	for (int i = 0; i <row; i++)
	{
		for (int j = 0; j < col; j++)
		{
			//x = x + 1;
			//x++;
			//y = i;
			//y = x;
			test[i][j] = test[i][j]+0.5;
		}
	}
}
