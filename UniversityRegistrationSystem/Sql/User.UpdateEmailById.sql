/*
 * User.UpdateEmailById.sql
 */

UPDATE asp_net_users 
SET email = lower(@email) 
WHERE Id = @id