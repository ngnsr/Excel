// using CommunityToolkit.Maui.Views;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;

namespace MyExcel {
	class GDriveManager : IFileManager
	{
        private const string AppDataFolderName = "appDataFolder";
        private const string DefaultDataStoreFolderName = ".authdata";
        private const string ClientSecretsFileName = "client_secrets.json";
        private static DriveService driveService { get; set; }


        public GDriveManager(DriveService driveService)
		{
            if(driveService == null)
            {
                throw new ArgumentNullException(nameof(driveService));
            }

            GDriveManager.driveService = driveService;
		}

        public async static Task<GDriveManager> Create()
        {
            await InitClientAsync();
            return new GDriveManager(driveService);
        }        

        private static async Task InitClientAsync()
        {
            if (driveService is not null)
            {
                return;
            }
            Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(ClientSecretsFileName);

            var clientSecrets = (await GoogleClientSecrets
                .FromStreamAsync(fileStream)).Secrets;
            var dataStore = new FileDataStore(DefaultDataStoreFolderName);
            var scopes = new[] { DriveService.Scope.DriveAppdata };

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets
                        , scopes
                        , "user"
                        , CancellationToken.None
                        , dataStore);
            driveService = new DriveService(new DriveService .Initializer { HttpClientInitializer = credential });
        }

        public async Task<string> SaveToFileAsync(FileRepresentation file, string fileName)
		{
            if (driveService is null)
            {
                throw new InvalidOperationException("Drive service has not been initialized");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Incorect file name");
            }

            if (!fileName.EndsWith(".json")) fileName = fileName + ".json";

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Parents = new List<string>() { AppDataFolderName }
            };
            var content = JsonFileManager.ToJson(file);
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var request = driveService.Files.Create(fileMetadata, stream, "text/plain");
            request.Fields = "id";
            var uploadResult = await request.UploadAsync();
            if (uploadResult.Exception is not null)
            {
                return string.Empty;
            }

            return fileName;
		}

        public async Task<List<string>> ListFilesAsync()
        {
            if (driveService is null)
            {
                throw new InvalidOperationException("Drive service has not been initialized");
            }

            var request = driveService.Files.List();
            request.Spaces = AppDataFolderName;
            request.Fields = "files(id, name) ";

            var result = await request.ExecuteAsync();

            List<string> list = new List<string>(result.Files.Count);
            foreach(var file in result.Files)
            {
                list.Add(file.ToString());
            }
            return list;
        }


        public async Task<FileRepresentation> LoadFromFileAsync()
		{
            return new FileRepresentation();
		}

    }

}
