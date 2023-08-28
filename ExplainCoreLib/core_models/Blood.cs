using System;
using ExplainCoreLib.base_models;
using ExplainCoreLib.core_models;
using ExplainCoreLib.Interfaces;

namespace ExplainCoreLib.core_models
{
	public class Blood : BaseModel, IBlood
	{
        public Dictionary<string, double> solutes { get; set; } = new();
        public Dictionary<string, double> aboxy { get; set; } = new();

        public Blood(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            Dictionary<string, double> _solutes,
            Dictionary<string, double> _aboxy
            ) : base(_name, _description, _model_type, _is_enabled)
        {
            solutes = _solutes;
            aboxy = _aboxy;
		}

        public override bool InitModel(Dictionary<string, BaseModel> models, double stepsize = 0.0005)
        {
            base.InitModel(models, stepsize);

            // set the solutes of all models which implement the IBlood interface
            foreach(var m in models)
            {
                if (m.Value is IBlood blood)
                {
                    blood.solutes = new Dictionary<string, double>(solutes);
                    blood.aboxy = new Dictionary<string, double>(aboxy);
                }
             
            }
            is_initialized = true;
            return is_initialized;
        }
    }
}

