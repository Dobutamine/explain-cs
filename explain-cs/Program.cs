using System.IO;

// load the model definition file
string modelDefinition = File.ReadAllText("normal_neonate.json");

// instantiate a model engine with the model definition file
ExplainCoreLib.ModelEngine engine = new(modelDefinition);

// calculate 10 seconds
engine.Calculate(10);