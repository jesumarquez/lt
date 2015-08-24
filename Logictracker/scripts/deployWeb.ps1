<# deployWeb.ps1 - Copy website to inetpub #>
Param (
	[parameter(Mandatory=$True)] [string]$target
    )

If (-not (Test-Path $target -PathType container)) {
    Write-Output "No se encuentra la carpeta destino $target"
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

Write-Output "Copiando sitio web..`r"
#%windir%\system32\inetsrv\appcmd.exe stop site /site.name:lgtkr45
xcopy $source\web $target /S /R /Y
#%windir%\system32\inetsrv\appcmd.exe start site /site.name:lgtkr45

Write-Output "Finalizado.`r"