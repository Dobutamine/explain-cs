using System;
namespace ExplainCoreLib.functions
{
	public static class ActivationFunction
	{
		public static double Activation(double value, double max, double setpoint, double min)
		{
            double act = 0.0;

            if (value >= max)
            {
                act = max - setpoint;
            } else
            {
                if (value <= min)
                {
                    act = min - setpoint;
                } else
                {
                    act = value - setpoint;
                }
            }

            return act;
        }
	}
}

