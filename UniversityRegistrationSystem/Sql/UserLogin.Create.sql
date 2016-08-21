/*
 * UserLogin.Create.sql
 */

INSERT INTO asp_net_user_logins(
	user_id, 
	login_provider, 
	provider_key)
VALUES(
	@userId, 
	@loginProvider, 
	@providerKey)