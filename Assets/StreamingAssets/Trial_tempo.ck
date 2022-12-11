// scale degrees in semi-tones
[ 0, 2, 4, 7, 9 ] @=> int f[];

// various oscialltors
SinOsc sin => dac;
SawOsc saw => dac;
TriOsc tri => dac;
//SqrOsc tri2 => dac;
PulseOsc pul => dac;
// modulator and carrier
TriOsc trictrl => SinOsc sintri => dac;

// objects for dream mode
SinOsc osc => JCRev r => ADSR env => dac;
0.5 => osc.gain;
[0,3,7,12,15,17] @=> int minor[];
85 => int offset;

// interpret input as frequency modulation
2 => sintri.sync;
100  => trictrl.gain;

0.20::second => dur curPlayTime => dur playTime;
55 => int baseFreq;
// global (incoming) data from Unity
0.20 => global float ck_tempo;
60.0 => global float ck_freq;
global Event changeTempoUp;
global Event changeTempoDown;
global Event changeFreqUp;
global Event changeFreqDown;
global Event changeGainUp;
global Event changeGainDown;
global Event dreamOn;
global Event dreamOff;

// array of Oscs
[sin, saw, tri, pul, trictrl] @=> Osc oscillators[];

// set gains
0.2 => float sinGain => sin.gain;
0.1 => float sawGain => saw.gain;
0.1 => float triGain => tri.gain;
//0.1 => tri2.gain;
0.1 => float pulGain => pul.gain;
0.1 => float sintriGain => sintri.gain;

// spork listener
spork ~ listenForTempoUp();
spork ~ listenForTempoDown();
spork ~ listenForFreqUp();
spork ~ listenForFreqDown();
spork ~ listenForGainUp();
spork ~ listenForGainDown();
spork ~ listenForDreamOn();
spork ~ listenForDreamOff();

// this listens for events from Unity to update the sequence
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

// this listens for events from Unity to update the sequence
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

// this listens for events from Unity to update the sequence
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


// this listens for events from Unity to update the sequence
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

// this listens for events from Unity to update the sequence
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

fun void listenForDreamOn()
{
    while( true )
    {
        // wait for event
        dreamOn => now;
        
        // reduce to two oscillators
        0.00 => saw.gain;
        0.00 => pul.gain;
        1::second  => playTime; 
        
        while(true)
        {
            // little bells - dreamy
            for(0 => int i; i < 2; i++)
            {
                for(0 => int j; j < 4; j++) {
                    Std.mtof(minor[j] + offset) => osc.freq;
                    1 => env.keyOn;
                    playTime => now;
                }
            }
            1 => env.keyOff;
        }
    }
}

fun void listenForDreamOff()
{
    while( true )
    {
        // wait for event
        dreamOff => now;
        // reduce to two oscillators
        1 => env.keyOff;
        0 => osc.gain;
        sawGain => saw.gain;
        pulGain => pul.gain;
        curPlayTime => playTime;      
    }
}

// infinite time-loop
while( true )
{ 
    // randomize
    Math.random2(0,7) => int select;
    // clamp (giving more weight to 5)
    if( select > 4 ) 4 => select;
    // generate new frequency value
    Std.mtof( f[Math.random2( 0, 4 )] + baseFreq ) => float newnote;
    // set frequency
    newnote => oscillators[select].freq;
    // wait a bit
    0.25::second => now;
    // 10 times
    repeat(10)
    {
        Math.random2f( 0.2, 0.8 ) => trictrl.width;
        // <<< "trictrl width:", trictrl.width() >>>;
        playTime => now;
    }
}
