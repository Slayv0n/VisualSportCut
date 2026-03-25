using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
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
        public ISeries[] Series { get; set; }
    = new ISeries[]
    {
                new LineSeries<int>
                {
                    Values = new int[] { 4, 6, 5, 3, -3, -1, 2 }
                },
                new ColumnSeries<double>
                {
                    Values = new double[] { 2, 5, 4, -2, 4, -3, 5 }
                }
    };
    
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
        private string _statusMessage = "Загрузите JSON файл для начала работы";

        [ObservableProperty]
        private PieSeries<int>[] _pieSeries = Array.Empty<PieSeries<int>>();

        [ObservableProperty]
        private ISeries[] _chartSeries = Array.Empty<ISeries>();

        [ObservableProperty]
        private Axis[] _xAxes = Array.Empty<Axis>();

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
                StatusMessage = "Выберите файл...";
                var dialog = new Microsoft.Win32.OpenFileDialog { Filter = "JSON files|*.json" };

                if (dialog.ShowDialog() == false)
                {
                    StatusMessage = "Загрузка отменена";
                    return;
                }

                StatusMessage = "Загрузка файла...";
                var stamps = await _jsonLoader.LoadAsync(dialog.FileName);
                _statsService.SetStamps(stamps);

                AvailableGroups.Clear();
                AvailableGroups.Add("Все теги");

                foreach (var group in stamps
                    .Select(s => s.Tag?.Group)
                    .Distinct()
                    .Where(t => !string.IsNullOrEmpty(t)))
                {
                    AvailableGroups.Add(group);
                }

                IsDataLoaded = true;
                SelectedTag = "Все теги";
                StatusMessage = $"Файл загружен";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
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

                    MaxRadialColumnWidth = 120,
                    Pushout = 2
                }
            ).ToArray();

            // Обновляем график
            ChartSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Name = "Статистика",
                    Values = Statistics.Select(s => s.Count).ToArray(),
                    Fill = new SolidColorPaint(new SKColor(255, 0, 0)),
                    MaxBarWidth = 60
                }
            };

            XAxes = new[]
            {
                new Axis
                {
                    Labels = Statistics.Select(s => s.Category).ToArray(),
                    MinStep = 0
                }
            };

            StatusMessage = $"Показано статистики: {Statistics.Count}";
        }

        private bool CanRefreshStats() => IsDataLoaded;

        partial void OnSelectedTagChanged(string value)
        {
            if (IsDataLoaded)
                UpdateStatistics();
        }
    }
}
