# cs426_CyberBadddezzz_Asgn4
# Cache me or Not
This is a multiplayer serious game inspired by the "Computer Architecture and Thieves" theme, set inside a computer’s architecture, including input devices, output devices, memory, and a CPU. Two players, a hacker and a defender, compete to destroy or protect the system while answering meaningful questions that teach basic computer architecture. The game features unique obstacles, unusual procedures, and an engaging UI to match the theme.

Drive Link to Window Exe: https://drive.google.com/file/d/1RKmyDmDysm7mdSsXsazD9L8dj8gGHWRK/view?usp=sharing

## Assignment 4 Changes
- Added 2 lights, one blue one red
- Lights pulse
- Added particle breaking to targets(physics, collision)
- Added bounce material to main vault
- Added bounce material to lights
- Added 2 billboards with serious question

## Players

- 2 Players, Host and Client
- Networked in real-time via Unity Netcode
- Players need to be physically next to each other or talk on Discord

---

## Objectives

- Hit input or output targets(depends on if you're a hacker or defender)
- Work within the constraints of the 45s countdown timer
- Progress through the scene before time runs out!

---

## Procedures

Players must:

- Join the session, the game will not start without both players
- Navigate the map to their input/output box
- Hit boxes with bullets
- Complete objectives in the required order.
- Reach progression triggers (portals / scene transitions).

---

## Rules

- The game only begins when 2 players are connected!
- A shared global timer runs for the duration of the round
- Players cannot exceed time limits!
- Turn logic can make it diffcult for both players to hit their targets in time

---

## Resources

- Time (45-second global countdown)
- Player movement
- Bullets
- Scene progression mechanics
- Network connection

---

## Conflict

- Time pressure
- Players must complete tasks before the timer reaches zero
- Players can purposefully take a while to do their one hit to waste time

---

## Boundaries

- The game world is confined to the Unity scene
- Interaction is limited to designated devices and triggers.
- Progression is controlled by portals or scripted events

---

## Outcome

- **Success:** Objectives completed before timer expires.
- **Failure:** Time runs out
- The game state is synchronized for both players.

## Unusual Procedure 
- An unusual procedure is that the combination of the global timer and the turn taking. A game can be time based, and who ever does a certain task the most wins, but the turn taking aspect added in our game is a big limit on both players. Time is constrained and so it their shooting ability so even though they are competing, everytime you shoot your letting your opponenent shoot too, increasing their chance of a win too. Furthermore, the game isn't like a regular shooter game because you are unable to see how many targets you opponent has since you are opposite sides and the objective isn't to shoot your opponent.
---
## Serious Objective
-- The serious objective of the game is to make the players realize how intense the battle can be between a hacker and a defender since they are utilizing the same resources but for opposite purposes, as such the hacker and defender were made with the same prefab. It is also to make the player realize that the memory is the core of the CPU (the middle of the map) and protecting it is critical because it contains data from I/O devices. Once someone has access to the I/O device, they can steal precious data and install malware.

## 📚 References

- Liz Marai's Networking Tutorial
- https://discussions.unity.com/t/how-to-make-a-timer/765622
- https://assetstore.unity.com/packages/3d/characters/robots/drone-guard-175607
- https://assetstore.unity.com/packages/2d/textures-materials/sky/free-skyboxes-sci-fi-fantasy-184932
- https://assetstore.unity.com/packages/3d/environments/sci-fi/sci-fi-styled-modular-pack-82913
- https://youtu.be/m7Jf2ayY7B8?si=tohKu4L_aRvY4ijq


