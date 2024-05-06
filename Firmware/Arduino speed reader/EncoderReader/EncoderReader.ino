/* Encoder Library - NoInterrupts Example
 * http://www.pjrc.com/teensy/td_libs_Encoder.html
 *
 * This example code is in the public domain.
 */

// If you define ENCODER_DO_NOT_USE_INTERRUPTS *before* including
// Encoder, the library will never use interrupts.  This is mainly
// useful to reduce the size of the library when you are using it
// with pins that do not support interrupts.  Without interrupts,
// your program must call the read() function rapidly, or risk
// missing changes in position.
//#define ENCODER_DO_NOT_USE_INTERRUPTS
#include "Encoder.h"

// Beware of Serial.print() speed.  Without interrupts, if you
// transmit too much data with Serial.print() it can slow your
// reading from Encoder.  Arduino 1.0 has improved transmit code.
// Using the fastest baud rate also helps.  Teensy has USB packet
// buffering.  But all boards can experience problems if you print
// too much and fill up buffers.

// Change these two numbers to the pins connected to your encoder.
//   With ENCODER_DO_NOT_USE_INTERRUPTS, no interrupts are ever
//   used, even if the pin has interrupt capability
Encoder myEnc1(2, 4);
Encoder myEnc2(3, 5);
Encoder myEnc3(18, 20);
Encoder myEnc4(19, 21);
//   avoid using pins with LEDs attached
const int PPC = 2400;
long start = millis();
typedef union
{
  float number;
  uint8_t bytes[4];
} FLOATUNION_t;

void setup() {
  Serial.begin(115200);
}

long position  = -999;

void loop() {
  if(millis() - start > 1000)
  {
    start = millis();
    long newPos1 = myEnc1.readAndReset();
    long newPos2 = myEnc2.readAndReset();
    long newPos3 = myEnc3.readAndReset();
    long newPos4 = myEnc4.readAndReset();
  }
}

void Read()
{
    //5A A5 A9 0A |||| |||| |||| |||| |||| 56
    //1  2  3  4  5 6  7 8  9 10 1112 1314 15
    unsigned char ResponseFrame[22] = {0x44, 0x45, 0x13, 0x50, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x56};

    FLOATUNION_t FloatNumber;
    long time = millis() - start;
    start = millis();
    // long newPos1 = myEnc1.read();
    // long newPos2 = myEnc2.read();
    // long newPos3 = myEnc3.read();
    // long newPos4 = myEnc4.read();
    long newPos1 = myEnc1.readAndReset();
    long newPos2 = myEnc2.readAndReset();
    long newPos3 = myEnc3.readAndReset();
    long newPos4 = myEnc4.readAndReset();
    delay(500);
    time = 500;
    newPos1 = myEnc1.read();
    newPos2 = myEnc2.read();
    newPos3 = myEnc3.read();
    newPos4 = myEnc4.read();

    FloatNumber.number = newPos1 / PPC * 60 * 1000.0/time;
    uint8_t index = 4;
    for (uint8_t i = 0; i < 4; i++)
    {
      ResponseFrame[index] = FloatNumber.bytes[i];
      index++;
    }
    FloatNumber.number = newPos2 / PPC * 60 * 1000.0/time;
    for (uint8_t i = 0; i < 4; i++)
    {
      ResponseFrame[index] = FloatNumber.bytes[i];
      index++;
    }
    FloatNumber.number = newPos3 / PPC * 60 * 1000.0/time;
    for (uint8_t i = 0; i < 4; i++)
    {
      ResponseFrame[index] = FloatNumber.bytes[i];
      index++;
    }
    FloatNumber.number = newPos4 / PPC * 60 * 1000.0/time;
    for (uint8_t i = 0; i < 4; i++)
    {
      ResponseFrame[index] = FloatNumber.bytes[i];
      index++;
    }
    unsigned char xorTemp = ResponseFrame[0];
    for(int i = 1; i < 20; i++){
        xorTemp ^= ResponseFrame[i];
    }
    ResponseFrame[20] = xorTemp;
    Serial.write(ResponseFrame, sizeof(ResponseFrame));
}


void serialEvent() {
  if(Serial.available() >= 5)
  {
    Read();
    while (Serial.available())
    {
       Serial.read();
    }
    
  }
}