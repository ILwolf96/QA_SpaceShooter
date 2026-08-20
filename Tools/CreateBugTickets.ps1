# CreateBugTickets.ps1
# Creates the 9 grouped QA bug tickets in GitHub Issues.
#
# Prerequisites:
#   1. Install GitHub CLI: https://cli.github.com/
#   2. Authenticate once:
#        gh auth login
#   3. Run this script from PowerShell:
#        .\CreateBugTickets.ps1
#
# By default this targets:
#   ILwolf96/QA_SpaceShooter
#
# The script creates required labels if they do not already exist.
# Existing tickets with the same titles are NOT duplicated.

$ErrorActionPreference = "Stop"

$Repository = "ILwolf96/QA_SpaceShooter"

# ------------------------------------------------------------
# Verify GitHub CLI
# ------------------------------------------------------------

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error "GitHub CLI (gh) was not found. Install it from https://cli.github.com/ and try again."
    exit 1
}

# ------------------------------------------------------------
# Verify authentication
# ------------------------------------------------------------

gh auth status --hostname github.com | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Error "GitHub CLI is not authenticated. Run 'gh auth login' first."
    exit 1
}

# ------------------------------------------------------------
# Labels
# ------------------------------------------------------------

$labels = @(
    @{
        Name = "bug"
        Color = "d73a4a"
        Description = "Confirmed or observed defect/failure"
    },
    @{
        Name = "automated-test"
        Color = "1d76db"
        Description = "Detected by an automated test"
    },
    @{
        Name = "baseline"
        Color = "6f42c1"
        Description = "Pre-existing baseline finding"
    },
    @{
        Name = "regression"
        Color = "b60205"
        Description = "Regression-related finding"
    },
    @{
        Name = "compatibility"
        Color = "fbca04"
        Description = "Cross-platform compatibility finding"
    },
    @{
        Name = "webgl"
        Color = "5319e7"
        Description = "WebGL-specific finding"
    },
    @{
        Name = "android"
        Color = "0e8a16"
        Description = "Android/mobile-specific finding"
    }
)

Write-Host ""
Write-Host "Ensuring GitHub labels exist..." -ForegroundColor Cyan

foreach ($label in $labels) {
    $existing = gh label list `
        --repo $Repository `
        --search $label.Name `
        --limit 100 `
        --json name `
        --jq ".[] | select(.name == `"$($label.Name)`") | .name" 2>$null

    if ($existing -ne $label.Name) {
        Write-Host "Creating label: $($label.Name)"
        gh label create $label.Name `
            --repo $Repository `
            --color $label.Color `
            --description $label.Description `
            --force | Out-Null
    }
    else {
        Write-Host "Label already exists: $($label.Name)"
    }
}

# ------------------------------------------------------------
# Helper: create issue only if title does not already exist
# ------------------------------------------------------------

function New-QABugIssue {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Title,

        [Parameter(Mandatory = $true)]
        [string]$Body,

        [Parameter(Mandatory = $true)]
        [string[]]$Labels
    )

    $existingIssueNumber = gh issue list `
        --repo $Repository `
        --state all `
        --search "`"$Title`" in:title" `
        --limit 100 `
        --json number,title `
        --jq ".[] | select(.title == `"$Title`") | .number" 2>$null

    if ($existingIssueNumber) {
        Write-Host "SKIP: Issue already exists #$existingIssueNumber - $Title" -ForegroundColor Yellow
        return
    }

    $labelArguments = @()
    foreach ($label in $Labels) {
        $labelArguments += @("--label", $label)
    }

    Write-Host "CREATE: $Title" -ForegroundColor Green

    gh issue create `
        --repo $Repository `
        --title $Title `
        @labelArguments `
        --body $Body

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to create GitHub Issue: $Title"
    }
}

# ------------------------------------------------------------
# BUG-001
# ------------------------------------------------------------

New-QABugIssue `
    -Title "BUG-001 Player shooting projectile count mismatch" `
    -Labels @("bug", "automated-test", "baseline") `
    -Body @"
## Bug ID
BUG-001

## Summary
PlayerShooting automated tests report more projectiles than the expected weapon-power behavior.

