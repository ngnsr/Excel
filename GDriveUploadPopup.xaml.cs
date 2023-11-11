using Mopups.Pages;
using Mopups.Services;


namespace MyExcel;

public partial class GDriveUploadPopup : PopupPage
{
    TaskCompletionSource<string> _taskCompletionSource;
    public Task<string> PopupDismissedTask => _taskCompletionSource.Task;

    public string ReturnValue { get; set; }

    public GDriveUploadPopup()
    {
        InitializeComponent();
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        ReturnValue = FileNameEntry.Text;
        await MopupService.Instance.PopAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _taskCompletionSource = new TaskCompletionSource<string>();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _taskCompletionSource.SetResult(ReturnValue);
    }

    public async void PopupPage_BackgroundClicked(System.Object sender, System.EventArgs e)
    {
        await MopupService.Instance.PopAsync();
    }
}
