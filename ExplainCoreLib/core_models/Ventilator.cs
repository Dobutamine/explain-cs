using System;
using System.Reflection;
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
        public double ivr { get; set; } = 2200;
        public double hfo_map { get; set; } = 10.0;
        public double hfo_freq { get; set; } = 10.0;
        public double hfo_amplitude { get; set; } = 10.0;
        public double hfo_pres { get; set; } = 0.0;

        // dependent properties
        public double exp_time { get; set; }
        public double etco2 { get; set; }
        public double compliance { get; set; }
        public double exp_tidal_volume { get; set; } = 0.0;
        public double insp_tidal_volume { get; set; } = 0.0;
        private bool _inspiration = true;
        private bool _expiration = false;
        private double _insp_counter = 0;
        private double _insp_volume_counter = 0;
        private double _exp_counter = 0;
        private double _exp_volume_counter = 0;

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

        public void SwitchVentilator(bool state)
        {
            // switch the ventilator
            is_enabled = state;

            // switch the flow between the et tube and the patient
            _ettube_ds.no_flow = !state;

            // switch all ventilator parts
            foreach (var m in _vent_parts)
            {
                m.is_enabled = state;
            }

            // switch the flow between the patient and the outside world
            ((GasResistor)_models["MOUTH_DS"]).no_flow = state;

        }

        public void SetVentilatorPc(double _pip= 14.0, double _peep= 4.0, double _rate= 40.0, double _t_in= 0.4, double _insp_flow= 10.0)
        {
            SwitchVentilator(true);
            vent_mode = "PC";
            pip_max = _pip * 0.735559;
            pip = _pip * 0.735559;
            peep = _peep * 0.735559;
            vent_rate = _rate;
            insp_time = _t_in;
            insp_flow = _insp_flow;
        }

        public void SetVentilatorPrvc(double _pip = 14.0, double _peep = 4.0, double _rate = 40.0, double _tv = 15, double _t_in = 0.4, double _insp_flow = 10.0)
        {
            SwitchVentilator(true);
            vent_mode = "PRVC";
            pip_max = _pip * 0.735559;
            pip = _pip * 0.735559;
            peep = _peep * 0.735559;
            vent_rate = _rate;
            insp_time = _t_in;
            tidal_volume = _tv / 1000.0;
            insp_flow = _insp_flow;
        }

        public override void CalcModel()
        {
            // select the ventilator mode
            if (vent_mode == "PC" || vent_mode == "PRVC")
            {
                ConventionalVentilation();
            }
        }

        private void ConventionalVentilation()
        {
            // calculate the expiration time
            exp_time = (60.0 / vent_rate) - insp_time;

            // do the time cycling
            if (_insp_counter > insp_time)
            {
                // inspiration has elasped
                _inspiration = false;
                _expiration = true;
                _insp_counter = 0;
                _exp_counter = 0;
                insp_tidal_volume = _insp_volume_counter;
                _insp_volume_counter = 0;
            }

            if (_exp_counter > exp_time)
            {
                // expiration has elasped
                _inspiration = true;
                _expiration = false;
                _insp_counter = 0;
                _exp_counter = 0;
                exp_tidal_volume = _exp_volume_counter;
                _exp_volume_counter = 0;
                etco2 = _ettube.pco2;
                compliance = exp_tidal_volume / (pip - peep);
                compliance = compliance * 1000 * 0.73555;
                // guard the tidal volume if the ventilator is in PRVC mode
                if (vent_mode == "PRVC")
                {
                    PressureRegulatedVolumeControl();
                }
            }

            // increase the timers
            if (_inspiration)
            {
                _insp_counter += _t;
            }

            if (_expiration)
            {
                _exp_counter += _t;
            }

            // cycle the pressures
            PressureControl();
        }

        private void PressureControl()
        {
            if (_inspiration)
            {
                // open the inspiration valve and calculate the inspiratory valve position depending on the desired flow
                _insp_valve.no_flow = false;
                _insp_valve.no_back_flow = true;
                _insp_valve.r_for = ((_ventin.pres - p_atm) / (insp_flow / 60.0)) + 500;

                // guard the inspiration pressures as this is pressure control
                if (_tubingin.pres > pip + p_atm)
                {
                    _insp_valve.no_flow = true;
                }

                // close the expiration valve
                _exp_valve.no_flow = true;

                // increase the inspiratory volume
                _insp_volume_counter += _tubingin_ettube.flow * _t;
            }

            if (_expiration)
            {
                // close the inspiratory valve
                _insp_valve.no_flow = true;

                // open the expiration valve and calculate the expiration valve position depending on the desired peep
                _exp_valve.no_flow = false;
                _exp_valve.r_for = 15.0;
                if (_tubingin.pres < peep + p_atm)
                {
                    _ventout.vol = (peep / _ventout.el_base) + _ventout.u_vol;
                }

                // increase the expiratory volume
                _exp_volume_counter += _ettube_tubingout.flow * _t;
            }
        }

        private void PressureRegulatedVolumeControl()
        {
            // if the tidal volume is not reached then increase the pressure
            if (exp_tidal_volume < tidal_volume - 0.001)
            {
                pip += 0.74;
                // make sure the pressure does not exceed the set max pressure
                if (pip > pip_max)
                {
                    pip = pip_max;
                }
            }

            // if the set tidal volume is exceeded then reduced the pressure
            if (exp_tidal_volume > tidal_volume + 0.001)
            {
                pip -= 0.74;
                // make sure the inspiratory pressure doesn't get lower then 2 + peep
                if (pip < peep + 1.5)
                {
                    pip = peep + 1.5;
                }
            }
        }
        
        public void BuildVentilator()
        {
            // clear the ventilator part list
            _vent_parts = new();

            // build the mechanical ventilator using the gas capacitance model
            _ventin = new GasCapacitance("VENTIN", "internal ventilator reservoir", "GasCapacitance", false, 5.0, 5.4, 1000.0, 0.0, true)
            {
                pres_atm = p_atm
            };
            _ventin.InitModel(_models);
            _ventin.CalcModel();
            AirComposition.SetAirComposition(_ventin, fio2, temp, humidity);
            _models.Add(_ventin.name, _ventin);
            _vent_parts.Add(_ventin);

            _tubingin = new GasCapacitance("TUBINGIN", "inspiratory tubing", "GasCapacitance", false, tubing_volume, tubing_volume, tubing_elastance, 0.0, false)
            {
                pres_atm = p_atm
            };
            _tubingin.InitModel(_models);
            _tubingin.CalcModel();
            AirComposition.SetAirComposition(_tubingin, fio2, temp, humidity);
            _models.Add(_tubingin.name, _tubingin);
            _vent_parts.Add(_tubingin);

            _ettube = new GasCapacitance("ETTUBE", "endotracheal tube", "GasCapacitance", false, ettube_volume, ettube_volume, ettube_elastance, 0.0, false)
            {
                pres_atm = p_atm
            };
            _ettube.InitModel(_models);
            _ettube.CalcModel();
            AirComposition.SetAirComposition(_ettube, fio2, temp, humidity);
            _models.Add(_ettube.name, _ettube);
            _vent_parts.Add(_ettube);

            _tubingout = new GasCapacitance("TUBINGOUT", "expiratory tubing", "GasCapacitance", false, tubing_volume, tubing_volume, tubing_elastance, 0.0, false)
            {
                pres_atm = p_atm
            };
            _tubingout.InitModel(_models);
            _tubingout.CalcModel();
            AirComposition.SetAirComposition(_tubingout, fio2, temp, humidity);
            _models.Add(_tubingout.name, _tubingout);
            _vent_parts.Add(_tubingout);

            // build the mechanical ventilator using the gas capacitance model
            _ventout = new GasCapacitance("VENTOUT", "outside air of ventilator", "GasCapacitance", false, 1000.0, 1000.0, 1000.0, 0.0, true)
            {
                pres_atm = p_atm
            };
            _ventout.InitModel(_models);
            _ventout.CalcModel();
            AirComposition.SetAirComposition(_ventout, fio2, temp, humidity);
            _models.Add(_ventout.name, _ventout);
            _vent_parts.Add(_ventout);


            // connect all the gas capacitance
            _insp_valve = new GasResistor("INSPVALVE", "inspiration valve", "GasResistor", false, false, true, "VENTIN", "TUBINGIN", 30000, 100000, 0);
            _insp_valve.InitModel(_models);
            _models.Add(_insp_valve.name, _insp_valve);
            _vent_parts.Add(_insp_valve);

            _tubingin_ettube = new GasResistor("TUBINGIN_ETTUBE", "tubing in to et tube", "GasResistor", false, false, false, "TUBINGIN", "ETTUBE", 25, 25, 0);
            _tubingin_ettube.InitModel(_models);
            _models.Add(_tubingin_ettube.name, _tubingin_ettube);
            _vent_parts.Add(_tubingin_ettube);

            _ettube_ds = new GasResistor("ETTUBE_DS", "et tube to dead space", "GasResistor", false, false, false, "ETTUBE", "DS", 50, 50, 0);
            _ettube_ds.InitModel(_models);
            _models.Add(_ettube_ds.name, _ettube_ds);
            _vent_parts.Add(_ettube_ds);

            _ettube_tubingout = new GasResistor("ETTUBE_TUBINGOUT", "et tube to expiration tubing", "GasResistor", false, false, false, "ETTUBE", "TUBINGOUT", 25, 25, 0);
            _ettube_tubingout.InitModel(_models);
            _models.Add(_ettube_tubingout.name, _ettube_tubingout);
            _vent_parts.Add(_ettube_tubingout);

            _exp_valve = new GasResistor("EXPVALVE", "expiration valve", "GasResistor", false, false, true, "TUBINGOUT", "VENTOUT", 300000, 300000, 0);
            _exp_valve.InitModel(_models);
            _models.Add(_exp_valve.name, _exp_valve);
            _vent_parts.Add(_exp_valve);

        }
    }
}

