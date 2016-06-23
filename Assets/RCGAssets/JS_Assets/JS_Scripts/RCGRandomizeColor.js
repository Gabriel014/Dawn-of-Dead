//This script randomizes the color of an object
#pragma strict

//A list of possible colors for the object
var colorList:Color[];

function Start() 
{
	//Choose a random color from the list
	var randomColor:int = Mathf.Floor(Random.Range(0, colorList.Length));
	
	//Set the color to all parts of the object
	for ( var part in GetComponentsInChildren(Renderer)) 
	{
		part.GetComponent.<Renderer>().material.color = colorList[randomColor];
	}
}