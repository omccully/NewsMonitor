using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewsMonitor.WPF.MachineLearning
{
    class MinimumPredictedRatingNewsFilter : INewsFilterExtension
    {
        public string Name => "Minimum predicted rating";

        NewsArticleRatingPredictor _predictor;

        const int DefaultMinimumRating = 2;
        const string DefaultMinimumRatingKey = "DefaultMinimumRating";

        public MinimumPredictedRatingNewsFilter(NewsArticleRatingPredictor predictor)
        {
            _predictor = predictor;
        }

        public bool AllowArticle(NewsArticle newsArticle, string searchTerm, KeyValueStorage storage)
        {
            if (_predictor.LearningDataCount < 50) return true;
            int predictedRating = _predictor.Predict(newsArticle);
            int mimimumRating = storage.GetInteger(DefaultMinimumRatingKey, DefaultMinimumRating);
            bool allow = predictedRating >= mimimumRating;
            if (!allow) System.Diagnostics.Debug.WriteLine($"\"{newsArticle.Title}\" predicted rated {predictedRating}, needs {predictedRating}");
            return allow;
        }

        public Window CreateQuickFilterWindow(NewsArticle newsArticle, KeyValueStorage storage)
        {
            return null;
        }

        public SettingsPage CreateSettingsPage()
        {
            return null;
        }
    }
}
