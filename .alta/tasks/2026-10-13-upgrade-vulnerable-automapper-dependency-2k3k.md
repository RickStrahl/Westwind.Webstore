---
title: "Upgrade vulnerable AutoMapper dependency"
kind: problem
status: pending
created: 2026-10-13
summary: "The web project build reports a known high-severity vulnerability in AutoMapper 13.0.1 (GHSA-rvv3-g6hj-g44x)."
---

## Why
`dotnet build Westwind.Webstore.Web/Westwind.Webstore.Web.csproj --no-restore` succeeds but emits NU1903 for AutoMapper 13.0.1 and links GHSA-rvv3-g6hj-g44x.

## What to do
Identify the nearest compatible patched AutoMapper version, upgrade it using the existing NuGet workflow, and run the relevant build/tests.

## Where
NuGet package references for the web/business projects and their dependency lock/restore state.
