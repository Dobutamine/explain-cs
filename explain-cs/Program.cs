using System.IO;

// load the model definition file
string modelDefinition = File.ReadAllText("normal_neonate.json");

// instantiate a model engine with the model definition file
ExplainCoreLib.ModelEngine engine = new(modelDefinition);

engine.SwitchVentilator(false);

// calculate 10 seconds
engine.Calculate(60);

// start realtime model
engine.Start(1.0);



