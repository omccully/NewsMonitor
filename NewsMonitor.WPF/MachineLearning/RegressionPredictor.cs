using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.MachineLearning
{
    abstract class RegressionPredictor
    {
        protected abstract IEnumerable<IEstimator<ITransformer>> Pipelines { get; }

        protected MLContext MlContext;
        protected abstract IDataView AllData { get; }

        private ITransformer _model = null;
        protected ITransformer Model
        {
            get
            {
                if (_model == null)
                {
                    IEstimator<ITransformer> pipeline = GetBestEstimator();

                    _model = pipeline.Fit(AllData);
                }
                return _model;
            }
        }

        public RegressionPredictor()
        {
            MlContext = new MLContext();
        }

        private IEstimator<ITransformer> GetBestEstimator()
        {
            double highestRSquared = Double.MinValue;
            IEstimator<ITransformer> bestPipeline = null;
            foreach (var pipeline in Pipelines)
            {
                double rSquared = GetRSquared(pipeline);
                System.Diagnostics.Debug.WriteLine("RSquared = " + rSquared);
                if (rSquared > highestRSquared)
                {
                    highestRSquared = rSquared;
                    bestPipeline = pipeline;
                }
            }

            System.Diagnostics.Debug.WriteLine("Best RSquared = " + highestRSquared);

            return bestPipeline;
        }

        double GetRSquared(IEstimator<ITransformer> pipeline)
        {
            DataOperationsCatalog.TrainTestData dataSplit = MlContext.Data.TrainTestSplit(AllData, testFraction: 0.2);
            IDataView trainData = dataSplit.TrainSet;
            IDataView testData = dataSplit.TestSet;

            var model = pipeline.Fit(trainData);

            IDataView transformedData = model.Transform(testData);
            RegressionMetrics metrics = MlContext.Regression.Evaluate(transformedData);
            return metrics.RSquared;
        }
    }
}
