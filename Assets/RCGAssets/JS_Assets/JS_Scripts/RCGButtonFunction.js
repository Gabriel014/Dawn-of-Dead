//This script runs a function in a target object when clicked on. In order to detect clicks you need to attach a collider to this object. It can also have
//an input button assigned (ex "Fire1") to trigger it with keyboard or gamepad controls.
#pragma strict

private var thisTransform:Transform;

//The target object in which the function needs to be executed
var functionTarget:Transform;

//The name of the function that will be executed
var functionName:String;

//The numerical parameter passed along with this function
var functionParameter:String;

//The sound of the click and the source of the sound
var soundClick:AudioClip;
var soundSource:Transform;

function Start() 
{
	//Cache this transform
	thisTransform = transform;
}

function OnMouseDrag()
{
	//Execute the button function when there's a mouse click on it
	ExecuteFunction();
}

function ExecuteFunction()
{
	//Play a sound from the source
	if ( soundSource )    if ( soundSource.GetComponent.<AudioSource>() )    soundSource.GetComponent.<AudioSource>().PlayOneShot(soundClick);
	
	//Run the function at the target object
	if ( functionName )
	{  
		if ( functionTarget )    
		{
			//Send the message to the target object
			functionTarget.SendMessage(functionName, functionParameter);
		}
	}
}