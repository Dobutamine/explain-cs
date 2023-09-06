using System;
using ExplainCoreLib.base_models;
using ExplainCoreLib.core_models;
using ExplainCoreLib.functions;

namespace ExplainCoreLib.core_models
{
	public class Mob : BaseModel
	{
		public Mob(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled
			) : base(_name, _description, _model_type, _is_enabled)
        {
		}
	}
}

