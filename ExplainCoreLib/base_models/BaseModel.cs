using System;
using System.Collections.Generic;
namespace ExplainCoreLib.base_models
{
	public class BaseModel
	{
		public string name { get; set; }
		public string description{ get; set; }
		public string model_type { get; set; }
        public bool is_enabled { get; set; }
		public bool is_initialized = false;
		public Dictionary<string, BaseModel> _models = new();
		public double _t = 0.0005;

        public BaseModel(string _name, string _description, string _modelType, bool _isEnabled)
		{
            name = _name;
			description = _description;
			model_type = _modelType;
			is_enabled = _isEnabled;
			is_initialized = false;
        }

		public virtual bool InitModel(Dictionary<string, BaseModel> models, double stepsize = 0.0005)
		{
			_models = models;
			_t = stepsize;
			is_initialized = true;

			return is_initialized;
		}

		public virtual void StepModel()
		{
            if (is_enabled && is_initialized)
			{
				CalcModel();
			}
		}

		public virtual void CalcModel() {}
	}
}

