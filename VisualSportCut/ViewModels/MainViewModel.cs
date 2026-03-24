using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using VisualSportCut.Domain.Interfaces;
using VisualSportCut.Domain.Models;
using VisualSportCut.Domain.Services;

namespace VisualSportCut.Presentation.ViewModels
{
    partial class MainViewModel : ObservableObject
    {
        private readonly IJsonLoader _jsonLoader;
        private readonly IStatisticService _statsService;

        [ObservableProperty]
        private ObservableCollection<StatItem> _statistics = new();

        [ObservableProperty]
        private ObservableCollection<string> _availableTags = new();

        [ObservableProperty]
        private string _selectedTag = string.Empty;

        [ObservableProperty]
        private bool _isDataLoaded = false;

        [ObservableProperty]
        private string _statusMessage = "Загрузите JSON файл для начала работы";

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

                AvailableTags.Clear();
                AvailableTags.Add("Все теги");

                foreach (var tag in stamps
                    .Select(s => s.Tag?.Name)
                    .Distinct()
                    .Where(t => !string.IsNullOrEmpty(t)))
                {
                    AvailableTags.Add(tag);
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

            var stats =  _statsService.GetStatsByTag(SelectedTag);

            foreach (var stat in stats)
                Statistics.Add(stat);

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
