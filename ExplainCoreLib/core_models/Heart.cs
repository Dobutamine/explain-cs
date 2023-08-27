using System;
using ExplainCoreLib.base_models;
namespace ExplainCoreLib.core_models
{
	public class Heart : BaseModel
	{
        // independent variables
        public double heart_rate { get; set; } = 140.0;
        public double pq_time { get; set; } = 0.1;
        public double qrs_time { get; set; } = 0.075;
        public double qt_time { get; set; } = 0.4;

        // dependent variables
        public double cqt_time = 0.0;
        public double ncc_atrial = 0.0;
        public double ncc_ventricular = 0.0;
        public double aaf = 0.0;
        public double vaf = 0.0;

        public string left_atrium { get; set; }
        public string right_atrium { get; set; }
        public string left_ventricle { get; set; }
        public string right_ventricle { get; set; }

        private BloodTimeVaryingElastance? la { get; set; }
        private BloodTimeVaryingElastance? ra { get; set; }
        private BloodTimeVaryingElastance? lv { get; set; }
        private BloodTimeVaryingElastance? rv { get; set; }

        private double _sa_node_interval = 0.0;
        private double _sa_node_timer = 0.0;
        private bool _pq_running = false;
        private double _pq_timer = 0.0;
        private bool _qrs_running = false;
        private double _qrs_timer = 0.0;
        private bool _qt_running = false;
        private double _qt_timer = 0.0;
        private bool _ventricle_is_refractory = false;
        private double _kn = 0.579;

        public Heart(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            double _heart_rate,
            double _pq_time,
            double _qrs_time,
            double _qt_time,
            string _la,
            string _ra,
            string _lv,
            string _rv
           ) : base(_name, _description, _model_type, _is_enabled)
		{
            heart_rate = _heart_rate;
            pq_time = _pq_time;
            qrs_time = _qrs_time;
            qt_time = _qt_time;

            left_atrium = _la;
            right_atrium = _ra;
            left_ventricle = _lv;
            right_ventricle = _rv;
		}

        public override bool InitModel(Dictionary<string, BaseModel> models, double stepsize = 0.0005)
        {
            base.InitModel(models, stepsize);

            // find a reference to the heart chambers
            la = (BloodTimeVaryingElastance)models[left_atrium];
            ra = (BloodTimeVaryingElastance)models[right_atrium];
            lv = (BloodTimeVaryingElastance)models[left_ventricle];
            rv = (BloodTimeVaryingElastance)models[right_ventricle];

            is_initialized = true;

            return true;
        }

        public override void CalcModel()
        {
            // calculate the qtc time depending on the heartrate
            cqt_time = calc_qtc(heart_rate);

            // calculate the sinus node interval in seconds depending on the heart rate
            _sa_node_interval = 60.0 / heart_rate;

            // has the sinus node period elapsed?
            if (_sa_node_timer > _sa_node_interval)
            {
                // reset the sinus node timer
                _sa_node_timer = 0.0;
                // signal that the pq-time starts running
                _pq_running = true;
                // reset the atrial activation curve counter
                ncc_atrial = -1;
            }

            // Check if the PQ time period has elapsed
            if (_pq_timer > pq_time)
            {
                // Reset the PQ timer
                _pq_timer = 0.0;
                // Signal that the PQ timer has stopped
                _pq_running = false;
                // Check whether the ventricles are in a refractory state
                if (!_ventricle_is_refractory)
                {
                    // Signal that the QRS time starts running
                    _qrs_running = true;
                    // Reset the ventricular activation curve
                    ncc_ventricular = -1;
                }
            }

            // Check if the QRS time period has elapsed
            if (_qrs_timer > qrs_time)
            {
                // Reset the QRS timer
                _qrs_timer = 0.0;
                // Signal that the QRS timer has stopped
                _qrs_running = false;
                // Signal that the AT timer starts running
                _qt_running = true;
                // Signal that the ventricles are now in a refractory state
                _ventricle_is_refractory = true;
            }

            // Check if the QT time period has elapsed
            if (_qt_timer > cqt_time)
            {
                // Reset the QT timer
                _qt_timer = 0.0;
                // Signal that the QT timer has stopped
                _qt_running = false;
                // Signal that the ventricles are coming out of their refractory state
                _ventricle_is_refractory = false;
            }

            // Increase the timers with the modeling step size as set by the model base class
            _sa_node_timer += _t;
            if (_pq_running)
            {
                _pq_timer += _t;
            }
            if (_qrs_running)
            {
                _qrs_timer += _t;
            }
            if (_qt_running)
            {
                _qt_timer += _t;
            }

            // Increase the heart activation function counters
            ncc_atrial += 1;
            ncc_ventricular += 1;

            // calculate the varying elastance factor
            CalcVaryingElastance();
        }

        public void CalcVaryingElastance()
        {
            // Calculate the atrial activation factor
            if (ncc_atrial >= 0 && ncc_atrial < pq_time / _t)
            {
                aaf = Math.Sin(Math.PI * (ncc_atrial / (pq_time / _t)));
            }
            else
            {
                aaf = 0.0;
            }

            // Calculate the ventricular activation factor
            double ventricular_duration = qrs_time + cqt_time;
            if (ncc_ventricular >= 0 && ncc_ventricular < ventricular_duration / _t)
            {
                vaf = (ncc_ventricular / (_kn * (ventricular_duration / _t))) *
                      Math.Sin(Math.PI * (ncc_ventricular / (ventricular_duration / _t)));
            }
            else
            {
                vaf = 0.0;
            }

            // Transfer the activation factor to the heart components
            la.act_factor = aaf;
            ra.act_factor = aaf;
            lv.act_factor = vaf;
            rv.act_factor = vaf;
        }


        private double calc_qtc(double hr)
        {
            if (hr > 10)
            {
                return qt_time * Math.Sqrt(60.0 / hr);
            } else
            {
                return qt_time * Math.Sqrt(6);
            }
        }
    }
}

