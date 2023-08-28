using System;
using ExplainCoreLib.base_models;

namespace ExplainCoreLib.core_models
{
	public class Container : Capacitance
	{
        public List<string> contained_components { get; set; } = new List<string>();
        public double vol_ext { get; set; } = 0.0;

        public List<Capacitance> _caps { get; set; } = new List<Capacitance>();
 
        public Container(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            List<string> _contained_components,
            double _u_vol,
            double _vol,
            double _vol_ext,
            double _el_base,
            double _el_k,
            bool _fixed_composition)
            : base(_name, _description, _model_type, _is_enabled, _u_vol, _vol, _el_base, _el_k, _fixed_composition) {
            contained_components = _contained_components;
            vol_ext = _vol_ext;
        }

        public override bool InitModel(Dictionary<string, BaseModel> models, double stepsize = 0.0005)
        {
            base.InitModel(models, stepsize);
            // store a reference to the contained components for easy access
            foreach (var m in contained_components)
            {
                try
                {
                    _caps.Add((Capacitance)_models[m]);
                }
                catch
                {
                    Console.WriteLine("Container did not find {0}", m);
                }
            }


            is_initialized = true;
            return is_initialized;
        }

        public override void CalcModel()
        {
            // calculate the current volume
            vol = vol_ext;
            foreach (var rm in _caps)
            {
                vol += rm.vol;
            }

            // calculate the pressure
            base.CalcModel();

            // transfer the pressures to the models the container contains
            foreach (var rm in _caps)
            {
                rm.pres_ext = pres;
            }
        }
    }
}

