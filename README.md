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

We use [CSharpier](https://csharpier.com/) to enforce code style on pull requests. 
To help with formatting we have added a pre-commit configuration in `.pre-commit-config.yaml`. 
In order to use the pre-commit you first need to install a version of `.NET SDK` (We've tested with 10.0.102, but other 
versions may work).

 - [Instructions for installing .NET SDK](https://aka.ms/dotnet/download).

We recommend using [prek](https://github.com/j178/prek) as an alternative to pre-commit. 
Run: 
```
prek                  # to reformat staged files, or
prek run --all-files  # at any time to fix all files.
```
Or with pre-commit:
```
pre-commit                  # to reformat staged files, or
pre-commit run --all-files  # at any time to fix all files.
```

Optionally you can run `pre-commit install` or `prek install` to run these checks automatically on every commit.

## Acknowledgements

The Dionaea Fonts (DOTMATRIX) are made by Svein K�re Gunnarsoni. http://www.dionaea.com/information/fonts.html. The fonts are freeware

Funded by [Ménière’s & Vestibular UK](https://www.meandve.org.uk/).
