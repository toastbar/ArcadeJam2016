#include <Wire.h>
#include <Adafruit_NeoPixel.h>
#include <LPD8806.h>    
#include "Adafruit_Trellis.h"
#include <SPI.h>
#include <EEPROM.h>

#define VERSION "Trellis Arrows Panel V.2"

// EEPROM addresses
#define PANEL_ID             0

Adafruit_Trellis trellis = Adafruit_Trellis();

#define numKeys 16

// Serial Communication Config
#define MAX_COMMAND_LENGTH     128

// LED strip controlling arrow LEDs
#define STRIP_LED_COUNT        2

LPD8806 LEDStrip = LPD8806( STRIP_LED_COUNT, 9 /* data pin */, 10 /* clock pin */); 

void setup() {
  Serial.begin(38400);

  trellis.begin(0x70);
  
  // Joystick configured for Manual Send, Joystick.send_now() must be called
  //  to send a USB packet accross the wire
  Joystick.useManualSend(true);

  LEDStrip.begin();
  LEDStrip.show();
  
  Strip_Init();

  for (uint8_t i=0; i<numKeys; i++) {
    trellis.setLED(i);
    trellis.writeDisplay();
    delay(50);
  }  

//  for (uint8_t i=0; i<numKeys; i++) {
//    trellis.clrLED(i);
//  }

  trellis.writeDisplay();
  
  Joystick.X(512);
  Joystick.Y(512);
}

void toggle(int placeVal) {
 if (trellis.isLED(placeVal))
    trellis.clrLED(placeVal);
  else
    trellis.setLED(placeVal);
}

void loop() {
  static char SerialCommand[MAX_COMMAND_LENGTH + 1];
  static int SerialCommandLength = 0;
  
  while (Serial.available())
  {
    
    char c = Serial.read();
    if ( c == 13 || c == '\n' )
    {
      ProcessCommand( SerialCommand, SerialCommandLength );
      SerialCommandLength = 0;
      Serial.printf("\n");
    }
    else
    {
      if ( SerialCommandLength < MAX_COMMAND_LENGTH )
      {
        SerialCommand[SerialCommandLength] = c;
        SerialCommandLength += 1;
        Serial.printf("%c", c);
      }
    }
  }

    delay(30); // 30ms delay is required, dont remove me!
    // If a button was just pressed or released...
    if (trellis.readSwitches()) {
      // go through every button
      for (uint8_t i=0; i<16; i++) {
        Joystick.button(i + 1, trellis.isKeyPressed(i));
      }

//      Joystick.button(13, trellis.isKeyPressed(12) ||
//                          trellis.isKeyPressed(13) ||
//                          trellis.isKeyPressed(14) ||
//                          trellis.isKeyPressed(15));
    }
    // tell the trellis to set the LEDs we requested
    trellis.writeDisplay();
    Joystick.send_now();
    
    Strip_Update();
}

void ProcessCommand(char* command, int len)
{
  int pad_led;
  int panelId;
  
  if (len < 1)
    return;
    
  switch (command[0])
  {
    case 's':
      Strip_ProcessCommand(command, len);
      break;
      
    case 'o':
      if (len != 2)
        goto error;
        
      if (command[1] == '*')
      {
        for (pad_led = 0; pad_led < numKeys; ++pad_led)
        {
          trellis.setLED(pad_led);
        }
      }
      else
      { 
        pad_led = IntFromHex(command[1]);
        if (pad_led < 0)
          goto error;
    
        trellis.setLED(pad_led);
      }
      break;

    case 'f':
      if (len != 2)
        goto error;
        
      if (command[1] == '*')
      {
        for (pad_led = 0; pad_led < numKeys; ++pad_led)
        {
          trellis.clrLED(pad_led);
        }
      }
      else
      { 
        pad_led = IntFromHex(command[1]);
        if (pad_led < 0)
          goto error;
    
        trellis.clrLED(pad_led);
      }
      break;

     case 'v':
       Serial.printf("%s\n", VERSION);
       break;
       
     case 'i':
       panelId = EEPROM.read(PANEL_ID);
       Serial.printf("%02X\n", panelId);
       Joystick.button(1, 1);
       Strip_ProcessCommand("s0fffffff0030", 13);
       Strip_ProcessCommand("s1s000000", 9);
       trellis.clear();
       trellis.blinkRate(2);
       trellis.setLED(0);
       trellis.writeDisplay();
       for (int i = 0; i < 100; ++i)
       {
 
         Joystick.send_now();
         Strip_Update();
         delay(10);
       }
       Joystick.button(1, 0);
       Joystick.send_now();
       trellis.clear();
       trellis.blinkRate(0);
       trellis.writeDisplay();
       Strip_ProcessCommand("s0r0004", 7);
       Strip_ProcessCommand("s1r0004", 7);
       break;
       
     case 'w':
       if (len == 3)
       {
         panelId = IntFromHex(&command[1], 2);
         if (panelId < 0)
         {
           Serial.printf("Invalid panel id: %s\n", &command[1]);
         }
         else
         {
           EEPROM.write(PANEL_ID, panelId);
           Serial.printf("success");
         }
       }
       else
       {
         Serial.printf("Invalid command length for: w\n");
       }
       break;
       
    default:
      goto error;
  }
  return;

  error:
    Serial.printf("Invalid command.\n");
}


int IntFromHex( char c )
{
  if ( c >= '0' && c <= '9' )
  {
    return c - '0';
  }
  
  if ( c >= 'A' && c <= 'F' )
  {
    return c - 'A' + 10;
  }
  
  if ( c >= 'a' && c <= 'f' )
  {
    return c - 'a' + 10;
  }
  
  return -1;
}

int IntFromHex( char* input, int numDigits )
{
  int returnVal = 0;
  
  for ( int i = 0; i < numDigits; ++i )
  {
    int i_add = IntFromHex( input[i] ) << (4 * (numDigits - i - 1) );
    
    if (i_add == -1)
      return -1;
 
    returnVal += i_add;
  }
 
  return returnVal;
}
