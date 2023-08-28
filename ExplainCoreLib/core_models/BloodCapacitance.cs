using System;
using ExplainCoreLib.base_models;
using ExplainCoreLib.Interfaces;

namespace ExplainCoreLib.core_models
{
	public class BloodCapacitance : Capacitance, IBlood
	{
        public Dictionary<string, double> solutes { get; set; } = new();
        public Dictionary<string, double> aboxy { get; set; } = new();

        public BloodCapacitance(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            double _u_vol,
            double _vol,
            double _el_base,
            double _el_k,
            bool _fixed_composition)
            : base(_name, _description, _model_type, _is_enabled, _u_vol, _vol, _el_base, _el_k, _fixed_composition) {}

        public override void VolumeIn(double dvol, Capacitance model_from)
        {
            base.VolumeIn(dvol, model_from);

            if (vol <= 0)
            {
                return;
            }


            // process the to2 and tco2
            IBlood mf = (IBlood)model_from;

            double d_to2 = (mf.aboxy["to2"] - aboxy["to2"]) * dvol;
            aboxy["to2"] += d_to2 / vol;

            double d_tco2 = (mf.aboxy["tco2"] - aboxy["tco2"]) * dvol;
            aboxy["tco2"] += d_tco2 / vol;

            // process the solutes
            foreach (var solute in solutes)
            {
                double d_solute = (mf.solutes[solute.Key] - solutes[solute.Key]) * dvol;
                solutes[solute.Key] += d_solute / vol;

            }
        }
    }
}

