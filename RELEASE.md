# GitHub Actions Releases

Using Github Actions, releases are automatically created when a new tag is pushed.

## How to create a release

Create and push a tag:

   ```powershell
   git tag v1.0.0
   git push origin v1.0.0
   ```

A release will be automatically created by the GitHub Actions workflow.

(This document is mainly just for that I won't forget)