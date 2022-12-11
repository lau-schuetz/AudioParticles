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

// interpret input as frequency modulation
2 => sintri.sync;
100  => trictrl.gain;

0 => int freqMod;

// global (incoming) data from Unity
0.05::second => dur playTime;
// index for semi-tones array
0 => global int ck_freq_mod;
global Event changeFreq;


// array of Oscs
[ sin, saw, tri, pul, trictrl ] @=> Osc oscillators[];

// set gains
0.2 => sin.gain;
0.2 => saw.gain;
0.1 => tri.gain;
//0.1 => tri2.gain;
0.1 => pul.gain;
0.1 => sintri.gain;

// spork edit listener
spork ~ listenForFreqMod();

// this listens for events from Unity to update the sequence
fun void listenForFreqMod()
{
    while( true )
    {
        // wait for event
        changeFreq => now;
        // update the frequency of one osc
        ck_freq_mod => freqMod;
        <<< "FREQ mod Unity:", ck_freq_mod, "GREQ mod Chuck:", freqMod >>>;
    }
}

// generate new frequenc value
Std.mtof( f[freqMod] + 55 ) => float newnote;
// set frequency
newnote => oscillators[0].freq;
// wait a bit
0.5::second => now;

// 10 times
repeat(10)
{
    Math.random2f( 0.2, 0.8 ) => trictrl.width;
    // <<< "trictrl width:", trictrl.width() >>>;
    //ck_tempo::second => now;
    0.20::second => now;
}


// generate new frequenc value
Std.mtof( f[freqMod] + 55 ) => newnote;
// set frequency
newnote => oscillators[0].freq;
// wait a bit
0.5::second => now;

    // 10 times
    repeat(10)
    {
        Math.random2f( 0.2, 0.8 ) => trictrl.width;
        // <<< "trictrl width:", trictrl.width() >>>;
        //ck_tempo::second => now;
        0.20::second => now;
    }
