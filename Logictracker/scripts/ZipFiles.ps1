Param (
	[parameter(Mandatory=$True)] [string]$compress = "", [string]$source = "", [string]$destination = ""
    )

	[boolean]$toCompress = ([System.Convert]::ToBoolean($compress))
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
    If(!(Test-path $source)) { Write-Output "No se encuentra el archivo a descomprimir: $source"}

    #Add-Type -assembly "system.io.compression.filesystem"

    #[io.compression.zipfile]::ExractToDirectory($Source, $destination)

    [System.Reflection.Assembly]::LoadWithPartialName("System.IO.Compression.FileSystem")
    [System.IO.Compression.ZipFile]::ExtractToDirectory("$Source", "$destination")
}

function CleanPath($destination)
{
	If(Test-path $destination\Web) {Remove-item $destination\Web -recurse}
	If(Test-path $destination\WebApi) {Remove-item $destination\WebApi -recurse}
	If(Test-path $destination\Express) {Remove-item $destination\Express -recurse}
	If(Test-path $destination\ParserHost) {Remove-item $destination\ParserHost -recurse}
	If(Test-path $destination\ReportsHost) {Remove-item $destination\ReportsHost -recurse}
}

if($toCompress)
{
    Zip-Files $source $destination
}
else
{
	CleanPath $destination
    Unzip-Files $source $destination
}
