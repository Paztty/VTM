//---------SYSTEM INPUT---------(switch and sensor)
// define-------name--------pin name-----note
//#define       SWUP        A0          //switch up 
//#define       SWDN        A1          //switch down aka start signal
//#define       SWBR        A1          //JIG BOTTOM LOCK
//#define       SWBF        A1          //JIG BOTTOM UNLOCK
//#define       SWTR        A1          //JIG TOP LOCK
//#define       SWTF        A1          //JIG TOP UNLOCK
//#define       SWEMG       A2          //switch emergency
//#define       SWDOOR      A3          //switch door safety

//#define       SUP         A3          //sensor top level
#define         SDOWN       4          //sensor bottom level aka start signal
//#define       SBR         A1          //sensor JIG BOTTOM LOCK
//#define       SBF         A1          //sensor JIG BOTTOM UNLOCK
//#define       STR         A1          //sensor JIG TOP LOCK
//#define       STF         A1          //sensor JIG TOP UNLOCK
//#define       SBL         A1          //JIG BOTTOM LOCK status
//#define       STL         A1          //JIG TOP UNLOCK status

//---------SYSTEM OUTPUT---------(power, cylinder, tower light, buzzer)
// define-------name--------pin name-----note
//#define       AC110       A8          //A site 110V power
//#define       AC0         A1          //A site 0V power
#define         AC220       A8          //A site 220V power
//#define       ADSC1       A1          //A site dicharge
//#define       ADCS2       A1          //A site dicharge
//#define       ASP1        A1          //A site spare 1
//#define       ASP2        A2          //A site spare 2
//#define       ASP3        A3          //A site spare 3

//#define       BC110       A8          //B site 110V power
//#define       BC0         A1          //B site 0V power
//#define       BC220       A9          //B site 220V power
//#define       BDSC1       A1          //B site dicharge
//#define       BDCS2       A1          //B site dicharge
//#define       BSP1        A1          //B site spare 1
//#define       BSP2        A2          //B site spare 2
//#define       BSP3        A3          //B site spare 3

#define         LPR         A10         //Tower lamp red
#define         LPY         A11         //Tower lamp yellow
#define         LPG         A12         //Tower lamp green
#define         BZ          A13         //Tower lamp buzzer
//#define       SP1         A1          //spare 1
//#define       SP2         A2          //spare 2
//#define       SP3         A3          //spare 3
//#define       SP4         A3          //spare 4

#define         CLUP        A14         //Cilynder Up control aka reset
//#define       CLDN        A15         //Cilynder Down control
//#define       SP1         A1          //spare 1
//#define       SP2         A2          //spare 2
//#define       SP3         A3          //spare 3
//#define       SP3         A3          //spare 3
//#define       SP3         A3          //spare 3
//#define       SP3         A3          //spare 3


void SetSystemIOPinMode()
{
    Serial.begin(9600);

    pinMode(SDOWN,INPUT_PULLUP);

    pinMode( LPG,OUTPUT);
    pinMode( LPY,OUTPUT);
    pinMode( LPR,OUTPUT);
    pinMode( BZ, OUTPUT);

    pinMode( AC220, OUTPUT);

    pinMode( CLUP,OUTPUT);
}


uint8_t systemRespoenseInput[10] = {0x44,0x45,0x06,0x49,0x00,0xE8,0x00,0x00,0x52,0x56};
uint8_t systemRespoenseOutput[10] = {0x44,0x45,0x4F,0x00,0x52,0x56};
uint8_t LastStartState = false;

void CollectInput()
{
    uint8_t startSignal = !digitalRead(SDOWN);
    if(startSignal != LastStartState)
    {
        delay(500);
        startSignal = !digitalRead(SDOWN);
            if(startSignal != LastStartState)
            {
                if(startSignal == 1)
                {
                    digitalWrite(LPR,   LOW);
                    digitalWrite(LPY,   HIGH);
                    digitalWrite(LPG,   LOW);
                }
                else
                {
                    digitalWrite(AC220, LOW);
                }

                LastStartState = startSignal;
                bitWrite(systemRespoenseInput[5],1,startSignal);
                unsigned char xorTemp = systemRespoenseInput[0];
                for(int i = 1; i < 8; i++){
                    xorTemp ^= systemRespoenseInput[i];
                }
                systemRespoenseInput[8] = xorTemp;
                Serial.write(systemRespoenseInput, 10);
            }
    }
}
void ResponsetInput()
{
    uint8_t startSignal = !digitalRead(SDOWN);

    bitWrite(systemRespoenseInput[5],1,startSignal);
    unsigned char xorTemp = systemRespoenseInput[0];
    for(int i = 1; i < 8; i++){
        xorTemp ^= systemRespoenseInput[i];
    }
    systemRespoenseInput[8] = xorTemp;
    Serial.write(systemRespoenseInput, 10);
}


void SetSystemOutput(uint8_t data[4])
{
    uint32_t data32 = 0x00000000;

    for (int index = 0; index < 4; index++)
        {    
            data32 = data32 << 8 | data[index];
        }
    digitalWrite(LPR,   bitRead(data32,8));
    digitalWrite(LPY,   bitRead(data32,9));
    digitalWrite(LPG,   bitRead(data32,10));
    digitalWrite(BZ,    bitRead(data32,11));
    digitalWrite(CLUP,  bitRead(data32,0));
    if(!digitalRead(SDOWN))
        digitalWrite(AC220,  bitRead(data32,26));
}

