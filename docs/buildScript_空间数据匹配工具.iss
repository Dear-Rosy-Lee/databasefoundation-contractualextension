; �ű��� Inno Setup �ű��� ���ɣ�
; �йش��� Inno Setup �ű��ļ�����ϸ��������İ����ĵ���

#define MyAppName "�ռ�����ƥ�乤��"
#define MyAppVersion "1.0.2507.2301"
#define MyAppPublisher "����ͼ"
#define MyAppURL "http://www.yulintu.com/"
#define MyAppExeName "SpitalDataMatchTool.exe"

[Setup]
; ע: AppId��ֵΪ������ʶ��Ӧ�ó���
; ��ҪΪ������װ����ʹ����ͬ��AppIdֵ��
; (�����µ�GUID����� ����|��IDE������GUID��)
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
OutputDir=D:\Program Files (x86)\Jenkins\jobs\ͼ��ƥ�乤��\WorkSpace\��װ��
OutputBaseFilename=ͼ��ƥ�乤��-{#MyAppVersion}
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
Source: "D:\Program Files (x86)\Jenkins\jobs\ͼ��ƥ�乤��\WorkSpace\Workspace\SpitalDataMatchTool.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Program Files (x86)\Jenkins\jobs\ͼ��ƥ�乤��\WorkSpace\Workspace\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; ע��: ��Ҫ���κι���ϵͳ�ļ���ʹ�á�Flags: ignoreversion��

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Code]
//��װǰ�ж��Ƿ��н�����������
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
if MsgBox('�ռ�����ƥ�乤����������,�رճ��������װ?', mbConfirmation, MB_OKCANCEL) = IDOK then 
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


// ж��ǰ�жϽ����Ƿ�������
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
if MsgBox('�ռ�����ƥ�乤����������,�رչ��߼���ж��?', mbConfirmation, MB_OKCANCEL) = IDOK then 
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
