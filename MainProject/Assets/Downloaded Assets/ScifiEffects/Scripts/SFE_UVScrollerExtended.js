//This script scrolls the UV on an object (by offsetting its material).


var velocityY:float = 0.0;
var velocityX:float = 0.5;
var matNumber:int=0;

function Start()
{	
	if ( renderer )
		enabled = false;
}

function Update() 
{
	renderer.materials[matNumber].mainTextureOffset.y += velocityY * Time.deltaTime;
	renderer.materials[matNumber].mainTextureOffset.x += velocityX * Time.deltaTime;
}

function OnBecameVisible()
{
	this.enabled = true;
}

function OnBecameInvisible()
{
	this.enabled = false;
}
