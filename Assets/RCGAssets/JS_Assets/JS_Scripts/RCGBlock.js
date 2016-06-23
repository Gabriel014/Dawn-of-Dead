//This script defines a block which can interact with the player in various ways. A block may be a rock or a wall that
//bounces the player back, or it can be an enemy that kills the player, or it can be a coin that can be collected.
#pragma strict

//The tag of the object that can touch this block
var touchTargetTag:String = "Player";

//A list of functions that run when this block is touched by the target
var touchFunctions:TouchFunction[];

public class TouchFunction
{
	//The name of the function that will run
	var functionName:String = "CancelMove";
	
	//The tag of the target that the function will run on
	var targetTag:String = "Player";
	
	//A parameter that is passed along with the function
	var functionParameter:float = 0;
}

//Remove this object after a ceratin amount of touches
var removeAfterTouches:int = 0;
internal var isRemovable:boolean = false;

//THESE ATTRIBUTES WILL BE AVAILABLE IN A FUTURE UPDATE
//Can the target attach to this object
//var attachOnTouch:boolean = false;
//internal var isAttached:boolean = false;
//
//var blockStates:BlockState[];
//var currentBlockState:int = 0;
//internal var isRunningBlockStates:boolean = true;
//
//public class BlockState
//{
//	var stateName:String = "Warning";
//	var stateAnimation:AnimationClip;
//	var stateDelay:float = 2;
//}

//The animation that plays when this object is touched
var hitAnimation:AnimationClip;

//The sound that plays when this object is touched
var soundHit:AudioClip;
var soundSourceTag:String = "GameController";


//The effect that is created at the location of this object when it is destroyed
var deathEffect:Transform;

function Start()
{
	//If removeAfterTouches is higher than 0, make this object removable after one or more touches
	if ( removeAfterTouches > 0 )    isRemovable = true;
	
	//Start animating the first state of this block
//	if ( blockStates.Length > 0 )    
//	{
//		isRunningBlockStates = true;
//		
//		//Animate the next state
//		ActivateBlockState();
//	}
}

//This function runs when this obstacle touches another object with a trigger collider
function OnTriggerEnter(other:Collider) 
{	
	//Check if the object that was touched has the correct tag
	if ( other.tag == touchTargetTag )
	{
		//Go through the list of functions and runs them on the correct targets
		for ( var touchFunction in touchFunctions )
		{
			//Check that we have a target tag and function name before running
			if ( touchFunction.targetTag != String.Empty && touchFunction.functionName != String.Empty )
			{
				//Run the function
				GameObject.FindGameObjectWithTag(touchFunction.targetTag).SendMessage(touchFunction.functionName, touchFunction.functionParameter);
			}
		}
		
		//If there is an animation, play it
		if ( GetComponent.<Animation>() && hitAnimation )    
		{
			//Stop the animation
			GetComponent.<Animation>().Stop();
			
			//Play the animation
			GetComponent.<Animation>().Play(hitAnimation.name);
		}
		
		//If this object can attach targets to it, do so
//		if ( attachOnTouch == true )
//		{
//			//If the target is not already attached, attach it
//			if ( isAttached == false )
//			{
//				//The target is attached
//				isAttached = true;
//				
//				//Set the target as the child of this object
//				other.transform.parent = transform;
//				
//				//Stop the target from moving
//				other.GetComponent(RCGPlayer).isMoving = false;
//				
//				//Set the position of the object to 0
//				other.GetComponent(RCGPlayer).transform.localPosition *= 0;
//			}
//		}
		
		//If this object is removable, count down the touches and then remove it
		if ( isRemovable == true )
		{
			//Reduce the number of times this object was touched by the target
			removeAfterTouches--;
			
			if ( removeAfterTouches <= 0 )    
			{
				if ( deathEffect )    Instantiate( deathEffect, transform.position, Quaternion.identity);
				
				Destroy(gameObject);
			}
		}
		
		//If there is a sound source and a sound assigned, play it
		if ( soundSourceTag != "" && soundHit )    GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent.<AudioSource>().PlayOneShot(soundHit);
	}
}
//
////This function runs when this obstacle touches another object with a trigger collider
//function OnTriggerExit(other:Collider) 
//{
//	//Check if the object that was touched has the correct tag
//	if ( other.tag == touchTargetTag )
//	{
//		//If this object can attach targets to it, detach it
//		if ( attachOnTouch == true && isAttached == true )
//		{
//			//The target is detached
//			isAttached = false;
//			
//			//Detach the target from the hierarchy of this object
//			other.transform.parent = null;
//			
//			//Stop the target from moving
//			//other.GetComponent(RCGPlayer).isMoving = false;
//			
//			//Set the vertical target position to 0
//			other.GetComponent(RCGPlayer).targetPosition.y = 0;
//		}
//	}
//}

//function ActivateBlockState()
//{
//	//Wait for a few seconds before playing the state animation
//	yield WaitForSeconds(blockStates[currentBlockState].stateDelay);
//	
//	//If there is an animation, play it
//	if ( animation && blockStates[currentBlockState].stateAnimation )    
//	{
//		//Stop the animation
//		animation.Stop();
//		
//		//Play the animation
//		animation.Play(blockStates[currentBlockState].stateAnimation.name);
//	}
//	
//	if ( isRunningBlockStates == true )    ActivateNextBlockState();
//}
//
//function ActivateNextBlockState()
//{
//	//Go to the next animation state
//	if ( currentBlockState < blockStates.Length- 1 )    currentBlockState++;
//	else    currentBlockState = 0;
//	
//	//Animate the next state
//	ActivateBlockState();
//}
