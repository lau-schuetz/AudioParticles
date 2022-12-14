// objects for dream mode
SinOsc osc => JCRev r => ADSR env => dac;
[0,2,4,7,9,12] @=> int major[];
60 => int offset; //C4

// set initial duration and osc gain
1::second => dur playTime;
1 => env.keyOn;
0.2 => osc.gain;


// ------------- bells loop ------------ //

while( true )
{
    // play dreamy sound
    Math.random2(0, 5) => int select_ran;
    Std.mtof(major[select_ran] + offset) => osc.freq;
    playTime => now;
}
