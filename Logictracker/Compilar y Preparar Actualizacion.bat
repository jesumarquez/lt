@echo off

del compile.log

echo Compilando Solucion... 
echo (esta operacion puede tardar varios minutos, horas, dias)
rem "%PROGRAMFILES(X86)%\Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe" "Logictracker.Full.sln" /build Debug /out "compile.log"
msbuild "Logictracker.Full.sln" /p:Configuration=Release
type compile.log

echo Compilacion Terminada
pause
echo Publicando Web...
echo (es probable que esta operacion tarde varios lustros)
C:\Windows\Microsoft.NET\Framework\v2.0.50727\aspnet_compiler -nologo -v / -p "Logictracker\src\Web\Logictracker\Logictracker.Web" -u "PrecompiledWeb\Logictracker.Web" -f -fixednames
echo Publicacion Terminada (finalmente)
pause
echo Preparando archivos para copiar...
"Preparar Actualizacion.bat"

pause
