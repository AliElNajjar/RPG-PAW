using UnityEngine;
#if UNITY_SWITCH
using nn.fs;
using nn.account;
#endif

#if UNITY_SWITCH
public class NintendoSwitchSaveData : ISaveData
{
    private const string MOUNT_NAME = "Acolyte";
    private const string FILE_NAME = "AcolyteData";
    private const int SAVE_DATA_SIZE = 64000;

    private FileHandle _fileHandle;
    private Uid _userId;
    private string _filePath;

    private bool _accountInitialized;

    private object save;

#region PROPERTIES
    public Uid UserID
    {
        get { return _userId; }
        set { _userId = value; }
    }

    public bool DirectoryExists
    {
        get {
            // Use FileSystem.GetEntryType() to check if the file exists without incurring a write operation.
            // This error handling is not required if you know the file will always exist.
            nn.fs.EntryType entry = new nn.fs.EntryType();
            nn.Result result = nn.fs.FileSystem.GetEntryType(ref entry, _filePath);
            if (nn.fs.FileSystem.ResultPathNotFound.Includes(result))
            {
                return false;
            }

            return true;
        }
    }

    public bool SaveFileExists
    {
        get
        {
            // Use FileSystem.GetEntryType() to check if the file exists without incurring a write operation.
            // This error handling is not required if you know the file will always exist.
            nn.fs.EntryType entry = new nn.fs.EntryType();
            nn.Result result = nn.fs.FileSystem.GetEntryType(ref entry, _filePath);
            if (nn.fs.FileSystem.ResultPathNotFound.Includes(result))
            {
                return false;
            }

            return true;
        }
    }
#endregion

    public NintendoSwitchSaveData()
    {
        Initialize();
    }

    public void OnDestroy()
    {
        FileSystem.Unmount(MOUNT_NAME);
    }

    public void Initialize()
    {
        if (!_accountInitialized)
        {
            //This should be in an initializer for Switch
            nn.account.Account.Initialize();

            // Open the user that was selected before the application started.
            // This assumes that Startup user account is set to Required.
            nn.account.UserHandle userHandle = new nn.account.UserHandle();
            nn.account.Account.OpenPreselectedUser(ref userHandle);

            // Get the user ID of the preselected user account.            
            nn.account.Account.GetUserId(ref _userId, userHandle);

            _accountInitialized = true;
        }

        _filePath = string.Format("{0}:/{1}", MOUNT_NAME, FILE_NAME);

        // mount save data        
        nn.Result result = nn.fs.SaveData.Mount(MOUNT_NAME, _userId);

        // This error handling is optional.
        // The mount operation will not fail unless the save data is already mounted or the mount name is in use.
        if (FileSystem.ResultTargetLocked.Includes(result))
        {
            // Save data for specified user ID is already mounted. Get account name and display an error.
            Nickname nickname = new Nickname();
            Account.GetNickname(ref nickname, _userId);
            Debug.LogErrorFormat("The save data for {0} is already mounted: {1}", nickname.name, result.innerValue);
        }
        else if (FileSystem.ResultMountNameAlreadyExists.Includes(result))
        {
            // The specified mount name is already in use.
            Debug.LogErrorFormat("The mount name '{0}' is already in use: {1}", MOUNT_NAME, result.innerValue);
        }
        result.abortUnlessSuccess();

        if (result.IsSuccess())
            Debug.Log("Mount succesful");

        SaveSystem.Initialize(true);

        if (SaveFileExists) LoadNintendos();
        //SaveSystem.OnSave += SaveNintendos;
    }

    public void CreateDirectory()
    {
        throw new System.NotSupportedException("Not Supported on Nintendo Switch.");
    }

    public void DeleteSaveFile()
    {
        nn.Result result = File.Delete(_filePath);
        if (!FileSystem.ResultPathNotFound.Includes(result))
            result.abortUnlessSuccess();
    }

    public object Load(object fileToLoad)
    {        
        byte[] data = Read();
        return SaveSystem.FromByteArray(data);
    }

    public void SaveNintendos()
    {
        Write(SaveUtils.ToByteArray(save));
    }

    public void LoadNintendos()
    {        
        byte[] data = Read();
        SaveSystem.RestoreFromByteArray(data);
    }

    public void Save(object save)
    {
        this.save = save;
        SaveNintendos();
    }

    private byte[] Read()
    {
        EntryType entry = 0;
        nn.Result result = FileSystem.GetEntryType(ref entry, _filePath);
        if (FileSystem.ResultPathNotFound.Includes(result))
        {
            Debug.Log("File not found");
            return new byte[0];
        }
        result.abortUnlessSuccess();

        result = File.Open(ref _fileHandle, _filePath, OpenFileMode.Read);
        result.abortUnlessSuccess();

        long fileSize = 0;
        result = File.GetSize(ref fileSize, _fileHandle);
        result.abortUnlessSuccess();

        byte[] data = new byte[fileSize];
        result = File.Read(_fileHandle, 0, data, fileSize);
        result.abortUnlessSuccess();

        File.Close(_fileHandle);
        return data;
    }

    private void Write(byte[] data)
    {
        UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();

        nn.Result result = new nn.Result();

        if (SaveFileExists)
        {
            result = File.Delete(_filePath);
            if (!FileSystem.ResultPathNotFound.Includes(result))
                result.abortUnlessSuccess();
        } 

        result = File.Create(_filePath, SAVE_DATA_SIZE);
        result.abortUnlessSuccess();

        result = File.Open(ref _fileHandle, _filePath, OpenFileMode.Write);
        result.abortUnlessSuccess();

        result = File.Write(_fileHandle, 0, data, data.LongLength, WriteOption.Flush);
        result.abortUnlessSuccess();

        File.Close(_fileHandle);

        result = nn.fs.FileSystem.Commit(MOUNT_NAME);
        result.abortUnlessSuccess();

        UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();

        Debug.Log("saved successfully");
    }

    public void SetFilePath(string setDirectoryPath, string setFileName, string setFileExtension)
    {
        throw new System.NotSupportedException("Not Supported");
    }
}
#endif