//This script defines a lane, which may have several moving objects in it. A lane can also have an item.
#pragma strict

internal var thisTransform:Transform;

//The start and end points of the lane
var laneStart:Vector3 = Vector3(0,0,-7);
var laneEnd:Vector3 = Vector3(0,0,7);

//Possible objects that will be used in this lane
var movingObjects:Transform[];

//A list of the objects that will be moving in this lane
internal var movingObjectsList:Transform[];

//The minimum/maximum number of moving objects in a lane
var movingObjectsNumber:Vector2 = Vector2(1,3);

//The minimum gap between objects. This is used to prevent objects from potentially overlapping eachother.
var minimumObjectGap:float = 1;

//The movement speed of all the objects in a lane
var moveSpeed:Vector2 = Vector2(0.5,1);

//The movement direction of the objects in the lane
internal var moveDirection:int = 1;

//Should the starting position of the objects be random? If true, the object has a 50% of starting from the laneEnd and going to the laneStart
var randomDirection:boolean = true;

function Start()
{
	thisTransform = transform;
	
	//Randomize the initial movement direction of the objects
	if ( randomDirection == true && Random.value > 0.5 )   moveDirection= -1;
	
	//Randomize the movement speed of the objects in the lane
	moveSpeed.x = Random.Range(moveSpeed.x, moveSpeed.y);
	
//Add random objects to the moving object list
	//Calculate the length of the path of the moving objects in the lane
	var laneLength:float = Vector3.Distance(laneStart, laneEnd);
	
	//Set the number of moving objects in the lane randomly
	movingObjectsNumber.x = Mathf.Floor(Random.Range(movingObjectsNumber.x, movingObjectsNumber.y));
	
	//Create a list that will contain all the moving objects in the lane
	movingObjectsList = new Transform[movingObjectsNumber.x];
	
	//Create the moving objects and place them in the list
	for ( var index:int = 0 ; index < movingObjectsNumber.x ; index++ )
	{
		//Create a random moving object
		var newMovingObject:Transform = Instantiate(movingObjects[Mathf.Floor(Random.Range(0,movingObjects.Length))]);
		
		// Place the object at the start point, and look at the end of the lane
		newMovingObject.position = thisTransform.position + laneStart;
		newMovingObject.LookAt(thisTransform.position + laneEnd);
		
		//Spread the objects evenly along the lane
		newMovingObject.Translate( Vector3.forward * index * laneLength/movingObjectsNumber.x, Space.Self);
		
		//Move each object randomly along the lane to make it more varied
		newMovingObject.Translate( Vector3.forward * Random.Range( minimumObjectGap, laneLength/movingObjectsNumber.x - minimumObjectGap), Space.Self);
		
		//Add the moving object to the list
		movingObjectsList[index] = newMovingObject;
	}
	
	//Go through all the moving objects and move them!
	for ( var movingObject in movingObjectsList )
	{
		//Make the object look at the direction it is moving in
		if ( moveDirection== 1 )    movingObject.LookAt(thisTransform.position + laneEnd);
		else    movingObject.LookAt(thisTransform.position + laneStart);
		
		//If there is an animation, play it
		if ( movingObject.GetComponent.<Animation>() )    
		{
			//Set the animation speed base on the movement speed
			movingObject.GetComponent.<Animation>()[movingObject.GetComponent.<Animation>().clip.name].speed = moveSpeed.x;
		}
	}
}

function Update() 
{
	//Go through all the moving objects in a lane...
	for ( var movingObject in movingObjectsList )
	{
		//Check if the object moves normally, or in reverse
		if ( moveDirection == 1 )    
		{
			if ( movingObject )
			{
				//Move the object towards the lane end point
				movingObject.position = Vector3.MoveTowards( movingObject.position, thisTransform.position + laneEnd, moveSpeed.x * Time.deltaTime );
				
				//If the object reaches the lane end point, reset it to the lane start point
				if ( movingObject.position == thisTransform.position + laneEnd )    
				{
					movingObject.position = thisTransform.position + laneStart;
					movingObject.LookAt(thisTransform.position + laneEnd);
				}
			}
		}
		else if ( movingObject )  
		{
			
			//Move the object towards the lane start point
			movingObject.position = Vector3.MoveTowards( movingObject.position, thisTransform.position + laneStart, moveSpeed.x * Time.deltaTime );
			
			//If the object reaches the lane start point, reset it to the lane end point
			if ( movingObject.position == thisTransform.position + laneStart )    
			{
				movingObject.position = thisTransform.position + laneEnd;
				movingObject.LookAt(thisTransform.position + laneStart);
			}
		}
	}
}

//This function draws the start and end points of the moving objects in a lane
function OnDrawGizmos()
{
	Gizmos.color = Color.green;
	Gizmos.DrawSphere(transform.position + laneStart, 0.2);
	
	Gizmos.color = Color.red;
	Gizmos.DrawSphere(transform.position + laneEnd, 0.2);
}