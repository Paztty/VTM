#define	CH1	    22
#define	CH2	    23
#define	CH3	    24
#define	CH4	    25
#define	CH5	    26
#define	CH6	    27
#define	CH7	    28
#define	CH8	    29
#define	CH9	    30
#define	CH10	31
#define	CH11	32
#define	CH12	33
#define	CH13	34
#define	CH14	35
#define	CH15	36
#define	CH16	37
#define	CH17	38
#define	CH18	39
#define	CH19	40
#define	CH20	41
#define	CH21	42
#define	CH22	43
#define	CH23	44
#define	CH24	45


void SetSolenoidPinMode()
{
    pinMode(CH1	, OUTPUT);
    pinMode(CH2	, OUTPUT);
    pinMode(CH3	, OUTPUT);
    pinMode(CH4	, OUTPUT);
    pinMode(CH5	, OUTPUT);
    pinMode(CH6	, OUTPUT);
    pinMode(CH7	, OUTPUT);
    pinMode(CH8	, OUTPUT);
    pinMode(CH9	, OUTPUT);
    pinMode(CH10	, OUTPUT);
    pinMode(CH11	, OUTPUT);
    pinMode(CH12	, OUTPUT);
    pinMode(CH13	, OUTPUT);  
    pinMode(CH14	, OUTPUT);
    pinMode(CH15	, OUTPUT);
    pinMode(CH16	, OUTPUT);
    pinMode(CH17	, OUTPUT);
    pinMode(CH18	, OUTPUT);
    pinMode(CH19	, OUTPUT);
    pinMode(CH20	, OUTPUT);
    pinMode(CH21	, OUTPUT);
    pinMode(CH22	, OUTPUT);
    pinMode(CH23	, OUTPUT);
    pinMode(CH24	, OUTPUT);
}

void SetSolenoidPin(uint8_t data[4])
{
    uint32_t data32 = 0x00000000;

   for (int index = 0; index < 4; index++)
   {    
        data32 = data32 << 8 | data[index];
   }
    for(int i = 0 ; i < 24; i++)
    {
        digitalWrite(i + 22, bitRead(data32,i));
    }
}