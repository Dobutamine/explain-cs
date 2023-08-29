namespace ExplainCoreLib;

using System;
using System.Threading;
using System.Diagnostics;
using ExplainCoreLib.base_models;
using ExplainCoreLib.core_models;
using ExplainCoreLib.helpers;
using ExplainCoreLib.functions;
using Newtonsoft.Json;

public class ModelEngine
{
    public Dictionary<string, BaseModel> models { get; set; } = new();

    public string name { get; set; } = "";
    public string description { get; set; } = "";
    public double weight { get; set; } = 3.3;
    public double modeling_stepsize { get; set; } = 0.0005;
    public double model_time_total { get; set; } = 0.0;

    public double rtInterval { get; set; } = 0.015;
    public DataCollector? dataCollector;
    public TaskScheduler? taskScheduler;

    private Timer _rtTimer;

    // Create a Stopwatch instance
    readonly Stopwatch stopwatch = new();

    public ModelEngine(string _modelDefinition)
    {
        // the modeldefinition file containes all the model properties and is used by all current
        // versions of explain (c++, python, javascript and c#). Due to it's structure it needs processing.
        bool res_processing = ProcessModelDefinition(_modelDefinition);
        if (!res_processing)
        {
            Console.WriteLine("Error processing the model definition file!");
        }

        // all the submodels need to be initialized which can't be done in the constructor
        bool res_initialization = InitSubModels();
        if (!res_initialization)
        {
            Console.WriteLine("Error initializing the submodels!");
        }

        if (res_processing && res_initialization)
        {
            // define a data collector and task scheduler
            dataCollector = new(models, modeling_stepsize);
            taskScheduler = new(models, modeling_stepsize);
            Console.WriteLine("Instantiated a new model engine: {0}", description);
        }
    }

    private bool ProcessModelDefinition(string _modelDefinition)
    {
        try
        {
            // deserialize the model definition to a JSON object for further processing
            var jsonModel = JsonConvert.DeserializeObject<dynamic>(_modelDefinition);

            // set the general model properties
            name = jsonModel["name"].ToString();
            description = jsonModel["description"].ToString();
            weight = (double)jsonModel["weight"];
            modeling_stepsize = (double)jsonModel["modeling_stepsize"];
            model_time_total = (double)jsonModel["model_time_total"];

            // process all submodels
            foreach (var model in jsonModel["models"])
            {
                switch (model.Value.model_type.ToString())
                {
                    case "BloodTimeVaryingElastance":
                        BloodTimeVaryingElastance newBtve = model.Value.ToObject<BloodTimeVaryingElastance>();
                        models.Add(newBtve.name, newBtve);
                        break;
                    case "BloodCapacitance":
                        BloodCapacitance newBc = model.Value.ToObject<BloodCapacitance>();
                        models.Add(newBc.name, newBc);
                        break;
                    case "BloodResistor":
                        BloodResistor newRes = model.Value.ToObject<BloodResistor>();
                        models.Add(newRes.name, newRes);
                        break;
                    case "Shunt":
                        Shunt shunt = model.Value.ToObject<Shunt>();
                        models.Add(shunt.name, shunt);
                        break;
                    case "BloodValve":
                        BloodResistor newValve = model.Value.ToObject<BloodResistor>();
                        models.Add(newValve.name, newValve);
                        break;
                    case "GasCapacitance":
                        GasCapacitance newGc = model.Value.ToObject<GasCapacitance>();
                        models.Add(newGc.name, newGc);
                        break;
                    case "GasResistor":
                        GasResistor newGasRes = model.Value.ToObject<GasResistor>();
                        models.Add(newGasRes.name, newGasRes);
                        break;
                    case "Heart":
                        Heart heart = model.Value.ToObject<Heart>();
                        models.Add(heart.name, heart);
                        break;
                    case "Container":
                        Container container = model.Value.ToObject<Container>();
                        models.Add(container.name, container);
                        break;
                    case "GasExchanger":
                        GasExchanger gasExchanger = model.Value.ToObject<GasExchanger>();
                        models.Add(gasExchanger.name, gasExchanger);
                        break;
                    case "Blood":
                        Blood blood = model.Value.ToObject<Blood>();
                        models.Add(blood.name, blood);
                        break;
                    case "Gas":
                        Gas gas = model.Value.ToObject<Gas>();
                        models.Add(gas.name, gas);
                        break;
                    case "Breathing":
                        Breathing breathing = model.Value.ToObject<Breathing>();
                        models.Add(breathing.name, breathing);
                        break;
                    case "Metabolism":
                        Metabolism metabolism = model.Value.ToObject<Metabolism>();
                        models.Add(metabolism.name, metabolism);
                        break;
                    case "Ans":
                        Ans ans = model.Value.ToObject<Ans>();
                        models.Add(ans.name, ans);
                        break;
                    case "Ventilator":
                        Ventilator ventilator = model.Value.ToObject<Ventilator>();
                        models.Add(ventilator.name, ventilator);
                        break;
                }
            }
        } catch
        {
            return false;
        }
 
        // return true if no errors were encountered
        return true;
    }
   
