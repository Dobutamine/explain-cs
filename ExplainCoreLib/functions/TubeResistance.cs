using System;
namespace ExplainCoreLib.functions
{
	public static class TubeResistance
	{
        public static double CalcResistanceTube(double diameter, double length, double viscosity = 6.0)
        {
            // resistance is calculated using Poiseuille's Law : R = (8 * n * L) / (PI * r^4)
            // # we have to watch the units carefully where we have to make sure that the units in the formula are
            // resistance is in mmHg * s / l
            // L = length in meters from millimeters
            // r = radius in meters from millimeters
            // n = viscosity in mmHg * s from centiPoise

            // convert viscosity from centiPoise to mmHg * s
            double n_mmhgs = viscosity * 0.001 * 0.00750062;

            // convert the length to meters
            double length_meters = length / 1000.0;

            // calculate radius in meters
            double radius_meters = diameter / 2 / 1000.0;

            // calculate the resistance
            double res = (8.0 * n_mmhgs * length_meters) / (Math.PI * Math.Pow(radius_meters, 4));

            // convert resistance of mmHg * s / mm^3 to mmHg *s / l
            res = res / 1000.0;

            return res;
        }



    }
}

