using System;
using ExplainCoreLib.functions;
using ExplainCoreLib.base_models;
using ExplainCoreLib.core_models;
using ExplainCoreLib.Interfaces;

namespace ExplainCoreLib.functions
{
	public static class BloodComposition
	{
        // set the brent root finding properties
        private static readonly double brent_accuracy = 1e-8;
        private static readonly int max_iterations = 100;
        private static readonly double gas_constant = 62.36367;

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
        private static double sid = 0.0;
        private static double albumin = 0.0;
        private static double phosphates = 0.0;
        private static double uma = 0.0;

        // oxygenation constants
        private static readonly double left_o2 = 0.01;
        private static readonly double right_o2 = 1000.0;

        // oxygenation state variables
        private static double dpg = 5;
        private static double hemoglobin = 8.0;
        private static double temp = 37;
        private static double to2 = 0.0;
        private static double ph = 0.0;
        private static double be = 0.0;
        private static double po2 = 0.0;
        private static double so2 = 0.0;

        public static void SetBloodComposition(BloodTimeVaryingElastance bc)
		{
			AcidBaseResult acidBaseResult = CalcAcidBaseFromTco2(bc);
            if (acidBaseResult.valid)
            {
                bc.aboxy["ph"] = acidBaseResult.ph;
                bc.aboxy["pco2"] = acidBaseResult.pco2;
                bc.aboxy["hco3"] = acidBaseResult.hco3;
                bc.aboxy["be"] = acidBaseResult.be;
                bc.aboxy["sid_app"] = acidBaseResult.sid_app;
            }

            OxyResult oxyResult = CalcOxygenationFromTo2(bc);
            if (oxyResult.valid)
            {
                bc.aboxy["po2"] = oxyResult.po2;
                bc.aboxy["so2"] = oxyResult.so2;
            }
        }

        public static void SetBloodComposition(BloodCapacitance bc)
        {
            AcidBaseResult acidBaseResult = CalcAcidBaseFromTco2(bc);
            if (acidBaseResult.valid)
            {
                bc.aboxy["ph"] = acidBaseResult.ph;
                bc.aboxy["pco2"] = acidBaseResult.pco2;
                bc.aboxy["hco3"] = acidBaseResult.hco3;
                bc.aboxy["be"] = acidBaseResult.be;
                bc.aboxy["sid_app"] = acidBaseResult.sid_app;
            }

            OxyResult oxyResult = CalcOxygenationFromTo2(bc);
            if (oxyResult.valid)
            {
                bc.aboxy["po2"] = oxyResult.po2;
                bc.aboxy["so2"] = oxyResult.so2;
            }
        }

        private static AcidBaseResult CalcAcidBaseFromTco2(BloodTimeVaryingElastance comp)
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

        private static AcidBaseResult CalcAcidBaseFromTco2(BloodCapacitance comp)
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

        private static OxyResult CalcOxygenationFromTo2(BloodTimeVaryingElastance comp)
        {
            // declare a dictionary for the result
            OxyResult result = new()
            {
                valid = false
            };

            // get the for the oxygenation independent parameters from the component
            to2 = comp.aboxy["to2"];
            ph = comp.aboxy["ph"];
            be = comp.aboxy["be"];
            dpg = comp.aboxy["dpg"];
            hemoglobin = comp.aboxy["hemoglobin"];
            temp = comp.aboxy["temp"];

            // calculate the po2 from the to2 using a brent root finding function and oxygen dissociation curve
            po2 = BrentRootFindingProcedure.BrentRootFinding(OxygenContent, left_o2, right_o2, max_iterations, brent_accuracy);

            // if a po2 is found then return the result
            if (po2 > 0)
            {
                result.valid = true;
                result.po2 = po2;
                result.so2 = so2 * 100.0;
            }
            return result;
        }

        private static OxyResult CalcOxygenationFromTo2(BloodCapacitance comp)
        {
            // declare a dictionary for the result
            OxyResult result = new()
            {
                valid = false
            };

            // get the for the oxygenation independent parameters from the component
            to2 = comp.aboxy["to2"];
            ph = comp.aboxy["ph"];
            be = comp.aboxy["be"];
            dpg = comp.aboxy["dpg"];
            hemoglobin = comp.aboxy["hemoglobin"];
            temp = comp.aboxy["temp"];

            // calculate the po2 from the to2 using a brent root finding function and oxygen dissociation curve
            po2 = BrentRootFindingProcedure.BrentRootFinding(OxygenContent, left_o2, right_o2, max_iterations, brent_accuracy);

            // if a po2 is found then return the result
            if (po2 > 0)
            {
                result.valid = true;
                result.po2 = po2;
                result.so2 = so2 * 100.0;
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

        private static double OxygenContent(double po2Estimate)
        {
            // calculate the saturation from the current po2 from the current po2 estimate
            so2 = OxygenDissociationCurve(po2Estimate);

            // calculate the to2 from the current po2 estimate
            // INPUTS: po2 in mmHg, so2 in fraction, hemoglobin in mmol/l
            // convert the hemoglobin unit from mmol/l to g/dL  (/ 0.6206)
            // convert to output from ml O2/dL blood to ml O2/l blood (* 10.0)
            double to2_new_estimate = (0.0031 * po2Estimate + 1.36 * (hemoglobin / 0.6206) * so2) * 10.0;

            // conversion factor for converting ml O2/l to mmol/l
            double mmol_to_ml = (gas_constant * (273.15 + temp)) / 760.0;

            // convert the ml O2/l to mmol/l
            to2_new_estimate = to2_new_estimate / mmol_to_ml;

            // calculate the difference between the real to2 and the to2 based on the new po2 estimate and return it to the brent root finding function
            double dto2 = to2 - to2_new_estimate;

            return dto2;
        }

        private static double OxygenDissociationCurve(double po2Estimate)
        {
            // calculate the saturation from the po2 depending on the ph,be, temperature and dpg level.
            double a = 1.04 * (7.4 - ph) + 0.005 * be + 0.07 * (dpg - 5.0);
            double b = 0.055 * (temp + 273.15 - 310.15);
            double x0 = 1.875 + a + b;
            double h0 = 3.5 + a;
            double x = Math.Log((po2Estimate * 0.1333), Math.E);  // po2 in kPa
            double y = x - x0 + h0 * Math.Tanh(0.5343 * (x - x0)) + 1.875;

            // return the o2 saturation in fraction so 0.98
            return 1.0 / (Math.Exp(-y) + 1.0);
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

    public struct OxyResult
    {
        public bool valid { get; set; }
        public double po2 { get; set; }
        public double so2 { get; set; }

    }
}


