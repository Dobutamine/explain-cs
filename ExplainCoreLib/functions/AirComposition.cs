using System;
using ExplainCoreLib.core_models;
namespace ExplainCoreLib.functions
{
	public static class AirComposition
	{
        // define an object which holds the air composition at 0 degrees celcius and 0 humidity
        static readonly double fo2_dry = 0.205;
        static readonly double fco2_dry = 0.000392;
        static readonly double fn2_dry = 0.794608;
        static readonly double fother_dry = 0.0;

        public static void SetAirComposition(GasCapacitance gc, double fio2 = 0.205, double temp = 37.0, double humidity = 0.5)
        {
            AirCompositionResult result = CalcAirComposition(gc, fio2, temp, humidity);

            // store a result
            gc.po2 = result.po2;
            gc.pco2 = result.pco2;
            gc.pn2 = result.pn2;
            gc.pother = result.pother;
            gc.ph2o = result.ph2o;

            gc.fo2 = result.fo2;
            gc.fco2 = result.fco2;
            gc.fn2 = result.fn2;
            gc.fother = result.fother;
            gc.fh2o = result.fh2o;

            gc.co2 = result.co2;
            gc.cco2 = result.cco2;
            gc.cn2 = result.cn2;
            gc.cother = result.cother;
            gc.ch2o = result.ch2o;

        }

		public static AirCompositionResult CalcAirComposition(GasCapacitance gascomp, double fio2, double temp, double humidity)
		{
            // make sure the latest pressure is available
            gascomp.CalcModel();

            // calculate the dry air composition depending on the supplied fio2
            double new_fo2_dry = fio2;
            double new_fco2_dry = fco2_dry * (1.0 - fio2) / (1.0 - fo2_dry);
            double new_fn2_dry = fn2_dry * (1.0 - fio2) / (1.0 - fo2_dry);
            double new_fother_dry = fother_dry * (1.0 - fio2) / (1.0 - fo2_dry);

            // if temp is set then transfer that temp to the gascomp
            gascomp.target_temp = temp;
            gascomp.temp = temp;

            // if humidity is set then transfer that humidity to the gascomp
            gascomp.humidity = humidity;

            // calculate the air composition
            return NewAirComposition(gascomp.pres, new_fo2_dry, new_fco2_dry, new_fn2_dry, new_fother_dry, gascomp.temp, gascomp.humidity);
        }

		private static AirCompositionResult NewAirComposition(double pressure, double new_fo2_dry, double new_fco2_dry, double new_fn2_dry, double new_fother_dry, double temp, double humidity)
		{
            const double GasConstant = 62.36367;

            double ctotal = (pressure / (GasConstant * (273.15 + temp))) * 1000.0;

            double ph2o = (Math.Pow(Math.E, 20.386 - 5132 / (temp + 273)) * humidity);
            double fh2o = ph2o / pressure;
            double ch2o = fh2o * ctotal;

            double po2 = new_fo2_dry * (pressure - ph2o);
            double fo2 = po2 / pressure;
            double co2 = fo2 * ctotal;

            double pco2 = new_fco2_dry * (pressure - ph2o);
            double fco2 = pco2 / pressure;
            double cco2 = fco2 * ctotal;

            double pn2 = new_fn2_dry * (pressure - ph2o);
            double fn2 = pn2 / pressure;
            double cn2 = fn2 * ctotal;

            double pother = new_fother_dry * (pressure - ph2o);
            double fother = pother / pressure;
            double cother = fother * ctotal;

            AirCompositionResult result = new()
            {
                po2 = po2,
                pco2 = pco2,
                pn2 = pn2,
                pother = pother,
                ph2o = ph2o,

                fo2 = fo2,
                fco2 = fco2,
                fn2 = fn2,
                fother = fother,
                fh2o = fh2o,

                co2 = co2,
                cco2 = cco2,
                cn2 = cn2,
                cother = cother,
                ch2o = ch2o,
            };

            return result;
        }
	}

    public struct AirCompositionResult
    {
        public double po2 { get; set; }
        public double pco2 { get; set; }
        public double pn2 { get; set; }
        public double pother { get; set; }
        public double ph2o { get; set; }

        public double fo2 { get; set; }
        public double fco2 { get; set; }
        public double fn2 { get; set; }
        public double fother { get; set; }
        public double fh2o { get; set; }

        public double co2 { get; set; }
        public double cco2 { get; set; }
        public double cn2 { get; set; }
        public double cother { get; set; }
        public double ch2o { get; set; }
    }
}

