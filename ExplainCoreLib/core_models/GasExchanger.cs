using System;
using System.Xml.Linq;
using ExplainCoreLib.base_models;
using ExplainCoreLib.core_models;
using ExplainCoreLib.functions;
namespace ExplainCoreLib.core_models
{
	public class GasExchanger : BaseModel
    {
        // Independent variables
        public double dif_o2 { get; set; } = 0.01;
        public double dif_o2_factor { get; set; } = 1.0;
        public double dif_co2 { get; set; } = 0.01;
        public double dif_co2_factor { get; set; } = 1.0;

        public string comp_blood { get; set; }
        public string comp_gas { get; set; }


        // Local variables
        private BloodCapacitance? _blood;
        private GasCapacitance? _gas;
        private double _flux_o2 = 0;
        private double _flux_co2 = 0;

        public GasExchanger(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            string _comp_blood,
            string _comp_gas,
            double _dif_co2,
            double _dif_o2
            ) : base(_name, _description, _model_type, _is_enabled)
        {
            comp_blood = _comp_blood;
            comp_gas = _comp_gas;
            dif_co2 = _dif_co2;
            dif_o2 = _dif_o2;
		}

        public override bool InitModel(Dictionary<string, BaseModel> models, double stepsize = 0.0005)
        {
            base.InitModel(models, stepsize);

            // find a reference to the blood and gas capacitance
            _blood = (BloodCapacitance)_models[comp_blood];
            _gas = (GasCapacitance)_models[comp_gas];

            is_initialized = true;
            return is_initialized;
        }

        public override void CalcModel()
        {
            // calculate the po2 and pco2 in the blood compartments
            var result = Acidbase.CalcAcidBaseFromTco2(_blood);
            if (result.valid)
            {
                _blood.aboxy["ph"] = result.ph;
                _blood.aboxy["pco2"] = result.pco2;
                _blood.aboxy["hco3"] = result.hco3;
                _blood.aboxy["be"] = result.be;
                _blood.aboxy["sid_app"] = result.sid_app;
            }

            var result_oxy = Oxygenation.CalcOxygenationFromTo2(_blood);
            if (result_oxy.valid)
            {
                _blood.aboxy["po2"] = result_oxy.po2;
                _blood.aboxy["so2"] = result_oxy.so2;
            }

            // get the partial pressures and gas concentrations from the components
            double po2_blood = _blood.aboxy["po2"];
            double pco2_blood = _blood.aboxy["pco2"];
            double to2_blood = _blood.aboxy["to2"];
            double tco2_blood = _blood.aboxy["tco2"];

            double co2_gas = _gas.co2;
            double cco2_gas = _gas.cco2;
            double po2_gas = _gas.po2;
            double pco2_gas = _gas.pco2;

            // calculate the O2 flux from the blood to the gas compartment
            _flux_o2 = (po2_blood - po2_gas) * dif_o2 * dif_o2_factor * _t;

            // calculate the new O2 concentrations of the gas and blood compartments
            double new_to2_blood = (to2_blood * _blood.vol - _flux_o2) / _blood.vol;
            if (new_to2_blood < 0)
            {
                new_to2_blood = 0;
            }

            double new_co2_gas = (co2_gas * _gas.vol + _flux_o2) / _gas.vol;
            if (new_co2_gas < 0)
            {
                new_co2_gas = 0;
            }

            // calculate the CO2 flux from the blood to the gas compartment
            _flux_co2 = (pco2_blood - pco2_gas) * dif_co2 * dif_co2_factor * _t;

            // calculate the new CO2 concentrations of the gas and blood compartments
            double new_tco2_blood = (tco2_blood * _blood.vol - _flux_co2) / _blood.vol;
            if (new_tco2_blood < 0)
            {
                new_tco2_blood = 0;
            }

            double new_cco2_gas = (cco2_gas * _gas.vol + _flux_co2) / _gas.vol;
            if (new_cco2_gas < 0)
            {
                new_cco2_gas = 0;
            }

            // transfer the new concentrations
            _blood.aboxy["to2"] = new_to2_blood;
            _blood.aboxy["tco2"] = new_tco2_blood;
            _gas.co2 = new_co2_gas;
            _gas.cco2 = new_cco2_gas;
        }
    }
}

