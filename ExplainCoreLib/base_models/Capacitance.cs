using System;
using ExplainCoreLib.base_models;
using ExplainCoreLib.Interfaces;
namespace ExplainCoreLib.base_models
{
	public class Capacitance : BaseModel
	{
		public double u_vol { get; set; } = 0.0;
		public double el_base { get; set; } = 0.0;
		public double el_k { get; set; } = 0.0;

		public double u_vol_factor { get; set; } = 1.0;
        public double el_base_factor { get; set; } = 1.0;
        public double el_k_factor { get; set; } = 1.0;

        public double vol { get; set; } = 1.0;
        public double pres { get; set; } = 0.0;
        public double pres_ext { get; set; } = 1.0;
        public double pres_cc { get; set; } = 1.0;
        public double pres_atm { get; set; } = 1.0;
        public double pres_mus { get; set; } = 1.0;

        public Capacitance(
			string _name,
			string _description,
			string _model_type,
			bool _is_enabled,
			double _u_vol,
			double _vol,
			double _el_base,
			double _el_k
			)
			: base (_name, _description, _model_type, _is_enabled)
		{
			u_vol = _u_vol;
			vol = _vol;
			el_base = _el_base;
			el_k = _el_k;
        }

        public virtual void volume_in(double dvol, Capacitance model_from) { }

    }
}

