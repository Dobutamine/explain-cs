using System;
using ExplainCoreLib.base_models;
using ExplainCoreLib.core_models;
using ExplainCoreLib.functions;
using ExplainCoreLib.Interfaces;

namespace ExplainCoreLib.core_models
{
    public class Ans : BaseModel
    {
        public string baroreceptor_location { get; set; } = "AA";
        public string chemoreceptor_location { get; set; } = "AA";
        public double heart_rate_ref { get; set; } = 120;
        public double minute_volume_ref { get; set; } = 0.66;
        public double set_baro { get; set; } = 53.0;
        public double max_baro { get; set; } = 76.0;
        public double min_baro { get; set; } = 30.0;
        public double g_map_hp { get; set; } = -3.4;
        public double tc_map_hp { get; set; } = 10.0;
        public double set_po2 { get; set; } = 80.0;
        public double max_po2 { get; set; } = 150.0;
        public double min_po2 { get; set; } = 30.0;
        public double g_po2_hp { get; set; } = 0.0;
        public double g_po2_ve { get; set; } = -0.005;
        public double tc_po2_hp { get; set; } = 12.0;
        public double tc_po2_ve { get; set; } = 12.0;
        public double set_pco2 { get; set; } = 40.0;
        public double max_pco2 { get; set; } = 75.0;
        public double min_pco2 { get; set; } = 15.0;
        public double g_pco2_hp { get; set; } = 0.0;
        public double g_pco2_ve { get; set; } = 0.02;
        public double tc_pco2_hp { get; set; } = 12.0;
        public double tc_pco2_ve { get; set; } = 12.0;
        public double set_ph { get; set; } = 7.3;
        public double max_ph { get; set; } = 7.7;
        public double min_ph { get; set; } = 6.9;
        public double g_ph_hp { get; set; } = 0.0;
        public double g_ph_ve { get; set; } = -1.5;
        public double tc_ph_hp { get; set; } = 10.0;
        public double tc_ph_ve { get; set; } = 10.0;

        // local variables
        BloodCapacitance? _baroreceptor;
        BloodCapacitance? _chemoreceptor;
        Heart? _heart;
        Breathing? _breathing;
        private double _a_map = 0.0;
        private double _a_ph = 0.0;
        private double _a_po2 = 0.0;
        private double _a_pco2 = 0.0;

        private double _d_map_hp = 0.0;
        private double _d_po2_hp = 0.0;
        private double _d_pco2_hp = 0.0;
        private double _d_ph_hp = 0.0;

        private double _d_po2_ve = 0.0;
        private double _d_pco2_ve = 0.0;
        private double _d_ph_ve = 0.0;

        public Ans(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            string _baroreceptor_location,
            string _chemoreceptor_location,
            double _heart_rate_ref,
            double _minute_volume_ref,
            double _set_baro,
            double _max_baro,
            double _min_baro,
            double _g_map_hp,
            double _tc_map_hp,
            double _set_po2,
            double _max_po2,
            double _min_po2,
            double _g_po2_hp,
            double _g_po2_ve,
            double _tc_po2_hp,
            double _tc_po2_ve,
            double _set_pco2,
            double _max_pco2,
            double _min_pco2,
            double _g_pco2_hp,
            double _g_pco2_ve,
            double _tc_pco2_hp,
            double _tc_pco2_ve,
            double _set_ph,
            double _max_ph,
            double _min_ph,
            double _g_ph_hp,
            double _g_ph_ve,
            double _tc_ph_hp,
            double _tc_ph_ve
            ) : base(_name, _description, _model_type, _is_enabled)
        {
            baroreceptor_location = _baroreceptor_location;
            chemoreceptor_location = _chemoreceptor_location;
            heart_rate_ref = _heart_rate_ref;
            minute_volume_ref = _minute_volume_ref;
            set_baro = _set_baro;
            max_baro = _max_baro;
            min_baro = _min_baro;
            g_map_hp = _g_map_hp;
            tc_map_hp = _tc_map_hp;

            set_po2 = _set_po2;
            max_po2 = _max_po2;
            min_po2 = _min_po2;
            g_po2_hp = _g_po2_hp;
            g_po2_ve = _g_po2_ve;
            tc_po2_hp = _tc_po2_hp;
            tc_po2_ve = _tc_po2_ve;

            set_pco2 = _set_pco2;
            max_pco2 = _max_pco2;
            min_pco2 = _min_pco2;
            g_pco2_hp = _g_pco2_hp;
            g_pco2_ve = _g_pco2_ve;
            tc_pco2_hp = _tc_pco2_hp;
            tc_pco2_ve = _tc_pco2_ve;

            set_ph = _set_ph;
            max_ph = _max_ph;
            min_ph = _min_ph;
            g_ph_hp = _g_ph_hp;
            g_ph_ve = _g_ph_ve;
            tc_ph_hp = _tc_ph_hp;
            tc_ph_ve = _tc_ph_ve;
        }

