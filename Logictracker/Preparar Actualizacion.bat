@echo off
rd /s/q Actualizacion
md Actualizacion
echo Copiando Web
xcopy /E/I/Q PrecompiledWeb\Logictracker.Web Actualizacion\Web
echo Copiando Express
xcopy /E/I/Q Applications\run\bin\Debug Actualizacion\Express
echo Copiando Report Host
xcopy /E/I/Q BackEnd\Hosts\Logictracker.Tracker.Application.Reports.Host\bin\Debug Actualizacion\ReportsHost
echo Copiando Web Api
xcopy /E/I/Q Logictracker\src\Web\Logictracker\LogicTracker.App.Web.Api\bin Actualizacion\WebApi
echo Copiando Tester
xcopy /E/I/Q Testing\HandlerTest\bin\Debug Actualizacion\Tester
echo Copiando UrbeOut
xcopy /E/I/Q Applications\LogicOut\LogicOut\bin\Debug Actualizacion\LogicOut\
echo Copiando LogicOut Intaller
xcopy /E/I/Q Applications\LogicOut\LogicOut.Install\Debug Actualizacion\LogicOut.Install\
echo Copiando LogicLink
xcopy /E/I/Q Applications\LogicLink\bin\Debug Actualizacion\LogicLink\
echo Copiando LogicLink Intaller
xcopy /E/I/Q Applications\LogicLinkInstall\Debug Actualizacion\LogicLink.Install\


cd Actualizacion\Web
echo Borrando .config
del *.config
echo Borrando .xml
del *.xml
echo Borrando licence.spk
del bin\licence.spk
echo Borrando App_Liceses
del bin\App_Licenses.dll
echo Borrando sonidos
rd /s/q sonido
echo Borrando iconos
rd /s/q iconos
echo Borrando Qtree
rd /s/q Qtree
echo Borrando Pictures
rd /s/q Pictures
echo Borrando FusionCharts
rd /s/q FusionCharts
echo Borrando CompumapISAPI
rd /s/q CompumapISAPI
cd..
cd..

cd Actualizacion\Express
echo Borrando .config
del *.config
echo Borrando .xml
del *.xml
echo Borrando licence.spk
del licence.spk
echo Borrando Applications
rd /s/q Applications
echo Borrando Config
rd /s/q Config
cd..
cd..

cd Actualizacion\WebApi
echo Borrando .config
del *.config
echo Borrando .xml
del *.xml
cd..
cd..

cd Actualizacion\ReportsHost
echo Borrando .config
del *.config
echo Borrando .xml
del *.xml
cd..
cd..

cd Actualizacion\Tester
echo Borrando .config
del *.config
echo Borrando .xml
del *.xml
cd..
cd..

cd Actualizacion\LogicOut
echo Borrando .config
del *.config
echo Borrando .xml
del *.xml
cd..
cd..

cd Actualizacion\LogicLink
echo Borrando .config
del *.config
echo Borrando .xml
del *.xml
cd..
cd..


echo Finalizado.