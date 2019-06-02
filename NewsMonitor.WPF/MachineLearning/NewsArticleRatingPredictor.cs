using Microsoft.ML;
using Microsoft.ML.Data;
using NewsMonitor.Data.Models;
using NewsMonitor.WPF.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NewsMonitor.WPF.MachineLearning
{
    class NewsArticleRatingPredictor : RegressionPredictor
    {
        class NewsArticleRatingPrediction
        {
            [ColumnName("Score")]
            public float PredictedRating=-1;
        }

        class PredictableNewsArticle
        {
            NewsArticle _newsArticle;
            IDomainRater _domainRater;
            public PredictableNewsArticle(NewsArticle newsArticle, IDomainRater domainRater)
            {
                _newsArticle = newsArticle;
                _domainRater = domainRater;
            }

            public string Title => _newsArticle.Title;
            public string OrganizationName => _newsArticle.OrganizationName;

            bool _monthlyVisitorsInitialized = false;
            long _monthlyVisitors;
            object _visitorsLock = new object();
            public float MonthlyVisitors
            {
                get
                {
                    lock(_visitorsLock)
                    {
                        if (!_monthlyVisitorsInitialized)
                        {
                            Uri uri = new Uri(_newsArticle.Url);
                            string domain = String.Join(".", TakeLast(uri.Host.Split('.'),
                                IsThreePartDomain(uri.Host) ? 3 : 2));
                            _monthlyVisitors = _domainRater.GetMonthlyVisitors(domain);
                            System.Diagnostics.Debug.WriteLine($"Domain {domain} {_monthlyVisitors}");
                            _monthlyVisitorsInitialized = true;
                        }
                        return _monthlyVisitors == 0 ? 150000 : _monthlyVisitors;
                    }
                }
            }
            public float Rating => _newsArticle.Rating;

            static IEnumerable<T> TakeLast<T>(IEnumerable<T> source, int N)
            {
                return source.Skip(Math.Max(0, source.Count() - N));
            }

            bool IsThreePartDomain(string domain)
            {
                Regex regex = new Regex(@"\bcom?.\w+$");
                return regex.IsMatch(domain);
            }
        }

        PredictionEngine<PredictableNewsArticle, NewsArticleRatingPrediction> _predictionEngine;

        public int LearningDataCount { get; set; }

        protected override IEnumerable<IEstimator<ITransformer>> Pipelines => new List<IEstimator<ITransformer>>()
        {
            GetPoissonEstimator(),
            GetSdcaEstimator(),
            GetOnlineGradientDescentEstimator(),
            //GetDomainPoissonEstimator(),
            //GetDomainSdcaEstimator(),
            //GetDomainOnlineGradientDescentEstimator()
        };

        IDataView _allData;
        protected override IDataView AllData => _allData;

        List<PredictableNewsArticle> _learningData;
        IDomainRater _domainRater;

        public NewsArticleRatingPredictor(IEnumerable<NewsArticle> articles, IDomainRater domainRater)
        {
            _domainRater = domainRater;
            _learningData = articles.Where(a => a.UserSetRating && a.Rating > 0)
                .Select(a => new PredictableNewsArticle(a, domainRater)).ToList();
            LearningDataCount = _learningData.Count;

            _allData = MlContext.Data.LoadFromEnumerable(_learningData);

            _predictionEngine = MlContext.Model
                .CreatePredictionEngine<PredictableNewsArticle, NewsArticleRatingPrediction>(Model);
        }

        public int Predict(NewsArticle article)
        {
            var predictinoObj = _predictionEngine.Predict(
                new PredictableNewsArticle(article, _domainRater));
            float prediction = predictinoObj.PredictedRating;
            return Math.Max(1, Math.Min(5, (int)Math.Round(prediction)));
        }

        private IEstimator<ITransformer> GetDefaultEstimator(IEstimator<ITransformer> trainer)
        {
            return MlContext.Transforms
                .CopyColumns(outputColumnName: "Label", inputColumnName: "Rating")
                .Append(MlContext.Transforms.Text.FeaturizeText("FeaturizedTitle", "Title"))
                .Append(MlContext.Transforms.Text.FeaturizeText("FeaturizedOrganizationName", "OrganizationName"))
                .Append(MlContext.Transforms.Concatenate("Features", "FeaturizedTitle", "FeaturizedOrganizationName"))
                .Append(trainer);
        }

        private IEstimator<ITransformer> GetPoissonEstimator()
        {
            return GetDefaultEstimator(MlContext.Regression.Trainers.LbfgsPoissonRegression());
        }

        private IEstimator<ITransformer> GetSdcaEstimator()
        {
            return GetDefaultEstimator(MlContext.Regression.Trainers.Sdca());
        }

        private IEstimator<ITransformer> GetOnlineGradientDescentEstimator()
        {
            return GetDefaultEstimator(MlContext.Regression.Trainers.OnlineGradientDescent());
        }

        private IEstimator<ITransformer> GetDomainEstimator(IEstimator<ITransformer> trainer)
        {
            return MlContext.Transforms
                .CopyColumns(outputColumnName: "Label", inputColumnName: "Rating")
                .Append(MlContext.Transforms.Text.FeaturizeText("FeaturizedTitle", "Title"))
                .Append(MlContext.Transforms.Text.FeaturizeText("FeaturizedOrganizationName", "OrganizationName"))
                .Append(MlContext.Transforms.NormalizeMeanVariance("FeaturizedMonthlyVisitors", "MonthlyVisitors"))
                .Append(MlContext.Transforms.Concatenate("Features", "FeaturizedTitle", 
                        "FeaturizedOrganizationName", "FeaturizedMonthlyVisitors"))
                .Append(trainer);
        }

        private IEstimator<ITransformer> GetDomainPoissonEstimator()
        {
            return GetDomainEstimator(MlContext.Regression.Trainers.LbfgsPoissonRegression());
        }

        private IEstimator<ITransformer> GetDomainSdcaEstimator()
        {
            return GetDomainEstimator(MlContext.Regression.Trainers.Sdca());
        }

        private IEstimator<ITransformer> GetDomainOnlineGradientDescentEstimator()
        {
            return GetDomainEstimator(MlContext.Regression.Trainers.OnlineGradientDescent());
        }
    }

}
