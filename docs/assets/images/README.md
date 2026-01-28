# Logo and Favicon Assets

This directory contains branding assets for the documentation site.

## Required Files

### Logo
- **File**: `datadog-maui-logo.png`
- **Recommended size**: 200-400px wide
- **Format**: PNG with transparent background
- **Usage**: Appears in the top-left of the documentation site

### Favicon
- **File**: `favicon.ico`
- **Size**: 32x32px (or 16x16px)
- **Format**: ICO or PNG
- **Usage**: Browser tab icon

## Where to Get Datadog Branding

You can download official Datadog logos from:
- [Datadog Brand Assets](https://www.datadoghq.com/about/press/)
- Internal Datadog branding resources

## Temporary Placeholder

If you don't have the official logo yet, you can:

1. Use the Datadog dog icon
2. Create a simple text logo with "DD MAUI"
3. Use a placeholder service like https://placeholder.com/ to generate a temporary logo

## Adding Your Logo

1. Save your logo as `datadog-maui-logo.png` in this directory
2. Save your favicon as `favicon.ico` in this directory
3. The Jekyll build will automatically pick them up from the `_config.base.yml` configuration

## Current Configuration

The site is configured to use:
- Logo: `/assets/images/datadog-maui-logo.png`
- Favicon: `/assets/images/favicon.ico`

If these files don't exist, the site will fall back to displaying the site title text.
