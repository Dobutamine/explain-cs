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
        public bool fixed_composition { get; set; } = false;

        public Capacitance(
			string _name,
			string _description,
			string _model_type,
			bool _is_enabled,
			double _u_vol,
			double _vol,
			double _el_base,
			double _el_k,
            bool _fixed_composition
			)
			: base (_name, _description, _model_type, _is_enabled)
		{
			u_vol = _u_vol;
			vol = _vol;
			el_base = _el_base;
			el_k = _el_k;
            fixed_composition = _fixed_composition;
        }

        public override void CalcModel()
        {
            // Calculate the pressure depending on the volume, unstressed volume, elastance, and external pressure
            pres = el_k * el_k_factor * Math.Pow(vol - (u_vol * u_vol_factor), 2) +
                   el_base * el_base_factor * (vol - (u_vol * u_vol_factor)) +
                   pres_ext + pres_cc + pres_atm + pres_mus;

            // Reset the pressures that are recalculated every model iteration
            pres_ext = 0.0;
            pres_cc = 0.0;
            pres_mus = 0.0;
        }
		
        public virtual void volume_in(double dvol, Capacitance model_from) {
            // increase the volume
            vol += dvol;
        }

        public virtual double volume_out(double dvol)
        {
            if (fixed_composition)
            {
                return 0;
            }

            // Assume all dvol can be removed
            double vol_not_removed = 0.0;

            // Decrease the volume
            vol -= dvol;

            // Guard against negative volumes
            if (vol < 0)
            {
                // We need to remove more volume than we have, which is not possible. Calculate how much volume can be removed
                vol_not_removed = -vol;

                // Reset the volume to zero
                vol = 0.0;
            }

            return vol_not_removed;

        }

    }
}

