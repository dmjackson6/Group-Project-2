-- WasteNaut Admin Complete Database Setup
-- Combined DDL and seed data for MySQL

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- Drop tables if they exist (for clean setup)
DROP TABLE IF EXISTS `audit_logs`;
DROP TABLE IF EXISTS `match_notes`;
DROP TABLE IF EXISTS `report_notes`;
DROP TABLE IF EXISTS `report_evidence`;
DROP TABLE IF EXISTS `matches`;
DROP TABLE IF EXISTS `reports`;
DROP TABLE IF EXISTS `donations`;
DROP TABLE IF EXISTS `organizations`;
DROP TABLE IF EXISTS `users`;
DROP TABLE IF EXISTS `admins`;
DROP TABLE IF EXISTS `roles`;
DROP TABLE IF EXISTS `notification_templates`;

-- Create roles table
CREATE TABLE `roles` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `name` VARCHAR(50) NOT NULL UNIQUE,
  `description` TEXT,
  `permissions` JSON,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create admins table
CREATE TABLE `admins` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `name` VARCHAR(100) NOT NULL,
  `email` VARCHAR(255) NOT NULL UNIQUE,
  `password_hash` VARCHAR(255) NOT NULL,
  `role_id` INT NOT NULL,
  `status` ENUM('active', 'inactive', 'suspended') DEFAULT 'active',
  `last_login` TIMESTAMP NULL,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX `idx_email` (`email`),
  INDEX `idx_role` (`role_id`),
  INDEX `idx_status` (`status`),
  FOREIGN KEY (`role_id`) REFERENCES `roles`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create users table
