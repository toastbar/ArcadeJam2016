#include <Bounce.h>
#include <Adafruit_NeoPixel.h>
#include <LPD8806.h>
#include <SPI.h>
#include <EEPROM.h>

#define VERSION "Three Button RGB Panel V.1"

// EEPROM addresses
#define PANEL_ID             0

// Digital Input Pin Setup
#define PIN_BUTTON_A1        6
#define PIN_BUTTON_A2        7
#define PIN_BUTTON_A3        8
#define PIN_BUTTON_A_UP      3
#define PIN_BUTTON_A_DOWN    2
#define PIN_BUTTON_A_LEFT    5
#define PIN_BUTTON_A_RIGHT   4

// Debounce Milliseconds use for button input
#define DEBOUNCE_MILLIS      10

// RGB LED Button setup (No pin configuration, because the LED Strip uses the hardware SPI)
//                      (  Teensy 3.1 SPI SCK = pin 13  DOUT = pin 11                     )
#define STRIP_LED_COUNT        4

// Serial Communication Config
#define MAX_COMMAND_LENGTH     128

Bounce button_A1 = Bounce(PIN_BUTTON_A1, DEBOUNCE_MILLIS);
Bounce button_A2 = Bounce(PIN_BUTTON_A2, DEBOUNCE_MILLIS);
Bounce button_A3 = Bounce(PIN_BUTTON_A3, DEBOUNCE_MILLIS);
Bounce button_A_UP = Bounce(PIN_BUTTON_A_UP, DEBOUNCE_MILLIS);
Bounce button_A_DOWN = Bounce(PIN_BUTTON_A_DOWN, DEBOUNCE_MILLIS);
Bounce button_A_LEFT = Bounce(PIN_BUTTON_A_LEFT, DEBOUNCE_MILLIS);
Bounce button_A_RIGHT = Bounce(PIN_BUTTON_A_RIGHT, DEBOUNCE_MILLIS);

LPD8806 LEDStrip = LPD8806( STRIP_LED_COUNT, 9 /* data pin */, 10 /* clock pin */); 

void setup()
{
  pinMode(PIN_BUTTON_A1, INPUT_PULLUP);
  pinMode(PIN_BUTTON_A2, INPUT_PULLUP);
  pinMode(PIN_BUTTON_A3, INPUT_PULLUP);
  pinMode(PIN_BUTTON_A_UP, INPUT_PULLUP);
  pinMode(PIN_BUTTON_A_DOWN, INPUT_PULLUP);
  pinMode(PIN_BUTTON_A_LEFT, INPUT_PULLUP);
  pinMode(PIN_BUTTON_A_RIGHT, INPUT_PULLUP);
 
  // Joystick configured for Manual Send, Joystick.send_now() must be called
  //  to send a USB packet accross the wire
  Joystick.useManualSend(true);
  
  LEDStrip.begin();
  LEDStrip.show();
  
  Serial.begin(38400);
  
  delay(2000);
    Strip_Init();
}

void loop()
{
  static char SerialCommand[MAX_COMMAND_LENGTH + 1];
  static int SerialCommandLength = 0;
  
  // Update all the buttons.  There should not be any long
  // delays in loop(), so this runs repetitively at a rate
  // faster than the buttons could be pressed and released
  button_A1.update();
  button_A2.update();
  button_A3.update();
  button_A_UP.update();
  button_A_DOWN.update();
  button_A_LEFT.update();
  button_A_RIGHT.update();

  // Set USB Joystick Button state by sampling debounced button state
  
  if (button_A1.fallingEdge())
  {
    Joystick.button(1, 1);
  }
  else if (button_A1.risingEdge())
  {
    Joystick.button(1, 0);
  }
 
  if (button_A2.fallingEdge())
  {
    Joystick.button(2, 1);
  }
  else if (button_A2.risingEdge())
  {
    Joystick.button(2, 0);
  }
  
  if (button_A3.fallingEdge())
  {
    Joystick.button(3, 1);
  }
  else if (button_A3.risingEdge())
  {
    Joystick.button(3, 0);
  }

  // Recalculate Joystick Axis values (0-1023) based off of button presses 
  Joystick.X( 512 + ( button_A_LEFT.read() * 511 ) - ( button_A_RIGHT.read() * 512 ) );
  Joystick.Y( 512 + ( button_A_UP.read() * 511 ) - ( button_A_DOWN.read() * 512 ) );
  
  Joystick.send_now();
  
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

  Strip_Update();
 }
 
 
 void ProcessCommand( char* Command, int CommandLength )
 {
   int panelId;

   if ( CommandLength <= 0 )
     return;
     
   Command[CommandLength] = 0;
   
   switch ( Command[0] )
   {
     case 's':
       Strip_ProcessCommand( Command, CommandLength );
       break;
       
     case 'v':
       Serial.printf("%s\n", VERSION);
       break;
       
     case 'i':
       panelId = EEPROM.read(PANEL_ID);
       Serial.printf("%02X\n", panelId);
       Joystick.button(1, 1);
       Strip_ProcessCommand("s0fffffff0020", 13);
       Strip_ProcessCommand("s1s000000", 9);
       Strip_ProcessCommand("s2s000000", 9);
       Strip_ProcessCommand("s3s000000", 9);

       for (int i = 0; i < 100; ++i)
       {
         Joystick.send_now();
         Strip_Update();
         delay(10);
       }
       Joystick.button(1, 0);
       Joystick.send_now();
       Strip_ProcessCommand("s0r0004", 7);
       Strip_ProcessCommand("s1r0004", 7);
       Strip_ProcessCommand("s2r0004", 7);
       Strip_ProcessCommand("s3r0004", 7);
       break;
       
     case 'w':
       if (CommandLength == 3)
       {
         panelId = IntFromHex(&Command[1], 2);
         if (panelId < 0)
         {
           Serial.printf("Invalid panel id: %s\n", &Command[1]);
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
       Serial.printf("Invalid command: %s\n", Command);
       break;
   }
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
    int hexVal = IntFromHex( input[i] ) << (4 * (numDigits - i - 1) );
    if (hexVal < 0)
      return -1;

    returnVal += hexVal;
  }
 
  return returnVal;
}
 

