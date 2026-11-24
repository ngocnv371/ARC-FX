@echo off
setlocal
rem Opens ARC FX Gallery. Options go after the script name, e.g. run.cmd --effect=explosion_fire
rem   --view=close             start on the close look instead of the game camera
rem   --orbit=60,-20           start with the camera turned 60 degrees round and 20 lower
rem   --filter=skill           start with the list narrowed (commas for several words)
rem   --speed=0.25             start slowed down (1, 0.5, 0.25, 0.1, 0.05; - and + change it)
rem   --shot=dir [--only=a,b]  save dir\<effect>.png for each effect (or just those) and quit;
rem                            the window opens minimised and never takes focus
rem Needs a Godot 4.7 .NET (mono) editor: GODOT, else the newest under D:\installed or C:\installed.
rem Set GALLERY_NOBUILD=1 to skip the C# build.
if defined GODOT goto :found
for %%D in ("D:\installed" "C:\installed" "%ProgramFiles%\Godot" "%LOCALAPPDATA%\Godot") do (
    rem dir /s lists folders in name order, so the last match is the newest version.
    for /f "delims=" %%G in ('dir /b /s /o:n "%%~D\Godot_v4*_mono_win64_console.exe" 2^>nul') do set "GODOT=%%G"
)
:found
if not exist "%GODOT%" (
    echo Godot 4 .NET not found. Set GODOT to your Godot 4.7 mono executable.
    exit /b 1
)

rem The gallery sees the library as res://addons/arc_fx, a junction to this folder's parent
rem (made here, as git cannot hold one). Godot skips this folder when it scans through it,
rem because it holds a project.godot of its own.
if not exist "%~dp0addons\arc_fx\FxWorld.cs" (
    if not exist "%~dp0addons" mkdir "%~dp0addons"
    mklink /J "%~dp0addons\arc_fx" "%~dp0.." >nul || exit /b 1
)
if not defined GALLERY_NOBUILD dotnet build "%~dp0ArcFxGallery.csproj" -nologo -v:q || exit /b 1
rem Import on the first run, and again whenever a sound has none yet (a game run won't import it).
set "IMPORT="
if not exist "%~dp0.godot" set "IMPORT=1"
for %%F in ("%~dp0..\audio\*.ogg" "%~dp0..\audio\*.wav") do if not exist "%%~F.import" set "IMPORT=1"
if defined IMPORT "%GODOT%" --headless --path "%~dp0." --import >nul 2>&1

rem GALLERY_CWD keeps a relative --shot=dir relative to where you ran this.
set "GALLERY_CWD=%CD%"
echo.%* | findstr /c:"--shot=" >nul && goto :bake
start "" "%GODOT%" --path "%~dp0." -- %*
exit /b 0

:bake
rem Godot reads override.cfg before it opens the window: unfocusable from the first frame, so a
rem bake never pulls you out of what you are typing in, and parked far off screen rather than
rem minimised, because a minimised window draws only now and then and the pictures would lag
rem the effects. Removed afterwards.
set "OVERRIDE=%~dp0override.cfg"
> "%OVERRIDE%" (
    echo [display]
    echo window/size/no_focus=true
    echo window/size/borderless=true
    echo window/size/initial_position_type=0
    echo window/size/initial_position=Vector2i(-30000, -30000^)
)
"%GODOT%" --path "%~dp0." -- %*
set "CODE=%ERRORLEVEL%"
del "%OVERRIDE%" >nul 2>&1
exit /b %CODE%
