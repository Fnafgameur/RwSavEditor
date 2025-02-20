using RwSavEditor.utils;
using RwSavEditor.utils.logs.logtypes;

namespace RwSavEditor.managers;

public class SaveManager
{
    private string backupFolderPath = "backups";
    private string saveFilePath;
    private string saveContent;
    
    public SaveManager(string saveFilePath)
    {
        string absPath = Path.GetFullPath(saveFilePath);
        
        LogWriter.WriteLog($"Checking for file at {absPath}...", new Info());
        if (!File.Exists(saveFilePath))
        {
            throw new FileNotFoundException($"Save file not found at {absPath}");
        }
        
        LogWriter.WriteLog($"File found at {absPath}", new Success());
        LogWriter.WriteLog("Checking for backups directory...", new Info());
        
        this.saveFilePath = saveFilePath;

        if (!Directory.Exists("backups"))
        {
            LogWriter.WriteLog("Backups directory not found, creating...", new Info());
            Directory.CreateDirectory("backups");
            LogWriter.WriteLog("Backups directory created", new Success());
        }
        else
        {
            LogWriter.WriteLog("Backups directory found", new Success());
        }
        
        saveContent = "";
    }

    public void LoadSave()
    {
        LogWriter.WriteLog("Creating backup of the save file...", new Info());
    }

    public string BackupFolderPath
    {
        get => backupFolderPath;
        set => backupFolderPath = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string SaveFilePath
    {
        get => saveFilePath;
        set => saveFilePath = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string SaveContent
    {
        get => saveContent;
        set => saveContent = value ?? throw new ArgumentNullException(nameof(value));
    }
}