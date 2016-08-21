/*
 * UserLogin.RemoveLoginByProviderAndKey.sql
 */

DELETE FROM asp_net_user_logins 
WHERE
	user = @userId and 
	login_provider = @loginProvider and 
	provider_key = @providerKey