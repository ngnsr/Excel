namespace MyExcel
{
	public interface IFileManager
	{
		Task<string> SaveToFileAsync(FileRepresentation file, string fileName);
		Task<FileRepresentation> LoadFromFileAsync();
	}
}