## Affected Test Cases
- SHO-UT-001
- SHO-UT-002
- SHO-UT-003
- SHO-UT-004

## Environment
- Unity 6.0.43f1
- PlayMode automated testing

## Expected Result
- Weapon Power 1 -> 1 projectile
- Weapon Power 2 -> 2 projectiles
- Weapon Power 3 -> 3 projectiles
- Weapon Power 4 -> 6 projectiles

## Actual Result
- Weapon Power 1 -> 2 projectiles
- Weapon Power 2 -> 4 projectiles
- Weapon Power 3 -> 6 projectiles
- Weapon Power 4 -> 10 projectiles

## Evidence
Final automated regression run:
124 total tests
109 passed
15 failed

## Notes
This was already observed in the baseline automated testing before the Shield/Boss/Level 2 feature work. It is therefore recorded as a baseline finding rather than a new Level 2 regression.

## Status
Open
"@

# ------------------------------------------------------------
# BUG-002
# ------------------------------------------------------------

New-QABugIssue `
    -Title "BUG-002 LevelController planet spawning test failure" `
    -Labels @("bug", "automated-test", "baseline") `
    -Body @"
## Bug ID
BUG-002

## Summary
The LevelController planet-speed functional test does not detect a spawned planet.

## Affected Test Case
- LVL-UT-005

## Environment
- Unity 6.0.43f1
- PlayMode automated testing

## Expected Result
LevelController should spawn a planet and apply the configured planet speed.

## Actual Result
The test reported:
"LevelController should spawn a planet. Expected: not null But was: null"

## Evidence
Final automated regression run.

## Notes
Recorded as a QA finding. No repair is required under the current assignment scope.

## Status
Open
"@

# ------------------------------------------------------------
# BUG-003
# ------------------------------------------------------------

New-QABugIssue `
    -Title "BUG-003 LevelController wave start delay test mismatch" `
    -Labels @("bug", "automated-test", "baseline") `
    -Body @"
## Bug ID
BUG-003

## Summary
The LevelController delayed-wave functional test detected a wave before the configured delay elapsed.

## Affected Test Case
- LVL-UT-002

## Environment
- Unity 6.0.43f1
- PlayMode automated testing

## Expected Result
A wave with a configured start delay should not exist before that delay expires.

## Actual Result
The test reported:
"A delayed wave should not have been created immediately. Expected: False But was: True"

## Evidence
Final automated regression run.

## Notes
Recorded as a QA finding. No repair is required under the current assignment scope.

## Status
Open
"@

# ------------------------------------------------------------
# BUG-004
# ------------------------------------------------------------

New-QABugIssue `
    -Title "BUG-004 Existing Level 1 regression test throws MissingReferenceException" `
    -Labels @("bug", "regression", "automated-test", "baseline") `
    -Body @"
## Bug ID
BUG-004

## Summary
The existing Level 1 regression test accesses a GameObject after it has been destroyed.

## Affected Test Case
- LVL-RT-001

## Environment
- Unity 6.0.43f1
- PlayMode automated testing

## Expected Result
The existing Level 1 configuration should remain functional without accessing destroyed objects.

## Actual Result
The test throws:
UnityEngine.MissingReferenceException

The stack trace indicates a destroyed GameObject is accessed through GetComponent() in LevelControllerFunctionalTests.cs.

## Evidence
Final automated regression run.

## Notes
This is a regression test finding. It is recorded for the final QA package and is not being repaired under the current assignment scope.

## Status
Open
"@

# ------------------------------------------------------------
# BUG-005
# ------------------------------------------------------------

New-QABugIssue `
    -Title "BUG-005 Wave configuration values are not propagated as expected" `
    -Labels @("bug", "automated-test", "baseline") `
    -Body @"
## Bug ID
BUG-005

## Summary
Multiple Wave integration tests report configuration values that differ from the expected values.

## Affected Test Cases
- WAV-UT-003
- WAV-UT-005
- WAV-UT-006
- WAV-UT-007

## Environment
- Unity 6.0.43f1
- PlayMode automated testing

## Observed Mismatches
- Speed: expected 42, actual 25
- Rotation by path: expected true, actual false
- Loop: expected false, actual true
- Shot chance: expected 65, actual 75

