# Boids & FSM

This project is also a score based game made with Unity. The idea was to work around boids and finite state machines.

Players increase their score by killing ogres in a small room. To do that, they deal damages by shooting boids mushrooms towards them.

## Mushrooms 

Mushrooms move around the room with a flocking behaviour. Thus, they follow three rules :
* **Separation:** steer to avoid crowding local mushrooms.
* **Alignment:** steer towards the average heading of local mushrooms.
* **cohesion:** steer to move towards the average position  of local mushrooms.

They are also able to find a free path when they face an obstacle and their behaviour is influenced by a state machine such as :
* They move around freely in the room. (state ROAM)
* If they are in the player range, they are attracted. (state FOLLOW)
* If they are in an ogre range, they try to avoid it. (state EVADE)
* If they are used as projectiles by the player, every states are blocked during the shot. (state SHOT)

## Ogres

Ogres spawn randomly and permanently from the doors of the room. Their behaviour is implemented such as :
* They are attracted by the largest group of mushrooms.
* If they collide a mushroom, they eat it and gain health points.
* If they are hit by a mushroom shot, they lose a health point and they are stunned for a short amount of time.
* If they die, they release every eaten mushrooms.

The game ends if the player gets hit by an ogre or if ogres eat every mushrooms in the room.

![Game view]()

## Free to use assets

16x16 Enchanted Forest Characters - Superdark : https://superdark.itch.io/enchanted-forest-characters

16x16 Dungeon Tileset - 0x72 : https://0x72.itch.io/16x16-dungeon-tileset