    private bool InitSubModels()
    {
        //try
        //{
            // initialize all models now the model list as has been constructed
            foreach (var submodel in models)
            {
                // pass a reference to the models dictionary and the current stepsize to the submodels
                submodel.Value.InitModel(models, modeling_stepsize);
            }

            // initialize the dependent models. These models add model components to the list
            ((Ventilator)models["Ventilator"]).BuildVentilator();
        //}
        //catch
        //{
        //    return false;
        //}
        return true;
    }

    public void Calculate(double timeToCalculate = 10.0)
    {
        // calculate the number of steps needed
        int noSteps = (int)(timeToCalculate / modeling_stepsize);

        // Start the stopwatch before the step you want to measure
        stopwatch.Start();

        for (int i = 0; i < noSteps; i++)
        {
            // calculate all the submodels
            foreach (var submodel in models)
            {
                submodel.Value.StepModel();
            }

            // update the datacollector
            dataCollector.CollectData(model_time_total);

            // update the taskscheduler
            taskScheduler.RunTasks(model_time_total);

            // update the model total model time
            model_time_total += modeling_stepsize;
        }

        // Stop the stopwatch after the step
        stopwatch.Stop();

        // Get the elapsed time in various formats
        double elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        // Print the elapsed time
        Console.WriteLine($"Calculating model run of {timeToCalculate} sec. in {noSteps} steps.");
        Console.WriteLine($"Ready in {elapsedMilliseconds / 1000.0} sec. Average model step in {elapsedMilliseconds / (double) noSteps } ms.");

    }

    public void Start(double _interval = 0.015)
    {
        // Create a Timer instance that calls the DoSomething function every 1000 milliseconds (1 second).
        rtInterval = _interval;
        _rtTimer = new Timer(ModelStepRt, null, 0, (int)(rtInterval * 1000));

        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine(); // This will keep the console application running
    }
    public void Stop()
    {
        _rtTimer.Dispose();
    }

    public void ModelStepRt(object state)
    {
   
        // calculate the number of steps needed
        int noSteps = (int)(rtInterval / modeling_stepsize);

        // calculate all models
        for (int i = 0; i < noSteps; i++)
        {
            // calculate all the submodels
            foreach (var submodel in models)
            {
                submodel.Value.StepModel();
            }

            // update the datacollector
            dataCollector.CollectData(model_time_total);

            // update the taskscheduler
            taskScheduler.RunTasks(model_time_total);

            // update the model total model time
            model_time_total += modeling_stepsize;
        }

        double pco2 = ((BloodCapacitance)models["AA"]).aboxy["pco2"];
        double po2 = ((BloodCapacitance)models["AA"]).aboxy["po2"];
        Console.WriteLine("po2: {0}, pco2: {1}", po2, pco2);

    }

    public void SwitchVentilator(bool state)
    {
        ((Ventilator)models["Ventilator"]).SwitchVentilator(state);

    }
}

