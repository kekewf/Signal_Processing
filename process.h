#define FULL_DUPLEX 1

//Use compiler "PreProcessor definitions" to distinguish 
//between these projects 
// For BoseVNC2D (barcode) use:  _VNC2D
// For BoseANC2D (barcode) use:  _ANC2D
// For Apple J5  use:            _APPLEJ5


//Project Name
#define PROJECT_NAME L"BOSE_ANC2D"

//Number of sound card channels
#define N_CHANNELS  2
#define M_PI 3.14159265358979323846
//Sample Rate
#define SAMPLERATE 48000//48000 //
//0.5s THD 1/16 spec  0.5s leak  1/16 spec  0.5s Sens  1/16 spec  1s FrPh  0.5s Noise
#define STIMLEN 4  //3.1875  2019-6-27由2 改为4S

// Not using FFT, put the 1Khz index where it happens to fall.
#define KHZ 35//100Hz 7 GA20B    // 1Khz 12   GA20A

//初始化系统默认最多频点 1/24 1-100Khz how many frequencies to do DFT on
#define NTODO 402 

// 如果信号检索正确，那么数据是最稳定的部分，如果不正确，那么数据取信号开始部分!
// distortion test first, start 0.05S 以后
#define DIST_START (SAMPLERATE*0.2) //只供初始化使用

// distortion test end (load 0.2-0.4s data)
#define DIST_END (DIST_START +(SAMPLERATE*0.2))// 只供初始化使用

// impedance test starts for cycles after 0.25 seconds  (increased to 512 10/30/06)
//#define IMP_START (384)  //(256) //(8000 + 200 + 128)

// impedance test ends 4 cycles early   // 200 samples is the silent period between L and R 1Khz tones.
//#define IMP_END (384 + 9600) //(256 + 6400)  //(16000 + 200 - 256)

// Freq start stop not as critical, but should capture entire 1 second period plus some of the silence on each end.
// 从0.7S-0.9S范围获取数据
#define FREQ_START (SAMPLERATE*0.7)//只供初始化使用//(1.5+3/16+0.2)) //0.5s THD 1/16 spec  0.5s leak  1/16 spec  0.5s Sens  1/16 spec
//Freq end
#define FREQ_END (FREQ_START+(SAMPLERATE*0.2))//只供初始化使用 //0.5s THD 1/16 spec  0.5s leak  1/16 spec  0.5s Sens  1/16 spec  1s FrPh

// Noise testing lasts 0.5 second, minus 4 cycles at 1K on each end  1.8S-2.0S
#define NOISE_START (SAMPLERATE*1.8)//只供初始化使用(SAMPLERATE*1.8
#define NOISE_END (NOISE_START+(SAMPLERATE*0.2))//只供初始化使用(NOISE_START+SAMPLERATE*0.2)
//================================================================

// ALPHA is 1/ Time Constant in number of DFT Frames
#define ALPHA 0.05  // was 0.03
#define NBUFS 4
#define NBUFSM1 3

//Size in bytes of the buffers (4 bytes per "sample)
// multiples of 32 for distortion for even numbers of periods for 1Khz at 32K sample rate
#define SM_BUFFER_SIZES (SAMPLERATE/2)  // 1/2
#define SM_BUFFER_SIZESO2 (SAMPLERATE/4) // 1/4
#define SM_BUFFER_SIZESO4 (SAMPLERATE/8) // 1/8
#define SM_BUFFER_SIZESO8 (SAMPLERATE/16) // 1/16

//  FLOOR is a small value that is insignificant to energy to make sure there are no 0 values for logs or divide by 0 errors
#define FLOOR ((float) 0.00000001)  

// Cross Correlation Constants
#define CORLENGTH (14400)//(SAMPLERATE*3/10) //(9600) // Number of samples of signal to look at
#define COROFFSET (1500)//(SM_BUFFER_SIZESO8) //(1000)  // Number of samples to shift test signal in either direction with respect to ref signal

void processinit();
void GenSignal();
