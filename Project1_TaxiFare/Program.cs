using System;
using System.IO;
using Microsoft.ML;

namespace Project1_TaxiFare
{
    class Program
    {
        static readonly string _trainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "taxi-fare-train.csv");
        static readonly string _testDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "taxi-fare-test.csv");
        static readonly string _modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "Model.zip");
        static void Main(string[] args)
        {
            MLContext mlContext = new MLContext(seed: 0);
            //call the Train method
            var model = Train(mlContext, _trainDataPath);
            //call the Evaluate method
            Evaluate(mlContext, model);
            // Call the TestSinglePrediction method
            TestSinglePrediction(mlContext, model);
        }

        // Load, extracts and transform the data. Train and returns the model
        public static ITransformer Train(MLContext mlContext, string dataPath)
        {
            //Load from CSV file
            IDataView dataView = mlContext.Data.LoadFromTextFile<TaxiTrip>(dataPath, hasHeader: true, separatorChar: ',');
            //Copy the FareAmount to predict the taxi trip
            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "FareAmount")
            // Transform the categorical data(VendorId, RateCode and Payment type) values into numbers
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "VendorIdEncoded", inputColumnName: "VendorId"))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "RateCodeEncoded", inputColumnName: "RateCode"))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PaymentTypeEncoded", inputColumnName: "PaymentType"))
            //combine all the faeture columns into Feature column
            .Append(mlContext.Transforms.Concatenate("Features", "VendorIdEncoded", "RateCodeEncoded", "PassengerCount", "TripDistance", "PaymentTypeEncoded"))
            // Choose Regression learning algorithm
            .Append(mlContext.Regression.Trainers.FastTree());

            // Fit the model to the training dataview and return the trained model
            var model = pipeline.Fit(dataView);
            return model;
        }

        // Load the test dataset. Creates regression evaluator
        //Evaluates the model and create metrics. Display the metrics
        private static void Evaluate(MLContext mlContext, ITransformer model)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<TaxiTrip>(_testDataPath, hasHeader: true, separatorChar: ',');
            var predictions = model.Transform(dataView);
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics evaluation         ");
            Console.WriteLine($"*------------------------------------------------");
            //The closer the value is to 1, the better model is:
            Console.WriteLine($"*       RSquared Score:      {metrics.RSquared:0.##}");
            // the lower it is, the better the model is:
            Console.WriteLine($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");
        }

        //Creates a single comment of test data. Predicts fare amounts
        //Combines test data and predictions for reporting. Display predicted results
        private static void TestSinglePrediction(MLContext mlContext, ITransformer model)
        {
            var predictionFunction = mlContext.Model.CreatePredictionEngine<TaxiTrip, TaxiTripFarePrediction>(model);

            var taxiTripSample = new TaxiTrip()
            {
                VendorId = "VTS",
                RateCode = "1",
                PassengerCount = 1,
                TripTime = 1140,
                TripDistance = 3.75f,
                PaymentType = "CRD",
                FareAmount = 0 // To predict. Actual/Observed = 15.5
            };

            var prediction = predictionFunction.Predict(taxiTripSample);
            Console.WriteLine($"**********************************************************************");
            Console.WriteLine($"Predicted fare: {prediction.FareAmount:0.####}, actual fare: 15.5");
            Console.WriteLine($"**********************************************************************");
        }

    }
}
