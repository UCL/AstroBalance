# Session summary

An overview of every time `Astrobalance` has been played is provided in the `SessionSummary.csv` file.
There is one row per session (which is the time from launching Unity, to quitting the application), with the 
following values:

- `sessionNumber`: a unique id per session
- `sessionDate`: the date of the session in format YYYY-MM-DD
- `sessionStartTime`: the session start time in format HH:MM:ss. This is the local time (e.g. if your computer is set to UK time - this is UK time).
- `sessionEndTime`: the session end time in format HH:MM:ss (local time - see sessionStartTime description)
- `totalSessionDurationMinutes`: total number of minutes in this session..
- `game1RocketLaunchPlayed`: whether a _complete_ game of rocket launch was played in this session.
- `game2StarCollectorPlayed`: whether a _complete_ game of star collector was played in this session.
- `game3StarSeekPlayed`: whether a _complete_ game of star seek was played in this session.
- `game4StarMapPlayed`: whether a _complete_ game of star map was played in this session.
- `game5SpaceWalkPlayed`: whether a _complete_ game of space walk was played in this session.
- `game6ZeroGravityPlayed`: whether a _complete_ game of zero gravity was played in this session.
- `totalGamesPlayed`: total number of mini-games played. Maximum is 6 (playing the same game multiple times doesn't count)