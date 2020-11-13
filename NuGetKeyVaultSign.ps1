[CmdletBinding()]
param (
    [string]$FilePath,
    [string]$AzureKeyVaultUrl = $Env:AzureKeyVaultUrl ,
    [string]$AzureKeyVaultTenantId = $Env:AzureKeyVaultTenantId,
    [string]$AzureKeyVaultClientId = $Env:AzureKeyVaultClientId,
    [string]$AzureKeyVaultClientSecret = $Env:AzureKeyVaultClientSecret,
    [string]$AzureKeyVaultCertificate = "JosephGuadagno-2021"
)

Write-Host ("##[group]Sign Tool")
$files = Get-ChildItem -Path $FilePath -Recurse

foreach ($file in $files) {
    
    Write-Host("##[debug]Signing file: " + $file.FullName)

    .\NuGetKeyVaultSignTool sign $file `
    --file-digest "sha256" `
    --timestamp-rfc3161 "http://timestamp.digicert.com" `
    --timestamp-digest "sha256" `
    --azure-key-vault-url $AzureKeyVaultUrl `
    --azure-key-vault-tenant-id $AzureKeyVaultTenantId `
    --azure-key-vault-client-id $AzureKeyVaultClientId `
    --azure-key-vault-client-secret $AzureKeyVaultClientSecret `
    --azure-key-vault-certificate $AzureKeyVaultCertificate
}
Write-Host ("##[endgroup]")