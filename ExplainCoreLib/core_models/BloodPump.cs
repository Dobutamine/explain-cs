using System;
using ExplainCoreLib.base_models;
using ExplainCoreLib.Interfaces;

namespace ExplainCoreLib.core_models
{
    public class BloodPump : Capacitance, IBlood
    {
        public int pump_mode { get; set; } = 0;
        public double pump_rpm { get; set; } = 0;
        public string inlet { get; set; } = "";
        public string outlet { get; set; } = "";

        public double pump_pressure { get; set; }
        public double pres_inlet { get; set; }
        public double pres_outlet { get; set; }

        public Dictionary<string, double> solutes { get; set; } = new();
        public Dictionary<string, double> aboxy { get; set; } = new();

        private BloodResistor? _inlet_res;
        private BloodResistor? _outlet_res;

        public BloodPump(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            double _u_vol,
            double _vol,
            double _el_base,
            double _el_k,
            bool _fixed_composition,
            int _pump_mode,
            double _pump_rpm,
            string _inlet,
            string _outlet)
            : base(_name, _description, _model_type, _is_enabled, _u_vol, _vol, _el_base, _el_k, _fixed_composition)
        {
            pump_mode = _pump_mode;
            pump_rpm = _pump_rpm;
            inlet = _inlet;
            outlet = _outlet;
        }

        public override bool InitModel(Dictionary<string, BaseModel> models, double stepsize = 0.0005)
        {
            base.InitModel(models, stepsize);

            // find the inlet and outlet connectors
            _inlet_res = (BloodResistor)_models[inlet];
            _outlet_res = (BloodResistor)_models[outlet];

            is_initialized = true;
            return is_initialized;
        }

        public override void CalcModel()
        {
            // calculate the parent class
            base.CalcModel();

            // do the pump specific actions
            pump_pressure = -pump_rpm / 25.0;

            // determine the inlet and outlet pressures and transfer them to the connected bloodresistors
            if (pump_mode == 0)
            {
                _inlet_res.p1_ext = 0.0;
                _inlet_res.p2_ext = pump_pressure;
            } else
            {
                _outlet_res.p1_ext = pump_pressure;
                _outlet_res.p2_ext = 0.0;
            }

        }
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

