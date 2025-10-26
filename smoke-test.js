#!/usr/bin/env node

/**
 * WasteNaut Smoke Test Script
 * Tests header functionality, navigation, and basic CRUD operations
 */

const fs = require('fs');
const path = require('path');

// Test configuration
const TEST_CONFIG = {
  baseUrl: 'http://localhost:3000',
  testPages: [
    'index.html',
    'admin-dashboard.html', 
    'organization-foodbank-dashboard.html',
    'login.html'
  ],
  expectedElements: [
    'header-container',
    'navigation-menu',
    'logout-button',
    'notification-area'
  ]
};

class SmokeTester {
  constructor() {
    this.results = {
      passed: 0,
      failed: 0,
      tests: []
    };
  }

  log(message, type = 'info') {
    const timestamp = new Date().toISOString();
    const prefix = type === 'error' ? '❌' : type === 'success' ? '✅' : 'ℹ️';
    console.log(`${prefix} [${timestamp}] ${message}`);
  }

  async runTest(testName, testFunction) {
    try {
      this.log(`Running test: ${testName}`);
      await testFunction();
      this.results.passed++;
      this.results.tests.push({ name: testName, status: 'PASSED' });
      this.log(`Test passed: ${testName}`, 'success');
    } catch (error) {
      this.results.failed++;
      this.results.tests.push({ name: testName, status: 'FAILED', error: error.message });
      this.log(`Test failed: ${testName} - ${error.message}`, 'error');
    }
  }

  // Test 1: Check if header partial exists
  async testHeaderPartialExists() {
    const headerPath = path.join(__dirname, 'frontend', 'html', 'partials', 'header.html');
    if (!fs.existsSync(headerPath)) {
      throw new Error('Header partial not found at expected location');
    }
    
    const headerContent = fs.readFileSync(headerPath, 'utf8');
    if (!headerContent.includes('app-header') || !headerContent.includes('navigation-menu')) {
      throw new Error('Header partial missing required elements');
    }
  }

  // Test 2: Check if CSS variables are defined
  async testCSSVariables() {
    const cssPath = path.join(__dirname, 'frontend', 'resources', 'styles', 'main.css');
    if (!fs.existsSync(cssPath)) {
      throw new Error('Main CSS file not found');
    }
    
    const cssContent = fs.readFileSync(cssPath, 'utf8');
    const requiredVariables = [
      '--app-font-family',
      '--header-bg',
      '--button-border-radius',
      '--notification-bg'
    ];
    
    for (const variable of requiredVariables) {
      if (!cssContent.includes(variable)) {
        throw new Error(`CSS variable ${variable} not found`);
      }
    }
  }

  // Test 3: Check if header JavaScript exists
  async testHeaderJavaScript() {
    const headerJsPath = path.join(__dirname, 'frontend', 'resources', 'scripts', 'header.js');
    if (!fs.existsSync(headerJsPath)) {
      throw new Error('Header JavaScript file not found');
    }
    
    const jsContent = fs.readFileSync(headerJsPath, 'utf8');
    if (!jsContent.includes('HeaderManager') || !jsContent.includes('showNotification')) {
      throw new Error('Header JavaScript missing required functionality');
    }
  }

  // Test 4: Check if pages include header system
  async testPagesIncludeHeader() {
    for (const page of TEST_CONFIG.testPages) {
      const pagePath = path.join(__dirname, 'frontend', 'html', page);
      if (!fs.existsSync(pagePath)) {
        throw new Error(`Page ${page} not found`);
      }
      
      const pageContent = fs.readFileSync(pagePath, 'utf8');
      if (!pageContent.includes('header-container') || !pageContent.includes('header.js')) {
        throw new Error(`Page ${page} missing header system integration`);
      }
    }
  }

  // Test 5: Check authentication system
  async testAuthenticationSystem() {
    const authPath = path.join(__dirname, 'frontend', 'resources', 'scripts', 'auth.js');
    if (!fs.existsSync(authPath)) {
      throw new Error('Authentication system not found');
    }
    
    const authContent = fs.readFileSync(authPath, 'utf8');
    if (!authContent.includes('AuthManager') || !authContent.includes('getNavigationForRole')) {
      throw new Error('Authentication system missing required functionality');
    }
  }

  // Test 6: Check notification system
  async testNotificationSystem() {
    const headerPath = path.join(__dirname, 'frontend', 'resources', 'scripts', 'header.js');
    const headerContent = fs.readFileSync(headerPath, 'utf8');
    
    if (!headerContent.includes('showNotification') || !headerContent.includes('app-notification')) {
      throw new Error('Notification system not properly implemented');
    }
  }

  // Test 7: Check responsive design
  async testResponsiveDesign() {
    const cssPath = path.join(__dirname, 'frontend', 'resources', 'styles', 'main.css');
    const cssContent = fs.readFileSync(cssPath, 'utf8');
    
    if (!cssContent.includes('@media') || !cssContent.includes('app-nav-toggle')) {
      throw new Error('Responsive design not properly implemented');
    }
  }

  // Test 8: Check accessibility features
  async testAccessibilityFeatures() {
    const headerPath = path.join(__dirname, 'frontend', 'html', 'partials', 'header.html');
    const headerContent = fs.readFileSync(headerPath, 'utf8');
    
    const accessibilityFeatures = [
      'aria-label',
      'role=',
      'aria-live',
      'aria-controls'
    ];
    
    for (const feature of accessibilityFeatures) {
      if (!headerContent.includes(feature)) {
        throw new Error(`Accessibility feature ${feature} not found in header`);
      }
    }
  }

  async runAllTests() {
    this.log('Starting WasteNaut Smoke Tests...');
    this.log('='.repeat(50));

    await this.runTest('Header Partial Exists', () => this.testHeaderPartialExists());
    await this.runTest('CSS Variables Defined', () => this.testCSSVariables());
    await this.runTest('Header JavaScript Exists', () => this.testHeaderJavaScript());
    await this.runTest('Pages Include Header System', () => this.testPagesIncludeHeader());
    await this.runTest('Authentication System', () => this.testAuthenticationSystem());
    await this.runTest('Notification System', () => this.testNotificationSystem());
    await this.runTest('Responsive Design', () => this.testResponsiveDesign());
    await this.runTest('Accessibility Features', () => this.testAccessibilityFeatures());

    this.log('='.repeat(50));
    this.log(`Tests completed: ${this.results.passed} passed, ${this.results.failed} failed`);
    
    if (this.results.failed > 0) {
      this.log('Failed tests:', 'error');
      this.results.tests
        .filter(test => test.status === 'FAILED')
        .forEach(test => {
          this.log(`  - ${test.name}: ${test.error}`, 'error');
        });
      process.exit(1);
    } else {
      this.log('All tests passed! 🎉', 'success');
      process.exit(0);
    }
  }
}

// Run tests if this script is executed directly
if (require.main === module) {
  const tester = new SmokeTester();
  tester.runAllTests().catch(error => {
    console.error('Test runner error:', error);
    process.exit(1);
  });
}

module.exports = SmokeTester;
