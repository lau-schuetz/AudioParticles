// scale degrees in semi-tones
[ 0, 2, 4, 7, 9 ] @=> int f[];

// a mix of oscialltors
SinOsc sin => dac;
SawOsc saw => dac;
TriOsc tri => dac;
PulseOsc pul => dac;
// modulator and carrier
TriOsc trictrl => SinOsc sintri => dac;

// objects for dream mode
SinOsc osc => JCRev r => ADSR env => dac;
[0,2,4,7,9,12] @=> int major[];
[0,3,7,12,15,17] @=> int minor[];
60 => int offset; //C4
85 => int offset2;

// interpret input as frequency modulation
2 => sintri.sync;
100  => trictrl.gain;

// set initial duration and frequency of osc
0.20::second => dur curPlayTime => dur playTime;
55 => int baseFreq;

// varibales across Chuck and Unity
0.20 => global float ck_tempo;
60.0 => global float ck_freq;

// incoming events from Unity
global Event changeTempoUp;
global Event changeTempoDown;
global Event changeFreqUp;
global Event changeFreqDown;
global Event changeGainUp;
global Event changeGainDown;
global Event dreamOn;
global Event dreamOff;

// array of oscs
[sin, saw, tri, pul, trictrl] @=> Osc oscillators[];

// set gains
0.2 => float sinGain => sin.gain;
0.1 => float sawGain => saw.gain;
0.1 => float triGain => tri.gain;
0.1 => float pulGain => pul.gain;
0.1 => float sintriGain => sintri.gain;

// spork listeners
spork ~ listenForTempoUp();
spork ~ listenForTempoDown();
spork ~ listenForFreqUp();
spork ~ listenForFreqDown();
spork ~ listenForGainUp();
spork ~ listenForGainDown();
spork ~ listenForDreamOn();
spork ~ listenForDreamOff();


// ------------- listener funcs ------------ //

// listens for event to speed up tempo
fun void listenForTempoUp()
{
    while( true )
    {
        // wait for event
        changeTempoUp => now;
        // update the tempo
        playTime / 2.0 => curPlayTime => playTime;
        <<< "tempo mod Chuck:", playTime >>>;
    }
}

// listens for event to slow down tempo
fun void listenForTempoDown()
{
    while( true )
    {
        // wait for event
        changeTempoDown => now;
        // update the tempo
        playTime * 2.0 => curPlayTime => playTime;
        <<< "tempo mod Chuck:", playTime >>>;
    }
}

// listens for event to increase base freq
fun void listenForFreqUp()
{
    while( true )
    {
        // wait for event
        changeFreqUp => now;
        // update the frequency
        2 +=> baseFreq;
        <<< "freq mod Chuck:", baseFreq>>>;
    }
}

// listens for event to decrease base freq
fun void listenForFreqDown()
{
    while( true )
    {
        // wait for event
        changeFreqDown => now;
        // update the frequency
        2 -=> baseFreq;
        <<< "freq mod Chuck:", baseFreq>>>;
    }
}

// listens for event to increase gain / volume
fun void listenForGainUp()
{
    while( true )
    {
        // wait for event
        changeGainUp => now;
        // update the gain
        //if (sinGain < 0.35)
        //{
            sinGain + 0.01 => sinGain => sin.gain;
            sawGain + 0.01 => sawGain => saw.gain;
            triGain + 0.01 => triGain => tri.gain;
            pulGain + 0.01 => pulGain => pul.gain;
            sintriGain + 0.01 => sintriGain => sintri.gain;
            <<< "gain mod up Chuck:", sin.gain >>>;
        //}
    }
}

// listens for event to decrease gain / volume
fun void listenForGainDown()
{
    while( true )
    {
        // wait for event
        changeGainDown => now;
        // update the gain
        //if (sinGain > 0.1)
        //{
            sinGain - 0.01 => sinGain => sin.gain;
            sawGain - 0.01 => sawGain => saw.gain;
            triGain - 0.01 => triGain => tri.gain;
            pulGain - 0.01 => pulGain => pul.gain;
            sintriGain - 0.01 => sintriGain => sintri.gain;
            <<< "gain mod down Chuck:", sin.gain >>>;
        //}
    }
}

// listens for event to enter dream mode
fun void listenForDreamOn()
{
    while( true )
    {
        // wait for event
        dreamOn => now;
        1 => env.keyOn;
        
        // reduce to two oscillators
        0.03 => saw.gain;
        0.03 => pul.gain;
        0.5 => osc.gain;
        
        // slow down beat
        1.5::second  => playTime;
        
        while(true)
        {
            // play dreamy sound
            Math.random2(0, 5) => int select_ran;
            Std.mtof(major[select_ran] + offset) => osc.freq;
            playTime => now;
        }
    }
}

// listens for event to exit dream mode
fun void listenForDreamOff()
{
    while( true )
    {
        // wait for event
        dreamOff => now;
        // reduce to two oscillators
        0.0 => osc.gain;
        sawGain => saw.gain;
        pulGain => pul.gain;     
    }
}

// ------------- osc loop ------------ //

// infinite time-loop
while( true )
{ 
    // randomize which osc
    Math.random2(0,7) => int select;
    // clamp (giving more weight to 5)
    if( select > 4 ) 4 => select;
    // generate new frequency value
    Std.mtof( f[Math.random2( 0, 4 )] + baseFreq ) => float newnote;
    // set freq of randomly selected osc
    newnote => oscillators[select].freq;
    // wait a bit
    0.25::second => now;
    // play 10 times
    repeat(10)
    {
        Math.random2f( 0.2, 0.8 ) => trictrl.width;
        playTime => now;
    }
}
