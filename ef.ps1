function Show-Usage {
    $help = @"
Usage:
option 1: .\ef.ps1 <command> [options]
option 2: pwsh ef.ps1 <command> [options]

Commands:
  a, add <migration-name> - Add a new migration
  u, up - Update the database to the latest migration
  s, sql <output-name> [--from <migration-name>] [--to <migration-name>] - Generate SQL for a migration
  i, info - Show information about the project and migrations

Common Options:
  -B, --no-build - Skip building the project
  -c, --connection <connection-string> - Use a specific connection string
"@
    Write-Host $help
}

Write-Host "> working directory: $(Get-Location)"

function Get-Flatten {
    param (
        [hashtable]$hash
    )
    return ($hash.GetEnumerator() | ForEach-Object { "$($_.Key) $($_.Value)" }) -join ' '
}

function Invoke-EfCommand {
    param (
        [string]$cmd,
        [hashtable]$params
    )

    # common options
    $efOptions = @(
        "-s", ".\TimesheetCli\TimesheetCli.Cli\TimesheetCli.Cli.csproj",
        "-p", ".\TimesheetCli\TimesheetCli.Core\TimesheetCli.Core.csproj",
        "--context", "TimesheetCli.Core.Db.ApplicationDbContext"
    )

    # check if skip build
    if ($script:args.Contains("--no-build") -or $script:args.Contains("-B")) {
        $efOptions += @("--no-build")
    }

    # start additional args
    $efOptions += @("--", "--automation")

    # check for connection string
    $connection = $null
    if ($script:args.Contains("--connection")) {
        $connection = Get-OptValue "--connection"
    } elseif ($script:args.Contains("-c")) {
        $connection = Get-OptValue "-c"
    }

    # make a clone to format the connection string
    $optValues = @($efOptions)
    # Build automation args
    if (![string]::IsNullOrWhiteSpace($connection)) {
        $efOptions += @("--connection", $connection)
        $optValues += @("--connection", "`"$connection`"")
    }

    $valueLog = $optValues -join ' '
    switch ($cmd) {
        "add" {
            Write-Host "> dotnet ef migrations add $valueLog"
            & dotnet ef migrations add $($params["migrationName"]) @efOptions
        }
        "update" {
            Write-Host "> dotnet ef database update $valueLog"
            & dotnet ef database update @efOptions
        }
        "sql" {
            Write-Host "> dotnet ef migrations script $($params["outputName"]) $valueLog"
            & dotnet ef migrations script -o $($params["outputName"]) @efOptions
        }
        "info" {
            Write-Host "> dotnet ef dbcontext info $valueLog"
            & dotnet ef dbcontext info @efOptions
        }
    }
}

function Get-OptValue {
    param (
        [string]$key
    )

    if (!$script:args.Contains($key)) {
        return $null
    }

    # if args[i] is the key then its value is the next arg
    for ($i=0; $i -lt $script:args.Count; $i++) {
        $arg = $script:args[$i]
        if ($arg -eq $key -and $i -lt $script:args.Count - 1) {
            return $script:args[$i + 1]
        }
    }

    return $null
}

$cmd = $args[0] # add, up, sql
switch ($cmd) {
    { $_ -in @('a', 'add') } {
        # migration name is required
        $migrationName = $script:args.Count -gt 1 ? $script:args[1] : $null
        if ($null -eq $migrationName) {
            Write-Error "Error: Migration name is required for add command"
            Show-Usage
            exit 0
        }

        Invoke-EfCommand "add" @{migrationName = $migrationName}
    }
    { $_ -in @('u', 'up') } {
        Invoke-EfCommand "update" @{}
    }
    { $_ -in @('i', 'info') } {
        Invoke-EfCommand "info" @{}
    }
    { $_ -in @('s', 'sql') } {
        # expectation: ef sql <output-name> [--from <migration-name>] [--to <migration-name>]
        $outputName = $script:args.Count -gt 1 ? $script:args[1] : $null
        if ($outputName.StartsWith("-")) {
            $outputName = $null
        }

        if ([string]::IsNullOrWhiteSpace($outputName)) {
            # use current date and time as output name
            $outputName = ("migration-{0:yyyyMMdd}" -f (Get-Date))
        }

        # append .sql to the output name if it doesn't already have it
        if (-not $outputName.EndsWith('.sql')) {
            $outputName = "$outputName.sql"
        }

        Invoke-EfCommand "sql" @{outputName = $outputName}
    }
    default {
        Show-Usage
    }
}
