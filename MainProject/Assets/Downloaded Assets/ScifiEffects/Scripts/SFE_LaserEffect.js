//this script is about how a laser changes during its lifetime (fading, enlarging, etc)
//this is added individually for each line effect, so each line can be parametrised differently




var laser:LineRenderer;
var laserSize:float=0.1;
var fadeSpeed:float=1;
var enlargeSpeed:float=0;
var beginTintAlpha:float=0.5;
var myColor:Color;  

private var time:float=0.0;
private var alpha:float=0.0;

var normalizeUV:boolean=false;
var normalizeUvLength:float=1;

var maxRange:float=300;

private var lasBegin:Vector3;
private var lasEnd:Vector3;





function Start()
{	

var direction = transform.TransformDirection(Vector3.up);

var hit : RaycastHit;


 
if (Physics.Raycast (transform.position, direction, hit)) { // if it hits something, we set the laser beam to be that long

laser.SetPosition(0, transform.position);
laser.SetPosition(1, hit.point);

lasBegin=transform.position;
lasEnd=hit.point;


}
else   //if the raycast hits nothing, we set the line's endpoint to maxRange
{
laser.SetPosition(0, transform.position);

var endOfLaser:Vector3=transform.TransformDirection(transform.position.x, transform.position.y+maxRange, transform.position.z);
laser.SetPosition(1, endOfLaser);

lasBegin=transform.position;
lasEnd=endOfLaser;

}



if (normalizeUV==true)  //if normalizing the UV. Important when the ray has some pattern, that it doesn't get stretched.
{

var distance = Vector3.Distance(lasBegin, lasEnd);
renderer.materials[0].mainTextureScale.x=distance/normalizeUvLength;

}


}

function Update() //enlarging, alpha stuff
{
time+=Time.deltaTime;
alpha=beginTintAlpha-fadeSpeed*time;

laserSize=(enlargeSpeed*Time.deltaTime)+laserSize;
laser.SetWidth(laserSize,laserSize);


laser.renderer.material.SetColor ("_TintColor", Color(myColor.r, myColor.g, myColor.b ,alpha));  

}


  