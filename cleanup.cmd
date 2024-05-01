@ECHO OFF

IF EXIST "%localappdata%\Zup" (RD /S /Q "%localappdata%\Zup")

SET "ZupDb=%USERPROFILE%\Documents\Zup"
IF NOT EXIST "%USERPROFILE%\Documents\Zup" (
    SET "ZupDb=%USERPROFILE%\OneDrive\Documents\Zup"
)

IF EXIST "%ZupDb%" (RD /S /Q "%ZupDb%")