enum LEDMode
{
  Mode_Off,
  Mode_SolidColor,
  Mode_Flashing,
  Mode_Meter,
  Mode_RainbowScroll
};

class LEDGroupState
{
public:
  LEDGroupState() {}
  
  void Init( LPD8806* driver, int LEDoffset, int LEDcount );
  void Init( Adafruit_NeoPixel* driver, int LEDoffset, int LEDcount );
  
  void Update( unsigned long currentMillis );

  void SetOff();
  void SetSolid( uint8_t r, uint8_t g, uint8_t b );
  void SetFlashing( uint8_t r, uint8_t g, uint8_t b, uint32_t flashInterval );
  void SetMeter( uint8_t r, uint8_t g, uint8_t b, uint8_t value );
  void SetRainbowScroll( uint32_t changeInterval );

private:
  void _FillSolid( uint32_t color );
  void _FillRainbow( uint16_t colorWheelOffset );
  void _FillMeter( uint32_t color, uint8_t meterValue );
 
  uint32_t _Color( uint8_t red, uint8_t green, uint8_t blue );
  uint32_t _ColorWheel( uint16_t WheelPos );
  
  unsigned long mMilliCounter;
  uint32_t mFlashInterval;
  LEDMode mCurrentMode;
  uint32_t mColorValue;
  int mMeterValue;
  uint16_t mColorWheelOffset;
  bool mFlashOn;
  
  int mLEDoffset;
  int mLEDcount;
  LPD8806* mLPD8806Driver;
  Adafruit_NeoPixel* mNeoPixelDriver;
};

void LEDGroupState::Init( LPD8806* driver, int LEDoffset, int LEDcount )
{
  mLPD8806Driver = driver;
    mNeoPixelDriver = 0;
  mLEDoffset = LEDoffset;
  mLEDcount = LEDcount;
  
  SetOff();
}

void LEDGroupState::Init( Adafruit_NeoPixel* driver, int LEDoffset, int LEDcount )
{
  mLPD8806Driver = 0;
  mNeoPixelDriver = driver;
  mLEDoffset = LEDoffset;
  mLEDcount = LEDcount;
  
  SetOff();
}

void LEDGroupState::Update( unsigned long currentMillis )
{
  switch ( mCurrentMode )
  {
    case Mode_Flashing:
    {
      if ( currentMillis > (mMilliCounter + mFlashInterval) )
      {
        mMilliCounter += mFlashInterval;
        
        mFlashOn = !mFlashOn;
        if ( mFlashOn )
        {
          _FillSolid( mColorValue );
        }
        else
        {
          _FillSolid( 0 );     
        }
        
      }
    }
    break;
    
    case Mode_RainbowScroll:
    {
      if ( currentMillis > (mMilliCounter + mFlashInterval) )
      {
        mMilliCounter += mFlashInterval;
        
        mColorWheelOffset = (mColorWheelOffset + 1) % (128 * 3);
        _FillRainbow( mColorWheelOffset );
      }
    }
    break;
    
    default:
      break;
  }
}

void LEDGroupState::SetOff()
{
  mCurrentMode = Mode_Off;
  
  mColorValue = 0;
  _FillSolid( mColorValue );
}

void LEDGroupState::SetSolid( uint8_t r, uint8_t g, uint8_t b )
{
  mCurrentMode = Mode_SolidColor;
  
  mColorValue = _Color( r,g,b );
  _FillSolid( mColorValue );
}

void LEDGroupState::SetFlashing( uint8_t r, uint8_t g, uint8_t b, uint32_t flashInterval )
{
  mCurrentMode = Mode_Flashing;
  mFlashInterval = flashInterval;
  mFlashOn = true;
  mMilliCounter = millis();
  
  mColorValue = _Color( r,g,b );
  _FillSolid( mColorValue );
}

void LEDGroupState::SetRainbowScroll( uint32_t changeInterval )
{
  mCurrentMode = Mode_RainbowScroll;
  mFlashInterval = changeInterval;
  mMilliCounter = millis();
  mColorWheelOffset = 0;
  
  _FillRainbow( mColorWheelOffset );
}

void LEDGroupState::SetMeter( uint8_t r, uint8_t g, uint8_t b, uint8_t meterValue )
{
  mCurrentMode = Mode_Meter;
  mMeterValue = meterValue;
  mColorValue = _Color( r,g,b );
  
  _FillMeter( mColorValue, mMeterValue );
}


uint32_t LEDGroupState::_Color( uint8_t red, uint8_t green, uint8_t blue )
{
  if ( mLPD8806Driver )
    return mLPD8806Driver->Color( green, blue, red     );
  else
    return mNeoPixelDriver->Color( red, green, blue );
}

uint32_t LEDGroupState::_ColorWheel(uint16_t WheelPos)
{
  uint8_t r, g, b;
  WheelPos %= (128*3);
  
  switch(WheelPos / 128)
  {
    case 0:
      r = 127 - WheelPos % 128;   //Red down
      g = WheelPos % 128;      // Green up
      b = 0;                  //blue off
      break; 
    case 1:
      g = 127 - WheelPos % 128;  //green down
      b = WheelPos % 128;      //blue up
      r = 0;                  //red off
      break; 
    case 2:
      b = 127 - WheelPos % 128;  //blue down 
      r = WheelPos % 128;      //red up
      g = 0;                  //green off
      break; 
  }
  return(_Color(r,g,b));
}

void LEDGroupState::_FillSolid( uint32_t color )
{
  int LEDend = mLEDoffset + mLEDcount;
  
  if ( mLPD8806Driver )
  {   
     for(int i=mLEDoffset; i<LEDend; i++)
     {
       mLPD8806Driver->setPixelColor(i, color );
     }
  }
  else
  {
     for(int i=mLEDoffset; i<LEDend; i++)
     {
       mNeoPixelDriver->setPixelColor(i, color );
     }
  }
}

void LEDGroupState::_FillRainbow( uint16_t wheelOffset )
{
  int wheelSkip = (128*3) / mLEDcount;
  
  int LEDend = mLEDoffset + mLEDcount;
  
  if ( mLPD8806Driver )
  {   
     for(int i=mLEDoffset; i<LEDend; i++)
     {
       mLPD8806Driver->setPixelColor(i, _ColorWheel(wheelOffset) );
       wheelOffset += wheelSkip;
     }
  }
  else
  {
     for(int i=mLEDoffset; i<LEDend; i++)
     {
       mNeoPixelDriver->setPixelColor(i, _ColorWheel(wheelOffset) );
       wheelOffset += wheelSkip;
     }
  }
}

void LEDGroupState::_FillMeter( uint32_t color, uint8_t meterValue )
{
  int LEDendOn = mLEDoffset + meterValue;
  int LEDend = mLEDoffset + mLEDcount;
  
  if ( mLPD8806Driver )
  {   
     for(int i=mLEDoffset; i<LEDend; i++)
     {
       if ( i < LEDendOn )
         mLPD8806Driver->setPixelColor(i, color );
       else
         mLPD8806Driver->setPixelColor(i, 0 );       
     }
  }
  else
  {
     for(int i=mLEDoffset; i<LEDend; i++)
     {
       if ( i < LEDendOn )
         mNeoPixelDriver->setPixelColor(i, color );
       else
         mNeoPixelDriver->setPixelColor(i, 0 );
     }
  }
}

