using System;
using ExplainCoreLib.base_models;
using ExplainCoreLib.core_models;
using ExplainCoreLib.functions;
namespace ExplainCoreLib.core_models
{
	public class Ventilator : BaseModel
	{
        // independent properties
        public double p_atm{ get; set; } = 760.0;
        public double temp { get; set; } = 37.0;
        public double humidity { get; set; } = 1.0;
        public double fio2 { get; set; } = 0.21;
        public double tubing_elastance { get; set; } = 1160.0;
        public double tubing_diameter { get; set; } = 0.0102;
        public double tubing_length { get; set; } = 1.6;
        public double tubing_volume { get; set; } = 0.13;
        public double ettube_diameter { get; set; } = 0.0035;
        public double ettube_length { get; set; } = 0.11;
        public double ettube_volume { get; set; } = 0.0025;
        public double ettube_elastance { get; set; } = 20000.0;
        public string vent_mode { get; set; } = "PC";
        public double vent_rate { get; set; } = 40.0;
        public bool vent_sync { get; set; } = true;
        public double vent_trigger { get; set; } = 0.001;
        public double insp_time { get; set; } = 0.4;
        public double insp_flow { get; set; } = 10.0;
        public double exp_flow { get; set; } = 3.0;
        public double pip { get; set; } = 14.3;
        public double pip_max { get; set; } = 20.0;
        public double peep { get; set; } = 2.9;
        public double tidal_volume { get; set; } = 0.015;
        public double exp_tidal_volume { get; set; } = 0.0;
        public double insp_tidal_volume { get; set; } = 0.0;
        public double ivr { get; set; } = 2200;
        public double hfo_map { get; set; } = 10.0;
        public double hfo_freq { get; set; } = 10.0;
        public double hfo_amplitude { get; set; } = 10.0;
        public double hfo_pres { get; set; } = 0.0;

        // ventilator parts
        private List<BaseModel>? _vent_parts = new();
        private GasCapacitance? _ventin;
        private GasCapacitance? _tubingin;
        private GasCapacitance? _ettube;
        private GasCapacitance? _tubingout;
        private GasCapacitance? _ventout;

        private GasResistor? _insp_valve;
        private GasResistor? _tubingin_ettube;
        private GasResistor? _ettube_ds;
        private GasResistor? _ettube_tubingout;
        private GasResistor? _exp_valve;
        private GasResistor? _hfo_valve;

        public Ventilator(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            double _p_atm,
            double _temp,
            double _humidity,
            double _fio2,
            double _tubing_elastance,
            double _tubing_diameter,
            double _tubing_length,
            double _tubing_volume,
            double _ettube_diameter,
            double _ettube_length,
            string _vent_mode,
            double _vent_rate,
            bool _vent_sync,
            double _vent_trigger,
            double _insp_time,
            double _insp_flow,
            double _exp_flow,
            double _pip,
            double _pip_max,
            double _peep,
            double _tidal_volume,
            double _hfo_map,
            double _hfo_freq,
            double _hfo_amplitude) : base(_name, _description, _model_type, _is_enabled)
		{
            p_atm = _p_atm;
            temp = _temp;
            humidity = _humidity;
            fio2 = _fio2;
            tubing_elastance = _tubing_elastance;
            tubing_diameter = _tubing_diameter;
            tubing_length = _tubing_length;
            tubing_volume = _tubing_volume;
            ettube_diameter = _ettube_diameter;
            ettube_length = _ettube_length;
            vent_mode = _vent_mode;
            vent_rate = _vent_rate;
            vent_sync = _vent_sync;
            vent_trigger = _vent_trigger;
            insp_time = insp_time;
            insp_flow = _insp_flow;
            exp_flow = _exp_flow;
            pip = _pip;
            pip_max = _pip_max;
            peep = _peep;
            tidal_volume = _tidal_volume;
            hfo_map = _hfo_map;
            hfo_freq = _hfo_freq;
            hfo_amplitude = _hfo_amplitude;
        }

        public override bool InitModel(Dictionary<string, BaseModel> models, double stepsize = 0.0005)
        {
            base.InitModel(models, stepsize);

            is_initialized = true;
            return is_initialized;
        }

        public void SwitchVentilator(bool state) { }

        private void BuildVentilator()
        {
            // clear the ventilator part list
            _vent_parts = new();

            // build the mechanical ventilator using the gas capacitance model
            _ventin = new GasCapacitance("VENTIN", "internal ventilator reservoir", "GasCapacitance", true, 5.0, 5.4, 1000.0, 0.0, true)
            {
                pres_atm = p_atm
            };
            _ventin.InitModel(_models);
            _ventin.CalcModel();
 


        }
    }
}

