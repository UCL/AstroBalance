# Star Collector

The star collector mini-game uses a ship (controlled by head rotation) to collect a wave of stars falling from the
top of the screen.

## Main objects / values to edit during play testing

- **StarCollectorManager**: values related to the overall win condition of the game
  - Min / max time limit in seconds
  - Time limit increment in seconds (if time limit upgrade % is met)
  - Number of games in a row that must meet the upgrade % to increase the time limit
  - The length of the time window used to evaluate player performance
  - The % of stars that must be collected to increase speed or time limit
 
- **StarGenerator**: values related to generation of the wave of stars
  - Min, max and base star speed
  - The amount the star speed increases per upgrade
  - Shape of the wave (e.g. width, swerve, star sampling)

- **Ship**: values related to ship movement
  - The amount the ship moves per degree of head movement (X By Degrees)