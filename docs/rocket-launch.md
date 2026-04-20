# Rocket Launch

The rocket launch mini-game requires the player to hold a steady gaze at a target object whilst 
either shaking or nodding their head. If the gaze is steady and a minimum head speed is maintained 
then a timer decrements until launch time is achieved.

## Main objects / values to edit during play testing

- **LaunchController**: Attached to the rocket object. Controls the overall launch process.
  - Launch time: The starting launch time in seconds. May be increased by adaptive difficulty features below.
  - **Head Movement Variables**: Determine the amount of head movement required to decrement the timer.
    - Head Pose Buffer Capacity (n) and speed time (s): head speed is measured as the average change in pitch or yaw over the time period speed time. The buffer will need to be sufficiently large to support the time based on game frame rate.
    - Shake Speed Reduction: Because it is possible (for me at least) to shake my head quicker than I can nod, there is a scaling factor between the head speeds required for shaking or nodding. Setting to 0.5 for example means that the player must shake their head twice as fast as nodding to achieve the same effect. 
  - **Steady Gaze Variables**: Determine how steady the gave must be to decrement the timer.
    - Timer Duration: How long (in seconds) the player must maintain a steady gaze to increment the count down code display.
    - Gaze Pose Buffer Capacity (n) and gaze time (s), gaze steadiness is measured as the standard deviation of gaze over gaze time seconds. The buffer will need to be sufficiently large to support the time based on game frame rate.
    - Gaze Tolerance - the allowable gaze standard deviation to be steady. Smaller number will require steadier gaze. This may be reduced by the adaptive difficulty settings below.
    - Target Object if this is set you are required to look at that object, if not gaze can be anywhere on screen but must be steady.

  - **User Interface Items**:
    - Count down sprites: A list of sprites to use for the count down code display.
    - Instructions Text: A text box to place the instruction text.
    - Win Screen: Screen to show a successful launch.
  - **Launch Speed Variables**: Control rocket behaviour at launch.
    - acceleration: The acceleration of the rocket when it launches.
  
  - **Debugging Variables**: Intended for debugging only.
    - Use Mouse For Tracker: Can be used for debugging when no Tobii eye tracker is available.
    - Gaze and Speed Status Text: Text boxes where we can write debugging information to screen.

- **FlameController**: Attached to the flame object, which is a child of the rocker object.
  - Flicker Amplitude and frequency. At rest the flame will flicker slightly to match the aesthetics of other levels. Amplitude and frequency can be altered.
  - Flame Speed Scale: The size of the flame will increase as the head speed increases. Increasing the scale will create a bigger flame.
  - Flame Speed move: As the flame grows we also need to move it down relative to the rocket in order to prevent the flame appearing to come out of the top of the rocket. Faster head speeds or a larger flame speed scale will require a larger value for flame speed move.

- **SmokeController**: Attached to ground left/right emitter.
  - Smoke Emission Scale: A larger value will increase the amount of smoke emitted for a given head speed.
