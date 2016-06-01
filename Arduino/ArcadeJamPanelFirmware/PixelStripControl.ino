

// Button Command Reference
//
//  KEY: ## = ButtonNumber  RR = red   GG = green   BB = blue  TTTT = FlashInterval in milliseconds
//
// Solid Color
// bs##RRGGBB
//
// Flasing Color
// bf##RRGGBBTTTT
//


// LED Panel Strip Command Reference
//
//  KEY: RR = red   GG = green   BB = blue  TTTT = FlashInterval in milliseconds
//       VV = Meter Value
//
// Solid Color
// psRRGGBB
//  
// Flashing Color
// pfRRGGBBTTTT
//
// Rainbow Scroll
// prTTTT
//
// Meter
// pmRRGGBBVV
//



#define FIRST_BUTTON_LED_OFFSET   0
#define BUTTON_LED_COUNT          4

static LEDGroupState ButtonLEDState[BUTTON_LED_COUNT];

void Strip_Init()
{
  for ( int i = 0; i < BUTTON_LED_COUNT; ++i )
  {
    ButtonLEDState[i].Init( &LEDStrip, FIRST_BUTTON_LED_OFFSET + i, 1 );
    //ButtonLEDState[i].SetOff();
    ButtonLEDState[i].SetRainbowScroll(4);
  }
  
  //ButtonLEDState[0].SetFlashing( 0, 127, 0, 1000 );
}



void Strip_Update()
{
  unsigned long currentMillis = millis();

  for ( int i = 0; i < BUTTON_LED_COUNT; ++i )
  {
    ButtonLEDState[i].Update( currentMillis );
  } 
  
  LEDStrip.show();
}


void Strip_ProcessCommand( char* Command, int CommandLength )
{
  if ( CommandLength < 3 )
    return;
  
  LEDGroupState* group;
  
  int buttonId = IntFromHex( Command[1] );
  if (buttonId < 0)
    goto error;
 
  group = &ButtonLEDState[buttonId];

  switch ( Command[2] )
  {
    case 's':
    {
      if ( CommandLength != 9 )
        goto error;
        
      int red = IntFromHex( &Command[3], 2 );
      int green = IntFromHex( &Command[5], 2 );
      int blue = IntFromHex( &Command[7], 2 );
      
      if (red < 0 || green < 0 || blue < 0)
        goto error;
      
      group->SetSolid( red,green,blue );
    }
    break;
      
    case 'f':
    {
      if ( CommandLength != 13 )
        goto error;
        
      int red = IntFromHex( &Command[3], 2 );
      int green = IntFromHex( &Command[5], 2 );
      int blue = IntFromHex( &Command[7], 2 );
      
      if (red < 0 || green < 0 || blue < 0)
        goto error;
 
      int flashInterval = IntFromHex( &Command[9], 4 );
      if (flashInterval < 0)
        goto error;
      
      group->SetFlashing( red,green,blue, flashInterval );
    }
    break;
  
    case 'r':
    {
        if ( CommandLength != 7 )
          goto error;
          
        int flashInterval = IntFromHex( &Command[3], 4 );
        if (flashInterval < 0)
          goto error;

        group->SetRainbowScroll( flashInterval ); 
    }
    break;
    
    case 'm':
    {
        if ( CommandLength != 11 )
          goto error;
          
      int red = IntFromHex( &Command[3], 2 );
      int green = IntFromHex( &Command[5], 2 );
      int blue = IntFromHex( &Command[7], 2 );
      int value = IntFromHex( &Command[9], 2 );
      
      if (red < 0 || green < 0 || blue < 0 || value < 0)
        goto error;

      group->SetMeter( red,green,blue, value );
    }
  }
  
  return;
  
error:
  Serial.printf( "Bad LED Strip command: %s\n", Command );
}
