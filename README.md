# AudioParticles
 
# What It Is
Particle Paint is an expressive tool inviting the player to reflect on the ephemerality of life. The tool allows the creation of an ephemeral drawing using sound.
The tool includes a pause mode, providing the player the option to escape the ephemerality of life. When on hold, the drawing can be enhanced by adding strokes to a seemingly constant canvas. Once the player exits the pause mode, the drawing faints.
Particle Paint further contains a secret mode only visible to the attentive. Unlike the expressive nature of the tool, this mode invites the user to sit and contemplate. An audiovisual drawing unflods in front of the user's eyes; the colors maped to the sound's pitch.

# How It Works
During play time the sound is modified using keyboard input. The pitch, beat and volume of the oscillators can be adjusted. Altering the audio parameters changes the stroke and color of the paint brush. The spread of the brush increases and decreases with the audio volume. Higher volume results in a larger brush diameter and vice versa. The color of the paint is determined by the sound's pitch. The pitch-to-color mapping results in a color spectrum ranging from red to violet. A low pitch generates the color red, a high pitch creates violet paint.

The tool is built using Unity and Chunity to generate the audiovisual experience. The audio programming language ChucK is used to generate the sound. Unity is used to create the visual effects. An audio waveform and spectrum analysis is run in Unity to create the visual response to the change in sound. When user input occurs, an event is broadcasted to Chuck to modify the sound, which in turn modifies the visuals in Unity. The project was built for Mac OS - Apple Silicon and Intel 64-bit.

# How To Play
N - Start playing oscillator audio  
Right Arrow / Left Arrow - spped up / slow down beat  
W / S - turn volume up / down  
D / A - adjust pitch up / down  
C - switch between Pause Mode and Flow Mode  
Y - switch between paint brush and eraser  
X - change into Contemplative Mode  
B - start playing calm audio  

Reference-style: 
![alt text][img1]
![alt text][img2]

[img1]: https://github.com/lau-schuetz/AudioParticles/Images/ephemeralMode
[img2]: https://github.com/lau-schuetz/AudioParticles/Images/secretMode2

# Ressources
Tutorials:  
[Particles - Unity VFX Graph](https://www.youtube.com/watch?v=FvZNVQuLDjI)   
[Particle System Trails | Unity Particle Effects](https://www.youtube.com/watch?v=agr-QEsYwD0)  
[Map value range to RGB rainbow-colormap](https://stackoverflow.com/questions/37876316/map-value-range-to-rainbow-colormap)
[Start Menu in Unity](https://www.youtube.com/watch?v=zc8ac_qUXQY)  
[How to fade UI in Unity | Unity Fading Tutorial](https://www.youtube.com/watch?v=tF9RMjF9wDc)  
Assets:  
[Dark Theme UI](https://assetstore.unity.com/packages/2d/gui/dark-theme-ui-199010)  
