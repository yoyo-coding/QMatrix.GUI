using System;
using System.Threading.Tasks;
using System.Windows;
using QMatrix.GUI.Services;

namespace QMatrix.GUI.Views;

public partial class AdapterWindow : Window
{
    private readonly QMatrixAdapterService _adapterService;
    private AdapterInfo _adapterInfo;
    private bool _isAdapting;

    public bool IsConfirmed { get; private set; }

    public AdapterWindow()
    {
        InitializeComponent();
        _adapterService = new QMatrixAdapterService();
        IsConfirmed = false;
        _isAdapting = false;
    }

    public async Task InitializeAsync()
    {
        _adapterInfo = await _adapterService.GetAdapterInfoAsync();
        if (_adapterInfo == null)
        {
            MessageBox.Show("未找到 QMatrix 核心程序", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            DialogResult = false;
            Close();
        }
    }

    private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isAdapting)
            return;

        _isAdapting = true;
        ConfirmButton.IsEnabled = false;
        CancelButton.IsEnabled = false;

        try
        {
            var progress = new Progress<int>(value =>
            {
                ProgressBar.Value = value;
                ProgressText.Text = value switch
                {
                    0 => "准备就绪",
                    10 => "检查 HTTP 服务",
                    30 => "复制配置文件",
                    50 => "启动 Core 服务",
                    70 => "启动 HTTP 服务",
                    90 => "更新 GUI 配置",
                    100 => "适配完成",
                    _ => $"适配中... {value}%"
                };
            });

            var success = await _adapterService.PerformAdapterAsync(_adapterInfo, progress);
            if (success)
            {
                MessageBox.Show("QMatrix 适配成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                IsConfirmed = true;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("QMatrix 适配失败，请检查错误信息", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                IsConfirmed = false;
                DialogResult = false;
                Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"适配过程中发生错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            IsConfirmed = false;
            DialogResult = false;
            Close();
        }
        finally
        {
            _isAdapting = false;
            ConfirmButton.IsEnabled = true;
            CancelButton.IsEnabled = true;
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isAdapting)
            return;

        IsConfirmed = false;
        DialogResult = false;
        Close();
    }
}
