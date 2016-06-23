//This script removes an object after some time. Used to remove the effects after they are done animating.
#pragma strict

//How many seconds to wait before removing hte object
var removeAfter:float = 1;

function Update() 
{
	//Count down
	removeAfter -= Time.deltaTime;
	
	//If the timer reaches 0, remove the object
	if ( removeAfter <= 0 )
	{
		Destroy( gameObject);
	}
}