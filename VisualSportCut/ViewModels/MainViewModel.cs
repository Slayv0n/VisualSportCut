using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using VisualSportCut.Domain.Interfaces;
using VisualSportCut.Domain.Models;


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
        private ObservableCollection<string> _availablePeriods = new();

        [ObservableProperty]
        private ObservableCollection<string> _availableLabels = new();

        [ObservableProperty]
        private ObservableCollection<string> _groupByTypes = new() { "По тегам", "По периодам", "По лейблам" };

        [ObservableProperty]
        private string _selectedGroupByType = string.Empty;

        [ObservableProperty]
        private string _selectedTagGroup = string.Empty;

        [ObservableProperty]
        private string _selectedPeriod = string.Empty;

        [ObservableProperty]
        private string _selectedLabel = string.Empty;

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

        [ObservableProperty]
        private SolidColorPaint _backgroundColor = new SolidColorPaint(new SKColor(31, 31, 31));

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

                AvailablePeriods.Clear();
                AvailablePeriods.Add("Все периоды");

                var periods = new ObservableCollection<string>(
                    stamps.SelectMany(s => s.TimeEvents)
                          .Select(p => p.Name!)
                          .Distinct()
                );

                foreach (var period in periods)
                {
                    AvailablePeriods.Add(period);
                }

                AvailableLabels.Clear();
                AvailableLabels.Add("Все лейблы");

                var labels = new ObservableCollection<string>(
                    stamps.SelectMany(s => s.LabelEvents)
                          .Select(le => $"{le.Group} - {le.Name}")
                          .Distinct()
                );

                foreach (var label in labels)
                {
                    AvailableLabels.Add(label);
                }

                IsDataLoaded = true;
                SelectedGroupByType = "По тегам";
                SelectedTagGroup = "Все теги";
                SelectedPeriod = "Все периоды";
                SelectedLabel = "Все лейблы";
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

            IEnumerable<StatItem> stats = Array.Empty<StatItem>();

            if (SelectedGroupByType == "По тегам")
            {
                stats = _statsService.GetStatsByGroup(SelectedTagGroup == "Все теги" ? "" : SelectedTagGroup);
            }
            else if (SelectedGroupByType == "По периодам")
            {
                stats = _statsService.GetStatsByPeriod(SelectedPeriod == "Все периоды" ? "" : SelectedPeriod);
            }
            else if (SelectedGroupByType == "По лейблам")
            {
                var labelGroup = SelectedLabel == "Все лейблы" ? "" : SelectedLabel?.Split(' ').FirstOrDefault();
                var labelName = SelectedLabel == "Все лейблы" ? "" : SelectedLabel?.Substring(SelectedLabel.LastIndexOf(' ') + 1);
                stats = _statsService.GetStatsByLabel(labelGroup ?? "", labelName ?? "");
            }

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
                    ToolTipLabelFormatter = point =>
                    {
                        var total = pieData.Sum(d => d.Total);
                        var percent = (double)point.Model / total;
                        return $"{d.Total} - {percent.ToString("P2")}";
                    },
                    MaxRadialColumnWidth = 60,
                    InnerRadius = 30,
                    Pushout = 4,
                    HoverPushout = 12

                }
            ).ToArray();

            СartesianSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = Statistics.Select(s => s.Count).ToArray(),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    Fill = new SolidColorPaint(SKColors.Red),
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
                    LabelsPaint = new SolidColorPaint(SKColors.Transparent),
                    LabelsBackground = LvcColor.Empty,
                    SeparatorsPaint = new SolidColorPaint(new SKColor(90, 90, 90))
                }
            };

            AngleAxes = new[]
            {
                new PolarAxis
                {
                    Labels = Statistics.Select(s => s.Category).ToArray(),
                    TextSize = 14,
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    LabelsBackground = LvcColor.Empty,
                    SeparatorsPaint = new SolidColorPaint(new SKColor(90, 90, 90)),
                }
            };
        }

        private bool CanRefreshStats() => IsDataLoaded;

        partial void OnSelectedTagGroupChanged(string value)
        {
            if (IsDataLoaded)
                UpdateStatistics();
        }
        partial void OnSelectedPeriodChanged(string value)
        {
            if (IsDataLoaded)
                UpdateStatistics();
        }
        partial void OnSelectedLabelChanged(string value)
        {
            if (IsDataLoaded)
                UpdateStatistics();
        }
        partial void OnSelectedGroupByTypeChanged(string value)
        {
            SelectedTagGroup = "Все теги";
            SelectedPeriod = "Все периоды";
            SelectedLabel = "Все лейблы";
            if (IsDataLoaded)
                UpdateStatistics();

        }
    }
}
