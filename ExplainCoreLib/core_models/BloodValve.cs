using System;
using ExplainCoreLib.base_models;
using ExplainCoreLib.Interfaces;

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

        public override bool InitModel(Dictionary<string, BaseModel> models, double stepsize = 0.0005)
        {
            base.InitModel(models, stepsize);

            try
            {
                _model_comp_from = (Capacitance)models[comp_from];
                _model_comp_to = (Capacitance)models[comp_to];
            }
            catch
            {
                is_initialized = false;
                Console.WriteLine("error instantiating valve {0}: {1} to {2}", name, comp_from, comp_to);
                return false;
            }

            is_initialized = true;
            return true;
        }
    }
}

