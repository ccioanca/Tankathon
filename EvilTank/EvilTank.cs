using Godot;
using System;
using Tankathon.API;

namespace Tankathon.EvilTank;
public class EvilTank : ITank
{
	bool movingRight = false;
	bool movingLeft = false;

	bool posSetup = false;

	Vector2 origPos = new Vector2();
	State _s;

    public void Setup()
	{ 

    }

    //Logic to do every frame
    public void Do(IActions actions, IScoreboard scoreboard)
	{
		actions.Fire();
		_s = State.MoveRight;

		if (_s == State.MoveRight)
		{
			if(!posSetup)
			{
                origPos.X = actions.stats.xPos;
                origPos.Y = actions.stats.yPos;
				posSetup = true;
                movingRight = true;
            }
			//rotate to point right
			if (actions.stats.rotation > 90 && movingRight)
			{
				actions.Aim(Rotation.CCW);
			}
			else
			{
				movingLeft = false;

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
                        GD.Print($"rotation: {actions.stats.rotation}");
                    }
                    else
                    {
                        movingRight = false;
                        movingLeft = true;
                        float ttl = actions.Fire();
                        if (ttl > 9)
                            _s = State.MoveMiddle;
                    }
                }
            }

			

            
        }
		else if (_s == State.MoveMiddle)
		{

		}

        //GD.Print(actions.stats.rotation);

    }

	enum State
	{
		None,
		MoveRight, 
		MoveMiddle,
		MoveLeft,
	}

}
