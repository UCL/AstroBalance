# AstroBalance
<img width="128" height="128" alt="image" src="https://github.com/user-attachments/assets/632d0b65-3cc4-4888-be51-d3a4afcd0e34" />


## Setup

 - Install unity editor version `6000.0.63f1` (LTS)
 - Download the Tobii game development API. This code was built and tested with version 9.0.4.26.
 - You need to add `.net/Tobii.GameIntegration.Net.dll` and all the `.dll` and `.lib` files in the `lib` folder to your project in the `Assets/Tobii API` folder.

You may need to [download the drivers](https://www.gaming.tobii.com/getstarted/) for the Tobii Eyetracker 5, and calibrate the tracker using the Tobii Experience app.

## Documentation

Documentation for each mini-game is provided under [`docs/`](/docs/), focusing on how to edit important values during play testing.

## Developing

We use [csharpier](https://csharpier.com/) to enforce code style on pull requests. To help with formatting we have added a pre-commit configuration in `.pre-commit-config.yaml`. We have not added pre-commit hooks, so you can commit to a branch without a forced style check. You can run `pre-commit` to reformat staged files, or `pre-commit run --all-files` at any time to fix all files. We recommend using [prek](https://github.com/j178/prek) as a faster alternative `prek` or `prek run --all-files`.

## Acknowledgements

The Dionaea Fonts (DOTMATRIX) are made by Svein K�re Gunnarsoni. http://www.dionaea.com/information/fonts.html. The fonts are freeware

Funded by [Ménière’s & Vestibular UK](https://www.meandve.org.uk/).
