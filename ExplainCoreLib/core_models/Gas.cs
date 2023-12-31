﻿using System;
using System.Reflection;
using ExplainCoreLib.base_models;
using ExplainCoreLib.Interfaces;
using ExplainCoreLib.functions;

namespace ExplainCoreLib.core_models
{
	public class Gas : BaseModel
	{
        public Dictionary<string, double> humidity_settings { get; set; } = new();
        public Dictionary<string, double> temp_settings { get; set; } = new();
        public double fio2 { get; set; } = 0.21;
        public double pres_atm { get; set; } = 760.0;

        public Gas(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            double _pres_atm,
            double _fio2,
            Dictionary<string, double> _humidity_settings,
            Dictionary<string, double> _temp_settings) : base (_name, _description, _model_type, _is_enabled)
		{
            pres_atm = _pres_atm;
            fio2 = _fio2;
            humidity_settings = _humidity_settings;
            temp_settings = _temp_settings;
		}

        public override bool InitModel(Dictionary<string, BaseModel> models, double stepsize = 0.0005)
        {
            base.InitModel(models, stepsize);
            
            SetAtmosphericPressure();
            SetTemperatures();
            SetHumidity();

            // we need a pressure to calculate the composition of the air in the gas capacitances
            // so we need to run the calc model on the gas capacitances first
            foreach(var model in _models)
            {
                // store a reference
                if (model.Value is GasCapacitance gc)
                {
                    // calculate the gas composition
                    AirComposition.SetAirComposition(gc, fio2, gc.temp, gc.humidity);
                }
            }

            is_initialized = true;
            return is_initialized;
        }

        public void SetAtmosphericPressure ()
        {
            foreach (var m in _models)
            {
                if (m.Value is IGas)
                {
                    ((Capacitance)m.Value).pres_atm = pres_atm;
                }
            }
        }

        public void SetTemperatures()
        {
            foreach(var t in temp_settings)
            {
                ((IGas)_models[t.Key]).temp = t.Value;
                ((IGas)_models[t.Key]).target_temp = t.Value;
            }
        }

        public void SetHumidity()
        {
            foreach (var t in humidity_settings)
            {
                ((IGas)_models[t.Key]).humidity = t.Value;
            }
        }
    }
}

