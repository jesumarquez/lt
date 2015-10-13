<# deployWeb.ps1 - Copy website to inetpub #>
Param (
	[parameter(Mandatory=$True)] [string]$targetWebsitePath = "", [string]$targetRunServicePath = "", [string]$targetWebApiPath, [string]$targetReportsServicePath, [string]$targetParserServicePath
    )

If (-not (Test-Path $targetWebsitePath -PathType container)) {
    Write-Output "No se encuentra la carpeta destino $targetWebsitePath"
    Write-Output "Uso: $($MyInvocation.MyCommand.Name) <targetFolder>"
    Exit
}

function Get-ScriptDirectory
{
    $Invocation = (Get-Variable MyInvocation -Scope 1).Value;
    if($Invocation.PSScriptRoot)
    {
        $Invocation.PSScriptRoot;
    }
    Elseif($Invocation.MyCommand.Path)
    {
        Split-Path $Invocation.MyCommand.Path
    }
    else
    {
        $Invocation.InvocationName.Substring(0,$Invocation.InvocationName.LastIndexOf("\"));
    }
}

$source = Get-ScriptDirectory
Write-Output "Source folder: $source`r"

function RemoveItem($item) 
{
    If (Test-Path $item) {
        Write-Output "Borrando $item..`r"
        Remove-Item -recurse $item
    }
}

RemoveItem("$source\web\*.config")
RemoveItem("$source\web\*.xml")
RemoveItem("$source\web\bin\licence.spk")
RemoveItem("$source\web\bin\App_Licenses.dll")
RemoveItem("$source\web\sonido")
RemoveItem("$source\web\iconos")
RemoveItem("$source\web\Qtree")
RemoveItem("$source\web\Pictures")
RemoveItem("$source\web\FusionCharts")
RemoveItem("$source\web\CompumapISAPI")


function Stop-Services($aliasServiceName, $processName)
{
    write-output "Servicios que seran detenidos: `n`r"
    get-service *$aliasServiceName*

    get-service *$aliasServiceName* | stop-service
    #Get-Service | Where-Object {$_.Name -like $aliasServiceName -or $_.Name -like "*re*"}
    #$serviceFilter = "{$_.Name -like $aliasServiceName -or $_.Name -like '*re*'}"

    start-sleep 4

    if(!$processName){
        kill -Force -processname $processName -ErrorAction SilentlyContinue}

    write-output "Estado de los servicios luego de detenidos: `n`r"
    get-service *$aliasServiceName*
}

function Start-Services($aliasServiceName)
{
    write-output "Servicios que seran iniciados: `r`n"
    get-service *$aliasServiceName*

    get-service *$aliasServiceName* | start-service

    start-sleep 4
    write-output "Estado de los servicios luego de iniciados: `n`r"
    get-service *$aliasServiceName*
}

write-output "Borrando archivos de configuracion en directorio Express `r`n"
RemoveItem("$source\Express\*.config")
RemoveItem("$source\Express\*.xml")
RemoveItem("$source\Express\licence.spk")
RemoveItem("$source\Express\Applications")
RemoveItem("$source\Express\Config")

write-output "Borrando archivos de configuracion en directorio WebApi `r`n"
RemoveItem("$source\WebApi\*.config")
RemoveItem("$source\WebApi\*.xml")

write-output "Borrando archivos de configuracion en directorio ReportsHost `r`n"
RemoveItem("$source\ReportsHost\*.config")
RemoveItem("$source\ReportsHost\*.xml")

write-output "Borrando archivos de configuracion en directorio ParserHost `r`n"
RemoveItem("$source\ParserHost\*.config")
RemoveItem("$source\ParserHost\*.xml")

Write-Output "Copiando sitio web. `n`r"
#%windir%\system32\inetsrv\appcmd.exe stop site /site.name:lgtkr45
xcopy $source\web $targetWebsitePath /S /R /Y

Write-Output "Copiando sitio webapi. `n`r"
xcopy $source\webapi $targetWebApiPath /S /R /Y
#%windir%\system32\inetsrv\appcmd.exe start site /site.name:lgtkr45

Write-Output "Finalizado.`r"

write-output "Deployando servicios de LT. `r`n"
Stop-Services "GatewayTrax"
Stop-Services "DispatcherTrax" "run.exe"
Stop-Services "Caesat.Gateway"
Stop-Services "LogicTracker.Reports"

# Copiar servicio
xcopy $source\express $targetRunServicePath /S /R /Y
xcopy $source\reportshost $targetReportsServicePath /S /R /Y
xcopy $source\parserhost $targetParserServicePath /S /R /Y

Start-Services "GatewayTrax"
Start-Services "DispatcherTrax" "run.exe"
Start-Services "Caesat.Gateway"
Start-Services "LogicTracker.Reports"