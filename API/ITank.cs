using System;
using Tankathon.API;

namespace Tankathon.API;

public interface ITank
{
	//void Setup();
	void Do(IActions actions, IScoreboard scoreboard);
}
