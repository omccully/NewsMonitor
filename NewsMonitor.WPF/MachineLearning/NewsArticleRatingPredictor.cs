using Microsoft.ML;
using Microsoft.ML.Data;
using NewsMonitor.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.MachineLearning
{
    class NewsArticleRatingPredictor
    {
        class NewsArticleRatingPrediction
        {
            [ColumnName("Score")]
            public float PredictedRating=-1;
        }

        PredictionEngine<NewsArticle, NewsArticleRatingPrediction> _predictionEngine;

        public int LearningDataCount { get; set; }

        public NewsArticleRatingPredictor(IEnumerable<NewsArticle> articles)
        {
            MLContext mlContext = new MLContext(seed: 0);

            List<NewsArticle> learningData = articles.Where(a => a.UserSetRating && a.Rating > 0).ToList();
            LearningDataCount = learningData.Count;

            IDataView dataView = mlContext.Data.LoadFromEnumerable<NewsArticle>(learningData);

            var pipeline = mlContext.Transforms
                .CopyColumns(outputColumnName: "Label", inputColumnName: "FloatRating")
                .Append(mlContext.Transforms.Text.FeaturizeText("FeaturizedTitle", "Title"))
                .Append(mlContext.Transforms.Text.FeaturizeText("FeaturizedOrganizationName", "OrganizationName"))
                .Append(mlContext.Transforms.Concatenate("Features", "FeaturizedTitle", "FeaturizedOrganizationName"))
                .Append(mlContext.Regression.Trainers.LbfgsPoissonRegression());

            var model = pipeline.Fit(dataView);

            _predictionEngine = mlContext.Model
                .CreatePredictionEngine<NewsArticle, NewsArticleRatingPrediction>(model);
        }

        public int Predict(NewsArticle article)
        {
            var predictinoObj = _predictionEngine.Predict(article);
            float prediction = predictinoObj.PredictedRating;
            return Math.Max(1, Math.Min(5, (int)Math.Round(prediction)));
        }
    }
}
