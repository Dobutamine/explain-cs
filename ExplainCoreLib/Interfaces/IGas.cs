using System;
namespace ExplainCoreLib.Interfaces
{
	public interface IGas
	{
        double po2 { get; set; }
        double pco2 { get; set; }
        double pn2 { get; set; }
        double ph2o { get; set; }
        double pother { get; set; }

        double fo2 { get; set; }
        double fco2 { get; set; }
        double fn2 { get; set; }
        double fh2o { get; set; }
        double fother { get; set; }

        double co2 { get; set; }
        double cco2 { get; set; }
        double cn2 { get; set; }
        double ch2o { get; set; }
        double cother { get; set; }

        double humidity { get; set; }
        double temp { get; set; }
        double target_temp { get; set; }
    }
}

