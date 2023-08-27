using System;
using ExplainCoreLib.base_models;
namespace ExplainCoreLib.core_models
{
	public class BloodCapacitance : Capacitance
	{
        public BloodCapacitance(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            double _u_vol,
            double _vol,
            double _el_base,
            double _el_k
            )
            : base(_name, _description, _model_type, _is_enabled, _u_vol, _vol, _el_base, _el_k)
        {

        }
    }
}

