# Releasing

A release is a tag on `master`. The tag starts a workflow that builds, tests and packs the NuGet package. The workflow does not publish it: you download it from GitHub and upload it to nuget.org yourself.

## Steps

1. **Set the version.** It is `<Version>` in `src/Zametek.Utility/Zametek.Utility.csproj`.

2. **Merge `develop` into `master`** with a pull request. CI runs on it, and only `develop` can be the source of a pull request to `master`.

3. **Tag the merge on `master`**, and push the tag:

   ```bash
   git switch master
   git pull
   git tag v3.0.1
   git push origin v3.0.1
   ```

4. **Wait for the Release workflow** (Actions tab). It stops with an error in these conditions:

   - The tag is not `v` plus the version in the project file.
   - The commit of a release tag is not on `master`. A tag with a suffix, such as `v3.1.0-rc.1`, can come from any branch.
   - A test fails.
   - The API of the package is different from the newest earlier release of the same major version on nuget.org (see below).

5. **Download the artifact** `nuget-packages` from the run (it is kept for 30 days) and unzip it. It holds two files: the `.nupkg` and the `.snupkg`. The summary of the run gives their SHA-256 hashes.

6. **Upload the package.** `dotnet nuget push` uploads the `.snupkg` with the `.nupkg` that is in the same folder:

   ```bash
   dotnet nuget push Zametek.Utility.3.0.1.nupkg --api-key <key> --source https://api.nuget.org/v3/index.json
   ```

## Dry run

To test the packing without a tag: Actions, Release, Run workflow, and choose a branch. The checks of the tag are skipped, and the summary of the run says that it is a dry run. The workflow file must be on the default branch (`develop`) for the button to appear.

## The API check

Every CI build and the Release workflow pack the package with `.github/scripts/pack.sh`. The script finds the newest earlier release of the same major version on nuget.org, and package validation compares the new API with it. A removed or changed public member fails the build (error `CP0002`).

To break the API on purpose, do one of two things:

- Raise the major version. A new major version has nothing to be compared with, and a first release has no earlier release at all.
- Record the break in `src/Zametek.Utility/CompatibilitySuppressions.xml`. The version number then stays. Set the new version in the project file first. Then generate the entry from the root of the repository, with the newest earlier release as the baseline:

  ```bash
  dotnet pack src/Zametek.Utility --configuration Release --output /tmp/packages \
    -p:EnablePackageValidation=true -p:PackageValidationBaselineVersion=3.0.0 \
    -p:ApiCompatGenerateSuppressionFile=true
  ```

  After the release, the entry does nothing, because the next release compares with a package that already has the change. Delete it then.

## Pins

Nothing in the workflows moves by itself:

- The actions are pinned to commit SHAs, and Dependabot proposes the updates (weekly, as pull requests against `develop`). It does the same for the NuGet packages.
- The .NET SDK is pinned in `ci.yml` and `release.yml`. Bump the two together. `global.json` keeps `dotnet` on .NET 10, because the runner image can also have a later SDK.
- The runners are `ubuntu-24.04`, not `ubuntu-latest`.
