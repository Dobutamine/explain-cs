using System;
using ExplainCoreLib.base_models;
namespace ExplainCoreLib.core_models
{
	public class Breathing : BaseModel
    {
        public bool breathing_enabled { get; set; } = true;
        public double resp_rate { get; set; } = 40.0;
        public double target_minute_volume { get; set; } = 0.4;
        public double target_tidal_volume { get; set; } = 17.0;
        public double vt_rr_ratio { get; set; } = 0.03;
        public double resp_muscle_pressure { get; set; } = 0.0;
        public double rmp_gain { get; set; } = 5.0;
        public double rmp_gain_max { get; set; } = 12.0;
        public double ie_ratio { get; set; } = 0.3;
        public List<string> targets { get; set; } = new();

        // Dependent variables
        public double resp_signal { get; set; } = 0.0;
        public double minute_volume { get; set; } = 0.0;
        public double exp_tidal_volume { get; set; } = 0.0;
        public double insp_tidal_volume { get; set; } = 0.0;

        // Local variables
        private double _eMin4 = Math.Pow(Math.E, -4);
        private double _rmp_gain = 2.0;
        private double _ti = 0.4;
        private double _te = 1.0;
        private double _breath_timer = 0.0;
        private double _breath_interval = 60.0;
        private bool _insp_running = false;
        private double _insp_timer = 0.0;
        private double _ncc_insp = 0;
        private double _temp_insp_volume = 0.0;
        private bool _exp_running = false;
        private double _exp_timer = 0.0;
        private double _ncc_exp = 0;
        private double _temp_exp_volume = 0.0;


        public Breathing(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            bool _breathing_enabled,
            double _resp_rate,
            double _target_minute_volume,
            double _target_tidal_volume,
            double _vt_rr_ratio,
            double _resp_muscle_pressure,
            double _rmp_gain,
            double _rmp_gain_max,
            double _ie_ratio,
            List<string> _targets
            ) : base(_name, _description, _model_type, _is_enabled)
        {
            breathing_enabled = _breathing_enabled;
            resp_rate = _resp_rate;
            target_minute_volume = _target_minute_volume;
            target_tidal_volume = _target_tidal_volume;
            vt_rr_ratio = _vt_rr_ratio;
            resp_muscle_pressure = _resp_muscle_pressure;
            rmp_gain = _rmp_gain;
            rmp_gain_max = _rmp_gain_max;
            ie_ratio = _ie_ratio;
            targets = _targets;
        }
        public override void CalcModel()
        {
            // calculate the respiratory rate and target tidal volume from the target minute volume
            VtRrController();

            // calculate the rmp gain
            if (exp_tidal_volume < target_tidal_volume)
            {
                _rmp_gain += 0.0001;
            }
            if (exp_tidal_volume > target_tidal_volume)
            {
                _rmp_gain -= 0.0001;
            }
            if (_rmp_gain < 0)
            {
                _rmp_gain = 0;
            }
            if (_rmp_gain > rmp_gain_max)
            {
                _rmp_gain = rmp_gain_max;
            }

            // calculate the inspiratory and expiratory time
            _breath_interval = 60.0;
            if (resp_rate > 0)
            {
                _breath_interval = 60.0 / resp_rate;
                _ti = ie_ratio * _breath_interval; // in seconds
                _te = _breath_interval - _ti;     // in seconds
            }

            // is it time to start a breath?
            if (_breath_timer > _breath_interval)
            {
                _breath_timer = 0.0;
                _insp_running = true;
                _insp_timer = 0.0;
                _ncc_insp = 0;
            }

            // has the inspiration time elapsed?
            if (_insp_timer > _ti)
            {
                _insp_timer = 0.0;
                _insp_running = false;
                _exp_running = true;
                _ncc_exp = 0;
                _temp_exp_volume = 0.0;
                insp_tidal_volume = -_temp_insp_volume;
            }

            // has the expiration time elapsed?
            if (_exp_timer > _te)
            {
                _exp_timer = 0.0;
                _exp_running = false;
                _temp_insp_volume = 0.0;
                exp_tidal_volume = _temp_exp_volume;

                minute_volume = exp_tidal_volume * resp_rate;
            }

            // increase the timers
            _breath_timer += _t;

            if (_insp_running)
            {
                _insp_timer += _t;
                _ncc_insp++;
                _temp_insp_volume += ((GasResistor) _models["MOUTH_DS"]).flow * _t;
            }

            if (_exp_running)
            {
                _exp_timer += _t;
                _ncc_exp++;
                _temp_exp_volume += ((GasResistor) _models["MOUTH_DS"]).flow * _t;
            }

            // reset the respiratory muscle pressure
            resp_muscle_pressure = 0.0;

            // calculate the new respiratory muscle pressure
            if (breathing_enabled)
            {
                resp_muscle_pressure = CalcRespMusclePressure();
            }
            else
            {
                resp_rate = 0.0;
                target_tidal_volume = 0.0;
                resp_muscle_pressure = 0.0;
            }

            // transfer the respiratory muscle pressure to the targets
            foreach (var target in targets)
            {
                ((Capacitance) _models[target]).pres_mus = resp_muscle_pressure;
            }
        }

        public double CalcRespMusclePressure()
        {
            double mp = 0.0;
            // inspiration
            if (_insp_running)
            {
                mp = (_ncc_insp / (_ti / _t)) * _rmp_gain;
            }

            // expiration
            if (_exp_running)
            {
                mp = ((Math.Pow(Math.E, -4.0 * (_ncc_exp / (_te / _t))) - _eMin4) / (1.0 - _eMin4)) * _rmp_gain;
            }

            return mp;
        }

        public void VtRrController()
        {
            // calculate the spontaneous resp rate depending on the target minute volume (from ANS) and the set vt-rr ratio
            resp_rate = Math.Sqrt(target_minute_volume / vt_rr_ratio);

            // calculate the target tidal volume depending on the target resp rate and target minute volume (from ANS)
            if (resp_rate > 0)
            {
                target_tidal_volume = target_minute_volume / resp_rate;
            }
        }
    }
}

