using System;
using ExplainCoreLib.base_models;
using ExplainCoreLib.Interfaces;

namespace ExplainCoreLib.core_models
{
	public class GasCapacitance : Capacitance, IGas
	{

        public double humidity { get; set; } = 0.5;
        public double temp { get; set; } = 37.0;
        public double target_temp { get; set; } = 37.0;

        public double po2 { get; set; }
        public double pco2 { get; set; }
        public double pn2 { get; set; }
        public double ph2o { get; set; }
        public double pother { get; set; }

        public double fo2 { get; set; }
        public double fco2 { get; set; }
        public double fn2 { get; set; }
        public double fh2o { get; set; }
        public double fother { get; set; }

        public double ctotal { get; set; }
        public double co2 { get; set; }
        public double cco2 { get; set; }
        public double cn2 { get; set; }
        public double ch2o { get; set; }
        public double cother { get; set; }

        private double _gas_constant = 62.36367;

        public GasCapacitance(
            string _name,
            string _description,
            string _model_type,
            bool _is_enabled,
            double _u_vol,
            double _vol,
            double _el_base,
            double _el_k,
            bool _fixed_composition)
            : base(_name, _description, _model_type, _is_enabled, _u_vol, _vol, _el_base, _el_k, _fixed_composition) {}

        public override void CalcModel()
        {
            // add heat to the gas
            AddHeat();

            // add water vapour to the gas
            AddWaterVapour();

            // do the capacitance action
            base.CalcModel();

            // calculate the current gas composition
            CalcGasComposition();


        }

        public override void VolumeIn(double dvol, Capacitance model_from)
        {
            // if the capacitance has a fixed composition then return
            if (fixed_composition)
            {
                return;
            }

            // execute the parent class method
            base.VolumeIn(dvol, model_from);

            // change the gas composition
            if (vol <= 0)
            {
                vol = 0;
                ctotal = 0;
                co2 = 0;
                cco2 = 0;
                cn2 = 0;
                ch2o = 0;
                cother = 0;
                ctotal = 0;
            } else
            {
                // change the gas concentrations
                GasCapacitance gc = (GasCapacitance)model_from;

                double dco2 = (gc.co2 - co2) * dvol;
                co2 = (co2 * vol + dco2) / vol;

                double dcco2 = (gc.cco2 - cco2) * dvol;
                cco2 = (cco2 * vol + dcco2) / vol;

                double dcn2 = (gc.cn2 - cn2) * dvol;
                cn2 = (cn2 * vol + dcn2) / vol;

                double dch2o = (gc.ch2o - ch2o) * dvol;
                ch2o = (ch2o * vol + dch2o) / vol;

                double dcother = (gc.cother - cother) * dvol;
                cother = (cother * vol + dcother) / vol;

                double dtemp = (gc.temp - temp) * dvol;
                temp = (temp * vol + dtemp) / vol;
            }
        }

        public void CalcGasComposition()
        {
            // calculate the concentration in mmol/l using the sum of all concentrations
            ctotal = ch2o + co2 + cco2 + cn2 + cother;

            // protect against division by zero
            if (ctotal == 0)
            {
                return;
            }

            // calculate the partial pressures
            ph2o = (ch2o / ctotal) * pres;
            po2 = (co2 / ctotal) * pres;
            pco2 = (cco2 / ctotal) * pres;
            pn2 = (cn2 / ctotal) * pres;
            pother = (cother / ctotal) * pres;

            // calculate the fractions
            fh2o = ch2o / ctotal;
            fo2 = co2 / ctotal;
            fco2 = cco2 / ctotal;
            fn2 = cn2 / ctotal;
            fother = cother / ctotal;

        }
        public double CalcWaterVapourPressure()
        {
            // calculate the water vapour pressure in air depending on the temperature
            return Math.Pow(Math.E, 20.386 - 5132 / (temp + 273));
        }

        public void AddWaterVapour()
        {
            // Calculate water vapour pressure at compliance temperature
            double pH2Ot = CalcWaterVapourPressure();

            // do the diffusion from water vapour depending on the tissue water vapour and gas water vapour pressure
            double dH2O = 0.00001 * (pH2Ot - ph2o) * _t;
            if (vol > 0)
            {
                ch2o = (ch2o * vol + dH2O) / vol;
            }

            // as the water vapour also takes volume this is added to the compliance
            if (pres != 0 && !fixed_composition)
            {
                // as dH2O is in mmol/l we have convert it as the gas constant is in mol
                vol += ((_gas_constant * (273.15 + temp)) / pres) * (dH2O / 1000.0);
            }
        }

        public void AddHeat()
        {
            // calculate a temperature change depending on the target temperature and the current temperature
            double dT = (target_temp - temp) * 0.0005;
            temp += dT;

            // change the volume as the temperature changes
            if (pres != 0 && !fixed_composition)
            {
                // as Ctotal is in mmol/l we have convert it as the gas constant is in mol
                double dV = (ctotal * vol * _gas_constant * dT) / pres;
                vol += dV / 1000.0;
            }

            // guard against negative volumes
            if (vol < 0)
            {
                vol = 0;
            }

        }

    }
}

