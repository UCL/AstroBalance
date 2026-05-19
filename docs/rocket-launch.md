# Rocket Launch

The rocket launch mini-game requires the player to hold a steady gaze at a target object whilst 
either shaking or nodding their head. If the gaze is steady and a minimum head speed is maintained 
then a timer decrements until launch time is achieved.

## Main objects / values to edit during play testing

- **LaunchController**: Attached to the rocket object. Controls the overall launch process.
  - Launch time: The starting launch time in seconds. May be increased by adaptive difficulty features below.
  - **Head Movement Variables**: Determine the amount of head movement required to decrement the timer.
    - Head Pose Buffer Capacity (n) and speed time (s): head speed is measured as the average change in pitch or yaw over the time period speed time. The buffer will need to be sufficiently large to support the time based on game frame rate.
    - Minimum head speed (pitch or yaw) required to reduce the launch timer. The yaw speed is set higher than pitch, because it is possible (for me at least) to shake my head quicker than I can nod.
  - **Steady Gaze Variables**: Determine how steady the gave must be to decrement the timer.
    - Timer Duration: How long (in seconds) the player must maintain a steady gaze to increment the count down code display.
    - Gaze Pose Buffer Capacity (n) and gaze time (s), gaze steadiness is measured as the standard deviation of gaze over gaze time seconds. The buffer will need to be sufficiently large to support the time based on game frame rate.
    - Gaze Tolerance - the allowable gaze standard deviation to be steady. Smaller number will require steadier gaze. This may be reduced by the adaptive difficulty settings below.
    - Target Object - if this is set you are required to look at that object, if not gaze can be anywhere on screen but must be steady. The size of the target object will be matched to the gaze tolerance.

  - **Adaptive difficulty variables**
    - Max previous games: The maximum number of previous games to retrieve to determine experience based difficulty
    - Adaptive difficulty: integer describing level of adaptive difficulty - higher numbers are more difficult.

  - **Save data variables**
    - Sampling interval: the interval in seconds between samples for the save data. 

  - **User Interface Items**:
    - Count down sprites: A list of sprites to use for the count down code display.
    - Instructions Text: A text box to place the instruction text.
    - Win Screen: Screen to show a successful launch.
  
  - **Launch Speed Variables**: Control rocket behaviour at launch.
    - acceleration: The acceleration of the rocket when it launches.
  
  - **Debugging Variables**: Intended for debugging only.
    - Use Mouse For Tracker: Can be used for debugging when no Tobii eye tracker is available.
    - Gaze and Speed Status Text: Text boxes where we can write debugging information to screen.

- **FlameController**: Attached to the flame object, which is a child of the rocket object.
  - Flicker Amplitude and frequency. At rest the flame will flicker slightly to match the aesthetics of other levels. Amplitude and frequency can be altered.
  - Flame Speed Scale: The size of the flame will increase as the head speed increases. Increasing the scale will create a bigger flame.
  - Flame Speed move: As the flame grows we also need to move it down relative to the rocket in order to prevent the flame appearing to come out of the top of the rocket. Faster head speeds or a larger flame speed scale will require a larger value for flame speed move.

- **SmokeController**: Attached to ground left/right emitter.
  - Smoke Emission Scale: A larger value will increase the amount of smoke emitted for a given head speed.

## Adaptive difficulty

Difficulty is increased between games by reducing the gaze tolerance, and increasing the overall launch time (explained below). Note: all parameters mentioned below can be adjusted on `LaunchControl`. 

At the start of each game, a scaling factor is calculated as: 

`adaptiveDifficulty` * ( (`maxPreviousGames` + `nGames`) / `maxPreviousGames`) 

`nGames` is the total number of rocket launch games completed by the player so far (up to a maximum of `maxPreviousGames`). The scaling factor is then applied:
- the gaze tolerance (i.e. the tolerance in unity coordinates that gaze needs to stay within) is divided by the scaling factor. This will also scale the gaze target (the box displaying the launch code numbers) to match. This means the player must keep their gaze closer to the target after more games have been played.
- the launch time is multiplied by the scaling factor. This means the player must move their head while looking at the target for longer to launch the rocket.

