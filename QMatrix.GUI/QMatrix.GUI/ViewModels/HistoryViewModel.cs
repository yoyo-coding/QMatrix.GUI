using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QMatrix.GUI.Models;
using QMatrix.GUI.Services;

namespace QMatrix.GUI.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    private readonly IQMatrixApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<QMHistoryItem> _historyItems = new();

    [ObservableProperty]
    private QMHistoryItem? _selectedItem;

    [ObservableProperty]
    private bool _isLoading;

    public event EventHandler<QMHistoryItem>? ItemSelected;

    public HistoryViewModel(IQMatrixApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoadHistoryAsync()
    {
        IsLoading = true;
        try
        {
            var items = await _apiService.GetHistoryAsync();
            HistoryItems.Clear();
            foreach (var item in items)
            {
                HistoryItems.Add(item);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedItemChanged(QMHistoryItem? value)
    {
        if (value != null)
        {
            ItemSelected?.Invoke(this, value);
        }
    }

    [RelayCommand]
    private void SelectItem(QMHistoryItem item)
    {
        SelectedItem = item;
    }
}
