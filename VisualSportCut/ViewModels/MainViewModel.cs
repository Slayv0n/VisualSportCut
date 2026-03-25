using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using VisualSportCut.Domain.Interfaces;
using VisualSportCut.Domain.Models;
using VisualSportCut.Domain.Services;

namespace VisualSportCut.Presentation.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {


        private readonly IJsonLoader _jsonLoader;
        private readonly IStatisticService _statsService;

        [ObservableProperty]
        private ObservableCollection<StatItem> _statistics = new();

        [ObservableProperty]
        private ObservableCollection<string> _availableGroups = new();

        [ObservableProperty]
        private string _selectedTag = string.Empty;

        [ObservableProperty]
        private bool _isDataLoaded = false;

        [ObservableProperty]
        private PieSeries<int>[] _pieSeries = Array.Empty<PieSeries<int>>();

        [ObservableProperty]
        private ISeries[] _сartesianSeries = Array.Empty<ISeries>();

        [ObservableProperty]
        private ISeries[] _polarLineSeries = Array.Empty<ISeries>();

        [ObservableProperty]
        private Axis[] _xAxes = Array.Empty<Axis>();

        [ObservableProperty]
        private Axis[] _yAxes = Array.Empty<Axis>();

        [ObservableProperty] 
        private PolarAxis[] _radialAxes = Array.Empty<PolarAxis>();

        [ObservableProperty]
        private PolarAxis[] _angleAxes = Array.Empty<PolarAxis>();

        [ObservableProperty]
        private SolidColorPaint _whiteColor = new SolidColorPaint(SKColors.White);

        public MainViewModel(IJsonLoader jsonLoader,
                             IStatisticService statsService)
        {
            _jsonLoader = jsonLoader;
            _statsService = statsService;
        }

        [RelayCommand]
        private async Task LoadFile()
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog { Filter = "JSON files|*.json" };

                if (dialog.ShowDialog() == false)
                {
                    return;
                }

                var stamps = await _jsonLoader.LoadAsync(dialog.FileName);
                _statsService.SetStamps(stamps);

                AvailableGroups.Clear();
                AvailableGroups.Add("Все теги");

                foreach (var group in stamps
                    .Select(s => s.Tag?.Group)
                    .Distinct()
                    .Where(t => !string.IsNullOrEmpty(t)))
                {
                    AvailableGroups.Add(group!);
                }

                IsDataLoaded = true;
                SelectedTag = "Все теги";
            }
            catch (Exception)
            {
                IsDataLoaded = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanRefreshStats))]
        private void UpdateStatistics()
        {
            Statistics.Clear();
            var stats = _statsService.GetStatsByGroup(SelectedTag == "Все теги" ? "" : SelectedTag);

            foreach (var stat in stats)
                Statistics.Add(stat);

            var pieData = Statistics.GroupBy(s => s.Category)
            .Select(g => new { Name = g.Key, Total = g.Sum(x => x.Count), Color = g.First().Color })
            .ToList();

            PieSeries = pieData.Select(d =>
                new PieSeries<int>
                {
                    Name = d.Name,
                    Values = new[] { d.Total },
                    Fill = new SolidColorPaint(new SKColor(byte.Parse(d.Color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(d.Color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(d.Color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber))),
                    Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 0.25f},
                    MaxRadialColumnWidth = 60,

                    InnerRadius = 30

                }
            ).ToArray();

            СartesianSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = Statistics.Select(s => s.Count).ToArray(),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    Fill = new SolidColorPaint(SKColors.Red),
                    Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 0.25f},
                    MaxBarWidth = 60
                }
            };

            YAxes = new[]
            {
                new Axis
                {
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                }
            };

            XAxes = new[]
            {
                new Axis
                {
                    Labels = Statistics.Select(s => s.Category).ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    MinStep = 0
                }
            };

            PolarLineSeries = new ISeries[]
            {
                new PolarLineSeries<int>
                {
                    Values = Statistics.Select(s => s.Count).ToArray(),
                    DataLabelsPaint = new SolidColorPaint(SKColors.Red),
                    GeometryFill = new SolidColorPaint(SKColors.Red),
                    GeometryStroke = new SolidColorPaint(SKColors.Black),
                    Stroke = new SolidColorPaint(SKColors.Red),
                    Fill = new SolidColorPaint(new SKColor(255, 0, 0, 100)),
                    GeometrySize = 10,
                    DataLabelsSize = 0,
                    DataLabelsPosition = PolarLabelsPosition.Middle,
                    DataLabelsRotation = LiveCharts.CotangentAngle,
                    IsClosed = true
                }
            };

            RadialAxes = new[]
            {
                new PolarAxis
                {
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    LabelsBackground = LvcColor.Empty,
                    SeparatorsPaint = new SolidColorPaint(new SKColor(90, 90, 90))
                }
            };

            AngleAxes = new[]
            {
                new PolarAxis
                {
                    Labels = Statistics.Select(s => s.Category).ToArray(),
                    TextSize = 10,
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    LabelsBackground = LvcColor.Empty,
                    SeparatorsPaint = new SolidColorPaint(new SKColor(90, 90, 90)),
                    ForceStepToMin = true
                }
            };
        }

        private bool CanRefreshStats() => IsDataLoaded;

        partial void OnSelectedTagChanged(string value)
        {
            if (IsDataLoaded)
                UpdateStatistics();
        }
    }
}
