using System;
using ExplainCoreLib.core_models;

namespace ExplainCoreLib.functions
{
	public static class Acidbase
	{
		public static AcidBaseResult CalcAcidBaseFromTco2(BloodCapacitance comp)
		{
            // declare a dictionary for the result
            AcidBaseResult result = new()
            {
                valid = false
            };

            Console.WriteLine("acidbase calc");

			return result;
		}
	}

	public struct AcidBaseResult
	{
        public bool valid { get; set; }
        public double ph { get; set; }
        public double pco2 { get; set; }
        public double hco3 { get; set; }
        public double be { get; set; }
        public double sid_app { get; set; }

    }
}

