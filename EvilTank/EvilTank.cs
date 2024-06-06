using Godot;
using System;
using Tankathon.API;

namespace Tankathon.EvilTank;
public class EvilTank : ITank
{
	bool movingRight = false;
	bool movingLeft = false;

	bool posSetup = false;

    int randoShoots = 0;

	Vector2 origPos = new Vector2();
	State _s;

    public void Setup()
	{
        //set starting state
        _s = State.MoveRight;
    }

    //Logic to do every frame
    public void Do(IActions actions, IScoreboard scoreboard)
	{

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
			if (Math.Abs(actions.stats.rotation) > 90 && movingRight)
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
                    if (actions.stats.rotation < 179 && actions.stats.rotation > -179 && !movingRight && !movingLeft)
                    {
                        actions.Aim(Rotation.CW);
                        //GD.Print($"rotation: {actions.stats.rotation}");
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
                        }
                    }
                }
            }

        }
		else if (_s == State.MoveMiddle)
		{
            //start by rotating back towards the left
            if (Math.Abs(actions.stats.rotation) > 90 && movingLeft)
            {
                //GD.Print("Rotate! ");
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
                    if(actions.FireCooldown() != 0 && randoShoots < 3)
                    {
                        
                        actions.Aim(Rotation.CCW);
                    }
                    else
                    {
                        actions.Fire();
                        randoShoots++;
                        //alright we're done, lets move more left. Lets rotate first

                        if (Math.Abs(actions.stats.rotation) > 90)
                        {
                            GD.Print("Rotate CW");
                            actions.Aim(Rotation.CW);
                        }
                        //else if (Math.Abs(actions.stats.rotation) < 90)
                        //{
                        //    GD.Print("Rotate CCW");
                        //    actions.Aim(Rotation.CCW);
                        //}
                        else
                        {
                            _s = State.MoveLeft;
                            //reset some values
                            origPos.X = actions.stats.xPos;
                            origPos.Y = actions.stats.yPos;
                            movingLeft = true;
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

                if (Math.Abs(actions.stats.rotation) > 90)
                {
                    actions.Aim(Rotation.CCW);
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
