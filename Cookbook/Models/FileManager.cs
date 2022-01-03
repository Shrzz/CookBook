namespace Cookbook.Models
{
    public class FileManager
    {
        private const string _relativePath = $"/uploads/images/recipes/";
        private readonly string _webRootPath;

        public FileManager(string webRootPath)
        {
            _webRootPath = webRootPath;
        }

        // create new directory in file system and return relative path to it
        public string UploadFiles(string directoryName, FileManagerModel fileManagerModel)
        {
            EnsureDirectoryCreated(directoryName);

            if (fileManagerModel is null)
            {
                return directoryName;
            }

            if (fileManagerModel.IFormFiles is null)
            {
                return directoryName;
            }

            foreach (var item in fileManagerModel.IFormFiles)
            {
                var fileName = RemoveWhiteSpacesFromString(item.FileName);
                var fullFilePath = $"{_webRootPath}/{directoryName}/{fileName}";
                if (File.Exists(fullFilePath))
                {
                    File.Delete(fullFilePath);
                }

                using (FileStream fs = new FileStream(fullFilePath, FileMode.Create))
                {
                    item.CopyTo(fs);
                }
            }

            return directoryName;
        }

        // get FileManagerModel by path, recorded in a corresponding database field
        public FileManagerModel GetFileManagerModel(string imagesDirectory)
        {
            if (String.IsNullOrWhiteSpace(imagesDirectory))
            {
                return new FileManagerModel();
            }

            EnsureDirectoryCreated(imagesDirectory);

            FileManagerModel model = new FileManagerModel();
            DirectoryInfo dir = new DirectoryInfo($"{_webRootPath}/{imagesDirectory}");
            FileInfo[] files = dir.GetFiles();
            model.Files = files;

            return model;
        }

        // remove directory by its relative path
        public bool RemoveDirectory(string directoryRelativePath)
        {
            string fullPath = $"{_webRootPath}/{directoryRelativePath}";

            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
            }

            return true;
        }

        // remove file by its absolute path
        public bool RemoveFile(string fileToRemove)
        {
            if (System.IO.File.Exists(fileToRemove))
            {
                System.IO.File.Delete(fileToRemove);
            }

            return true;
        }

        // remove whitespaces and add timestamp to a string
        public string GenerateDirectoryName(string directoryName)
        {
            var formattedDirectoryName = RemoveWhiteSpacesFromString(directoryName);
            return $"{DateTime.Now.Ticks}_{formattedDirectoryName}";
        }

        // checks relative path (/www/../directoryName/) and creates folder if doesn't exist
        private void EnsureDirectoryCreated(string directoryRelativePath)
        {
            if (!Directory.Exists($"{_webRootPath}/{directoryRelativePath}"))
            {
                Directory.CreateDirectory($"{_webRootPath}/{directoryRelativePath}");
            }
        }

        // returns directory relative path to store it in database
        public string GetDirectoryRelativePath(string directoryName)
        {
            return $"{_relativePath}/{directoryName}";
        }

        private string RemoveWhiteSpacesFromString(string name)
        {
            return name.Replace(' ', '_');
        }


    }
}
