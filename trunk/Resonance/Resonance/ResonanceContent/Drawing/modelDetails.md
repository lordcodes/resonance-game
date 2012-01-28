MODELS
-Model Ref-|--File name
tree,		Models\tree.fbx
treePhysics,Models\treePhysics.fbx
house,		Models\house.fbx
virus,		Models\bad_vibe.fbx
goodVibe,		Models\GoodVibe.fbx
terrain,	Models\terrain.fbx
terrainHiPoly,	Models\terrainHiPoly.fbx
terrain64,	Models\terrain64.fbx
wave,		Models\wave.fbx
box,		Models\box.fbx
spaceShip,  Models\spaceShip.fbx
pickup,		Models\crate.fbx
ShieldGoodVibe, Models\GoodVibeShield.fbx
BV_animated, Models\bad_vibe_animation.fbx
--dude        Models\dude.fbx

TEXTURES
-Texture Ref-|--Files Name(s)--|
tronFloor,		Textures\texTronFloor.png
green,			Textures\texGreen.jpg
yellow,			Textures\texYellow.jpg
blue,			Textures\texBlue.jpg
red,			Textures\texRed.jpg
cymbal,			Textures\texCymbal.jpg
allColours,		Textures\texGreen.jpg;Textures\texYellow.jpg;Textures\texBlue.jpg;Textures\texRed.jpg;Textures\texCymbal.jpg
animationTest,	Textures\animationTest\animationTest0001.jpg;Textures\animationTest\animationTest0002.jpg;Textures\animationTest\animationTest0003.jpg;Textures\animationTest\animationTest0004.jpg;Textures\animationTest\animationTest0005.jpg;Textures\animationTest\animationTest0006.jpg;Textures\animationTest\animationTest0007.jpg;Textures\animationTest\animationTest0008.jpg;Textures\animationTest\animationTest0009.jpg;Textures\animationTest\animationTest0010.jpg;Textures\animationTest\animationTest0011.jpg;Textures\animationTest\animationTest0012.jpg;Textures\animationTest\animationTest0013.jpg;Textures\animationTest\animationTest0014.jpg;Textures\animationTest\animationTest0015.jpg;Textures\animationTest\animationTest0016.jpg;Textures\animationTest\animationTest0017.jpg;Textures\animationTest\animationTest0018.jpg;Textures\animationTest\animationTest0019.jpg;Textures\animationTest\animationTest0020.jpg;Textures\animationTest\animationTest0021.jpg;Textures\animationTest\animationTest0022.jpg;Textures\animationTest\animationTest0023.jpg;Textures\animationTest\animationTest0024.jpg

---You can use dashes at the start of lines for comments

---Texture delay(ms) is the time spent on each frame of the animation.
---Set Texture delay(ms) to 0 if you dont want the textures to animate.

-Model No.-|--Graphics Model----|-- Scale--|----Physics Model---|-P. Scale--|------Texture(s) (if needed)------|-Texture delay(ms)-|-Model Animation (if needed)-|
1,			tree,					1,		treePhysics,			1
2,			house,					1,		house,					1
3,			BV_animated,			0.2,	box,					3.5,			allColours,							0,					1
4,			goodVibe,				0.5,	spaceShip,				0.2
5,			terrain64,			    250,	terrain,			    250,			tronFloor,							0,					0
6,			house,					1,		house,					1,
7,			wave,					1,		wave,					1,				green
8,			wave,					1,		wave,					1,				yellow
9,			wave,					1,		wave,					1,				blue
10,			wave,					1,		wave,					1,				red
11,			wave,					1,		wave,					1,				cymbal
12,			BV_animated,			0.2,	box,					3.5,			green,								0,					1
13,			BV_animated,			0.2,	box,					3.5,			yellow,								0,					1
14,			BV_animated,			0.2,	box,					3.5,			blue,								0,					1
15,			BV_animated,			0.2,	box,					3.5,			red,								0,					1
16,			BV_animated,			0.2,	box,					3.5,			cymbal,								0,					1
----17,			pickup,					2,		pickup,					2
17,			box,					2,		pickup,					2,				animationTest,						80
18,         ShieldGoodVibe,         0.5,    spaceShip,              0.2
