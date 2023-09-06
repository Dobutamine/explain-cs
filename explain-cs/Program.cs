using System.IO;

// load the model definition file
string modelDefinition = File.ReadAllText("normal_neonate.json");

// instantiate a model engine with the model definition file
ExplainCoreLib.ModelEngine engine = new(modelDefinition);


List<string> disabled_models = new List<string> {};
engine.Calibrate(60, "AA", "MOUTH", disabled_models);


