namespace minecraft_launcher_v2.ConstantValues
{
    static class Messages
    {
        public const string CAPTION_COMMON = "Minecraft launcher";
        public const string CAPTION_SET_JAVA = "Set Java folder";
        public const string CAPTION_SET_MAIN_DIR = "Set Main Directory folder";


        public const string ERROR_GET_FILES = "Can't download file(s)\n";
        public const string ERROR_GET_ASSETS_URLS = "Can't get assets urls\n";
        public const string ERROR_GET_LIBRARIES = "Can't download libraries\n";
        public const string ERROR_GET_VERSION_JSON = "Can't get version json file\n";
        public const string ERROR_GET_INSTALLED_VERSIONS = "Can't get all installed versions\n";
        public const string ERROR_GET_VERSION_FROM_MANIFEST_F = "Can't find version {0} in versions manifest\n";

        public const string ERROR_READ_FILE_F = "Can't read data from {0}\n";

        public const string ERROR_DELETE_FOLDER_F = "Can't delete folder {0}\n";
        public const string ERROR_DELETE_FILE_F = "Can't delete file {0}\n";


        public const string ERROR_SAVE_SETTINGS_PROFILE = "Can't save profile data\n";
        public const string ERROR_SAVE_SETTINGS_GLOBAL = "Can't save global settings\n";
        public const string ERROR_SAVE_SETTINGS_COMMON = "Can't save common settings\n";
        public const string ERROR_SAVE_SETTINGS_VERSION = "Can't save version's settings\n";

        public const string ERROR_LOAD_SETTINGS_PROFILE = "Can't load profile data\n";
        public const string ERROR_LOAD_SETTINGS_GLOBAL = "Can't load global settings\n";
        public const string ERROR_LOAD_SETTINGS_COMMON = "Can't load common settings\n";
        public const string ERROR_LOAD_SETTINGS_VERSION = "Can't load version's settings\n";


        public const string ERROR_JAVA_NOT_FOUND = "Java directory isn't set or Java executable files aren't found\n";
        public const string ERROR_JAVA_CANT_FIND = "Can't find Java on your machine.\nPlease set Java folder wich contains \"bin\"folder\n";
        public const string ERROR_JAVA_PATH_WRONG_F = "Can't find Java by this path: \"{0}\".\nFind Java automatically?";


        public const string ERROR_RAM_FORMAT_INCORRECT = "Incorrect memory format (i.e. try 1024M)\n";
        public const string ERROR_RAM_ALLOCATION_SIZE_INCORRECT_F = "Allocated memory is less than {0}m or more than your total RAM\n";
        public const string ERROR_RAM_ALLOCATION_INSUFICIENT = "Insuficient free RAM\n";

        public const string ERROR_LAUNCH_VERSION_EMPTY = "Please, choose appropriate version to launch\n";
        public const string ERROR_NICKNAME_INCORRECT = "Incorrect nickname\nYour nickname shouln'd be empty or contain whitespaces";
        public const string ERROR_INITIALIZATION_VERISON = "Error occurred while initialization\n";

        public const string ERROR_PROCESS_START_MINECRAFT = "Can't start minecraft process\n";
        public const string ERROR_PROCESS_START_BROWSER = "Can't start browser to open download link\n";


        public const string ERROR_APPLICATION_RUNNING = "Another instance of launcher is already running\n";
        public const string ERROR_EXTRACTION_FILES_F = "Can't extract file(s) {0}\n";
        public const string ERROR_LOG_SAVE = "Error occured while saving error log\n";

        public const string LOG_EXCEPTION_FOUND_IN_VERSION = "Exception Found in version ";
        public const string LOG_EXCEPTION_TYPE = "Type: ";
        public const string LOG_EXCEPTION_MESSAGE = "Message: ";
        public const string LOG_EXCEPTION_SOURCE = "Source: ";
        public const string LOG_EXCEPTION_STACKTRACE = "Stacktrace: ";
        public const string LOG_EXCEPTION_INNER = "Inner Exception: ";


        public const string QUESTION_SETTINGS_RESET = "Are you sure, if you want to reset settings?\n";
        public const string QUESTION_EXIT_IF_DOWNLOADING = "You are downloading game files.\nAre you sure you want to exit now?\n";
        public const string QUESTION_LOG_OUT_EVERYWHERE = "Sign out from all sessions?\nPress \"No\" to sign out from current session.\n";
        public const string QUESTION_LOG_OUT_HERE = "Sign out from current session?\n";
        public const string QUESTION_START_WITHOUT_LOGGING = "You are not logged in.\nYou will be able to play only single player.\nDo you want to start game?\n";


        public const string REQUEST_SET_MAIN_DIRECTORY = "Please, set game directory\n";
        public const string REQUEST_SET_JAVA_PATH = "Please, set Java directory\n";


        public const string NOTIFICATION_CONTENT = "New versions of hungry launcher was found";
        public const string NOTIFICATION_ACTION_DOWNLOAD = "Go to download page";
        public const string NOTIFICATION_ACTION_LATER_F = "Remind in {0} days";
        public const string NOTIFICATION_UPDATE_AVAILABLE = "Update available";


        public const string AUTHENTICATION_SUCCESS = "Successfully logged in\n";
        public const string AUTHENTICATION_METHOD_NOT_ALLOWED = "The method specified in the request is not allowed for the resource identified by the request URI\n";
        public const string AUTHENTICATION_NOT_FOUND = "The server has not found anything matching the request URI\n";
        public const string AUTHENTICATION_INVALID_CREDENTIALS = "Invalid credentials.\n";
        public const string AUTHENTICATION_INVALID_CREDENTIALS_MIGRATED = "Invalid credentials. Account migrated, use e-mail as username.\n";
        public const string AUTHENTICATION_INVALID_CREDENTIALS_TOO_MANY_ATTEMPTS = "Too many login attempts with this username recently\n";
        public const string AUTHENTICATION_CREDENTIALS_IS_NULL = "Credentials is null\n";
        public const string AUTHENTICATION_INVALID_TOKEN = "Invalid access token.\n";
        public const string AUTHENTICATION_ACCESS_TOKEN_HAS_PROFILE = "Access token already has a profile assigned. Selecting profiles isn't implemented yet.\n";
        public const string AUTHENTICATION_UNSUPPORTED_MEDIA_TYPE = "Data was not submitted as application/json\n";
        public const string AUTHENTICATION_UNKNOWN_ERROR = "Unknown error occurred while authentication";

    }
}
