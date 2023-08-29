using System;
using System.Security.Cryptography;
using ExplainCoreLib.core_models;
using ExplainCoreLib.functions;

namespace ExplainCoreLib.functions
{
	public static class Acidbase
	{
        // set the brent root finding properties
        private static readonly double brent_accuracy = 1e-8;
        private static readonly int max_iterations = 100;

        // acidbase constants
        private static readonly double kw = Math.Pow(10.0, -13.6) * 1000.0;
        private static readonly double kc = Math.Pow(10.0, -6.1) * 1000.0;
        private static readonly double kd = Math.Pow(10.0, -10.22) * 1000.0;
        private static readonly double alpha_co2p = 0.03067;
        private static readonly double left_hp = Math.Pow(10.0, -7.8) * 1000.0;
        private static readonly double right_hp = Math.Pow(10.0, -6.5) * 1000.0;

        // acidbase state variables
        private static double tco2 = 0.0;
        private static double pco2 = 0.0;
        private static double hco3 = 0.0;
        private static double be = 0.0;
        private static double sid = 0.0;
        private static double albumin = 0.0;
        private static double phosphates = 0.0;
        private static double uma = 0.0;
        private static double hemoglobin = 0.0;

		public static AcidBaseResult CalcAcidBaseFromTco2(BloodCapacitance comp)
		{
            // declare a dictionary for the result
            AcidBaseResult result = new()
            {
                valid = false
            };

            // calculate the apparent strong ion difference(SID) in mEq / l
            // comp.sid = comp.sodium + comp.potassium + 2 * comp.calcium + 2 * comp.magnesium - comp.chloride - comp.lactate - comp.urate

            // get the total co2 concentration in mmol/l
            tco2 = comp.aboxy["tco2"];

            // calculate the apparent SID
            sid = comp.solutes["na"] + comp.solutes["k"] + 2 * comp.solutes["ca"] + 2 * comp.solutes["mg"] - comp.solutes["cl"] - comp.solutes["lact"];

            // get the albumin concentration in g/l
            albumin = comp.aboxy["albumin"];

            // get the inorganic phosphates concentration in mEq/l
            phosphates = comp.aboxy["phosphates"];

            // get the unmeasured anions in mEq/l
            uma = comp.aboxy["uma"];

            // get the hemoglobin concentration in mmol/l
            hemoglobin = comp.aboxy["hemoglobin"];

            // now try to find the hydrogen concentration at the point where the net charge of the plasma is zero within limits of the brent accuracy
            double hp = BrentRootFindingProcedure.BrentRootFinding(NetChargePlasma, left_hp, right_hp, max_iterations, brent_accuracy);

            // if a hp is found then return the result
            if (hp > 0)
            {
                result.valid = true;
                result.ph = (-Math.Log10(hp / 1000));
                result.pco2 = pco2;
                result.hco3 = hco3;
                result.be = be;
                result.sid_app = sid;
            }
            return result;
        }

        private static double NetChargePlasma(double hp_estimate)
        {
            // Calculate the pH based on the current hp estimate
            double ph = -Math.Log10(hp_estimate / 1000.0);

            // Calculate the plasma co2 concentration based on the total co2 in the plasma, hydrogen concentration, and the constants Kc and Kd
            double cco2p = tco2 / (1.0 + kc / hp_estimate + (kc * kd) / Math.Pow(hp_estimate, 2.0));

            // Calculate the plasma hco3(-) concentration (bicarbonate)
            double hco3p = (kc * cco2p) / hp_estimate;

            // Calculate the plasma co3(2-) concentration (carbonate)
            double co3p = (kd * hco3p) / hp_estimate;

            // Calculate the plasma OH(-) concentration (water dissociation)
            double ohp = kw / hp_estimate;

            // Calculate the pco2 of the plasma
            double pco2p = cco2p / alpha_co2p;

            // Calculate the weak acids (albumin and phosphates)
            double a_base = albumin * (0.123 * ph - 0.631) +
                            phosphates * (0.309 * ph - 0.469);

            // Calculate the net charge of the plasma
            double netcharge = hp_estimate + sid - hco3p - 2.0 * co3p - ohp - a_base - uma;

            // Calculate the base excess according to the van Slyke equation
            be = (hco3p - 25.1 + (2.3 * hemoglobin + 7.7) * (ph - 7.4)) * (1.0 - 0.023 * hemoglobin);

            // Store the calculated values in class members
            pco2 = pco2p;
            hco3 = hco3p;
            // cco3 = co3p; // Uncomment this if cco3 is a class member
            // cco2 = cco2p; // Uncomment this if cco2 is a class member

            // Return the net charge
            return netcharge;
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

