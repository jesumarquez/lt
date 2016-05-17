Start-Transcript c:\temp\fota_clean.txt
$fota = 'C:\Temp\FOTA'
$a = Get-Date
$a = $a.AddDays( -1)
Write-Host "borrando fota mas viejo de : " $a
Get-ChildItem -Path $fota -Recurse -Filter *.txR | Where-Object { $_.LastWriteTime -le $a } | ForEach-Object { Write-output $_.FullName ; Remove-Item $_.FullName }
Stop-Transcript 

$smtpServer = "127.0.0.1"
$att = new-object Net.Mail.Attachment("c:\temp\fota_clean.txt")
$msg = new-object Net.Mail.MailMessage
$smtp = new-object Net.Mail.SmtpClient($smtpServer)
$msg.From = "soporte@logictracker.com"
$msg.To.Add("matiasa@logictracker.com")
$msg.To.Add("soporte@logictracker.com")
$msg.Subject = "Borrado de Fotas"
$msg.Body = "En el attach esta el reporte"
$msg.Attachments.Add($att)
$smtp.Send($msg)
$att.Dispose()