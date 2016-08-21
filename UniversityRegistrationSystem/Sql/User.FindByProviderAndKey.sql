/*
 * User.FindByProviderAndKey.sql
 */
 
SELECT
	u.user_name AS UserName, 
	u.password_hash AS PasswordHash, 
	u.security_stamp AS SecurityStamp,
	u.phone_number AS PhoneNumber, 
	u.email AS Email, 
	u.first_name AS FirstName, 
	u.last_name AS LastName, 
	u.id AS Id, 
	u.email_confirmed AS EmailConfirmed, 
	u.phone_number_confirmed AS PhoneNumberConfirmed 
from asp_net_users u 
INNER JOIN asp_net_user_logins l ON l.user_id = u.Id 
WHERE 
	l.login_provider = @loginProvider AND 
	l.provider_key = @providerKey