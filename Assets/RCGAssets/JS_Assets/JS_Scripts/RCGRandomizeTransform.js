//This script randomizes the scale, rotation, and color of an object
#pragma strict

//The range of the rotation for each axis
var rotationRangeX:Vector2 = Vector2(0, 360);
var rotationRangeY:Vector2 = Vector2(0, 360);
var rotationRangeZ:Vector2 = Vector2(0, 360);

//The scale of the rotation for each axis
var scaleRangeX:Vector2 = Vector2(1,1.3);
var scaleRangeY:Vector2 = Vector2(1,1.3);
var scaleRangeZ:Vector2 = Vector2(1,1.3);

//Should scaling be uniform for all axes?
var uniformScale:boolean = true;

//A list of possible colors for the object
var colorList:Color[];

function Start() 
{
	//Set a random rotation for the object
	transform.localEulerAngles.x = Random.Range(rotationRangeX.x, rotationRangeX.y);
	transform.localEulerAngles.y = Random.Range(rotationRangeY.x, rotationRangeY.y);
	transform.localEulerAngles.z = Random.Range(rotationRangeZ.x, rotationRangeZ.y);
	
	//If uniform, set the scale of every axis based on the X axis
	if ( uniformScale == true )    scaleRangeY = scaleRangeZ = scaleRangeX;
	
	//Set a random scale for the object
	transform.localScale.x = Random.Range(scaleRangeX.x, scaleRangeX.y);
	transform.localScale.y = Random.Range(scaleRangeY.x, scaleRangeY.y);
	transform.localScale.z = Random.Range(scaleRangeZ.x, scaleRangeZ.y);
	
	//Choose a random color from the list
	var randomColor:int = Mathf.Floor(Random.Range(0, colorList.Length));
	
	//Set the color to all parts of the object
	for ( var part in GetComponentsInChildren(Renderer)) 
	{
		part.GetComponent.<Renderer>().material.color = colorList[randomColor];
	}
}




	
