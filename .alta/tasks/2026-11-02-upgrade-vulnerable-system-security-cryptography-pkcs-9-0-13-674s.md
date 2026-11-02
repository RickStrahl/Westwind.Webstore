---
title: "Upgrade vulnerable System.Security.Cryptography.Pkcs 9.0.13"
kind: problem
status: later
created: 2026-11-02
summary: "Builds report known high and moderate severity vulnerabilities in System.Security.Cryptography.Pkcs 9.0.13 (GHSA-846f-f764-76mx, GHSA-2fmv-rv2c-59x7)."
---

## Why
`dotnet build Westwind.Webstore.Web/Westwind.Webstore.Web.csproj` emits NU1903/NU1902 warnings for `System.Security.Cryptography.Pkcs` 9.0.13 in both the Business and Web projects (GHSA-846f-f764-76mx high, GHSA-2fmv-rv2c-59x7 moderate).

## What to do
Find which package pulls in Pkcs 9.0.13 (`dotnet list package --include-transitive --vulnerable`) and upgrade it, or add a direct reference to a patched Pkcs version; rebuild and run tests.

## Where
`Westwind.Webstore.Business/Westwind.Webstore.Business.csproj`, `Westwind.Webstore.Web/Westwind.Webstore.Web.csproj`
