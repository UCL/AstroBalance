# Rocket Launch

The rocket launch mini-game increases the size of the rocket launch flame based on head movement (pitch or yaw)
and gaze steadiness.

## Main objects / values to edit during play testing

- **LaunchController**: Attached to the rocket object. Varies flame size depending on head motion and contains overall timer.
  - Launch time: The overall time in seconds that the level takes.
  - Head Pose Buffer Capacity (n) and speed time (s): head speed is measured as the average change in pitch or yaw over the time period speed time. The buffer will need to be sufficiently large to support the time based on game frame rate.
  - speed scale: Scales the size of the flame based on head speed. A bigger number means a bigger flame.
  - acceleration: The acceleration of the rocket when it launches.
  

- **LaunchCode**: Controls the random launch code and gaze steadiness
  - Gaze Pose Buffer Capacity (n) and gaze time (s), gaze steadiness is measured as the standard deviation of gaze over gaze time seconds. The buffer will need to be sufficiently large to support the time based on game frame rate.
  - Gaze Tolerance - the allowable gaze standard deviation to be steady. Smaller number will require steadier gaze.
  - Target Object if this is set you are required to look at that object, if not gaze can be anywhere on screen but must be steady.
