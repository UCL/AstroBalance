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
