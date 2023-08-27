using System;
using System.Collections.Generic;
using System.Reflection;
using ExplainCoreLib.base_models;

namespace ExplainCoreLib.helpers
{
	public class TaskScheduler
	{
        private Dictionary<string, BaseModel> _models = new();
        private double _t = 0.0005;
        public TaskScheduler(Dictionary<string, BaseModel> models, double stepsize = 0.0005)
        {
            _models = models;
            _t = stepsize;

        }
        public void RunTasks(double clock)
        {
        }
    }
}

