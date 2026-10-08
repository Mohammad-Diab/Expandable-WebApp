# Creates the ExpandableWebAppDB database from ExpandableWebAppDB.sql, signing in with your Windows account.
#   .\Setup-Database.ps1                          (LocalDB, the default)
#   .\Setup-Database.ps1 -Server ".\SQLEXPRESS"   (SQL Server Express)
param(
    [string]$Server = '(localdb)\MSSQLLocalDB',
    [string]$Script = (Join-Path $PSScriptRoot 'ExpandableWebAppDB.sql')
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Data

$connection = New-Object System.Data.SqlClient.SqlConnection "Data Source=$Server;Initial Catalog=master;Integrated Security=True;Connect Timeout=60"
$connection.Open()
try {
    $check = $connection.CreateCommand()
    $check.CommandText = "SELECT DB_ID('ExpandableWebAppDB')"
    if ($check.ExecuteScalar() -isnot [DBNull]) {
        Write-Host "The ExpandableWebAppDB database already exists on $Server. Nothing to do."
        return
    }

    # The script has batches separated by GO lines. Its CREATE DATABASE names data files under a full
    # SQL Server's folder, so the database is created with the server's default file location instead.
    $text = [IO.File]::ReadAllText($Script)
    $batches = [regex]::Split($text, '^\s*GO\s*$', 'Multiline') | Where-Object { $_.Trim() }
    foreach ($batch in $batches) {
        $sql = $batch.Trim()
        if ($sql -match '(?m)^CREATE DATABASE \[ExpandableWebAppDB\]') { $sql = 'CREATE DATABASE [ExpandableWebAppDB]' }
        $command = $connection.CreateCommand()
        $command.CommandText = $sql
        $command.CommandTimeout = 120
        try {
            [void]$command.ExecuteNonQuery()
        }
        catch {
            # Database options this edition doesn't support are skipped; anything else stops the setup.
            if ($sql -notmatch '^(ALTER DATABASE|IF \(1 = FULLTEXTSERVICEPROPERTY)') { throw }
            Write-Host "Skipped: $(($sql -split "`n")[0].Trim())"
        }
    }
    $count = $connection.CreateCommand()
    $count.CommandText = 'SELECT COUNT(*) FROM [ExpandableWebAppDB].[dbo].[tblUsers]'
    Write-Host "Created the ExpandableWebAppDB database on $Server with $($count.ExecuteScalar()) demo users."
}
finally {
    $connection.Close()
}
