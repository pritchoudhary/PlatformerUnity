# PlatformerUnity
Unity Version - 2021.1.1f1
Unity Maze puzzeler prototype

A basic Maze Game, where the goal of the player is to reach the end of the maze without dying. The maze consists of static and non-static hazards - that include wandering enemey objects, bullet projectiles and moving rocks. If the player comes in contact with any of these objects, the player dies.

There are static interactive objects, if the player picks these up, it recieves a speed boost.

Hitting the 'Escape' button brings up the pause menu. Player moves through the Horizontal and Vertical Axis. (W,A,S,D)

Tool SetUp
I have created a tool that enables the designer to place various prefabs into the scene. Steps to follow - 

1) Create a Layer named "PrefabPlacer" and change the LayerMask of any of they prefabs to "PrefabPlacer" so that if multiple prefabs are being spawned, they don't stack up on top of each other.

2) The designer can create a Node by Clicking on - PrefabPlacer -> CreateNode
	NOTE: Before doing this - Create a empty gameobject, make it a prefab and add the Node script on this prefab. Place this prefab under a folder called - PrefabPlacer.

3) Now all you need to do is follow the settings in the tool. Add the prefabs to the list and hit the Spawn Prefab button. 

You will now be able to see the prefabs in that particular spot. 

Create as many nodes you want and spawn prefabs of your choice.