<#
PowerShell helper: build_and_package.ps1
- Builds the solution using dotnet build, and creates a zip package containing the built DLL and README.
- Usage: Run in an environment where .NET SDK is installed and dotnet is on PATH.
#>

param(
  [string]$Solution = "TeddyRhinoPlugin.sln",
  [string]$Configuration = "Debug",
  [string]$OutDir = "bin\$($Configuration)",
  [string]$PackageName = "TeddyRhinoPlugin-DevPackage.zip"
)

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) {
  Write-Error "dotnet not found. Install .NET SDK."
  exit 1
}

$dotnetPath = $dotnet.Path
Write-Host "Using dotnet: $dotnetPath"
& $dotnetPath build -c $Configuration $Solution
if ($LASTEXITCODE -ne 0) { Write-Error "Build failed with exit code $LASTEXITCODE"; exit $LASTEXITCODE }

$binPath = Join-Path -Path (Get-Location) -ChildPath $OutDir
if (-not (Test-Path $binPath)) { Write-Error "Build succeeded but output directory not found: $binPath"; exit 1 }

$packagePath = Join-Path -Path (Get-Location) -ChildPath $PackageName
Write-Host "Creating package: $packagePath"

# Copy README and LICENSE to binPath for inclusion in zip
Copy-Item -Path (Join-Path (Get-Location) 'README.md') -Destination $binPath -Force
Copy-Item -Path (Join-Path (Get-Location) 'LICENSE') -Destination $binPath -Force

if (Test-Path $packagePath) { Remove-Item $packagePath -Force }

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($binPath, $packagePath)

Write-Host "Package created: $packagePath"
Write-Host "Done."
