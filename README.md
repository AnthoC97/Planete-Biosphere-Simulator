# Planet Biosphere Simulator

Planet Biosphere Simulator is a Unity3D project.

## Installation

- Install Unity 2019.3.4f1.
- Go to Window -> Package Manager:
  - Install Universal  RP 7.1.8.
  - Install Lightweight RP 7.2.1.
  - Install Shader Graph 7.1.8.

## Build

Don't use the "Build And Run" option from Unity Editor. Instead, follow those
steps:

- build the project in Unity Editor via the "Build Settings" window, for the
  target of your choice
- copy the Lua scripts that will be needed for the simulation to a `lua` folder
  in the `_Data` directory of the build
- run the executable manually

The reason for this is that the "Build And Run" option will empty the
`_Data/lua` folder during the build process. Thus, no script will be available
when the software is run with this method.

## Usage

Upon running the executable, you will be presented with the main menu. This menu
allows you to edit the parameters that will be used for generating a planet.

Tweak those parameters to your liking and click the "Generate" button. The
planet generation process will begin. This process can take some time as it will
try to generate a planet that matches your criteria using a genetic algorithm.
For the "Generation Type", you should select the "scorer.lua" script or some
appropriate script.

Once a good planet has been generated, the vegetation will start generating on
the planet's surface. When this process is done, you can look around using your
mouse and generate creatures using the creatures generation panel.

## Controls

During the simulation, drag and drop inside the simulation view to move the
orbital camera.

Press 'c' to open the creatures generation panel. Press 'Escape' to close it.