CREATE TABLE `users` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `name` VARCHAR(100) NOT NULL,
  `email` VARCHAR(255) NOT NULL UNIQUE,
  `password_hash` VARCHAR(255) NOT NULL,
  `role` ENUM('individual', 'organization', 'donor') NOT NULL,
  `status` ENUM('verified', 'pending', 'suspended') DEFAULT 'pending',
  `phone` VARCHAR(20),
  `address` TEXT,
  `preferences` JSON,
  `organization_id` INT NULL,
  `suspension_reason` TEXT NULL,
  `last_active` TIMESTAMP NULL,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX `idx_email` (`email`),
  INDEX `idx_role` (`role`),
  INDEX `idx_status` (`status`),
  INDEX `idx_organization` (`organization_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create organizations table
CREATE TABLE `organizations` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `name` VARCHAR(200) NOT NULL,
  `type` ENUM('foodbank', 'charity', 'restaurant') NOT NULL,
  `status` ENUM('approved', 'pending', 'rejected') DEFAULT 'pending',
  `registration_number` VARCHAR(50) UNIQUE,
  `address` TEXT NOT NULL,
  `contact_name` VARCHAR(100) NOT NULL,
  `contact_email` VARCHAR(255) NOT NULL,
  `contact_phone` VARCHAR(20),
  `capacity_max` INT DEFAULT 0,
  `capacity_used` INT DEFAULT 0,
  `capacity_notes` TEXT,
  `service_areas` JSON,
  `description` TEXT,
  `approved_at` TIMESTAMP NULL,
  `rejected_at` TIMESTAMP NULL,
  `rejection_reason` TEXT,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX `idx_name` (`name`),
  INDEX `idx_type` (`type`),
  INDEX `idx_status` (`status`),
  INDEX `idx_registration` (`registration_number`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Add foreign key constraint for users.organization_id
ALTER TABLE `users` 
ADD CONSTRAINT `fk_users_organization` 
FOREIGN KEY (`organization_id`) REFERENCES `organizations`(`id`) ON DELETE SET NULL;

-- Create donations table
CREATE TABLE `donations` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `title` VARCHAR(200) NOT NULL,
  `description` TEXT,
  `type` ENUM('food', 'equipment', 'volunteer') NOT NULL,
  `status` ENUM('active', 'completed', 'expired', 'cancelled') DEFAULT 'active',
  `quantity` INT NOT NULL,
  `unit` VARCHAR(50) NOT NULL,
  `donor_id` INT NOT NULL,
  `recipient_id` INT NULL,
  `location_address` TEXT,
  `location_lat` DECIMAL(10, 8),
  `location_lng` DECIMAL(11, 8),
  `pickup_window_start` TIMESTAMP NULL,
  `pickup_window_end` TIMESTAMP NULL,
  `expires_at` TIMESTAMP NULL,
  `completed_at` TIMESTAMP NULL,
  `tags` JSON,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX `idx_donor` (`donor_id`),
  INDEX `idx_recipient` (`recipient_id`),
  INDEX `idx_status` (`status`),
  INDEX `idx_type` (`type`),
  INDEX `idx_expires` (`expires_at`),
  FOREIGN KEY (`donor_id`) REFERENCES `users`(`id`) ON DELETE CASCADE,
  FOREIGN KEY (`recipient_id`) REFERENCES `users`(`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create reports table
CREATE TABLE `reports` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `title` VARCHAR(200) NOT NULL,
  `description` TEXT NOT NULL,
  `type` ENUM('safety', 'fraud', 'inappropriate', 'technical') NOT NULL,
  `priority` ENUM('low', 'medium', 'high', 'critical') DEFAULT 'medium',
  `status` ENUM('pending', 'investigating', 'resolved', 'dismissed') DEFAULT 'pending',
  `reporter_id` INT NOT NULL,
  `reported_user_id` INT NULL,
  `assigned_admin_id` INT NULL,
  `resolved_at` TIMESTAMP NULL,
  `resolution_notes` TEXT,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX `idx_reporter` (`reporter_id`),
  INDEX `idx_reported_user` (`reported_user_id`),
  INDEX `idx_assigned_admin` (`assigned_admin_id`),
  INDEX `idx_status` (`status`),
  INDEX `idx_priority` (`priority`),
  INDEX `idx_type` (`type`),
  FOREIGN KEY (`reporter_id`) REFERENCES `users`(`id`) ON DELETE CASCADE,
  FOREIGN KEY (`reported_user_id`) REFERENCES `users`(`id`) ON DELETE SET NULL,
  FOREIGN KEY (`assigned_admin_id`) REFERENCES `admins`(`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create report_evidence table
CREATE TABLE `report_evidence` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `report_id` INT NOT NULL,
  `type` ENUM('text', 'image', 'document', 'video') NOT NULL,
  `content` TEXT NOT NULL,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  INDEX `idx_report` (`report_id`),
  FOREIGN KEY (`report_id`) REFERENCES `reports`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create report_notes table
CREATE TABLE `report_notes` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `report_id` INT NOT NULL,
  `author` VARCHAR(100) NOT NULL,
  `text` TEXT NOT NULL,
  `type` ENUM('internal', 'public') DEFAULT 'internal',
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  INDEX `idx_report` (`report_id`),
  FOREIGN KEY (`report_id`) REFERENCES `reports`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create matches table
CREATE TABLE `matches` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `title` VARCHAR(200) NOT NULL,
  `type` ENUM('donation', 'volunteer', 'request') NOT NULL,
  `confidence` INT NOT NULL CHECK (`confidence` >= 0 AND `confidence` <= 100),
  `status` ENUM('pending', 'accepted', 'rejected', 'expired') DEFAULT 'pending',
  `from_user_id` INT NOT NULL,
  `to_user_id` INT NOT NULL,
  `ai_notes` TEXT,
  `factors` JSON,
  `override_reason` TEXT NULL,
  `override_notes` TEXT NULL,
  `override_confidence` INT NULL,
  `expires_at` TIMESTAMP NULL,
  `accepted_at` TIMESTAMP NULL,
  `rejected_at` TIMESTAMP NULL,
  `rejection_reason` TEXT NULL,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX `idx_from_user` (`from_user_id`),
  INDEX `idx_to_user` (`to_user_id`),
  INDEX `idx_status` (`status`),
  INDEX `idx_confidence` (`confidence`),
  INDEX `idx_type` (`type`),
  INDEX `idx_expires` (`expires_at`),
  FOREIGN KEY (`from_user_id`) REFERENCES `users`(`id`) ON DELETE CASCADE,
  FOREIGN KEY (`to_user_id`) REFERENCES `users`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create match_notes table
CREATE TABLE `match_notes` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `match_id` INT NOT NULL,
  `author` VARCHAR(100) NOT NULL,
  `text` TEXT NOT NULL,
  `type` ENUM('internal', 'public') DEFAULT 'internal',
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  INDEX `idx_match` (`match_id`),
  FOREIGN KEY (`match_id`) REFERENCES `matches`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create audit_logs table
CREATE TABLE `audit_logs` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `action` VARCHAR(100) NOT NULL,
  `user` VARCHAR(100) NOT NULL,
  `user_id` INT NULL,
  `target` VARCHAR(200) NOT NULL,
  `target_id` INT NULL,
  `details` TEXT,
  `ip_address` VARCHAR(45),
  `user_agent` TEXT,
  `timestamp` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  INDEX `idx_action` (`action`),
  INDEX `idx_user` (`user`),
  INDEX `idx_user_id` (`user_id`),
  INDEX `idx_target` (`target`),
  INDEX `idx_target_id` (`target_id`),
  INDEX `idx_timestamp` (`timestamp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create notification_templates table
CREATE TABLE `notification_templates` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `name` VARCHAR(100) NOT NULL,
  `description` TEXT,
  `type` ENUM('email', 'sms', 'push') NOT NULL,
  `status` ENUM('active', 'inactive') DEFAULT 'active',
  `subject` VARCHAR(200),
  `content` TEXT NOT NULL,
  `variables` JSON,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX `idx_type` (`type`),
  INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create indexes for better performance
CREATE INDEX `idx_donations_created` ON `donations`(`created_at`);
CREATE INDEX `idx_reports_created` ON `reports`(`created_at`);
CREATE INDEX `idx_matches_created` ON `matches`(`created_at`);
CREATE INDEX `idx_users_created` ON `users`(`created_at`);
CREATE INDEX `idx_organizations_created` ON `organizations`(`created_at`);

-- Create full-text search indexes
CREATE FULLTEXT INDEX `ft_users_name_email` ON `users`(`name`, `email`);
CREATE FULLTEXT INDEX `ft_organizations_name` ON `organizations`(`name`);
CREATE FULLTEXT INDEX `ft_donations_title_description` ON `donations`(`title`, `description`);
CREATE FULLTEXT INDEX `ft_reports_title_description` ON `reports`(`title`, `description`);

-- Insert roles
INSERT INTO `roles` (`id`, `name`, `description`, `permissions`) VALUES
(1, 'Super Admin', 'Full system access with all permissions', '["all"]'),
(2, 'Moderator', 'User and content moderation permissions', '["users", "reports", "content"]'),
(3, 'Support', 'Customer support and user assistance', '["users", "tickets"]');

-- Insert admin users
-- Password for all test accounts: admin123
INSERT INTO `admins` (`id`, `name`, `email`, `password_hash`, `role_id`, `status`, `last_login`) VALUES
(1, 'Admin User', 'admin@wastenaut.test', '$2b$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj4J8K8K8K8K', 1, 'active', '2024-01-20T10:30:00Z'),
(2, 'Moderator User', 'moderator@wastenaut.test', '$2b$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj4J8K8K8K8K', 2, 'active', '2024-01-19T16:45:00Z'),
(3, 'Support User', 'support@wastenaut.test', '$2b$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj4J8K8K8K8K', 3, 'active', '2024-01-18T14:20:00Z');

-- Insert organizations
INSERT INTO `organizations` (`id`, `name`, `type`, `status`, `registration_number`, `address`, `contact_name`, `contact_email`, `contact_phone`, `capacity_max`, `capacity_used`, `capacity_notes`, `service_areas`, `description`, `approved_at`) VALUES
(1, 'Green Earth Foundation', 'charity', 'approved', 'REG-2024-001', '123 Green St, Eco City, EC 12345', 'Jane Smith', 'jane@greenearth.org', '+1-555-0123', 100, 75, 'Operating at 75% capacity', '["Downtown", "Westside", "North District"]', 'Environmental charity focused on sustainable practices', '2024-01-16T14:20:00Z'),
(2, 'Community Food Bank', 'foodbank', 'pending', 'REG-2024-002', '456 Food Ave, Hunger City, HC 67890', 'Bob Brown', 'bob@foodbank.org', '+1-555-0456', 200, 120, 'Expanding operations', '["Eastside", "South District"]', 'Local food bank serving the community', NULL),
(3, 'Local Restaurant Chain', 'restaurant', 'approved', 'REG-2024-003', '789 Main St, Food City, FC 54321', 'Alice Green', 'alice@restaurant.com', '+1-555-0789', 50, 30, 'Regular food donations', '["Central", "Downtown"]', 'Restaurant chain committed to reducing food waste', '2024-01-13T09:30:00Z');

-- Insert users
INSERT INTO `users` (`id`, `name`, `email`, `password_hash`, `role`, `status`, `phone`, `address`, `preferences`, `organization_id`, `last_active`) VALUES
(1, 'John Doe', 'john@example.com', '$2b$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj4J8K8K8K8K', 'individual', 'verified', '+1-555-0101', '123 Main St, City, State 12345', '{"notifications": true, "privacy": "public"}', NULL, '2024-01-20T14:22:00Z'),
(2, 'Jane Smith', 'jane@example.com', '$2b$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj4J8K8K8K8K', 'organization', 'pending', '+1-555-0202', '456 Org Ave, City, State 12345', '{"notifications": true, "privacy": "organization"}', 1, '2024-01-19T16:45:00Z'),
(3, 'Mike Johnson', 'mike@example.com', '$2b$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj4J8K8K8K8K', 'donor', 'verified', '+1-555-0303', '789 Donor St, City, State 12345', '{"notifications": true, "privacy": "public"}', NULL, '2024-01-20T11:30:00Z'),
(4, 'Sarah Wilson', 'sarah@example.com', '$2b$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj4J8K8K8K8K', 'individual', 'suspended', '+1-555-0404', '321 User Ave, City, State 12345', '{"notifications": false, "privacy": "private"}', NULL, '2024-01-18T09:20:00Z'),
(5, 'Local Bakery', 'bakery@local.com', '$2b$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj4J8K8K8K8K', 'donor', 'verified', '+1-555-0505', '555 Bakery Lane, City, State 12345', '{"notifications": true, "privacy": "public"}', NULL, '2024-01-20T08:15:00Z');

-- Update user 4 with suspension reason
UPDATE `users` SET `suspension_reason` = 'Violation of community guidelines' WHERE `id` = 4;

-- Insert donations
INSERT INTO `donations` (`id`, `title`, `description`, `type`, `status`, `quantity`, `unit`, `donor_id`, `recipient_id`, `location_address`, `location_lat`, `location_lng`, `pickup_window_start`, `pickup_window_end`, `expires_at`, `completed_at`, `tags`) VALUES
(1, 'Fresh Vegetables', 'Assorted fresh vegetables from local farm', 'food', 'active', 50, 'lbs', 3, 2, '123 Green St, Eco City, EC 12345', 40.7128, -74.0060, '2024-01-21T09:00:00Z', '2024-01-21T17:00:00Z', '2024-01-25T18:00:00Z', NULL, '["vegetables", "fresh", "organic"]'),
(2, 'Bread and Pastries', 'Day-old bread and pastries from bakery', 'food', 'completed', 25, 'items', 5, 2, '456 Food Ave, Hunger City, HC 67890', 40.7589, -73.9851, '2024-01-20T08:00:00Z', '2024-01-20T12:00:00Z', '2024-01-22T18:00:00Z', '2024-01-20T16:45:00Z', '["bread", "pastries", "bakery"]'),
(3, 'Canned Goods', 'Various canned food items', 'food', 'pending', 100, 'cans', 1, NULL, '123 Main St, City, State 12345', 40.7505, -73.9934, '2024-01-21T10:00:00Z', '2024-01-21T16:00:00Z', '2024-01-30T18:00:00Z', NULL, '["canned", "non-perishable", "shelf-stable"]');

-- Insert reports
INSERT INTO `reports` (`id`, `title`, `description`, `type`, `priority`, `status`, `reporter_id`, `reported_user_id`, `assigned_admin_id`, `resolved_at`, `resolution_notes`) VALUES
(1, 'Suspicious Activity Report', 'User reported suspicious behavior from another user during donation pickup. The user was acting aggressively and not following safety protocols.', 'safety', 'high', 'pending', 1, 4, NULL, NULL, NULL),
(2, 'Fraudulent Organization', 'Report of organization providing false information about their registration and capacity. They claim to be a registered charity but verification shows otherwise.', 'fraud', 'critical', 'investigating', 3, 6, 1, NULL, NULL),
(3, 'Inappropriate Content', 'User posted inappropriate content in the community forum. Content violates community guidelines.', 'inappropriate', 'medium', 'resolved', 2, 7, 2, '2024-01-19T09:30:00Z', 'Content removed and user warned.');

-- Insert report evidence
INSERT INTO `report_evidence` (`report_id`, `type`, `content`) VALUES
(1, 'text', 'User was shouting and being confrontational'),
(2, 'document', 'Screenshot of false registration claim'),
(3, 'text', 'Screenshot of inappropriate post');

-- Insert report notes
INSERT INTO `report_notes` (`report_id`, `author`, `text`, `type`) VALUES
(2, 'Admin User', 'Initial investigation started. Checking registration documents.', 'internal'),
(3, 'Moderator User', 'Content removed and user warned.', 'public');

-- Insert matches
INSERT INTO `matches` (`id`, `title`, `type`, `confidence`, `status`, `from_user_id`, `to_user_id`, `ai_notes`, `factors`, `expires_at`, `accepted_at`, `rejected_at`, `rejection_reason`) VALUES
(1, 'Food Donation Match', 'donation', 92, 'pending', 3, 2, 'High confidence match based on location and food preferences. Both parties have expressed interest in sustainable practices.', '[{"weight": 40, "description": "Geographic proximity (2.3 miles apart)"}, {"weight": 30, "description": "Food type compatibility (vegetables match organization needs)"}, {"weight": 22, "description": "Timing alignment (both available during pickup window)"}]', '2024-01-22T11:00:00Z', NULL, NULL, NULL),
(2, 'Volunteer Request Match', 'volunteer', 78, 'accepted', 1, 2, 'Good match for volunteer opportunity. Volunteer has relevant skills and flexible schedule.', '[{"weight": 35, "description": "Skill match (volunteer has relevant experience)"}, {"weight": 25, "description": "Availability (matches organization schedule)"}, {"weight": 18, "description": "Location preference (volunteer willing to travel)"}]', '2024-01-21T14:20:00Z', '2024-01-19T16:45:00Z', NULL, NULL),
(3, 'Equipment Donation Match', 'donation', 65, 'rejected', 4, 3, 'Moderate match. Equipment is compatible but distance and timing constraints reduce confidence.', '[{"weight": 30, "description": "Equipment compatibility (restaurant needs match donation)"}, {"weight": 20, "description": "Distance factor (15 miles apart)"}, {"weight": 15, "description": "Timing constraints (limited pickup availability)"}]', '2024-01-20T10:30:00Z', NULL, '2024-01-18T15:20:00Z', 'Distance too far for practical pickup');

-- Insert audit logs
INSERT INTO `audit_logs` (`action`, `user`, `user_id`, `target`, `target_id`, `details`, `ip_address`, `user_agent`, `timestamp`) VALUES
('user_verified', 'Admin User', 1, 'John Doe', 1, 'User account verified successfully', '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '2024-01-20T10:30:00Z'),
('organization_approved', 'Admin User', 1, 'Green Earth Foundation', 1, 'Organization approval completed', '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '2024-01-19T15:45:00Z'),
('user_suspended', 'Admin User', 1, 'Sarah Wilson', 4, 'User suspended for violation of community guidelines', '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '2024-01-18T14:20:00Z'),
('report_resolved', 'Moderator User', 2, 'Inappropriate Content Report', 3, 'Report resolved - content removed and user warned', '192.168.1.101', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36', '2024-01-19T09:30:00Z'),
('match_override', 'Admin User', 1, 'Equipment Donation Match', 3, 'Match overridden due to distance constraints', '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '2024-01-18T15:20:00Z'),
('system_settings_updated', 'Admin User', 1, 'AI Settings', NULL, 'AI confidence threshold updated to 70%', '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '2024-01-17T11:15:00Z');

-- Insert notification templates
INSERT INTO `notification_templates` (`id`, `name`, `description`, `type`, `status`, `subject`, `content`, `variables`) VALUES
(1, 'Welcome Email', 'Welcome message for new users', 'email', 'active', 'Welcome to WasteNaut!', 'Welcome to our platform! We are excited to have you join our community of sustainable resource sharing.', '["user_name", "site_name"]'),
(2, 'Verification SMS', 'SMS for account verification', 'sms', 'active', NULL, 'Your verification code is: {code}', '["code"]'),
(3, 'Match Notification', 'Notification for new matches', 'push', 'inactive', NULL, 'You have a new match! Check it out in your dashboard.', '["match_title", "user_name"]'),
(4, 'Report Status Update', 'Notification when report status changes', 'email', 'active', 'Report Status Update', 'Your report "{report_title}" status has been updated to {status}.', '["report_title", "status", "user_name"]'),
(5, 'Organization Approval', 'Notification when organization is approved', 'email', 'active', 'Organization Approved', 'Congratulations! Your organization "{org_name}" has been approved and is now active on the platform.', '["org_name", "user_name"]');

SET FOREIGN_KEY_CHECKS = 1;
