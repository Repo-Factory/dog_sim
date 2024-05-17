# DiceLabs GO1 Simulation

## File Structure
You'll find all the relevant code you need [here](Assest/Scripts). It contains a PathPlanning Script which will control the pipeline of the dog's movement. This can be thought of as the 'main' program that will be run and interact with other services provided from [this codebase](https://github.com/DiceLabs/go1) to perform all the needed functionality.
DogInterface.cs is what acts as a socket bridge program from Unity to the python code in the other codebase. 