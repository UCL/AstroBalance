# Rocket Launch

The rocker launch mini-game increases the size of the rocket launch flame based on head movement (pitch or yaw)
and gaze steadiness.

## Main objects / values to edit during play testing

- **LaunchController**: values related to gaze steadiness, movement speed, game time.
  - Target Object - This is the object that you're supposed to look at. Currently set to the launch code text. [Currently boroken, target object coordinates are in pixels, but tracker is not].
  - Gaze Buffer Capacity (n) and gaze time (s), the game uses a buffer to store gaze values, then assesses stability based on the standard deviation of gaze points from the target point over the time period gaze time. The buffer will need to be sufficiently large to support the time based on game frame rate.
  - Head Pose Buffer Capacity (n) and speed time (s), head speed is measured as the average change in pitch or yaw over the time period speed time. The buffer will need to be sufficiently large to support the time based on game frame rate.
  - Gaze Tolerance - the allowable gaze standard deviation to be steady. Smaller number will require steadier gaze.

