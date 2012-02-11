MODELS
-Model Ref-|--File name
tree,		Models\tree.fbx
treePhysics,Models\tree-physics.fbx
house,		Models\house.fbx
goodVibe,	Models\GoodVibe.fbx
ShieldGoodVibe, Models\GoodVibeShield.fbx
terrain,	Models\terrain.fbx
terrain64,	Models\terrain64.fbx
wave,		Models\wave.fbx
box,		Models\box.fbx
spaceShip,  Models\spaceShip.fbx
pickup,		Models\crate.fbx
badVibe, Models\BadVibe.fbx
badVibePhysics, Models\BadVibe-physics.fbx
BV_Spawner, Models\BV_Spawner.fbx
--dude        Models\dude.fbx
x2,			Models\x2.fbx
BV_Explosion, Models\BVExplosion.fbx

TEXTURES
-Texture Ref-|--Files Name(s)--|
tronFloor,		Textures\texOrganicGround.png
green,			Textures\texGreen.jpg
yellow,			Textures\texYellow.jpg
blue,			Textures\texBlue.jpg
red,			Textures\texRed.jpg
cymbal,			Textures\texCymbal.jpg
pulseGreyToRed,	Textures\pulseGreyToRed\pulseGreyToRed0001.jpg;Textures\pulseGreyToRed\pulseGreyToRed0004.jpg;Textures\pulseGreyToRed\pulseGreyToRed0003.jpg;Textures\pulseGreyToRed\pulseGreyToRed0002.jpg;Textures\pulseGreyToRed\pulseGreyToRed0001.jpg
allColours,		Textures\texGreen.jpg;Textures\texYellow.jpg;Textures\texBlue.jpg;Textures\texRed.jpg;Textures\texCymbal.jpg
animationTest,	Textures\animationTest\animationTest0001.jpg;Textures\animationTest\animationTest0002.jpg;Textures\animationTest\animationTest0003.jpg;Textures\animationTest\animationTest0004.jpg;Textures\animationTest\animationTest0005.jpg;Textures\animationTest\animationTest0006.jpg;Textures\animationTest\animationTest0007.jpg;Textures\animationTest\animationTest0008.jpg;Textures\animationTest\animationTest0009.jpg;Textures\animationTest\animationTest0010.jpg;Textures\animationTest\animationTest0011.jpg;Textures\animationTest\animationTest0012.jpg;Textures\animationTest\animationTest0013.jpg;Textures\animationTest\animationTest0014.jpg;Textures\animationTest\animationTest0015.jpg;Textures\animationTest\animationTest0016.jpg;Textures\animationTest\animationTest0017.jpg;Textures\animationTest\animationTest0018.jpg;Textures\animationTest\animationTest0019.jpg;Textures\animationTest\animationTest0020.jpg;Textures\animationTest\animationTest0021.jpg;Textures\animationTest\animationTest0022.jpg;Textures\animationTest\animationTest0023.jpg;Textures\animationTest\animationTest0024.jpg
goodVibeTexture, Textures\GoodVibeTexture.jpg

---You can use dashes at the start of lines for comments

---Texture delay(ms) is the time spent on each frame of the animation.
---Set "Texture delay(ms)" to 0 or "Start texture anim." to 0 if you dont want the textures to animate.
---If "Start texture anim." is set to 1 the animation will start automatically.

-Model No.-|--Graphics Model----|-- Scale--|----Physics Model---|-P. Scale--|------Texture(s) (if needed)------|-Texture delay(ms)-|--Start texture anim.--|-Model Animation (if needed)-|
1,			tree,					1,		treePhysics,			0.1
2,			house,					1,		house,					1
3,			badVibe,			    0.1,	badVibe,			    0.1,			allColours,							0,						0,						1
4,			goodVibe,				0.35,	spaceShip,				0.35,			goodVibeTexture,					40,						0,
5,			terrain64,			    250,	terrain,			    250,			tronFloor,							0,						0,						0
6,			house,					1,		house,					1,
7,			wave,					1,		wave,					1,				allColours,							0,						
8,			box,					2,		pickup,					2,				animationTest,						80,						1
9,          ShieldGoodVibe,         0.35,    spaceShip,              0,             goodVibeTexture
10,         BV_Spawner,             0.6,    box,                    0.6
11,			x2,						0.5,		x2,						0.5
12,         BV_Explosion,           0.1,    BV_Explosion,           0.1,                          ,                     0,                      0,                      1
