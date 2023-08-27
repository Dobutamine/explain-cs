using System;
using ExplainCoreLib.base_models;

namespace ExplainCoreLib.core_models
{
    public class BloodValve : Resistor
    {
        public BloodValve(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            bool _no_flow,
            bool _no_back_flow,
            string _comp_from,
            string _comp_to,
            double _r_for,
            double _r_back,
            double _r_k
            ) : base(_name, _description, _model_type, _is_enabled, _no_flow, _no_back_flow, _comp_from, _comp_to, _r_for, _r_back, _r_k)
        {
        }
    }
}

