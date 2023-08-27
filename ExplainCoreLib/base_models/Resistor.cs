using System;
using ExplainCoreLib.Interfaces;

namespace ExplainCoreLib.base_models
{
	public class Resistor : BaseModel
	{
        public bool no_flow { get; set; } = true;
        public bool no_back_flow { get; set; } = false;
		public string comp_from { get; set; }
        public string comp_to { get; set; }
		public double r_for { get; set; } = 1000.0;
        public double r_back { get; set; } = 1000.0;
		public double r_k { get; set; } = 1.0;

        public double r_for_factor { get; set; } = 1.0;
        public double r_back_factor { get; set; } = 1.0;
        public double r_k_factor { get; set; } = 1.0;
        public double p1_ext { get; set; } = 1.0;
        public double p2_ext { get; set; } = 1.0;
        public double flow { get; set; } = 0.0;

        public Capacitance? _model_comp_from;
        public Capacitance? _model_comp_to;

        public Resistor(
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
            ) : base(_name, _description, _model_type, _is_enabled)
        {
            no_flow = _no_flow;
            no_back_flow = _no_back_flow;
            comp_from = _comp_from;
            comp_to = _comp_to;
            r_for = _r_for;
            r_back = _r_back;
            r_k = _r_k;
        }

        public override bool InitModel(Dictionary<string, BaseModel> models, double stepsize = 0.0005)
        {
            base.InitModel(models, stepsize);

            try
            {
                _model_comp_from = (Capacitance) models[comp_from];
                _model_comp_to = (Capacitance) models[comp_to];
            } catch
            {
                Console.WriteLine("error instantiating resistor {0}: {1} to {2}", name, comp_from, comp_to);
            }

            return true;
        }

        public override void CalcModel()
        {
            // Get the pressures
            double _p1 = _model_comp_from.pres + p1_ext;
            double _p2 = _model_comp_to.pres + p2_ext;

            // Reset the external pressures
            p1_ext = 0;
            p2_ext = 0;

            // Calculate the flow
            if (no_flow || (_p1 <= _p2 && no_back_flow))
            {
                flow = 0.0;
            }
            else if (_p1 > _p2)  // Forward flow
            {
                flow = (_p1 - _p2) / (r_for * r_for_factor) -
                       r_k * r_k_factor * Math.Pow(flow, 2);
            }
            else  // Back flow
            {
                flow = (_p1 - _p2) / (r_back * r_back_factor) +
                       r_k * r_k_factor * Math.Pow(flow, 2);
            }

            // Update the volume
            UpdateVolumes();

        }

        private void UpdateVolumes()
        {
            // Now update the volumes of the model components connected by this resistor
            if (flow > 0)
            {
                // Flow is from comp_from to comp_to
                double vol_not_removed = _model_comp_from.volume_out(flow * _t);
                _model_comp_to.volume_in((flow * _t) - vol_not_removed, _model_comp_from);
                return;
            }

            if (flow < 0)
            {
                // Flow is from comp_to to comp_from
                double vol_not_removed = _model_comp_to.volume_out(-flow * _t);
                _model_comp_from.volume_in((-flow * _t) - vol_not_removed, _model_comp_to);
                return;
            }

        }
    }
}

