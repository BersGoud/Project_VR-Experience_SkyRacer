# Sky Racer

## Introduction

For the VR-Expierence course we were tasked with creating a VR game. We decided to create a game where the player is a pilot of a small airplane. The player has to fly through a series of rings to complete the level. In this document we will explain the process of creating this game.

## Methodology

### Tools

For this project we used Unity to create a game. We used occulus quest 2 to test the game. We used the following assets:

**insert assets here**

### The gameplay loop

The gameplay loop consists of the player being able to fly through checkpoints. When the player flies through a checkpoint they can progress to the next one. When the player crashes they are teleported a couple of meters above. The player can also restart the level by pressing a button in the pause menu. The game is played against an ML agent that tries to fly through the checkpoints as well, simulating a race.

### Actions

The player can perform the following actions:

- Boost (L-trigger)
- Pause/Resume (Y-button)
- Roll left (R-stick left)
- Roll right (R-stick right)
- Yaw left (R-stick up)
- Yaw right (R-stick down)

The ML agent can perform the following actions:

- Move left
- Move right
- Move up
- Move down
- Move forward
- Move backward
- Rotate left
- Rotate right

Oberservations the ML agent can make: #TODO Lander kijken

- Position
- Rotation
- Position of the next checkpoint
- Distance to the next checkpoint

The ML agent is trained using the PPO algorithm and has the following rewards and penalties:

- Big reward for flying through a checkpoint
- Small reward for closing distance to the checkpoint
- Small penalty for gaining distance to the checkpoint
- Penalty for crashing
- Penalty for flying too far away from the checkpoint
- Penalty for flying too slow
- Penalty for going out of bounds

### Installation

#### Version control

Unity: ...
ML-Agents: ...
Anaconda: ...

### Objects

The game consists of the following objects:

- Airplane
- - This airplane is a model we got from sketchfab. We linked the model in the [references](#references) section.
- Checkpoints
- - The checkpoints are objects we created in Unity. They are rings that the player has to fly through. The checkpoints contain following scripts:
- - - Checkpoint.cs
- Ground
- - The ground is a terain we made using the terain tool in Unity.
- Skybox
- - We used a skybox from the Unity asset store. Linked in the [references](#references) section.
- UI
- - The UI contains ...
- ML agent
- Camera
- - The camera system used came with the airplane model. We made some adjustments to the camera system to make it work with the game.
- Lights
- Audio
- - We used a handful of audio files from different sources to make the game feel more realistic and imersive. We linked the sources in the [references](#references) section.

### One-pager

## Results

### Tensorboard

### About the board

### Noteworthy observations

## Conclusion

## References