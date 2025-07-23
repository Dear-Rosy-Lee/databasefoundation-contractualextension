; 脚本由 Inno Setup 脚本向导 生成！
; 有关创建 Inno Setup 脚本文件的详细资料请查阅帮助文档！

#define MyAppName "空间数据匹配工具"
#define MyAppVersion "1.0.2507.2301"
#define MyAppPublisher "鱼鳞图"
#define MyAppURL "http://www.yulintu.com/"
#define MyAppExeName "SpitalDataMatchTool.exe"

[Setup]
; 注: AppId的值为单独标识该应用程序。
; 不要为其他安装程序使用相同的AppId值。
; (生成新的GUID，点击 工具|在IDE中生成GUID。)
AppId={{9A75153E-E8E3-4219-A3D3-3670CAB204E9}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName=D:\Program Files\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=D:\Program Files (x86)\Jenkins\jobs\图斑匹配工具\WorkSpace\安装包
OutputBaseFilename=图斑匹配工具-{#MyAppVersion}
Compression=lzma                                                                       
SolidCompression=yes
WizardImageBackColor=clBlue
BackColor=clBlue
BackColor2=clBlack
WizardImageStretch=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkablealone;
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkablealone;

[Files]
Source: "D:\Program Files (x86)\Jenkins\jobs\图斑匹配工具\WorkSpace\Workspace\SpitalDataMatchTool.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Program Files (x86)\Jenkins\jobs\图斑匹配工具\WorkSpace\Workspace\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; 注意: 不要在任何共享系统文件上使用“Flags: ignoreversion”

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Code]
//安装前判断是否有进程正在运行
function IsModuleLoaded(modulename: String ): Boolean;
external 'IsModuleLoaded@files:psvince.dll stdcall setuponly';    

function KillTask(ExeFileName: string): Integer;
external 'KillTask@files:ISTask.dll stdcall delayload'; 

function InitializeSetup(): boolean;

var IsAppRunning: boolean;

begin
Result:= true;
IsAppRunning:= IsModuleLoaded('escertd_et199.exe');
if IsAppRunning then
    KillTask('escertd_et199.exe');
IsAppRunning:= IsModuleLoaded('SpitalDataMatchTool.exe');
while IsAppRunning do
begin
if MsgBox('空间数据匹配工具正在运行,关闭程序继续安装?', mbConfirmation, MB_OKCANCEL) = IDOK then 
 begin
  KillTask('SpitalDataMatchTool.exe');
  IsAppRunning:= false;
 end
else
begin
IsAppRunning:= false;
Result:= false;
end;
end;
end;    


// 卸载前判断进程是否在运行
function RunTaskU(FileName: string; bFullpath: Boolean): Boolean;
  external 'RunTask@{app}/ISTask.dll stdcall delayload uninstallonly';
function KillTaskU(ExeFileName: string): Integer;
  external 'KillTask@{app}/ISTask.dll stdcall delayload uninstallonly';

function InitializeUninstall(): boolean;

var IsAppRunning: boolean;

begin
Result:= true;
IsAppRunning:= RunTaskU('escertd_et199.exe',false);
if IsAppRunning then
    KillTaskU('escertd_et199.exe');
IsAppRunning:= RunTaskU('SpitalDataMatchTool.exe',false);
while IsAppRunning do
begin
if MsgBox('空间数据匹配工具正在运行,关闭工具继续卸载?', mbConfirmation, MB_OKCANCEL) = IDOK then 
 begin
  KillTaskU('SpitalDataMatchTool.exe');
  IsAppRunning:= false;
 end
else
begin
IsAppRunning:= false;
Result:= false;
end;
end;
end;
