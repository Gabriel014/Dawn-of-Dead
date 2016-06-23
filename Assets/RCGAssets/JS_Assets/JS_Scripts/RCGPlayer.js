//This script defines the player, its speed and movement limits, as well as the different types of deaths it may suffer.
#pragma strict

internal var thisTransform:Transform;
internal var gameController:GameObject;

//The player's movement speed, and variables that check if the player is moving now, where it came from, and where it's going to
var speed:float = 0.1;
internal var isMoving:boolean = false;
internal var previousPosition:Vector3;
internal var targetPosition:Vector3;

//The movement limits object. This object contains some colliders that bounce the player back into the game area
var moveLimits:Transform;

//Various animations for the player
var animationMove:AnimationClip;
var animationCoin:AnimationClip;

//Death effects that show when the player is killed
var deathEffect:Transform[];

//The player can't move or perform actions for a few seconds
var moveDelay:float = 0;

//Various sounds and their source
var soundMove:AudioClip[];
var soundCoin:AudioClip[];
var soundSourceTag:String = "GameController";

function Start() 
{
	thisTransform = transform;
	
	targetPosition = thisTransform.position;
	
	gameController = GameObject.FindGameObjectWithTag("GameController");
}

function Update() 
{
	//Keep the move limits object moving forward/backward along with the player
	if ( moveLimits )    moveLimits.position.x = thisTransform.position.x;
	
	//Count down the move delay
	if ( moveDelay > 0 )    moveDelay -= Time.deltaTime;
	
	//If the player is not already moving, it can move
	if ( isMoving == false && moveDelay <= 0 && Time.timeScale > 0 )
	{
		//You can move left/right only if you are not already moving forward/backwards
		if ( Input.GetAxisRaw("Vertical") == 0 )
		{
			//Moving right
			if ( Input.GetAxisRaw("Horizontal") > 0 )
			{
				//Move one step to the right
				Move("right");
			}
			
			//Moving left
			if ( Input.GetAxisRaw("Horizontal") < 0 )
			{
				//Move one step to the left
				Move("left");
			}
		}
		
		//You can move forward/backwards only if you are not already moving left/right
		if ( Input.GetAxisRaw("Horizontal") == 0 )
		{
			//Moving forward
			if ( Input.GetAxisRaw("Vertical") > 0 )
			{
				//Move one step forward
				Move("forward");
			}
			
			//Moving backwards
			if ( Input.GetAxisRaw("Vertical") < 0 )
			{
				//Move one step backwards
				Move("backward");
			}
		}
	}
	else //Otherwise, move the player to its target position
	{
		//Keep moving towards the target position until we reach it
		if ( thisTransform.position != targetPosition )
		{
			//Move this object towards the target position
			thisTransform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
		}
		else
		{
			//The object is not moving anymore
			isMoving = false;
		}
	}
}

