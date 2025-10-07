# NuGet Trusted Publishing Setup

This repository uses **Trusted Publishing** for secure NuGet package publishing. This approach eliminates the need for long-lived API keys and uses short-lived tokens issued by GitHub Actions via OpenID Connect (OIDC).

## What is Trusted Publishing?

Trusted Publishing is a security feature on NuGet.org that allows CI/CD systems to publish packages without storing long-lived API keys as secrets. Instead:

1. GitHub Actions requests an OIDC token when running the workflow
2. The token is sent to NuGet.org for validation
3. NuGet.org issues a short-lived API key (valid for ~1 hour) if the token matches a configured trusted publisher
4. The workflow uses this temporary key to publish the package

## Benefits

- **Enhanced Security**: No long-lived secrets stored in repository settings
- **Automated Credential Management**: Tokens are automatically issued and expire quickly
- **Reduced Risk**: Even if a token is leaked, it expires within an hour
- **Compliance**: Aligns with security best practices

## Configuration Required on NuGet.org

To enable Trusted Publishing for this repository, a package owner must configure a trusted publisher on NuGet.org:

### Steps:

1. **Log in to NuGet.org** with an account that has ownership of the package
2. **Navigate to the package** you want to configure (e.g., `EnergiDataService.Client`)
3. **Go to the Trusted Publishers section** in the package management page
4. **Add a new trusted publisher** with these settings:
   - **Provider**: GitHub Actions
   - **Repository Owner**: `christianhelle`
   - **Repository Name**: `edsclient`
   - **Workflow File**: `.github/workflows/release.yml`
   - **Environment** (optional): `production`

### Important Notes:

- The workflow file path must exactly match the one in the repository
- If using GitHub environments (like `production` in this repo), include it in the configuration
- Multiple trusted publishers can be configured for different workflows or environments
- The package must already exist on NuGet.org before configuring trusted publishing

## Workflow Configuration

The release workflow (`.github/workflows/release.yml`) has been configured with:

```yaml
permissions:
  id-token: write  # Required for OIDC token requests
  contents: read   # Required for repository checkout
```

The publish step no longer uses `--api-key`:

```yaml
- name: Publish to NuGet.org
  run: dotnet nuget push ./artifacts/*.nupkg --source https://api.nuget.org/v3/index.json --skip-duplicate
```

## Testing the Setup

After configuring the trusted publisher on NuGet.org:

1. Create a new release or manually trigger the workflow
2. Monitor the workflow run in GitHub Actions
3. The "Publish to NuGet.org" step should succeed without requiring `NUGET_API_KEY`
4. If there are issues, check that:
   - The trusted publisher configuration matches exactly
   - The workflow has the correct permissions
   - The package already exists on NuGet.org

## Troubleshooting

### Common Issues:

**Error: "Unable to obtain a publish key"**
- Verify the trusted publisher is configured correctly on NuGet.org
- Ensure the repository owner, name, and workflow file path match exactly
- Check that the GitHub environment name matches if one is used

**Error: "Unauthorized"**
- The OIDC token may not be properly configured
- Verify `id-token: write` permission is set in the workflow
- Ensure the workflow is running from the correct repository and branch

**First-time Setup**
- The package must exist on NuGet.org before trusted publishing can be used
- For the first publish, use a traditional API key, then configure trusted publishing for subsequent releases

## References

- [Official NuGet Trusted Publishing Documentation](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing)
- [GitHub OIDC Documentation](https://docs.github.com/en/actions/deployment/security-hardening-your-deployments/about-security-hardening-with-openid-connect)
- [NuGet Blog: Trusted Publishing Launch](https://devblogs.microsoft.com/dotnet/enhanced-security-is-here-with-the-new-trust-publishing-on-nuget-org/)

## Migration Notes

**Previous Configuration:**
- Required `NUGET_API_KEY` secret in repository settings
- API key was long-lived and needed manual rotation

**New Configuration:**
- No secrets required in repository settings
- Automatic token issuance and expiration
- Configuration is done on NuGet.org package settings

The `NUGET_API_KEY` secret can be safely removed from GitHub repository settings once trusted publishing is confirmed working.
