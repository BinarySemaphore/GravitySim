# Gravity Sim #


## Information ##
* **Author**: BinarySemaphore
* **Version**: 0.9.8
* **Description**: N-Body Simulation


## Published Builds ##
* Windows: [Release](../../releases/tag/release)
* WebGL: [Play.Unity](https://play.unity.com/mg/other/gravitysim)


## Controls ##

### Movement ###
| Key   | Action    |
| ----- |:---------:|
| W     | Forward   |
| S     | Backward  |
| A     | Left      |
| D     | Right     |
| Shift | Up        |
| Ctrl  | Down      |

> Movement is relative to the orientation of the camera
_________
> Movement speed is relative to zoom (further out is faster)
_________
> Movement is disabled if camera is in Tracking [Mode](#modes)

### Camera ###
With Left Click Down:

| Cursor Movement | Action                               |
| --------------- |:------------------------------------:|
| Horizontal      | Rotate camera about Y axis           |
| Vertical        | Rotate camera about X+Z axis (Pitch) |

> Rotation is plane-aligned with the x and z axis; y axis is always vertical to perspective

| Mouse Wheel     | Action   |
| --------------- |:--------:|
| Scroll Down     | Zoom In  |
| Scroll Up       | Zoom Out |

> Zoom is logarithmic

### Modes ###
1. Free Mode: Free to use movement to position camera focus anywhere
2. Tracking Mode: Track an object

| Key   | Action                              |
| ----- |:-----------------------------------:|
| R     | Reset focus to starting position    |
| F     | Enable Free Mode                    |
| -     | Cycle Tracking Mode previous object |
| =     | Cycle Tracking Mode next object     |
| 1-9   | Set Tracking Mode on object by mass |

> Tracking with 1-9, targets objects with the current highest masses (1: most mass; 9: 9th most mass)

### Other ###
| Key    | Action                      |
| ------ |:---------------------------:|
| Comma  | Decrease time factor        |
| Period | Increase time factor        |
| Escape | Open Menu or Resume         |
| Q      | While in menu; quit program |

> Time factors are 1, 2, 4, 8, and 16; Larger time factors reduce accuracy
_________
> The Menu will pause the simulation until you resume


## Menu ##
1. __Exit Game__: Quit the program.
2. __Reset__: Reset simulation to original or loaded universe settings
3. __Load__: Bring up load game sub-menu
4. __Save__: Bring up save game sub-menu
5. __Action View__: Hide menu and use normal free/tracking mode camera; `Escape` or `Space` to return to Main Menu
6. __Resume__: Resume simulation


## Configuration ##
Configuration can be set in a JSON file labeled `universe.json` existing in the same directory as the EXE.

### Reference ###
| Label                   | Default         | Limits          | Description |
| ----------------------- |:---------------:| :-------------: | ----------- |
| massTransferRate        | 25              | 1 to Infinity   | The mass transfer coefficient to mass difference transfer rate. Higher values means smaller objects are consumed by larger objects at a faster rate. |
| massTransferRestriction | 50000           | 1 to Infinity   | Restrict mass transfer due to relative velocity at contact. Higher values means less mass can transfer while there is relative moment between two or more colliding objects. Implies grouped objects must settle before forming into a unified object. |
| heatScale               | 0.1             | 0.0 to 1.0      | Heat coefficient applied between two or more particles during mass transfer. Higher values means less mass transfer to produce heat (faster red coloring effect). |
| tailEmissionByRate      | 8.0             | 0.0 to 50.0     | Number of tail particle effects to produce per 1 meter per second of object/particle's velocity. |
| gravConst               | 0.0006674       | 0 to Infinity   | Gravitational Constant. |
| particleDensity         | 990.0           | 1.0 to Infinity | Density used to calculate volume of the objects by mass.  Lower density values means larger volume |
| particleCount           | 50              | 0 to Many       | Initial Particle/Object count. |
| initialRadius           | 550             | 10 to Infinity  | Boundary for randomly setting initial starting points of particles/objects.       |
| initialDistribution     | [2, 1, 1, 1, 1] | 0 to 500 Each   | Distribution of initial locations of objects inside `initialRadius` boundary |
| initialMass.min         | 100             | 1 to Infinity   | Lower bounds for random initial mass given to objects/particles. |
| initialMass.max         | 500             | 1 to Infinity   |Upper bounds for random initial mass given to objects/particles. |

> All values are floats/decimal values
_________
> The `massTransferRate` and `massTransferRestriction` are inversely proportional to one another; to keep ratio, if both must increase by same amount
_________
> For `heatScale`, Heat is not currently dissipated. Any heat (red coloring) an object picks up, it will keep
_________
> The `tailEmissionByRate` can affect performance, zero disables tail effect
_________
> The `gravConst` is the `G` coefficient in the equation `Force = G * (mass1 * mass2) / distance^2`. Default value is accurate to real life except by a factor of 1*10^6 (to speed things up)
_________
> The `particleDensity` default is water on average between 0c and 90c.
_________
> Currently N-Body simulation is partial brute force with O(n!), not Quad-tree, so large numbers of `particalCount` can kill performance
_________
> The `initialDistribution` sets random weights from origin of the radius, `initialRadius`, of a sphere by random ranges [0% to 100%, 0% to 80%, 0% to 60%, 0% to 40%, 0% to 20%, 0% to 1%].  Suggest not messing with this.

### Example ###
```
{
    "massTransferRate": 25.0,
    "massTransferRestriction": 50000.0,
    "heatScale": 0.01,
    "tailEmissionRate": 8.0,
    "gravConst": 0.0006674,
    "particleDensity": 990.0,
    "particleCount": 50,
    "initialRadius": 100.0,
    "initialDistribution": [
        2, 1, 1, 1, 1
    ],
    "initialMass": {
        "min": 100.0,
        "max": 500.0
    }
}
```
