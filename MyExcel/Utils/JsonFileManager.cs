using System.Text;
using System.Text.Json;
using CommunityToolkit.Maui.Storage;
namespace MyExcel
{
    public class JsonFileManager : IFileManager
    {
        IFileSaver fileSaver;
        public JsonFileManager(IFileSaver fileSaver)
        {
            this.fileSaver = fileSaver;
        }

        public async Task<string> SaveToFileAsync(FileRepresentation file, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException();

            string homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                Environment.OSVersion.Platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");


            var jsonString = JsonSerializer.Serialize(file);
            CancellationToken cancellationToken = new CancellationTokenSource().Token;
            var fileSaverResult = await fileSaver.SaveAsync(homePath, fileName, new MemoryStream(Encoding.Default.GetBytes(jsonString)), cancellationToken);
            return fileSaverResult.FilePath;
        }

        public async Task<FileRepresentation> LoadFromFileAsync()
        {
            var jsonFileType =
                new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>{
                        { DevicePlatform.MacCatalyst, new[] {"json"}},
                        { DevicePlatform.WinUI, new[] {".json"}}
                        });

            var result = await FilePicker.PickAsync(new PickOptions{ FileTypes = jsonFileType });
            if(result == null) 
                throw new ArgumentException();

            using var stream = await result.OpenReadAsync();
            var fileRepresentation = JsonSerializer.Deserialize<FileRepresentation>(stream);
            return fileRepresentation;
        }

        public static string ToJson(FileRepresentation file)
        {
            return JsonSerializer.Serialize(file);
        }
    }
}

