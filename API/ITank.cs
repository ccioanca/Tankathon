using System;
using TestTankathon.API;

namespace Tankathon.API;

public interface ITank
{
	//void Setup();
	void Do(IActions actions, IScoreboard scoreboard);
}
