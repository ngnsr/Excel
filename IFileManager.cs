using CommunityToolkit.Maui.Storage;
namespace MyExcel
{
	public interface IFileManager
	{
		Task<FileSaverResult> SaveToFileAsync(FileRepresentation file);
		Task<FileRepresentation> LoadFromFile();
	}
}

