using System;

namespace Tankathon.API;

public interface ITank
{
	void Setup();
	void Do(Actions actions);
}
