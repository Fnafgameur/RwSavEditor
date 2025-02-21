using System.Text.RegularExpressions;
using RwSavEditor.objects;
using RwSavEditor.utils;
using RwSavEditor.utils.logs.logtypes;
using RwSavEditor.utils.save;

namespace RwSavEditor.managers;

public class SaveManager
{
    // File & backups
    private const string backupFileEnd = "_backup";
    private const string backupFolder = "rwsav_backups";
    private string saveFilePath;
    private string saveContent;

    private List<Save> saves = new List<Save>();
    
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
            Directory.CreateDirectory(backupFolder);
            LogWriter.WriteLog("Backups directory created", new Success());
        }
        else
        {
            LogWriter.WriteLog("Backups directory found", new Success());
        }
        
        CreateBackup();
        
        saveContent = "";
    }

    public void LoadSave()
    {
        saveContent = File.ReadAllText(saveFilePath);
        string startPatt = Save.SAVE_START_STRING_PATTERN;
        Regex regexStart = new Regex(startPatt);
        MatchCollection matches = regexStart.Matches(saveContent);
        
        if (matches.Count == 0)
        {
            LogWriter.WriteLog("Save file is not a valid Rain World save file", new Error());
            return;
        }
        
        LogWriter.WriteLog("Save file is a valid Rain World save file", new Success());
        
        foreach (Match match in matches)
        {
            Save save = new Save();
            int startIndex = match.Index;
            Console.WriteLine(startIndex);
            int endIndex = regexStart.Match(saveContent, startIndex + 1).Index;
            
            save.Slugcat = LoadSlugcat(startIndex, endIndex);
        }
        
    }

    private Slugcat LoadSlugcat(int startI, int endI)
    {
        string regPatt = Slugcat.SLUGCATS_NAME_STRING_PATTERN;
        Regex regex = new Regex(regPatt);
        Match match = regex.Match(saveContent, startI);

        int startScug = match.Index;

        Console.WriteLine(GetValue(startScug, endI, "DENPOS"));
        
        

        return null;
    }

    private string GetValue(int startIndex, int endIndex, string searchStr)
    {
        string regPatt = SaveUtils.CreateStringPattern(searchStr + "&lt", SaveUtils.TAG_SV);
        Regex regex = new Regex(regPatt);
        int valueStartIndex = regex.Match(saveContent, startIndex).Index;
        
        Console.WriteLine(startIndex);
        Console.WriteLine(valueStartIndex);
        
        regPatt = ";[a-zA-Z0-9]+&lt;";
        regex = new Regex(regPatt);
        string value = regex.Match(saveContent, valueStartIndex + 1).Value;
        
        return value;
    }
    
    private void CreateBackup()
    {
        string backupPath = Path.Combine(backupFolder, Path.GetFileName(saveFilePath) + backupFileEnd);
        LogWriter.WriteLog($"Creating backup at {backupPath}...", new Info());
        
        if (File.Exists(backupPath))
        {
            LogWriter.WriteLog("Backup already exists, would you like to replace it?", new Info());
            
            // Temporary, replace with pop-up window later
            string input = Console.ReadLine();
            if (input.ToLower() == "y" || input.ToLower() == "yes")
            {
                LogWriter.WriteLog("Replacing backup...", new Info());
                File.Delete(backupPath);
                File.Copy(saveFilePath, backupPath);
                LogWriter.WriteLog("Backup replaced", new Success());
            }
            else
            {
                LogWriter.WriteLog("Backup not replaced", new Info());
            }

            return;
        }
        
        File.Copy(saveFilePath, backupPath);
        LogWriter.WriteLog("Backup created", new Success());
    }

    public string BackupFolderPath => backupFolder;

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