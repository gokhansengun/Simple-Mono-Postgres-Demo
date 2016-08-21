/*
 * User.Update.sql
 */

UPDATE asp_net_users 
SET 
	user_name = lower(@userName), 
	password_hash = @passwordHash, 
	security_stamp = @securityStamp
WHERE id = @id