## Evidence
Final automated regression run.

## Notes
These failures were present in baseline Wave testing. They are grouped into one tracking ticket because they concern Wave configuration propagation.

## Status
Open
"@

# ------------------------------------------------------------
# BUG-006
# ------------------------------------------------------------

New-QABugIssue `
    -Title "BUG-006 Wave usability validation throws NullReferenceException" `
    -Labels @("bug", "automated-test", "baseline") `
    -Body @"
## Bug ID
BUG-006

## Summary
The Wave usability test throws a NullReferenceException while validating Wave configuration fields.

## Affected Test Case
- UsabilityTests.Wave_HasValidConfigurationFields

## Environment
- Unity 6.0.43f1
- PlayMode automated testing

## Expected Result
Wave configuration validation should complete without an unhandled exception.

## Actual Result
NullReferenceException was thrown from UsabilityTests.cs.

## Evidence
Final automated regression run.

## Notes
Recorded as a non-functional usability-test finding. No repair is required under the current assignment scope.

## Status
Open
"@

# ------------------------------------------------------------
# BUG-007
# ------------------------------------------------------------

New-QABugIssue `
    -Title "BUG-007 VisualEffect functional tests throw MissingReferenceException" `
    -Labels @("bug", "automated-test", "baseline") `
    -Body @"
## Bug ID
BUG-007

## Summary
All three VisualEffect lifecycle tests access a GameObject after it has been destroyed.

## Affected Test Cases
- VFX-UT-001
- VFX-UT-002
- VFX-UT-003

## Environment
- Unity 6.0.43f1
- PlayMode automated testing

## Expected Result
VisualEffect lifecycle tests should be able to verify enable, active duration, and destruction behavior without throwing exceptions.

## Actual Result
The tests throw UnityEngine.MissingReferenceException when attempting to access the destroyed GameObject.

## Evidence
Final automated regression run.

## Notes
Grouped because all three failures share the same destroyed-GameObject access symptom.

## Status
Open
"@

# ------------------------------------------------------------
# BUG-008
# ------------------------------------------------------------

New-QABugIssue `
    -Title "BUG-008 WebGL runtime gameplay does not function" `
    -Labels @("bug", "compatibility", "webgl") `
    -Body @"
## Bug ID
BUG-008

## Summary
The WebGL build launches but gameplay logic does not operate correctly.

## Affected Test Cases
- WEB-FT-002
- WEB-FT-003

## Platform
WebGL

## Expected Result
The WebGL build should provide normal gameplay, including player movement, enemy spawning, and Level 2/Shield/Boss functionality.

## Actual Result
The WebGL build launched, but:
- Player movement did not operate.
- Enemy spawning/gameplay logic did not operate.
- Level 2/Shield/Boss gameplay could not be completed because core gameplay was unavailable.

## Evidence
Manual WebGL compatibility testing.

## Notes
The assignment requires performing cross-platform testing. Repairing WebGL compatibility is not required by the current scope.

## Status
Open
"@

# ------------------------------------------------------------
# BUG-009
# ------------------------------------------------------------

New-QABugIssue `
    -Title "BUG-009 Android runtime gameplay does not function" `
    -Labels @("bug", "compatibility", "android") `
    -Body @"
## Bug ID
BUG-009

## Summary
The Android build launches successfully, but gameplay logic does not operate correctly.

## Affected Test Cases
- MOB-FT-002
- MOB-FT-003
- MOB-FT-004

## Platform
Android

## Expected Result
The Android build should provide working touch controls and core gameplay, including Level 2/Shield/Boss functionality.

## Actual Result
- Android build completed successfully.
- Game launched successfully.
- Touch movement did not work because gameplay logic did not operate.
- Core gameplay did not operate.
- Level 2/Shield/Boss testing could not be completed because gameplay logic failed.

## Evidence
Manual Android compatibility testing.

## Notes
The assignment requires performing cross-platform testing. Repairing Android compatibility is not required by the current scope.

## Status
Open
"@

Write-Host ""
Write-Host "Bug ticket creation complete." -ForegroundColor Cyan
Write-Host "Repository: $Repository"
Write-Host "Review the Issues page for the 9 grouped QA tickets." -ForegroundColor Green
