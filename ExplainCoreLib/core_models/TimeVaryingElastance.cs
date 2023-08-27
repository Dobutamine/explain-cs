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
            double _el_k
            )
            : base(_name, _description, _model_type, _is_enabled, _u_vol, _vol, _el_min, _el_k)
        {
            el_min = _el_min;
            el_max = _el_max;
        }
    }
}

