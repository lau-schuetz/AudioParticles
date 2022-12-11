// oscillator demo
// - philipd

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

// global (incoming) data from Unity
0.05::second => dur playTime;
//0.15 => global float ck_tempo;
60.0 => global float ck_freq;

// array of Oscs
[sin, saw, tri, pul, trictrl] @=> Osc oscillators[];

// set gains
0.2 => sin.gain;
0.1 => saw.gain;
0.1 => tri.gain;
//0.1 => tri2.gain;
0.1 => pul.gain;
0.1 => sintri.gain;

// infinite time-loop
while( true )
{ 
    // randomize
    Math.random2(0,7) => int select;
    // clamp (giving more weight to 5)
    if( select > 4 ) 4 => select;
    // generate new frequenc value
    Std.mtof( f[Math.random2( 0, 4 )] + 55 ) => float newnote;
    // set frequency
    newnote => oscillators[select].freq;
    // wait a bit
    0.25::second => now;
    // 10 times
    repeat(10)
    {
        Math.random2f( 0.2, 0.8 ) => trictrl.width;
        // <<< "trictrl width:", trictrl.width() >>>;
        //ck_tempo::second => now;
        0.20::second => now;
    }
}
