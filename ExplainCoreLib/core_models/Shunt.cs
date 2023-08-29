using System;
using ExplainCoreLib.base_models;
using ExplainCoreLib.Interfaces;
using ExplainCoreLib.functions;

namespace ExplainCoreLib.core_models
{
    public class Shunt : Resistor
    {
        public double length { get; set; } = 1.0;
        public double diameter { get; set; } = 2.0;
        public double non_lin_factor { get; set; } = 0.0;
        public double viscosity { get; set; } = 6.0;

        public Shunt(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            bool _no_flow,
            bool _no_back_flow,
            string _comp_from,
            string _comp_to,
            double _length,
            double _diameter,
            double _non_lin_factor,
            double _r_for,
            double _r_back,
            double _r_k
            ) : base(_name, _description, _model_type, _is_enabled, _no_flow, _no_back_flow, _comp_from, _comp_to, _r_for, _r_back, _r_k)
        {
            length = _length;
            diameter = _diameter;
            non_lin_factor = _non_lin_factor;
        }

        public override bool InitModel(Dictionary<string, BaseModel> models, double stepsize = 0.0005)
        {
            // calculate the current resistance of the shunt
            r_for = TubeResistance.CalcResistanceTube(diameter, length, viscosity);
            r_back = r_for;
            r_k = non_lin_factor;

            base.InitModel(models, stepsize);

            try
            {
                _model_comp_from = (Capacitance)models[comp_from];
                _model_comp_to = (Capacitance)models[comp_to];
            }
            catch
            {
                is_initialized = false;
                Console.WriteLine("error instantiating resistor {0}: {1} to {2}", name, comp_from, comp_to);
            }

            is_initialized = true;
            return true;
        }

        public void SetDiameter(double new_diameter)
        {
            diameter = new_diameter;
            r_for = TubeResistance.CalcResistanceTube(diameter, length, viscosity);
            r_back = r_for;
        }

        public void SetLength(double new_length)
        {
            length = new_length;
            r_for = TubeResistance.CalcResistanceTube(diameter, length, viscosity);
            r_back = r_for;
        }

        public void SetViscosity(double new_viscosity)
        {
            viscosity = new_viscosity;
            r_for = TubeResistance.CalcResistanceTube(diameter, length, viscosity);
            r_back = r_for;
        }

    }
}

