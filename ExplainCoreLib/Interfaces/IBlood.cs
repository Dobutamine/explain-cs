using System;
using System.Collections.Generic;

namespace ExplainCoreLib.Interfaces
{
	public interface IBlood
	{
		Dictionary<string, double> solutes { get; set; }
        Dictionary<string, double> aboxy { get; set; }
    }
}