        public override bool InitModel(Dictionary<string, BaseModel> models, double stepsize = 0.0005)
        {
            base.InitModel(models, stepsize);

            // get a reference to the models
            _baroreceptor = (BloodCapacitance)_models[baroreceptor_location];
            _chemoreceptor = (BloodCapacitance)_models[chemoreceptor_location];
            _heart = (Heart)_models["Heart"];
            _breathing = (Breathing)_models["Breathing"];

            is_initialized = true;
            return is_initialized;
        }

        public override void CalcModel()
        {
            // get the baroreflex input
            double _baro_pres = _baroreceptor.pres;

            // for the chemoreflex we need the acidbase and oxygenation of the location of the chemoreceptor
            BloodComposition.SetBloodComposition(_chemoreceptor);

            // get the chemoreflex inputs
            double _po2 = _chemoreceptor.aboxy["po2"];
            double _pco2 = _chemoreceptor.aboxy["pco2"];
            double _ph = _chemoreceptor.aboxy["ph"];

            // calculate the activation function of the baroreceptor
            _a_map = ActivationFunction.Activation(_baro_pres, max_baro, set_baro, min_baro);

            // calculate the activation functions of the chemoreceptors
            _a_po2 = ActivationFunction.Activation(_po2, max_po2, set_po2, min_po2);
            _a_pco2 = ActivationFunction.Activation(_pco2, max_pco2, set_pco2, min_pco2);
            _a_ph = ActivationFunction.Activation(_ph, max_ph, set_ph, min_ph);

            // calculate the effectors and use the time constant
            _d_map_hp = _t * ((1 / tc_map_hp) * (-_d_map_hp + _a_map)) + _d_map_hp;
            _d_po2_hp = _t * ((1 / tc_po2_hp) * (-_d_po2_hp + _a_po2)) + _d_po2_hp;
            _d_pco2_hp = _t * ((1 / tc_pco2_hp) * (-_d_pco2_hp + _a_pco2)) + _d_pco2_hp;
            _d_ph_hp = _t * ((1 / tc_ph_hp) * (-_d_ph_hp + _a_ph)) + _d_ph_hp;
            _d_po2_ve = _t * ((1 / tc_po2_ve) * (-_d_po2_ve + _a_po2)) + _d_po2_ve;
            _d_pco2_ve = _t * ((1 / tc_pco2_ve) * (-_d_pco2_ve + _a_pco2)) + _d_pco2_ve;
            _d_ph_ve = _t * ((1 / tc_ph_ve) * (-_d_ph_ve + _a_ph)) + _d_ph_ve;

            // apply the effects using the gain
            double new_heartrate = heart_rate_ref + g_map_hp * _d_map_hp + g_po2_hp * _d_po2_hp + g_pco2_hp * _d_pco2_hp + g_ph_hp * _d_ph_hp;
            if (new_heartrate < 5)
            {
                new_heartrate = 5;
            }
            _heart.heart_rate = new_heartrate;


            double target_mv = minute_volume_ref + g_po2_ve * _d_po2_ve + g_pco2_ve * _d_pco2_ve + g_ph_ve * _d_ph_ve;
            if (target_mv < 0.01)
            {
                target_mv = 0.01;
            }
            _breathing.target_minute_volume = target_mv;
        }
    }
}

