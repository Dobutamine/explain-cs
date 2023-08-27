﻿using System;
using ExplainCoreLib.base_models;
using ExplainCoreLib.Interfaces;

namespace ExplainCoreLib.core_models
{
	public class BloodTimeVaryingElastance : TimeVaryingElastance, IBlood
	{
        public double sodium { get; set; } = 140.0;

        public BloodTimeVaryingElastance(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            double _u_vol,
            double _vol,
            double _el_min,
            double _el_max,
            double _el_k,
            bool _fixed_composition)
            : base(_name, _description, _model_type, _is_enabled, _u_vol, _vol, _el_min, _el_max, _el_k, _fixed_composition) {}

    }
}

