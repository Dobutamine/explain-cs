using System;
using System.Collections.Generic;
using ExplainCoreLib.base_models;

namespace ExplainCoreLib.helpers
{

    public class DataCollector
	{
        private Dictionary<string, BaseModel> _models = new();
        private double _t = 0.0005;

        public DataCollector(Dictionary<string, BaseModel> models, double stepsize = 0.0005)
		{
			_models = models;
			_t = stepsize;

		}

		public void CollectData(double clock) {
		}
	}
}

