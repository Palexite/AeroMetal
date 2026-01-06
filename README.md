# AeroMetal
A 3rd person game project made in the S&amp;Box Source 2 Engine API where you play as a powerfully armored person mounted ontop of moving vehicles, incorporating elements of combat, parkour, destruction, and chaotic physics.
Programmed in C#.


Information is limited due to this project being in beginner stages and the best that can be provided is an overview and plan.
A name for this game has not been decided, and "AeroMetal" is most likely a placeholder for it, with milesway being a code-name.


## Overview
In this game, you play as a hero in aerodynamic power armor that enables him to remain standing on moving surfaces and absurdly high speeds.
You start from inside of a vehicle and mount onto it through its hood as a driver assists you throughout assigned missions which vary from seizing operating enemy militant vehicles, to defending VIP vehicles, or stealing cargo.
The environment you play is made up into modular stages which together, randomized, compromise an infinite roadpath.

## Plans

### Mechanics
Everything in the game is objectively physical and dynamic. Even the bystander traffic flow will be physically vulnerable which itself offers unique
obstacles and methods for the player to be wary of. A player could cause a pileup, or collide against a car that an opponent is mounted on, or blow up a vehicle's
propane accessories close to another player, etc. The possibilities are virtually endless.

The game environment works on a dynamic stage generating system. It can generate hills, bridges, turns, and intersections, all of which are predesigned by level designers.
There is no end but the game is always moving. Unlike other systems most games have, where the environment moves instead of the player or mobile objects in the scene,
the environment in this game is not moving and everything else is in order to make it compatible with the physics engine.

Like most objects in the game, the player is also physically vulnerable, and a dynamic ragdoll system may be introduced, but how exactly this will be implemented securely
is unclear as server-based ragdoll physics is an expensive operation, and such addition may lower the tangibility of the game with high user counts.

The game is designed to be cooperative, with another player being a driver of a vehicle that can help directly guide a mounted player across traffic and toward
enemy vehicles.

### Options of play
Multiplayer is first priority, especially running on an API that came with the tools primarily designed for this, and may be the first play option.
It is also desired that the game becomes a standalone that is released onto Steam, but may sit on the S&Box platform during it's development phases. By then, a campaign would be introduced in the forseeable case that
multiplayer services are inoperating may it be temporary maintenances, or end of life. It also features non-campaign single player, where missions are randomized or customized through options.

The game's difficulty can be scaled, and depending on this scale, it can affect the way the environment is created around the player. Steeper difficulties make the roads
more rough and unforgiving, changing the flow of the game's procedural generation, making turns and other environmental obstacles more likely.

Additionally weather can impact the physics of the game, such as rain or snow lowering friction on the road surface, or wind gradually pushing the player off
of surfaces.


