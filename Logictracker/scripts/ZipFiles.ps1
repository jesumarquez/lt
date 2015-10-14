Param (
	[parameter(Mandatory=$True)] [bool]$compress = $True, [string]$source = "", [string]$destination = ""
    )

#If (-not (Test-Path $source -PathType container)) {
 #   Write-Output "No se encuentra la carpeta source $source"
  #  Write-Output "Uso: $($MyInvocation.MyCommand.Name) <sourceFolder>"
   # Exit
#}

function Zip-Files($source, $destination)
{
    If(Test-path $destination) {Remove-item $destination}

    Add-Type -assembly "system.io.compression.filesystem"

    [io.compression.zipfile]::CreateFromDirectory($Source, $destination) 
}

function Unzip-Files($source, $destination)
{
    If(!Test-path $source) { Write-Output "No se encuentra el archivo a descomprimir: $source"}

    #Add-Type -assembly "system.io.compression.filesystem"

    #[io.compression.zipfile]::ExractToDirectory($Source, $destination)

    [System.Reflection.Assembly]::LoadWithPartialName("System.IO.Compression.FileSystem")
    [System.IO.Compression.ZipFile]::ExtractToDirectory("$Source", "$destination")
}

if($compress)
{
    Zip-Files $source $destination
}
else
{
    Unzip-Files $source $destination
}
