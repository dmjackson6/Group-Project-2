# WasteNaut Smoke Test Script (PowerShell)
# Tests header functionality, navigation, and basic system integration

Write-Host "🚀 Starting WasteNaut Smoke Tests..." -ForegroundColor Green
Write-Host "=" * 50 -ForegroundColor Cyan

$testResults = @{
    Passed = 0
    Failed = 0
    Tests = @()
}

function Write-TestResult {
    param(
        [string]$TestName,
        [bool]$Passed,
        [string]$ErrorMessage = ""
    )
    
    if ($Passed) {
        Write-Host "✅ Test passed: $TestName" -ForegroundColor Green
        $testResults.Passed++
        $testResults.Tests += @{ Name = $TestName; Status = "PASSED" }
    } else {
        Write-Host "❌ Test failed: $TestName - $ErrorMessage" -ForegroundColor Red
        $testResults.Failed++
        $testResults.Tests += @{ Name = $TestName; Status = "FAILED"; Error = $ErrorMessage }
    }
}

# Test 1: Check if header partial exists
try {
    $headerPath = "frontend\client\partials\header.html"
    if (Test-Path $headerPath) {
        $headerContent = Get-Content $headerPath -Raw
        if ($headerContent -match "app-header" -and $headerContent -match "navigation-menu") {
            Write-TestResult "Header Partial Exists" $true
        } else {
            Write-TestResult "Header Partial Exists" $false "Missing required elements"
        }
    } else {
        Write-TestResult "Header Partial Exists" $false "File not found"
    }
} catch {
    Write-TestResult "Header Partial Exists" $false $_.Exception.Message
}

# Test 2: Check if CSS variables are defined
try {
    $cssPath = "frontend\resources\styles\main.css"
    if (Test-Path $cssPath) {
        $cssContent = Get-Content $cssPath -Raw
        $requiredVariables = @(
            "--app-font-family",
            "--header-bg", 
            "--button-border-radius",
            "--notification-bg"
        )
        
        $allVariablesFound = $true
        foreach ($variable in $requiredVariables) {
            if ($cssContent -notmatch [regex]::Escape($variable)) {
                $allVariablesFound = $false
                break
            }
        }
        
        Write-TestResult "CSS Variables Defined" $allVariablesFound
    } else {
        Write-TestResult "CSS Variables Defined" $false "CSS file not found"
    }
} catch {
    Write-TestResult "CSS Variables Defined" $false $_.Exception.Message
}

# Test 3: Check if header JavaScript exists
try {
    $headerJsPath = "frontend\resources\scripts\header.js"
    if (Test-Path $headerJsPath) {
        $jsContent = Get-Content $headerJsPath -Raw
        if ($jsContent -match "HeaderManager" -and $jsContent -match "showNotification") {
            Write-TestResult "Header JavaScript Exists" $true
        } else {
            Write-TestResult "Header JavaScript Exists" $false "Missing required functionality"
        }
    } else {
        Write-TestResult "Header JavaScript Exists" $false "File not found"
    }
} catch {
    Write-TestResult "Header JavaScript Exists" $false $_.Exception.Message
}

# Test 4: Check if pages include header system
try {
    $testPages = @("index.html", "admin-dashboard.html", "organization-foodbank-dashboard.html", "login.html")
    $allPagesValid = $true
    
    foreach ($page in $testPages) {
        $pagePath = "frontend\html\$page"
        if (Test-Path $pagePath) {
            $pageContent = Get-Content $pagePath -Raw
            if ($pageContent -notmatch "header-container" -or $pageContent -notmatch "header.js") {
                $allPagesValid = $false
                break
            }
        } else {
            $allPagesValid = $false
            break
        }
    }
    
    Write-TestResult "Pages Include Header System" $allPagesValid
} catch {
    Write-TestResult "Pages Include Header System" $false $_.Exception.Message
}

# Test 5: Check authentication system
try {
    $authPath = "frontend\resources\scripts\auth.js"
    if (Test-Path $authPath) {
        $authContent = Get-Content $authPath -Raw
        if ($authContent -match "AuthManager" -and $authContent -match "getNavigationForRole") {
            Write-TestResult "Authentication System" $true
        } else {
            Write-TestResult "Authentication System" $false "Missing required functionality"
        }
    } else {
        Write-TestResult "Authentication System" $false "File not found"
    }
} catch {
    Write-TestResult "Authentication System" $false $_.Exception.Message
}

# Test 6: Check notification system
try {
    $headerPath = "frontend\resources\scripts\header.js"
    if (Test-Path $headerPath) {
        $headerContent = Get-Content $headerPath -Raw
        if ($headerContent -match "showNotification" -and $headerContent -match "app-notification") {
            Write-TestResult "Notification System" $true
        } else {
            Write-TestResult "Notification System" $false "Not properly implemented"
        }
    } else {
        Write-TestResult "Notification System" $false "Header file not found"
    }
} catch {
    Write-TestResult "Notification System" $false $_.Exception.Message
}

# Test 7: Check responsive design
try {
    $cssPath = "frontend\resources\styles\main.css"
    if (Test-Path $cssPath) {
        $cssContent = Get-Content $cssPath -Raw
        if ($cssContent -match "@media" -and $cssContent -match "app-nav-toggle") {
            Write-TestResult "Responsive Design" $true
        } else {
            Write-TestResult "Responsive Design" $false "Not properly implemented"
        }
    } else {
        Write-TestResult "Responsive Design" $false "CSS file not found"
    }
} catch {
    Write-TestResult "Responsive Design" $false $_.Exception.Message
}

# Test 8: Check accessibility features
try {
    $headerPath = "frontend\client\partials\header.html"
    if (Test-Path $headerPath) {
        $headerContent = Get-Content $headerPath -Raw
        $accessibilityFeatures = @("aria-label", "role=", "aria-live", "aria-controls")
        
        $allFeaturesFound = $true
        foreach ($feature in $accessibilityFeatures) {
            if ($headerContent -notmatch [regex]::Escape($feature)) {
                $allFeaturesFound = $false
                break
            }
        }
        
        Write-TestResult "Accessibility Features" $allFeaturesFound
    } else {
        Write-TestResult "Accessibility Features" $false "Header file not found"
    }
} catch {
    Write-TestResult "Accessibility Features" $false $_.Exception.Message
}

# Display results
Write-Host "=" * 50 -ForegroundColor Cyan
Write-Host "Tests completed: $($testResults.Passed) passed, $($testResults.Failed) failed" -ForegroundColor Yellow

if ($testResults.Failed -gt 0) {
    Write-Host "Failed tests:" -ForegroundColor Red
    foreach ($test in $testResults.Tests) {
        if ($test.Status -eq "FAILED") {
            Write-Host "  - $($test.Name): $($test.Error)" -ForegroundColor Red
        }
    }
    exit 1
} else {
    Write-Host "All tests passed! 🎉" -ForegroundColor Green
    exit 0
}
