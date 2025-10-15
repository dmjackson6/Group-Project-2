-- WasteNaut Admin Database Schema
-- MySQL DDL for creating all required tables

-- Set character set and collation
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

SET FOREIGN_KEY_CHECKS = 1;
