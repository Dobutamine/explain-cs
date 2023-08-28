using System;
using ExplainCoreLib.core_models;
using ExplainCoreLib.functions;

namespace ExplainCoreLib.functions
{
	public static class Oxygenation
	{
        // set the brent root finding properties
        private static double brent_accuracy = 1e-8;
        private static int max_iterations = 100;
        private static double gas_constant = 62.36367;
        private static double steps = 0;

        // oxygenation constants
        private static double left_o2 = 0.01;
        private static double right_o2 = 1000.0;
        private static double alpha_o2p = 0.0095;
        private static double mmoltoml = 22.2674;

        // oxygenation
        private static double dpg = 5;
        private static double hemoglobin = 8.0;
        private static double temp = 37;
        private static double pres = 0.0;

        private static double to2 = 0.0;
        private static double ph = 0.0;
        private static double be = 0.0;
        private static double po2 = 0.0;
        private static double so2 = 0.0;

        public static OxyResult CalcOxygenationFromTo2(BloodCapacitance comp)
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
            pres = comp.pres;

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

    public struct OxyResult
    {
        public bool valid { get; set; }
        public double po2 { get; set; }
        public double so2 { get; set; }

    }
}

