using System;
using System.Reflection;
using ExplainCoreLib.base_models;
using ExplainCoreLib.core_models;
using ExplainCoreLib.functions;
namespace ExplainCoreLib.core_models
{
	public class Metabolism : BaseModel
	{
        public double vo2 { get; set; } = 7.0;
        public double vo2_factor { get; set; } = 1.0;
        public double resp_q { get; set; } = 0.6;
        public double body_temp { get; set; } = 37.0;
        public Dictionary<string, double> metabolic_active_models { get; set; } = new();

        public Metabolism(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            double _vo2,
            double _resp_q,
            double _body_temp,
            Dictionary<string, double> _metabolic_active_models
            ) : base (_name, _description, _model_type, _is_enabled)
        {
            vo2 = _vo2;
            resp_q = _resp_q;
            body_temp = _body_temp;
            metabolic_active_models = _metabolic_active_models;
		}

        public override void CalcModel()
        {
            // translate the VO2 in ml/kg/min to VO2 in mmol for this stepsize (assumption is 37 degrees and atmospheric pressure)
            double vo2_step = ((0.039 * vo2 * vo2_factor * 3.3) / 60.0) * _t;

            // do the metabolism in the metabolic active models
            foreach(var mam in metabolic_active_models)
            {
                BloodCapacitance bc = (BloodCapacitance)_models[mam.Key];
                double fvo2 = mam.Value;

                // get the vol, tco2 and to2 from the blood compartment
                double vol = bc.vol;
                double to2 = bc.aboxy["to2"];
                double tco2 = bc.aboxy["tco2"];

                // calculate the change in oxygen concentration in this step
                double dto2 = vo2_step * fvo2;

                // calculate the new oxygen concentration in blood
                double new_to2 = (to2 * vol - dto2) / vol;
                // guard against negative values
                if (new_to2 < 0)
                {
                    new_to2 = 0;
                }

                // calculate the change in co2 concentration in this step
                double dtco2 = vo2_step * fvo2 * resp_q;

                // calculate the new co2 concentration in blood
                double new_tco2 = (tco2 * vol + dtco2) / vol;
                // guard against negative values
                if (new_tco2 < 0)
                {
                    new_tco2 = 0;
                }

                // store the new to2 and tco2
                bc.aboxy["to2"] = new_to2;
                bc.aboxy["tco2"] = new_tco2;
            }
        }
    }
}