//This function moves an object from its current position to a target position, over time
function Move( moveDirection:String )
{
	if ( isMoving == false && moveDelay <= 0 )
	{
		//The object is moving
		isMoving = true;
		
		switch ( moveDirection )
	    {
		    case "forward":
		        //Turn to the front
				thisTransform.eulerAngles.y = 0;
				
				//Set the new target position to move to
				targetPosition = thisTransform.position + Vector3(1,0,0);
				
				//Make sure the player lands on the grid 
				targetPosition.x = Mathf.Round(targetPosition.x);
				targetPosition.z = Mathf.Round(targetPosition.z);
				
				//Register the last position the player was at, so we can return to it if the path is blocked
				previousPosition = thisTransform.position;
					
		        break;
		    
		    case "backward":
		        //Turn to the back
				thisTransform.eulerAngles.y = 180;
				
				//Register the last position the player was at, so we can return to it if the path is blocked
				previousPosition = thisTransform.position;
				
				//Make sure the player lands on the grid 
				targetPosition.x = Mathf.Round(targetPosition.x);
				targetPosition.z = Mathf.Round(targetPosition.z);
				
				//Set the new target position to move to
				targetPosition = thisTransform.position + Vector3(-1,0,0);
		        
		        break;
		        
		    case "right":
		        //Turn to the right
				thisTransform.eulerAngles.y = 90;
				
				//Register the last position the player was at, so we can return to it if the path is blocked
				previousPosition = thisTransform.position;
				
				//Make sure the player lands on the grid 
				targetPosition.x = Mathf.Round(targetPosition.x);
				targetPosition.z = Mathf.Round(targetPosition.z);
				
				//Set the new target position to move to
				targetPosition = thisTransform.position + Vector3(0,0,-1);
		        
		        break;
		        
		    case "left":
	        	//Turn to the left
				thisTransform.eulerAngles.y = -90;
				
				//Register the last position the player was at, so we can return to it if the path is blocked
				previousPosition = thisTransform.position;
				
				//Make sure the player lands on the grid 
				targetPosition.x = Mathf.Round(targetPosition.x);
				targetPosition.z = Mathf.Round(targetPosition.z);
				
				//Set the new target position to move to
				targetPosition = thisTransform.position + Vector3(0,0,1);
					
		        break;
		        
		    default:
		        //Turn to the front
				thisTransform.eulerAngles.y = 0;
				
				//Set the new target position to move to
				targetPosition = thisTransform.position + Vector3(1,0,0);
				
				targetPosition.Normalize();
				
				//Register the last position the player was at, so we can return to it if the path is blocked
				previousPosition = thisTransform.position;

		        break;
	    }
	    
		//If there is an animation, play it
		if ( GetComponent.<Animation>() && animationMove )    
		{
			//Stop the animation
			GetComponent.<Animation>().Stop();
			
			//Play the animation
			GetComponent.<Animation>().Play(animationMove.name);
			
			//Set the animation speed base on the movement speed
			GetComponent.<Animation>()[animationMove.name].speed = speed;
			
			//If there is a sound source and more than one sound assigned, play one of them from the source
			if ( soundSourceTag != String.Empty && soundMove.Length > 0 )    GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent.<AudioSource>().PlayOneShot(soundMove[Mathf.Floor(Random.value * soundMove.Length)]);
		}
	}
}

//This function cancels the player's current move, bouncing it back to it's previous position 
function CancelMove( moveDelayTime:float )
{
	//If there is an animation, play it
	if ( GetComponent.<Animation>() && animationMove )    
	{
		//Set the animation speed base on the movement speed
		GetComponent.<Animation>()[animationMove.name].speed = -speed;
	}
	
	//Set the previous positoin as the target position to move to
	targetPosition = previousPosition;
	
	//If there is a move delay, prevent movement for a while
	moveDelay = moveDelayTime;
}

//This function animates a coin added to the player
function AddCoin( addValue:int )
{
	gameController.SendMessage("ChangeScore", addValue);
	
	//If there is an animation, play it
	if ( GetComponent.<Animation>() && animationCoin && animationMove )    
	{
		//Play the animation
		//animation.Play(animationCoin.name);
		
		//animation.Play(animationMove.name); 
		GetComponent.<Animation>()[animationCoin.name].layer = 1; 
		GetComponent.<Animation>().Play(animationCoin.name); 
		GetComponent.<Animation>()[animationCoin.name].weight = 0.5f;
	}
	
	//If there is a sound source and more than one sound assigned, play one of them from the source
	if ( soundSourceTag != String.Empty && soundCoin.Length > 0 )    GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent.<AudioSource>().PlayOneShot(soundCoin[Mathf.Floor(Random.value * soundCoin.Length)]);
}

//This function destroys the player, and triggers the game over event
function Die( deathType:int )
{
	//Call the game over function from the game controller
	gameController.SendMessage("GameOver", 0.5);
	
	//If we have death effects...
	if ( deathEffect.Length > 0 )
	{
		//Create the correct death effect
		Instantiate(deathEffect[deathType], thisTransform.position, thisTransform.rotation);
	}
	
	//Remove this object
	Destroy(gameObject);
}