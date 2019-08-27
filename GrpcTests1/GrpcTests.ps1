[CmdletBinding()]
param (
	[string] $bind,
	[int] $count = 10,
	[int] $dumpCount = 10
	)
function Log($msg) { Write-Host "[$(Get-Date)] $msg" }

if (!$bind) { 
	$bind = [System.Net.Dns]::GetHostEntry('localhost').HostName
}
$dumpDir = "$PsScriptRoot\dumps"

$exe = "$PsScriptRoot\GrpcTests.exe"
if (!(Test-Path $exe)) { $exe = "$PsScriptRoot\bin\debug\net462\GrpcTests.exe" }
if (!(Test-Path $exe)) { throw "Unable to find GrpcTests.exe" }

if (!(Test-Path $dumpDir)) { mkdir $dumpDir | Out-Null }
$a = @('-ma', '-e', '-f', '*AccessViolation*', '-x', $dumpDir, $exe, $bind, $count)
$plist = @()

while (1) {
	$existingDumpCount = dir "$dumpDir\*.dmp" | measure | Select -Exp Count
	if ($existingDumpCount -lt $dumpCount) {
		while ($plist.Length -lt $count) {
			$p = Start-Process procdump.exe -Argument $a -PassThru
			$instance = @{ 
				Process = $p
				Start = Get-Date
				Minutes = Get-Random -Min 1 -Max 180
			}
			Log "Started new instance with lifetime $($instance.Minutes) minutes"
			$plist += $instance
		}
	} else {
		Log "Dump count exceeded: $existingDumpCount"
	}

	Start-Sleep -Seconds 5

	$now = Get-Date
	[array] $plist = $plist | % {
		if ($_.Process.HasExited -eq $false) {
			$age = ($now - $_.Start).TotalMinutes
			if ($age -ge $_.Minutes) {
				Log "Killing instance"
				$_.Process.Kill()
			} else {
				$_
			}
		}
	}
}

