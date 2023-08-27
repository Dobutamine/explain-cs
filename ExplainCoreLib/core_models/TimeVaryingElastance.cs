using System;
using ExplainCoreLib.Interfaces;
namespace ExplainCoreLib.base_models
{
	public class TimeVaryingElastance : Capacitance
	{

        public double el_min { get; set; } = 0.0;
        public double el_max { get; set; } = 0.0;
        public double el_min_factor { get; set; } = 1.0;
        public double el_max_factor { get; set; } = 1.0;
        public double act_factor { get; set; } = 1.0;
        public double pres_ed { get; set; } = 0.0;
        public double pres_ms { get; set; } = 0.0;


        public TimeVaryingElastance(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            double _u_vol,
            double _vol,
            double _el_min,
            double _el_max,
            double _el_k,
            bool _fixed_composition
            )
            : base(_name, _description, _model_type, _is_enabled, _u_vol, _vol, _el_min, _el_k, _fixed_composition)
        {
            el_min = _el_min;
            el_max = _el_max;
        }

        public override void CalcModel()
        {
            if (vol > u_vol * u_vol_factor)
            {
                double vol_diff = vol - (u_vol * u_vol_factor);

                pres_ed = el_k * el_k_factor * Math.Pow(vol_diff, 2) +
                          el_min * el_min_factor * vol_diff + pres_ext;
                pres_ms = el_max * el_max_factor * vol_diff;
                pres = act_factor * (pres_ms - pres_ed) + pres_ed +
                        pres_cc + pres_atm + pres_mus;
            }
            else
            {
                pres = pres_ext + pres_cc + pres_atm + pres_mus;
            }

            // Reset the pressures that are recalculated every model iteration
            pres_ext = 0.0;
            pres_cc = 0.0;
            pres_mus = 0.0;

        }
    }
}

