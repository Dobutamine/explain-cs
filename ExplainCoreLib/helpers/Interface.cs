using System;
using System.Collections.Generic;
using ExplainCoreLib.base_models;

namespace ExplainCoreLib.helpers
{

    public class Interface
    {
        private Dictionary<string, BaseModel> _models = new();
        private double _t = 0.0005;

        public Interface(Dictionary<string, BaseModel> models, double stepsize = 0.0005)
        {
            _models = models;
            _t = stepsize;


        }

        public void AirwayPressure(double _airway_pressure)
        {

        }

        public void AirwayPosition(double _airway_position)
        {

        }

        public void TactileStimulation(string _stimulus_type, double _stimulus_force)
        {

        }

        public void CompressionPressure(double _comp_pressure)
        {

        }

        public void AdministerFluid(string _fluid_type, double _fluid_dose, double _infusion_time = 5)
        {

        }
        public void AdministerDrug(string _drug_type, double _drug_dose, double _infusion_time = 5)
        {

        }


    }
}

