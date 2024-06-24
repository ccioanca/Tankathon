using Godot;
using System;
using Tankathon.API;

namespace Tankathon.EvilTank;
public class EvilTank : ITank
{
	bool movingRight = false;
	bool movingLeft = false;

    bool midFromRight = false;
    bool midFromLeft = false;

	bool posSetup = false;

    int randoShoots = 0;

	Vector2 origPos = new Vector2();
	State _s;

    Random rand;

    public void Setup(TankStats stats)
	{
        //set starting state
        _s = State.MoveRight;
        rand = new Random();
    }

    //Logic to do every frame
    public void Do(IActions actions, IScoreboard scoreboard)
	{
        actions.Scan();
        if (_s == State.MoveRight)
		{
            if (!posSetup)
			{
                origPos.X = actions.stats.xPos;
                origPos.Y = actions.stats.yPos;
				posSetup = true;
                movingRight = true;
            }
			//rotate to point right
			if (!Mathf.IsEqualApprox(actions.stats.rotation, 90f) && Math.Abs(actions.stats.rotation) > 90 && movingRight)
			{
                actions.Fire();
                actions.Aim(Rotation.CCW);
			}
			else
			{
                //move to the right
                if (movingRight && actions.stats.xPos < origPos.X + 250)
                {
                    actions.MoveForward();
                }
                else
                {
                    movingRight = false;

                    //we finished moving right, lets shoot down again 
                    if (!Mathf.IsEqualApprox(actions.stats.rotation, 180) && !movingRight && !movingLeft)
                    {
                        actions.Aim(Rotation.CW);
                    }
                    else
                    {
                        if (actions.FireCooldown() == 0)
                        {
                            actions.Fire();
                            _s = State.MoveMiddle;
                            //reset some values
                            origPos.X = actions.stats.xPos;
                            origPos.Y = actions.stats.yPos;
                            movingLeft = true;
                            midFromRight = true;
                        }
                    }
                }
            }

        }
		else if (_s == State.MoveMiddle && midFromRight)
		{
            //start by rotating back towards the left
            if (!Mathf.IsEqualApprox(Mathf.Round(actions.stats.rotation), -90) && movingLeft)
            {
                actions.Aim(Rotation.CW);
            }
            else
            {
                //move towards the middle
                if (movingLeft && actions.stats.xPos > origPos.X - 250)
                {
                    actions.MoveForward();
                }
                else
                {
                    movingLeft = false;
                    //lets shoot again! but this time, in a random direction! (lets do it 3 times)
                    if(actions.FireCooldown() != 0 && randoShoots < rand.Next(2, 4))
                    {
                        actions.Aim(Rotation.CCW);
                    }
                    else
                    {
                        actions.Fire();
                        randoShoots++;
                        //alright we're done, lets move more left. Lets rotate first
                        if (!Mathf.IsEqualApprox(Mathf.Round(actions.stats.rotation), -90))
                        {
                            actions.Aim(Rotation.CW);
                        }
                        else
                        {
                            _s = State.MoveLeft;
                            //reset some values
                            origPos.X = actions.stats.xPos;
                            origPos.Y = actions.stats.yPos;
                            movingLeft = true;
                            midFromRight = false;
                            randoShoots = 0;
                        }
                    }

                }
            }
        }
        else if (_s == State.MoveLeft)
        {
            //time to go left
            if (movingLeft && actions.stats.xPos > origPos.X - 250)
            {
                actions.MoveForward();
            }
            else
            {
                movingLeft = false;

                if (!Mathf.IsEqualApprox(Mathf.Round(actions.stats.rotation), 180) && !(actions.Scan().eType == EntityType.Tank))
                {
                    //if ((actions.Scan().eType == EntityType.Tank))
                    //    GD.Print("Seeing Tank");
                    //else GD.Print(actions.Scan().eType);
                    actions.Aim(Rotation.CW);
                }
                else
                {
                    //shoot down, and then reset
                    if(actions.FireCooldown() == 0)
                    {
                        actions.Fire();
                        movingRight = true;
                        midFromLeft = true;
                        _s = State.MoveMiddle;
                        origPos.X = actions.stats.xPos;
                        origPos.Y = actions.stats.yPos;

                    }
                }
            }
        }
        else if (_s == State.MoveMiddle && midFromLeft)
        {
            if (!Mathf.IsEqualApprox(Mathf.Round(actions.stats.rotation), 90) && movingRight)
            {
                actions.Aim(Rotation.CCW);
            }
            else if(movingRight && actions.stats.xPos < origPos.X + 250)
            {
                actions.MoveForward();
            }
            else
            {
                movingRight = false;
                //point back down
                if (!Mathf.IsEqualApprox(Mathf.Round(actions.stats.rotation), 180))
                {
                    actions.Aim(Rotation.CW);
                }
                else if (actions.FireCooldown() == 0)
                {
                    actions.Fire();
                    midFromRight = false;
                    midFromLeft = false;
                    posSetup = false;
                    _s = State.MoveRight;
                }
            }
        }


    }

	enum State
	{
		None,
		MoveRight, 
		MoveMiddle,
		MoveLeft,
	}

}
