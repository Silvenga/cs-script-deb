var target = Argument<string>("target", "Default");

const string sevenZip = "7z";
const string chmod = "chmod";
const string downloadLink = "http://download-codeplex.sec.s-msft.com/Download/Release?ProjectName=csscriptsource&DownloadId=1484264&FileTime=130882256360700000&Build=21031";
const string tempPath = "./tmp";

const string baseDir = "./cs-script";
const string inc = baseDir + "/inc";
const string bin = baseDir + "/bin";
const string doc = baseDir + "/doc";

ProcessSettings GetSettings(string args, DirectoryPath directory = null)
{
    return new ProcessSettings
    {
        Arguments = args,
        RedirectStandardOutput = true,
        WorkingDirectory = directory
    };
}

void DeleteOrIgnore(string dir)
{
    Information("Cleaning {0}.", dir);
    if(DirectoryExists(dir)){
        DeleteDirectory(dir, true);
    }
}

var tempDir = Directory(tempPath);
var extractDir = tempDir + Directory("release");

Task("Clean")
    .Does(() =>
{
    DeleteOrIgnore(tempDir);
    DeleteOrIgnore(bin);
    DeleteOrIgnore(inc);
    DeleteOrIgnore(doc);
    DeleteFiles("*.deb");
    DeleteFiles("*.build");
    DeleteFiles("*.changes");
    var exit = StartProcess("debuild", GetSettings("clean", baseDir));
    if(exit != 0)
    {
        throw new Exception("Cleaning was not successful.");
    }
});

Task("DownloadAndExtract")
    .IsDependentOn("Clean")
    .Does(() =>
{
    CreateDirectory(tempDir);
    var releaseArchive = tempDir + File("release.7z");
    Information("Downloading release.", downloadLink, releaseArchive);
    DownloadFile(downloadLink, releaseArchive);

    var args = string.Format("x {0} -o{1}", releaseArchive, extractDir);

    Information("Extracting release.");
    var exit = StartProcess(sevenZip, GetSettings(args));
    if(exit != 0)
    {
        throw new Exception("Extracting was not successful.");
    }
});

Task("Configure")
    .IsDependentOn("DownloadAndExtract")
    .Does(() =>
{
    CreateDirectory(bin);
    CreateDirectory(inc);
    CreateDirectory(doc);
    var extractDirStr = extractDir.ToString();
    CopyFiles(extractDirStr + "/cs-script/inc/*", inc);
    CopyFiles(extractDirStr + "/cs-script/*.txt", doc);
    CopyFile(extractDirStr + "/cs-script/cscs.exe", bin + "/cscs");

    Information("Setting permissions.");
    var exit = StartProcess(chmod, GetSettings(string.Format("+x {0}/cscs", bin)));
    if(exit != 0)
    {
        throw new Exception("Set permissions was not successful.");
    }
});

Task("Build")
    .IsDependentOn("Configure")
    .Does(() =>
{
    var exit = StartProcess("debuild", GetSettings("-us -uc -b", baseDir));
    if(exit != 0)
    {
        throw new Exception("Debuild was not successful.");
    }
});

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);
