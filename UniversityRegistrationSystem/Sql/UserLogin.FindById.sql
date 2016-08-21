/*
 * UserLogin.FindById.sql
 */

SELECT
	login_provider, 
	provider_key 
FROM asp_net_user_logins 
WHERE id = @id