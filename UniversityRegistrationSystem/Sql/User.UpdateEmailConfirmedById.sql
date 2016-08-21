/*
 * User.UpdateEmailConfirmedById.sql
 */

UPDATE asp_net_users 
SET email_confirmed = @emailConfirmed 
WHERE id = @id