# Honours Project

This project is a behaviour simulation that compares the performance between Binary and Fuzzy versions of a behaviour logic.

The user can select one of 7 preset simulation files to run in the simulation, and toggle which version of the behaviour logic to use. 

# Simulation Flow
NPCs will spawn in one of two ways depending on the population count: 
- Under 100, NPCs have an assigned seat number to spawn in one of the 100 pre-placed spawn points
- Above 100, NPCs have an assigned (randomised in file creation) spawn point, centred around the spawn object and on a valid navmesh point

The criminal follows a set path and triggers the crime response, beginning the behaviour simulation cycle
NPCs have one of three possible reactions: 
- Fight,
- Flight, 
- Freeze.

# Traits Influence
In the binary version, NPC traits (bravery, distress, etc.) have a value between 0-10, if these values are above 5, then the corresponding trait is true. 

In the fuzzy version, NPC traits are normalised between 0f-1f and have a weighting on the corresponding scores of how likely an NPC is to choose each reaction.