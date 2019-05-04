<#
.SYNOPSIS
    Script to update databases
.DESCRIPTION
    This is a interactive script used to update databases for different database contexts and connection strings
.NOTES
    File Name      : update-database.ps1
    Author         : Fahao Tang
    Prerequisite   : dotnet core CLI 2.x or above, PowerShell V3+
.LINK
.EXAMPLE
#>

$dir=Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$packageLocation="$dir\src\Kontext.Docu.Web.Portals\bin\Debug\netcoreapp2.1\packages"

function Get-DbContextList
{
	process
	{
        $libPath="$dir\src\Kontext.Docu.Web.Portals\bin\Debug\netcoreapp2.1\Kontext.Docu.Web.Portals.dll"
		Write-Host -ForegroundColor Green "Retrieve database context list from $libPath."
        $assembly = [Reflection.Assembly]::Loadfile($libPath)
        $assemblyName = $Assembly.GetName()
        Write-Host $assemblyName
        # Hard-code currently
        return @("ApplicationDbContext","ContextBlogDbContext")
	}
}

function Get-DataBaseTypes
{
    process
	{
        # Hard-code currently
        return @("Development","DevSQLServer")
	}
}

cd $dir\src\Kontext.Docu.Web.Portals

#&dotnet restore --packages $packageLocation
&dotnet build -v q
$contextList=Get-DbContextList
Write-Host -ForegroundColor Green "Select database context"
$i=0
foreach($context in $contextList)
{
    Write-Host "[$i] - $context"
    $i=$i+1
}

$dbContextOption=Read-Host -Prompt "input the index of the DbContext"
$dbContext = $contextList[$dbContextOption]
Write-Host "$dbContext is selected"

$envList=Get-DataBaseTypes

Write-Host -ForegroundColor Green "Select database type"
$i=0
foreach($env in $envList)
{
    Write-Host "[$i] - $env"
    $i=$i+1
}

$envOption=Read-Host -Prompt "input the index of the environment"
$env = $envList[$envOption]
Write-Host "$env is selected"

$env:ASPNETCORE_ENVIRONMENT=$env
$command = "dotnet ef database update -c $dbContext"
Write-Host -ForegroundColor Yellow "Executing command: $command"
&dotnet ef database update -c $dbContext