using System;
using ExplainCoreLib.functions;
using ExplainCoreLib.base_models;
using ExplainCoreLib.core_models;
using ExplainCoreLib.Interfaces;

namespace ExplainCoreLib.functions
{
	public static class BloodComposition
	{
		public static void SetBloodComposition(BloodTimeVaryingElastance bc)
		{
			AcidBaseResult acidBaseResult = Acidbase.CalcAcidBaseFromTco2(bc);
            if (acidBaseResult.valid)
            {
                bc.aboxy["ph"] = acidBaseResult.ph;
                bc.aboxy["pco2"] = acidBaseResult.pco2;
                bc.aboxy["hco3"] = acidBaseResult.hco3;
                bc.aboxy["be"] = acidBaseResult.be;
                bc.aboxy["sid_app"] = acidBaseResult.sid_app;
            }

            OxyResult oxyResult = Oxygenation.CalcOxygenationFromTo2(bc);
            if (oxyResult.valid)
            {
                bc.aboxy["po2"] = oxyResult.po2;
                bc.aboxy["so2"] = oxyResult.so2;
            }
        }

        public static void SetBloodComposition(BloodCapacitance bc)
        {
            AcidBaseResult acidBaseResult = Acidbase.CalcAcidBaseFromTco2(bc);
            if (acidBaseResult.valid)
            {
                bc.aboxy["ph"] = acidBaseResult.ph;
                bc.aboxy["pco2"] = acidBaseResult.pco2;
                bc.aboxy["hco3"] = acidBaseResult.hco3;
                bc.aboxy["be"] = acidBaseResult.be;
                bc.aboxy["sid_app"] = acidBaseResult.sid_app;
            }

            OxyResult oxyResult = Oxygenation.CalcOxygenationFromTo2(bc);
            if (oxyResult.valid)
            {
                bc.aboxy["po2"] = oxyResult.po2;
                bc.aboxy["so2"] = oxyResult.so2;
            }
        }
    }
}


