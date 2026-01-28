# Zero Gravity

The zero gravity mini-game asks the player to copy various poses, and awards points for keeping still in each.

## Main objects / values to edit during play testing

- **ZeroGravityManager**: values related to timers and scores
  - The number of seconds each pose is displayed
  - The number of seconds of countdown before pose hold begins
  - The number of seconds each pose must be held
  - The score awarded for a hold of a chosen number of seconds
  - The number of seconds the pose must be held for a score increase

- **SwayLine**: values related to movement of the sway line
  - The number of unity units moved per mm of head movement
  - The limit of head movement before scoring stops
  - The colour when the head is in / out of range

- **PoseAvatar**: values related to display of poses
  - Sprites for each pose
  - Text explanations for each pose