## Save data

Data is saved to `RocketLaunchScores.csv`, with one row per played game. Values are:

- `gameNumber`: a unique id per played rocket launch game
- `sessionNumber`: the session this game was played in (corresponds to sessionNumber in [`SessionSummary.csv`](./session-summary.md))
- `date`: the date of the game session in format YYYY-MM-DD
- `startTime`: the game start time in format HH:MM:ss. This is the local time (e.g. if your computer is set to UK time - this is UK time).
- `endTime`: the game end time in format HH:MM:ss (local time - see startTime description)
- `gameCompleted`: whether this game was completed. If they exited early, this will be false.
- `headMovementPlane`: the direction of head movement - either 'pitch' (up-down movement) or 'yaw' (left-right movement).
- `launchTimeSeconds`: the number of seconds the player had to keep their eyes steady on the target while moving their head at the required speed to launch the rocket. Rounded to 2 decimal places.
- `gameDurationSeconds`: how long the game was played (rounded to the nearest second). If the player exited before the game finished, this will be less than launchTimeSeconds; if they played the game through to completion it will be longer than launchTimeSeconds (as this includes time when the player wasn't looking at the target / wasn't moving their head fast enough etc.)
- `minimumHeadSpeed`: the minimum head speed required (while looking at the target) to reduce the launch timer.
- `gazeTolerance`: the maximum number of unity units the gaze can be from the centre of the target, and still be counted as 'on target'. Rounded to 2 decimal places.

**Note**: all measures below are calculated as follows. Every `samplingIntervalSeconds` (a configurable parameter in `LaunchControl`), a gaze steady sample is taken: 

- Gaze steady: a boolean value (either true or false). It will be true if gaze positions in the time period are (on average) less than `gazeTolerance` from the centre of the target.

If `gaze steady = true`, a head speed sample is also taken:

- Head speed: the absolute differences of head angle between consecutive readings in the time period are summed together and divided by the total difference in time to give a speed in degrees per second.

If the player goes out of range at any point in those `samplingIntervalSeconds` (i.e. the tracker can no longer detect them), then all samples for that time period are discarded. 

At the end of the game, all samples are used to calculate the summary statistics below:

- `headSpeedDegPerSecMean`: Mean head rotation speed while the gaze is on target, measured in degrees per second. Rounded to 2 decimal places. 
- `headSpeedDegPerSecPeak`: Peak head head rotation speed while the gaze is on target, measured in degrees per second. Rounded to 2 decimal places. 
- `headSpeedDegPerSecMedian`: Median head rotation speed while the gaze is on target, measured in degrees per second. Rounded to 2 decimal places. 
- `headSpeedDegPerSecSD`: Standard deviation of head rotation speed while the gaze is on target, measured in degrees per second. Rounded to 2 decimal places.
- `percentTimeAbove40DegPerSec` - the % of on-target time, that the head speed was over 40 degrees per second. (this is the % of speed samples that were >40 - speed samples are _only_ taken when the gaze is on target). Rounded to 2 decimal places.
- `percentTimeGazeOnTarget` - the % of time with gaze on target (this is the % of samples with `gaze steady = true` as described above). Rounded to 2 decimal places.

For the adaptation window values below, this is calculated as the number of speed samples in the given range multiplied by the sampling interval in seconds:
- `timeInAdaptationWindow1` - number of seconds with `60 <= head speed < 90` degrees per second and gaze on target. Rounded to 2 decimal places.
- `timeInAdaptationWindow2` - number of seconds with `90 <= head speed < 130` degrees per second and gaze on target. Rounded to 2 decimal places.
- `timeInAdaptationWindow3` - number of seconds with `130 <= head speed < 180` degrees per second and gaze on target. Rounded to 2 decimal places.
- `timeInAdaptationWindow4` - number of seconds with `head speed > 180` degrees per second and gaze on target. Rounded to 2 decimal places.
