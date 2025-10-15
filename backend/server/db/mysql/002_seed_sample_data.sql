-- WasteNaut Admin Sample Data
-- Seed data for development and testing

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- Insert roles
INSERT INTO `roles` (`id`, `name`, `description`, `permissions`) VALUES
(1, 'Super Admin', 'Full system access with all permissions', '["all"]'),
(2, 'Moderator', 'User and content moderation permissions', '["users", "reports", "content"]'),
(3, 'Support', 'Customer support and user assistance', '["users", "tickets"]');

-- Insert admin users
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
