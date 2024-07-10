[CmdletBinding()]
param (
    [string]$FilePath,
    [string]$AzureKeyVaultUrl = $Env:PremiumAzureKeyVaultUrl ,
    [string]$AzureKeyVaultTenantId = $Env:PremiumAzureKeyVaultTenantId,
    [string]$AzureKeyVaultClientId = $Env:PremiumAzureKeyVaultClientId,
    [string]$AzureKeyVaultClientSecret = $Env:PremiumAzureKeyVaultClientSecret,
    [string]$AzureKeyVaultCertificate = "EVCodeSigning-2024"
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